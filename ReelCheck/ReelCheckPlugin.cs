using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrh.ApplicationContainer;
using ReelCheck.Core;

namespace ReelCheck
{
    public class ReelCheckPlugin : PluginAncestor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private ReelCheckPlugin()
        {
            EndLoad();
        }

        /// <summary>
        /// Static Factory (Ha nincs megadva, akkor egy egy paraméteres konstruktort kell implementálni, amely konstruktor paraméterben fogja megkapni a )
        /// </summary>
        /// <param name="instanceDefinition">A példány definiciója</param>
        /// <param name="instanceData">A példánynak átadott adat(ok)</param>
        /// <returns></returns>
        public static ReelCheckPlugin ReelCheckPluginFactory(InstanceDefinition instanceDefinition, Object instanceData)
        {
            var instance = new ReelCheckPlugin();
            instance._myData = instanceDefinition;
            return instance;
        }

        /// <summary>
        /// IPlugin.Start
        /// </summary>
        public override void Start()
        {
            if (MyStatus == PluginStateEnum.Starting || MyStatus == PluginStateEnum.Running)
            {
                return;
            }
            BeginStart();
            try
            {
                // Implement Start logic here 
                _reelCheck = new Core.ReelCheck(_myData.InstanceConfig, _myData.Type.PluginConfig, _myData.Id);
                base.Start();
            }
            catch (Exception ex)
            {
                SetErrorState(ex);
            }
        }

        /// <summary>
        /// IPlugin.Stop
        /// </summary>
        public override void Stop()
        {
            if (MyStatus == PluginStateEnum.Stopping || MyStatus == PluginStateEnum.Loaded)
            {
                return;
            }
            BeginStop();
            try
            {
                // Implement stop logic here
                _reelCheck?.Dispose();
                _reelCheck = null;
                base.Stop();
            }
            catch (Exception ex)
            {
                SetErrorState(ex);
            }
        }

        private Core.ReelCheck _reelCheck;

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        BeginDispose();
                        // dispose managed state (managed objects).

                        Stop();
                    }
                    finally
                    {
                        base.Dispose(disposing);
                    }
                }
                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TestPlugin() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

