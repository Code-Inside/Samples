namespace System.Web.Mvc.Test {
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RouteCollectionExtensionsTest {
        private static string[] _nameSpaces = new string[] {"nsA.nsB.nsC", "ns1.ns2.ns3" };

        [TestMethod]
        public void MapRoute3() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.MapRoute("RouteName", "SomeUrl");

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute3WithNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            //string[] namespaces = new string[] { "nsA.nsB.nsC", "ns1.ns2.ns3" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", _nameSpaces);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.DataTokens);
            Assert.IsNotNull(route.DataTokens["Namespaces"]);
            string[] routeNameSpaces = route.DataTokens["Namespaces"] as string[];
            Assert.AreEqual(routeNameSpaces.Length, 2);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreSame(routeNameSpaces, _nameSpaces);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute3WithEmptyNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.MapRoute("RouteName", "SomeUrl", new string[] {});

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute4() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", defaults);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute4WithNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", defaults, _nameSpaces);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.DataTokens);
            Assert.IsNotNull(route.DataTokens["Namespaces"]);
            string[] routeNameSpaces = route.DataTokens["Namespaces"] as string[];
            Assert.AreEqual(routeNameSpaces.Length, 2);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreSame(routeNameSpaces, _nameSpaces);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute5() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };
            var constraints = new { Foo = "ConstraintFoo" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", defaults, constraints);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual("ConstraintFoo", route.Constraints["Foo"]);
        }

        [TestMethod]
        public void MapRoute5WithNullRouteCollectionThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    RouteCollectionExtensions.MapRoute(null, null, null, null, null);
                },
                "routes");
        }

        [TestMethod]
        public void MapRoute5WithNullUrlThrows() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    routes.MapRoute(null, null /* url */, null, null);
                },
                "url");
        }

        [TestMethod]
        public void IgnoreRoute1WithNullRouteCollectionThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    RouteCollectionExtensions.IgnoreRoute(null, "foo");
                },
                "routes");
        }

        [TestMethod]
        public void IgnoreRoute1WithNullUrlThrows() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    routes.IgnoreRoute(null);
                },
                "url");
        }

        [TestMethod]
        public void IgnoreRoute3() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.IgnoreRoute("SomeUrl");

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(StopRoutingHandler));
            Assert.IsNull(route.Defaults);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void IgnoreRoute4() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var constraints = new { Foo = "DefaultFoo" };

            // Act
            routes.IgnoreRoute("SomeUrl", constraints);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(StopRoutingHandler));
            Assert.IsNull(route.Defaults);
            Assert.AreEqual(1, route.Constraints.Count);
            Assert.AreEqual("DefaultFoo", route.Constraints["Foo"]);
        }

        [TestMethod]
        public void IgnoreRouteInternalNeverMatchesUrlGeneration() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            routes.IgnoreRoute("SomeUrl");
            Route route = routes[0] as Route;

            // Act
            VirtualPathData vpd = route.GetVirtualPath(new RequestContext(new Mock<HttpContextBase>().Object, new RouteData()), null);

            // Assert
            Assert.IsNull(vpd);
        }
    }
}
