using RegexChecker;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vrh.CameraService.EventHubContract;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using Vrh.Logger;
using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Core
{
    /// <summary>
    /// ELlenőrzés művelet
    ///  sOlid: Ez az osztály a kinyomtatott címke visszaellenőrzési műveletének futásáért felelős
    /// </summary>
    internal class CheckOperation : OperationWithCameraService
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="main">Referencia a main objektumra</param>
        public CheckOperation(ReelCheck main, object checkCameraServiceCallLocker)
            : base(checkCameraServiceCallLocker)
        {
            _main = main;
            _camera = $"{main.WorkstationData.StationId}_{main.Configuration.GetCamera(CameraType.Check)}";
            _cameraEventHubChannel = _camera;
            CreateStateMachine();
            EventHubCore.InitielizeChannel<RedisPubSubChannel>(_cameraEventHubChannel);
        }

        #region Public methods (kivülről is hozzáférhető műveletek)

        /// <summary>
        /// Ellenőrzés művelet indítása
        /// </summary>
        public void Start()
        {
            lock (_locker)
            {
                _checkStateMachine.Fire(CheckTrigger.Start);
            }
        }

        /// <summary>
        /// A check camera engedélyezett jel magasra vált
        /// </summary>
        public void CheckCameraEnableToHigh()
        {
            lock (_locker)
            {
                if (_checkStateMachine.State == CheckState.WaitForEnable)
                {
                    if ((_reel.IdentificationResult != IdentificationResult.Pass || _reel.PrintResult != PrintResult.Pass) && _main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
                    {
                        _checkStateMachine.Fire(CheckTrigger.NotNeed);
                    }
                    else
                    {
                        _checkStateMachine.Fire(CheckTrigger.StartRead);
                    }
                }
            }

        }

        /// <summary>
        /// Folyamat reset
        /// </summary>
        public void Reset()
        {
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                if (_checkStateMachine.State == CheckState.WaitForEnable)
                {
                    _checkStateMachine.Fire(CheckTrigger.EndCheck);
                }
            }
        }

        /// <summary>
        /// A check camera engedélyezett jel alacsonyra vált
        /// </summary>
        public void CheckCameraEnableToLow()
        {
            // do nothing
        }

        /// <summary>
        /// Könyveli CustomFVS-be a tekercs bevét eredményét PASS/FAIL
        /// </summary>
        /// <param name="result">Eredmény PASS/FAIL</param>
        /// <param name="reel">A tekercs, amire a könyvelés történjen, ha null, akkor a check pózición lévő tekercs</param>
        public async void Book(Result result, ReelData reel)
        {
            try
            {
                _startBooking.Restart();
                if (reel.GoldenSample || String.IsNullOrEmpty(reel.InternalFVS))
                {
                    return;
                }
                reel.CheckExternalBookingResult = ExternalBookingResult.InProgress;
                Dictionary<string, string> logData = new Dictionary<string, string>();
                string outgoingKey = result == Result.Pass ? "BOOKOK" : "BOOKNOK";
                var outgoingMessage = _main.Configuration.GetOutgoingMessage(outgoingKey);
                if (outgoingMessage != null)
                {
                    string contentText = reel.AllIdentificationData.ConstructString(outgoingMessage.Value);
                    string uri = _main.PluginLevelConfiguration.CustomFVSUri;
                    logData.Add("outgoingMessage", outgoingMessage.Value);
                    logData.Add("uri", uri);
                    logData.Add("content", contentText);
                    VrhLogger.Log("Get BOOK OK/NOK OutgoingMessage is successful!",
                            logData,
                            null,
                            LogLevel.Verbose,
                            this.GetType()
                        );
                    var content = new StringContent(contentText, Encoding.UTF8, "text/plain");
                    HttpResponseMessage httpResponse = null;
                    string httpResponseString = null;
                    try
                    {
                        httpResponse = await _main.HttpClient.PostAsync(uri, content);
                        httpResponseString = await httpResponse.Content.ReadAsStringAsync();
                        logData.Add("response", httpResponseString);
                        reel.CheckExternalBookingResult = result == Result.Pass //httpResponseString.Contains(Result.Pass.ToString().ToUpper())
                                                            ? ExternalBookingResult.PassBookOK
                                                            : ExternalBookingResult.PassBookNOK;
                    }
                    catch (Exception ex)
                    {
                        reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.CustomFVSNoAck))} ({ex.Message})";
                        reel.CheckResult = CheckResult.NOK;
                        reel.CheckExternalBookingResult = ExternalBookingResult.NoAck;
                        VrhLogger.Log("CustomFVS communication error in BOOK OK/NOK!",
                                logData,
                                ex,
                                LogLevel.Error,
                                this.GetType()
                            );
                    }
                    VrhLogger.Log("CustomFVS communication success in BOOK OK / NOK!",
                            logData,
                            null,
                            LogLevel.Verbose,
                            this.GetType()
                        );
                    reel.BookingNeed = false;
                }
                else
                {
                    reel.CheckResult = CheckResult.DataErr;
                    reel.CheckExternalBookingResult = ExternalBookingResult.DataErr;
                    reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.OutgoingSectionError))} ({outgoingKey}: {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))} OutgoingMessages {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))}";
                    VrhLogger.Log("BOOK OK/NOK Outgoing message NOT found or epty! Check the OutgoingMessages configuration section!",
                            _logData,
                            null,
                            LogLevel.Error,
                            this.GetType()
                        );
                }
            }
            finally
            {
                reel.BookingCost = _startBooking.Elapsed;
            }
        }

        #endregion Public methods (kivülről is hozzáférhető műveletek)

        #region CheckStateMachine

        /// <summary>
        /// Művelet indítása
        /// </summary>
        private void OnStart()
        {
            _startOperation.Restart();
            _startWaitForTrigger.Restart();
            _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.CheckStatus, OperationStatus.Operating);
            if (_reel.IdentificationResult != IdentificationResult.Pass || _reel.PrintResult != PrintResult.Pass || _reel.GoldenSample)
            {
                if (_reel.IdentificationReadResult == IdentificationResult.Empty)
                {
                    _reel.CheckReadResult = CheckResult.Empty;
                    _reel.CheckRegexCheckResult = CheckResult.Empty;
                    _reel.CheckExternalBookingResult = ExternalBookingResult.Empty;
                    _reel.CheckResult = CheckResult.Empty;
                }
                else
                {
                    _reel.CheckReadResult = CheckResult.None;
                    _reel.CheckRegexCheckResult = CheckResult.None;
                    _reel.CheckExternalBookingResult = ExternalBookingResult.None;
                    _reel.CheckResult = CheckResult.None;
                }
                _checkStateMachine.Fire(CheckTrigger.NotNeed);
                return;
            }
            if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic || _main.IOOperation.CheckCameraEnable == OnOff.On)
            {
                _checkStateMachine.Fire(CheckTrigger.StartRead);
            }
            else
            {
                _checkStateMachine.Fire(CheckTrigger.StartWaitForEnable);
            }
        }

        /// <summary>
        /// Címke olvasása
        /// </summary>
        private void OnRead()
        {
            _reel.CheckWaitForStartTriggerCost = _startWaitForTrigger.Elapsed;
            _startRead.Restart();
            try
            {
                _label = null;
                _reel.CheckResult = CheckResult.InProgress;
                _reel.CheckReadResult = CheckResult.InProgress;
                _reel.AllCheckData.Clear();
                _logData = new Dictionary<string, string>()
                {
                    { "Start", DateTime.Now.ToString() },
                    { "Used configuration", _main.Configuration.XmlFileDefinition },
                    { "Check Camera", _camera },
                };
                try
                {
                    LabelDataMessageResult readData;
                    lock (_sharedEventHubCallLocker)
                    {
                        readData = EventHubCore.Call<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>(_cameraEventHubChannel, new ReadMessage(), new TimeSpan(0, 0, 10));
                    }
                    _reel.CheckReadsData = readData.LabelData;
                    _reel.CheckReadAttempts += 1;
                    _reel.CheckCamera = _camera;
                    _logData.Add("Check camera readed data", readData.LabelData);
                    VrhLogger.Log("Check camera Read OK", _logData, null, LogLevel.Information, this.GetType());
                }
                catch (Exception ex)
                {
                    _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.CheckReadError))} ({ex.Message})";
                    VrhLogger.Log("Read error occured!", _logData, ex, LogLevel.Error, this.GetType());
                }
            }
            finally
            {
                _reel.CheckReadCost = _startRead.Elapsed;
            }
            // --> BarcodeMatch
            _checkStateMachine.Fire(CheckTrigger.StartBarcodeMatch);
        }

        /// <summary>
        /// Címke azonosítása, adattartalom kinyerése
        /// </summary>
        private void OnBarCodeMatch()
        {
            _startRegexAndDataCheck.Restart();
            _reel.CheckRegexCheckResult = CheckResult.InProgress;
            LabelMessage labelMessage;

            var labelMessageDefinationForCheckCamera = _main.Configuration.LabelMessages.FirstOrDefault(x => x.Source == CameraType.Check && x.Id.ToUpper() == "DELPHICHECK");
            if (labelMessageDefinationForCheckCamera != null)
            {
                string checkLabelId = labelMessageDefinationForCheckCamera.Id;
                var label = _main.BarcodeMatch.Labels.FirstOrDefault(x => x.Name == checkLabelId);
                if (label != null)
                {
                    var labelDefinitionForCheckCamera = _main.Configuration.Labels.FirstOrDefault(x => x.Id == checkLabelId);
                    if (labelDefinitionForCheckCamera != null)
                    {
                        foreach (var barcodeDefinitionForCheckCamera in labelDefinitionForCheckCamera.Barcodes)
                        {
                            var barcode = label.Barcodes.FirstOrDefault(x => x.Name == barcodeDefinitionForCheckCamera.Name);
                            if (barcode != null)
                            {
                                barcode.Value = _reel.AllIdentificationData.ConstructString(barcodeDefinitionForCheckCamera.Value);
                            }
                        }
                    }
                }
            }                        
            var labels = _main.BarcodeMatch.Match(_reel.CheckReadsData, (int)CameraType.Check, out labelMessage);
            _label = labels.FirstOrDefault();

            if (labelMessage != null)
            {
                _reel.AllCheckData.SafeAdd(new KeyAndValue() { Key = "LabelId", Value = labelMessage.Name });
                _logData.Add("Readed label id", labelMessage.Name);
                if (labelMessage.Name.ToUpper().Contains(SpecialLabelType.Empty.ToString().ToUpper()))
                {
                    // EMPTY
                    _reel.CheckResult = CheckResult.Empty;
                    _reel.CheckReadResult = CheckResult.Empty;
                    _reel.CheckRegexCheckResult = CheckResult.Empty;
                    _reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.EptyReelPosition));
                    VrhLogger.Log("Check operation waited for reel, but Empty label readed!",
                            _logData,
                            null,
                            LogLevel.Error,
                            this.GetType()
                        );
                    // --> CloseOperation state
                    _checkStateMachine.Fire(CheckTrigger.EmptyRead);
                }
                else
                {
                    // NOREAD
                    if (labelMessage.Name.ToUpper().Contains(SpecialLabelType.NoRead.ToString().ToUpper()))
                    {
                        _reel.CheckResult = CheckResult.NoRead;
                        _reel.CheckReadResult = CheckResult.NoRead;
                        _reel.CheckRegexCheckResult = CheckResult.NoRead;
                        _reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.NoRead));
                        VrhLogger.Log("Check camera NoRead, probably the printed label id unreadable!",
                                _logData,
                                null,
                                LogLevel.Error,
                                this.GetType()
                            );
                        // --> CloseOperation state
                        _checkStateMachine.Fire(CheckTrigger.NoRead);
                    }
                    else
                    {
                        if (_label != null)
                        {
                            _reel.CheckReadResult = CheckResult.Pass;
                            _reel.CheckRegexCheckResult = CheckResult.Pass;
                            _reel.CheckReadsData = labelMessage.ListValue;
                            _reel.AllCheckData.SafeAdd(new KeyAndValue() { Key = "BCLISTCHECK", Value = labelMessage.ListValue });
                            _logData.Add("BCLISTCHECK", labelMessage.ListValue);
                            foreach (var item in _label.DataElements)
                            {
                                _reel.AllCheckData.SafeAdd(new KeyAndValue() { Key = item.Name, Value = item.Value });
                                _logData.Add($"DataElement:{item.Name}", item.Value);
                                switch (item.Name)
                                {
                                    case "CMTSID":
                                        _reel.CheckMTSId = item.Value;
                                        break;
                                    case "CFVS":
                                        _reel.CheckFVS = item.Value;
                                        break;
                                }
                            }
                            VrhLogger.Log("Known label readed!",
                                    _logData,
                                    null,
                                    LogLevel.Verbose,
                                    this.GetType()
                                );
                        }
                        else
                        {
                            // No LABEL from RegexChecker!!!
                            _reel.CheckResult = CheckResult.DataErr;
                            _reel.CheckReadResult = CheckResult.Pass;
                            _reel.CheckRegexCheckResult = CheckResult.DataErr;
                            _logData.Add("LabelMessage id", labelMessage.Name);
                            _logData.Add("Readed bclist", labelMessage.ListValue);
                            _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.LabelsSectionError))} ({labelMessage.Name.ToUpper()} {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))}: Labels {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))})";
                            VrhLogger.Log("Message defined, but not label found! Configuration Error! Check the LabelMessages and Labels configuration sections!",
                                    _logData,
                                    null,
                                    LogLevel.Error,
                                    this.GetType()
                               );
                        }
                        _checkStateMachine.Fire(CheckTrigger.StartCheck);
                    }
                }
            }
            else
            {
                _reel.CheckResult = CheckResult.DataErr;
                _reel.CheckReadResult = CheckResult.Pass;
                _reel.CheckRegexCheckResult = CheckResult.DataErr;
                _reel.LabelId = SpecialLabelType.Unknown.ToString().ToUpper();
                _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.LabelMessagesSectionError))} ({MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))}: LabelMessages {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))})";
                VrhLogger.Log("Unknown Label! Check the LabelMessages configuration section!",
                        _logData,
                        null,
                        LogLevel.Error,
                        this.GetType()
                    );
                // --> CloseOperation state
                _checkStateMachine.Fire(CheckTrigger.NoMatch);
            }
        }

        /// <summary>
        /// Címkén lévő adatok ellenőrzése
        /// </summary>
        private void OnCheck()
        {
            var definedCheck = _main.Configuration.CheckSuccess;
            CheckResult checkResult = Vrh.CheckSuccess.CheckSuccess.DoCheck(definedCheck,
                _reel.AllIdentificationData.ToDictionary<KeyAndValue, string, string>(x => x.Key, x => x.Value),
                _reel.AllCheckData.ToDictionary<KeyAndValue, string, string>(x => x.Key, x => x.Value),
                Vrh.CheckSuccess.CheckSuccess.CheckOperations.AND) ? CheckResult.Pass : CheckResult.NOK;
            if (checkResult == CheckResult.Pass)

            {
                _reel.CheckResult = CheckResult.Pass;
            }
            else
            {
                _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.CheckOperation.CheckSuccessError))} ({MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))}: CheckSuccess {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))})";
                _reel.CheckResult = CheckResult.DataErr;
                _reel.CheckRegexCheckResult = CheckResult.DataErr;
            }
            VrhLogger.Log($"Check with this defined Check: {definedCheck}", LogLevel.Information, this.GetType());
            VrhLogger.Log("Check with this TestData", _reel.AllIdentificationData.ToDictionary<KeyAndValue, string, string>(x => x.Key, x => x.Value), null, LogLevel.Information, this.GetType());
            VrhLogger.Log("Check with this WithData", _reel.AllCheckData.ToDictionary<KeyAndValue, string, string>(x => x.Key, x => x.Value), null, LogLevel.Information, this.GetType());
            VrhLogger.Log($"Check with this Result: {checkResult}", LogLevel.Information, this.GetType());
            _checkStateMachine.Fire(CheckTrigger.EndCheck);
        }

        /// <summary>
        /// Művelet zárása
        /// </summary>
        private void OnCloseOperation()
        {
            _reel.CheckRegexAndDataCheckCost = _startRegexAndDataCheck.Elapsed;
            _reel.CheckFullCost = _startOperation.Elapsed;
            _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.CheckStatus, OperationStatus.Ready);
            _checkStateMachine.Fire(CheckTrigger.End);
            _main.OperationEnd(Operations.Check);
        }

        /// <summary>
        /// Creálja az állapotgépet
        /// </summary>
        private void CreateStateMachine()
        {
            _checkStateMachine = new StateMachine<CheckState, CheckTrigger>(CheckState.Rest);
            _checkStateMachine.Configure(CheckState.Rest)
                    .Permit(CheckTrigger.Start, CheckState.StartCheck);
            _checkStateMachine.Configure(CheckState.StartCheck)
                .OnEntry(OnStart, "Start operation:\\n" +
                    "- Check need or not\\n" +
                    "- if need: Fire ReadStart\\n" +
                    "- if not need: Fire NotNeed\\n")
                .Permit(CheckTrigger.NotNeed, CheckState.CloseOperation)
                .Permit(CheckTrigger.StartRead, CheckState.Read);
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                _checkStateMachine.Configure(CheckState.StartCheck)
                    .Permit(CheckTrigger.StartWaitForEnable, CheckState.WaitForEnable);
                _checkStateMachine.Configure(CheckState.WaitForEnable)
                    .Permit(CheckTrigger.StartRead, CheckState.Read)
                    .Permit(CheckTrigger.NotNeed, CheckState.CloseOperation)
                    .Permit(CheckTrigger.EndCheck, CheckState.Rest);
            }
            _checkStateMachine.Configure(CheckState.Read)
                .OnEntry(OnRead, "Read label:\\n" +
                    "- Read the label\\n" +
                    "- Save readed data" +
                    "- Fire: BarcodMatch")
                .Permit(CheckTrigger.StartBarcodeMatch, CheckState.BarcodeMatch);
            _checkStateMachine.Configure(CheckState.BarcodeMatch)
                .OnEntry(OnBarCodeMatch, "BarcodeMatch:\\n" +
                    "- Identification the label\\n" +
                    "- Extract label data\\n" +
                    "Fire next state (StartCheck or EmptyRead or NotRead)")
                .Permit(CheckTrigger.StartCheck, CheckState.Check)
                .Permit(CheckTrigger.EmptyRead, CheckState.CloseOperation)
                .Permit(CheckTrigger.NoRead, CheckState.CloseOperation)
                .Permit(CheckTrigger.NoMatch, CheckState.CloseOperation);
            _checkStateMachine.Configure(CheckState.Check)
                .OnEntry(OnCheck, "Check the label data:\\n" +
                    "- Save check status\\n" +
                    "- Fire: EndCheck")
                .Permit(CheckTrigger.EndCheck, CheckState.CloseOperation);
            _checkStateMachine.Configure(CheckState.CloseOperation)
                .OnEntry(OnCloseOperation, "Close operation:\\n" +
                    "- data into reel object\\n" +
                    "- Fire: End")
                .Permit(CheckTrigger.End, CheckState.Rest);
            _checkStateMachine.OnTransitioned(CheckStateMachineTransition);
            VrhLogger.Log($"CheckStateMachine graph ({_main.WorkstationData.WorkStationType})",
            new Dictionary<string, string>()
                {
                    { "in DOT graph language", UmlDotGraph.Format(_checkStateMachine.GetInfo()) }
                },
                null, LogLevel.Information, this.GetType());
        }

        /// <summary>
        /// Nyomtatás művelet állapotot vált
        /// </summary>
        /// <param name="transition">Állapot átmenet</param>
        private void CheckStateMachineTransition(StateMachine<CheckState, CheckTrigger>.Transition transition)
        {
            VrhLogger.Log("CheckStateMachine state change",
                new Dictionary<string, string>()
                {
                    { "from state", transition.Source.ToString() },
                    { "to state", transition.Destination.ToString() },
                    { "by trigger", transition.Trigger.ToString() }
                },
                null, LogLevel.Debug, _checkStateMachine.GetType());
        }

        #endregion CheckStateMachine

        /// <summary>
        /// A pozición lévő tekercs
        /// </summary>
        private ReelData _reel
        {
            get
            {
                return _main.Reels[ReelPosition.Check];
            }
        }

        /// <summary>
        /// Utoljára olvasott címke
        /// </summary>
        private Label _label = null;

        /// <summary>
        /// lognak szánt adatok
        /// </summary>
        private Dictionary<string, string> _logData;

        /// <summary>
        /// Print folyamot állapotgépe
        /// </summary>
        private StateMachine<CheckState, CheckTrigger> _checkStateMachine;

        private Stopwatch _startOperation = new Stopwatch();
        private Stopwatch _startWaitForTrigger = new Stopwatch();
        private Stopwatch _startRead = new Stopwatch();
        private Stopwatch _startRegexAndDataCheck = new Stopwatch();
        private Stopwatch _startSaveDb = new Stopwatch();
        private Stopwatch _startBooking = new Stopwatch();
    }
}
