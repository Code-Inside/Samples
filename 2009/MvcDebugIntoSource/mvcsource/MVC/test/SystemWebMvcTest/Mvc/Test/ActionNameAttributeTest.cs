namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionNameAttributeTest {

        [TestMethod]
        public void ConstructorThrowsIfNameIsEmpty() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ActionNameAttribute(String.Empty);
                }, "name");
        }

        [TestMethod]
        public void ConstructorThrowsIfNameIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ActionNameAttribute(null);
                }, "name");
        }

        [TestMethod]
        public void IsValidForRequestReturnsFalseIfGivenNameDoesNotMatch() {
            // Arrange
            ActionNameAttribute attr = new ActionNameAttribute("Bar");

            // Act
            bool returned = attr.IsValidName(null, "foo", null);

            // Assert
            Assert.IsFalse(returned, "Given name should not have matched.");
        }

        [TestMethod]
        public void IsValidForRequestReturnsTrueIfGivenNameMatches() {
            // Arrange
            ActionNameAttribute attr = new ActionNameAttribute("Bar");

            // Act
            bool returned = attr.IsValidName(null, "bar", null);

            // Assert
            Assert.IsTrue(returned, "Given name should have matched.");
        }

        [TestMethod]
        public void NameProperty() {
            // Arrange
            ActionNameAttribute attr = new ActionNameAttribute("someName");

            // Act & Assert
            Assert.AreEqual("someName", attr.Name);
        }

    }
}
