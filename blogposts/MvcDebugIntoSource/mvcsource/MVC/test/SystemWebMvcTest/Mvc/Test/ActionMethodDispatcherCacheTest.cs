namespace System.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionMethodDispatcherCacheTest {

        [TestMethod]
        public void GetDispatcher() {
            // Arrange
            MethodInfo methodInfo = typeof(object).GetMethod("ToString");
            ActionMethodDispatcherCache cache = new ActionMethodDispatcherCache();

            // Act
            ActionMethodDispatcher dispatcher1 = cache.GetDispatcher(methodInfo);
            ActionMethodDispatcher dispatcher2 = cache.GetDispatcher(methodInfo);

            // Assert
            Assert.AreSame(methodInfo, dispatcher1.MethodInfo);
            Assert.AreSame(dispatcher1, dispatcher2, "Dispatcher was not correctly cached.");
        }

    }
}
