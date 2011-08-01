namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ActionExecutedContextTest {

        [TestMethod]
        public void ConstructorThrowsIfActionDescriptorIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionDescriptor actionDescriptor = null;
            bool canceled = true;
            Exception exception = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ActionExecutedContext(controllerContext, actionDescriptor, canceled, exception);
                }, "actionDescriptor");
        }

        [TestMethod]
        public void PropertiesAreSetByConstructor() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionDescriptor actionDescriptor = new Mock<ActionDescriptor>().Object;
            bool canceled = true;
            Exception exception = new Exception() ;

            // Act
            ActionExecutedContext actionExecutedContext = new ActionExecutedContext(controllerContext, actionDescriptor, canceled,exception);

            // Assert
            Assert.AreEqual(actionDescriptor, actionExecutedContext.ActionDescriptor);
            Assert.AreEqual(canceled, actionExecutedContext.Canceled);
            Assert.AreEqual(exception, actionExecutedContext.Exception);
        }

        [TestMethod]
        public void ResultProperty() {
            // Arrange
            ActionExecutedContext actionExecutedContext = new Mock<ActionExecutedContext>().Object;

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(actionExecutedContext, "Result", new ViewResult(), EmptyResult.Instance);
        }

    }
}
