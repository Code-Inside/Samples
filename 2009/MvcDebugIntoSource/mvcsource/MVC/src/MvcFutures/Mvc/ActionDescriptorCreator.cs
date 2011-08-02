namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;

    internal delegate ActionDescriptor ActionDescriptorCreator(string actionName, ControllerDescriptor controllerDescriptor);

}