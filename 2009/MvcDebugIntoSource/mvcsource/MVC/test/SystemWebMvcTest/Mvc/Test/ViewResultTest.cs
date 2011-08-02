namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ViewResultTest {

        private const string _viewName = "My cool view.";
        private const string _masterName = "My cool master.";

        [TestMethod]
        public void EmptyViewNameUsesActionNameAsViewName() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            HttpContextBase httpContext = CreateHttpContext();
            RouteData routeData = new RouteData();
            routeData.Values["action"] = _viewName;
            ControllerContext context = new ControllerContext(httpContext, routeData, controller);
            Mock<IViewEngine> viewEngine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            List<IViewEngine> viewEngines = new List<IViewEngine>();
            viewEngines.Add(viewEngine.Object);
            Mock<ViewEngineCollection> viewEngineCollection = new Mock<ViewEngineCollection>(MockBehavior.Strict, viewEngines);
            ViewResult result = new ViewResultHelper { ViewEngineCollection = viewEngineCollection.Object };
            viewEngineCollection
                .Expect(e => e.FindView(It.IsAny<ControllerContext>(), _viewName, _masterName))
                .Returns(new ViewEngineResult(view.Object, viewEngine.Object));
            viewEngine
                .Expect(e => e.FindView(It.IsAny<ControllerContext>(), _viewName, _masterName, It.IsAny<bool>()))
                .Callback<ControllerContext, string, string>(
                    (controllerContext, viewName, masterName) => {
                        Assert.AreSame(httpContext, controllerContext.HttpContext);
                        Assert.AreSame(routeData, controllerContext.RouteData);
                    })
                .Returns(new ViewEngineResult(view.Object, viewEngine.Object));
            view
                .Expect(o => o.Render(It.IsAny<ViewContext>(), httpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(view.Object, viewContext.View);
                        Assert.AreSame(result.ViewData, viewContext.ViewData);
                        Assert.AreSame(result.TempData, viewContext.TempData);
                        Assert.AreSame(controller, viewContext.Controller);
                    });
            viewEngine
                .Expect(e => e.ReleaseView(context, It.IsAny<IView>()))
                .Callback<ControllerContext, IView>(
                    (controllerContext, releasedView) => {
                        Assert.AreSame(releasedView, view.Object);
                    });

            // Act
            result.ExecuteResult(context);

            // Assert
            viewEngine.Verify();
            viewEngineCollection.Verify();
            view.Verify();
        }

        [TestMethod]
        public void EngineLookupFailureThrows() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            HttpContextBase httpContext = CreateHttpContext();
            RouteData routeData = new RouteData();
            routeData.Values["action"] = _viewName;
            ControllerContext context = new ControllerContext(httpContext, routeData, controller);
            Mock<IViewEngine> viewEngine = new Mock<IViewEngine>(MockBehavior.Strict);
            List<IViewEngine> viewEngines = new List<IViewEngine>();
            viewEngines.Add(viewEngine.Object);
            Mock<ViewEngineCollection> viewEngineCollection = new Mock<ViewEngineCollection>(MockBehavior.Strict, viewEngines);
            ViewResult result = new ViewResultHelper { ViewEngineCollection = viewEngineCollection.Object };
            viewEngineCollection
                .Expect(e => e.FindView(It.IsAny<ControllerContext>(), _viewName, _masterName))
                .Returns(new ViewEngineResult(new[] { "location1", "location2" }));
            viewEngine
                .Expect(e => e.FindView(It.IsAny<ControllerContext>(), _viewName, _masterName, It.IsAny<bool>()))
                .Callback<ControllerContext, string, string>(
                    (controllerContext, viewName, masterName) => {
                        Assert.AreSame(httpContext, controllerContext.HttpContext);
                        Assert.AreSame(routeData, controllerContext.RouteData);
                    })
                .Returns(new ViewEngineResult(new[] { "location1", "location2" }));

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => result.ExecuteResult(context),
                "The view '" + _viewName + "' or its master could not be found. The following locations were searched:\r\nlocation1\r\nlocation2");

            viewEngine.Verify();
            viewEngineCollection.Verify();
        }

        [TestMethod]
        public void EngineLookupSuccessRendersView() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            HttpContextBase httpContext = CreateHttpContext();
            RouteData routeData = new RouteData();
            ControllerContext context = new ControllerContext(httpContext, routeData, controller);
            Mock<IViewEngine> viewEngine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            List<IViewEngine> viewEngines = new List<IViewEngine>();
            viewEngines.Add(viewEngine.Object);
            Mock<ViewEngineCollection> viewEngineCollection = new Mock<ViewEngineCollection>(MockBehavior.Strict, viewEngines);
            ViewResult result = new ViewResultHelper { ViewName = _viewName, ViewEngineCollection = viewEngineCollection.Object };
            view
                .Expect(o => o.Render(It.IsAny<ViewContext>(), httpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(view.Object, viewContext.View);
                        Assert.AreSame(result.ViewData, viewContext.ViewData);
                        Assert.AreSame(result.TempData, viewContext.TempData);
                        Assert.AreSame(controller, viewContext.Controller);
                    });
            viewEngineCollection
                .Expect(e => e.FindView(It.IsAny<ControllerContext>(), _viewName, _masterName))
                .Returns(new ViewEngineResult(view.Object, viewEngine.Object));
            viewEngine
                .Expect(e => e.FindView(It.IsAny<ControllerContext>(), _viewName, _masterName, It.IsAny<bool>()))
                .Callback<ControllerContext, string, string>(
                    (controllerContext, viewName, masterName) => {
                        Assert.AreSame(httpContext, controllerContext.HttpContext);
                        Assert.AreSame(routeData, controllerContext.RouteData);
                    })
                .Returns(new ViewEngineResult(view.Object, viewEngine.Object));
            viewEngine
                .Expect(e => e.ReleaseView(context, It.IsAny<IView>()))
                .Callback<ControllerContext, IView>(
                    (controllerContext, releasedView) => {
                        Assert.AreSame(releasedView, view.Object);
                    });

            // Act
            result.ExecuteResult(context);

            // Assert
            viewEngine.Verify();
            viewEngineCollection.Verify();
            view.Verify();
        }

        [TestMethod]
        public void ExecuteResultWithExplicitViewObject() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            HttpContextBase httpContext = CreateHttpContext();
            RouteData routeData = new RouteData();
            routeData.Values["action"] = _viewName;
            ControllerContext context = new ControllerContext(httpContext, routeData, controller);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            ViewResult result = new ViewResultHelper { View = view.Object };
            view
                .Expect(o => o.Render(It.IsAny<ViewContext>(), httpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(view.Object, viewContext.View);
                        Assert.AreSame(result.ViewData, viewContext.ViewData);
                        Assert.AreSame(result.TempData, viewContext.TempData);
                        Assert.AreSame(controller, viewContext.Controller);
                    });

            // Act
            result.ExecuteResult(context);

            // Assert
            view.Verify();
        }

        [TestMethod]
        public void ExecuteResultWithNullControllerContextThrows() {
            // Arrange
            ViewResult result = new ViewResultHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => result.ExecuteResult(null),
                "context");
        }

        [TestMethod]
        public void MasterNameProperty() {
            // Arrange
            ViewResult result = new ViewResult();

            // Act & Assert
            MemberHelper.TestStringProperty(result, "MasterName", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        private static HttpContextBase CreateHttpContext() {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Response.Output).Returns(TextWriter.Null);
            return mockHttpContext.Object;
        }


        private class ViewResultHelper : ViewResult {

            public ViewResultHelper() {
                ViewEngineCollection = new ViewEngineCollection(new IViewEngine[] { new WebFormViewEngine() });
                MasterName = _masterName;
            }

        }

    }
}
