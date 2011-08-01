namespace Microsoft.Web.Mvc {
    using System;
    using System.Web;
    using System.Web.Mvc;

    internal sealed class AsyncControllerDescriptorCache : ReaderWriterCache<Type, ControllerDescriptor> {

        public AsyncControllerDescriptorCache() {
        }

        public ControllerDescriptor GetDescriptor(Type controllerType) {
            return FetchOrCreateItem(controllerType, () => new ReflectedAsyncControllerDescriptor(controllerType));
        }

    }
}
