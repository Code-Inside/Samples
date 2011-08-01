namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HandleErrorAttributeTest {

        [TestMethod]
        public void ExceptionTypeProperty() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();

            // Act
            Type origType = attr.ExceptionType;
            attr.ExceptionType = typeof(SystemException);
            Type newType = attr.ExceptionType;

            // Assert
            Assert.AreEqual(typeof(Exception), origType);
            Assert.AreEqual(typeof(SystemException), attr.ExceptionType);
        }

        [TestMethod]
        public void ExceptionTypePropertyWithNonExceptionTypeThrows() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    attr.ExceptionType = typeof(string);
                },
                "The type 'System.String' does not inherit from Exception.");
        }

        [TestMethod]
        public void ExceptionTypePropertyWithNullValueThrows() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.ExceptionType = null;
                }, "value");
        }

        [TestMethod]
        public void MasterProperty() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();

            // Act & Assert
            MemberHelper.TestStringProperty(attr, "Master", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void OnException() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute() {
                View = "SomeView",
                Master = "SomeMaster",
                ExceptionType = typeof(ArgumentException) 
            };
            Exception exception = new ArgumentNullException();

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            mockHttpContext.Expect(c => c.IsCustomErrorEnabled).Returns(true);
            mockHttpContext.Expect(c => c.Session).Returns((HttpSessionStateBase)null);
            mockHttpContext.Expect(c => c.Response.Clear()).Verifiable();
            mockHttpContext.ExpectSet(c => c.Response.StatusCode, 500).Verifiable();
            mockHttpContext.ExpectSet(c => c.Response.TrySkipIisCustomErrors, true).Verifiable(); 
            
            TempDataDictionary tempData = new TempDataDictionary();
            IViewEngine viewEngine = new Mock<IViewEngine>().Object;
            Controller controller = new Mock<Controller>().Object;
            controller.TempData = tempData;

            ExceptionContext context = GetExceptionContext(mockHttpContext.Object, controller, exception);

            // Exception
            attr.OnException(context);

            // Assert
            mockHttpContext.Verify();
            ViewResult viewResult = context.Result as ViewResult;
            Assert.IsNotNull(viewResult, "The Result property should have been set to an instance of ViewResult.");
            Assert.AreEqual(tempData, viewResult.TempData);
            Assert.AreEqual("SomeView", viewResult.ViewName);
            Assert.AreEqual("SomeMaster", viewResult.MasterName);

            HandleErrorInfo viewData = viewResult.ViewData.Model as HandleErrorInfo;
            Assert.IsNotNull(viewData, "The ViewData model should have been set to an instance of ExceptionViewData.");
            Assert.AreSame(exception, viewData.Exception);
            Assert.AreEqual("SomeController", viewData.ControllerName);
            Assert.AreEqual("SomeAction", viewData.ActionName);
        }

        [TestMethod]
        public void OnExceptionWithCustomErrorsDisabledDoesNothing() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();
            ActionResult result = new EmptyResult();
            ExceptionContext context = GetExceptionContext(GetHttpContext(false), null, new Exception());
            context.Result = result;

            // Exception
            attr.OnException(context);

            // Assert
            Assert.AreSame(result, context.Result, "The context's Result property should have remain unchanged.");
        }

        [TestMethod]
        public void OnExceptionWithExceptionHandledDoesNothing() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();
            ActionResult result = new EmptyResult();
            ExceptionContext context = GetExceptionContext(GetHttpContext(), null, new Exception());
            context.Result = result;
            context.ExceptionHandled = true;

            // Exception
            attr.OnException(context);

            // Assert
            Assert.AreSame(result, context.Result, "The context's Result property should have remain unchanged.");
        }

        [TestMethod]
        public void OnExceptionWithNon500ExceptionDoesNothing() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();
            ActionResult result = new EmptyResult();
            ExceptionContext context = GetExceptionContext(GetHttpContext(), null, new HttpException(404, "Some Exception"));
            context.Result = result;

            // Exception
            attr.OnException(context);

            // Assert
            Assert.AreSame(result, context.Result, "The context's Result property should have remain unchanged.");
        }

        [TestMethod]
        public void OnExceptionWithNullFilterContextThrows() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnException(null /* filterContext */);
                }, "filterContext");
        }

        [TestMethod]
        public void OnExceptionWithWrongExceptionTypeDoesNothing() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute() { ExceptionType = typeof(ArgumentException) };
            ActionResult result = new EmptyResult();
            ExceptionContext context = GetExceptionContext(GetHttpContext(), null, new InvalidCastException());
            context.Result = result;

            // Exception
            attr.OnException(context);

            // Assert
            Assert.AreSame(result, context.Result, "The context's Result property should have remain unchanged.");
        }

        [TestMethod]
        public void ViewProperty() {
            // Arrange
            HandleErrorAttribute attr = new HandleErrorAttribute();

            // Act & Assert
            MemberHelper.TestStringProperty(attr, "View", "Error", false /* testDefaultValue */, true /* allowNullAndEmpty */, "Error");
        }

        private static ExceptionContext GetExceptionContext(HttpContextBase httpContext, ControllerBase controller, Exception exception) {
            RouteData rd = new RouteData();
            rd.Values["controller"] = "SomeController";
            rd.Values["action"] = "SomeAction";

            Mock<ExceptionContext> mockExceptionContext = new Mock<ExceptionContext>();
            mockExceptionContext.Expect(c => c.Controller).Returns(controller);
            mockExceptionContext.Expect(c => c.Exception).Returns(exception);
            mockExceptionContext.Expect(c => c.RouteData).Returns(rd);
            mockExceptionContext.Expect(c => c.HttpContext).Returns(httpContext);
            return mockExceptionContext.Object;
        }

        private static HttpContextBase GetHttpContext() {
            return GetHttpContext(true);
        }

        private static HttpContextBase GetHttpContext(bool isCustomErrorEnabled) {
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            mockContext.Expect(c => c.IsCustomErrorEnabled).Returns(isCustomErrorEnabled);
            return mockContext.Object;
        }

    }
}
