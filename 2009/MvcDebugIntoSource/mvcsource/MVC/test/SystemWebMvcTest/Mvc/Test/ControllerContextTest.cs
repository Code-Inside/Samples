namespace System.Web.Mvc.Test {
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ControllerContextTest {

        [TestMethod]
        public void ConstructorThrowsIfControllerIsNull() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            Controller controller = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ControllerContext(requestContext, controller);
                }, "controller");
        }

        [TestMethod]
        public void ConstructorThrowsIfRequestContextIsNull() {
            // Arrange
            RequestContext requestContext = null;
            Controller controller = new Mock<Controller>().Object;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ControllerContext(requestContext, controller);
                }, "requestContext");
        }

        [TestMethod]
        public void ConstructorWithHttpContextAndRouteData() {
            // Arrange
            HttpContextBase httpContext = new Mock<HttpContextBase>().Object;
            RouteData routeData = new RouteData();
            Controller controller = new Mock<Controller>().Object;

            // Act
            ControllerContext controllerContext = new ControllerContext(httpContext, routeData, controller);

            // Assert
            Assert.AreEqual(httpContext, controllerContext.HttpContext);
            Assert.AreEqual(routeData, controllerContext.RouteData);
            Assert.AreEqual(controller, controllerContext.Controller);
        }

        [TestMethod]
        public void ControllerProperty() {
            // Arrange
            HttpContextBase httpContext = new Mock<HttpContextBase>().Object;
            RouteData routeData = new RouteData();
            Controller controller = new Mock<Controller>().Object;

            // Act
            ControllerContext controllerContext = new ControllerContext(httpContext, routeData, controller);

            // Assert
            Assert.AreEqual(controller, controllerContext.Controller);
        }

        [TestMethod]
        public void CopyConstructorSetsProperties() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            Controller controller = new Mock<Controller>().Object;

            ControllerContext innerControllerContext = new ControllerContext(requestContext, controller);

            // Act
            ControllerContext outerControllerContext = new SubclassedControllerContext(innerControllerContext);

            // Assert
            Assert.AreEqual(requestContext, outerControllerContext.RequestContext);
            Assert.AreEqual(controller, outerControllerContext.Controller);
        }

        [TestMethod]
        public void CopyConstructorThrowsIfControllerContextIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new SubclassedControllerContext(null);
                }, "controllerContext");
        }

        [TestMethod]
        public void HttpContextPropertyGetSetBehavior() {
            // Arrange
            HttpContextBase httpContext = new Mock<HttpContextBase>().Object;
            ControllerContext controllerContext = new ControllerContext();

            // Act & assert
            MemberHelper.TestPropertyValue(controllerContext, "HttpContext", httpContext);
        }

        [TestMethod]
        public void HttpContextPropertyReturnsEmptyHttpContextIfRequestContextNotPresent() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();

            // Act
            HttpContextBase httpContext = controllerContext.HttpContext;
            HttpContextBase httpContext2 = controllerContext.HttpContext;

            // Assert
            Assert.IsNotNull(httpContext, "Should have created a default HttpContextBase.");
            Assert.AreEqual(httpContext, httpContext2, "Should cache the property.");
        }

        [TestMethod]
        public void HttpContextPropertyReturnsRequestContextHttpContextIfPresent() {
            // Arrange
            HttpContextBase httpContext = new Mock<HttpContextBase>().Object;
            RouteData routeData = new RouteData();
            RequestContext requestContext = new RequestContext(httpContext, routeData);
            Controller controller = new Mock<Controller>().Object;

            // Act
            ControllerContext controllerContext = new ControllerContext(requestContext, controller);

            // Assert
            Assert.AreEqual(httpContext, controllerContext.HttpContext);
        }

        [TestMethod]
        public void RequestContextPropertyCreatesDummyHttpContextAndRouteDataIfNecessary() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext).Returns((HttpContextBase)null);
            mockControllerContext.Expect(c => c.RouteData).Returns((RouteData)null);
            ControllerContext controllerContext = mockControllerContext.Object;

            // Act
            RequestContext requestContext = controllerContext.RequestContext;
            RequestContext requestContext2 = controllerContext.RequestContext;

            // Assert
            Assert.AreEqual(requestContext, requestContext2, "Should have cached property.");
            Assert.IsNotNull(requestContext.HttpContext, "HttpContext was not instantiated properly.");
            Assert.IsNotNull(requestContext.RouteData, "RouteData was not instantiated properly.");
        }

        [TestMethod]
        public void RequestContextPropertyUsesExistingHttpContextAndRouteData() {
            // Arrange
            HttpContextBase httpContext = new Mock<HttpContextBase>().Object;
            RouteData routeData = new RouteData();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext).Returns(httpContext);
            mockControllerContext.Expect(c => c.RouteData).Returns(routeData);
            ControllerContext controllerContext = mockControllerContext.Object;

            // Act
            RequestContext requestContext = controllerContext.RequestContext;
            RequestContext requestContext2 = controllerContext.RequestContext;

            // Assert
            Assert.AreEqual(requestContext, requestContext2, "Should have cached property.");
            Assert.AreEqual(httpContext, requestContext.HttpContext, "HttpContext was not recorded properly.");
            Assert.AreEqual(routeData, requestContext.RouteData, "RouteData was not recorded properly.");
        }

        [TestMethod]
        public void RouteDataPropertyGetSetBehavior() {
            // Arrange
            RouteData routeData = new RouteData();
            ControllerContext controllerContext = new ControllerContext();

            // Act & assert
            MemberHelper.TestPropertyValue(controllerContext, "RouteData", routeData);
        }

        [TestMethod]
        public void RouteDataPropertyReturnsEmptyRouteDataIfRequestContextNotPresent() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();

            // Act
            RouteData routeData = controllerContext.RouteData;
            RouteData routeData2 = controllerContext.RouteData;

            // Assert
            Assert.AreEqual(routeData, routeData2, "Should cache the property.");
            Assert.AreEqual(0, routeData.Values.Count, "Should have returned an empty RouteData.");
        }

        [TestMethod]
        public void RouteDataPropertyReturnsRequestContextRouteDataIfPresent() {
            // Arrange
            HttpContextBase httpContext = new Mock<HttpContextBase>().Object;
            RouteData routeData = new RouteData();
            RequestContext requestContext = new RequestContext(httpContext, routeData);
            Controller controller = new Mock<Controller>().Object;

            // Act
            ControllerContext controllerContext = new ControllerContext(requestContext, controller);

            // Assert
            Assert.AreEqual(routeData, controllerContext.RouteData);
        }

        public static ControllerContext CreateEmptyContext() {
            return new ControllerContext(new Mock<HttpContextBase>().Object, new RouteData(), new Mock<Controller>().Object);
        }

        private class SubclassedControllerContext : ControllerContext {
            public SubclassedControllerContext(ControllerContext controllerContext)
                : base(controllerContext) {
            }
        }

    }
}
