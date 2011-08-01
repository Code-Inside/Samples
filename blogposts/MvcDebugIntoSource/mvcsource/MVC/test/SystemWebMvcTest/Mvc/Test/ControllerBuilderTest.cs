namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ControllerBuilderTest {

        [TestMethod]
        public void ControllerBuilderReturnsDefaultControllerBuilderByDefault() {
            // Arrange
            ControllerBuilder cb = new ControllerBuilder();

            // Act
            IControllerFactory cf = cb.GetControllerFactory();

            // Assert
            Assert.IsInstanceOfType(cf, typeof(DefaultControllerFactory));
        }

        [TestMethod]
        public void CreateControllerWithFactoryThatCannotBeCreatedThrows() {
            // Arrange
            ControllerBuilder cb = new ControllerBuilder();
            cb.SetControllerFactory(typeof(ControllerFactoryThrowsFromConstructor));

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    RequestContext reqContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
                    reqContext.RouteData.Values["controller"] = "foo";
                    MvcHandlerWithNoVersionHeader handler = new MvcHandlerWithNoVersionHeader(reqContext) {
                        ControllerBuilder = cb
                    };
                    handler.ProcessRequest(reqContext.HttpContext);
                },
                "There was an error creating the IControllerFactory 'System.Web.Mvc.Test.ControllerBuilderTest+ControllerFactoryThrowsFromConstructor'. Check that it has a public parameterless constructor.");
        }

        [TestMethod]
        public void CreateControllerWithFactoryThatReturnsNullThrows() {
            // Arrange
            ControllerBuilder cb = new ControllerBuilder();
            cb.SetControllerFactory(typeof(ControllerFactoryReturnsNull));

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    RequestContext reqContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
                    reqContext.RouteData.Values["controller"] = "boo";
                    MvcHandlerWithNoVersionHeader handler = new MvcHandlerWithNoVersionHeader(reqContext) {
                        ControllerBuilder = cb
                    };
                    handler.ProcessRequest(reqContext.HttpContext);
                },
                "The IControllerFactory 'System.Web.Mvc.Test.ControllerBuilderTest+ControllerFactoryReturnsNull' did not return a controller for a controller named 'boo'.");
        }

        [TestMethod]
        public void CreateControllerWithFactoryThatThrowsDoesNothingSpecial() {
            // Arrange
            ControllerBuilder cb = new ControllerBuilder();
            cb.SetControllerFactory(typeof(ControllerFactoryThrows));

            // Act
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    RequestContext reqContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
                    reqContext.RouteData.Values["controller"] = "foo";
                    MvcHandlerWithNoVersionHeader handler = new MvcHandlerWithNoVersionHeader(reqContext) {
                        ControllerBuilder = cb
                    };
                    handler.ProcessRequest(reqContext.HttpContext);
                },
                "ControllerFactoryThrows");
        }

        [TestMethod]
        public void CreateControllerWithFactoryInstanceReturnsInstance() {
            // Arrange
            ControllerBuilder cb = new ControllerBuilder();
            DefaultControllerFactory factory = new DefaultControllerFactory();
            cb.SetControllerFactory(factory);

            // Act
            IControllerFactory cf = cb.GetControllerFactory();

            // Assert
            Assert.AreSame(factory, cf);
        }

        [TestMethod]
        public void CreateControllerWithFactoryTypeReturnsValidType() {
            // Arrange
            ControllerBuilder cb = new ControllerBuilder();
            cb.SetControllerFactory(typeof(MockControllerFactory));

            // Act
            IControllerFactory cf = cb.GetControllerFactory();

            // Assert
            Assert.IsInstanceOfType(cf, typeof(MockControllerFactory));
        }

        [TestMethod]
        public void SetControllerFactoryInstanceWithNullThrows() {
            ControllerBuilder cb = new ControllerBuilder();
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    cb.SetControllerFactory((IControllerFactory)null);
                },
                "controllerFactory");
        }

        [TestMethod]
        public void SetControllerFactoryTypeWithNullThrows() {
            ControllerBuilder cb = new ControllerBuilder();
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    cb.SetControllerFactory((Type)null);
                },
                "controllerFactoryType");
        }

        [TestMethod]
        public void SetControllerFactoryTypeWithNonFactoryTypeThrows() {
            ControllerBuilder cb = new ControllerBuilder();
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    cb.SetControllerFactory(typeof(int));
                },
                "The controller factory type 'System.Int32' must implement the IControllerFactory interface.\r\nParameter name: controllerFactoryType");
        }

        public class ControllerFactoryThrowsFromConstructor : IControllerFactory {
            public ControllerFactoryThrowsFromConstructor() {
                throw new Exception("ControllerFactoryThrowsFromConstructor");
            }

            public IController CreateController(RequestContext context, string controllerName) {
                return null;
            }

            public void ReleaseController(IController controller) {
            }
        }

        public class ControllerFactoryReturnsNull : IControllerFactory {
            public IController CreateController(RequestContext context, string controllerName) {
                return null;
            }

            public void ReleaseController(IController controller) {
            }
        }

        public class ControllerFactoryThrows : IControllerFactory {
            public IController CreateController(RequestContext context, string controllerName) {
                throw new Exception("ControllerFactoryThrows");
            }

            public void ReleaseController(IController controller) {
            }
        }

        public class MockControllerFactory : IControllerFactory {

            public IController CreateController(RequestContext context, string controllerName) {
                throw new NotImplementedException();
            }

            public void ReleaseController(IController controller) {
            }
        }

        private sealed class MvcHandlerWithNoVersionHeader : MvcHandler {
            public MvcHandlerWithNoVersionHeader(RequestContext requestContext)
                : base(requestContext) {
            }

            protected internal override void AddVersionHeader(HttpContextBase httpContext) {
                // Don't try to set the version header for the unit tests
            }
        }
    }
}
