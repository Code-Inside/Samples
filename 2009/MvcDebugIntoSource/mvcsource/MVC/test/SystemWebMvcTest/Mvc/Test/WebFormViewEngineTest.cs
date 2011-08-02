namespace System.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WebFormViewEngineTest {

        [TestMethod]
        public void BuildManagerProperty() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            MockBuildManager buildManagerMock = new MockBuildManager(null, null, null);

            // Act
            engine.BuildManager = buildManagerMock;

            // Assert
            Assert.AreEqual(engine.BuildManager, buildManagerMock);
        }

        [TestMethod]
        public void CreatePartialViewCreatesWebFormView() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Act
            WebFormView result = (WebFormView)engine.CreatePartialView("partial path");

            // Assert
            Assert.AreEqual("partial path", result.ViewPath);
            Assert.AreEqual(String.Empty, result.MasterPath);
        }

        [TestMethod]
        public void CreateViewCreatesWebFormView() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Act
            WebFormView result = (WebFormView)engine.CreateView("view path", "master path");

            // Assert
            Assert.AreEqual("view path", result.ViewPath);
            Assert.AreEqual("master path", result.MasterPath);
        }

        [TestMethod]
        public void FileExistsReturnsTrueForExistingPath() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            object instanceResult = new object();
            MockBuildManager buildManagerMock = new MockBuildManager("some path", typeof(object), instanceResult);
            engine.BuildManager = buildManagerMock;

            // Act
            bool foundResult = engine.PublicFileExists(null, "some path");

            // Assert
            Assert.IsTrue(foundResult);
        }

        [TestMethod]
        public void FileExistsReturnsFalseForNonExistingPath() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            object instanceResult = new object();
            MockBuildManager buildManagerMock = new MockBuildManager("some path", typeof(object), instanceResult);
            engine.BuildManager = buildManagerMock;

            // Act
            bool notFoundResult = engine.PublicFileExists(null, "some other path");

            // Assert
            Assert.IsFalse(notFoundResult);
        }

        [TestMethod]
        public void FileExistsReturnsFalseWhenBuildManagerThrows404() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            object instanceResult = new object();
            MockBuildManager buildManagerMock = new MockBuildManager(new HttpException(404, "HTTP message Not Found"));
            engine.BuildManager = buildManagerMock;

            // Act
            bool notFoundResult = engine.PublicFileExists(null, "some other path");

            // Assert
            Assert.IsFalse(notFoundResult);
        }

        [TestMethod]
        public void FileExistsThrowsWhenBuildManagerThrowsNon404() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            object instanceResult = new object();
            MockBuildManager buildManagerMock = new MockBuildManager(new HttpException(123, "HTTP random message"));
            engine.BuildManager = buildManagerMock;

            // Act & Assert
            ExceptionHelper.ExpectHttpException(
                () => engine.PublicFileExists(null, "some other path"),
                "HTTP random message",
                123);
        }

        [TestMethod]
        public void MasterLocationFormatsProperty() {
            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            Assert.AreEqual(2, engine.MasterLocationFormats.Length);
            Assert.AreEqual("~/Views/{1}/{0}.master", engine.MasterLocationFormats[0]);
            Assert.AreEqual("~/Views/Shared/{0}.master", engine.MasterLocationFormats[1]);
        }

        [TestMethod]
        public void PartialViewLocationFormatsProperty() {
            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            Assert.AreEqual(4, engine.PartialViewLocationFormats.Length);
            Assert.AreEqual("~/Views/{1}/{0}.aspx", engine.PartialViewLocationFormats[0]);
            Assert.AreEqual("~/Views/{1}/{0}.ascx", engine.PartialViewLocationFormats[1]);
            Assert.AreEqual("~/Views/Shared/{0}.aspx", engine.PartialViewLocationFormats[2]);
            Assert.AreEqual("~/Views/Shared/{0}.ascx", engine.PartialViewLocationFormats[3]);
        }

        [TestMethod]
        public void ViewLocationFormatsProperty() {
            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            Assert.AreEqual(4, engine.ViewLocationFormats.Length);
            Assert.AreEqual("~/Views/{1}/{0}.aspx", engine.ViewLocationFormats[0]);
            Assert.AreEqual("~/Views/{1}/{0}.ascx", engine.ViewLocationFormats[1]);
            Assert.AreEqual("~/Views/Shared/{0}.aspx", engine.ViewLocationFormats[2]);
            Assert.AreEqual("~/Views/Shared/{0}.ascx", engine.ViewLocationFormats[3]);
        }

        internal sealed class TestableWebFormViewEngine : WebFormViewEngine {

            public new string[] MasterLocationFormats {
                get { return base.MasterLocationFormats; }
            }

            public new string[] PartialViewLocationFormats {
                get { return base.PartialViewLocationFormats; }
            }

            public new string[] ViewLocationFormats {
                get { return base.ViewLocationFormats; }
            }

            public IView CreatePartialView(string partialPath) {
                return base.CreatePartialView(null, partialPath);
            }

            public IView CreateView(string viewPath, string masterPath) {
                return base.CreateView(null, viewPath, masterPath);
            }

            public bool PublicFileExists(ControllerContext controllerContext, string virtualPath) {
                return base.FileExists(controllerContext, virtualPath);
            }
        }
    }
}
