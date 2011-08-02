namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RedirectToRouteResultTest {

        [TestMethod]
        public void ConstructorWithNullValuesDictionary() {
            // Act
            var result = new RedirectToRouteResult(null /* routeValues */);

            // Assert
            Assert.IsNotNull(result.RouteValues);
            Assert.AreEqual<int>(0, result.RouteValues.Count);
            Assert.AreEqual<string>(String.Empty, result.RouteName);
        }

        [TestMethod]
        public void ConstructorSetsValuesDictionary() {
            // Arrange
            RouteValueDictionary dict = new RouteValueDictionary();

            // Act
            var result = new RedirectToRouteResult(dict);

            // Assert
            Assert.AreSame(dict, result.RouteValues);
            Assert.AreEqual<string>(String.Empty, result.RouteName);
        }

        [TestMethod]
        public void ConstructorSetsValuesDictionaryAndEmptyName() {
            // Arrange
            RouteValueDictionary dict = new RouteValueDictionary();

            // Act
            var result = new RedirectToRouteResult(null, dict);

            // Assert
            Assert.AreSame(dict, result.RouteValues);
            Assert.AreEqual<string>(String.Empty, result.RouteName);
        }

        [TestMethod]
        public void ConstructorSetsValuesDictionaryAndName() {
            // Arrange
            RouteValueDictionary dict = new RouteValueDictionary();

            // Act
            var result = new RedirectToRouteResult("foo", dict);

            // Assert
            Assert.AreSame(dict, result.RouteValues);
            Assert.AreEqual<string>("foo", result.RouteName);
        }

        [TestMethod]
        public void ExecuteResult() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.ApplicationPath).Returns("/somepath");
            mockControllerContext.Expect(c => c.HttpContext.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns((string s) => s);
            mockControllerContext.Expect(c => c.HttpContext.Response.Redirect("/somepath/c/a/i", false)).Verifiable();

            var values = new { Controller = "c", Action = "a", Id = "i" };
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary(values)) {
                Routes = new RouteCollection() { new Route("{controller}/{action}/{id}", null) },
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResultThrowsIfVirtualPathDataIsNull() {
            // Arrange
            var result = new RedirectToRouteResult(null) {
                Routes = new RouteCollection()
            };

            // Act & Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    result.ExecuteResult(ControllerContextTest.CreateEmptyContext());
                },
                "No route in the route table matches the supplied values.");
        }

        [TestMethod]
        public void ExecuteResultWithNullControllerContextThrows() {
            // Arrange
            var result = new RedirectToRouteResult(null);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    result.ExecuteResult(null /* context */);
                },
                "context");
        }

        [TestMethod]
        public void RoutesPropertyDefaultsToGlobalRouteTable() {
            // Act
            var result = new RedirectToRouteResult(new RouteValueDictionary());

            // Assert
            Assert.AreSame(RouteTable.Routes, result.Routes);
        }
    }
}
