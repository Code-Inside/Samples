namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelErrorTest {

        [TestMethod]
        public void ConstructorThrowsIfExceptionIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ModelError((Exception)null);
                }, "exception");
        }

        [TestMethod]
        public void ConstructorWithExceptionAndStringArguments() {
            // Arrange
            Exception ex = new Exception("some message");

            // Act
            ModelError modelError = new ModelError(ex, "some other message");

            // Assert
            Assert.AreEqual("some other message", modelError.ErrorMessage);
            Assert.AreSame(ex, modelError.Exception);
        }

        [TestMethod]
        public void ConstructorWithExceptionArgument() {
            // Arrange
            Exception ex = new Exception("some message");

            // Act
            ModelError modelError = new ModelError(ex);

            // Assert
            Assert.AreEqual(String.Empty, modelError.ErrorMessage);
            Assert.AreSame(ex, modelError.Exception);
        }

        [TestMethod]
        public void ConstructorWithNullStringArgumentCreatesEmptyStringErrorMessage() {
            // Act
            ModelError modelError = new ModelError((string)null);

            // Assert
            Assert.AreEqual(String.Empty, modelError.ErrorMessage);
        }

        [TestMethod]
        public void ConstructorWithStringArgument() {
            // Act
            ModelError modelError = new ModelError("some message");

            // Assert
            Assert.AreEqual("some message", modelError.ErrorMessage);
            Assert.IsNull(modelError.Exception);
        }

    }
}