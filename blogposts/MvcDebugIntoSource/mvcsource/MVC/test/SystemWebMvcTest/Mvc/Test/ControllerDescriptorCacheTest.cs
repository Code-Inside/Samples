namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ControllerDescriptorCacheTest {

        [TestMethod]
        public void GetDescriptor() {
            // Arrange
            Type controllerType = typeof(object);
            ControllerDescriptorCache cache = new ControllerDescriptorCache();

            // Act
            ControllerDescriptor descriptor1 = cache.GetDescriptor(controllerType);
            ControllerDescriptor descriptor2 = cache.GetDescriptor(controllerType);

            // Assert
            Assert.AreSame(controllerType, descriptor1.ControllerType, "ControllerType was incorrect.");
            Assert.AreSame(descriptor1, descriptor2, "Selector was not correctly cached.");
        }

    }
}
