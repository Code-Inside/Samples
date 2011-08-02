namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Threading;

    public class MockAsyncResult : IAsyncResult {

        private volatile object _asyncState;
        private readonly ManualResetEvent _asyncWaitHandle = new ManualResetEvent(false);
        private volatile bool _completedSynchronously;
        private volatile bool _isCompleted;

        public object AsyncState {
            get {
                return _asyncState;
            }
            set {
                _asyncState = value;
            }
        }

        public ManualResetEvent AsyncWaitHandle {
            get {
                return _asyncWaitHandle;
            }
        }

        public bool CompletedSynchronously {
            get {
                return _completedSynchronously;
            }
            set {
                _completedSynchronously = value;
            }
        }

        public bool IsCompleted {
            get {
                return _isCompleted;
            }
            set {
                _isCompleted = value;
            }
        }

        #region IAsyncResult Members
        WaitHandle IAsyncResult.AsyncWaitHandle {
            get {
                return _asyncWaitHandle;
            }
        }
        #endregion

    }
}
