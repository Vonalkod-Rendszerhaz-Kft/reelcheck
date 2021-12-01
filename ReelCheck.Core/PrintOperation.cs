using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stateless;
using Stateless.Graph;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using Vrh.Logger;
using VRH.Log4Pro.MultiLanguageManager;
using Vrh.PrintingService.EventHubContract;
using System.Diagnostics;
using System.Globalization;

namespace ReelCheck.Core
{
    /// <summary>
    /// Nyomtatás művelet
    /// sOlid: Ez az osztály a címke nyomtatás műveletének futásáért felelős
    /// </summary>
    internal class PrintOperation : OperationWithPrintingService
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="main">Referencia a main objektumra</param>
        public PrintOperation(ReelCheck main, object sharedPrintServiceCallLocker)
            : base(sharedPrintServiceCallLocker)
        {
            lock (_locker)
            {
                _main = main;
                _printerEventHubChannel = _main.PluginLevelConfiguration.PrintingServiceEventHubChannel;
                CreateStateMachine();
                EventHubCore.InitielizeChannel<RedisPubSubChannel>(_printerEventHubChannel);
            }
        }

        #region Public methods (kivülről is hozzáférhető műveletek)

        /// <summary>
        /// Nyomtatás művelet indítása
        /// </summary>
        public void Start()
        {
            lock (_locker)
            {
                _printStateMachine.Fire(PrintTriggers.Start);
            }
        }

        /// <summary>
        /// Címke újranyomtatása
        /// </summary>
        public void Reprint()
        {
            if (_printStateMachine.State != PrintState.WaitForIntervention)
            {
                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.RePrint.NoNeed)));
            }
            _reel.WaitPrintInterventionCost = _startWaitForIntervention.Elapsed;
            _printStateMachine.Fire(PrintTriggers.Reprint);
        }

        /// <summary>
        /// Nyomtatás kihagyása
        /// </summary>
        public void SkipPrint()
        {
            if (_printStateMachine.State != PrintState.WaitForIntervention && _printStateMachine.State != PrintState.WaitForSticking)
            {
                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.SkipPrint.NoNeed)));
            }
            if (_printStateMachine.State == PrintState.WaitForIntervention)
            {
                _reel.WaitPrintInterventionCost = _startWaitForIntervention.Elapsed;
                _printStateMachine.Fire(PrintTriggers.SkipPrint);
            }
            if (_printStateMachine.State == PrintState.WaitForSticking)
            {
                _reel.StickingCost = _startWaitForSticking.Elapsed;
                _stickingSkiped = true;
                _printStateMachine.Fire(PrintTriggers.StickingDone);
                _printStateMachine.Fire(PrintTriggers.ProcessEnd);
            }
        }

        /// <summary>
        /// Alapállapotba vissza (LabelE)
        /// </summary>
        public void Reset()
        {
            if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
            {
                if (_printStateMachine.State == PrintState.WaitForIntervention)
                {
                    _printStateMachine.Fire(PrintTriggers.SkipPrint);              
                }
                if (_printStateMachine.State == PrintState.WaitForSticking
                    || _printStateMachine.State == PrintState.WaitForStickingDoneReset)
                {
                    _printStateMachine.Fire(PrintTriggers.End);
                }
            }
        }

        /// <summary>
        /// A cimkefelrakás kész jel magasra vált
        /// </summary>
        public void StickingDoneToHigh()
        {
            if (_printStateMachine.State == PrintState.WaitForSticking)
            {
                _reel.StickingCost = _startWaitForSticking.Elapsed;
                _printStateMachine.Fire(PrintTriggers.StickingDone);
            }
        }

        public void HardReset()
        {
            if (_printStateMachine.State == PrintState.WaitForSticking
                || _printStateMachine.State == PrintState.WaitForStickingDoneReset
                || _printStateMachine.State == PrintState.WaitForIntervention)
            {
                _printStateMachine.Fire(PrintTriggers.End);
            }
        }

        /// <summary>
        /// A címkefelrakás kész jel alacsonyra vált
        /// </summary>
        public void StickingDoneToLow()
        {
            if (_printStateMachine.State == PrintState.WaitForStickingDoneReset)
            {
                _printStateMachine.Fire(PrintTriggers.ProcessEnd);
            }
        }

        #endregion Public methods (kivülről is hozzáférhető műveletek)

        #region PrintStateMachine

        /// <summary>
        /// Sikeres nyomtatás
        /// </summary>
        private void OnPrintOk()
        {
            lock (_locker)
            {
                _reel.PrintResult = PrintResult.Pass;
                _startWaitForSticking.Restart();
                _printStateMachine.Fire(PrintTriggers.WaitForSticking);
            }
        }

        /// <summary>
        /// Nyomtatás kihagyása
        /// </summary>
        private void OnPrintSkip()
        {
            lock (_locker)
            {
                _reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.SkipPrint)); 
                if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
                {
                    _reel.PrintResult = PrintResult.Skip;
                }
                else
                {
                    if (_main.IOOperation.Enable == OnOff.On)
                    {
                        _reel.PrintResult = PrintResult.Skip;
                    }
                }
                _main.IOOperation.SetLabelPrinted(OnOff.Off);
                _printStateMachine.Fire(PrintTriggers.End);
                _main.OperationEnd(Operations.Print);
            }
        }

        /// <summary>
        /// Nyomtatás
        /// </summary>
        private void OnPrint()
        {
            lock (_locker)
            {
                _startOperation.Restart();
                _startPrint.Restart();
                PrintTriggers outTrigger = PrintTriggers.PrintOk;
                try
                {
					try
					{
						string ifvsDate = _reel.InternalFVS.Substring(14, 6);
						var printTimeStamp = DateTime.ParseExact(ifvsDate, "yyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);
						_reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "PRINTTIMESTAMP", Value = printTimeStamp.ToString("yyyy-MM-dd") });
						_reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "WEEKNO", Value = printTimeStamp.WeekNo() });
					}
					catch (Exception)
					{
						_reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "PRINTTIMESTAMP", Value = DateTime.Now.ToString("yyyy-MM-dd") });
						_reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = "WEEKNO", Value = DateTime.Now.WeekNo() });
					}
					PrintMessage printMessage = null;
                    try
                    {
                        if (_reel.GoldenSample)
                        {
                            outTrigger = PrintTriggers.End;
                            _reel.PrintResult = PrintResult.None;
                            return;
                        }
                        _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.PrintStatus, OperationStatus.Operating);
                        _reel.PrintResult = PrintResult.InProgress;
                        if (_reel.IdentificationResult == IdentificationResult.None || _reel.IdentificationResult == IdentificationResult.Empty)
                        {
                            outTrigger = PrintTriggers.End;
                            if (_reel.IdentificationReadResult == IdentificationResult.Empty)
                            {
                                _reel.PrintResult = PrintResult.Empty;
                                _reel.StickingResult = PrintResult.Empty;
                            }
                            else
                            {
                                _reel.PrintResult = PrintResult.None;
                                _reel.StickingResult = PrintResult.None;
                            }
                            _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PrintNoNeed)));
                            return;
                        }
                        _printer = _main.Configuration.GetPrinter(_main.WorkstationData.WorkStationType.ToString().ToUpper(), _main.WorkstationData.StationId.ToUpper());
                        if (_printer == null)
                        {
                            _reel.PrintResult = PrintResult.None;
                            _reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PrinterNotDefined));
                            throw new Exception(_reel.ErrorMessage);
                        }
                        // Print
                        var message = GetMessage();
                        if (message != null)
                        {
                            printMessage = new PrintMessage(_printer.PhisycalName, message);
                            lock (_sharedEventHubCallLocker)
                            {
                                EventHubCore.Call<RedisPubSubChannel, PrintMessage, MessageResult>(_printerEventHubChannel, printMessage, new TimeSpan(0, 0, 30));
                            }
                            _reel.PrintAttempts++;
                            _reel.PrintResult = PrintResult.Pass;
                            _reel.Printer = _printer.PhisycalName;
                            _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PrintSuccess)));
                            VrhLogger.Log("Print successful!",
                                GetLogData(new KeyValuePair<string, string>("Label id", printMessage?.Message?.LabelDefinitionName)),
                                null, LogLevel.Debug, this.GetType());
                        }
                        else
                        {
                            _reel.PrintResult = PrintResult.None;
                            if (_reel.IdentificationResult != IdentificationResult.Pass)
                            {
                                //_reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.NoLabelThisStatus));
                                _main.UIMessages.DropMessage(_reel.ErrorMessage);
                            }
                            else
                            {
                                _reel.ErrorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PassPrintEventNotDefined));
                                _main.UIMessages.DropMessage(_reel.ErrorMessage);
                                throw new Exception(_reel.ErrorMessage);
                            }
                            VrhLogger.Log("No print. (No label defined this status.)",
                                GetLogData(),
                                null, LogLevel.Debug, this.GetType());
                            outTrigger = PrintTriggers.End;
                        }
                    }
                    catch (Exception ex)
                    {
                        //_reel.PrintResult = PrintResult.
                        outTrigger = PrintTriggers.PrintNok;
                        _reel.ErrorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PrintError))} ({ex.Message})";
                        _main.UIMessages.DropMessage(ex.Message);
                        _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PrintError)));
                        VrhLogger.Log("Print Error!",
                            GetLogData(new KeyValuePair<string, string>("Label id", printMessage?.Message?.LabelDefinitionName)),
                            ex, LogLevel.Error, this.GetType());
                        _startWaitForIntervention.Restart();
                    }
                }
                finally
                {                    
                    _reel.PrintCost = _startPrint.Elapsed;
                    _printStateMachine.Fire(outTrigger);
                    if (outTrigger == PrintTriggers.End)
                    {
                        _main.OperationEnd(Operations.Print);
                    }
                }
            }
        }

        /// <summary>
        /// Creálja az állapotgépet
        /// </summary>
        private void CreateStateMachine()
        {
            _printStateMachine = new StateMachine<PrintState, PrintTriggers>(PrintState.Rest);
            _printStateMachine.Configure(PrintState.Rest)
                .OnEntry(OnRest, "Goto Rest (initial) state:\\n" +
                        "- LabelPrinted set to LOW")
                .Permit(PrintTriggers.Start, PrintState.Print);
            _printStateMachine.Configure(PrintState.Print)
                .OnEntry(OnPrint, "Printing:\\n" +
                    "- if print not need: Fire End\\n" +
                    "- if print need: \\n" +
                    " - Print label & receive print succes\\n" +
                    " - Save print data into reel data\\n" +
                    " - if print fail: Fire PrintNok\\n" +
                    " - if print successful: Fire PrintOk\\n")
                .Permit(PrintTriggers.PrintOk, PrintState.PrintOk)
                .Permit(PrintTriggers.PrintNok, PrintState.WaitForIntervention)
                .Permit(PrintTriggers.End, PrintState.Rest);
            _printStateMachine.Configure(PrintState.WaitForIntervention)
                .Permit(PrintTriggers.Reprint, PrintState.Print)
                .Permit(PrintTriggers.SkipPrint, PrintState.NoPrint)
                .Permit(PrintTriggers.End, PrintState.Rest);
            _printStateMachine.Configure(PrintState.NoPrint)
                .OnEntry(OnPrintSkip, "Skip print operation:\\n" +
                    "- Set data in reel data\\n" +
                    "- Fire End")
                .Permit(PrintTriggers.End, PrintState.Rest);
            _printStateMachine.Configure(PrintState.PrintOk)
                .OnEntry(OnPrintOk, "Print Successful:\\n" +
                    "- Set data in real data\\n" +
                    "- Fire: WiatForSticking")
                .Permit(PrintTriggers.WaitForSticking, PrintState.WaitForSticking);
            _printStateMachine.Configure(PrintState.WaitForSticking)
                .OnEntry(OnWaitForSticking, "Wait For sticking:\\n" +
                    "- Set LabelPrinted to High\\n" +
                    "- Wait For StickingDone input High (Fire: StickingDone)")
                .Permit(PrintTriggers.StickingDone, PrintState.WaitForStickingDoneReset)
                .Permit(PrintTriggers.End, PrintState.Rest);
            _printStateMachine.Configure(PrintState.WaitForStickingDoneReset)
                .OnEntry(OnWaitForStickingDoneReset, "Wait for StickingDonetio Low:\\n" +
                    "- Set LabelPrinted to Low\\n" +
                    "- Wait For StickingDone input Low (Fire: PrintProcessEnd)")
                .Permit(PrintTriggers.ProcessEnd, PrintState.PrintProcessEnd)
                .Permit(PrintTriggers.End, PrintState.Rest);
            _printStateMachine.Configure(PrintState.PrintProcessEnd)
                .OnEntry(OnPrintProcessEnd, "Print process end:\\n" +
                    "- Report print process end to main\\n" +
                    "- Fire End trigger (Go to Rest)")
                .Permit(PrintTriggers.End, PrintState.Rest);
            _printStateMachine.OnTransitioned(PrintStateMachineTransition);
            VrhLogger.Log($"PrintStateMachine graph  ({_main.WorkstationData.WorkStationType})",
            new Dictionary<string, string>()
                {
                    { "in DOT graph language", UmlDotGraph.Format(_printStateMachine.GetInfo()) }
                },
                null, LogLevel.Information, this.GetType());
        }

        /// <summary>
        /// Noymtatás művelet zárása
        /// </summary>
        private void OnPrintProcessEnd()
        {
            _reel.FullPrintCost = _startOperation.Elapsed;
            _printStateMachine.Fire(PrintTriggers.End);
            _main.OperationEnd(Operations.Print);
            _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PrintEnd)));
        }

        /// <summary>
        /// Cimkézés kész jel visszavétellére várakozás
        /// </summary>
        private void OnWaitForStickingDoneReset()
        {
            if (_stickingSkiped)
            {
                _reel.StickingResult = PrintResult.Skip;
            }
            else
            {
                _reel.StickingResult = PrintResult.Pass;
            }
            _stickingSkiped = false;
            _main.IOOperation.SetLabelPrinted(OnOff.Off);
        }

        /// <summary>
        /// Cimkézésre várakozás
        /// </summary>
        private void OnWaitForSticking()
        {
            _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.PrintStatus, OperationStatus.Wait);
            _main.IOOperation.SetLabelPrinted(OnOff.On);
        }

        /// <summary>
        /// Nyugalmi állapot
        /// </summary>
        private void OnRest()
        {
            _main.IOOperation.SetLabelPrinted(OnOff.Off);
            _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.PrintStatus, OperationStatus.Ready);
        }

        /// <summary>
        /// Nyomtatás művelet állapotot vált
        /// </summary>
        /// <param name="transition">Állapot átmenet</param>
        private void PrintStateMachineTransition(StateMachine<PrintState, PrintTriggers>.Transition transition)
        {
            VrhLogger.Log("PrintStateMachine state change",
                new Dictionary<string, string>()
                {
                    { "from state", transition.Source.ToString() },
                    { "to state", transition.Destination.ToString() },
                    { "by trigger", transition.Trigger.ToString() }
                },
                null, LogLevel.Debug, _printStateMachine.GetType());
        }

        #endregion PrintStateMachine

        /// <summary>
        /// Visszadja a logdata dictionary-t
        /// </summary>
        /// <param name="plusItems">a megadott elemeket is hozzáfűzi</param>
        /// <returns>logdata dictionary</returns>
        private Dictionary<string, string> GetLogData(params KeyValuePair<string, string>[] plusItems)
        {
            var result = new Dictionary<string, string>()
                            {
                                { "Configuration", _main.Configuration.XmlFileDefinition },
                                { "Used EventHub cahnnel", _printerEventHubChannel },
                                { "Printer logical name" , _main.WorkstationData.WorkStationType.ToString().ToUpper()},
                                { "Status to print", _reel.IdentificationResult.ToString() },
                                { "Printer phisical name", _printer != null ? _printer.LogicalName : "???" },
                            };
            foreach (var item in plusItems)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

        /// <summary>
        /// Visszadja a nyomtatási üzenetet
        /// </summary>
        /// <returns></returns>
        public Message GetMessage(ReelData thisReel = null)
        {
            ReelData reel = thisReel == null ? _reel : thisReel;
            string eventId = null;
            string errorMessage = string.Empty;
            switch (reel.IdentificationResult)
            {
                case IdentificationResult.NoRead:
                    eventId = "LABELFAILNOREAD";
                    errorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.PrintedLabelMessage.NoRead));
                    break;
                case IdentificationResult.DataErr:
                    eventId = "LABELFAILDATAERROR";
                    errorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.PrintedLabelMessage.DataError));
                    break;
                case IdentificationResult.NoAck:
                    eventId = "LABELFAILNOACK";
                    errorMessage = MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.PrintedLabelMessage.NoAck));
                    break;
                case IdentificationResult.Refuse:
                    errorMessage = $"{MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.PrintedLabelMessage.Refuse))} {reel.BackgroundSystemMessage}";
                    eventId = "LABELFAILREFUSE";
                    break;
                case IdentificationResult.Pass:
                    eventId = "LABELPASS";
                    break;
            }
            if (String.IsNullOrEmpty(eventId))
            {
                VrhLogger.Log($"No print event for this result: {reel.IdentificationResult}",
                        null,
                        null,
                        LogLevel.Error,
                        this.GetType());
                return null;
            }
            else
            {
                if (thisReel == null)
                {
                    _reel.ErrorMessage = errorMessage;

                    int lineNumber = 1;
                    foreach (var line in errorMessage.SplitErrorMessage())
                    {
                        _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = $"MSG{lineNumber}", Value = line.RemoveAccents() });
                        lineNumber++;
                    }
                    for (int i = lineNumber + 1; i < 6; i++)
                    {
                        _reel.AllIdentificationData.SafeAdd(new KeyAndValue() { Key = $"MSG{lineNumber}", Value = string.Empty });
                        lineNumber++;
                    }
                    VrhLogger.Log($"Identificated print event: {eventId}",
                            null,
                            null,
                            LogLevel.Verbose,
                            this.GetType());
                }
                _printer = _main.Configuration.GetPrinter(_main.WorkstationData.WorkStationType.ToString().ToUpper(),
                                                          _main.StationId.ToUpper());
                var printEvent = _printer.PrintEvents.FirstOrDefault(x => x.EventId.ToUpper() == eventId.ToUpper());
                if (printEvent == null)
                {
                    VrhLogger.Log($"Event not defined for: {eventId}. Use defult event...",
                            null,
                            null,
                            LogLevel.Warning,
                            this.GetType());
                    printEvent = _printer.PrintEvents.FirstOrDefault(x => x.EventId == _printer.DefaultPrintEventId && x.Active);
                    if (printEvent == null)
                    {
                        if (reel.IdentificationResult == IdentificationResult.Pass)
                        {
                            throw new Exception(
                                MultiLanguageManager.GetTranslation(typeof(TrueWordCodes.ReelCheckService.UiMessage.PrintOperation.PassPrintEventNotDefined)));
                        }
                        return null;
                    }
                }
                Message message = null;
                if (printEvent != null && printEvent.Active)
                {
                    var dic = new Dictionary<string, string>();
                    foreach (var item in printEvent.LabelVars)
                    {
                        string cosnstructorSting = reel.AllIdentificationData.ConstructString(item.Value);
                        dic.Add(item.Name, cosnstructorSting);
                    }
                    message = new Message(printEvent.LabelId, null, dic, Modes.SYNC);
                }
                return message;
            }
        }

        /// <summary>
        /// A pozición lévő tekercs 
        /// </summary>
        private ReelData _reel
        {
            get
            {
                lock (_locker)
                {
                    return _main.Reels[ReelPosition.Print];
                }
            }
        }

        /// <summary>
        /// Print folyamot állapotgépe
        /// </summary>
        private StateMachine<PrintState, PrintTriggers> _printStateMachine;

        private bool _stickingSkiped = false;

        private Stopwatch _startOperation = new Stopwatch();
        private Stopwatch _startPrint = new Stopwatch();
        private Stopwatch _startWaitForIntervention = new Stopwatch();
        private Stopwatch _startWaitForSticking = new Stopwatch();
    }
}
