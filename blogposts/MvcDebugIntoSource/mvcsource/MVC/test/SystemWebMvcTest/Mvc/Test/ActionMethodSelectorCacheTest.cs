namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionMethodSelectorCacheTest {

        [TestMethod]
        public void GetSelector() {
            // Arrange
            Type controllerType = typeof(object);
            ActionMethodSelectorCache cache = new ActionMethodSelectorCache();

            // Act
            ActionMethodSelector selector1 = cache.GetSelector(controllerType);
            ActionMethodSelector selector2 = cache.GetSelector(controllerType);

            // Assert
            Assert.AreSame(controllerType, selector1.ControllerType);
            Assert.AreSame(selector1, selector2, "Selector was not correctly cached.");
        }

    }
}
