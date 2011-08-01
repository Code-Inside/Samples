namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ResultExecutedContextTest {

        [TestMethod]
        public void ConstructorThrowsIfControllerDescriptorIsNull() {
            // Arrange
            ControllerContext controllerContext = null;
            ActionResult result = new ViewResult();
            bool canceled = true;
            Exception exception = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ResultExecutedContext(controllerContext, result, canceled, exception);
                }, "controllerContext");
        }

        [TestMethod]
        public void ConstructorThrowsIfResultIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionResult result = null;
            bool canceled = true;
            Exception exception = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ResultExecutedContext(controllerContext, result, canceled, exception);
                }, "result");
        }

        [TestMethod]
        public void PropertiesAreSetByConstructor() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionResult result = new ViewResult();
            bool canceled = true;
            Exception exception = new Exception();

            // Act
            ResultExecutedContext resultExecutedContext = new ResultExecutedContext(controllerContext, result, canceled, exception);

            // Assert
            Assert.AreEqual(result, resultExecutedContext.Result);
            Assert.AreEqual(canceled, resultExecutedContext.Canceled);
            Assert.AreEqual(exception, resultExecutedContext.Exception);
        }

    }
}
