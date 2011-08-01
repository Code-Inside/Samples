namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Web.Resources;

    public class AsyncManager {

        // default timeout is 30 sec
        private int _timeout = 30 * 1000;

        public AsyncManager()
            : this(null /* syncContext */) {
        }

        public AsyncManager(SynchronizationContext syncContext) {
            SynchronizationContext = syncContext ?? SynchronizationContextHelper.GetSynchronizationContext();
            OutstandingOperations = new OutstandingAsyncOperations();
            Parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public OutstandingAsyncOperations OutstandingOperations {
            get;
            private set;
        }

        public IDictionary<string, object> Parameters {
            get;
            private set;
        }

        public SynchronizationContext SynchronizationContext {
            get;
            private set;
        }

        public void Post(Action d) {
            SynchronizationContext.Sync(d);
        }

        public void RegisterTask(Action<AsyncCallback> beginDelegate, AsyncCallback endDelegate) {
            if (beginDelegate == null) {
                throw new ArgumentNullException("beginDelegate");
            }

            RegisterTaskInternal(
                callback => {
                    beginDelegate(callback);
                    return null;
                },
                endDelegate);
        }

        public IAsyncResult RegisterTask(Func<AsyncCallback, IAsyncResult> beginDelegate, AsyncCallback endDelegate) {
            if (beginDelegate == null) {
                throw new ArgumentNullException("beginDelegate");
            }

            return RegisterTaskInternal(beginDelegate, endDelegate);
        }

        private IAsyncResult RegisterTaskInternal(Func<AsyncCallback, IAsyncResult> beginDelegate, AsyncCallback endDelegate) {
            AsyncCallback callback = ar => {
                Send(() => {
                    if (endDelegate != null) {
                        endDelegate(ar);
                    }
                    OutstandingOperations.Decrement();
                });
            };

            OutstandingOperations.Increment();
            return beginDelegate(callback);
        }

        public void Send(Action d) {
            SynchronizationContext.Sync(d);
        }

        // measured in milliseconds, Timeout.Infinite means 'no timeout'
        public int Timeout {
            get {
                return _timeout;
            }
            set {
                if (value < -1) {
                    throw new ArgumentOutOfRangeException("value", MvcResources.AsyncCommon_TimeoutMustBeNonNegativeOrInfinite);
                }
                _timeout = value;
            }
        }

    }
}
