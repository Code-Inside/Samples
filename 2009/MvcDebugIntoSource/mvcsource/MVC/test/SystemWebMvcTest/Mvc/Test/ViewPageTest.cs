namespace System.Web.Mvc.Test {
    using System.IO;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using System.Web.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ViewPageTest {
        private const string _fakeID = "fakeID";

        [TestMethod]
        public void ModelProperty() {
            // Arrange
            object model = new object();
            ViewDataDictionary viewData = new ViewDataDictionary(model);
            ViewPage viewPage = new ViewPage();
            viewPage.ViewData = viewData;

            // Act
            object viewPageModel = viewPage.Model;

            // Assert
            Assert.AreEqual(model, viewPageModel);
            Assert.AreEqual(model, viewPage.ViewData.Model);
        }

        [TestMethod]
        public void ModelPropertyStronglyTypedViewPage() {
            // Arrange
            FooModel model = new FooModel();
            ViewDataDictionary<FooModel> viewData = new ViewDataDictionary<FooModel>(model);
            ViewPage<FooModel> viewPage = new ViewPage<FooModel>();
            viewPage.ViewData = viewData;

            // Act
            object viewPageModelObject = ((ViewPage)viewPage).Model;
            FooModel viewPageModelPerson = viewPage.Model;

            // Assert
            Assert.AreEqual(model, viewPageModelObject);
            Assert.AreEqual(model, viewPageModelPerson);
        }

        [TestMethod]
        public void SetViewItemOnBaseClassPropagatesToDerivedClass() {
            // Arrange
            ViewPage<object> vpInt = new ViewPage<object>();
            ViewPage vp = vpInt;
            object o = new object();

            // Act
            vp.ViewData.Model = o;

            // Assert
            Assert.AreEqual(o, vpInt.ViewData.Model);
            Assert.AreEqual(o, vp.ViewData.Model);
        }

        [TestMethod]
        public void SetViewItemOnDerivedClassPropagatesToBaseClass() {
            // Arrange
            ViewPage<object> vpInt = new ViewPage<object>();
            ViewPage vp = vpInt;
            object o = new object();

            // Act
            vpInt.ViewData.Model = o;

            // Assert
            Assert.AreEqual(o, vpInt.ViewData.Model);
            Assert.AreEqual(o, vp.ViewData.Model);
        }

        [TestMethod]
        public void SetViewItemToWrongTypeThrows() {
            // Arrange
            ViewPage vp = new ViewPage<string>();

            // Act & Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    vp.ViewData.Model = 50;
                },
                "The model item passed into the dictionary is of type 'System.Int32' but this dictionary requires a model item of type 'System.String'.");
        }

        [TestMethod]
        public void RenderInitsHelpersAndSetsID() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext.Response.Output).Returns(TextWriter.Null);

            ViewPageWithNoProcessRequest viewPage = new ViewPageWithNoProcessRequest();
            viewPage.ID = _fakeID;

            // Act
            viewPage.RenderView(mockViewContext.Object);

            // Assert
            Assert.AreNotEqual(_fakeID, viewPage.ID);
            Assert.IsNotNull(viewPage.Ajax);
            Assert.IsNotNull(viewPage.Html);
            Assert.IsNotNull(viewPage.Url);
        }

        [TestMethod]
        public void GenericPageRenderInitsHelpersAndSetsID() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext.Response.Output).Returns(TextWriter.Null);

            ViewPageWithNoProcessRequest<Controller> viewPage = new ViewPageWithNoProcessRequest<Controller>();
            viewPage.ID = _fakeID;

            // Act
            viewPage.RenderView(mockViewContext.Object);

            // Assert
            Assert.AreNotEqual(_fakeID, viewPage.ID);
            Assert.IsNotNull(viewPage.Ajax);
            Assert.IsNotNull(viewPage.Html);
            Assert.IsNotNull(viewPage.Url);
            Assert.IsNotNull(((ViewPage)viewPage).Html);
            Assert.IsNotNull(((ViewPage)viewPage).Url);
        }

        private static void WriterSetCorrectlyInternal(bool throwException) {

            // Arrange
            bool triggered = false;
            HtmlTextWriter writer = new HtmlTextWriter(System.IO.TextWriter.Null);
            MockViewPage vp = new MockViewPage();
            vp.RenderCallback = delegate() {
                triggered = true;
                Assert.AreEqual(writer, vp.Writer);
                if (throwException) {
                    throw new CallbackException();
                }
            };

            // Act & Assert
            Assert.IsNull(vp.Writer);
            try {
                vp.RenderControl(writer);
            }
            catch (CallbackException) { }
            Assert.IsNull(vp.Writer);
            Assert.IsTrue(triggered);

        }

        [TestMethod]
        public void WriterSetCorrectly() {
            WriterSetCorrectlyInternal(false /* throwException */);
        }

        [TestMethod]
        public void WriterSetCorrectlyThrowException() {
            WriterSetCorrectlyInternal(true /* throwException */);
        }

        private sealed class ViewPageWithNoProcessRequest : ViewPage {
            public override void ProcessRequest(HttpContext context) {
            }
        }

        private sealed class ViewPageWithNoProcessRequest<TModel> : ViewPage<TModel> where TModel : class {
            public override void ProcessRequest(HttpContext context) {
            }
        }

        private sealed class MockViewPage : ViewPage {

            public MockViewPage() {
            }

            public Action RenderCallback { get; set; }

            protected override void RenderChildren(HtmlTextWriter writer) {
                if (RenderCallback != null) {
                    RenderCallback();
                }
                base.RenderChildren(writer);
            }

        }

        private sealed class FooModel {

        }

        private sealed class CallbackException : Exception {

        }
    }
}
