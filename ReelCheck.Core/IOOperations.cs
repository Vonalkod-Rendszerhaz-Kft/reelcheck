using System;
using System.Collections.Generic;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using Vrh.CameraService.EventHubContract;
using Vrh.Logger;
using ReelCheck.Core.Configuration;
using System.Threading.Tasks;

namespace ReelCheck.Core
{
    /// <summary>
    /// I/O be és kimenetek kezelése
    /// </summary>
    internal class IOOperations : OperationWithCameraService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="camera">Kamera logikai neve (eventhub channel a camera szolgáltatásban)</param>
        /// <param name="main">EventHub Csatorna, melyen át a kameraszolgáltatás elérhető</param>
        /// <param name="idCameraServiceCallLocker">megosztott locker, ami sorosítja </param>
        public IOOperations(string camera, ReelCheck main, object idCameraServiceCallLocker)
            : base(idCameraServiceCallLocker)
        {
            _camera = camera;
            _cameraEventHubChannel = camera;
            _main = main;
            lock (idCameraServiceCallLocker)
            {
                EventHubCore.InitielizeChannel<RedisPubSubChannel>(_cameraEventHubChannel);
            }
            // Feliratkozás camera channel IOCHANGE
            lock (_locker)
            {
                EventHubCore.RegisterHandler<RedisPubSubChannel, IOEXTIOStateMessageResult>(_cameraEventHubChannel, IOInputChanged);
                EventHubCore.RegisterHandler<RedisPubSubChannel, IOChangeMessageResult>(_cameraEventHubChannel, IOInputChanged);
            }
        }

        /// <summary>
        /// Lekérdezi az összes bemenet állapotát
        /// </summary>
        public void GetAllIOInputs()
        {
            if (_disposedValue)
            {
                return;
            }
            // Bemenetek jelenleg
            GetIOInputState(_main.Configuration.EnableIOInput);
            GetIOInputState(_main.Configuration.StickingDoneIOInput);
            GetIOInputState(_main.Configuration.StatusResetIOInput);
            if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
            {
                GetIOInputState(_main.Configuration.HardResetIOInput);
            }
            else
            {
                GetIOInputState(_main.Configuration.IdCameraEnable);
                GetIOInputState(_main.Configuration.CheckCameraEnable);
            }
        }


        /// <summary>
        /// Kezdeményezi, hogy megkaopja az aktuális bemeneti értéket
        /// </summary>
        /// <param name="ioInput">Ennek a bemenetnek az értékeét</param>
        public void GetIOInputState(ReelcheckConfiguration.DigitalIO ioInput)
        {
            if (_disposedValue)
            {
                return;
            }
            try
            {
                if (ioInput.Type == DigitalIOType.IOExt)
                {
                    var request = new GetIOEXTIOMessage(ioInput.Port);
                    IOEXTIOStateMessageResult response = null;
                    lock (_sharedEventHubCallLocker)
                    {
                        response = EventHubCore.Call<RedisPubSubChannel, GetIOEXTIOMessage, IOEXTIOStateMessageResult>
                                    (_cameraEventHubChannel, request, new TimeSpan(0, 0, 2));
                    }
                    IOInputChanged(response);
                }
                else
                {
                    var request = new GetIOMessage();
                    IOChangeMessageResult response = null;
                    lock (_sharedEventHubCallLocker)
                    {
                        //var t = Task<IOChangeMessageResult>.Factory.StartNew(() => EventHubCore.Call<RedisPubSubChannel, GetIOMessage, IOChangeMessageResult>
                        //    (_cameraEventHubChannel, request, new TimeSpan(0, 0, 2)), TaskCreationOptions.LongRunning);
                        //t.Wait();
                        //response = t.Result;
                        response = EventHubCore.Call<RedisPubSubChannel, GetIOMessage, IOChangeMessageResult>
                            (_cameraEventHubChannel, request, new TimeSpan(0, 0, 2));
                    }
                    IOInputChanged(response);
                }
                VrhLogger.Log($"Get I/O input ({ioInput})!", null, null, LogLevel.Debug, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"Get I/O input ({ioInput}) state error!", null, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Beállítja a Ready kimenetet
        /// </summary>
        /// <param name="state">on/off</param>
        public void SetReady(OnOff state)
        {
            if (_disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "state", $"{state}" },
                            { "configured meaning", $"READY" },
                        };
                if (_main.Configuration.ReadyIOOutput.Type == DigitalIOType.IOExt)
                {
                    byte port = _main.Configuration.ReadyIOOutput.Port;
                    logData.Add("type", "IOEXT");
                    logData.Add("port", $"{port}");
                    var request = new SetIOEXTIOMessage(port, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(
                            _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                else
                {
                    Bulbs bulbs = _main.Configuration.ReadyIOOutput.Port == 0 ? Bulbs.Green : Bulbs.Red;
                    logData.Add("type", "IO");
                    logData.Add("port", $"{bulbs}");
                    var request = new SetIOMessage(bulbs, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                VrhLogger.Log($"DiO: Set I/O output", logData, null, LogLevel.Verbose, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"DiO: Set I/O output error", logData, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Beállítja a sikeres nyomtatást jelző kimenetet
        /// </summary>
        /// <param name="state">on/off</param>
        public void SetLabelPrinted(OnOff state)
        {
            if (_disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "state", $"{state}" },
                            { "configured meaning", $"LABEL PRINTED" },
                        };
                if (_main.Configuration.LabelPrintedIOOutput.Type == DigitalIOType.IOExt)
                {
                    byte port = _main.Configuration.LabelPrintedIOOutput.Port;
                    logData.Add("type", "IOEXT");
                    logData.Add("port", $"{port}");
                    var request = new SetIOEXTIOMessage(port, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                else
                {
                    Bulbs bulbs = _main.Configuration.LabelPrintedIOOutput.Port == 0 ? Bulbs.Green : Bulbs.Red;
                    logData.Add("type", "IO");
                    logData.Add("port", $"{bulbs}");
                    var request = new SetIOMessage(bulbs, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                VrhLogger.Log($"DiO: Set I/O output", logData, null, LogLevel.Verbose, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"DiO: Set I/O output error", logData, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Beállítja a minden folymamt befejeztét jelző kimenetet
        /// </summary>
        /// <param name="state"></param>
        public void AllStationFinished(OnOff state)
        {
            if (_disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "state", $"{state}" },
                            { "configured meaning", $"ALL STATION FINISHED" },
                        };
                if (_main.Configuration.AllStationFinishedIOOutput.Type == DigitalIOType.IOExt)
                {
                    byte port = _main.Configuration.AllStationFinishedIOOutput.Port;
                    logData.Add("type", "IOEXT");
                    logData.Add("port", $"{port}");
                    var request = new SetIOEXTIOMessage(port, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                else
                {
                    Bulbs bulbs = _main.Configuration.AllStationFinishedIOOutput.Port == 0 ? Bulbs.Green : Bulbs.Red;
                    logData.Add("type", "IO");
                    logData.Add("port", $"{bulbs}");
                    var request = new SetIOMessage(bulbs, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                VrhLogger.Log($"DiO: Set I/O output", logData, null, LogLevel.Verbose, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"DiO: Set I/O output error", logData, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Beállítja a kiforgó tekercs engedélyezését jelző kimenetet
        /// </summary>
        /// <param name="state">on/off</param>
        public void SetWorkPieceOk(OnOff state)
        {
            if (_disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "state", $"{state}" },
                            { "configured meaning", $"WORKPIECE OK" },
                        };
                if (_main.Configuration.WorkpieceOKIOOutput.Type == DigitalIOType.IOExt)
                {
                    byte port = _main.Configuration.WorkpieceOKIOOutput.Port;
                    logData.Add("type", "IOEXT");
                    logData.Add("port", $"{port}");
                    var request = new SetIOEXTIOMessage(port, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                else
                {
                    Bulbs bulbs = _main.Configuration.WorkpieceOKIOOutput.Port == 0 ? Bulbs.Green : Bulbs.Red;
                    logData.Add("type", "IO");
                    logData.Add("port", $"{bulbs}");
                    var request = new SetIOMessage(bulbs, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                VrhLogger.Log($"DiO: Set I/O output", logData, null, LogLevel.Verbose, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"DiO: Set I/O output error", logData, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Beállítja a kiforgó tekercs tiltását jelző kimenetet
        /// </summary>
        /// <param name="state">on/off</param>
        public void SetWorkPieceNok(OnOff state)
        {
            if (_disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "state", $"{state}" },
                            { "configured meaning", $"WORKPIECE NOK" },
                        };
                if (_main.Configuration.WorkpieceNOKIOOutput.Type == DigitalIOType.IOExt)
                {
                    byte port = _main.Configuration.WorkpieceNOKIOOutput.Port;
                    logData.Add("type", "IOEXT");
                    logData.Add("port", $"{port}");
                    var request = new SetIOEXTIOMessage(port, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                else
                {
                    Bulbs bulbs = _main.Configuration.WorkpieceNOKIOOutput.Port == 0 ? Bulbs.Green : Bulbs.Red;
                    logData.Add("type", "IO");
                    logData.Add("port", $"{bulbs}");
                    var request = new SetIOMessage(bulbs, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                VrhLogger.Log($"DiO: Set I/O output", logData, null, LogLevel.Verbose, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"DiO: Set I/O output error", logData, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Beállítja, hogy a kiforgó tekercs pozició üres
        /// </summary>
        /// <param name="state">on/off</param>
        public void SetEmpty(OnOff state)
        {
            if (_disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "state", $"{state}" },
                            { "configured meaning", $"EMPTY" },
                        };
                if (_main.Configuration.EmptyIOOutput.Type == DigitalIOType.IOExt)
                {
                    byte port = _main.Configuration.EmptyIOOutput.Port;
                    logData.Add("type", "IOEXT");
                    logData.Add("port", $"{port}");
                    var request = new SetIOEXTIOMessage(port, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                else
                {
                    Bulbs bulbs = _main.Configuration.EmptyIOOutput.Port == 0 ? Bulbs.Green : Bulbs.Red;
                    logData.Add("type", "IO");
                    logData.Add("port", $"{bulbs}");
                    var request = new SetIOMessage(bulbs, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                VrhLogger.Log($"DiO: Set I/O output", logData, null, LogLevel.Verbose, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"DiO: Set I/O output error", logData, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Beállítja, hogy az ID camera készen van-e az azonosítással
        /// </summary>
        /// <param name="state">on/off</param>
        public void SetIdCameraReady(OnOff state)
        {
            if (_disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "state", $"{state}" },
                            { "configured meaning", $"ID CAMREA READY" },
                        };
                if (_main.Configuration.IdCameraReady.Type == DigitalIOType.IOExt)
                {
                    byte port = _main.Configuration.IdCameraReady.Port;
                    logData.Add("type", "IOEXT");
                    logData.Add("port", $"{port}");
                    var request = new SetIOEXTIOMessage(port, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                else
                {
                    Bulbs bulbs = _main.Configuration.IdCameraReady.Port == 0 ? Bulbs.Green : Bulbs.Red;
                    logData.Add("type", "IO");
                    logData.Add("port", $"{bulbs}");
                    var request = new SetIOMessage(bulbs, state == OnOff.On);
                    lock (_sharedEventHubCallLocker)
                    {
                        EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(
                        _cameraEventHubChannel, request, new TimeSpan(0, 0, 10));
                    }
                }
                VrhLogger.Log($"DiO: Set I/O output", logData, null, LogLevel.Verbose, this.GetType());
            }
            catch (Exception ex)
            {
                VrhLogger.Log($"DiO: Set I/O output error", logData, ex, LogLevel.Error, this.GetType());
            }
        }

        /// <summary>
        /// Enable bemenet utolsó ismert állalpota
        /// </summary>
        public OnOff Enable
        {
            get
            {
                lock (_locker)
                {
                    return _enable;
                }
            }
        }
        private OnOff _enable;

        /// <summary>
        /// Címkézés kész utolsó ismert állapota
        /// </summary>
        public OnOff StickingDone
        {
            get
            {
                lock (_locker)
                {
                    return _stickingDone;
                }
            }
        }
        private OnOff _stickingDone;

        /// <summary>
        /// 
        /// </summary>
        public OnOff StatusReset
        {
            get
            {
                lock (_locker)
                {
                    return _statusReset;
                }
            }
        }
        private OnOff _statusReset;

        /// <summary>
        /// LABELD: Hard reset jel utolsó ismert állapota
        /// </summary>
        public OnOff HardReset
        {
            get
            {
                lock (_locker)
                {
                    return _hardReset;
                }
            }
        }
        private OnOff _hardReset;

        /// <summary>
        /// LABELE: ID kamera utolsó ismert állapota
        /// </summary>
        public OnOff IdCameraEnable
        {
            get
            {
                lock (_locker)
                {
                    return _idCameraEnable;
                }
            }
        }
        private OnOff _idCameraEnable;

        /// <summary>
        /// LABELE: Check kamera utolsó ismert állapota
        /// </summary>
        public OnOff CheckCameraEnable
        {
            get
            {
                lock (_locker)
                {
                    return _checkCameraEnable;
                }
            }
        }
        private OnOff _checkCameraEnable;

        /// <summary>
        /// Change I/O input handler
        /// </summary>
        /// <param name="changeMessage">Az input változásEventHub üzenet</param>
        private void IOInputChanged(IOEXTIOStateMessageResult changeMessage)
        {
            if (changeMessage == null || _disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "input type", "IOEXT" },
                            { "input no.", $"{changeMessage.IOEXTIOPort}" },
                            { "state to high", $"{changeMessage.IOEXTIOState}" },
                        };
                if (_main.Configuration.EnableIOInput.Type == DigitalIOType.IOExt &&
                    _main.Configuration.EnableIOInput.Port == changeMessage.IOEXTIOPort)
                {
                    logData.Add("Configured meaning", "ENABLE");
                    // ENABLE    
                    if (changeMessage.IOEXTIOState)
                    {
                        // HIGH
                        lock (_locker)
                        {
                            _enable = OnOff.On;
                            _main.EnableToHigh();
                        }
                    }
                    else
                    {
                        // LOW
                        lock (_locker)
                        {
                            _enable = OnOff.Off;
                            _main.EnableToLow();
                        }
                    }
                }
                if (_main.Configuration.StickingDoneIOInput.Type == DigitalIOType.IOExt &&
                    _main.Configuration.StickingDoneIOInput.Port == changeMessage.IOEXTIOPort)
                {
                    logData.Add("Configured meaning", "STICKINGDONE");
                    //STICKING DONE
                    if (changeMessage.IOEXTIOState)
                    {
                        // HIGH
                        lock (_locker)
                        {
                            _stickingDone = OnOff.On;
                            _main.PrintOperation?.StickingDoneToHigh();
                        }
                    }
                    else
                    {
                        // LOW
                        lock (_locker)
                        {
                            _stickingDone = OnOff.Off;
                            _main.PrintOperation?.StickingDoneToLow();
                        }
                    }
                }
                if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
                {
                    if (_main.Configuration.StatusResetIOInput.Type == DigitalIOType.IOExt &&
                        _main.Configuration.StatusResetIOInput.Port == changeMessage.IOEXTIOPort)
                    {
                        logData.Add("Configured meaning", "STATUS RESET");
                        //HARD RESET
                        if (changeMessage.IOEXTIOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _statusReset = OnOff.On;
                                _main.StatusResetOn();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _statusReset = OnOff.Off;
                                _main.StatusResetOff();
                            }
                        }
                    }
                    if (_main.Configuration.HardResetIOInput.Type == DigitalIOType.IOExt &&
                        _main.Configuration.HardResetIOInput.Port == changeMessage.IOEXTIOPort)
                    {
                        logData.Add("Configured meaning2", "HARDRESET");
                        //HARD RESET
                        if (changeMessage.IOEXTIOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _hardReset = OnOff.On;
                                _main.HardResetOn();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _hardReset = OnOff.Off;
                                _main.HardResetOff();
                            }
                        }
                    }
                }
                if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
                {
                    if (_main.Configuration.IdCameraEnable.Type == DigitalIOType.IOExt &&
                        _main.Configuration.IdCameraEnable.Port == changeMessage.IOEXTIOPort)
                    {
                        logData.Add("Configured meaning", "IDCAMERA ENABLE");
                        //ID CAMERA ENABLE
                        if (changeMessage.IOEXTIOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _idCameraEnable = OnOff.On;
                                _main.IdentificationOperation?.IdCamEnableToHigh();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _idCameraEnable = OnOff.Off;
                                _main.IdentificationOperation?.IdCamEnableToLow();
                            }
                        }
                    }
                    if (_main.Configuration.CheckCameraEnable.Type == DigitalIOType.IOExt &&
                        _main.Configuration.CheckCameraEnable.Port == changeMessage.IOEXTIOPort)
                    {
                        logData.Add("Configured meaning", "CHECKCAMERA ENABLE");
                        //CHECK CAMERA ENABLE
                        if (changeMessage.IOEXTIOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _checkCameraEnable = OnOff.On;
                                _main.CheckOperation?.CheckCameraEnableToHigh();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _checkCameraEnable = OnOff.Off;
                                _main.CheckOperation?.CheckCameraEnableToLow();
                            }
                        }
                    }
                }
            }
            finally
            {
                VrhLogger.Log("DiI: Digital Input changed (or queried)", logData,
                        null, LogLevel.Debug, this.GetType()
                    );
            }
        }

        /// <summary>
        /// Kamera bemenet változás
        /// </summary>
        /// <param name="changeMessage">Az egyetlen bemenet változásának iránya</param>
        private void IOInputChanged(IOChangeMessageResult changeMessage)
        {
            if (changeMessage == null || _disposedValue)
            {
                return;
            }
            Dictionary<string, string> logData = null;
            try
            {
                logData = new Dictionary<string, string>()
                        {
                            { "input type", "IO" },
                            { "input no.", $"0" },
                            { "state to high", $"{changeMessage.IOState}" },
                        };
                if (_main.Configuration.EnableIOInput.Type == DigitalIOType.IO)
                {
                    logData.Add("Configured meaning", "ENABLE");
                    // ENABLE    
                    if (changeMessage.IOState)
                    {
                        // HIGH
                        lock (_locker)
                        {
                            _enable = OnOff.On;
                            _main.EnableToHigh();
                        }
                    }
                    else
                    {
                        // LOW
                        lock (_locker)
                        {
                            _enable = OnOff.Off;
                            _main.EnableToLow();
                        }
                    }
                }
                if (_main.Configuration.StickingDoneIOInput.Type == DigitalIOType.IO)
                {
                    logData.Add("Configured meaning", "STICKINGDONE");
                    //STICKING DONE
                    if (changeMessage.IOState)
                    {
                        // HIGH
                        lock (_locker)
                        {
                            _stickingDone = OnOff.On;
                            _main.PrintOperation?.StickingDoneToHigh();
                        }
                    }
                    else
                    {
                        // LOW
                        lock (_locker)
                        {
                            _stickingDone = OnOff.Off;
                            _main.PrintOperation?.StickingDoneToLow();
                        }
                    }
                }
                if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
                {
                    if (_main.Configuration.StatusResetIOInput.Type == DigitalIOType.IO)
                    {
                        logData.Add("Configured meaning", "STATUS RESET");
                        //Status RESET
                        if (changeMessage.IOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _statusReset = OnOff.On;
                                _main.StatusResetOn();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _statusReset = OnOff.Off;
                                _main.StatusResetOff();
                            }
                        }
                    }
                    if (_main.Configuration.HardResetIOInput.Type == DigitalIOType.IO)
                    {
                        logData.Add("Configured meaning", "HARDRESET");
                        //HARD RESET
                        if (changeMessage.IOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _hardReset = OnOff.On;
                                _main.HardResetOn();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _hardReset = OnOff.Off;
                                _main.HardResetOff();
                            }
                        }
                    }
                }
                if (_main.WorkstationData.WorkStationType == WorkStationType.SemiAutomatic)
                {
                    if (_main.Configuration.IdCameraEnable.Type == DigitalIOType.IO)
                    {
                        logData.Add("Configured meaning", "IDCAMERA ENABLE");
                        //ID CAMERA ENABLE
                        if (changeMessage.IOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _idCameraEnable = OnOff.On;
                                _main.IdentificationOperation?.IdCamEnableToHigh();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _idCameraEnable = OnOff.Off;
                                _main.IdentificationOperation?.IdCamEnableToLow();
                            }
                        }
                    }
                    if (_main.Configuration.CheckCameraEnable.Type == DigitalIOType.IO)
                    {
                        logData.Add("Configured meaning", "CHECKCAMERA ENABLE");
                        //CHECK CAMERA ENABLE
                        if (changeMessage.IOState)
                        {
                            // HIGH
                            lock (_locker)
                            {
                                _checkCameraEnable = OnOff.On;
                                _main.CheckOperation?.CheckCameraEnableToHigh();
                            }
                        }
                        else
                        {
                            // LOW
                            lock (_locker)
                            {
                                _checkCameraEnable = OnOff.Off;
                                _main.CheckOperation?.CheckCameraEnableToLow();
                            }
                        }
                    }
                }
            }
            finally
            {
                VrhLogger.Log("DIN: Digital Input changed (or queried)", logData,
                        null, LogLevel.Debug, this.GetType()
                    );
            }
        }

        #region IDisposable members

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    lock (_locker)
                    {
                        EventHubCore.DropHandler<RedisPubSubChannel, IOEXTIOStateMessageResult>(_cameraEventHubChannel, IOInputChanged);
                        EventHubCore.DropHandler<RedisPubSubChannel, IOChangeMessageResult>(_cameraEventHubChannel, IOInputChanged);
                    }
                    _main = null;
                }
                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                _disposedValue = true;
            }
        }

        #endregion IDisposable members
    }
}