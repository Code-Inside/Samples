namespace System.Web.Mvc.Html.Test {
    using System.Web;
    using System.Web.Mvc.Html;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MvcFormTest {
        [TestMethod]
        public void ConstructorWithNullHttpResponseThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new MvcForm(null);
                },
                "httpResponse");
        }

        [TestMethod]
        public void DisposeRendersCloseFormTag() {
            // Arrange
            Mock<HttpResponseBase> mockHttpResponse = GetHttpResponseForForm();
            MvcForm form = new MvcForm(mockHttpResponse.Object);

            // Act
            form.Dispose();

            // Assert
            mockHttpResponse.Verify();
        }

        [TestMethod]
        public void EndFormRendersCloseFormTag() {
            // Arrange
            Mock<HttpResponseBase> mockHttpResponse = GetHttpResponseForForm();
            MvcForm form = new MvcForm(mockHttpResponse.Object);

            // Act
            form.EndForm();

            // Assert
            mockHttpResponse.Verify();
        }

        [TestMethod]
        public void DisposeTwiceRendersCloseFormTagOnce() {
            // Arrange
            Mock<HttpResponseBase> mockHttpResponse = GetHttpResponseForForm();
            MvcForm form = new MvcForm(mockHttpResponse.Object);

            // Act
            form.Dispose();
            form.Dispose();

            // Assert
            mockHttpResponse.Verify();
        }

        [TestMethod]
        public void EndFormTwiceRendersCloseFormTagOnce() {
            // Arrange
            Mock<HttpResponseBase> mockHttpResponse = GetHttpResponseForForm();
            MvcForm form = new MvcForm(mockHttpResponse.Object);

            // Act
            form.EndForm();
            form.EndForm();

            // Assert
            mockHttpResponse.Verify();
        }

        private static Mock<HttpResponseBase> GetHttpResponseForForm() {
            Mock<HttpResponseBase> mockHttpResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            mockHttpResponse.Expect(r => r.Write("</form>")).AtMostOnce().Verifiable();
            return mockHttpResponse;
        }
    }
}
