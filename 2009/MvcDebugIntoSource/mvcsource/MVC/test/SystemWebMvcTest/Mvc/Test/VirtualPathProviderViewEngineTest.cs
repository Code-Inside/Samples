namespace System.Web.Mvc.Test {
    using System.Linq;
    using System.Web.Hosting;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class VirtualPathProviderViewEngineTest {

        [TestMethod]
        public void FindViewWithNullControllerContextThrows() {
            // Arrange
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => engine.FindView(null, "view name", null, false),
                "controllerContext"
            );
        }

        [TestMethod]
        public void FindViewWithNullViewNameThrows() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => engine.FindView(context, null, null, false),
                "viewName"
            );
        }

        [TestMethod]
        public void FindViewWithEmptyViewNameThrows() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => engine.FindView(context, "", null, false),
                "viewName"
            );
        }

        [TestMethod]
        public void FindViewControllerNameMustExistInRequestContext() {
            // Arrange
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            ControllerContext context = CreateContext();
            context.RouteData.Values.Remove("controller");

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => engine.FindView(context, "viewName", null, false),
                "The RouteData must contain an item named 'controller' with a non-empty string value."
            );
        }

        [TestMethod]
        public void FindViewViewLocationsCannotBeEmpty() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.ClearViewLocations();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => engine.FindView(context, "viewName", null, false),
                "The property 'ViewLocationFormats' cannot be null or empty."
            );
        }

        [TestMethod]
        public void CannotFindViewNoMaster() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/viewName.view"))
                .Returns(false)
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindView(context, "viewName", null, false);

            // Assert
            Assert.IsNull(result.View);
            Assert.AreEqual(1, result.SearchedLocations.Count());
            Assert.IsTrue(result.SearchedLocations.Contains("~/vpath/controllerName/viewName.view"));
            engine.MockPathProvider.Verify();
        }

        [TestMethod]
        public void ViewFoundNoMaster() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.ClearMasterLocations(); // If master is not provided, master locations can be empty
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/viewName.view"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/vpath/controllerName/viewName.view"))
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindView(context, "viewName", null, false);

            // Assert
            Assert.AreSame(engine.CreateViewResult, result.View);
            Assert.IsNull(result.SearchedLocations);
            Assert.AreSame(context, engine.CreateViewControllerContext);
            Assert.AreEqual("~/vpath/controllerName/viewName.view", engine.CreateViewViewPath);
            Assert.AreEqual(String.Empty, engine.CreateViewMasterPath);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void SpecificVirtualPathViewFoundNoMaster() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.ClearMasterLocations();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/foo/bar"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/foo/bar"))
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindView(context, "~/foo/bar", null, false);

            // Assert
            Assert.AreSame(engine.CreateViewResult, result.View);
            Assert.IsNull(result.SearchedLocations);
            Assert.AreSame(context, engine.CreateViewControllerContext);
            Assert.AreEqual("~/foo/bar", engine.CreateViewViewPath);
            Assert.AreEqual(String.Empty, engine.CreateViewMasterPath);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void SpecificAbsolutePathViewFoundNoMaster() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.ClearMasterLocations();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("/foo/bar"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "/foo/bar"))
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindView(context, "/foo/bar", null, false);

            // Assert
            Assert.AreSame(engine.CreateViewResult, result.View);
            Assert.IsNull(result.SearchedLocations);
            Assert.AreSame(context, engine.CreateViewControllerContext);
            Assert.AreEqual("/foo/bar", engine.CreateViewViewPath);
            Assert.AreEqual(String.Empty, engine.CreateViewMasterPath);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void MasterLocationsCannotBeEmptyMasterProvided() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.ClearMasterLocations();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/viewName.view"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/vpath/controllerName/viewName.view"))
                .Verifiable();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => engine.FindView(context, "viewName", "masterName", false),
                "The property 'MasterLocationFormats' cannot be null or empty."
            );
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void CannotFindViewCannotFindMaster() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/viewName.view"))
                .Returns(false)
                .Verifiable();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/masterName.master"))
                .Returns(false)
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindView(context, "viewName", "masterName", false);

            // Assert
            Assert.IsNull(result.View);
            Assert.AreEqual(2, result.SearchedLocations.Count()); // Both view and master locations
            Assert.IsTrue(result.SearchedLocations.Contains("~/vpath/controllerName/viewName.view"));
            Assert.IsTrue(result.SearchedLocations.Contains("~/vpath/controllerName/masterName.master"));
            engine.MockPathProvider.Verify();
        }

        [TestMethod]
        public void FoundViewCannotFindMaster() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/viewName.view"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/vpath/controllerName/viewName.view"))
                .Verifiable();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/masterName.master"))
                .Returns(false)
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindView(context, "viewName", "masterName", false);

            // Assert
            Assert.IsNull(result.View);
            Assert.AreEqual(1, result.SearchedLocations.Count()); // View was found, not included in 'searched locations'
            Assert.IsTrue(result.SearchedLocations.Contains("~/vpath/controllerName/masterName.master"));
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void FoundViewFoundMaster() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/viewName.view"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/vpath/controllerName/viewName.view"))
                .Verifiable();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/masterName.master"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/vpath/controllerName/masterName.master"))
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindView(context, "viewName", "masterName", false);

            // Assert
            Assert.AreSame(engine.CreateViewResult, result.View);
            Assert.IsNull(result.SearchedLocations);
            Assert.AreSame(context, engine.CreateViewControllerContext);
            Assert.AreEqual("~/vpath/controllerName/viewName.view", engine.CreateViewViewPath);
            Assert.AreEqual("~/vpath/controllerName/masterName.master", engine.CreateViewMasterPath);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void FindPartialViewWithNullControllerContextThrows() {
            // Arrange
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => engine.FindPartialView(null, "view name", false),
                "controllerContext"
            );
        }

        [TestMethod]
        public void FindPartialViewWithNullViewNameThrows() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => engine.FindPartialView(context, null, false),
                "partialViewName"
            );
        }

        [TestMethod]
        public void FindPartialViewWithEmptyViewNameThrows() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => engine.FindPartialView(context, "", false),
                "partialViewName"
            );
        }

        [TestMethod]
        public void FindPartialViewControllerNameMustExistInRequestContext() {
            // Arrange
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            ControllerContext context = CreateContext();
            context.RouteData.Values.Remove("controller");

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => engine.FindPartialView(context, "partialName", false),
                "The RouteData must contain an item named 'controller' with a non-empty string value."
            );
        }

        [TestMethod]
        public void FindPartialViewViewLocationsCannotBeEmpty() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.ClearPartialViewLocations();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => engine.FindPartialView(context, "partialName", false),
                "The property 'PartialViewLocationFormats' cannot be null or empty."
            );
        }

        [TestMethod]
        public void CannotFindView() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/partialName.partial"))
                .Returns(false)
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindPartialView(context, "partialName", false);

            // Assert
            Assert.IsNull(result.View);
            Assert.AreEqual(1, result.SearchedLocations.Count());
            Assert.IsTrue(result.SearchedLocations.Contains("~/vpath/controllerName/partialName.partial"));
            engine.MockPathProvider.Verify();
        }

        [TestMethod]
        public void ViewFound() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/vpath/controllerName/partialName.partial"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/vpath/controllerName/partialName.partial"))
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindPartialView(context, "partialName", false);

            // Assert
            Assert.AreSame(engine.CreatePartialViewResult, result.View);
            Assert.IsNull(result.SearchedLocations);
            Assert.AreSame(context, engine.CreatePartialViewControllerContext);
            Assert.AreEqual("~/vpath/controllerName/partialName.partial", engine.CreatePartialViewPartialPath);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void SpecificVirtualPathViewFound() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("~/foo/bar"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "~/foo/bar"))
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindPartialView(context, "~/foo/bar", false);

            // Assert
            Assert.AreSame(engine.CreatePartialViewResult, result.View);
            Assert.IsNull(result.SearchedLocations);
            Assert.AreSame(context, engine.CreatePartialViewControllerContext);
            Assert.AreEqual("~/foo/bar", engine.CreatePartialViewPartialPath);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void SpecificAbsolutePathViewFound() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists("/foo/bar"))
                .Returns(true)
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), "/foo/bar"))
                .Verifiable();

            // Act
            ViewEngineResult result = engine.FindPartialView(context, "/foo/bar", false);

            // Assert
            Assert.AreSame(engine.CreatePartialViewResult, result.View);
            Assert.IsNull(result.SearchedLocations);
            Assert.AreSame(context, engine.CreatePartialViewControllerContext);
            Assert.AreEqual("/foo/bar", engine.CreatePartialViewPartialPath);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        // The core caching scenarios are covered in the FindView/FindPartialView tests. These
        // extra tests deal with the cache itself, rather than specifics around finding views.

        private const string MASTER_VIRTUAL = "~/vpath/controllerName/name.master";
        private const string PARTIAL_VIRTUAL = "~/vpath/controllerName/name.partial";
        private const string VIEW_VIRTUAL = "~/vpath/controllerName/name.view";

        [TestMethod]
        public void UsesDifferentKeysForViewMasterAndPartial() {
            string keyMaster = null;
            string keyPartial = null;
            string keyView = null;

            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists(VIEW_VIRTUAL))
                .Returns(true)
                .AtMostOnce()
                .Verifiable();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists(MASTER_VIRTUAL))
                .Returns(true)
                .AtMostOnce()
                .Verifiable();
            engine.MockPathProvider
                .Expect(vpp => vpp.FileExists(PARTIAL_VIRTUAL))
                .Returns(true)
                .AtMostOnce()
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), VIEW_VIRTUAL))
                .Callback<HttpContextBase, string, string>((httpContext, key, path) => keyView = key)
                .AtMostOnce()
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), MASTER_VIRTUAL))
                .Callback<HttpContextBase, string, string>((httpContext, key, path) => keyMaster = key)
                .AtMostOnce()
                .Verifiable();
            engine.MockCache
                .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>(), PARTIAL_VIRTUAL))
                .Callback<HttpContextBase, string, string>((httpContext, key, path) => keyPartial = key)
                .AtMostOnce()
                .Verifiable();

            // Act
            engine.FindView(context, "name", "name", false);
            engine.FindPartialView(context, "name", false);

            // Assert
            Assert.IsNotNull(keyMaster);
            Assert.IsNotNull(keyPartial);
            Assert.IsNotNull(keyView);
            Assert.AreNotEqual(keyMaster, keyPartial);
            Assert.AreNotEqual(keyMaster, keyView);
            Assert.AreNotEqual(keyPartial, keyView);
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        // This tests the protocol involved with two calls to FindView for the same view name
        // where the request succeeds. The calls happen in this order:
        //
        //    FindView("view")
        //      Cache.GetViewLocation(key for "view") -> returns null (not found)
        //      VirtualPathProvider.FileExists(virtual path for "view") -> returns true
        //      Cache.InsertViewLocation(key for "view", virtual path for "view")
        //    FindView("view")
        //      Cache.GetViewLocation(key for "view") -> returns virtual path for "view"
        //
        // The mocking code is written as it is because we don't want to make any assumptions
        // about the format of the cache key. So we intercept the first call to Cache.GetViewLocation and
        // take the key they gave us to set up the rest of the mock expectations.
        // The ViewCollection class will typically place to successive calls to FindView and FindPartialView and
        // set the useCache parameter to true/false respectively. To simulate this, both calls to FindView are executed
        // with useCache set to true. This mimics the behavior of always going to the cache first and after finding a
        // view, ensuring that subsequent calls from the cache are successful.

        [TestMethod]
        public void ValueInCacheBypassesVirtualPathProvider() {
            // Arrange
            ControllerContext context = CreateContext();
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            engine.MockCache
                .Expect(c => c.GetViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>()))    // Don't know what the key is yet
                .Returns((string)null)
                .Callback<HttpContextBase, string>((httpContext, key) => {
                    engine.MockPathProvider                // It wasn't found, so they call vpp.FileExists
                        .Expect(vpp => vpp.FileExists(VIEW_VIRTUAL))
                        .Returns(true)
                        .AtMostOnce()
                        .Verifiable();
                    engine.MockCache                       // Then they set the value into the cache
                        .Expect(c => c.InsertViewLocation(It.IsAny<HttpContextBase>(), key, VIEW_VIRTUAL))
                        .AtMostOnce()
                        .Verifiable();
                    engine.MockCache                       // Second time through, we give them a cache hit
                        .Expect(c => c.GetViewLocation(It.IsAny<HttpContextBase>(), key))
                        .Returns(VIEW_VIRTUAL)
                        .AtMostOnce()
                        .Verifiable();
                })
                .AtMostOnce()
                .Verifiable();

            // Act
            engine.FindView(context, "name", null, true);
            engine.FindView(context, "name", null, true);

            // Assert
            engine.MockPathProvider.Verify();
            engine.MockCache.Verify();
        }

        [TestMethod]
        public void ReleaseViewCallsDispose() {
            // Arrange
            TestableVirtualPathProviderViewEngine engine = new TestableVirtualPathProviderViewEngine();
            ControllerContext context = CreateContext();
            IView view = engine.CreateViewResult;

            // Act
            engine.ReleaseView(context, view);

            // Assert
            Assert.IsTrue(((TestView)view).Disposed);
        }

        private static ControllerContext CreateContext() {
            RouteData routeData = new RouteData();
            routeData.Values["controller"] = "controllerName";
            routeData.Values["action"] = "actionName";

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.RouteData).Returns(routeData);
            return mockControllerContext.Object;
        }

        private class TestView : IView, IDisposable {
            public bool Disposed {
                get;
                set;
            }

            #region IDisposable Members
            void IDisposable.Dispose() {
                Disposed = true;
            }
            #endregion

            #region IView Members
            void IView.Render(ViewContext viewContext, System.IO.TextWriter writer) {

            }
            #endregion
        }

        private class TestableVirtualPathProviderViewEngine : VirtualPathProviderViewEngine {
            public IView CreatePartialViewResult = new Mock<IView>().Object;
            public string CreatePartialViewPartialPath;
            public ControllerContext CreatePartialViewControllerContext;

            //public IView CreateViewResult = new Mock<IView>().Object;
            public IView CreateViewResult = new TestView();
            public string CreateViewMasterPath;
            public ControllerContext CreateViewControllerContext;
            public string CreateViewViewPath;

            public Mock<IViewLocationCache> MockCache = new Mock<IViewLocationCache>(MockBehavior.Strict);
            public Mock<VirtualPathProvider> MockPathProvider = new Mock<VirtualPathProvider>(MockBehavior.Strict);

            public TestableVirtualPathProviderViewEngine() {
                MasterLocationFormats = new[] { "~/vpath/{1}/{0}.master" };
                ViewLocationFormats = new[] { "~/vpath/{1}/{0}.view" };
                PartialViewLocationFormats = new[] { "~/vpath/{1}/{0}.partial" };

                ViewLocationCache = MockCache.Object;
                VirtualPathProvider = MockPathProvider.Object;

                MockCache
                    .Expect(c => c.GetViewLocation(It.IsAny<HttpContextBase>(), It.IsAny<string>()))
                    .Returns((string)null);
            }

            public void ClearViewLocations() {
                ViewLocationFormats = new string[0];
            }

            public void ClearMasterLocations() {
                MasterLocationFormats = new string[0];
            }

            public void ClearPartialViewLocations() {
                PartialViewLocationFormats = new string[0];
            }

            protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath) {
                CreatePartialViewControllerContext = controllerContext;
                CreatePartialViewPartialPath = partialPath;

                return CreatePartialViewResult;
            }

            protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath) {
                CreateViewControllerContext = controllerContext;
                CreateViewViewPath = viewPath;
                CreateViewMasterPath = masterPath;

                return CreateViewResult;
            }
        }
    }
}