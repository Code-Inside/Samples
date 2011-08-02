namespace System.Web.Mvc.Test {
    using System;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ViewResultBaseTest {

        [TestMethod]
        public void ExecuteResultWithNullControllerContextThrows() {
            // Arrange
            ViewResultBaseHelper result = new ViewResultBaseHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => result.ExecuteResult(null),
                "context");
        }

        [TestMethod]
        public void TempDataProperty() {
            // Arrange
            TempDataDictionary newDict = new TempDataDictionary();
            ViewResultBaseHelper result = new ViewResultBaseHelper();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(result, "TempData", newDict);
        }

        [TestMethod]
        public void ViewDataProperty() {
            // Arrange
            ViewDataDictionary newDict = new ViewDataDictionary();
            ViewResultBaseHelper result = new ViewResultBaseHelper();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(result, "ViewData", newDict);
        }

        [TestMethod]
        public void ViewEngineCollectionProperty() {
            // Arrange
            ViewEngineCollection viewEngineCollection = new ViewEngineCollection();
            ViewResultBaseHelper result = new ViewResultBaseHelper();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(result, "ViewEngineCollection", viewEngineCollection);
        }

        [TestMethod]
        public void ViewNameProperty() {
            // Arrange
            ViewResultBaseHelper result = new ViewResultBaseHelper();

            // Act & Assert
            MemberHelper.TestStringProperty(result, "ViewName", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        public class ViewResultBaseHelper : ViewResultBase {
            protected override ViewEngineResult FindView(ControllerContext context) {
                throw new NotImplementedException();
            }
        }
    }
}
