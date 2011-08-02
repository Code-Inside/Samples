namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class AcceptAjaxAttributeTest {

        [TestMethod]
        public void IsValidForRequestReturnsFalseIfRequestIsNotAjaxRequest() {
            // Arrange
            AcceptAjaxAttribute attr = new AcceptAjaxAttribute();
            ControllerContext controllerContext = GetControllerContext(false /* isAjaxRequest */);

            // Act
            bool isValid = attr.IsValidForRequest(controllerContext, null);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidForRequestReturnsTrueIfRequestIsAjaxRequest() {
            // Arrange
            AcceptAjaxAttribute attr = new AcceptAjaxAttribute();
            ControllerContext controllerContext = GetControllerContext(true /* isAjaxRequest */);

            // Act
            bool isValid = attr.IsValidForRequest(controllerContext, null);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValidForRequestThrowsIfControllerContextIsNull() {
            // Arrange
            AcceptAjaxAttribute attr = new AcceptAjaxAttribute();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.IsValidForRequest(null, null);
                }, "controllerContext");
        }

        private static ControllerContext GetControllerContext(bool isAjaxRequest) {
            Mock<ControllerContext> mockContext = new Mock<ControllerContext>() { DefaultValue = DefaultValue.Mock };
            if (isAjaxRequest) {
                mockContext.Expect(c => c.HttpContext.Request["X-Requested-With"]).Returns("XMLHttpRequest");
            }
            return mockContext.Object;
        }

    }
}
