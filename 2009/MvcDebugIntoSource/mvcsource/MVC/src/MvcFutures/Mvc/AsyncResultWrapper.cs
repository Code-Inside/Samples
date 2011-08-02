namespace Microsoft.Web.Mvc {
    using System;
    using System.Threading;
    using Microsoft.Web.Resources;

    // This class is used for the following pattern:

    // public IAsyncResult BeginInner(..., callback, state);
    // public TInnerResult EndInner(asyncResult);
    // public IAsyncResult BeginOuter(..., callback, state);
    // public TOuterResult EndOuter(asyncResult);

    // That is, the EndOuter() method needs to perform some post-processing of the data returned by EndInner().

    public static class AsyncResultWrapper {

        private static AsyncCallback<object> MakeGenericCallback(AsyncCallback callback) {
            return (ar) => {
                callback(ar);
                return null;
            };
        }

        // Executes the endCallback passed to Wrap() and returns the value in that callback.

        public static TResult UnwrapAndContinue<TResult>(IAsyncResult asyncResult) {
            return UnwrapAndContinue<TResult>(asyncResult, null /* tag */);
        }

        public static TResult UnwrapAndContinue<TResult>(IAsyncResult asyncResult, object tag) {
            if (asyncResult == null) {
                throw new ArgumentNullException("asyncResult");
            }

            AsyncResultWrapperImpl<TResult> wrapper = asyncResult as AsyncResultWrapperImpl<TResult>;
            if (wrapper == null || wrapper.Tag != tag) {
                throw new ArgumentException(MvcResources.AsyncCommon_InvalidAsyncResult, "asyncResult");
            }

            return wrapper.ExecuteEndDelegateOnce();
        }

        public static void UnwrapAndContinue(IAsyncResult asyncResult) {
            UnwrapAndContinue(asyncResult, null /* tag */);
        }

        public static void UnwrapAndContinue(IAsyncResult asyncResult, object tag) {
            UnwrapAndContinue<object>(asyncResult, tag);
        }

        private static void ValidateParameters(object beginCallback, object endCallback, int timeout) {
            if (beginCallback == null) {
                throw new ArgumentNullException("beginCallback");
            }
            if (endCallback == null) {
                throw new ArgumentNullException("endCallback");
            }
            if (timeout < -1) {
                throw new ArgumentOutOfRangeException("timeout", MvcResources.AsyncCommon_TimeoutMustBeNonNegativeOrInfinite);
            }
        }

        public static IAsyncResult Wrap<TResult>(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback<TResult> endCallback) {
            return WrapWithTimeout(callback, state, beginCallback, endCallback, Timeout.Infinite, null /* tag */);
        }

        public static IAsyncResult Wrap<TResult>(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback<TResult> endCallback, object tag) {
            return WrapWithTimeout(callback, state, beginCallback, endCallback, Timeout.Infinite, tag);
        }

        public static IAsyncResult Wrap(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback endCallback) {
            return WrapWithTimeout(callback, state, beginCallback, endCallback, Timeout.Infinite, null /* tag */);
        }

        public static IAsyncResult Wrap(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback endCallback, object tag) {
            return WrapWithTimeout(callback, state, beginCallback, endCallback, Timeout.Infinite, tag);
        }

        public static IAsyncResult WrapWithTimeout<TResult>(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback<TResult> endCallback, int timeout) {
            return WrapWithTimeout(callback, state, beginCallback, endCallback, timeout, null /* tag */);
        }

        public static IAsyncResult WrapWithTimeout<TResult>(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback<TResult> endCallback, int timeout, object tag) {
            ValidateParameters(beginCallback, endCallback, timeout);
            return WrapWithTimeoutInternal(callback, state, beginCallback, endCallback, timeout, tag);
        }

        public static IAsyncResult WrapWithTimeout(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback endCallback, int timeout) {
            return WrapWithTimeout(callback, state, beginCallback, endCallback, timeout, null /* tag */);
        }

        public static IAsyncResult WrapWithTimeout(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback endCallback, int timeout, object tag) {
            ValidateParameters(beginCallback, endCallback, timeout);
            return WrapWithTimeoutInternal(callback, state, beginCallback, MakeGenericCallback(endCallback), timeout, tag);
        }

        private static IAsyncResult WrapWithTimeoutInternal<TResult>(AsyncCallback callback, object state, BeginInvokeCallback beginCallback, AsyncCallback<TResult> endCallback, int timeout, object tag) {
            AsyncResultWrapperImpl<TResult> wrapper = new AsyncResultWrapperImpl<TResult>() {
                Callback = callback,
                AsyncState = state,
                BeginCallback = beginCallback,
                EndCallback = endCallback,
                Tag = tag,
                Timeout = timeout
            };

            wrapper.ExecuteBeginDelegate();
            return wrapper;
        }

        private class AsyncResultWrapperImpl<TResult> : IAsyncResult {

            private readonly SingleFireEvent _executeEndDelegateOnceEvent = new SingleFireEvent();
            private readonly SingleFireEvent _executeProvidedCallbackDelegateEvent = new SingleFireEvent();
            private volatile IAsyncResult _innerAsyncResult;
            private readonly SynchronizationContext _syncContext = SynchronizationContextHelper.GetSynchronizationContext();
            private volatile bool _timedOut;
            private volatile Timer _timer;
            private readonly object _timerLockObj = new object();
            private volatile bool _timerDisposed;

            public object AsyncState {
                get;
                set;
            }

            public WaitHandle AsyncWaitHandle {
                get {
                    return _innerAsyncResult.AsyncWaitHandle;
                }
            }

            public BeginInvokeCallback BeginCallback {
                get;
                set;
            }

            public AsyncCallback Callback {
                get;
                set;
            }

            public bool CompletedSynchronously {
                get {
                    return _innerAsyncResult.CompletedSynchronously;
                }
            }

            public AsyncCallback<TResult> EndCallback {
                get;
                set;
            }

            public bool IsCompleted {
                get {
                    return _innerAsyncResult.IsCompleted;
                }
            }

            public object Tag {
                get;
                set;
            }

            public int Timeout {
                get;
                set;
            }

            // the object is "baked" once this method is called
            public void ExecuteBeginDelegate() {
                _innerAsyncResult = BeginCallback(HandleBeginDelegateCallback, null /* state */);

                // only create a timer object if a timeout period has been specified
                if (Timeout > System.Threading.Timeout.Infinite) {
                    Timer timer = new Timer(HandleTimeout);
                    _timer = timer;
                    lock (_timerLockObj) {
                        if (!_timerDisposed) {
                            timer.Change(Timeout, System.Threading.Timeout.Infinite /* disable periodic signaling */);
                        }
                    }
                }
            }

            public TResult ExecuteEndDelegateOnce() {
                if (!_executeEndDelegateOnceEvent.Signal()) {
                    throw new ArgumentException(MvcResources.AsyncCommon_ResultAlreadyExecuted, "asyncResult");
                }

                Timer timer = _timer;
                if (timer != null) {
                    lock (_timerLockObj) {
                        _timerDisposed = true;
                        timer.Dispose();
                    }
                }
                if (_timedOut) {
                    throw new TimeoutException();
                }

                return _syncContext.Sync(() => EndCallback(_innerAsyncResult));
            }

            private void ExecuteProvidedCallbackDelegate(bool timedOut) {
                // want this method to execute only once
                if (_executeProvidedCallbackDelegateEvent.Signal()) {
                    _timedOut = timedOut;
                    if (Callback != null) {
                        _syncContext.Sync(() => Callback(this));
                    }
                }
            }

            private void HandleBeginDelegateCallback(IAsyncResult asyncResult) {
                _innerAsyncResult = asyncResult;
                ExecuteProvidedCallbackDelegate(false /* timedOut */);
            }

            private void HandleTimeout(object state) {
                ExecuteProvidedCallbackDelegate(true /* timedOut */);
            }

        }

    }
}
