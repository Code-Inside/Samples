namespace Microsoft.Web.Mvc {
    using System;
    using System.Threading;

    // This class is used to generate a Begin/End wrapper that returns a known object.

    // Default implementation uses built-in BeginInvoke / EndInvoke - we need to check performance
    // of these built-in methods.
    public sealed class ObjectAsyncResult<T> {

        private readonly Func<T> _itemThunk;
        private readonly SynchronizationContext _syncContext = SynchronizationContextHelper.GetSynchronizationContext();

        public ObjectAsyncResult(T item) {
            _itemThunk = () => item;
        }

        public IAsyncResult BeginInvoke(AsyncCallback callback, object state) {
            AsyncCallback wrappedCallback = (callback != null)
                ? ar => _syncContext.Sync(() => callback(ar))
                : (AsyncCallback)null;

            return _itemThunk.BeginInvoke(wrappedCallback, state);
        }

        public T EndInvoke(IAsyncResult asyncResult) {
            return _itemThunk.EndInvoke(asyncResult);
        }

        public IAsyncResult ToAsyncResultWrapper(AsyncCallback callback, object state) {
            return AsyncResultWrapper.Wrap<T>(callback, state, BeginInvoke, EndInvoke);
        }

    }
}
