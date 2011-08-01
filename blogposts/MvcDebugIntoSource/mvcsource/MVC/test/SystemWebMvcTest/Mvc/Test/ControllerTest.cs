namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Security.Principal;
    using System.Text;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Protected;

    [TestClass]
    public class ControllerTest {

        [TestMethod]
        public void ActionInvokerProperty() {
            // Arrange
            Controller controller = new EmptyController();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(controller, "ActionInvoker", new ControllerActionInvoker());
        }

        [TestMethod]
        public void ContentWithContentString() {
            // Arrange
            Controller controller = new EmptyController();
            string content = "Some content";

            // Act
            ContentResult result = controller.Content(content);

            // Assert
            Assert.AreEqual(content, result.Content);
        }

        public void ContentWithContentStringAndContentType() {
            // Arrange
            Controller controller = new EmptyController();
            string content = "Some content";
            string contentType = "Some content type";

            // Act
            ContentResult result = controller.Content(content, contentType);

            // Assert
            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(contentType, result.ContentType);
        }

        public void ContentWithContentStringAndContentTypeAndEncoding() {
            // Arrange
            Controller controller = new EmptyController();
            string content = "Some content";
            string contentType = "Some content type";
            Encoding contentEncoding = Encoding.UTF8;

            // Act
            ContentResult result = controller.Content(content, contentType, contentEncoding);

            // Assert
            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreSame(contentEncoding, result.ContentEncoding);
        }

        [TestMethod]
        public void ContextProperty() {
            var controller = new EmptyController();
            MemberHelper.TestPropertyValue(controller, "ControllerContext", new Mock<ControllerContext>().Object);
        }

        [TestMethod]
        public void HttpContextProperty() {
            var c = new EmptyController();
            Assert.IsNull(c.HttpContext, "Property should be null before Context is set");

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(cc => cc.Controller).Returns(c);
            mockControllerContext.Expect(cc => cc.HttpContext).Returns(mockHttpContext.Object);

            c.ControllerContext = mockControllerContext.Object;
            Assert.AreEqual<HttpContextBase>(mockHttpContext.Object, c.HttpContext, "Property should equal the value on the Context.");
        }

        [TestMethod]
        public void ModelStateProperty() {
            // Arrange
            Controller controller = new EmptyController();

            // Act & assert
            Assert.AreSame(controller.ViewData.ModelState, controller.ModelState);
        }

        [TestMethod]
        public void RequestProperty() {
            var c = new EmptyController();
            Assert.IsNull(c.Request, "Property should be null before Context is set");

            Mock<HttpRequestBase> mockHttpRequest = new Mock<HttpRequestBase>();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(cc => cc.Controller).Returns(c);
            mockControllerContext.Expect(cc => cc.HttpContext.Request).Returns(mockHttpRequest.Object);

            c.ControllerContext = mockControllerContext.Object;
            Assert.AreEqual(mockHttpRequest.Object, c.Request, "Property should equal the value on the Context.");
        }

        [TestMethod]
        public void ResponseProperty() {
            var c = new EmptyController();
            Assert.IsNull(c.Request, "Property should be null before Context is set");

            Mock<HttpResponseBase> mockHttpResponse = new Mock<HttpResponseBase>();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(cc => cc.Controller).Returns(c);
            mockControllerContext.Expect(cc => cc.HttpContext.Response).Returns(mockHttpResponse.Object);

            c.ControllerContext = mockControllerContext.Object;
            Assert.AreEqual(mockHttpResponse.Object, c.Response, "Property should equal the value on the Context.");
        }

        [TestMethod]
        public void ServerProperty() {
            var c = new EmptyController();
            Assert.IsNull(c.Request, "Property should be null before Context is set");

            Mock<HttpServerUtilityBase> mockServerUtility = new Mock<HttpServerUtilityBase>();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(cc => cc.Controller).Returns(c);
            mockControllerContext.Expect(cc => cc.HttpContext.Server).Returns(mockServerUtility.Object);

            c.ControllerContext = mockControllerContext.Object;
            Assert.AreEqual(mockServerUtility.Object, c.Server, "Property should equal the value on the Context.");
        }

        [TestMethod]
        public void SessionProperty() {
            var c = new EmptyController();
            Assert.IsNull(c.Request, "Property should be null before Context is set");

            Mock<HttpSessionStateBase> mockSessionState = new Mock<HttpSessionStateBase>();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(cc => cc.Controller).Returns(c);
            mockControllerContext.Expect(cc => cc.HttpContext.Session).Returns(mockSessionState.Object);

            c.ControllerContext = mockControllerContext.Object;
            Assert.AreEqual(mockSessionState.Object, c.Session, "Property should equal the value on the Context.");
        }

        [TestMethod]
        public void UrlProperty() {
            // Arrange
            EmptyController controller = new EmptyController();
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            // Act
            controller.PublicInitialize(requestContext);

            // Assert
            Assert.IsNotNull(controller.Url);
        }

        [TestMethod]
        public void UserProperty() {
            var c = new EmptyController();
            Assert.IsNull(c.Request, "Property should be null before Context is set");

            Mock<IPrincipal> mockUser = new Mock<IPrincipal>();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(cc => cc.Controller).Returns(c);
            mockControllerContext.Expect(cc => cc.HttpContext.User).Returns(mockUser.Object);

            c.ControllerContext = mockControllerContext.Object;
            Assert.AreEqual(mockUser.Object, c.User, "Property should equal the value on the Context.");
        }

        [TestMethod]
        public void RouteDataProperty() {
            var c = new EmptyController();
            Assert.IsNull(c.Request, "Property should be null before Context is set");

            RouteData rd = new RouteData();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(cc => cc.Controller).Returns(c);
            mockControllerContext.Expect(cc => cc.RouteData).Returns(rd);

            c.ControllerContext = mockControllerContext.Object;
            Assert.AreEqual(rd, c.RouteData, "Property should equal the value on the Context.");

        }

        [TestMethod]
        public void ControllerMethodsDoNotHaveNonActionAttribute() {
            var methods = typeof(Controller).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods) {
                var attrs = method.GetCustomAttributes(typeof(NonActionAttribute), true /* inherit */);
                Assert.AreEqual(0, attrs.Length, "Methods on the Controller class should not be marked [NonAction]: " + method);
            }
        }

        [TestMethod]
        public void DisposeCallsProtectedDisposingMethod() {
            // Arrange
            Mock<Controller> mockController = new Mock<Controller>();
            mockController.Protected().Expect("Dispose", true).Verifiable();
            Controller controller = mockController.Object;

            // Act
            controller.Dispose();

            // Assert
            mockController.Verify();
        }

        [TestMethod]
        public void ExecuteWithUnknownAction() {
            // Arrange
            UnknownActionController controller = new UnknownActionController();
            // We need a provider since Controller.Execute is called
            controller.TempDataProvider = new EmptyTempDataProvider();
            ControllerContext context = GetControllerContext("Foo");

            Mock<IActionInvoker> mockInvoker = new Mock<IActionInvoker>();
            mockInvoker.Expect(o => o.InvokeAction(context, "Foo")).Returns(false);
            controller.ActionInvoker = mockInvoker.Object;

            // Act
            ((IController)controller).Execute(context.RequestContext);

            // Assert
            Assert.IsTrue(controller.WasCalled);
        }

        [TestMethod]
        public void FileWithContents() {
            // Arrange
            EmptyController controller = new EmptyController();
            byte[] fileContents = new byte[0];

            // Act
            FileContentResult result = controller.File(fileContents, "someContentType");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(fileContents, result.FileContents);
            Assert.AreEqual("someContentType", result.ContentType);
            Assert.AreEqual(String.Empty, result.FileDownloadName);
        }

        [TestMethod]
        public void FileWithContentsAndFileDownloadName() {
            // Arrange
            EmptyController controller = new EmptyController();
            byte[] fileContents = new byte[0];

            // Act
            FileContentResult result = controller.File(fileContents, "someContentType", "someDownloadName");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(fileContents, result.FileContents);
            Assert.AreEqual("someContentType", result.ContentType);
            Assert.AreEqual("someDownloadName", result.FileDownloadName);
        }

        [TestMethod]
        public void FileWithPath() {
            // Arrange
            EmptyController controller = new EmptyController();

            // Act
            FilePathResult result = controller.File("somePath", "someContentType");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("somePath", result.FileName);
            Assert.AreEqual("someContentType", result.ContentType);
            Assert.AreEqual(String.Empty, result.FileDownloadName);
        }

        [TestMethod]
        public void FileWithPathAndFileDownloadName() {
            // Arrange
            EmptyController controller = new EmptyController();

            // Act
            FilePathResult result = controller.File("somePath", "someContentType", "someDownloadName");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("somePath", result.FileName);
            Assert.AreEqual("someContentType", result.ContentType);
            Assert.AreEqual("someDownloadName", result.FileDownloadName);
        }

        [TestMethod]
        public void FileWithStream() {
            // Arrange
            EmptyController controller = new EmptyController();
            Stream fileStream = Stream.Null;

            // Act
            FileStreamResult result = controller.File(fileStream, "someContentType");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(fileStream, result.FileStream);
            Assert.AreEqual("someContentType", result.ContentType);
            Assert.AreEqual(String.Empty, result.FileDownloadName);
        }

        [TestMethod]
        public void FileWithStreamAndFileDownloadName() {
            // Arrange
            EmptyController controller = new EmptyController();
            Stream fileStream = Stream.Null;

            // Act
            FileStreamResult result = controller.File(fileStream, "someContentType", "someDownloadName");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(fileStream, result.FileStream);
            Assert.AreEqual("someContentType", result.ContentType);
            Assert.AreEqual("someDownloadName", result.FileDownloadName);
        }

        [TestMethod]
        public void HandleUnknownActionThrows() {
            var controller = new EmptyController();
            ExceptionHelper.ExpectException<HttpException>(
                delegate {
                    controller.HandleUnknownAction("UnknownAction");
                },
                "A public action method 'UnknownAction' could not be found on controller 'System.Web.Mvc.Test.ControllerTest+EmptyController'.");
        }

        [TestMethod]
        public void JavaScript() {
            // Arrange
            Controller controller = GetEmptyController();
            string script = "alert('foo');";

            // Act
            JavaScriptResult result = controller.JavaScript(script);

            // Assert
            Assert.AreEqual(script, result.Script);
        }

        [TestMethod]
        public void PartialView() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            PartialViewResult result = controller.PartialView();

            // Assert
            Assert.AreSame(controller.TempData, result.TempData);
            Assert.AreSame(controller.ViewData, result.ViewData);
        }

        [TestMethod]
        public void PartialView_Model() {
            // Arrange
            Controller controller = GetEmptyController();
            object model = new object();

            // Act
            PartialViewResult result = controller.PartialView(model);

            // Assert
            Assert.AreSame(model, result.ViewData.Model);
            Assert.AreSame(controller.TempData, result.TempData);
            Assert.AreSame(controller.ViewData, result.ViewData);
        }

        [TestMethod]
        public void PartialView_ViewName() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            PartialViewResult result = controller.PartialView("Some partial view");

            // Assert
            Assert.AreEqual("Some partial view", result.ViewName);
            Assert.AreSame(controller.TempData, result.TempData);
            Assert.AreSame(controller.ViewData, result.ViewData);
        }

        [TestMethod]
        public void PartialView_ViewName_Model() {
            // Arrange
            Controller controller = GetEmptyController();
            object model = new object();

            // Act
            PartialViewResult result = controller.PartialView("Some partial view", model);

            // Assert
            Assert.AreEqual("Some partial view", result.ViewName);
            Assert.AreSame(model, result.ViewData.Model);
            Assert.AreSame(controller.TempData, result.TempData);
            Assert.AreSame(controller.ViewData, result.ViewData);
        }

        [TestMethod]
        public void RedirectToActionClonesRouteValueDictionary() {
            // The RedirectToAction() method should clone the provided dictionary, then operate on the clone.
            // The original dictionary should remain unmodified throughout the helper's execution.

            // Arrange
            Controller controller = GetEmptyController();
            RouteValueDictionary values = new RouteValueDictionary(new { Action = "SomeAction", Controller = "SomeController" });

            // Act
            controller.RedirectToAction("SomeOtherAction", "SomeOtherController", values);

            // Assert
            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("SomeAction", values["action"]);
            Assert.AreEqual("SomeController", values["controller"]);
        }

        [TestMethod]
        public void RedirectToActionOverwritesActionDictionaryKey() {
            // Arrange
            Controller controller = GetEmptyController();
            object values = new { Action = "SomeAction" };

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", values);
            RouteValueDictionary newValues = result.RouteValues;

            // Assert
            Assert.AreEqual("SomeOtherAction", newValues["action"]);
        }

        [TestMethod]
        public void RedirectToActionOverwritesControllerDictionaryKeyIfSpecified() {
            // Arrange
            Controller controller = GetEmptyController();
            object values = new { Action = "SomeAction", Controller = "SomeController" };

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", "SomeOtherController", values);
            RouteValueDictionary newValues = result.RouteValues;

            // Assert
            Assert.AreEqual("SomeOtherController", newValues["controller"]);
        }

        [TestMethod]
        public void RedirectToActionPreservesControllerDictionaryKeyIfNotSpecified() {
            // Arrange
            Controller controller = GetEmptyController();
            object values = new { Controller = "SomeController" };

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", values);
            RouteValueDictionary newValues = result.RouteValues;

            // Assert
            Assert.AreEqual("SomeController", newValues["controller"]);
        }

        [TestMethod]
        public void RedirectToActionWithActionName() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction");

            // Assert
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual("SomeOtherAction", result.RouteValues["action"]);
        }

        [TestMethod]
        public void RedirectToActionWithActionNameAndControllerName() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", "SomeOtherController");

            // Assert
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual("SomeOtherAction", result.RouteValues["action"]);
            Assert.AreEqual("SomeOtherController", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void RedirectToActionWithActionNameAndControllerNameAndValuesDictionary() {
            // Arrange
            Controller controller = GetEmptyController();
            RouteValueDictionary values = new RouteValueDictionary(new { Foo = "SomeFoo" });

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", "SomeOtherController", values);

            // Assert
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual("SomeOtherAction", result.RouteValues["action"]);
            Assert.AreEqual("SomeOtherController", result.RouteValues["controller"]);
            Assert.AreEqual("SomeFoo", result.RouteValues["foo"]);
        }

        [TestMethod]
        public void RedirectToActionWithActionNameAndControllerNameAndValuesObject() {
            // Arrange
            Controller controller = GetEmptyController();
            object values = new { Foo = "SomeFoo" };

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", "SomeOtherController", values);

            // Assert
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual("SomeOtherAction", result.RouteValues["action"]);
            Assert.AreEqual("SomeOtherController", result.RouteValues["controller"]);
            Assert.AreEqual("SomeFoo", result.RouteValues["foo"]);
        }

        [TestMethod]
        public void RedirectToActionSelectsCurrentControllerByDefault() {
            // Arrange
            TestRouteController controller = new TestRouteController();
            controller.ControllerContext = GetControllerContext("SomeAction", "TestRoute");

            // Act
            RedirectToRouteResult route = controller.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("SomeAction", route.RouteValues["action"]);
            Assert.AreEqual("TestRoute", route.RouteValues["controller"]);
        }

        [TestMethod]
        public void RedirectToActionDictionaryOverridesDefaultControllerName() {
            // Arrange
            TestRouteController controller = new TestRouteController();
            object values = new { controller = "SomeOtherController" };
            controller.ControllerContext = GetControllerContext("SomeAction", "TestRoute");

            // Act
            RedirectToRouteResult route = controller.RedirectToAction("SomeOtherAction", values);

            // Assert
            Assert.AreEqual("SomeOtherAction", route.RouteValues["action"]);
            Assert.AreEqual("SomeOtherController", route.RouteValues["controller"]);
        }

        [TestMethod]
        public void RedirectToActionWithActionNameAndValuesDictionary() {
            // Arrange
            Controller controller = GetEmptyController();
            RouteValueDictionary values = new RouteValueDictionary(new { Foo = "SomeFoo" });

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", values);

            // Assert
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual("SomeOtherAction", result.RouteValues["action"]);
            Assert.AreEqual("SomeFoo", result.RouteValues["foo"]);
        }

        [TestMethod]
        public void RedirectToActionWithActionNameAndValuesObject() {
            // Arrange
            Controller controller = GetEmptyController();
            object values = new { Foo = "SomeFoo" };

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", values);

            // Assert
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual("SomeOtherAction", result.RouteValues["action"]);
            Assert.AreEqual("SomeFoo", result.RouteValues["foo"]);
        }

        [TestMethod]
        public void RedirectToActionWithNullRouteValueDictionary() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            RedirectToRouteResult result = controller.RedirectToAction("SomeOtherAction", (RouteValueDictionary)null);
            RouteValueDictionary newValues = result.RouteValues;

            // Assert
            Assert.AreEqual(1, newValues.Count);
            Assert.AreEqual("SomeOtherAction", newValues["action"]);
        }

        [TestMethod]
        public void RedirectToRouteWithNullRouteValueDictionary() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute((RouteValueDictionary)null);

            // Assert
            Assert.AreEqual(0, result.RouteValues.Count);
        }

        [TestMethod]
        public void RedirectToRouteWithObjectDictionary() {
            // Arrange
            Controller controller = GetEmptyController();
            var values = new { Foo = "MyFoo" };

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute(values);

            // Assert
            Assert.AreEqual(1, result.RouteValues.Count);
            Assert.AreEqual("MyFoo", result.RouteValues["Foo"]);
        }

        [TestMethod]
        public void RedirectToRouteWithRouteValueDictionary() {
            // Arrange
            Controller controller = GetEmptyController();
            RouteValueDictionary values = new RouteValueDictionary() { { "Foo", "MyFoo" } };

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute(values);

            // Assert
            Assert.AreEqual(1, result.RouteValues.Count);
            Assert.AreEqual("MyFoo", result.RouteValues["Foo"]);
            Assert.AreNotSame(values, result.RouteValues);
        }

        [TestMethod]
        public void RedirectToRouteWithName() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute("foo");

            // Assert
            Assert.AreEqual(0, result.RouteValues.Count);
            Assert.AreEqual("foo", result.RouteName);
        }

        [TestMethod]
        public void RedirectToRouteWithNameAndNullRouteValueDictionary() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute("foo", (RouteValueDictionary)null);

            // Assert
            Assert.AreEqual(0, result.RouteValues.Count);
            Assert.AreEqual("foo", result.RouteName);
        }

        [TestMethod]
        public void RedirectToRouteWithNullNameAndNullRouteValueDictionary() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute(null, (RouteValueDictionary)null);

            // Assert
            Assert.AreEqual(0, result.RouteValues.Count);
            Assert.AreEqual(String.Empty, result.RouteName);
        }

        [TestMethod]
        public void RedirectToRouteWithNameAndObjectDictionary() {
            // Arrange
            Controller controller = GetEmptyController();
            var values = new { Foo = "MyFoo" };

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute("foo", values);

            // Assert
            Assert.AreEqual(1, result.RouteValues.Count);
            Assert.AreEqual("MyFoo", result.RouteValues["Foo"]);
            Assert.AreEqual("foo", result.RouteName);
        }

        [TestMethod]
        public void RedirectToRouteWithNameAndRouteValueDictionary() {
            // Arrange
            Controller controller = GetEmptyController();
            RouteValueDictionary values = new RouteValueDictionary() { { "Foo", "MyFoo" } };

            // Act
            RedirectToRouteResult result = controller.RedirectToRoute("foo", values);

            // Assert
            Assert.AreEqual(1, result.RouteValues.Count);
            Assert.AreEqual("MyFoo", result.RouteValues["Foo"]);
            Assert.AreNotSame(values, result.RouteValues);
            Assert.AreEqual("foo", result.RouteName);
        }

        [TestMethod]
        public void RedirectReturnsCorrectActionResult() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act & Assert
            var result = controller.Redirect("http://www.contoso.com/");

            // Assert
            Assert.AreEqual("http://www.contoso.com/", result.Url);
        }

        [TestMethod]
        public void RedirectWithEmptyUrlThrows() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    controller.Redirect(String.Empty);
                },
                "url");
        }

        [TestMethod]
        public void RedirectWithNullUrlThrows() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    controller.Redirect(null /* url */);
                },
                "url");
        }

        [TestMethod]
        public void RenderView0_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            ViewResult result = controller.View();

            // Assert
            Assert.AreSame(controller.ViewData, result.ViewData);
            Assert.AreSame(controller.TempData, result.TempData);
        }

        [TestMethod]
        public void RenderView1_obj_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();
            object viewItem = new object();

            // Act
            ViewResult result = controller.View(viewItem);

            // Assert
            Assert.AreSame(viewItem, result.ViewData.Model);
            Assert.AreSame(controller.TempData, result.TempData);
        }

        [TestMethod]
        public void RenderView1_str_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            ViewResult result = controller.View("Foo");

            // Assert
            Assert.AreEqual("Foo", result.ViewName);
            Assert.AreSame(controller.ViewData, result.ViewData);
            Assert.AreSame(controller.TempData, result.TempData);
        }

        [TestMethod]
        public void RenderView2_str_obj_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();
            object viewItem = new object();

            // Act
            ViewResult result = controller.View("Foo", viewItem);

            // Assert
            Assert.AreEqual("Foo", result.ViewName);
            Assert.AreSame(viewItem, result.ViewData.Model);
            Assert.AreSame(controller.TempData, result.TempData);
        }

        [TestMethod]
        public void RenderView2_str_str_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();

            // Act
            ViewResult result = controller.View("Foo", "Bar");

            // Assert
            Assert.AreEqual("Foo", result.ViewName);
            Assert.AreEqual("Bar", result.MasterName);
            Assert.AreSame(controller.ViewData, result.ViewData);
            Assert.AreSame(controller.TempData, result.TempData);
        }

        [TestMethod]
        public void RenderView3_str_str_obj_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();
            object viewItem = new object();

            // Act
            ViewResult result = controller.View("Foo", "Bar", viewItem);

            // Assert
            Assert.AreEqual("Foo", result.ViewName);
            Assert.AreEqual("Bar", result.MasterName);
            Assert.AreSame(viewItem, result.ViewData.Model);
            Assert.AreSame(controller.TempData, result.TempData);
        }

        [TestMethod]
        public void RenderView4_view_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();
            IView view = new Mock<IView>().Object;

            // Act
            ViewResult result = controller.View(view);

            // Assert
            Assert.AreSame(result.View, view);
            Assert.AreSame(controller.ViewData, result.ViewData);
            Assert.AreSame(controller.TempData, result.TempData);
        }

        [TestMethod]
        public void RenderView5_view_obj_SetsProperties() {
            // Arrange
            Controller controller = GetEmptyController();
            IView view = new Mock<IView>().Object;
            object model = new object();

            // Act
            ViewResult result = controller.View(view, model);

            // Assert
            Assert.AreSame(result.View, view);
            Assert.AreSame(controller.ViewData, result.ViewData);
            Assert.AreSame(controller.TempData, result.TempData);
            Assert.AreSame(model, result.ViewData.Model);
        }

        internal static void AddRequestParams(Mock<HttpRequestBase> requestMock, object paramValues) {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(paramValues);
            foreach (PropertyDescriptor prop in props) {
                requestMock.Expect(o => o[It.Is<string>(item => String.Equals(prop.Name, item, StringComparison.OrdinalIgnoreCase))]).Returns((string)prop.GetValue(paramValues));
            }
        }

        [TestMethod]
        public void TempDataGreetUserWithNoUserIDRedirects() {
            // Arrange
            TempDataHomeController tempDataHomeController = new TempDataHomeController();

            // Act
            RedirectToRouteResult result = tempDataHomeController.GreetUser() as RedirectToRouteResult;
            RouteValueDictionary values = result.RouteValues;

            // Assert
            Assert.IsTrue(values.ContainsKey("action"));
            Assert.AreEqual("ErrorPage", values["action"]);
            Assert.AreEqual(0, tempDataHomeController.TempData.Count);
        }

        [TestMethod]
        public void TempDataGreetUserWithUserIDCopiesToViewDataAndRenders() {
            // Arrange
            TempDataHomeController tempDataHomeController = new TempDataHomeController();
            tempDataHomeController.TempData["UserID"] = "TestUserID";

            // Act
            ViewResult result = tempDataHomeController.GreetUser() as ViewResult;
            ViewDataDictionary viewData = tempDataHomeController.ViewData;

            // Assert
            Assert.AreEqual("GreetUser", result.ViewName);
            Assert.IsNotNull(viewData);
            Assert.IsTrue(viewData.ContainsKey("NewUserID"));
            Assert.AreEqual("TestUserID", viewData["NewUserID"]);
        }

        [TestMethod]
        public void TempDataIndexSavesUserIDAndRedirects() {
            // Arrange
            TempDataHomeController tempDataHomeController = new TempDataHomeController();

            // Act
            RedirectToRouteResult result = tempDataHomeController.Index() as RedirectToRouteResult;
            RouteValueDictionary values = result.RouteValues;

            // Assert
            Assert.IsTrue(values.ContainsKey("action"));
            Assert.AreEqual("GreetUser", values["action"]);

            Assert.IsTrue(tempDataHomeController.TempData.ContainsKey("UserID"));
            Assert.AreEqual("user123", tempDataHomeController.TempData["UserID"]);
        }

        [TestMethod]
        public void TempDataSavedWhenControllerThrows() {
            // Arrange
            BrokenController controller = new BrokenController() { ValidateRequest = false };
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            HttpSessionStateBase session = GetEmptySession();
            mockContext.Expect(o => o.Session).Returns(session);
            RouteData rd = new RouteData();
            rd.Values.Add("action", "Crash");
            controller.ControllerContext = new ControllerContext(mockContext.Object, rd, controller);

            // Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    ((IController)controller).Execute(controller.ControllerContext.RequestContext);
                });
            Assert.AreNotEqual(mockContext.Object.Session[SessionStateTempDataProvider.TempDataSessionStateKey], null);
            TempDataDictionary tempData = new TempDataDictionary();
            tempData.Load(controller.ControllerContext, controller.TempDataProvider);
            Assert.AreEqual(tempData["Key1"], "Value1");
        }

        [TestMethod]
        public void TempDataMovedToPreviousTempDataInDestinationController() {
            // Arrange
            Mock<Controller> mockController = new Mock<Controller>() { CallBase = true };
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            HttpSessionStateBase session = GetEmptySession();
            mockContext.Expect(o => o.Session).Returns(session);
            mockController.Object.ControllerContext = new ControllerContext(mockContext.Object, new RouteData(), mockController.Object);

            // Act
            mockController.Object.TempData.Add("Key", "Value");
            mockController.Object.TempData.Save(mockController.Object.ControllerContext, mockController.Object.TempDataProvider);

            // Assert
            Assert.IsTrue(mockController.Object.TempData.ContainsKey("Key"), "The key should still exist in the old TempData");
            Assert.IsTrue(mockController.Object.TempData.ContainsValue("Value"), "The value should still exist in the old TempData");

            // Instantiate "destination" controller with the same session state and see that it gets the temp data
            Mock<Controller> mockDestinationController = new Mock<Controller>() { CallBase = true };
            Mock<HttpContextBase> mockDestinationContext = new Mock<HttpContextBase>();
            mockDestinationContext.Expect(o => o.Session).Returns(session);
            mockDestinationController.Object.ControllerContext = new ControllerContext(mockDestinationContext.Object, new RouteData(), mockDestinationController.Object);
            mockDestinationController.Object.TempData.Load(mockDestinationController.Object.ControllerContext, mockDestinationController.Object.TempDataProvider);

            // Assert
            Assert.AreEqual("Value", mockDestinationController.Object.TempData["Key"], "The key should exist in the new TempData");

            // Act
            mockDestinationController.Object.TempData["NewKey"] = "NewValue";
            Assert.AreEqual("NewValue", mockDestinationController.Object.TempData["NewKey"], "The new key should exist in the new TempData");
            mockDestinationController.Object.TempData.Save(mockDestinationController.Object.ControllerContext, mockDestinationController.Object.TempDataProvider);

            // Instantiate "second destination" controller with the same session state and see that it gets the temp data
            Mock<Controller> mockSecondDestinationController = new Mock<Controller>() { CallBase = true };
            Mock<HttpContextBase> mockSecondDestinationContext = new Mock<HttpContextBase>();
            mockSecondDestinationContext.Expect(o => o.Session).Returns(session);
            mockSecondDestinationController.Object.ControllerContext = new ControllerContext(mockSecondDestinationContext.Object, new RouteData(), mockSecondDestinationController.Object);
            mockSecondDestinationController.Object.TempData.Load(mockSecondDestinationController.Object.ControllerContext, mockSecondDestinationController.Object.TempDataProvider);

            // Assert
            Assert.IsFalse(mockSecondDestinationController.Object.TempData.ContainsKey("Key"), "The key should not exist in the new TempData");
            Assert.AreEqual("NewValue", mockSecondDestinationController.Object.TempData["NewKey"], "The new key should exist in the new TempData");
        }

        [TestMethod]
        public void TempDataValidForSingleControllerWhenSessionStateDisabled() {
            // Arrange
            Mock<Controller> mockController = new Mock<Controller>();
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            HttpSessionStateBase session = null;
            mockContext.Expect(o => o.Session).Returns(session);
            mockController.Object.ControllerContext = new ControllerContext(mockContext.Object, new RouteData(), mockController.Object);
            mockController.Object.TempData = new TempDataDictionary();

            // Act
            mockController.Object.TempData["Key"] = "Value";

            // Assert
            Assert.IsTrue(mockController.Object.TempData.ContainsKey("Key"), "The key should exist in TempData, even with SessionState disabled.");
        }

        [TestMethod]
        public void TryUpdateModelCallsModelBinderForModel() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();
            Dictionary<string, ValueProviderResult> valueProvider = new Dictionary<string, ValueProviderResult>();

            Controller controller = new EmptyController();
            controller.ControllerContext = GetControllerContext("someAction");

            // Act
            bool returned = controller.TryUpdateModel(myModel, "somePrefix", new[] { "prop1", "prop2" }, null, valueProvider);

            // Assert
            Assert.IsTrue(returned);
            Assert.AreEqual(valueProvider, myModel.BindingContext.ValueProvider);
            Assert.AreEqual("somePrefix", myModel.BindingContext.ModelName);
            Assert.AreEqual(controller.ModelState, myModel.BindingContext.ModelState);
            Assert.AreEqual(typeof(MyModel), myModel.BindingContext.ModelType);
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop1"), "Incorrect filter applied.");
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop2"), "Incorrect filter applied.");
            Assert.IsFalse(myModel.BindingContext.PropertyFilter("prop3"), "Incorrect filter applied.");
        }

        [TestMethod]
        public void TryUpdateModelReturnsFalseIfModelStateInvalid() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();

            Controller controller = new EmptyController();
            controller.ModelState.AddModelError("key", "some exception message");

            // Act
            bool returned = controller.TryUpdateModel(myModel);

            // Assert
            Assert.IsFalse(returned);
        }

        [TestMethod]
        public void TryUpdateModelSuppliesControllerValueProviderIfNoValueProviderSpecified() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();
            Dictionary<string, ValueProviderResult> valueProvider = new Dictionary<string, ValueProviderResult>();

            Controller controller = new EmptyController();
            controller.ValueProvider = valueProvider;

            // Act
            bool returned = controller.TryUpdateModel(myModel, "somePrefix", new[] { "prop1", "prop2" });

            // Assert
            Assert.IsTrue(returned);
            Assert.AreEqual(valueProvider, myModel.BindingContext.ValueProvider);
        }

        [TestMethod]
        public void TryUpdateModelSuppliesEmptyModelNameIfNoPrefixSpecified() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();
            Controller controller = new EmptyController();

            // Act
            bool returned = controller.TryUpdateModel(myModel, new[] { "prop1", "prop2" });

            // Assert
            Assert.IsTrue(returned);
            Assert.AreEqual(String.Empty, myModel.BindingContext.ModelName);
        }

        [TestMethod]
        public void TryUpdateModelThrowsIfModelIsNull() {
            // Arrange
            Controller controller = new EmptyController();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    controller.TryUpdateModel<object>(null);
                }, "model");
        }

        [TestMethod]
        public void TryUpdateModelThrowsIfValueProviderIsNull() {
            // Arrange
            Controller controller = new EmptyController();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    controller.TryUpdateModel(new object(), null, null, null, null);
                }, "valueProvider");
        }

        [TestMethod]
        public void UpdateModelReturnsIfModelStateValid() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();
            Controller controller = new EmptyController();

            // Act
            controller.UpdateModel(myModel);

            // Assert
            // nothing to do - if we got here, the test passed
        }

        [TestMethod]
        public void TryUpdateModelWithoutBindPropertiesImpliesAllPropertiesAreUpdateable() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();
            Controller controller = new EmptyController();

            // Act
            bool returned = controller.TryUpdateModel(myModel, "somePrefix");

            // Assert
            Assert.IsTrue(returned);
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop1"), "Incorrect filter applied.");
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop2"), "Incorrect filter applied.");
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop3"), "Incorrect filter applied.");
        }

        [TestMethod]
        public void UpdateModelSuppliesControllerValueProviderIfNoValueProviderSpecified() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();
            Dictionary<string, ValueProviderResult> valueProvider = new Dictionary<string, ValueProviderResult>();

            Controller controller = new EmptyController() { ValueProvider = valueProvider };

            // Act
            controller.UpdateModel(myModel, "somePrefix", new[] { "prop1", "prop2" });

            // Assert
            Assert.AreEqual(valueProvider, myModel.BindingContext.ValueProvider);
        }

        [TestMethod]
        public void UpdateModelThrowsIfModelStateInvalid() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();

            Controller controller = new EmptyController();
            controller.ModelState.AddModelError("key", "some exception message");

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    controller.UpdateModel(myModel);
                },
                "The model of type 'System.Web.Mvc.Test.ControllerTest+MyModel' was not successfully updated.");
        }

        [TestMethod]
        public void UpdateModelWithoutBindPropertiesImpliesAllPropertiesAreUpdateable() {
            // Arrange
            MyModel myModel = new MyModelSubclassed();
            Controller controller = new EmptyController();

            // Act
            controller.UpdateModel(myModel, "somePrefix");

            // Assert
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop1"), "Incorrect filter applied.");
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop2"), "Incorrect filter applied.");
            Assert.IsTrue(myModel.BindingContext.PropertyFilter("prop3"), "Incorrect filter applied.");
        }

        private static ControllerContext GetControllerContext(string actionName) {
            RouteData rd = new RouteData();
            rd.Values["action"] = actionName;

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Session).Returns((HttpSessionStateBase)null);

            return new ControllerContext(mockHttpContext.Object, rd, new Mock<Controller>().Object);
        }

        private static ControllerContext GetControllerContext(string actionName, string controllerName) {
            RouteData rd = new RouteData();
            rd.Values["action"] = actionName;
            rd.Values["controller"] = controllerName;

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Session).Returns((HttpSessionStateBase)null);

            return new ControllerContext(mockHttpContext.Object, rd, new Mock<Controller>().Object);
        }

        private static Controller GetEmptyController() {
            ControllerContext context = GetControllerContext("Foo");
            var controller = new EmptyController() {
                ControllerContext = context,
                RouteCollection = new RouteCollection(),
                TempData = new TempDataDictionary(),
                TempDataProvider = new SessionStateTempDataProvider()
            };
            return controller;
        }

        private static HttpSessionStateBase GetEmptySession() {
            HttpSessionStateMock mockSession = new HttpSessionStateMock();
            return mockSession;
        }

        private sealed class HttpSessionStateMock : HttpSessionStateBase {
            private Hashtable _sessionData = new Hashtable(StringComparer.OrdinalIgnoreCase);

            public override void Remove(string name) {
                Assert.AreEqual<string>(SessionStateTempDataProvider.TempDataSessionStateKey, name);
                _sessionData.Remove(name);
            }

            public override object this[string name] {
                get {
                    Assert.AreEqual<string>(SessionStateTempDataProvider.TempDataSessionStateKey, name);
                    return _sessionData[name];
                }
                set {
                    Assert.AreEqual<string>(SessionStateTempDataProvider.TempDataSessionStateKey, name);
                    _sessionData[name] = value;
                }
            }
        }

        public class Person {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private class EmptyController : Controller {
            public new void HandleUnknownAction(string actionName) {
                base.HandleUnknownAction(actionName);
            }

            public void PublicInitialize(RequestContext requestContext) {
                base.Initialize(requestContext);
            }
        }

        private sealed class UnknownActionController : Controller {
            public bool WasCalled;

            protected override void HandleUnknownAction(string actionName) {
                WasCalled = true;
            }
        }

        private sealed class TempDataHomeController : Controller {
            public ActionResult Index() {
                // Save UserID into TempData and redirect to greeting page
                TempData["UserID"] = "user123";
                return RedirectToAction("GreetUser");
            }

            public ActionResult GreetUser() {
                // Check that the UserID is present. If it's not
                // there, redirect to error page. If it is, show
                // the greet user view.
                if (!TempData.ContainsKey("UserID")) {
                    return RedirectToAction("ErrorPage");
                }
                ViewData["NewUserID"] = TempData["UserID"];
                return View("GreetUser");
            }
        }

        public class BrokenController : Controller {
            public BrokenController() {
                ActionInvoker = new ControllerActionInvoker() {
                    DescriptorCache = new ControllerDescriptorCache()
                };
            }
            public ActionResult Crash() {
                TempData["Key1"] = "Value1";
                throw new InvalidOperationException("Crashing....");
            }
        }

        private sealed class TestRouteController : Controller {
            public ActionResult Index() {
                return RedirectToAction("SomeAction");
            }
        }

        [ModelBinder(typeof(MyModelBinder))]
        private class MyModel {
            public ControllerContext ControllerContext;
            public ModelBindingContext BindingContext;
        }

        private class MyModelSubclassed : MyModel {
        }

        private class MyModelBinder : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                MyModel myModel = (MyModel)bindingContext.Model;
                myModel.ControllerContext = controllerContext;
                myModel.BindingContext = bindingContext;
                return myModel;
            }
        }
    }

    internal class EmptyTempDataProvider : ITempDataProvider {
        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values) {
        }

        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext) {
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
