namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class MvcAsyncRouteHandlerTest {

        [TestMethod]
        public void GetHttpHandler() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            IRouteHandler routeHandler = new MvcAsyncRouteHandler();

            // Act
            IHttpHandler httpHandler = routeHandler.GetHttpHandler(requestContext);

            // Assert
            Assert.IsInstanceOfType(httpHandler, typeof(MvcAsyncHandler));
            MvcAsyncHandler castHttpHandler = (MvcAsyncHandler)httpHandler;
            Assert.AreEqual(requestContext, castHttpHandler.RequestContext);
        }

    }
}
