namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ExceptionContextTest {

        [TestMethod]
        public void ConstructorThrowsIfExceptionIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Exception exception = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ExceptionContext(controllerContext, exception);
                }, "exception");
        }

        [TestMethod]
        public void ExceptionProperty() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Exception exception = new Exception();

            // Act
            ExceptionContext exceptionContext = new ExceptionContext(controllerContext, exception);

            // Assert
            Assert.AreEqual(exception, exceptionContext.Exception);
        }

        [TestMethod]
        public void ResultProperty() {
            // Arrange
            ExceptionContext exceptionContext = new Mock<ExceptionContext>().Object;

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(exceptionContext, "Result", new ViewResult(), EmptyResult.Instance);
        }

    }
}
