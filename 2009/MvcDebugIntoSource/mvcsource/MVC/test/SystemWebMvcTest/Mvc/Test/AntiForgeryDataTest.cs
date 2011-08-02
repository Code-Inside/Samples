namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AntiForgeryDataTest {

        [TestMethod]
        public void CopyConstructor() {
            // Arrange
            AntiForgeryData originalToken = new AntiForgeryData() {
                CreationDate = DateTime.Now,
                Salt = "some salt",
                Value = "some value"
            };

            // Act
            AntiForgeryData newToken = new AntiForgeryData(originalToken);

            // Assert
            Assert.AreEqual(originalToken.CreationDate, newToken.CreationDate);
            Assert.AreEqual(originalToken.Salt, newToken.Salt);
            Assert.AreEqual(originalToken.Value, newToken.Value);
        }

        [TestMethod]
        public void CopyConstructorThrowsIfTokenIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new AntiForgeryData(null);
                }, "token");
        }

        [TestMethod]
        public void CreationDateProperty() {
            // Arrange
            AntiForgeryData token = new AntiForgeryData();

            // Act & Assert
            MemberHelper.TestPropertyValue(token, "CreationDate", DateTime.Now);
        }

        [TestMethod]
        public void GetAntiForgeryTokenNameReturnsEncodedCookieNameIfAppPathIsNotEmpty() {
            // Arrange    
            // the string below (as UTF-8 bytes) base64-encodes to "Pz4/Pj8+Pz4/Pj8+Pz4/Pg=="
            string original = "?>?>?>?>?>?>?>?>";

            // Act
            string tokenName = AntiForgeryData.GetAntiForgeryTokenName(original);

            // Assert
            Assert.AreEqual("__RequestVerificationToken_Pz4-Pj8.Pz4-Pj8.Pz4-Pg__", tokenName);
        }

        [TestMethod]
        public void GetAntiForgeryTokenNameReturnsFieldNameIfAppPathIsNull() {
            // Act
            string tokenName = AntiForgeryData.GetAntiForgeryTokenName(null);

            // Assert
            Assert.AreEqual("__RequestVerificationToken", tokenName);
        }

        [TestMethod]
        public void NewToken() {
            // Act
            AntiForgeryData token = AntiForgeryData.NewToken();

            // Assert
            int valueLength = Convert.FromBase64String(token.Value).Length;
            Assert.AreEqual(16, valueLength, "Value was not of the correct length.");
            Assert.AreNotEqual(default(DateTime), token.CreationDate, "Creation date should have been initialized.");
        }

        [TestMethod]
        public void SaltProperty() {
            // Arrange
            AntiForgeryData token = new AntiForgeryData();

            // Act & Assert
            MemberHelper.TestStringProperty(token, "Salt", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void ValueProperty() {
            // Arrange
            AntiForgeryData token = new AntiForgeryData();

            // Act & Assert
            MemberHelper.TestStringProperty(token, "Value", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

    }
}
