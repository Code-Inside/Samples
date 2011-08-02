namespace Microsoft.Web.Mvc {
    using System;
    using System.Reflection;

    internal sealed class MethodDispatcherCache : ReaderWriterCache<MethodInfo, MethodDispatcher> {

        public MethodDispatcherCache() {
        }

        public MethodDispatcher GetDispatcher(MethodInfo methodInfo) {
            return FetchOrCreateItem(methodInfo, () => new MethodDispatcher(methodInfo));
        }

    }
}
