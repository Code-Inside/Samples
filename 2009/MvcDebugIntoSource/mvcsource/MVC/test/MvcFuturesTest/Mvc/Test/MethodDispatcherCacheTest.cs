namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class MethodDispatcherCacheTest {

        [TestMethod]
        public void GetDispatcher() {
            // Arrange
            MethodInfo methodInfo = typeof(object).GetMethod("ToString");
            MethodDispatcherCache cache = new MethodDispatcherCache();

            // Act
            MethodDispatcher dispatcher1 = cache.GetDispatcher(methodInfo);
            MethodDispatcher dispatcher2 = cache.GetDispatcher(methodInfo);

            // Assert
            Assert.AreSame(methodInfo, dispatcher1.MethodInfo);
            Assert.AreSame(dispatcher1, dispatcher2, "Dispatcher was not correctly cached.");
        }

    }
}
