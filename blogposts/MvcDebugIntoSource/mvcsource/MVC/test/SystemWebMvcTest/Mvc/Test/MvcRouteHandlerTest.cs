namespace System.Web.Mvc.Test {
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MvcRouteHandlerTest {
        [TestMethod]
        public void GetHttpHandlerReturnsMvcHandlerWithRouteData() {
            // Arrange
            RequestContext context =new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            IRouteHandler rh = new MvcRouteHandler();

            // Act
            IHttpHandler httpHandler = rh.GetHttpHandler(context);

            // Assert
            MvcHandler h = httpHandler as MvcHandler;
            Assert.IsNotNull(h, "The handler should be a valid MvcHandler instance");
            Assert.AreEqual<RequestContext>(context, h.RequestContext);
        }
    }
}
