using System;
using VRH.Common;
using Vrh.Redis.DataPoolHandler;

namespace ReelCheck.Core
{
    internal class UIMessages : IDisposable
    {
        public UIMessages(ushort messageCapacity, InstanceWriter instanceWriter)
        {
            _messages = new FixStack<string>(messageCapacity);
            _instanceWriter = instanceWriter;            
            WriteToCache();
        }

        public void DropMessage(string message)
        {
            _messages.DropItem(message);
            WriteToCache();
        }

        private void WriteToCache()
        {
            for (int i = 0; i < _messages.Capacity; i++)
            {
                OneData od = new OneData()
                {
                    DataKey = $"WS.UIMESSAGE{i + 1}",
                    FieldType = DataType.String,
                    Value = i < _messages.Items.Count ? $"{_messages.Items[i]}" : "",
                };
                _instanceWriter.WriteKeyValue(od);
            }
            return;
        }

        private InstanceWriter _instanceWriter;

        private FixStack<string> _messages;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    _instanceWriter = null;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UIMessages() {
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
