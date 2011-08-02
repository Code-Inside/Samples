namespace System.Web.Mvc.Test {
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ValidateInputAttributeTest {

        [TestMethod]
        public void EnableValidationProperty() {
            // Act
            ValidateInputAttribute attrTrue = new ValidateInputAttribute(true);
            ValidateInputAttribute attrFalse = new ValidateInputAttribute(false);

            // Assert
            Assert.IsTrue(attrTrue.EnableValidation);
            Assert.IsFalse(attrFalse.EnableValidation);
        }

        [TestMethod]
        public void OnAuthorizationSetsControllerValidateRequestToFalse() {
            // Arrange
            Controller controller = new EmptyController() { ValidateRequest = true };
            ValidateInputAttribute attr = new ValidateInputAttribute(false /* enableValidation */);
            AuthorizationContext authContext = GetAuthorizationContext(controller);

            // Act
            attr.OnAuthorization(authContext);

            // Assert
            Assert.IsFalse(controller.ValidateRequest);
        }

        [TestMethod]
        public void OnAuthorizationSetsControllerValidateRequestToTrue() {
            // Arrange
            Controller controller = new EmptyController() { ValidateRequest = false };
            ValidateInputAttribute attr = new ValidateInputAttribute(true /* enableValidation */);
            AuthorizationContext authContext = GetAuthorizationContext(controller);

            // Act
            attr.OnAuthorization(authContext);

            // Assert
            Assert.IsTrue(controller.ValidateRequest);
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfFilterContextIsNull() {
            // Arrange
            ValidateInputAttribute attr = new ValidateInputAttribute(true);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnAuthorization(null);
                }, "filterContext");
        }

        private static AuthorizationContext GetAuthorizationContext(ControllerBase controller) {
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.Controller).Returns(controller);
            return mockAuthContext.Object;
        }

        private class EmptyController : Controller {
        }

    }
}
