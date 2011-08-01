namespace System.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AuthorizeAttributeTest {

        [TestMethod]
        public void AuthorizeCoreReturnsFalseIfNameDoesNotMatch() {
            // Arrange
            AuthorizeAttributeHelper helper = new AuthorizeAttributeHelper() { Users = "SomeName" };

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.User.Identity.IsAuthenticated).Returns(true);
            mockHttpContext.Expect(c => c.User.Identity.Name).Returns("SomeOtherName");

            // Act
            bool retVal = helper.PublicAuthorizeCore(mockHttpContext.Object);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void AuthorizeCoreReturnsFalseIfRoleDoesNotMatch() {
            // Arrange
            AuthorizeAttributeHelper helper = new AuthorizeAttributeHelper() { Roles = "SomeRole" };

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.User.Identity.IsAuthenticated).Returns(true);
            mockHttpContext.Expect(c => c.User.IsInRole("SomeRole")).Returns(false).Verifiable();

            // Act
            bool retVal = helper.PublicAuthorizeCore(mockHttpContext.Object);

            // Assert
            Assert.IsFalse(retVal);
            mockHttpContext.Verify();
        }

        [TestMethod]
        public void AuthorizeCoreReturnsFalseIfUserIsUnauthenticated() {
            // Arrange
            AuthorizeAttributeHelper helper = new AuthorizeAttributeHelper();

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.User.Identity.IsAuthenticated).Returns(false);

            // Act
            bool retVal = helper.PublicAuthorizeCore(mockHttpContext.Object);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void AuthorizeCoreReturnsTrueIfUserIsAuthenticatedAndNamesOrRolesSpecified() {
            // Arrange
            AuthorizeAttributeHelper helper = new AuthorizeAttributeHelper() { Users = "SomeUser, SomeOtherUser", Roles = "SomeRole, SomeOtherRole" };

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.User.Identity.IsAuthenticated).Returns(true);
            mockHttpContext.Expect(c => c.User.Identity.Name).Returns("SomeUser");
            mockHttpContext.Expect(c => c.User.IsInRole("SomeRole")).Returns(false).Verifiable();
            mockHttpContext.Expect(c => c.User.IsInRole("SomeOtherRole")).Returns(true).Verifiable();

            // Act
            bool retVal = helper.PublicAuthorizeCore(mockHttpContext.Object);

            // Assert
            Assert.IsTrue(retVal);
            mockHttpContext.Verify();
        }

        [TestMethod]
        public void AuthorizeCoreReturnsTrueIfUserIsAuthenticatedAndNoNamesOrRolesSpecified() {
            // Arrange
            AuthorizeAttributeHelper helper = new AuthorizeAttributeHelper();

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.User.Identity.IsAuthenticated).Returns(true);

            // Act
            bool retVal = helper.PublicAuthorizeCore(mockHttpContext.Object);

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void AuthorizeCoreThrowsIfHttpContextIsNull() {
            // Arrange
            AuthorizeAttributeHelper helper = new AuthorizeAttributeHelper();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    helper.PublicAuthorizeCore((HttpContextBase)null);
                }, "httpContext");
        }

        [TestMethod]
        public void OnAuthorizationCancelsRequestIfUserUnauthorized() {
            // Arrange
            Mock<AuthorizeAttributeHelper> mockHelper = new Mock<AuthorizeAttributeHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicAuthorizeCore(It.IsAny<HttpContextBase>())).Returns(false);
            AuthorizeAttributeHelper helper = mockHelper.Object;

            AuthorizationContext filterContext = new Mock<AuthorizationContext>() { DefaultValue = DefaultValue.Mock }.Object;

            // Act
            helper.OnAuthorization(filterContext);

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(HttpUnauthorizedResult));
        }

        [TestMethod]
        public void OnAuthorizationHooksCacheValidationIfUserAuthorized() {
            // Arrange
            Mock<AuthorizeAttributeHelper> mockHelper = new Mock<AuthorizeAttributeHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicAuthorizeCore(It.IsAny<HttpContextBase>())).Returns(true);
            AuthorizeAttributeHelper helper = mockHelper.Object;

            MethodInfo callbackMethod = typeof(AuthorizeAttribute).GetMethod("CacheValidateHandler", BindingFlags.Instance | BindingFlags.NonPublic);
            Mock<AuthorizationContext> mockFilterContext = new Mock<AuthorizationContext>();
            mockFilterContext.Expect(c => c.HttpContext.Response.Cache.SetProxyMaxAge(new TimeSpan(0))).Verifiable();
            mockFilterContext
                .Expect(c => c.HttpContext.Response.Cache.AddValidationCallback(It.IsAny<HttpCacheValidateHandler>(), null /* data */))
                .Callback(
                    delegate(HttpCacheValidateHandler handler, object data) {
                        Assert.AreEqual(helper, handler.Target);
                        Assert.AreEqual(callbackMethod, handler.Method);
                    })
                .Verifiable();
            AuthorizationContext filterContext = mockFilterContext.Object;


            // Act
            helper.OnAuthorization(filterContext);

            // Assert
            mockFilterContext.Verify();
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfFilterContextIsNull() {
            // Arrange
            AuthorizeAttribute attr = new AuthorizeAttribute();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnAuthorization(null);
                }, "filterContext");
        }

        [TestMethod]
        public void OnCacheAuthorizationReturnsIgnoreRequestIfUserIsUnauthorized() {
            // Arrange
            Mock<AuthorizeAttributeHelper> mockHelper = new Mock<AuthorizeAttributeHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicAuthorizeCore(It.IsAny<HttpContextBase>())).Returns(false);
            AuthorizeAttributeHelper helper = mockHelper.Object;

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.User).Returns(new Mock<IPrincipal>().Object);

            // Act
            HttpValidationStatus validationStatus = helper.PublicOnCacheAuthorization(mockHttpContext.Object);

            // Assert
            Assert.AreEqual(HttpValidationStatus.IgnoreThisRequest, validationStatus);
        }

        [TestMethod]
        public void OnCacheAuthorizationReturnsValidIfUserIsAuthorized() {
            // Arrange
            Mock<AuthorizeAttributeHelper> mockHelper = new Mock<AuthorizeAttributeHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicAuthorizeCore(It.IsAny<HttpContextBase>())).Returns(true);
            AuthorizeAttributeHelper helper = mockHelper.Object;

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.User).Returns(new Mock<IPrincipal>().Object);

            // Act
            HttpValidationStatus validationStatus = helper.PublicOnCacheAuthorization(mockHttpContext.Object);

            // Assert
            Assert.AreEqual(HttpValidationStatus.Valid, validationStatus);
        }

        [TestMethod]
        public void OnCacheAuthorizationThrowsIfHttpContextIsNull() {
            // Arrange
            AuthorizeAttributeHelper helper = new AuthorizeAttributeHelper();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    helper.PublicOnCacheAuthorization(null);
                }, "httpContext");
        }

        [TestMethod]
        public void RolesProperty() {
            // Arrange
            AuthorizeAttribute attr = new AuthorizeAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "Roles", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void UsersProperty() {
            // Arrange
            AuthorizeAttribute attr = new AuthorizeAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "Users", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        public class AuthorizeAttributeHelper : AuthorizeAttribute {
            public virtual bool PublicAuthorizeCore(HttpContextBase httpContext) {
                return base.AuthorizeCore(httpContext);
            }
            protected override bool AuthorizeCore(HttpContextBase httpContext) {
                return PublicAuthorizeCore(httpContext);
            }
            public virtual HttpValidationStatus PublicOnCacheAuthorization(HttpContextBase httpContext) {
                return base.OnCacheAuthorization(httpContext);
            }
            protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext) {
                return PublicOnCacheAuthorization(httpContext);
            }
        }

    }
}
