using ReelCheck.Core.Configuration;
using ReelCheck.Core.DAL;
using RegexChecker;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Vrh.Logger;
using Vrh.Redis.DataPoolHandler;
using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Core
{
    /// <summary>
    /// A vezérlés fő objektuma
    /// </summary>
    public class ReelCheck : IDisposable
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="instanceConfigFile">Plugin instance szintű konfiguráció fájl</param>
        /// <param name="pluginConfigFile">Plugin típus szintá konfiguráció fájl</param>
        public ReelCheck(string instanceConfigFile, string pluginConfigFile, string stationId)
        {
            this.stationId = stationId;
            MultiLanguageManager.MultiLanguageManagerException += MultiLanguageManagerExceptionEvent;
            _pluginLevelConfig = new PluginLevelConfig(pluginConfigFile);
            _pluginLevelConfig.ConfigProcessorEvent += ConfigProcessorEvent;
            _configuration = new ReelcheckConfiguration(instanceConfigFile);
            _configuration.ConfigProcessorEvent += ConfigProcessorEvent;
            if (_pluginLevelConfig.InitializeMultilanguageDb)
            {
                MultiLanguageManager.InitializeWordCodes(typeof(TrueWordCodes), "hu-HU");
            }
            var e = _configuration.EnableIOInput;
            _instanceWriter = new InstanceWriter(_pluginLevelConfig.DataPoolName,
                                                 this.stationId,
                                                 _pluginLevelConfig.RedisConnectionString,
                                                 Serializers.XML)
            {
                StrictMode = false
            };
            var ph = new PoolHandler(_pluginLevelConfig.DataPoolName, _pluginLevelConfig.RedisConnectionString, Serializers.XML);
            DataPoolDefinition.CheckPool(ph, _instanceWriter);
            ph.Dispose();
            _barcodeMatch = new BarcodeMatch();
            _barcodeMatch.Error += BarcodeMatchErrorEvent;
            LoadBarcodeMatchDefinitions();
            _workstationData = new WorkstationData(_configuration.WorkStationType, _configuration.ProcessType, _instanceWriter, this.stationId);
            _uiMessages = new UIMessages(5, _instanceWriter);
            _reels = new ReelDataStoreHouse(_workstationData);
            CreateMainStateMachine();
            _identificationOperation = new IdentificationOperation(this, _idCameraServiceCallLocker);
            _printOperation = new PrintOperation(this, _printServiceCallLocker);
            _checkOperation = new CheckOperation(this, _checkCameraServiceCallLocker);
            InitReelStoreHouse();
            _ioOperation = new IOOperations($"{_workstationData.StationId}_{_configuration.GetCamera(CameraType.Id)}", this, _idCameraServiceCallLocker);
            _interventions = new Interventions(this);
            _mainStateMachine.FireAsync(MainTrigers.Loaded);
            _workstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.WaitLogin);
            _goldenSampleModule = new GoldenSample(this, _configuration.WorkStationType, _configuration.ProcessType, _instanceWriter);
            if (_workstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                _reels.Load(new ReelData(_instanceWriter, 1, _workstationData.UserName, _workstationData.WorkStationType, _workstationData.StationId, _workstationData.ProcessType));
            }
        }

        #region publikus funkciók

        /// <summary>
        /// Az Enable input alacsonyra vált
        /// </summary>
        public void EnableToLow()
        {
            lock (_locker)
            {
                if (WorkstationData.WorkStationType == WorkStationType.Automatic)
                {
                    if (IOOperation.StatusReset == OnOff.Off)
                    {
                        WorkstationData.WriteToCache(DataPoolDefinition.WS.MainStatus, MainStatus.Manual);
                    }
                }
                if (WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
                {
                    if (MainStateMachine.State == MainState.AtWork)
                    {
                        _identificationOperation.Reset();
                        _printOperation.Reset();
                        _checkOperation.Reset();
                        if (MainStateMachine.State == MainState.AtWork)
                        {
                            MainStateMachine.Fire(MainTrigers.Reset);
                        }
                    }
                    ResetReelsOnCustomFVS();
                    IOOperation.SetWorkPieceNok(OnOff.Off);
                    IOOperation.SetWorkPieceOk(OnOff.Off);
                    IOOperation.SetIdCameraReady(OnOff.Off);
                    IOOperation.SetLabelPrinted(OnOff.Off);
                }
            }
        }

        private void ResetReelsOnCustomFVS()
        {
            if (_reels == null)
            {
                return;
            }
            if (_workstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                var reel = _reels[ReelPosition.Check];
                if (reel != null && reel.BookingNeed)
                {
                    _checkOperation.Book(Result.Fail, reel);
                }
            }
            else
            {
                var reel = _reels[ReelPosition.Identification];
                if (reel != null && reel.BookingNeed)
                {
                    _checkOperation.Book(Result.Fail, reel);
                }
                reel = _reels[ReelPosition.Print];
                if (reel != null && reel.BookingNeed)
                {
                    _checkOperation.Book(Result.Fail, reel);
                }
                reel = _reels[ReelPosition.Check];
                if (reel != null && reel.BookingNeed)
                {
                    _checkOperation.Book(Result.Fail, reel);
                }
            }
        }

        /// <summary>
        /// Az Enable input magasba vált
        /// </summary>
        public void EnableToHigh()
        {
            if (_workstationData.OperationBlocked 
                && _workstationData.BlockReasonCode == BlockReason.ReelProcessingFailed 
                || _workstationData.BlockReasonCode == BlockReason.Manual
                || _workstationData.BlockReasonCode == BlockReason.CustomFVSMessage)
            {
                return;
            }
            if (WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                if (_mainStateMachine.State == MainState.Ready && IOOperation.HardReset != OnOff.On)
                {
                    if (_cycleEnd)
                    {
                        _cycleEnd = false;
                        WorkstationData.WriteToCache(DataPoolDefinition.WS.MainStatus, MainStatus.Auto);
                        _mainStateMachine.Fire(MainTrigers.ReceiveEnableSignal);
                    }
                }
                if (_mainStateMachine.State == MainState.AtWork || _mainStateMachine.State == MainState.NotReady)
                {
                    WorkstationData.WriteToCache(DataPoolDefinition.WS.MainStatus, MainStatus.Auto);
                }
            }
            if (WorkstationData.WorkStationType == WorkStationType.SemiAutomatic 
                && _mainStateMachine.State == MainState.Ready)
            {
                _reels.Load(new ReelData(_instanceWriter, 1, _workstationData.UserName, _workstationData.WorkStationType, _workstationData.StationId, _workstationData.ProcessType));
                _mainStateMachine.Fire(MainTrigers.ReceiveEnableSignal);
            }
        }

        /// <summary>
        /// A status reset magasba megy
        /// </summary>
        public void StatusResetOn()
        {
            _statusResetLowAsHigh = false;
            lock (_locker)
            {
                WorkstationData.WriteToCache(DataPoolDefinition.WS.MainStatus, MainStatus.Auto);
                if (WorkstationData.WorkStationType == WorkStationType.Automatic && _mainStateMachine.State == MainState.AtWork)
                {
                    _cycleEnd = true;
                    if (_identificationOperationEnd && _printOperationEnd && _checkOperationEnd)
                    {
                        _waitToAllFinishSemaphore.Reset();
                        OperationEnd(Operations.Machine);
                    }
                }
                if (WorkstationData.WorkStationType == WorkStationType.Automatic &&
                    (_mainStateMachine.State == MainState.Ready || _mainStateMachine.State == MainState.AtWork))
                {
                    IOOperation.AllStationFinished(OnOff.Off);
                    IOOperation.SetEmpty(OnOff.Off);
                    IOOperation.SetWorkPieceNok(OnOff.Off);
                    IOOperation.SetWorkPieceOk(OnOff.Off);
                }
            }
        }

        public void StatusResetOff()
        {
            if (_statusResetLowAsHigh)
            {
                StatusResetOn();
            }
        }

        /// <summary>
        /// Hardreset jel érkezik
        /// </summary>
        public void HardResetOn()
        {
            ResetReelsOnCustomFVS();
            _workstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.Reset);
            _cycleEnd = true;
            if (_mainStateMachine.State == MainState.FeedBack || _mainStateMachine.State == MainState.AtWork)
            {
                _waitToAllFinishSemaphore.WaitOne(2000);
            }
            IOOperation.SetReady(OnOff.Off);
            IOOperation.SetLabelPrinted(OnOff.Off);
            IOOperation.AllStationFinished(OnOff.Off);
            IOOperation.SetWorkPieceOk(OnOff.Off);
            IOOperation.SetWorkPieceNok(OnOff.Off);
            IOOperation.SetEmpty(OnOff.Off);
            InitReelStoreHouse();
            if (_mainStateMachine.State == MainState.AtWork)
            {
                _mainStateMachine.Fire(MainTrigers.AllReady);
            }
            _printOperation.HardReset();
        }

        /// <summary>
        /// Hardreset megszűnik
        /// </summary>
        public void HardResetOff()
        {
            _workstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode,
                string.IsNullOrEmpty(WorkstationData.UserName) ? ReelCheckMode.WaitLogin : ReelCheckMode.Normal);
            IOOperation?.SetReady(string.IsNullOrEmpty(WorkstationData.UserName) ? OnOff.Off : OnOff.On);
        }

        /// <summary>
        /// Bejelnetkezéskor (aktiválás végzett műveletek)
        /// </summary>
        public void Login()
        {
            IOOperation.SetReady(OnOff.On);
            _workstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.Normal);
            if (WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                IOOperation.SetIdCameraReady(OnOff.Off);
                IOOperation.SetIdCameraReady(OnOff.Off);
            }
            GoldenSampleModule.CheckDue();
        }

        /// <summary>
        /// Kijelentkezéskor (inaktiválás) végzett műveletek
        /// </summary>
        public void Logout()
        {
            _workstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.WaitLogin);
            //_cycleEnd = true;
        }

        /// <summary>
        /// Blokkolja a gép működését
        /// </summary>
        /// <param name="blockReason"></param>
        public void Block(BlockReason blockReason)
        {
            if (_workstationData.WorkStationType == WorkStationType.Automatic)
            {
                if ((_mainStateMachine.State == MainState.Ready 
                    || (_mainStateMachine.State == MainState.AtWork 
                            && _identificationOperationEnd && _printOperationEnd 
                            && _checkOperationEnd && !_enableCycleEnd)
                    &&
                    blockReason == BlockReason.Manual || blockReason == BlockReason.ReelProcessingFailed))
                {
                    IOOperation.SetReady(OnOff.Off);
                }
            }
            if (_workstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                if (blockReason == BlockReason.Manual || blockReason == BlockReason.ReelProcessingFailed)
                {
                    IOOperation.SetReady(OnOff.Off);
                }
            }
            _workstationData.OperationBlocked = true;
            _workstationData.BlockReasonCode = blockReason;
            _uiMessages.DropMessage(MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Blocking.Blocked)));
        }

        /// <summary>
        /// Feklszabadítja a gép blokkolását
        /// </summary>
        public void Unblock()
        {
            _workstationData.OperationBlocked = false;
            if (_workstationData.BlockReasonCode == BlockReason.Manual 
                || _workstationData.BlockReasonCode == BlockReason.ReelProcessingFailed 
                || _workstationData.BlockReasonCode == BlockReason.CustomFVSMessage)
            {
                if (!string.IsNullOrEmpty(_workstationData.UserName))
                {
                    IOOperation.SetReady(OnOff.On);
                }
                if (_workstationData.BlockReasonCode == BlockReason.CustomFVSMessage)
                {
                    _reels[ReelPosition.UnLoad].BackgroundSystemMessage = string.Empty;
                }
            }
            _workstationData.BlockReasonCode = BlockReason.NonBlocked;
        }

        public int One(int x)
        {
            return x /= x;
        }

        /// <summary>
        /// Jelzi, hogy a művelet befejeződött
        /// </summary>
        /// <param name="operation">Melyik műveletről van szó</param>
        public void OperationEnd(Operations operation)
        {
            switch (operation)
            {
                case Operations.Identification:
                    _identificationOperationEnd = true;
                    WorkstationData.WriteToCache(DataPoolDefinition.WS.IdStatus, OperationStatus.Ready);
                    break;
                case Operations.Print:
                    WorkstationData.WriteToCache(DataPoolDefinition.WS.PrintStatus, OperationStatus.Ready);
                    _printOperationEnd = true;
                    break;
                case Operations.Check:
                    WorkstationData.WriteToCache(DataPoolDefinition.WS.CheckStatus, OperationStatus.Ready);
                    _checkOperationEnd = true;
                    break;
                case Operations.Machine:
                    _enableCycleEnd = true;
                    break;
            }
            if (_identificationOperationEnd && _printOperationEnd && _checkOperationEnd && !_enableCycleEnd)
            {
                TimeCostLoging(_reels[ReelPosition.UnLoad]);
                if (WorkstationData.WorkStationType == WorkStationType.Automatic)
                {
                    IOOperation.AllStationFinished(OnOff.On);
                    if (!_reels[ReelPosition.UnLoad].GoldenSample && !WorkstationData.OperationBlocked)
                    {
                        if (_reels[ReelPosition.UnLoad].Result == Result.Fail)
                        {
                            Block(BlockReason.ReelProcessingFailed);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_reels[ReelPosition.UnLoad].BackgroundSystemMessage))
                            {
                                Block(BlockReason.CustomFVSMessage);
                            }
                        }
                    }
                    if (WorkstationData.OperationBlocked 
                        && (WorkstationData.BlockReasonCode == BlockReason.ReelProcessingFailed 
                            || WorkstationData.BlockReasonCode == BlockReason.Manual
                            || WorkstationData.BlockReasonCode == BlockReason.CustomFVSMessage))
                    {
                        IOOperation.SetReady(OnOff.Off);
                    }
                    _statusResetLowAsHigh = true;
                    _waitToAllFinishSemaphore.Set();
                }
            }
            if (_identificationOperationEnd && _printOperationEnd && _checkOperationEnd && _enableCycleEnd)
            {
                // Minden befejeződött és a gép Enable jele is lement nullába? --> kezdődhet  a várás az új Enable jelre
                if ((WorkstationData.WorkStationType == WorkStationType.Automatic && _mainStateMachine.State == MainState.AtWork) ||
                    (WorkstationData.WorkStationType == WorkStationType.SemiAutomatic && _mainStateMachine.State == MainState.FeedBack))
                {
                    _mainStateMachine.Fire(MainTrigers.AllReady);
                }
            }
            if (WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                switch (operation)
                {
                    case Operations.Identification:
                        PrintOperation.Start();
                        break;
                    case Operations.Print:
                        CheckOperation.Start();
                        break;
                    case Operations.Check:
                        if (_mainStateMachine.State == MainState.AtWork && IOOperation.Enable == OnOff.On)
                        {
                            _mainStateMachine.Fire(MainTrigers.GoFeedback);
                        }
                        break;
                    case Operations.Machine:
                        _enableCycleEnd = true;
                        break;
                }
            }
        }

        #endregion publikus funkciók

        #region szolgáltatás objektumok (minden operation-ből elérhetőek)

        /// <summary>
        /// Állomás azonosító
        /// </summary>
        internal string StationId
        {
            get { lock(_locker) { return stationId; } }
        }
        private string stationId;

        /// <summary>
        /// Munkaálomás szintű adatok
        /// </summary>
        internal WorkstationData WorkstationData
        {
            get { lock (_locker) { return _workstationData; } }
        }
        private WorkstationData _workstationData;

        /// <summary>
        /// Aranymintateszt modul
        /// </summary>
        internal GoldenSample GoldenSampleModule
        {
            get { lock (_locker) { return _goldenSampleModule; } }
        }
        private GoldenSample _goldenSampleModule;

        /// <summary>
        /// Felhasználói felületre szánt működési üzenetek 
        /// </summary>
        internal UIMessages UIMessages
        {
            get { lock (_locker) { return _uiMessages; } }
        }
        private readonly UIMessages _uiMessages;

        /// <summary>
        /// Instance szintű konfiguráció
        /// </summary>
        internal ReelcheckConfiguration Configuration
        {
            get { lock (_locker) { return _configuration; } }
        }
        private ReelcheckConfiguration _configuration;

        /// <summary>
        /// Plugin szintű konfiguráció
        /// </summary>
        internal PluginLevelConfig PluginLevelConfiguration
        {
            get { lock (_locker) { return _pluginLevelConfig; } }
        }
        private PluginLevelConfig _pluginLevelConfig;

        /// <summary>
        /// Tekercstár (a gépen lévő teketrcseket tartalmazza pozicióhelyesen)
        /// </summary>
        internal ReelDataStoreHouse Reels
        {
            get { lock (_locker) { return _reels; } }
        }
        private ReelDataStoreHouse _reels;

        /// <summary>
        /// Redis DP instancewriter
        /// </summary>
        internal InstanceWriter InstanceWriter
        {
            get { lock (_locker) { return _instanceWriter; } }
        }
        private readonly InstanceWriter _instanceWriter;

        /// <summary>
        /// Barcodematch
        /// </summary>
        internal BarcodeMatch BarcodeMatch
        {
            get { lock (_locker) { return _barcodeMatch; } }
        }
        private BarcodeMatch _barcodeMatch;

        /// <summary>
        /// HTTP cliens
        /// </summary>
        internal HttpClient HttpClient
        {
            get { lock (_locker) { return _httpClient; } }
        }
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Állapotgép a főállapothoz
        /// </summary>
        internal StateMachine<MainState, MainTrigers> MainStateMachine
        {
            get { lock (_locker) { return _mainStateMachine; } }
        }
        private StateMachine<MainState, MainTrigers> _mainStateMachine;

        #region Műveleti objektumok 

        /// <summary>
        /// IO műveletek
        /// </summary>
        internal IOOperations IOOperation
        {
            get { lock (_locker) { return _ioOperation; } }
        }
        private readonly IOOperations _ioOperation;

        /// <summary>
        /// Azonosítás művelet (ID camera read, Externalsystem azonosítás)
        /// </summary>
        internal IdentificationOperation IdentificationOperation
        {
            get { lock (_locker) { return _identificationOperation; } }
        }
        private IdentificationOperation _identificationOperation;

        /// <summary>
        /// Nyomtatás művelet
        /// </summary>
        internal PrintOperation PrintOperation
        {
            get { lock (_locker) { return _printOperation; } }
        }
        private PrintOperation _printOperation;

        /// <summary>
        /// Ellenőrzés művelet
        /// </summary>
        internal CheckOperation CheckOperation
        {
            get { lock (_locker) { return _checkOperation; } }
        }
        private CheckOperation _checkOperation;

        #endregion Műveleti objektumok

        #endregion szolgáltatás objektumok (minden operation-ből elérhetőek)

        #region Private Methods

        #region State Machine methods

        /// <summary>
        /// NotReady státuszba kerülve mindig végrehajtja
        /// </summary>
        private void OnNotReady()
        {
            //IOOperation.GetAllIOInputs();
            IOOperation.SetReady(OnOff.Off);
            IOOperation.SetEmpty(OnOff.Off);
            IOOperation.SetLabelPrinted(OnOff.Off);
            IOOperation.SetWorkPieceNok(OnOff.Off);
            IOOperation.SetWorkPieceOk(OnOff.Off);
            IOOperation.AllStationFinished(OnOff.Off);
        }

        /// <summary>
        /// Aktív, és a WSReady jel beérkezésére vár
        /// </summary>
        private void OnReady()
        {
        }

        /// <summary>
        /// Visszajelzés állapotban végzett műveletek
        /// </summary>
        private void OnFeedBack()
        {
            lock (_locker)
            {
                try
                {
                    // Unload and load new
                    if (WorkstationData.WorkStationType == WorkStationType.Automatic)
                    {
                        _reels.Load(new ReelData(_instanceWriter, 1, _workstationData.UserName, _workstationData.WorkStationType, _workstationData.StationId, _workstationData.ProcessType));
                    }
                    _identificationOperationEnd = false;
                    _printOperationEnd = false;
                    _checkOperationEnd = false;
                    _enableCycleEnd = false;
                    OnOff workPieceOk = OnOff.Off;
                    OnOff workPieceNok = OnOff.Off;
                    OnOff empty = OnOff.Off;
                    var reel = _reels[ReelPosition.UnLoad];
                    if (reel != null)
                    {
                        if (reel.Empty || reel.IdentificationResult == IdentificationResult.Empty)
                        {
                            // EMPTY
                            if (reel.IdentificationReadResult == IdentificationResult.Empty)
                            {
                                reel.Result = Result.Empty;
                            }
                            workPieceOk = OnOff.Off;
                            workPieceNok = OnOff.Off;
                            empty = OnOff.On;
                        }
                        else
                        {
                            if (reel.IdentificationReadResult == IdentificationResult.Pass &&
                                reel.IdentificationRegexCheckResult == IdentificationResult.Pass &&
                                reel.IdentificationExternalCheckResult == IdentificationResult.Pass &&
                                reel.PrintResult == PrintResult.Pass &&
                                reel.StickingResult == PrintResult.Pass &&
                                reel.CheckResult == CheckResult.Pass)
                            {
                                // OK
                                reel.Result = Result.Pass;
                                workPieceOk = OnOff.On;
                                workPieceNok = OnOff.Off;
                                empty = OnOff.Off;
                                _checkOperation.Book(Result.Pass, reel);
                            }
                            else
                            {
                                //NOK
                                reel.Result = Result.Fail;
                                workPieceOk = OnOff.Off;
                                workPieceNok = OnOff.On;
                                empty = OnOff.Off;
                                if (!reel.GoldenSample)
                                {
                                    _checkOperation.Book(Result.Fail, reel);
                                }
                            }
                            if (reel.GoldenSample)
                            {
                                reel.Result = reel.IdentificationReadResult == IdentificationResult.Pass ? Result.GsPass : Result.GsFail;
                            }
                        }
                        reel.EndTimeStamp = DateTime.Now;
                        if (Configuration.TransactionBooking.Enabled)
                        {
                            var br = Configuration.TransactionBooking.BookingRecords.FirstOrDefault(x => x.Name == BookingRecord.Empty.MyUglyName());
                            bool saveEmpty = br != null ? br.Enabled : false;
                            br = Configuration.TransactionBooking.BookingRecords.FirstOrDefault(x => x.Name == BookingRecord.NoRead.MyUglyName());
                            bool saveNoRead = br != null ? br.Enabled : false;
                            br = Configuration.TransactionBooking.BookingRecords.FirstOrDefault(x => x.Name == BookingRecord.DataError.MyUglyName());
                            bool saveDataError = br != null ? br.Enabled : false;
                            br = Configuration.TransactionBooking.BookingRecords.FirstOrDefault(x => x.Name == BookingRecord.NoAck.MyUglyName());
                            bool saveNoAck = br != null ? br.Enabled : false;
                            br = Configuration.TransactionBooking.BookingRecords.FirstOrDefault(x => x.Name == BookingRecord.Refuse.MyUglyName());
                            bool saveRefuse = br != null ? br.Enabled : false;
                            br = Configuration.TransactionBooking.BookingRecords.FirstOrDefault(x => x.Name == BookingRecord.Passed.MyUglyName());
                            bool savePassed = br != null ? br.Enabled : false;
                            bool saveNeed =
                                (reel.StartTimeStamp != DateTime.MinValue &&
                                (saveEmpty && (reel.Empty || reel.IdentificationResult == IdentificationResult.Empty)) ||
                                (saveNoRead && reel.IdentificationResult == IdentificationResult.NoRead) ||
                                (saveDataError && reel.IdentificationResult == IdentificationResult.DataErr) ||
                                (saveNoAck && reel.IdentificationResult == IdentificationResult.NoAck) ||
                                (saveRefuse && reel.IdentificationResult == IdentificationResult.Refuse) ||
                                (savePassed && reel.IdentificationResult == IdentificationResult.Pass));
                            if (saveNeed)
                            {
                                try
                                {
                                    var startDbSave = new Stopwatch();
                                    var reelEntity = reel.ReelEntity;
                                    using (var dbc = new DataBaseContext())
                                    {
                                        dbc.Reels.Add(reelEntity);
                                        dbc.SaveChanges();
                                    }
                                    _reels[ReelPosition.UnLoad].SaveDbCost = startDbSave.Elapsed;
                                }
                                catch (Exception ex)
                                {
                                    Dictionary<string, string> logData = new Dictionary<string, string>();
                                    int i = 0;
                                    if (ex is DbEntityValidationException)
                                    {
                                        foreach (var error in (ex as DbEntityValidationException).EntityValidationErrors)
                                        {
                                            foreach (var item in error.ValidationErrors)
                                            {
                                                i++;
                                                logData.Add($"Error{i}", $"{item.PropertyName}: {item.ErrorMessage}");
                                            }
                                        }

                                    }
                                    VrhLogger.Log("Feedback: database save error occured!", logData, ex, LogLevel.Error, this.GetType());
                                }
                            }
                        }
                    }
                    if (_workstationData.WorkStationType == WorkStationType.Automatic)
                    {
                        IOOperation.SetWorkPieceOk(workPieceOk);
                        IOOperation.SetWorkPieceNok(workPieceNok);
                        IOOperation.SetEmpty(empty);
                    }
                    else
                    {
                        if (_configuration.ReelProcessingFailedOperationBlockingEnabled
                            && !_workstationData.OperationBlocked && !_reels[ReelPosition.UnLoad].GoldenSample)
                        {
                            if (workPieceNok == OnOff.On)
                            {
                                Block(BlockReason.ReelProcessingFailed);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(reel.BackgroundSystemMessage))
                                {
                                    Block(BlockReason.CustomFVSMessage);
                                }
                            }
                        }
                        if (workPieceNok == OnOff.On || empty == OnOff.On)
                        {
                            IOOperation.SetWorkPieceNok(OnOff.On);
                        }
                        else
                        {
                            IOOperation.SetWorkPieceNok(OnOff.Off);
                        }
                        IOOperation.SetWorkPieceOk(workPieceOk);
                        if (_workstationData.OperationBlocked 
                            && (_workstationData.BlockReasonCode == BlockReason.Manual 
                            || _workstationData.BlockReasonCode == BlockReason.ReelProcessingFailed 
                            || _workstationData.BlockReasonCode == BlockReason.CustomFVSMessage))
                        {
                            IOOperation.SetReady(OnOff.Off);
                        }
                    }
                    if (_workstationData.WorkStationType == WorkStationType.Automatic || !_reels[ReelPosition.UnLoad].GoldenSample)
                    {
                        bool startDueStatus = _goldenSampleModule.GoldenSampleData.Due;
                        _goldenSampleModule.Cycle();
                        if (_goldenSampleModule.GoldenSampleData.Due && !startDueStatus)
                        {
                            _reels[ReelPosition.Identification].NotGoldenSampleDueYet = true;
                        }
                    }
                    VrhLogger.Log("Feedback success!",
                        new Dictionary<string, string>()
                        {
                            { "Workpiece OK I/O output", $"{workPieceOk}" },
                            { "Workpiece NOK I/O output", $"{workPieceNok}" }
                        },
                        null, LogLevel.Verbose, this.GetType());
                }
                catch (Exception ex)
                {
                    VrhLogger.Log("Feedback error occured!", null, ex, LogLevel.Error, this.GetType());
                }
            }
            if (WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                _mainStateMachine.Fire(MainTrigers.StartWorking);
            }
            else
            {
                _mainStateMachine.Fire(MainTrigers.AllReady);
            }
        }

        /// <summary>
        /// InProgress státusz --> a munkahely vezérlés dolgozni kezd
        /// </summary>
        private void StartWorking()
        {
            if (_workstationData.WorkStationType == WorkStationType.Automatic)
            {
                // automata
                // Id
                Task.Factory.StartNew(() => _identificationOperation.Start(), TaskCreationOptions.LongRunning);
                // Print
                Task.Factory.StartNew(() => _printOperation.Start(), TaskCreationOptions.LongRunning);
                // Check
                Task.Factory.StartNew(() => _checkOperation.Start(), TaskCreationOptions.LongRunning);
            }
            else
            {
                // félautomata
                _identificationOperation.Start();
            }
        }

        /// <summary>
        /// Kreálja a fő folymamt állapotgépét
        /// </summary>
        private void CreateMainStateMachine()
        {
            _mainStateMachine = new StateMachine<MainState, MainTrigers>(MainState.Starting);
            // Starting
            _mainStateMachine.Configure(MainState.Starting)
                .Permit(MainTrigers.Loaded, MainState.NotReady);
            // NotReady
            _mainStateMachine.Configure(MainState.NotReady)
                .OnEntry(OnNotReady, "Enter into not ready state (Inactivate/Logout):\\n" +
                    "- Switch READY I/O output to LOW\\n" +
                    "- Accept Activate intervention only")
                .Permit(MainTrigers.Stop, MainState.Stopping)
                .Permit(MainTrigers.Activate, MainState.Ready);
            // Ready
            _mainStateMachine.Configure(MainState.Ready)
                .OnEntry(OnReady, "Enter into Active state (Activate/Login):\\n" +
                    "- Switch READY I/O output to HIGH\\n" +
                    "- Wait for ENABLE I/O input to HIGH")
                .Permit(MainTrigers.InActivate, MainState.NotReady);
            if (WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                // ...LABELD
                _mainStateMachine.Configure(MainState.Ready)
                    .Permit(MainTrigers.ReceiveEnableSignal, MainState.FeedBack);
            }
            else
            {
                // ...LABELE
                _mainStateMachine.Configure(MainState.Ready)
                    .Permit(MainTrigers.ReceiveEnableSignal, MainState.AtWork);
            }
            // FeedBack
            _mainStateMachine.Configure(MainState.FeedBack)
                .OnEntry(OnFeedBack, "Feedback out reel status in out/load posistion:\\n" +
                    "- SET WORKPIECE OK I/O output\\n" +
                    "- SET WORKPIECE NOK I/O output\\n" +
                    "- Save Reel data into DB on out/load position & unload reel\\n" +
                    "- Load emty reel in out/load position");
            if (WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                // ...LABELD
                _mainStateMachine.Configure(MainState.FeedBack)
                    .Permit(MainTrigers.StartWorking, MainState.AtWork);
            }
            else
            {
                // ...LABELE
                _mainStateMachine.Configure(MainState.FeedBack)
                    .Permit(MainTrigers.AllReady, MainState.Ready);
            }
            // Stopping
            _mainStateMachine.Configure(MainState.Stopping)
                .Permit(MainTrigers.Stopped, MainState.Stopped);
            // AtWork
            _mainStateMachine.Configure(MainState.AtWork)
                .OnEntry(StartWorking, "Working in progress:\\n" +
                    "- Start Identification process\\n" +
                    "- Start Printing process\\n" +
                    "- Start Check process");
            if (WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                // ...LABELD
                _mainStateMachine.Configure(MainState.AtWork)
                    .Permit(MainTrigers.AllReady, MainState.Ready);
            }
            else
            {
                // ...LABELE
                _mainStateMachine.Configure(MainState.AtWork)
                    .Permit(MainTrigers.GoFeedback, MainState.FeedBack)
                    .Permit(MainTrigers.Reset, MainState.Ready)
                    .OnExit(OnAtWorkExitByReset, "Reset");

            }
            _mainStateMachine.OnTransitioned(MainStateMachineTransition);
            VrhLogger.Log($"MainStateMachine graph ({_workstationData.WorkStationType})",
            new Dictionary<string, string>()
                {
                    { "in DOT graph language", UmlDotGraph.Format(_mainStateMachine.GetInfo()) }
                },
                null, LogLevel.Information, this.GetType());
        }

        /// <summary>
        /// LABLE: AtWork Ready átmenet: Folymmat közben reset (Trigger: Reset)
        /// </summary>
        /// <param name="transition"></param>
        private void OnAtWorkExitByReset(StateMachine<MainState, MainTrigers>.Transition transition)
        {
            if (transition.Trigger == MainTrigers.Reset)
            {
                //
            }
        }

        /// <summary>
        /// Megváltozik a fő állapotgép állapota
        /// </summary>
        /// <param name="transition"></param>
        private void MainStateMachineTransition(StateMachine<MainState, MainTrigers>.Transition transition)
        {
            VrhLogger.Log("MainStateMachine state change",
                new Dictionary<string, string>()
                {
                    { "from state", transition.Source.ToString() },
                    { "to state", transition.Destination.ToString() },
                    { "by trigger", transition.Trigger.ToString() }
                },
                null, LogLevel.Debug, _mainStateMachine.GetType());
        }

        #endregion State Machine methods

        private void TimeCostLoging(ReelData reel)
        {
            if (reel == null || !_configuration.TimeLog || reel.Empty)
            {
                return;
            }
            DateTime now = DateTime.Now;
            string fileName = $"{_workstationData.WorkStationType.ToString()}_{now.Year}{now.Month}{now.Day}.csv";
            try
            {
                string logDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + System.IO.Path.DirectorySeparatorChar + "TimeCostLog";
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                fileName = logDir + System.IO.Path.DirectorySeparatorChar + fileName;
                using (FileStream fileStream = new FileStream(fileName,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.ReadWrite))
                {
                    string fieldSeparator = "\t";
                    var rr = fileStream.Length;
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        if (fileStream.Length == 0)
                        {
                            sw.WriteLine(
                                $"{"TIMESTAMP".PadRight(20, ' ')}{fieldSeparator}" +
                                $"{"INTERNAL FVS".PadRight(40, ' ')}{fieldSeparator}" +
                                //ID
                                $"ID READ         {fieldSeparator}" +
                                $"ID REGEXCHECK   {fieldSeparator}" +
                                $"ID CUSTOMFVS    {fieldSeparator}" +
                                $"ID TRIGGER      {fieldSeparator}" +
                                $"ID FULL         {fieldSeparator}" +
                                //PRINT
                                $"PRINT           {fieldSeparator}" +
                                $"INTERVENTION    {fieldSeparator}" +
                                $"WAIT STICKING   {fieldSeparator}" +
                                $"FULL PRINT COST {fieldSeparator}" +
                                //CHECK
                                $"CHECK TRIGGER   {fieldSeparator}" +
                                $"CHECK READ      {fieldSeparator}" +
                                $"CHECK DATACHECK {fieldSeparator}" +
                                $"CEHCK CUSTOMFVS {fieldSeparator}" +
                                $"CHECK FULL COST {fieldSeparator}" +
                                //DB
                                $"SAVE DB COST    "
                                );
                        }
                        sw.WriteLine(
                            $"{DateTime.Now.ToString().PadRight(20, ' ')}{fieldSeparator}" +
                            $"{reel.InternalFVS.PadRight(40, ' ')}{fieldSeparator}" +
                            // ID
                            $"{reel.IdentificationReadCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.IdentificationRegexCheckCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.IdentificationExternalSystemCheckCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.IdentificationWaitForTriggerToLowCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.IdentificationFullCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            // PRINT
                            $"{reel.PrintCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.WaitPrintInterventionCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.StickingCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.FullPrintCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            //CHECK
                            $"{reel.CheckWaitForStartTriggerCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.CheckReadCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.CheckRegexAndDataCheckCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.BookingCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            $"{reel.CheckFullCost.ToString().PadRight(16, ' ')}{fieldSeparator}" +
                            //DB
                            $"{reel.SaveDbCost.ToString().PadRight(16, ' ')}"
                            );
                    }
                }
            }
            catch (Exception)
            {

            }


        }

        /// <summary>
        /// Inicializálja (kiüríti) a tekercstárat
        /// </summary>
        private void InitReelStoreHouse()
        {
            for (int i = 1;
                i <= (_workstationData.WorkStationType == WorkStationType.Automatic ? 5 : 2);
                i++)
            {
                var r = new ReelData(_instanceWriter, 1, _workstationData.UserName, _workstationData.WorkStationType, _workstationData.StationId, _workstationData.ProcessType);
                r.SupplierLot = i.ToString();
                _reels.Load(r);
            }
        }

        /// <summary>
        /// Feltölti a konfigurációból a BarcodeMatch működési paramétereit (ID és Check kamerák álttal olvasott címkék)
        /// </summary>
        private void LoadBarcodeMatchDefinitions()
        {
            try
            {
                _barcodeMatch.LabelMessages = new List<LabelMessage>();
                foreach (var labelMessage in _configuration.LabelMessages)
                {
                    _barcodeMatch.LabelMessages.Add(new LabelMessage()
                    {
                        Labels = labelMessage.Labels,
                        ListName = labelMessage.ListName,
                        ListSeparator = labelMessage.ListSeparator.Length > 0 ? labelMessage.ListSeparator[0] : char.MinValue,
                        MessageMask = labelMessage.MessageMask,
                        Name = labelMessage.Id,
                        Source = (int)labelMessage.Source,
                    });
                }
                _barcodeMatch.Labels = new List<Label>();
                foreach (var labelDefinition in _configuration.Labels)
                {
                    var l = new Label()
                    {
                        Name = labelDefinition.Id,
                    };
                    l.Barcodes = new List<Barcode>();
                    foreach (var barcodeDefinition in labelDefinition.Barcodes)
                    {
                        var b = new Barcode()
                        {
                            Name = barcodeDefinition.Name,
                            Value = barcodeDefinition.Value,
                        };
                        l.Barcodes.Add(b);
                    }
                    l.DataElements = new List<DataElement>();
                    foreach (var dataElementDefinition in labelDefinition.DataElements)
                    {
                        var de = new DataElement()
                        {
                            Name = dataElementDefinition.Name,
                            Value = dataElementDefinition.Value,
                        };
                        l.DataElements.Add(de);
                    }
                    _barcodeMatch.Labels.Add(l);
                }
            }
            catch (Exception ex)
            {
                VrhLogger.Log("BarcodeMatch definitions Error!",
                    new Dictionary<string, string>()
                    {
                        { "Config file", _configuration.XmlFileDefinition }
                    },
                    ex, LogLevel.Fatal, this.GetType());
            }
        }

        /// <summary>
        /// A konfiguráció feldolgozó eseményeinek logolása 
        /// </summary>
        /// <param name="e"></param>
        private void ConfigProcessorEvent(Vrh.LinqXMLProcessor.Base.ConfigProcessorEventArgs e)
        {
            VrhLogger.Log($"Config processing: {e.Message}",
                    new Dictionary<string, string>()
                    {
                        { "Config file", e.ConfigFile },
                        { "Config processor", e.ConfigProcessor },
                    },
                    e.Exception, LogLevel.Warning, this.GetType()
                );
        }

        /// <summary>
        /// Multilanguage Exception-önök logolása
        /// </summary>
        /// <param name="exception"></param>
        private void MultiLanguageManagerExceptionEvent(Exception exception)
        {
            VrhLogger.Log(exception, this.GetType());
        }

        /// <summary>
        /// BarcodeMatch hibák
        /// </summary>
        /// <param name="message"></param>
        private void BarcodeMatchErrorEvent(string message)
        {
            VrhLogger.Log(message, LogLevel.Error, typeof(BarcodeMatch));
        }

        #endregion Private Methods

        /// <summary>
        /// Azonosítás művelet befelyeződött-e már?
        /// </summary>
        private bool _identificationOperationEnd = false;

        /// <summary>
        /// Nyomtatás művelet befejeződőtt-e már?
        /// </summary>
        private bool _printOperationEnd = false;

        /// <summary>
        /// Ellenőrzés művelet befejeződőtt-e már?
        /// </summary>
        private bool _checkOperationEnd = false;

        /// <summary>
        /// Jelzi, hogy a gépen a ciklus véget ért (ENABLE jel visszament alacsonyba (különben elindítanánk az új ciklust!))
        /// </summary>
        private bool _enableCycleEnd = false;

        /// <summary>
        /// ciklus vége
        /// </summary>
        private bool _cycleEnd = true;

        /// <summary>
        /// Beavatkozások
        /// </summary>
        private readonly Interventions _interventions;

        /// <summary>
        /// Szemafora a fő folamat várakoztatásához, amíg minden alfolymat befejeződik (csak automata (LABLED) munkaghely esetén)
        /// </summary>
        private ManualResetEvent _waitToAllFinishSemaphore = new ManualResetEvent(true);

        private bool _statusResetLowAsHigh = false;

        /// <summary>
        /// instance level locker
        /// </summary>
        private readonly object _locker = new object();

        private readonly object _printServiceCallLocker = new object();

        private readonly object _idCameraServiceCallLocker = new object();

        private readonly object _checkCameraServiceCallLocker = new object();

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    ResetReelsOnCustomFVS();
                    MultiLanguageManager.MultiLanguageManagerException -= MultiLanguageManagerExceptionEvent;
                    _barcodeMatch.Error -= BarcodeMatchErrorEvent;
                    _pluginLevelConfig.ConfigProcessorEvent -= ConfigProcessorEvent;
                    _configuration.ConfigProcessorEvent -= ConfigProcessorEvent;
                    _waitToAllFinishSemaphore.WaitOne(20000);
                    //if (_mainStateMachine.State == MainState.Ready)
                    //{
                    //    _mainStateMachine.Fire(MainTrigers.InActivate);
                    //    _mainStateMachine.Fire(MainTrigers.Stop);
                    //}
                    //if (_mainStateMachine.State == MainState.NotReady)
                    //{
                    //    _mainStateMachine.Fire(MainTrigers.Stop);
                    //}
                    _ioOperation?.Dispose();
                    _configuration?.Dispose();
                    _pluginLevelConfig?.Dispose();
                    _instanceWriter?.Dispose();
                    _uiMessages?.Dispose();
                    _interventions?.Dispose();
                    _identificationOperation?.Dispose();
                    _printOperation?.Dispose();
                    _checkOperation?.Dispose();
                    _reels?.Dispose();
                    _httpClient?.Dispose();
                    //_mainStateMachine.Fire(MainTrigers.Stopped);
                }
                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                _disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ReelCheck() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
