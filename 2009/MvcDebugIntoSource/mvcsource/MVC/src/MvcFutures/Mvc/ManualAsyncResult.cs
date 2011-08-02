namespace Microsoft.Web.Mvc {
    using System;
    using System.Threading;

    internal sealed class ManualAsyncResult : IAsyncResult {

        private ManualResetEvent _asyncWaitHandle;
        private volatile bool _asyncWaitHandleInitialState;
        private volatile bool _isCompleted;

        public object AsyncState {
            get;
            set;
        }

        public WaitHandle AsyncWaitHandle {
            get {
                ManualResetEvent waitHandle = Interlocked.CompareExchange(ref _asyncWaitHandle, null, null);
                if (waitHandle == null) {
                    ManualResetEvent newWaitHandle = new ManualResetEvent(_asyncWaitHandleInitialState);
                    waitHandle = Interlocked.CompareExchange(ref _asyncWaitHandle, newWaitHandle, null) ?? newWaitHandle;
                }
                return waitHandle;
            }
        }

        public bool CompletedSynchronously {
            get;
            private set;
        }

        public bool IsCompleted {
            get {
                return _isCompleted;
            }
            private set {
                _isCompleted = value;
            }
        }

        // Proper order of execution:
        // 1. Set the CompletedSynchronously property to the correct value
        // 2. Set the IsCompleted flag
        // 3. Execute the callback
        // 4. Signal the WaitHandle
        public void MarkCompleted(bool completedSynchronously, AsyncCallback callback) {
            CompletedSynchronously = completedSynchronously;

            IsCompleted = true;
            if (callback != null) {
                callback(this);
            }

            // set wait handle
            _asyncWaitHandleInitialState = true;
            ManualResetEvent waitHandle = Interlocked.CompareExchange(ref _asyncWaitHandle, null, null);
            if (waitHandle != null) {
                waitHandle.Set();
            }
        }

    }
}
