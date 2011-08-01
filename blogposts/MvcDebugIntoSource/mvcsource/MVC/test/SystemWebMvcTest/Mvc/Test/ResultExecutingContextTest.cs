namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ResultExecutingContextTest {

        [TestMethod]
        public void ConstructorThrowsIfControllerContextIsNull() {
            // Arrange
            ControllerContext controllerContext = null;
            ActionResult result = new ViewResult();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ResultExecutingContext(controllerContext, result);
                }, "controllerContext");
        }

        [TestMethod]
        public void ConstructorThrowsIfResultIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionResult result = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ResultExecutingContext(controllerContext, result);
                }, "result");
        }

        [TestMethod]
        public void ResultProperty() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionResult result = new ViewResult();

            // Act
            ResultExecutingContext resultExecutingContext = new ResultExecutingContext(controllerContext, result);

            // Assert
            Assert.AreEqual(result, resultExecutingContext.Result);
        }

    }
}
