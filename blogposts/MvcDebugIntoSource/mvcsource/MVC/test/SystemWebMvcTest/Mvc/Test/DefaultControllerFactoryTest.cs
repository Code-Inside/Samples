namespace System.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DefaultControllerFactoryTest {
        static DefaultControllerFactoryTest() {
            MvcTestHelper.CreateMvcAssemblies();
        }

        [TestMethod]
        public void CreateControllerWithNullContextThrows() {
            // Arrange
            DefaultControllerFactory factory = new DefaultControllerFactory();

            // Act
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ((IControllerFactory)factory).CreateController(
                        null,
                        "foo");
                },
                "requestContext");
        }

        [TestMethod]
        public void CreateControllerWithEmptyControllerNameThrows() {
            // Arrange
            DefaultControllerFactory factory = new DefaultControllerFactory();

            // Act
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ((IControllerFactory)factory).CreateController(
                        new RequestContext(new Mock<HttpContextBase>().Object, new RouteData()),
                        String.Empty);
                },
                "Value cannot be null or empty.\r\nParameter name: controllerName");
        }

        [TestMethod]
        public void CreateControllerReturnsControllerInstance() {
            // Arrange
            Mock<DefaultControllerFactory> factoryMock = new Mock<DefaultControllerFactory>();
            factoryMock.CallBase = true;
            factoryMock.Expect(o => o.GetControllerType("moo")).Returns(typeof(DummyController));

            // Act
            IController controller = ((IControllerFactory)factoryMock.Object).CreateController(
                new RequestContext(new Mock<HttpContextBase>().Object, new RouteData()),
                "moo");

            // Assert
            Assert.IsInstanceOfType(controller, typeof(DummyController));
        }

        [TestMethod]
        public void CreateControllerCanReturnNull() {
            // Arrange
            Mock<DefaultControllerFactory> factoryMock = new Mock<DefaultControllerFactory>();
            factoryMock.Expect(o => o.GetControllerType("moo")).Returns(typeof(DummyController));
            factoryMock.Expect(o => o.GetControllerInstance(typeof(DummyController))).Returns((ControllerBase)null);

            // Act
            IController controller = ((IControllerFactory)factoryMock.Object).CreateController(
                new RequestContext(new Mock<HttpContextBase>().Object, new RouteData()),
                "moo");

            // Assert
            Assert.IsNull(controller, "It should be OK for CreateController to return null");
        }

        [TestMethod]
        public void DisposeControllerFactoryWithDisposableController() {
            // Arrange
            IControllerFactory factory = new DefaultControllerFactory();
            Mock<ControllerBase> mockController = new Mock<ControllerBase>();
            IMock<IDisposable> mockDisposable = mockController.As<IDisposable>();
            mockDisposable.Expect(d => d.Dispose()).Verifiable();

            // Act
            factory.ReleaseController(mockController.Object);

            // Assert
            mockDisposable.Verify();
        }

        [TestMethod]
        public void GetControllerInstanceThrowsIfControllerTypeIsNull() {
            // Arrange
            Mock<DefaultControllerFactory> factoryMock = new Mock<DefaultControllerFactory> { CallBase = true };
            factoryMock.Expect(o => o.GetControllerType("moo")).Returns((Type)null);
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> requestMock = new Mock<HttpRequestBase>();
            contextMock.Expect(o => o.Request).Returns(requestMock.Object);
            requestMock.Expect(o => o.Path).Returns("somepath");

            // Act
            ExceptionHelper.ExpectHttpException(
                delegate {
                    ((IControllerFactory)factoryMock.Object).CreateController(
                        new RequestContext(contextMock.Object, new RouteData()),
                        "moo");
                },
                "The controller for path 'somepath' could not be found or it does not implement IController.",
                404);
        }

        [TestMethod]
        public void GetControllerInstanceThrowsIfControllerTypeIsNotControllerBase() {
            // Arrange
            DefaultControllerFactory factory = new DefaultControllerFactory();

            // Act
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    factory.GetControllerInstance(typeof(int));
                },
                "The controller type 'System.Int32' must implement IController.\r\nParameter name: controllerType");
        }

        [TestMethod]
        public void GetControllerInstanceWithBadConstructorThrows() {
            // Arrange
            Mock<DefaultControllerFactory> factoryMock = new Mock<DefaultControllerFactory>();
            factoryMock.CallBase = true;
            factoryMock.Expect(o => o.GetControllerType("moo")).Returns(typeof(DummyControllerThrows));
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();

            // Act
            Exception ex = ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    ((IControllerFactory)factoryMock.Object).CreateController(
                        new RequestContext(contextMock.Object, new RouteData()),
                        "moo");
                },
                "An error occurred while creating a controller of type 'System.Web.Mvc.Test.DefaultControllerFactoryTest+DummyControllerThrows'. If the controller doesn't have a controller factory, ensure that it has a parameterless public constructor.");

            Assert.AreEqual<string>("constructor", ex.InnerException.InnerException.Message);
        }

        [TestMethod]
        public void GetControllerTypeWithEmptyControllerNameThrows() {
            // Arrange
            DefaultControllerFactory factory = new DefaultControllerFactory();

            // Act
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    factory.GetControllerType(
                        String.Empty);
                },
                "Value cannot be null or empty.\r\nParameter name: controllerName");
        }

        [TestMethod]
        public void GetControllerTypeForNoAssemblies() {
            // Arrange
            DefaultControllerFactory factory = new DefaultControllerFactory();
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;

            // Act
            Type controllerType = factory.GetControllerType("sometype");

            // Assert
            Assert.IsNull(controllerType, "Shouldn't have found a controller type.");
            Assert.AreEqual<int>(0, controllerTypeCache.Count, "Cache should be empty.");
        }

        [TestMethod]
        public void GetControllerTypeForOneAssembly() {
            // Arrange
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b", "ns2a.ns2b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;

            // Act
            Type c1Type = factory.GetControllerType("C1");
            Type c2Type = factory.GetControllerType("c2");

            // Assert
            Assembly asm1 = Assembly.Load("MvcAssembly1");
            Type verifiedC1 = asm1.GetType("NS1a.NS1b.C1Controller");
            Type verifiedC2 = asm1.GetType("NS2a.NS2b.C2Controller");
            Assert.AreEqual<Type>(verifiedC1, c1Type, "Should have found C1Controller type.");
            Assert.AreEqual<Type>(verifiedC2, c2Type, "Should have found C2Controller type.");
            Assert.AreEqual<int>(2, controllerTypeCache.Count, "Cache should have 2 controller types.");
        }

        [TestMethod]
        public void GetControllerTypeForManyAssemblies() {
            // Arrange
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b", "ns2a.ns2b", "ns3a.ns3b", "ns4a.ns4b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1"), Assembly.Load("MvcAssembly2") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;

            // Act
            Type c1Type = factory.GetControllerType("C1");
            Type c2Type = factory.GetControllerType("C2");
            Type c3Type = factory.GetControllerType("c3"); // lower case
            Type c4Type = factory.GetControllerType("c4"); // lower case

            // Assert
            Assembly asm1 = Assembly.Load("MvcAssembly1");
            Type verifiedC1 = asm1.GetType("NS1a.NS1b.C1Controller");
            Type verifiedC2 = asm1.GetType("NS2a.NS2b.C2Controller");
            Assembly asm2 = Assembly.Load("MvcAssembly2");
            Type verifiedC3 = asm2.GetType("NS3a.NS3b.C3Controller");
            Type verifiedC4 = asm2.GetType("NS4a.NS4b.C4Controller");
            Assert.IsNotNull(verifiedC1, "Couldn't find real C1 type");
            Assert.IsNotNull(verifiedC2, "Couldn't find real C2 type");
            Assert.IsNotNull(verifiedC3, "Couldn't find real C3 type");
            Assert.IsNotNull(verifiedC4, "Couldn't find real C4 type");
            Assert.AreEqual<Type>(verifiedC1, c1Type, "Should have found C1Controller type.");
            Assert.AreEqual<Type>(verifiedC2, c2Type, "Should have found C2Controller type.");
            Assert.AreEqual<Type>(verifiedC3, c3Type, "Should have found C3Controller type.");
            Assert.AreEqual<Type>(verifiedC4, c4Type, "Should have found C4Controller type.");
            Assert.AreEqual<int>(4, controllerTypeCache.Count, "Cache should have 4 controller types.");
        }

        [TestMethod]
        public void GetControllerTypeForAssembliesWithSameTypeNamesInDifferentNamespaces() {
            // Arrange
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b", "ns2a.ns2b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1"), Assembly.Load("MvcAssembly3") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;

            // Act
            Type c1Type = factory.GetControllerType("C1");
            Type c2Type = factory.GetControllerType("C2");

            // Assert
            Assembly asm1 = Assembly.Load("MvcAssembly1");
            Type verifiedC1 = asm1.GetType("NS1a.NS1b.C1Controller");
            Type verifiedC2 = asm1.GetType("NS2a.NS2b.C2Controller");
            Assert.IsNotNull(verifiedC1, "Couldn't find real C1 type");
            Assert.IsNotNull(verifiedC2, "Couldn't find real C2 type");
            Assert.AreEqual<Type>(verifiedC1, c1Type, "Should have found C1Controller type.");
            Assert.AreEqual<Type>(verifiedC2, c2Type, "Should have found C2Controller type.");
            Assert.AreEqual<int>(4, controllerTypeCache.Count, "Cache should have 4 controller types.");
        }

        [TestMethod]
        public void GetControllerTypeForAssembliesWithSameTypeNamesInDifferentNamespacesThrowsIfAmbiguous() {
            // Arrange
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b", "ns3a.ns3b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1"), Assembly.Load("MvcAssembly3") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    factory.GetControllerType("C1");
                },
                @"The controller name 'C1' is ambiguous between the following types:
NS1a.NS1b.C1Controller
NS3a.NS3b.C1Controller");

            // Assert
            Assert.AreEqual<int>(4, controllerTypeCache.Count, "Cache should have 4 controller types.");
        }

        [TestMethod]
        public void GetControllerTypeForAssembliesWithSameTypeNamesInSameNamespaceThrows() {
            // Arrange
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1"), Assembly.Load("MvcAssembly4") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    factory.GetControllerType("C1");
                },
                @"The controller name 'C1' is ambiguous between the following types:
NS1a.NS1b.C1Controller
NS1a.NS1b.C1Controller");

            // Assert
            Assert.AreEqual<int>(4, controllerTypeCache.Count, "Cache should have 4 controller types.");
        }

        [TestMethod]
        public void GetControllerTypeSearchesAllNamespacesAsLastResort() {
            // Arrange
            RequestContext requestContext = GetRequestContextWithNamespaces("ns3a.ns3b");
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;
            factory.RequestContext = requestContext;

            // Act
            Type c2Type = factory.GetControllerType("C2");

            // Assert
            Assembly asm1 = Assembly.Load("MvcAssembly1");
            Type verifiedC2 = asm1.GetType("NS2a.NS2b.C2Controller");
            Assert.IsNotNull(verifiedC2, "Couldn't find real C2 type");
            Assert.AreEqual<Type>(verifiedC2, c2Type, "Should have found C2Controller type.");
            Assert.AreEqual<int>(2, controllerTypeCache.Count, "Cache should have 2 controller types.");
        }

        [TestMethod]
        public void GetControllerTypeSearchesRouteDefinedNamespacesBeforeApplicationDefinedNamespaces() {
            // Arrange
            RequestContext requestContext = GetRequestContextWithNamespaces("ns3a.ns3b");
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b", "ns2a.ns2b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1"), Assembly.Load("MvcAssembly3") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;
            factory.RequestContext = requestContext;

            // Act
            Type c1Type = factory.GetControllerType("C1");
            Type c2Type = factory.GetControllerType("C2");

            // Assert
            Assembly asm1 = Assembly.Load("MvcAssembly1");
            Type verifiedC2 = asm1.GetType("NS2a.NS2b.C2Controller");
            Assembly asm3 = Assembly.Load("MvcAssembly3");
            Type verifiedC1 = asm3.GetType("NS3a.NS3b.C1Controller");
            Assert.IsNotNull(verifiedC1, "Couldn't find real C1 type");
            Assert.IsNotNull(verifiedC2, "Couldn't find real C2 type");
            Assert.AreEqual<Type>(verifiedC1, c1Type, "Should have found C1Controller type.");
            Assert.AreEqual<Type>(verifiedC2, c2Type, "Should have found C2Controller type.");
            Assert.AreEqual<int>(4, controllerTypeCache.Count, "Cache should have 4 controller types.");
        }

        [TestMethod]
        public void GetControllerTypeThatDoesntExist() {
            // Arrange
            DefaultControllerFactory factory = GetDefaultControllerFactory("ns1a.ns1b", "ns2a.ns2b", "ns3a.ns3b", "ns4a.ns4b");
            MockBuildManager buildManagerMock = new MockBuildManager(new Assembly[] { Assembly.Load("MvcAssembly1"), Assembly.Load("MvcAssembly2"), Assembly.Load("MvcAssembly3"), Assembly.Load("MvcAssembly4") });
            ControllerTypeCache controllerTypeCache = new ControllerTypeCache();

            factory.BuildManager = buildManagerMock;
            factory.ControllerTypeCache = controllerTypeCache;

            // Act
            Type randomType1 = factory.GetControllerType("Cx");
            Type randomType2 = factory.GetControllerType("Cy");
            Type randomType3 = factory.GetControllerType("Foo.Bar");
            Type randomType4 = factory.GetControllerType("C1Controller");

            // Assert
            Assert.IsNull(randomType1, "Controller type should not have been found.");
            Assert.IsNull(randomType2, "Controller type should not have been found.");
            Assert.IsNull(randomType3, "Controller type should not have been found.");
            Assert.IsNull(randomType4, "Controller type should not have been found.");
            Assert.AreEqual<int>(8, controllerTypeCache.Count, "Cache should have 8 controller types.");
        }

        [TestMethod]
        public void IsControllerType() {
            // Act
            bool isController1 = ControllerTypeCache.IsControllerType(null);
            bool isController2 = ControllerTypeCache.IsControllerType(typeof(NonPublicController));
            bool isController3 = ControllerTypeCache.IsControllerType(typeof(MisspelledKontroller));
            bool isController4 = ControllerTypeCache.IsControllerType(typeof(AbstractController));
            bool isController5 = ControllerTypeCache.IsControllerType(typeof(NonIControllerController));
            bool isController6 = ControllerTypeCache.IsControllerType(typeof(Goodcontroller));

            // Assert
            Assert.IsFalse(isController1, "IsControllerType(null) should return false.");
            Assert.IsFalse(isController2, "Non-public types should not be considered controller types.");
            Assert.IsFalse(isController3, "Types not ending in 'Controller' should not be considered controller types.");
            Assert.IsFalse(isController4, "Abstract types should not be considered controller types.");
            Assert.IsFalse(isController5, "Types not implementing IController should not be considered controller types.");
            Assert.IsTrue(isController6, "The 'Controller' suffix on controller types is not required to be case-sensitive.");
        }

        private static DefaultControllerFactory GetDefaultControllerFactory(params string[] namespaces) {
            ControllerBuilder builder = new ControllerBuilder();
            builder.DefaultNamespaces.UnionWith(namespaces);
            return new DefaultControllerFactory() { ControllerBuilder = builder };
        }

        private static RequestContext GetRequestContextWithNamespaces(params string[] namespaces) {
            RouteData routeData = new RouteData();
            routeData.DataTokens["namespaces"] = namespaces;
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            RequestContext requestContext = new RequestContext(mockHttpContext.Object, routeData);
            return requestContext;
        }

        private sealed class DummyController : ControllerBase {
            protected override void ExecuteCore() {
                throw new NotImplementedException();
            }
        }

        private sealed class DummyControllerThrows : IController {
            public DummyControllerThrows() {
                throw new Exception("constructor");
            }

            #region IController Members
            void IController.Execute(RequestContext requestContext) {
                throw new NotImplementedException();
            }
            #endregion
        }

        public interface IDisposableController : IController, IDisposable {
        }
    }

    // BAD: type isn't public
    internal class NonPublicController : Controller {
    }

    // BAD: type doesn't end with 'Controller'
    public class MisspelledKontroller : Controller {
    }

    // BAD: type is abstract
    public abstract class AbstractController : Controller {
    }

    // BAD: type doesn't implement IController
    public class NonIControllerController {
    }

    // GOOD: 'Controller' suffix should be case-insensitive
    public class Goodcontroller : Controller {
    }
}
