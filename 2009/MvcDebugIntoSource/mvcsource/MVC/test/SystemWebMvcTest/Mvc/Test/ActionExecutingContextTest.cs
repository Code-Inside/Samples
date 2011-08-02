namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ActionExecutingContextTest {

        [TestMethod]
        public void ConstructorThrowsIfActionDescriptorIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionDescriptor actionDescriptor = null;
            Dictionary<string, object> actionParameters = new Dictionary<string, object>();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ActionExecutingContext(controllerContext, actionDescriptor, actionParameters);
                }, "actionDescriptor");
        }

        [TestMethod]
        public void ConstructorThrowsIfActionParametersIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionDescriptor actionDescriptor = new Mock<ActionDescriptor>().Object;
            Dictionary<string, object> actionParameters = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ActionExecutingContext(controllerContext, actionDescriptor, actionParameters);
                }, "actionParameters");
        }

        [TestMethod]
        public void PropertiesAreSetByConstructor() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionDescriptor actionDescriptor = new Mock<ActionDescriptor>().Object;
            Dictionary<string, object> actionParameters = new Dictionary<string, object>();

            // Act
            ActionExecutingContext actionExecutingContext = new ActionExecutingContext(controllerContext, actionDescriptor, actionParameters);

            // Assert
            Assert.AreEqual(actionDescriptor, actionExecutingContext.ActionDescriptor);
            Assert.AreEqual(actionParameters, actionExecutingContext.ActionParameters);
        }

    }
}
