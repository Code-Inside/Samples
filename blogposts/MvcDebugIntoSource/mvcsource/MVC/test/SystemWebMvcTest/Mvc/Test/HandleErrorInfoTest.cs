namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HandleErrorInfoTest {

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            Exception exception = new Exception();
            string controller = "SomeController";
            string action = "SomeAction";

            // Act
            HandleErrorInfo viewData = new HandleErrorInfo(exception, controller, action);

            // Assert
            Assert.AreSame(exception, viewData.Exception);
            Assert.AreEqual(controller, viewData.ControllerName);
            Assert.AreEqual(action, viewData.ActionName);
        }

        [TestMethod]
        public void ConstructorWithEmptyActionThrows() {
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new HandleErrorInfo(new Exception(), "SomeController", String.Empty);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorWithEmptyControllerThrows() {
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new HandleErrorInfo(new Exception(), String.Empty, "SomeAction");
                }, "controllerName");
        }

        [TestMethod]
        public void ConstructorWithNullActionThrows() {
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new HandleErrorInfo(new Exception(), "SomeController", null /* action */);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorWithNullControllerThrows() {
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new HandleErrorInfo(new Exception(), null /* controller */, "SomeAction");
                }, "controllerName");
        }

        [TestMethod]
        public void ConstructorWithNullExceptionThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HandleErrorInfo(null /* exception */, "SomeController", "SomeAction");
                }, "exception");
        }

    }
}
