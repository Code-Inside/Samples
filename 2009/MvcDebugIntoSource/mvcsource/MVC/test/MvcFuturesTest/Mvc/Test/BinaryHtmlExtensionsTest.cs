namespace Microsoft.Web.Mvc.Test {
    using System.Data.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class BinaryHtmlExtensionsTest {
        [TestMethod]
        public void HiddenWithByteArrayValueRendersBase64EncodedValue() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            ViewDataDictionary viewData = new ViewDataDictionary();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper htmlHelper = new HtmlHelper(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.Hidden("ProductName", ByteArrayModelBinderTest.Base64TestBytes);

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"hidden\" value=\"Fys1\" />", result);
        }

        [TestMethod]
        public void HiddenWithBinaryArrayValueRendersBase64EncodedValue() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            ViewDataDictionary viewData = new ViewDataDictionary();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper htmlHelper = new HtmlHelper(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.Hidden("ProductName", new Binary(new byte[] { 23, 43, 53 }));

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"hidden\" value=\"Fys1\" />", result);
        }

        [TestMethod]
        public void HiddenForWithByteArrayValueRendersBase64EncodedValue() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Gallery { Image = ByteArrayModelBinderTest.Base64TestBytes });
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Gallery> htmlHelper = new HtmlHelper<Gallery>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.HiddenFor(g => g.Image);

            // Assert
            Assert.AreEqual("<input id=\"Image\" name=\"Image\" type=\"hidden\" value=\"Fys1\" />", result);
        }

        [TestMethod]
        public void HiddenForWithBinaryArrayValueRendersBase64EncodedValue() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Gallery { TimeStamp = new Binary(ByteArrayModelBinderTest.Base64TestBytes) });
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Gallery> htmlHelper = new HtmlHelper<Gallery>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.HiddenFor(g => g.TimeStamp);

            // Assert
            Assert.AreEqual("<input id=\"TimeStamp\" name=\"TimeStamp\" type=\"hidden\" value=\"Fys1\" />", result);
        }

        private class Gallery {
            public byte[] Image {
                get;
                set;
            }

            public Binary TimeStamp {
                get;
                set;
            }
        }
    }
}
