namespace System.Web.Mvc.Test {
    using System.Web.Routing;
    using System.Web.TestUtil;
    using System.Web.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ViewMasterPageTest {

        [TestMethod]
        public void GetModelFromViewPage() {
            // Arrange
            ViewMasterPage vmp = new ViewMasterPage();
            ViewPage vp = new ViewPage();
            vmp.Page = vp;
            object model = new object();
            vp.ViewData = new ViewDataDictionary(model);

            // Assert
            Assert.AreEqual(model, vmp.Model);
        }

        [TestMethod]
        public void GetModelFromViewPageStronglyTyped() {
            // Arrange
            ViewMasterPage<FooModel> vmp = new ViewMasterPage<FooModel>();
            ViewPage vp = new ViewPage();
            vmp.Page = vp;
            FooModel model = new FooModel();
            vp.ViewData = new ViewDataDictionary(model);

            // Assert
            Assert.AreEqual(model, vmp.Model);
        }

        [TestMethod]
        public void GetViewDataFromViewPage() {
            // Arrange
            ViewMasterPage vmp = new ViewMasterPage();
            ViewPage vp = new ViewPage();
            vmp.Page = vp;
            vp.ViewData = new ViewDataDictionary { { "a", "123" }, { "b", "456" } };

            // Assert
            Assert.AreEqual("123", vmp.ViewData.Eval("a"));
            Assert.AreEqual("456", vmp.ViewData.Eval("b"));
        }

        [TestMethod]
        public void GetViewItemFromViewPageTViewData() {
            // Arrange
            MockViewMasterPageDummyViewData vmp = new MockViewMasterPageDummyViewData();
            MockViewPageDummyViewData vp = new MockViewPageDummyViewData();
            vmp.Page = vp;
            vp.ViewData.Model = new DummyViewData { MyInt = 123, MyString = "abc" };

            // Assert
            Assert.AreEqual(123, vmp.ViewData.Model.MyInt);
            Assert.AreEqual("abc", vmp.ViewData.Model.MyString);
        }

        [TestMethod]
        public void GetWriterFromViewPage() {
            // Arrange
            bool triggered = false;
            HtmlTextWriter writer = new HtmlTextWriter(System.IO.TextWriter.Null);
            ViewMasterPage vmp = new ViewMasterPage();
            MockViewPage vp = new MockViewPage();
            vp.RenderCallback = delegate() {
                triggered = true;
                Assert.AreEqual(writer, vmp.Writer);
            };
            vmp.Page = vp;

            // Act & Assert
            Assert.IsNull(vmp.Writer);
            vp.RenderControl(writer);
            Assert.IsNull(vmp.Writer);
            Assert.IsTrue(triggered);
        }

        [TestMethod]
        public void GetViewDataFromPageThrows() {
            // Arrange
            ViewMasterPage vmp = new ViewMasterPage();
            vmp.Page = new Page();

            // Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    object foo = vmp.ViewData;
                },
                "A ViewMasterPage can only be used with content pages that derive from ViewPage or ViewPage<TViewItem>.");
        }

        [TestMethod]
        public void GetViewItemFromWrongGenericViewPageType() {
            // Arrange
            MockViewMasterPageDummyViewData vmp = new MockViewMasterPageDummyViewData();
            MockViewPageBogusViewData vp = new MockViewPageBogusViewData();
            vmp.Page = vp;
            vp.ViewData.Model = new SelectListItem();

            // Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    object foo = vmp.ViewData.Model;
                },
                "The model item passed into the dictionary is of type 'System.Web.Mvc.SelectListItem' but this dictionary requires a model item of type 'System.Web.Mvc.Test.ViewMasterPageTest+DummyViewData'.");
        }

        [TestMethod]
        public void GetViewDataFromNullPageThrows() {
            // Arrange
            MockViewMasterPageDummyViewData vmp = new MockViewMasterPageDummyViewData();

            // Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    object foo = vmp.ViewData;
                },
                "A ViewMasterPage can only be used with content pages that derive from ViewPage or ViewPage<TViewItem>.");
        }

        [TestMethod]
        public void GetViewDataFromRegularPageThrows() {
            // Arrange
            MockViewMasterPageDummyViewData vmp = new MockViewMasterPageDummyViewData();
            vmp.Page = new Page();

            // Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    object foo = vmp.ViewData;
                },
                "A ViewMasterPage can only be used with content pages that derive from ViewPage or ViewPage<TViewItem>.");
        }

        [TestMethod]
        public void GetHtmlHelperFromViewPage() {
            // Arrange
            ViewMasterPage vmp = new ViewMasterPage();
            ViewPage vp = new ViewPage();
            vmp.Page = vp;
            ViewContext vc = new Mock<ViewContext>().Object;

            HtmlHelper htmlHelper = new HtmlHelper(vc, vp);
            vp.Html = htmlHelper;

            // Assert
            Assert.AreEqual(vmp.Html, htmlHelper);
        }

        [TestMethod]
        public void GetUrlHelperFromViewPage() {
            // Arrange
            ViewMasterPage vmp = new ViewMasterPage();
            ViewPage vp = new ViewPage();
            vmp.Page = vp;
            RequestContext rc = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            UrlHelper urlHelper = new UrlHelper(rc);
            vp.Url = urlHelper;

            // Assert
            Assert.AreEqual(vmp.Url, urlHelper);
        }

        // Master page types
        private sealed class MockViewMasterPageDummyViewData : ViewMasterPage<DummyViewData> {
        }

        // View data types
        private sealed class DummyViewData {
            public int MyInt { get; set; }
            public string MyString { get; set; }
        }

        // Page types
        private sealed class MockViewPageBogusViewData : ViewPage<SelectListItem> {
        }

        private sealed class MockViewPageDummyViewData : ViewPage<DummyViewData> {
        }

        private sealed class MockViewPage : ViewPage {

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
    }
}
