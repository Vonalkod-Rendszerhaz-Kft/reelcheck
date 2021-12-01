using System;
using System.Collections.Generic;
using System.Linq;

namespace ReelCheck.Core
{
    /// <summary>
    /// Tekercsek tárolása egy n elelmű képzeletbeli körpályán
    /// </summary>
    internal class ReelDataStoreHouse : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workStationData">munkahely</param>
        public ReelDataStoreHouse(WorkstationData workStationData)
        {
            switch (workStationData.WorkStationType)
            {
                default:
                case WorkStationType.Automatic:
                    _maxReels = 4;
                    break;
                case WorkStationType.SemiAutomatic:
                    _maxReels = 1;
                    break;
            }
            for (int i = 1; i <= _maxReels; i++)
            {
                var reel = new ReelDataHolder() { No = i };
                _reels.Add(reel);
            }
        }

        /// <summary>
        /// Visszadja az megadott pozición lévő reelt
        /// </summary>
        /// <param name="position">index</param>
        /// <returns>Tekercs</returns>
        public ReelData this[ReelPosition position]
        {
            get
            {
                int i = 1;                
                if (_maxReels > 1)
                {
                    switch (position)
                    {
                        default:
                        case ReelPosition.Load:
                            i = 0;
                            break;
                        case ReelPosition.Identification:
                            i = 1;
                            break;
                        case ReelPosition.Print:
                            i = 2;
                            break;
                        case ReelPosition.Check:
                            i = 3;
                            break;
                        case ReelPosition.UnLoad:
                            i = 4;
                            break;
                    }
                }
                return _reels.FirstOrDefault(x => x.No == i)?.Reel;
            }
        }

        /// <summary>
        /// Betesz egy újabb tekercset a töltő pozicióba
        /// 
        /// 1. ID (valójában ide töltünk)
        /// 2. Print
        /// 3. Check
        /// 4. Load/UnLoad
        /// </summary>
        /// <param name="reel">tekercs, vagy null</param>
        public void Load(ReelData reel)
        {
            lock (_locker)
            {
                if (_maxReels > 1)
                {
                    for (int i = _maxReels; i > 1; i--)
                    {
                        var from = _reels.FirstOrDefault(x => x.No == i - 1);
                        var to = _reels.FirstOrDefault(x => x.No == i);
                        to.Reel = from.Reel;
                        if (to.Reel != null)
                        {
                            to.Reel.Position = i;
                        }
                        if (i == 2)
                        {
                            from.Reel = reel;
                        }
                        else
                        {
                            from.Reel = null;
                        }
                    }
                }
                else
                {
                    var to =_reels.FirstOrDefault(x => x.No == 1);
                    to.Reel = reel;
                }
            }
        }

        /// <summary>
        /// Felhasználó jelentkezik be
        /// </summary>
        /// <param name="userName">felhasználó neve</param>
        public void Login(string userName)
        {
            foreach (var reel in _reels)
            {
                if (reel.Reel != null)
                {
                    if (String.IsNullOrEmpty(reel.Reel.UserName))
                    {
                        reel.Reel.UserName = userName;
                    }
                }
            }
        }

        /// <summary>
        /// Tekercsek maximális száma
        /// </summary>
        private int _maxReels;

        /// <summary>
        /// Tekercsek listája
        /// </summary>
        private List<ReelDataHolder> _reels = new List<ReelDataHolder>();

        /// <summary>
        /// instance level locker
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
                    lock (_locker)
                    {
                        // dispose managed state (managed objects).
                        foreach (var reel in _reels)
                        {
                            reel.Reel?.Dispose();
                        }
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ReelDataStoreHouse() {
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

    /// <summary>
    /// Létező tekercs poziciók automata munkahelynél (LABELD)
    /// </summary>
    internal enum ReelPosition
    {
        /// <summary>
        /// Töltő/kivevő pozició
        /// </summary>
        Load,
        /// <summary>
        /// Azonosítási pozició
        /// </summary>
        Identification,
        /// <summary>
        /// Nyomztatási pozició
        /// </summary>
        Print,
        /// <summary>
        /// Ellenőrző pozició
        /// </summary>
        Check,
        /// <summary>
        /// Kivevő pozició
        /// </summary>
        UnLoad,
    }
}
