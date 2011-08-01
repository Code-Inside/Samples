namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelErrorCollectionTest {

        [TestMethod]
        public void AddWithExceptionArgument() {
            // Arrange
            ModelErrorCollection collection = new ModelErrorCollection();
            Exception ex = new Exception("some message");

            // Act
            collection.Add(ex);

            // Assert
            Assert.AreEqual(1, collection.Count);
            ModelError modelError = collection[0];
            Assert.AreSame(ex, modelError.Exception);
        }

        [TestMethod]
        public void AddWithStringArgument() {
            // Arrange
            ModelErrorCollection collection = new ModelErrorCollection();

            // Act
            collection.Add("some message");

            // Assert
            Assert.AreEqual(1, collection.Count);
            ModelError modelError = collection[0];
            Assert.AreEqual("some message", modelError.ErrorMessage);
            Assert.IsNull(modelError.Exception);
        }
    }
}
