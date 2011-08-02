namespace Microsoft.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class RenderActionTest {
        [TestMethod]
        public void RenderActionWithActionAndControllerSpecifiedRendersCorrectAction() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary(), "/");
            StringBuilder written = html.ViewContext.HttpContext.SwitchResponseMockOutputToStringBuilder();

            SetupControllerForRenderAction(new TestController());

            html.RenderAction("index", "Test");
            Assert.AreEqual("It Worked!", written.ToString());
        }

        [TestMethod]
        public void RenderActionWithActionControllerAndParametersRendersCorrectAction() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary(), "/");
            StringBuilder written = html.ViewContext.HttpContext.SwitchResponseMockOutputToStringBuilder();

            SetupControllerForRenderAction(new TestController());

            html.RenderAction("About", "Test", new { page=75 });
            Assert.AreEqual("This is page #75", written.ToString());
        }

        [TestMethod]
        public void RenderActionUsingExpressionRendersCorrectly() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary(), "/");
            StringBuilder written = html.ViewContext.HttpContext.SwitchResponseMockOutputToStringBuilder();

            SetupControllerForRenderAction(new TestController());

            html.RenderAction<TestController>(c => c.About(76));
            Assert.AreEqual("This is page #76", written.ToString());
        }

        [TestMethod]
        public void RenderRouteWithActionAndControllerSpecifiedRendersCorrectAction() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary(), "/");
            StringBuilder written = html.ViewContext.HttpContext.SwitchResponseMockOutputToStringBuilder();

            SetupControllerForRenderAction(new TestController());

            html.RenderRoute(new RouteValueDictionary(new {action="Index", controller="Test"}));
            Assert.AreEqual("It Worked!", written.ToString());
        }

        [TestMethod]
        public void RenderActionWithActionOnlySpecifiedAndControllerInRouteDataRendersCorrectAction() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary(), "/");
            html.ViewContext.RouteData.Values.Add("controller", "Test");
            StringBuilder written = html.ViewContext.HttpContext.SwitchResponseMockOutputToStringBuilder();

            SetupControllerForRenderAction(new TestController());

            html.RenderAction("index");
            Assert.AreEqual("It Worked!", written.ToString());
        }

        private static void SetupControllerForRenderAction(Controller controller) {
            var factory = new Mock<IControllerFactory>();
            var tempDataProvider = new Mock<ITempDataProvider>();
            controller.TempDataProvider = tempDataProvider.Object;
            tempDataProvider.Expect(provider => provider.LoadTempData(controller.ControllerContext)).Returns(new Dictionary<string, object>());
            tempDataProvider.Expect(provider => provider.SaveTempData(controller.ControllerContext, It.IsAny<IDictionary<string, object>>()));

            factory.Expect(f => f.CreateController(It.IsAny<RequestContext>(), It.IsAny<string>())).Returns(controller);
            factory.Expect(f => f.ReleaseController(It.IsAny<ControllerBase>()));

            ControllerBuilder.Current.SetControllerFactory(factory.Object);
        }

        public class TestController : Controller {

            protected override void Initialize(RequestContext requestContext) {
                base.Initialize(requestContext);
                TempDataProvider = new EmptyTempDataProvider();
            }

            public string Index() {
                return "It Worked!";
            }

            public string About(int page) {
                return "This is page #" + page;
            }
        }

        internal class EmptyTempDataProvider : ITempDataProvider {
            public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values) {
            }

            public IDictionary<string, object> LoadTempData(ControllerContext controllerContext) {
                return new Dictionary<string, object>();
            }
        }
    }
}
