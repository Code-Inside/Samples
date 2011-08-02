namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class RequireSslAttributeTest {

        [TestMethod]
        public void OnAuthorizationDoesNothingIfRequestIsSecure() {
            // Arrange
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.IsSecureConnection).Returns(true);
            AuthorizationContext authContext = mockAuthContext.Object;

            RequireSslAttribute attr = new RequireSslAttribute();

            // Act
            attr.OnAuthorization(authContext);

            // Assert
            Assert.IsNull(authContext.Result, "Result should not have been set.");
        }

        [TestMethod]
        public void OnAuthorizationRedirectsIfRequestIsNotSecureAndRedirectPropertyIsSet() {
            // Arrange
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.IsSecureConnection).Returns(false);
            mockAuthContext.Expect(c => c.HttpContext.Request.RawUrl).Returns("/alpha/bravo/charlie");
            mockAuthContext.Expect(c => c.HttpContext.Request.Url).Returns(new Uri("http://www.example.com/foo/bar/baz"));
            AuthorizationContext authContext = mockAuthContext.Object;

            RequireSslAttribute attr = new RequireSslAttribute() { Redirect = true };

            // Act
            attr.OnAuthorization(authContext);
            RedirectResult result = authContext.Result as RedirectResult;

            // Assert
            Assert.IsNotNull(result, "Result should have been a RedirectResult.");
            Assert.AreEqual("https://www.example.com/alpha/bravo/charlie", result.Url);
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfFilterContextIsNull() {
            // Arrange
            RequireSslAttribute attr = new RequireSslAttribute();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnAuthorization(null);
                }, "filterContext");
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfRequestIsNotSecureAndRedirectPropertyIsNotSet() {
            // Arrange
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.IsSecureConnection).Returns(false);
            AuthorizationContext authContext = mockAuthContext.Object;

            RequireSslAttribute attr = new RequireSslAttribute();

            // Act & assert
            ExceptionHelper.ExpectHttpException(
                delegate {
                    attr.OnAuthorization(authContext);
                },
                @"Access forbidden. The requested resource requires an SSL connection.",
                (int)HttpStatusCode.Forbidden);
        }

    }
}
