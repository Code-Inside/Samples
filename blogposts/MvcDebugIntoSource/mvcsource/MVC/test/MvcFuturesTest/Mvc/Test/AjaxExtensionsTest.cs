namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class AjaxExtensionsTest {

        [TestMethod]
        public void JavaScriptStringEncodeReturnsEmptyStringIfMessageIsEmpty() {
            // Act
            string encoded = AjaxExtensions.JavaScriptStringEncode(null /* helper */, String.Empty);

            // Assert
            Assert.AreEqual(String.Empty, encoded);
        }

        [TestMethod]
        public void JavaScriptStringEncodeReturnsEncodedMessage() {
            // Arrange
            string message = "I said, \"Hello, world!\"\nHow are you?";

            // Act
            string encoded = AjaxExtensions.JavaScriptStringEncode(null /* helper */, message);

            // Assert
            Assert.AreEqual(@"I said, \""Hello, world!\""\nHow are you?", encoded);
        }

        [TestMethod]
        public void JavaScriptStringEncodeReturnsNullIfMessageIsNull() {
            // Act
            string encoded = AjaxExtensions.JavaScriptStringEncode(null /* helper */, null /* message */);

            // Assert
            Assert.IsNull(encoded);
        }

    }
}
