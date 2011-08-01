namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class AsyncControllerTest {

        [TestMethod]
        public void ActionInvokerIsAsyncInvokerByDefault() {
            // Arrange
            EmptyController controller = new EmptyController();

            // Act
            IActionInvoker invoker = controller.ActionInvoker;

            // Assert
            Assert.IsInstanceOfType(invoker, typeof(AsyncControllerActionInvoker));
        }

        [TestMethod]
        public void ExecuteCallsInitialize() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            MockAsyncResult asyncResult = new MockAsyncResult();

            Mock<AsyncController> mockController = new Mock<AsyncController>() { CallBase = true };
            mockController.Expect(c => c.BeginExecuteCore(It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(asyncResult).Verifiable();
            mockController.Expect(c => c.EndExecuteCore(asyncResult)).Verifiable();

            AsyncController controller = mockController.Object;
            IAsyncController iController = controller;

            // Act
            IAsyncResult returnedAsyncResult = iController.BeginExecute(requestContext, null, null);
            iController.EndExecute(returnedAsyncResult);

            // Assert
            Assert.AreEqual(requestContext, controller.ControllerContext.RequestContext);
            mockController.Verify();
        }

        [TestMethod]
        public void ExecuteCoreWithAsynchronousInvokerAndActionCompletesSuccessfully() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            MockAsyncResult asyncResult = new MockAsyncResult();

            Mock<ITempDataProvider> mockTempDataProvider = new Mock<ITempDataProvider>();
            mockTempDataProvider.Expect(p => p.LoadTempData(controllerContext)).Returns(new Dictionary<string, object>()).Verifiable();
            mockTempDataProvider.Expect(p => p.SaveTempData(controllerContext, It.IsAny<IDictionary<string, object>>())).AtMostOnce().Verifiable();

            Mock<IAsyncActionInvoker> mockInvoker = new Mock<IAsyncActionInvoker>();
            mockInvoker.Expect(i => i.BeginInvokeAction(controllerContext, "SomeAction", It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(asyncResult).Verifiable();
            mockInvoker.Expect(i => i.EndInvokeAction(asyncResult)).Returns(true).Verifiable();

            EmptyController controller = new EmptyController() {
                ControllerContext = controllerContext,
                TempDataProvider = mockTempDataProvider.Object,
                ActionInvoker = mockInvoker.Object
            };

            // Act
            IAsyncResult returnedAsyncResult = controller.BeginExecuteCore(null, null);
            controller.TempData["key"] = "value";
            controller.EndExecuteCore(returnedAsyncResult);

            // Assert
            mockInvoker.Verify();
            mockTempDataProvider.Verify();
        }

        [TestMethod]
        public void ExecuteCoreWithAsynchronousInvokerAndActionNotFound() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            MockAsyncResult asyncResult = new MockAsyncResult();

            Mock<ITempDataProvider> mockTempDataProvider = new Mock<ITempDataProvider>();
            mockTempDataProvider.Expect(p => p.LoadTempData(controllerContext)).Returns(new Dictionary<string, object>()).Verifiable();
            mockTempDataProvider.Expect(p => p.SaveTempData(controllerContext, It.IsAny<IDictionary<string, object>>())).AtMostOnce().Verifiable();

            Mock<IAsyncActionInvoker> mockInvoker = new Mock<IAsyncActionInvoker>();
            mockInvoker.Expect(i => i.BeginInvokeAction(controllerContext, "SomeAction", It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(asyncResult).Verifiable();
            mockInvoker.Expect(i => i.EndInvokeAction(asyncResult)).Returns(false).Verifiable();

            EmptyController controller = new EmptyController() {
                ControllerContext = controllerContext,
                TempDataProvider = mockTempDataProvider.Object,
                ActionInvoker = mockInvoker.Object
            };

            // Act
            IAsyncResult returnedAsyncResult = controller.BeginExecuteCore(null, null);
            controller.TempData["key"] = "value";
            ExceptionHelper.ExpectHttpException(
                delegate {
                    controller.EndExecuteCore(returnedAsyncResult);
                },
                @"A public action method 'SomeAction' could not be found on controller 'Microsoft.Web.Mvc.Test.AsyncControllerTest+EmptyController'.",
                404);

            // Assert
            mockInvoker.Verify();
            mockTempDataProvider.Verify();
        }

        [TestMethod]
        public void ExecuteCoreWithAsynchronousInvokerAndBeginInvokeActionThrows() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            MockAsyncResult asyncResult = new MockAsyncResult();

            Mock<ITempDataProvider> mockTempDataProvider = new Mock<ITempDataProvider>();
            mockTempDataProvider.Expect(p => p.LoadTempData(controllerContext)).Returns(new Dictionary<string, object>()).Verifiable();
            mockTempDataProvider.Expect(p => p.SaveTempData(controllerContext, It.IsAny<IDictionary<string, object>>())).AtMostOnce().Verifiable();

            Mock<IAsyncActionInvoker> mockInvoker = new Mock<IAsyncActionInvoker>();
            EmptyController controller = new EmptyController() {
                ControllerContext = controllerContext,
                TempDataProvider = mockTempDataProvider.Object,
                ActionInvoker = mockInvoker.Object
            };

            mockInvoker
                .Expect(i => i.BeginInvokeAction(controllerContext, "SomeAction", It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Callback(
                    delegate(ControllerContext cc, string an, AsyncCallback cb, object s) {
                        controller.TempData["key"] = "value";
                        throw new InvalidOperationException("Some exception text.");
                    });

            // Act
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    controller.BeginExecuteCore(null, null);
                },
                @"Some exception text.");

            // Assert
            mockTempDataProvider.Verify();
        }

        [TestMethod]
        public void ExecuteCoreWithSynchronousInvokerAndActionCompletesSuccessfully() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            MockAsyncResult asyncResult = new MockAsyncResult();

            Mock<ITempDataProvider> mockTempDataProvider = new Mock<ITempDataProvider>();
            mockTempDataProvider.Expect(p => p.LoadTempData(controllerContext)).Returns(new Dictionary<string, object>()).Verifiable();
            mockTempDataProvider.Expect(p => p.SaveTempData(controllerContext, It.IsAny<IDictionary<string, object>>())).AtMostOnce().Verifiable();

            Mock<IActionInvoker> mockInvoker = new Mock<IActionInvoker>();
            mockInvoker.Expect(i => i.InvokeAction(controllerContext, "SomeAction")).Returns(true).Verifiable();

            EmptyController controller = new EmptyController() {
                ControllerContext = controllerContext,
                TempDataProvider = mockTempDataProvider.Object,
                ActionInvoker = mockInvoker.Object
            };

            // Act
            IAsyncResult returnedAsyncResult = controller.BeginExecuteCore(null, null);
            controller.TempData["key"] = "value";
            controller.EndExecuteCore(returnedAsyncResult);

            // Assert
            mockInvoker.Verify();
            mockTempDataProvider.Verify();
        }

        [TestMethod]
        public void ExecuteThrowsIfRequestContextIsNull() {
            // Arrange
            IAsyncController controller = new EmptyController();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    controller.BeginExecute(null, null, null);
                }, "requestContext");
        }

        private static ControllerContext GetControllerContext() {
            RouteData routeData = new RouteData();
            routeData.Values["action"] = "SomeAction";

            return new ControllerContext() {
                RouteData = routeData
            };
        }

        private class EmptyController : AsyncController {
        }

    }
}
