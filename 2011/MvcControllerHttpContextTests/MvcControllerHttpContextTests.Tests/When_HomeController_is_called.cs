using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Moq;
using MvcControllerHttpContextTests.Controllers;
using NUnit.Framework;

namespace MvcControllerHttpContextTests.Tests
{
    [TestFixture]
    public class When_HomeController_Index_is_called
    {
        protected HomeController Sut;
        protected Mock<HttpContextBase> _httpContextBaseMock;
        protected Mock<HttpRequestBase> _httpRequestBaseMock;
        protected Mock<HttpResponseBase> _httpRepsonseBaseMock;
        protected HttpCookieCollection _cookieCollection;
        protected string _expectedUserAgentInViewBagMessage;
        protected HttpCookie _expectedCookie;

        protected ViewResult _result;

        [TestFixtureSetUp]
        public void Arrange()
        {
            _expectedUserAgentInViewBagMessage = "HelloTDDInMVC";
            _expectedCookie = new HttpCookie("HelloTDDInCookie");

            // Request expectations
            this._httpRequestBaseMock = new Mock<HttpRequestBase>();
            this._httpRequestBaseMock.Setup(x => x.UserAgent).Returns(this._expectedUserAgentInViewBagMessage);

            // Response expectations
            _cookieCollection = new HttpCookieCollection();
            this._httpRepsonseBaseMock = new Mock<HttpResponseBase>();
            this._httpRepsonseBaseMock.Setup(x => x.Cookies).Returns(_cookieCollection);


            // HttpContext expectations
            this._httpContextBaseMock = new Mock<HttpContextBase>();
            this._httpContextBaseMock.Setup(x => x.Request).Returns(this._httpRequestBaseMock.Object);
            this._httpContextBaseMock.Setup(x => x.Response).Returns(this._httpRepsonseBaseMock.Object);

            // Arrang Sut
            this.Sut = new HomeController();
            this.Sut.ControllerContext = new ControllerContext();
            this.Sut.ControllerContext.HttpContext = this._httpContextBaseMock.Object;
        }

        public void Act()
        {
            this._result = (ViewResult)this.Sut.Index();
        }

        [Test]
        public void Then_the_Request_UserAgent_should_be_read()
        {
            Act();
            Assert.AreEqual(this._expectedUserAgentInViewBagMessage, this._result.ViewBag.Message);
        }

        [Test]
        public void Then_a_Cookie_should_be_added_to_the_Response()
        {
            Act();
            Assert.AreSame(this._expectedCookie.Value, this._cookieCollection[this._expectedCookie.Name].Value);
        }
    }
}
