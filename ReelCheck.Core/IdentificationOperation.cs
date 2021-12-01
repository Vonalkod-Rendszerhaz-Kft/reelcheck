using RegexChecker;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using Vrh.CameraService.EventHubContract;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using Vrh.Logger;
using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Core
{
    /// <summary>
    /// Tekercs azonosítása (auzomata folymamt esetén)
    /// </summary>
    internal class IdentificationOperation : OperationWithCameraService
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="main">referencia a fő objektumra (ReelCheck)</param>
        public IdentificationOperation(ReelCheck main, object idCameraServiceCallLocker)
            : base(idCameraServiceCallLocker)
        {
            _camera = $"{main.WorkstationData.StationId}_{main.Configuration.GetCamera(CameraType.Id)}";
            _cameraEventHubChannel = _camera;
            _main = main;
            CreateStateMachine();
            EventHubCore.InitielizeChannel<RedisPubSubChannel>(_cameraEventHubChannel);
        }

        #region Public methods (kivülről is hozzáférhető műveletek)

        /// <summary>
        /// Azonosítás folyamat indítása
        /// </summary>
        public void Start()
        {
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                _main.IOOperation.SetIdCameraReady(OnOff.Off);
                _idStateMachine.Fire(IdentificationTriggers.Start);
            }
            else
            {
                _idStateMachine.Fire(IdentificationTriggers.StartRead);
            }
        }

        /// <summary>
        /// IDCAMENABLE I/O bemenet LOW-ra vált
        /// </summary>
        public void IdCamEnableToLow()
        {
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                _reel.IdentificationWaitForTriggerToLowCost = _startWaitForTriggerToLow.Elapsed;
                _main.IOOperation?.SetIdCameraReady(OnOff.Off);
                if (_idStateMachine.State == IdentificationState.WaitForIdCameraEnableToLow)
                {
                    _idStateMachine.Fire(IdentificationTriggers.IdCamEnableChangedToLow);
                }
                _idCameraEnabled = false;
            }
        }

        /// <summary>
        /// IDCAMENABLE I/O bemenet HIGH-ra vált
        /// </summary>
        public void IdCamEnableToHigh()
        {
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                _idCameraEnabled = true;
                if (_idStateMachine.State == IdentificationState.WaitForEnable)
                {
                    _idStateMachine.Fire(IdentificationTriggers.StartRead);
                }
            }
        }

        /// <summary>
        /// Reset Folyamat az elejére ugrik (csak LabelE)
        /// </summary>
        public void Reset()
        {
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                if (_idStateMachine.State == IdentificationState.WaitForEnable)
                {
                    _idStateMachine.Fire(IdentificationTriggers.End);
                }
            }
        }

        #endregion Public methods (kivülről is hozzáférhető műveletek)

        /// <summary>
        /// Rest --> Start: indul a folyamat
        /// </summary>
        private void OnStart()
        {
            _startOperation.Restart();
            _reel.IdentificationResult = IdentificationResult.InProgress;
            _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.IdStatus, OperationStatus.Operating);
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                if (_main.IOOperation.IdCameraEnable == OnOff.On)
                {
                    _idStateMachine.Fire(IdentificationTriggers.StartRead);
                }
            }
        }

        /// <summary>
        /// Olvasás az ID kamerával
        /// </summary>
        private void OnRead()
        {
            lock (_locker)
            {
                _startRead.Restart();
                _reel.IdentificationReadResult = IdentificationResult.InProgress;
                _label = null;
                _reel.AllIdentificationData.Clear();
                _reel.StartTimeStamp = DateTime.Now;
                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "CYCLESTARTTIMESTAMP", Value = _reel.StartTimeStamp.ToString("yyyyMMddHHmmssfff") });
                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "PROCESS", Value = _main.WorkstationData.ProcessType.ToString().ToUpper() });
                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "STATION", Value = _main.WorkstationData.StationId.ToUpper() });
                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "USERNAME", Value = _main.WorkstationData.UserName });
                _logData = new Dictionary<string, string>()
                {
                    { "Start", DateTime.Now.ToString() },
                    { "Used configuration", _main.Configuration.XmlFileDefinition },
                    { "ID Camera", _camera },
                };
                try
                {
                    LabelDataMessageResult readData = null;
                    lock (_sharedEventHubCallLocker)
                    {
                        readData = EventHubCore.Call<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>(_cameraEventHubChannel, new ReadMessage(), new TimeSpan(0, 0, 10));
                    }
                    if (!string.IsNullOrEmpty(_main.WorkstationData.ManualData) && !readData.LabelData.Contains("EMPTY"))
                    {
                        //Kézi
                        readData = new LabelDataMessageResult(_main.WorkstationData.ManualData);
                        _main.WorkstationData.ManualData = String.Empty;
                        _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.Normal);
                    }
                    _reel.IdentificationReadsData = readData.LabelData;
                    _reel.IdentificationReadAttempts += 1;
                    _reel.IdCamera = _camera;
                    _logData.Add("Id Camera data", readData.LabelData);
                    // GoldenSample
                    _main.GoldenSampleModule.GoldenSampleLabelRead(readData.LabelData, _reel);
                    // Blokkolt
                    if (_main.WorkstationData.OperationBlocked 
                        && !_main.GoldenSampleModule.GoldenSampleData.Active && !_reel.GoldenSample)
                    {
                        string errorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Blocking.BlocReason))}: ";
                        switch (_main.WorkstationData.BlockReasonCode)
                        {
                            case BlockReason.Manual:
                                errorMessage += MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Blocking.Reasons.Manual));
                                break;
                            case BlockReason.ReelProcessingFailed:
                                errorMessage += MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Blocking.Reasons.ReelProcessingFailed));
                                break;
                            case BlockReason.GoldenSampleTestFailed:
                                errorMessage += MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Blocking.Reasons.GoldenSampleTestFailed));
                                break;
                            case BlockReason.GoldenSampleTestDue:
                                errorMessage += MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Blocking.Reasons.GoldenSampleTestDue));
                                break;
                            default:
                                break;
                        }
                        if (!_reel.NotGoldenSampleDueYet)
                        {
                            _reel.ErrorMessage = errorMessage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    VrhLogger.Log("Read error occured!", _logData, ex, LogLevel.Error, GetType());
                    _reel.ErrorMessage = ex.Message;
                }
                VrhLogger.Log("ID camera Read OK", _logData, null, LogLevel.Information, GetType());
            }
            _reel.IdentificationReadCost = _startRead.Elapsed;
            // --> BarcodeMatch
            _idStateMachine.Fire(IdentificationTriggers.ReadEnd);
        }

        /// <summary>
        /// Olvasott adat (címke) felismerése
        /// </summary>
        private void OnMatch()
        {
            lock (_locker)
            {
                if (_reel.GoldenSample)
                {
                    if (_reel.IdentificationReadResult == IdentificationResult.InProgress)
                    {
                        _reel.IdentificationReadResult = IdentificationResult.Pass;
                    }
                    _reel.IdentificationRegexCheckResult = IdentificationResult.None;
                    _reel.IdentificationExternalCheckResult = IdentificationResult.None;
                    _idStateMachine.Fire(IdentificationTriggers.GoldenSample);
                    return;
                }
                _startRegexCheck.Restart();
                _reel.IdentificationRegexCheckResult = IdentificationResult.InProgress;
                var labels = _main.BarcodeMatch.Match(_reel.IdentificationReadsData, (int)CameraType.Id, out LabelMessage labelMessage);
                 _label = labels.FirstOrDefault();
                if (labelMessage != null)
                {
                    _reel.LabelId = labelMessage.Name;
                    if (labelMessage.Name.ToUpper().Contains(SpecialLabelType.Empty.ToString().ToUpper()))
                    {
                        // EMPTY
                        _reel.Empty = true;
                        _reel.IdentificationReadResult = IdentificationResult.Empty;
                        _reel.IdentificationResult = IdentificationResult.Empty;
                        _reel.IdentificationRegexCheckResult = IdentificationResult.Empty;
                        _reel.IdentificationExternalCheckResult = IdentificationResult.Empty;
                        _reel.LabelId = SpecialLabelType.Empty.ToString().ToUpper();
                        _reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.EptyReelPosition));
                        _logData.Add("Readed label id", _reel.LabelId);
                        VrhLogger.Log("Empty label readed!",
                                _logData,
                                null,
                                LogLevel.Information,
                                GetType()
                            );
                        _reel.IdentificationRegexCheckCost = _startRegexCheck.Elapsed;
                        // --> CloseOperation state
                        _idStateMachine.Fire(IdentificationTriggers.EmptyRead);
                    }
                    else
                    {
                        if (_main.WorkstationData.OperationBlocked && !_reel.NotGoldenSampleDueYet)
                        {
                            _reel.IdentificationReadResult = IdentificationResult.NoRead;
                            _reel.IdentificationRegexCheckResult = IdentificationResult.None;
                            _reel.Empty = false;
                            _idStateMachine.Fire(IdentificationTriggers.NoRead);
                            return;
                        }
                        // NOREAD
                        _reel.Empty = false;
                        if (labelMessage.Name.ToUpper().Contains(SpecialLabelType.NoRead.ToString().ToUpper()))
                        {
                            _reel.LabelId = SpecialLabelType.NoRead.ToString().ToUpper();
                            _reel.IdentificationReadResult = IdentificationResult.NoRead;
                            _reel.IdentificationResult = IdentificationResult.NoRead;
                            _reel.IdentificationRegexCheckResult = IdentificationResult.None;
                            _reel.IdentificationExternalCheckResult = IdentificationResult.None;
                            _reel.IdentificationResult = IdentificationResult.NoRead;
                            _reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.NoRead));
                            _logData.Add("Readed label id", _reel.LabelId);
                            VrhLogger.Log("Id Camera NoRead!",
                                    _logData,
                                    null,
                                    LogLevel.Warning,
                                    this.GetType()
                                );
                            _reel.IdentificationRegexCheckCost = _startRegexCheck.Elapsed;
                            // --> CloseOperation state
                            _idStateMachine.Fire(IdentificationTriggers.NoRead);
                        }
                        else
                        {
                            // --> IdData state
                            if (_label != null)
                            {
                                _reel.IdentificationReadResult = IdentificationResult.Pass;
                                _reel.IdentificationRegexCheckResult = IdentificationResult.Pass;
                                _reel.LabelId = _label.Name;
                                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "LABELID", Value = _reel.LabelId });
                                _reel.BCList = labelMessage.ListValue;
                                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "BCLIST", Value = _reel.BCList });
                                _logData.Add("Readed label id", _reel.LabelId);
                                _logData.Add("BCList", _reel.BCList);
                                foreach (var item in _label.DataElements)
                                {
                                    _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = item.Name, Value = item.Value.Trim() });
                                    _logData.Add($"DataElement:{item.Name}", item.Value);
                                    switch (item.Name.ToUpper())
                                    {
                                        case "SFVS":
                                            _reel.SupplierFVS = item.Value;
                                            break;
                                        case "SPN":
                                            _reel.SupplierPartNumber = item.Value;
                                            break;
                                        case "SVENDOR":
                                            _reel.Vendor = item.Value;
                                            break;
                                        case "SLOT":
                                            _reel.SupplierLot = item.Value;
                                            break;
                                        case "SRSN":
                                            _reel.SupplierReelSerialNumber = item.Value;
                                            break;
                                        case "SQTY":
                                            _reel.SupplierQty = item.Value;
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
                                _reel.IdentificationReadResult = IdentificationResult.Pass;
                                _reel.IdentificationResult = IdentificationResult.DataErr;
                                _reel.IdentificationRegexCheckResult = IdentificationResult.DataErr;
                                _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.LabelsSectionError))} ({labelMessage.Name.ToUpper()} {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))}: Labels {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))})";
                                _logData.Add("LabelMessage id", labelMessage.Name);
                                _logData.Add("Readed bclist", labelMessage.ListValue);
                                VrhLogger.Log("Configuration Error! Check the LabelMessages and Labels configuration sections!",
                                        _logData,
                                        null,
                                        LogLevel.Error,
                                        this.GetType()
                                    );
                            }
                            _reel.IdentificationRegexCheckCost = _startRegexCheck.Elapsed;
                            _idStateMachine.Fire(IdentificationTriggers.CallIdData);
                        }
                    }
                }
                else
                {
                    _reel.Empty = false;
                    _reel.IdentificationResult = IdentificationResult.DataErr;
                    _reel.IdentificationReadResult = IdentificationResult.Pass;
                    _reel.IdentificationRegexCheckResult = IdentificationResult.DataErr;
                    _reel.LabelId = SpecialLabelType.Unknown.ToString().ToUpper();
                    _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.LabelMessagesSectionError))} ({MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))}: LabelMessages {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))})";
                    VrhLogger.Log("Unknown Label! Check the LabelMessages configuration section!",
                            _logData,
                            null,
                            LogLevel.Error,
                            this.GetType()
                        );
                    // --> CloseOperation state
                    _reel.IdentificationRegexCheckCost = _startRegexCheck.Elapsed;
                    _idStateMachine.Fire(IdentificationTriggers.NoMatch);
                }
            }
        }

        /// <summary>
        /// Külső rendszer hívása Fail/pass beszerzése
        /// </summary>
        private async void OnIdData()
        {
            try
            {
                _startExternalSystemCheck.Restart();
                _reel.IdentificationExternalCheckResult = IdentificationResult.InProgress;
                _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.IdStatus, OperationStatus.Wait);
                var outgoingMessage = _main.Configuration.GetOutgoingMessage(_reel.LabelId);
                _logData.Add("OutgoingMessage", outgoingMessage?.Value);
                if (outgoingMessage != null)
                {
                    _reel.SendToBackgroundSystem = _reel.AllIdentificationData.ConstructString(outgoingMessage.Value);
                    _logData.Add("SendToBackgroundSystem", _reel.SendToBackgroundSystem);
                    VrhLogger.Log("Get OutgoingMessage is successful!",
                            _logData,
                            null,
                            LogLevel.Verbose,
                            this.GetType()
                        );
                    var content = new StringContent(_reel.SendToBackgroundSystem, Encoding.UTF8, "text/plain");
                    HttpResponseMessage httpResponse = null;
                    string httpResponseString = null;
                    try
                    {
                        httpResponse = await _main.HttpClient.PostAsync(_main.PluginLevelConfiguration.CustomFVSUri, content);
                        httpResponseString = await httpResponse.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        _reel.BackgroundSystemResult = BackgroundSystemResult.NoAck;
                        _reel.IdentificationResult = IdentificationResult.NoAck;
                        _reel.IdentificationExternalCheckResult = IdentificationResult.DataErr;
                        _reel.IdentificationExternalSystemCheckCost = _startExternalSystemCheck.Elapsed;
                        _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.CustomFVSNoAck))} ({ex.Message})";
                        _idStateMachine.Fire(IdentificationTriggers.Fail);
                        VrhLogger.Log("CustomFVS communication error!",
                                _logData,
                                ex,
                                LogLevel.Error,
                                this.GetType()
                            );
                        return;
                    }
                    _logData.Add("CustomFVS response", httpResponseString);
                    VrhLogger.Log("Call CustomFVS communication is successful.",
                            _logData,
                            null,
                            LogLevel.Verbose,
                            this.GetType()
                        );
                    _reel.BackgroundSystemResponse = httpResponseString;
                    string customFVSMessage = GetCustomFVSMessage(httpResponseString);
                    _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "CustomFVSMessage", Value = customFVSMessage });
                    _reel.BackgroundSystemMessage = customFVSMessage;
                    if (String.IsNullOrEmpty(customFVSMessage))
                    {
                        _reel.ErrorMessage = customFVSMessage;
                    }
                    _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = DataPoolDefinition.Reel.IdDataAck.ToString().ToUpper(), Value = httpResponseString });
                    var labels = _main.BarcodeMatch.Match(httpResponseString, 0, out LabelMessage labelMessage);
                    var label = labels.FirstOrDefault();
                    if (label != null)
                    {
                        GetLabelData(label);
                    }
                    if (labelMessage != null)
                    {
                        _logData.Add("CustomFVS response id", labelMessage.Name);
                        VrhLogger.Log("CustomFVS response identification is successful.",
                                _logData,
                                null,
                                LogLevel.Verbose,
                                this.GetType()
                            );
                        switch (labelMessage.Name.ToUpper())
                        {
                            case "PASS":
                                _reel.BookingNeed = true;
                                _reel.Status = "PASS";
                                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "STATUS", Value = _reel.Status });
                                _reel.BackgroundSystemResult = BackgroundSystemResult.Pass;
                                _reel.IdentificationResult = IdentificationResult.Pass;
                                _reel.IdentificationExternalCheckResult = IdentificationResult.Pass;
                                _reel.IdentificationExternalSystemCheckCost = _startExternalSystemCheck.Elapsed;
                                _idStateMachine.Fire(IdentificationTriggers.Pass);
                                break;
                            case "FAIL":
                                _reel.Status = "FAIL";
                                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "STATUS", Value = _reel.Status });
                                _reel.BackgroundSystemResult = BackgroundSystemResult.Fail;
                                _reel.IdentificationResult = IdentificationResult.Refuse;
                                _reel.IdentificationExternalCheckResult = IdentificationResult.DataErr;
                                _reel.IdentificationExternalSystemCheckCost = _startExternalSystemCheck.Elapsed;
                                _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.CustomFVSFail))} ({_reel.BackgroundSystemMessage})";
                                _idStateMachine.Fire(IdentificationTriggers.Fail);
                                break;
                            case "REREAD":
                                _reel.IdentificationExternalSystemCheckCost = _startExternalSystemCheck.Elapsed;
                                _idStateMachine.Fire(IdentificationTriggers.Reread);
#warning //TODO: Szüntesd meg a végtelen ciklusba lépési lehetőséget a maxreeread paraméter bevezetésével!
                                break;
                            default:
                                _reel.IdentificationResult = IdentificationResult.DataErr;
                                _reel.IdentificationExternalCheckResult = IdentificationResult.DataErr;
                                _reel.IdentificationExternalSystemCheckCost = _startExternalSystemCheck.Elapsed;
                                _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.IncommingSectionError))} ({labelMessage.Name.ToUpper()}: {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))} IncomingMessages {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))}";
                                _idStateMachine.Fire(IdentificationTriggers.Fail);
                                VrhLogger.Log("Unhandled CustomFVS response! Check the IncomingMessages configuration section! Posible IncomingMessage Ids: PASS, FAIL, REREAD.",
                                        _logData,
                                        null,
                                        LogLevel.Error,
                                        this.GetType()
                                    );
                                break;
                        }
                    }
                    else
                    {
                        _reel.IdentificationResult = IdentificationResult.DataErr;
                        _reel.IdentificationExternalCheckResult = IdentificationResult.DataErr;
                        _reel.IdentificationExternalSystemCheckCost = _startExternalSystemCheck.Elapsed;
                        _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.IncommingSectionError))} ({MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))} IncomingMessages {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))}";
                        _idStateMachine.Fire(IdentificationTriggers.Fail);
                        VrhLogger.Log("Unknown CustomFVS response! Check the IncomingMessages configuration section!",
                                _logData,
                                null,
                                LogLevel.Error,
                                this.GetType()
                            );
                    }
                }
                else
                {
                    _reel.IdentificationResult = IdentificationResult.DataErr;
                    _reel.IdentificationExternalCheckResult = IdentificationResult.DataErr;
                    _reel.IdentificationExternalSystemCheckCost = _startExternalSystemCheck.Elapsed;
                    _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.IdentificationOperation.OutgoingSectionError))} ({_reel.LabelId}: {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Configuration))} OutgoingMessages {MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.Section))}";
                    _idStateMachine.Fire(IdentificationTriggers.Fail);
                    VrhLogger.Log("Outgoing message NOT found or epty! Check the OutgoingMessages configuration section!",
                            _logData,
                            null,
                            LogLevel.Error,
                            this.GetType()
                        );
                }
            }
            finally
            {
                if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
                {
                    _startWaitForTriggerToLow.Restart();
                }
            }
        }

        private void GetLabelData(Label label)
        {
            foreach (var item in label.DataElements)
            {
                _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = item.Name, Value = item.Value });
                switch (item.Name.ToUpper())
                {
                    case "IMTSID":
                        _reel.InternalMTS = item.Value;
                        break;
                    case "IFVS":
                        _reel.InternalFVS = item.Value;
                        break;
                    case "IPN":
                        _reel.InternalPartNumber = item.Value;
                        break;
                    case "IVENDOR":
                        _reel.InternalVendor = item.Value;
                        break;
                    case "ILOT":
                        _reel.InternalLot = item.Value;
                        break;
                    case "ILOTDATE":
                        _reel.InternalLotDate = item.Value;
                        break;
                    case "ILOTSN":
                        _reel.InternalLotSN = item.Value;
                        break;
                    case "IQTY":
                        _reel.InternalQty = item.Value;
                        break;
                    case "MSL":
                        _reel.InternalMslCode = item.Value;
                        break;
                }
            }
        }

        private static string GetCustomFVSMessage(string httpResponseString)
        {
            string[] fields = httpResponseString.Split('|');
            string customFVSMessage = string.Empty;
            foreach (var field in fields)
            {
                int position = field.IndexOf("msg=");
                if (position > -1)
                {
                    customFVSMessage = field.Remove(0, 4).Trim();
                    break;
                }
            }
            return customFVSMessage;
        }

        private void OnWaitForIdCameraEnableToLow()
        {
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                if (_main.PrintOperation.GetMessage(_reel) != null)
                {
                    _main.IOOperation.SetIdCameraReady(OnOff.On);
                }
                else
                {
                    _idStateMachine.Fire(IdentificationTriggers.IdCamEnableChangedToLow);
                }
            }
        }

        /// <summary>
        /// Folyamat zárása
        /// </summary>
        private void OnCloseOperation()
        {
            lock (_locker)
            {
                _reel.IdentificationFullCost = _startOperation.Elapsed;
                _idStateMachine.Fire(IdentificationTriggers.End);
                _main.OperationEnd(Operations.Identification);
            }
        }

        /// <summary>
        /// Kreálja a folymmat állapotgépét
        /// </summary>
        private void CreateStateMachine()
        {
            _idStateMachine = new StateMachine<IdentificationState, IdentificationTriggers>(IdentificationState.Rest);
            // REST
            if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                _idStateMachine.Configure(IdentificationState.Rest)
                    .Permit(IdentificationTriggers.StartRead, IdentificationState.Read);
            }
            else
            {
                _idStateMachine.Configure(IdentificationState.Rest)
                    .Permit(IdentificationTriggers.Start, IdentificationState.WaitForEnable);
                _idStateMachine.Configure(IdentificationState.WaitForEnable)
                    .OnEntry(OnStart, "Start process:\\n" +
                        "- if IDCAMENABLE HIGH: start read (Fire StartRead)\\n")
                    .Permit(IdentificationTriggers.StartRead, IdentificationState.Read)
                    .Permit(IdentificationTriggers.End, IdentificationState.Rest);
            }
            // READ
            _idStateMachine.Configure(IdentificationState.Read)
                .OnEntry(OnRead, "Label read:\\n" +
                    "- Read label\\n" +
                    "- Set IDCAMREADY I/O out to HIGH\\n" +
                    "- Fire ReadEnd")
                .Permit(IdentificationTriggers.ReadEnd, IdentificationState.BarcodeMatch);
            // BARCODE MATCH
            if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                //LabelD
                _idStateMachine.Configure(IdentificationState.BarcodeMatch)
                    .OnEntry(OnMatch, "Label identification:\\n" +
                        "- Check & extract readed data with RegexChecker\\n" +
                        "- Fire EptyRead or NoRead or NoMatch or CallIdData")
                    .Permit(IdentificationTriggers.CallIdData, IdentificationState.IdData)
                    .Permit(IdentificationTriggers.EmptyRead, IdentificationState.CloseOperation)
                    .Permit(IdentificationTriggers.NoRead, IdentificationState.CloseOperation)
                    .Permit(IdentificationTriggers.NoMatch, IdentificationState.CloseOperation)
                    .Permit(IdentificationTriggers.GoldenSample, IdentificationState.CloseOperation);
            }
            else
            {
                // LabelE
                _idStateMachine.Configure(IdentificationState.BarcodeMatch)
                    .OnEntry(OnMatch, "Label identification:\\n" +
                        "- Check & extract readed data with RegexChecker\\n" +
                        "- Fire EptyRead or NoRead or NoMatch or CallIdData")
                    .Permit(IdentificationTriggers.CallIdData, IdentificationState.IdData)
                    .Permit(IdentificationTriggers.EmptyRead, IdentificationState.WaitForIdCameraEnableToLow)
                    .Permit(IdentificationTriggers.NoRead, IdentificationState.WaitForIdCameraEnableToLow)
                    .Permit(IdentificationTriggers.NoMatch, IdentificationState.WaitForIdCameraEnableToLow)
                    .Permit(IdentificationTriggers.GoldenSample, IdentificationState.WaitForIdCameraEnableToLow);
            }
            if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                // IDDATA
                _idStateMachine.Configure(IdentificationState.IdData)
                    .OnEntry(OnIdData, "Call IdData:\\n" +
                        "- Call IdData http API for fail/pass status\\n" +
                        "- Fire Fail or Pass or ReRead")
                    .Permit(IdentificationTriggers.Fail, IdentificationState.CloseOperation)
                    .Permit(IdentificationTriggers.Pass, IdentificationState.CloseOperation)
                    .Permit(IdentificationTriggers.Reread, IdentificationState.Read);
            }
            else
            {
                // IDDATA
                _idStateMachine.Configure(IdentificationState.IdData)
                    .OnEntry(OnIdData, "Call IdData:\\n" +
                        "- Call IdData http API for fail/pass status\\n" +
                        "- Fire Fail or Pass or ReRead")
                    .Permit(IdentificationTriggers.Fail, IdentificationState.WaitForIdCameraEnableToLow)
                    .Permit(IdentificationTriggers.Pass, IdentificationState.WaitForIdCameraEnableToLow)
                    .Permit(IdentificationTriggers.Reread, IdentificationState.Read);
                // WaitForIdCameraEnableToLow
                _idStateMachine.Configure(IdentificationState.WaitForIdCameraEnableToLow)
                    .OnEntry(OnWaitForIdCameraEnableToLow, "Wait for id cam enable (Trigger1) to low. If receive:\\n" +
                        "- Identification ready (ACK1) to low" +
                        "- Wait to ACK1 chenged to low" +
                        "- Fire ")
                    .Permit(IdentificationTriggers.IdCamEnableChangedToLow, IdentificationState.CloseOperation);
            }
            // CLOSE OPERATION
            _idStateMachine.Configure(IdentificationState.CloseOperation)
                .OnEntry(OnCloseOperation, "Close operation\\n" +
                    "Set data to reel\\n" +
                    "Fire End")
                .Permit(IdentificationTriggers.End, IdentificationState.Rest);
            _idStateMachine.OnTransitioned(IdStateMachineTransition);

            VrhLogger.Log($"Identification StateMachine graph  ({_main.WorkstationData.WorkStationType})",
            new Dictionary<string, string>()
                {
                    { "in DOT graph language", UmlDotGraph.Format(_idStateMachine.GetInfo()) }
                },
                null, LogLevel.Information, this.GetType());
        }

        /// <summary>
        /// Megváltozik az állapotgép állapota
        /// </summary>
        /// <param name="transition"></param>
        private void IdStateMachineTransition(StateMachine<IdentificationState, IdentificationTriggers>.Transition transition)
        {
            VrhLogger.Log("IdStateMachine state change",
                new Dictionary<string, string>()
                {
                    { "from state", transition.Source.ToString() },
                    { "to state", transition.Destination.ToString() },
                    { "by trigger", transition.Trigger.ToString() }
                },
                null, LogLevel.Debug, _idStateMachine.GetType());
        }

        /// <summary>
        /// azonosítás folymamt álalpotgépe
        /// </summary>
        private StateMachine<IdentificationState, IdentificationTriggers> _idStateMachine;

        /// <summary>
        /// A felismert cimke, amivel dolgozik
        /// </summary>
        private Label _label;

        /// <summary>
        /// A folymmat aktuális logdata-ja
        /// </summary>
        private Dictionary<string, string> _logData;

        /// <summary>
        /// A pozición lévő tekercs 
        /// </summary>
        private ReelData _reel
        {
            get
            {
                lock (_locker)
                {
                    return _main.Reels[ReelPosition.Identification];
                }
            }
        }

        private bool _idCameraEnabled = false;

        private Stopwatch _startOperation = new Stopwatch();
        private Stopwatch _startRead = new Stopwatch();
        private Stopwatch _startRegexCheck = new Stopwatch();
        private Stopwatch _startExternalSystemCheck = new Stopwatch();
        private Stopwatch _startWaitForTriggerToLow = new Stopwatch();

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
