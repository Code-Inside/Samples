namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class AsyncControllerDescriptorCacheTest {

        [TestMethod]
        public void GetDescriptor() {
            // Arrange
            Type controllerType = typeof(object);
            AsyncControllerDescriptorCache cache = new AsyncControllerDescriptorCache();

            // Act
            ControllerDescriptor descriptor1 = cache.GetDescriptor(controllerType);
            ControllerDescriptor descriptor2 = cache.GetDescriptor(controllerType);

            // Assert
            Assert.AreSame(controllerType, descriptor1.ControllerType, "ControllerType was incorrect.");
            Assert.AreSame(descriptor1, descriptor2, "Selector was not correctly cached.");
        }

    }
}
