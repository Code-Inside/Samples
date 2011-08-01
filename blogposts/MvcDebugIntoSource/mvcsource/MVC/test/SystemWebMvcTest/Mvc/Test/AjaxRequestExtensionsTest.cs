namespace System.Web.Mvc.Test {
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web.Mvc.Test;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AjaxRequestExtensionsTest {

        [TestMethod]
        public void IsAjaxRequestWithNullRequestThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AjaxRequestExtensions.IsAjaxRequest(null);
                }, "request");
        }

        [TestMethod]
        public void IsAjaxRequestWithKeyIsTrue() {
            // Arrange
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Expect(r => r["X-Requested-With"]).Returns("XMLHttpRequest").Verifiable();
            HttpRequestBase request = mockRequest.Object;

            // Act
            bool retVal = AjaxRequestExtensions.IsAjaxRequest(request);

            // Assert
            Assert.IsTrue(retVal);
            mockRequest.Verify();
        }

        [TestMethod]
        public void IsAjaxRequestWithoutKeyOrHeaderIsFalse() {
            // Arrange
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            NameValueCollection headerCollection = new NameValueCollection();
            mockRequest.Expect(r => r.Headers).Returns(headerCollection).Verifiable();
            mockRequest.Expect(r => r["X-Requested-With"]).Returns((string)null).Verifiable();
            HttpRequestBase request = mockRequest.Object;

            // Act
            bool retVal = AjaxRequestExtensions.IsAjaxRequest(request);

            // Assert
            Assert.IsFalse(retVal);
            mockRequest.Verify();
        }

        [TestMethod]
        public void IsAjaxRequestReturnsTrueIfHeaderSet() {
            // Arrange
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            NameValueCollection headerCollection = new NameValueCollection();
            headerCollection["X-Requested-With"] = "XMLHttpRequest";
            mockRequest.Expect(r => r.Headers).Returns(headerCollection).Verifiable();
            HttpRequestBase request = mockRequest.Object;

            // Act
            bool retVal = AjaxRequestExtensions.IsAjaxRequest(request);

            // Assert
            Assert.IsTrue(retVal);
            mockRequest.Verify();
        }
    }
}
