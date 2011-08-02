namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class ContentTypeAttributeTest {
        [TestMethod]
        public void ContentTypeSetInCtor() {
            var attr = new ContentTypeAttribute("text/html");
            Assert.AreEqual("text/html", attr.ContentType);
        }

        [TestMethod]
        public void ContentTypeCtorThrowsArgumentExceptionWhenContentTypeIsNull() {
            ExceptionHelper.ExpectArgumentException(() => new ContentTypeAttribute(null), "Value cannot be null or empty." + Environment.NewLine + "Parameter name: contentType");
        }

        [TestMethod]
        public void ExecuteResultSetsContentType() {
            var mockHttpResponse = new Mock<HttpResponseBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Response).Returns(mockHttpResponse.Object);

            mockHttpResponse.ExpectSetProperty(r => r.ContentType, "text/xml");

            var mockController = new Mock<Controller>();
            var controllerContext = new ControllerContext(new RequestContext(mockHttpContext.Object, new RouteData()), mockController.Object);
            var result = new ContentResult { Content = "blah blah" };
            var filterContext = new ResultExecutingContext(controllerContext, result);

            var filter = new ContentTypeAttribute("text/xml");
            filter.OnResultExecuting(filterContext);
        }
    }
}
