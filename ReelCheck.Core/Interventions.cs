using ReelCheck.EventHub.Contracts;
using System;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Core
{
    /// <summary>
    /// Osztály mely fogadja az összes lehetséges beavatkozást
    /// </summary>
    internal class Interventions : IDisposable
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="main">Main objektum referencia</param>
        public Interventions(ReelCheck main)
        {
            lock (_locker)
            {
                _main = main;
                string channelId = _main.WorkstationData.StationId.ToUpper();
                // Eventhub felíratkozások
                // Login/aktiválás
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.Activate,
                    InterventionContract.Result>(channelId, Activate);
                // Logout/inaktíválás
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.InActivate,
                    InterventionContract.Result>(channelId, InActivate);
                // Újranyomtatás
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.RePrint,
                    InterventionContract.Result>(channelId, RePrint);
                // Címkenyomtatás átlépése
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.SkipPrint,
                    InterventionContract.Result>(channelId, SkipPrint);
                // Manuális olvasás be (csak félautomata (LABELE))
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.ManualMode.On,
                    InterventionContract.Result>(channelId, ManualModeOn);
                // Manuális olvasás ki (csak félautomata (LABELE))
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.ManualMode.Off,
                    InterventionContract.Result>(channelId, ManualModeOff);
                // GoldenSample
                // indít
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.GoldenSampleMode.Enter,
                    InterventionContract.Result>(channelId, GoldenSampleStart);
                // leállít
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.GoldenSampleMode.Exit,
                    InterventionContract.Result>(channelId, GoldenSampleStop);
                // Határidőt töröl
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.GoldenSampleMode.DeleteDue,
                    InterventionContract.Result>(channelId, GoldenSampleDeleteDue);
                // Határidő most
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.GoldenSampleMode.SetDue,
                    InterventionContract.Result>(channelId, GoldenSampleSetDue);
                // Blokkolás funkció beavatkozásai
                // Blokkolás
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.Blocking.Block,
                    InterventionContract.Result>(channelId, Block);
                // Blokkolás feloldása
                EventHubCore.RegisterHandler<RedisPubSubChannel,
                    InterventionContract.Blocking.UnBlock,
                    InterventionContract.Result>(channelId, UnBlock);
            }
        }

        /// <summary>
        /// Gép blokkolása
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Response<InterventionContract.Result> Block(Request<InterventionContract.Blocking.Block, InterventionContract.Result> request)
        {
            var myResponse = request.MyResponse;
            try
            {
                if (!_main.Configuration.ManualOperationBlockingEnabled)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.ManualBlockingDisabled)));
                }
                CheckBadStates(false);
                if (_main.WorkstationData.OperationBlocked)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.Blocked)));
                }
                _main.Block(BlockReason.Manual);
            }
            catch (Exception ex)
            {
                myResponse.Exception = ex;
                _main.UIMessages.DropMessage(ex.Message);
            }
            return myResponse;
        }

        /// <summary>
        /// Blokkolás feloldása
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Response<InterventionContract.Result> UnBlock(Request<InterventionContract.Blocking.UnBlock, InterventionContract.Result> request)
        {
            var myResponse = request.MyResponse;
            try
            {
                CheckBadStates(false);
                _main.Unblock();
            }
            catch (Exception ex)
            {
                myResponse.Exception = ex;
                _main.UIMessages.DropMessage(ex.Message);
            }
            return myResponse;

        }

        /// <summary>
        /// Aranyminta teszt indítása
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Response<InterventionContract.Result> GoldenSampleStart(Request<InterventionContract.GoldenSampleMode.Enter, InterventionContract.Result> request)
        {
            var myResponse = request.MyResponse;
            try
            {
                CheckBadStates();
                _main.GoldenSampleModule.Start();
            }
            catch (Exception ex)
            {
                myResponse.Exception = ex;
                _main.UIMessages.DropMessage(ex.Message);
            }
            return myResponse;
        }

        private Response<InterventionContract.Result> GoldenSampleStop(Request<InterventionContract.GoldenSampleMode.Exit, InterventionContract.Result> request)
        {
            var myResponse = request.MyResponse;
            try
            {
                CheckBadStates();
                _main.GoldenSampleModule.Stop();
            }
            catch (Exception ex)
            {
                myResponse.Exception = ex;
                _main.UIMessages.DropMessage(ex.Message);
            }
            return myResponse;
        }

        private Response<InterventionContract.Result> GoldenSampleDeleteDue(Request<InterventionContract.GoldenSampleMode.DeleteDue, InterventionContract.Result> request)
        {
            var myResponse = request.MyResponse;
            try
            {
                CheckBadStates();
                _main.GoldenSampleModule.DeleteDue();
            }
            catch (Exception ex)
            {
                myResponse.Exception = ex;
                _main.UIMessages.DropMessage(ex.Message);
            }
            return myResponse;
        }

        private Response<InterventionContract.Result> GoldenSampleSetDue(Request<InterventionContract.GoldenSampleMode.SetDue, InterventionContract.Result> request)
        {
            var myResponse = request.MyResponse;
            try
            {
                if (_main.WorkstationData.OperationBlocked)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.Blocked)));
                }
                CheckBadStates();
                _main.GoldenSampleModule.SetDue(GoldenSampleDueMode.Iv);
            }
            catch (Exception ex)
            {
                myResponse.Exception = ex;
                _main.UIMessages.DropMessage(ex.Message);
            }
            return myResponse;
        }

        /// <summary>
        /// Manuálisan bevitt adat törlése
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Response<InterventionContract.Result> ManualModeOff(Request<InterventionContract.ManualMode.Off, InterventionContract.Result> request)
        {
            var myResponse = request.MyResponse;
            try
            {
                if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.On.LabelEOnly)));
                }
                CheckBadStates();
                if (_main.WorkstationData.OperationBlocked)
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.Blocked)));
                }
                if (string.IsNullOrEmpty(_main.WorkstationData.ManualData))
                {
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.Off.NoManualData)));
                }
                _main.WorkstationData.ManualData = string.Empty;
                _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.Normal);
                _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.Off.Success)));
            }
            catch (Exception ex)
            {
                myResponse.Exception = ex;
                _main.UIMessages.DropMessage(ex.Message);
            }
            return myResponse;
        }

        /// <summary>
        /// Manuális adatbevitel
        /// </summary>
        /// <param name="request">Kérés</param>
        /// <returns>válasz</returns>
        private Response<InterventionContract.Result> ManualModeOn(Request<InterventionContract.ManualMode.On, InterventionContract.Result> request)
        {
            lock (_locker)
            {
                var myResponse = request.MyResponse;
                try
                {
                    if (_main.WorkstationData.WorkStationType == WorkStationType.Automatic)
                    {
                        throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.On.LabelEOnly)));
                    }
                    CheckBadStates();
                    if (_main.WorkstationData.OperationBlocked)
                    {
                        throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.Blocked)));
                    }
                    string bcList = string.Empty;
                    if (!string.IsNullOrEmpty(request.RequestContent.FVSCode?.Trim()))
                    {
                        bcList = $"TMPL=FVSONLY;BCLIST::FVS={request.RequestContent.FVSCode.Trim()};";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(request.RequestContent.PartNumber?.Trim()))
                        {
                            throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.On.PartNumberRequired)));
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(request.RequestContent.LotNumber?.Trim()))
                            {
                                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.On.LotNumberRequired)));
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(request.RequestContent.Qty?.Trim()))
                                {
                                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.On.QtyRequired)));
                                }
                                else
                                {
                                    bcList = $"TMPL=BCLIST;BCLIST::{request.RequestContent.PartNumber.Trim()},{request.RequestContent.LotNumber.Trim()},{request.RequestContent.Qty.Trim()};";
                                }
                            }
                        }
                    }
                    _main.WorkstationData.ManualData = bcList;
                    _main.WorkstationData.WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.HandHeld);
                    _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.On.Success)));
                }
                catch (Exception ex)
                {
                    myResponse.Exception = ex;
                    _main.UIMessages.DropMessage(ex.Message);
                }
                return myResponse;
            }
        }

        /// <summary>
        /// Nyomtatás kihagyása
        /// </summary>
        /// <param name="request">fogadott kérés</param>
        /// <returns>válasz</returns>
        private Response<InterventionContract.Result> SkipPrint(Request<InterventionContract.SkipPrint, InterventionContract.Result> request)
        {
            lock (_locker)
            {
                var myResponse = request.MyResponse;
                try
                {
                    CheckBadStates();
                    if (_main.WorkstationData.OperationBlocked)
                    {
                        throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.Blocked)));
                    }
                    _main.PrintOperation.SkipPrint();
                    _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.SkipPrint.Success)));
                }
                catch (Exception ex)
                {
                    myResponse.Exception = ex;
                    _main.UIMessages.DropMessage(ex.Message);
                }
                return myResponse;
            }
        }

        /// <summary>
        /// Újranyomtatás
        /// </summary>
        /// <param name="request">Fogadott kérés</param>
        /// <returns>válasz</returns>
        private Response<InterventionContract.Result> RePrint(Request<InterventionContract.RePrint, InterventionContract.Result> request)
        {
            lock (_locker)
            {
                var myResponse = request.MyResponse;
                try
                {
                    CheckBadStates();
                    if (_main.WorkstationData.OperationBlocked)
                    {
                        throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.Blocked)));
                    }
                    _main.PrintOperation.Reprint();
                    _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.RePrint.Success)));
                }
                catch (Exception ex)
                {
                    myResponse.Exception = ex;
                    _main.UIMessages.DropMessage(ex.Message);
                }
                return myResponse;
            }
        }

        /// <summary>
        /// EventHub call handler: Aktíválás/bejelentkezés
        /// </summary>
        /// <param name="request">fogadott kérés</param>
        /// <returns>válasz</returns>
        private Response<InterventionContract.Result> Activate(Request<InterventionContract.Activate, InterventionContract.Result> request)
        {
            lock (_locker)
            {
                var myResponse = request.MyResponse;
                try
                {
                    CheckBadStates(false);
                    string delphiUserName = request.RequestContent.UserName.GetDelphiUserName();
                    switch (_main.MainStateMachine.State)
                    {
                        case MainState.NotReady:
                            _main.WorkstationData.UserName = delphiUserName;
                            _main.Login();
                            _main.MainStateMachine.Fire(MainTrigers.Activate);
                            break;
                        case MainState.Ready:
                            _main.WorkstationData.UserName = delphiUserName;
                            break;
                        case MainState.AtWork:
                            _main.WorkstationData.UserName = delphiUserName;
                            _main.Reels.Login(_main.WorkstationData.UserName);
                            _main.Login();
                            break;
                            //throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Activate.InProgress)));
                    }
                    _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Activate.Success)));
                }
                catch (Exception ex)
                {
                    myResponse.Exception = ex;
                    _main.UIMessages.DropMessage(ex.Message);
                }
                return myResponse;
            }
        }

        /// <summary>
        /// EventHub call handler: Inaktiválás/kijelentkezés
        /// </summary>
        /// <param name="request">fogadott kérés</param>
        /// <returns></returns>
        private Response<InterventionContract.Result> InActivate(Request<InterventionContract.InActivate, InterventionContract.Result> request)
        {
            lock (_locker)
            {
                var myResponse = request.MyResponse;
                try
                {
                    CheckBadStates();
                    string delphiUserName = request.RequestContent.UserName.GetDelphiUserName();
                    switch (_main.MainStateMachine.State)
                    {
                        case MainState.Ready:
                            if (delphiUserName.ToUpper() != _main.WorkstationData.UserName.ToUpper() &&
                            delphiUserName != "*")
                            {
                                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Inactivate.OtherUser)));
                            }
                            _main.WorkstationData.UserName = "";
                            _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Inactivate.Success)));
                            _main.Logout();
                            _main.MainStateMachine.Fire(MainTrigers.InActivate);
                            break;
                        case MainState.AtWork:
                            //_main.MainStateMachine.Fire(MainTrigers.AllReady);
                            _main.WorkstationData.UserName = "";
                            _main.UIMessages.DropMessage(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Inactivate.Success)));
                            _main.Logout();
                            //_main.MainStateMachine.Fire(MainTrigers.InActivate);
                            break;
                            //throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Inactivate.InProgress)));
                    }
                }
                catch (Exception ex)
                {
                    myResponse.Exception = ex;
                    _main.UIMessages.DropMessage(ex.Message);
                }
                return myResponse;
            }
        }

        /// <summary>
        /// Ha a bevaatkozások szempontjából hibás állapotokban van a munkahely, akkor kivételt dob,
        /// Ha withLoginCheck true, akkor is hibát dob, ha nincs felhazsnáló bejelentkezve a munkahelyen 
        /// </summary>
        /// <param name="withLoginCheck">Kell-e ellenőrizni, hogy van-e bejelentkezett felhazsnáló</param>
        private void CheckBadStates(bool withLoginCheck = true)
        {
            switch (_main.MainStateMachine.State)
            {
                case MainState.Starting:
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.BadStates.Starting)));
                case MainState.Stopping:
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.BadStates.Stopping)));
                case MainState.Stopped:
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.BadStates.Stopping)));
                case MainState.NotReady:
                    if (withLoginCheck)
                    {
                        throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.BadStates.NotLogin)));
                    }
                    break;
            }
        }

        /// <summary>
        /// Referencia a fő objektumra
        /// </summary>
        private ReelCheck _main;

        /// <summary>
        /// Instance level locker
        /// </summary>
        private object _locker = new object();

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    EventHubCore.DropChannel<RedisPubSubChannel>(_main.WorkstationData.WorkStationType.ToString().ToUpper());
                    _main = null;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Interventions() {
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
