using System;

namespace ReelCheck.Core
{
    /// <summary>
    /// Művelet ősosztály
    /// </summary>
    internal class Operation : IDisposable
    {
        /// <summary>
        /// Construktor, mellyel megosztott EventHub call locker referenciiával lehet létrehozni
        /// </summary>
        /// <param name="sharedEventHubCallLocker">Megosztott call locker</param>
        public Operation(object sharedEventHubCallLocker)
        {
            if (sharedEventHubCallLocker != null)
            {
                _sharedEventHubCallLocker = sharedEventHubCallLocker;
            }
        }

        /// <summary>
        /// ReelCheck referencia a fő objektumok elérésére
        /// </summary>
        protected ReelCheck _main;

        /// <summary>
        /// Instance level locker
        /// </summary>
        protected object _locker = new object();

        protected object _sharedEventHubCallLocker = new object();

        #region IDisposable Support
        protected bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    lock (_locker)
                    {
                        // dispose managed state (managed objects).
                        _main = null;
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Operation() {
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
