namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Threading;

    public sealed class SignalContainer<T> {

        private volatile object _item;
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false /* initialState */);

        public void Signal(T item) {
            _item = item;
            _waitHandle.Set();
        }

        public T Wait() {
            _waitHandle.WaitOne();
            return (T)_item;
        }

    }
}
