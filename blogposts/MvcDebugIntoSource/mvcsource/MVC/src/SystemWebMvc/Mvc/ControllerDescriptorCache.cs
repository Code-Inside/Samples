namespace System.Web.Mvc {
    using System;

    internal sealed class ControllerDescriptorCache : ReaderWriterCache<Type, ControllerDescriptor> {

        public ControllerDescriptorCache() {
        }

        public ControllerDescriptor GetDescriptor(Type controllerType) {
            return FetchOrCreateItem(controllerType, () => new ReflectedControllerDescriptor(controllerType));
        }

    }
}
