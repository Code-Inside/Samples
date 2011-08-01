namespace System.Web.Mvc.Test {
    using System.Web.Routing;
    using System.Web.TestUtil;
    using System.Web.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Stub;

    [TestClass]
    public class ViewUserControlTest {

        [TestMethod]
        public void ModelProperty() {
            // Arrange
            object model = new object();
            ViewDataDictionary viewData = new ViewDataDictionary(model);
            ViewUserControl viewUserControl = new ViewUserControl();
            viewUserControl.ViewData = viewData;

            // Act
            object viewPageModel = viewUserControl.Model;

            // Assert
            Assert.AreEqual(model, viewPageModel);
            Assert.AreEqual(model, viewUserControl.ViewData.Model);
        }

        [TestMethod]
        public void ModelPropertyStronglyTyped() {
            // Arrange
            FooModel model = new FooModel();
            ViewDataDictionary<FooModel> viewData = new ViewDataDictionary<FooModel>(model);
            ViewUserControl<FooModel> viewUserControl = new ViewUserControl<FooModel>();
            viewUserControl.ViewData = viewData;

            // Act
            object viewPageModelObject = ((ViewUserControl)viewUserControl).Model;
            FooModel viewPageModelPerson = viewUserControl.Model;

            // Assert
            Assert.AreEqual(model, viewPageModelObject);
            Assert.AreEqual(model, viewPageModelPerson);
        }

        [TestMethod]
        public void RenderViewAndRestoreContentType() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Stub(c => c.HttpContext.Response.ContentType);
            ViewContext vc = mockViewContext.Object;

            Mock<ViewPage> mockViewPage = new Mock<ViewPage>();
            mockViewPage.Expect(vp => vp.RenderView(vc)).Callback(() => vc.HttpContext.Response.ContentType = "newContentType");

            // Act
            vc.HttpContext.Response.ContentType = "oldContentType";
            ViewUserControl.RenderViewAndRestoreContentType(mockViewPage.Object, vc);
            string postContentType = vc.HttpContext.Response.ContentType;

            // Assert
            Assert.AreEqual("oldContentType", postContentType);
        }

        [TestMethod]
        public void SetViewItem() {
            // Arrange
            ViewUserControl vuc = new ViewUserControl();
            object viewItem = new object();
            vuc.ViewData = new ViewDataDictionary(viewItem);

            // Act
            vuc.ViewData.Model = viewItem;
            object newViewItem = vuc.ViewData.Model;

            // Assert
            Assert.AreSame(viewItem, newViewItem);
        }

        [TestMethod]
        public void SetViewItemOnBaseClassPropagatesToDerivedClass() {
            // Arrange
            ViewUserControl<object> vucInt = new ViewUserControl<object>();
            ViewUserControl vuc = vucInt;
            vuc.ViewData = new ViewDataDictionary();
            object o = new object();

            // Act
            vuc.ViewData.Model = o;

            // Assert
            Assert.AreEqual(o, vucInt.ViewData.Model);
            Assert.AreEqual(o, vuc.ViewData.Model);
        }

        [TestMethod]
        public void SetViewItemOnDerivedClassPropagatesToBaseClass() {
            // Arrange
            ViewUserControl<object> vucInt = new ViewUserControl<object>();
            ViewUserControl vuc = vucInt;
            vucInt.ViewData = new ViewDataDictionary<object>();
            object o = new object();

            // Act
            vucInt.ViewData.Model = o;

            // Assert
            Assert.AreEqual(o, vucInt.ViewData.Model);
            Assert.AreEqual(o, vuc.ViewData.Model);
        }

        [TestMethod]
        public void SetViewItemToWrongTypeThrows() {
            // Arrange
            ViewUserControl<string> vucString = new ViewUserControl<string>();
            vucString.ViewData = new ViewDataDictionary<string>();
            ViewUserControl vuc = vucString;

            // Act & Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    vuc.ViewData.Model = 50;
                },
                "The model item passed into the dictionary is of type 'System.Int32' but this dictionary requires a model item of type 'System.String'.");
        }

        [TestMethod]
        public void GetViewDataWhenNoPageSetThrows() {
            ViewUserControl vuc = new ViewUserControl();
            vuc.AppRelativeVirtualPath = "~/Foo.ascx";

            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    var foo = vuc.ViewData["Foo"];
                },
                "The ViewUserControl '~/Foo.ascx' cannot find an IViewDataContainer. The ViewUserControl must be inside a ViewPage, ViewMasterPage, or another ViewUserControl.");
        }

        [TestMethod]
        public void GetViewDataWhenRegularPageSetThrows() {
            Page p = new Page();
            p.Controls.Add(new Control());
            ViewUserControl vuc = new ViewUserControl();
            p.Controls[0].Controls.Add(vuc);
            vuc.AppRelativeVirtualPath = "~/Foo.ascx";

            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    var foo = vuc.ViewData["Foo"];
                },
                "The ViewUserControl '~/Foo.ascx' cannot find an IViewDataContainer. The ViewUserControl must be inside a ViewPage, ViewMasterPage, or another ViewUserControl.");
        }

        [TestMethod]
        public void GetViewDataFromViewPage() {
            // Arrange
            ViewPage p = new ViewPage();
            p.Controls.Add(new Control());
            ViewUserControl vuc = new ViewUserControl();
            p.Controls[0].Controls.Add(vuc);
            p.ViewData = new ViewDataDictionary { { "FirstName", "Joe" }, { "LastName", "Schmoe" } };

            // Act
            object firstName = vuc.ViewData.Eval("FirstName");
            object lastName = vuc.ViewData.Eval("LastName");

            // Assert
            Assert.AreEqual("Joe", firstName);
            Assert.AreEqual("Schmoe", lastName);
        }

        [TestMethod]
        public void GetViewDataFromViewPageWithViewDataKeyPointingToObject() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary() {
                { "Foo", "FooParent" },
                { "Bar", "BarParent" },
                { "Child", new object() }
            };

            ViewPage p = new ViewPage();
            p.Controls.Add(new Control());
            ViewUserControl vuc = new ViewUserControl() { ViewDataKey = "Child" };
            p.Controls[0].Controls.Add(vuc);
            p.ViewData = vdd;

            // Act
            object oFoo = vuc.ViewData.Eval("Foo");
            object oBar = vuc.ViewData.Eval("Bar");

            // Assert
            Assert.AreEqual(vdd["Child"], vuc.ViewData.Model);
            Assert.AreEqual("FooParent", oFoo);
            Assert.AreEqual("BarParent", oBar);
        }

        [TestMethod]
        public void GetViewDataFromViewPageWithViewDataKeyPointingToViewDataDictionary() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary() {
                { "Foo", "FooParent" },
                { "Bar", "BarParent" },
                { "Child",
                    new ViewDataDictionary() {
                        { "Foo", "FooChild" },
                        { "Bar", "BarChild" }
                    }
                }
            };

            ViewPage p = new ViewPage();
            p.Controls.Add(new Control());
            ViewUserControl vuc = new ViewUserControl() { ViewDataKey = "Child" };
            p.Controls[0].Controls.Add(vuc);
            p.ViewData = vdd;

            // Act
            object oFoo = vuc.ViewData.Eval("Foo");
            object oBar = vuc.ViewData.Eval("Bar");

            // Assert
            Assert.AreEqual(vdd["Child"], vuc.ViewData);
            Assert.AreEqual("FooChild", oFoo);
            Assert.AreEqual("BarChild", oBar);
        }

        [TestMethod]
        public void GetViewDataFromViewUserControl() {
            // Arrange
            ViewPage p = new ViewPage();
            p.Controls.Add(new Control());
            ViewUserControl outerVuc = new ViewUserControl();
            p.Controls[0].Controls.Add(outerVuc);
            outerVuc.Controls.Add(new Control());
            ViewUserControl vuc = new ViewUserControl();
            outerVuc.Controls[0].Controls.Add(vuc);

            p.ViewData = new ViewDataDictionary { { "FirstName", "Joe" }, { "LastName", "Schmoe" } };

            // Act
            object firstName = vuc.ViewData.Eval("FirstName");
            object lastName = vuc.ViewData.Eval("LastName");

            // Assert
            Assert.AreEqual("Joe", firstName);
            Assert.AreEqual("Schmoe", lastName);
        }

        [TestMethod]
        public void GetViewDataFromViewUserControlWithViewDataKeyOnInnerControl() {
            // Arrange
            ViewPage p = new ViewPage();
            p.Controls.Add(new Control());
            ViewUserControl outerVuc = new ViewUserControl();
            p.Controls[0].Controls.Add(outerVuc);
            outerVuc.Controls.Add(new Control());
            ViewUserControl vuc = new ViewUserControl() { ViewDataKey = "SubData" };
            outerVuc.Controls[0].Controls.Add(vuc);

            p.ViewData = new ViewDataDictionary { { "FirstName", "Joe" }, { "LastName", "Schmoe" } };
            p.ViewData["SubData"] = new ViewDataDictionary { { "FirstName", "SubJoe" }, { "LastName", "SubSchmoe" } };

            // Act
            object firstName = vuc.ViewData.Eval("FirstName");
            object lastName = vuc.ViewData.Eval("LastName");

            // Assert
            Assert.AreEqual("SubJoe", firstName);
            Assert.AreEqual("SubSchmoe", lastName);
        }

        [TestMethod]
        public void GetViewDataFromViewUserControlWithViewDataKeyOnOuterControl() {
            // Arrange
            ViewPage p = new ViewPage();
            p.Controls.Add(new Control());
            ViewUserControl outerVuc = new ViewUserControl() { ViewDataKey = "SubData" };
            p.Controls[0].Controls.Add(outerVuc);
            outerVuc.Controls.Add(new Control());
            ViewUserControl vuc = new ViewUserControl();
            outerVuc.Controls[0].Controls.Add(vuc);

            p.ViewData = new ViewDataDictionary { { "FirstName", "Joe" }, { "LastName", "Schmoe" } };
            p.ViewData["SubData"] = new ViewDataDictionary { { "FirstName", "SubJoe" }, { "LastName", "SubSchmoe" } };

            // Act
            object firstName = vuc.ViewData.Eval("FirstName");
            object lastName = vuc.ViewData.Eval("LastName");

            // Assert
            Assert.AreEqual("SubJoe", firstName);
            Assert.AreEqual("SubSchmoe", lastName);
        }

        [TestMethod]
        public void ViewDataKeyProperty() {
            MemberHelper.TestStringProperty(new ViewUserControl(), "ViewDataKey", String.Empty, true);
        }

        [TestMethod]
        public void GetWrongGenericViewItemTypeThrows() {
            // Arrange
            ViewPage p = new ViewPage();
            p.ViewData = new ViewDataDictionary();
            p.ViewData["Foo"] = new DummyViewData { MyInt = 123, MyString = "Whatever" };

            MockViewUserControl<MyViewData> vuc = new MockViewUserControl<MyViewData>() { ViewDataKey = "FOO" };
            vuc.AppRelativeVirtualPath = "~/Foo.aspx";
            p.Controls.Add(new Control());
            p.Controls[0].Controls.Add(vuc);

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    var foo = vuc.ViewData.Model.IntProp;
                },
                @"The model item passed into the dictionary is of type 'System.Web.Mvc.Test.ViewUserControlTest+DummyViewData' but this dictionary requires a model item of type 'System.Web.Mvc.Test.ViewUserControlTest+MyViewData'.");
        }

        [TestMethod]
        public void GetGenericViewItemType() {
            // Arrange
            ViewPage p = new ViewPage();
            p.Controls.Add(new Control());
            MockViewUserControl<MyViewData> vuc = new MockViewUserControl<MyViewData>() { ViewDataKey = "FOO" };
            p.Controls[0].Controls.Add(vuc);
            p.ViewData = new ViewDataDictionary();
            p.ViewData["Foo"] = new MyViewData { IntProp = 123, StringProp = "miao" };

            // Act
            int intProp = vuc.ViewData.Model.IntProp;
            string stringProp = vuc.ViewData.Model.StringProp;

            // Assert
            Assert.AreEqual<int>(123, intProp);
            Assert.AreEqual<string>("miao", stringProp);
        }

        [TestMethod]
        public void GetHtmlHelperFromViewPage() {
            // Arrange
            ViewUserControl vuc = new ViewUserControl();
            ViewPage containerPage = new ViewPage();
            containerPage.Controls.Add(vuc);
            ViewContext vc = new Mock<ViewContext>().Object;
            vuc.ViewContext = vc;

            // Act
            HtmlHelper htmlHelper = vuc.Html;

            // Assert
            Assert.AreEqual(vc, htmlHelper.ViewContext);
            Assert.AreEqual(vuc, htmlHelper.ViewDataContainer);
        }

        [TestMethod]
        public void GetHtmlHelperFromRegularPage() {
            // Arrange
            ViewUserControl vuc = new ViewUserControl();
            Page containerPage = new Page();
            containerPage.Controls.Add(vuc);

            // Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                 delegate {
                     HtmlHelper foo = vuc.Html;
                 },
                 "A ViewUserControl can only be used inside pages that derive from ViewPage or ViewPage<TViewItem>.");
        }

        [TestMethod]
        public void GetUrlHelperFromViewPage() {
            // Arrange
            ViewUserControl vuc = new ViewUserControl();
            ViewPage containerPage = new ViewPage();
            containerPage.Controls.Add(vuc);
            RequestContext rc = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            UrlHelper urlHelper = new UrlHelper(rc);
            containerPage.Url = urlHelper;

            // Assert
            Assert.AreEqual(vuc.Url, urlHelper);
        }

        [TestMethod]
        public void GetUrlHelperFromRegularPage() {
            // Arrange
            ViewUserControl vuc = new ViewUserControl();
            Page containerPage = new Page();
            containerPage.Controls.Add(vuc);

            // Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                 delegate {
                     UrlHelper foo = vuc.Url;
                 },
                 "A ViewUserControl can only be used inside pages that derive from ViewPage or ViewPage<TViewItem>.");
        }

        [TestMethod]
        public void GetWriterFromViewPage() {
            // Arrange
            MockViewUserControl vuc = new MockViewUserControl();
            MockViewUserControlContainerPage containerPage = new MockViewUserControlContainerPage(vuc);
            bool triggered = false;
            HtmlTextWriter writer = new HtmlTextWriter(System.IO.TextWriter.Null);
            containerPage.RenderCallback = delegate() {
                triggered = true;
                Assert.AreEqual(writer, vuc.Writer);
            };

            // Act & Assert
            Assert.IsNull(vuc.Writer);
            containerPage.RenderControl(writer);
            Assert.IsNull(vuc.Writer);
            Assert.IsTrue(triggered);
        }

        [TestMethod]
        public void GetWriterFromRegularPageThrows() {
            // Arrange
            MockViewUserControl vuc = new MockViewUserControl();
            Page containerPage = new Page();
            containerPage.Controls.Add(vuc);

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                 delegate {
                     HtmlTextWriter writer = vuc.Writer;
                 },
                 "A ViewUserControl can only be used inside pages that derive from ViewPage or ViewPage<TViewItem>.");
        }

        private sealed class DummyViewData {
            public int MyInt { get; set; }
            public string MyString { get; set; }
        }

        private sealed class MockViewUserControlContainerPage : ViewPage {

            public Action RenderCallback { get; set; }

            public MockViewUserControlContainerPage(ViewUserControl userControl) {
                Controls.Add(userControl);
            }

            protected override void RenderChildren(HtmlTextWriter writer) {
                if (RenderCallback != null) {
                    RenderCallback();
                }
                base.RenderChildren(writer);
            }

        }

        private sealed class MockViewUserControl : ViewUserControl {
        }

        private sealed class MockViewUserControl<TViewData> : ViewUserControl<TViewData> where TViewData : class {
        }

        private sealed class MyViewData {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
        }

        private sealed class FooModel {

        }
    }
}
