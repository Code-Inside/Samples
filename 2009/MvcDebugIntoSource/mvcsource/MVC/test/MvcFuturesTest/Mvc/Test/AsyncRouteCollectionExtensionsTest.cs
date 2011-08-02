namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class AsyncRouteCollectionExtensionsTest {
        private static string[] _nameSpaces = new string[] { "nsA.nsB.nsC", "ns1.ns2.ns3" };

        [TestMethod]
        public void MapRoute3() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.MapAsyncRoute("RouteName", "SomeUrl");

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcAsyncRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute3WithNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            //string[] namespaces = new string[] { "nsA.nsB.nsC", "ns1.ns2.ns3" };

            // Act
            routes.MapAsyncRoute("RouteName", "SomeUrl", _nameSpaces);

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
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcAsyncRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute3WithEmptyNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.MapAsyncRoute("RouteName", "SomeUrl", new string[] { });

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcAsyncRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute4() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };

            // Act
            routes.MapAsyncRoute("RouteName", "SomeUrl", defaults);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcAsyncRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute4WithNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };

            // Act
            routes.MapAsyncRoute("RouteName", "SomeUrl", defaults, _nameSpaces);

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
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcAsyncRouteHandler));
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
            routes.MapAsyncRoute("RouteName", "SomeUrl", defaults, constraints);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNull(route.DataTokens);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcAsyncRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual("ConstraintFoo", route.Constraints["Foo"]);
        }

    }
}
