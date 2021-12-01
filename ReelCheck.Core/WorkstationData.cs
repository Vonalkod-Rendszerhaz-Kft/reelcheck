using System;
using Vrh.Redis.DataPoolHandler;

namespace ReelCheck.Core
{
    /// <summary>
    /// Munkaállomás szintű adatokat tároló osztály
    /// </summary>
    internal class WorkstationData : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workStationType"></param>
        /// <param name="processType"></param>
        /// <param name="instanceWriter"></param>
        public WorkstationData(WorkStationType workStationType, ProcessType processType, InstanceWriter instanceWriter, string id)
        {
            _instanceWriter = instanceWriter;
            _workStationType = workStationType;
            _processType = processType;
            _stationId = id;
            WriteToCache(DataPoolDefinition.WS.Station, _stationId);
            WriteToCache(DataPoolDefinition.WS.StationType, _workStationType);
            WriteToCache(DataPoolDefinition.WS.Process, _processType);
            WriteToCache(DataPoolDefinition.WS.MainStatus, MainStatus.Auto);
            WriteToCache(DataPoolDefinition.WS.UserName, string.Empty);
            WriteToCache(DataPoolDefinition.WS.IdStatus, OperationStatus.Ready);
            WriteToCache(DataPoolDefinition.WS.PrintStatus, OperationStatus.Ready);
            WriteToCache(DataPoolDefinition.WS.CheckStatus, OperationStatus.Ready);
            WriteToCache(DataPoolDefinition.WS.ReelCheckMode, ReelCheckMode.WaitLogin);
            WriteToCache(DataPoolDefinition.WS.ManualData, string.Empty);
            WriteToCache(DataPoolDefinition.WS.BlockReasonCode, string.Empty);
            WriteToCache(DataPoolDefinition.WS.OperationBlocked, false);
        }

        /// <summary>
        /// Gép azonosító
        /// </summary>
        public string StationId
        {
            get { lock (_locker) { return _stationId; } }
        }
        private string _stationId;

        /// <summary>
        /// Felhasználó neve
        /// </summary>
        public string UserName
        {
            get { lock (_locker) { return _userName; } }
            set
            {
                lock (_locker)
                {
                    _userName = value;
                    WriteToCache(DataPoolDefinition.WS.UserName, _userName);
                }
            }
        }
        private string _userName;

        /// <summary>
        /// Manuális adatbevitellel bevuitt tekercsadat
        /// </summary>
        public string ManualData
        {
            get { lock (_locker) { return _manualData; } }
            set
            {
                lock (_locker)
                {
                    _manualData = value;
                    WriteToCache(DataPoolDefinition.WS.ManualData, value);
                }
            }
        }
        private string _manualData;

        /// <summary>
        /// Jelzi, hogy blokkolva van a működés
        /// </summary>
        public bool OperationBlocked
        {
            get { lock (_locker) { return _operationBlocked; } }
            set
            {
                lock (_locker)
                {
                    _operationBlocked = value;
                    WriteToCache(DataPoolDefinition.WS.OperationBlocked, value);
                }
            }
        }
        private bool _operationBlocked;

        /// <summary>
        /// A blokkolás oka
        /// </summary>
        public BlockReason BlockReasonCode
        {
            get { lock (_locker) { return _blockReasonCode; } }
            set
            {
                lock (_locker)
                {
                    _blockReasonCode = value;
                    WriteToCache(DataPoolDefinition.WS.BlockReasonCode, value);
                }
            }
        }
        private BlockReason _blockReasonCode;

        /// <summary>
        /// Folyamat típus
        /// </summary>
        public ProcessType ProcessType
        {
            get { return _processType; }
        }

        /// <summary>
        /// Munkahely típusa
        /// </summary>
        public WorkStationType WorkStationType
        {
            get { return _workStationType; }
        }

        /// <summary>
        /// Kiírja az adatot a Redis cash-re
        /// </summary>
        /// <typeparam name="T">kiírandó adat típusa</typeparam>
        /// <param name="key">az irandó kulcs</param>
        /// <param name="value">érték</param>
        public void WriteToCache<T>(DataPoolDefinition.WS key, T value)
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    _instanceWriter.WriteKeyValue<string>(key.GetRedisKey(), value.ToString().ToUpper());
                }
                else
                {
                    _instanceWriter.WriteKeyValue<T>(key.GetRedisKey(), value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}: {key}, {value}");
            }
        }

        /// <summary>
        /// Munkaállomás típusa
        /// </summary>
        private WorkStationType _workStationType;

        /// <summary>
        /// Munkafolymat típusa
        /// </summary>
        private ProcessType _processType;

        /// <summary>
        /// Redis DP instance writer
        /// </summary>
        private InstanceWriter _instanceWriter;

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
                    _instanceWriter = null;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WorkstationData() {
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
