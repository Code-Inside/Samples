namespace System.Web.Mvc.Test {
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class WebFormViewTest {

        [TestMethod]
        public void ConstructorWithEmptyViewPathThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => new WebFormView(String.Empty, "~/master"),
                "viewPath"
            );
        }

        [TestMethod]
        public void ConstructorWithNullViewPathThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => new WebFormView(null, "~/master"),
                "viewPath"
            );
        }

        [TestMethod]
        public void MasterPathProperty() {
            // Act
            WebFormView view = new WebFormView("view path", "master path");

            // Assert
            Assert.AreEqual("master path", view.MasterPath);
        }

        [TestMethod]
        public void MasterPathPropertyReturnsEmptyStringIfMasterNotSpecified() {
            // Act
            WebFormView view = new WebFormView("view path", null);

            // Assert
            Assert.AreEqual(String.Empty, view.MasterPath);
        }

        [TestMethod]
        public void RenderWithNullContextThrows() {
            // Arrange
            WebFormView view = new WebFormView("~/view", "~/master");

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => view.Render(null, new Mock<TextWriter>().Object),
                "viewContext"
            );
        }

        [TestMethod]
        public void RenderWithNullViewInstanceThrows() {
            // Arrange
            ViewContext context = new Mock<ViewContext>().Object;
            MockBuildManager buildManager = new MockBuildManager("view path", typeof(object), null);
            WebFormView view = new WebFormView("view path", null);
            view.BuildManager = buildManager;

            // Act & Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                () => view.Render(context, null),
                "The view found at 'view path' could not be created."
            );
        }

        [TestMethod]
        public void RenderWithUnsupportedTypeThrows() {
            // Arrange
            ViewContext context = new Mock<ViewContext>().Object;
            MockBuildManager buildManagerMock = new MockBuildManager("view path", typeof(object), 12345);
            WebFormView view = new WebFormView("view path", null);
            view.BuildManager = buildManagerMock;

            // Act & Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                () => view.Render(context, null),
                "The view at 'view path' must derive from ViewPage, ViewPage<TViewData>, ViewUserControl, or ViewUserControl<TViewData>."
            );
        }

        [TestMethod]
        public void RenderWithViewPageAndMasterRendersView() {
            // Arrange
            ViewContext context = new Mock<ViewContext>().Object;
            StubViewPage viewPage = new StubViewPage();
            MockBuildManager buildManager = new MockBuildManager("view path", typeof(object), viewPage);
            WebFormView view = new WebFormView("view path", "master path");
            view.BuildManager = buildManager;

            // Act
            view.Render(context, null);

            // Assert
            Assert.AreEqual(context, viewPage.ResultViewContext);
            Assert.AreEqual("master path", viewPage.MasterLocation);
        }

        [TestMethod]
        public void RenderWithViewPageRendersView() {
            // Arrange
            ViewContext context = new Mock<ViewContext>().Object;
            StubViewPage viewPage = new StubViewPage();
            MockBuildManager buildManager = new MockBuildManager("view path", typeof(object), viewPage);
            WebFormView view = new WebFormView("view path", null);
            view.BuildManager = buildManager;

            // Act
            view.Render(context, null);

            // Assert
            Assert.AreEqual(context, viewPage.ResultViewContext);
            Assert.AreEqual(String.Empty, viewPage.MasterLocation);
        }

        [TestMethod]
        public void RenderWithViewUserControlAndMasterThrows() {
            // Arrange
            ViewContext context = new Mock<ViewContext>().Object;
            StubViewUserControl viewUserControl = new StubViewUserControl();
            MockBuildManager buildManagerMock = new MockBuildManager("view path", typeof(object), viewUserControl);
            WebFormView view = new WebFormView("view path", "master path");
            view.BuildManager = buildManagerMock;

            // Act & Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                () => view.Render(context, null),
                "A master name cannot be specified when the view is a ViewUserControl."
            );
        }

        [TestMethod]
        public void RenderWithViewUserControlRendersView() {
            // Arrange
            ViewContext context = new Mock<ViewContext>().Object;
            StubViewUserControl viewUserControl = new StubViewUserControl();
            MockBuildManager buildManagerMock = new MockBuildManager("view path", typeof(object), viewUserControl);
            WebFormView view = new WebFormView("view path", null);
            view.BuildManager = buildManagerMock;

            // Act
            view.Render(context, null);

            // Assert
            Assert.AreEqual(context, viewUserControl.ResultViewContext);
        }

        [TestMethod]
        public void ViewPathProperty() {
            // Act
            WebFormView view = new WebFormView("view path", "master path");

            // Assert
            Assert.AreEqual("view path", view.ViewPath);
        }

        public sealed class StubViewPage : ViewPage {
            public ViewContext ResultViewContext;

            public override void RenderView(ViewContext viewContext) {
                ResultViewContext = viewContext;
            }
        }

        public sealed class StubViewUserControl : ViewUserControl {
            public ViewContext ResultViewContext;

            public override void RenderView(ViewContext viewContext) {
                ResultViewContext = viewContext;
            }
        }
    }
}
