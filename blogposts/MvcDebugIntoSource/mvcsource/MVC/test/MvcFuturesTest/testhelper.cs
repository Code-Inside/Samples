namespace Microsoft.Web.Mvc.Test {
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Test;
    using System.Web.Routing;
    using Moq;

    public static class TestHelper {
        internal static HtmlHelper GetHtmlHelper(ViewDataDictionary viewData, string appPath) {
            ViewContext viewContext = GetViewContext(appPath);
            Mock<IViewDataContainer> mockContainer = new Mock<IViewDataContainer>();
            mockContainer.Expect(c => c.ViewData).Returns(viewData);
            IViewDataContainer container = mockContainer.Object;
            return new HtmlHelper(viewContext, container);
        }

        internal static HtmlHelper GetHtmlHelper(ViewDataDictionary viewData) {
            return GetHtmlHelper(viewData, "/");
        }

        internal static ViewContext GetViewContext(string appPath) {
            HttpContextBase httpContext = HtmlHelperTest.GetHttpContext(appPath, "/request", "GET");

            Mock<ViewContext> mockViewContext = new Mock<ViewContext>() { DefaultValue = DefaultValue.Mock };
            mockViewContext.Expect(c => c.HttpContext).Returns(httpContext);
            return mockViewContext.Object;
        }

        internal static ViewContext GetViewContext() {
            return GetViewContext("/");
        }
    }
}
