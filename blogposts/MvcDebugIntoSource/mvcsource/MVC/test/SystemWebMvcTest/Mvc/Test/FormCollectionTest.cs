namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FormCollectionTest {

        [TestMethod]
        public void ConstructorCopiesProvidedCollection() {
            // Arrange
            NameValueCollection nvc = new NameValueCollection() {
                { "foo", "fooValue" },
                { "bar", "barValue" }
            };

            // Act
            FormCollection formCollection = new FormCollection(nvc);

            // Assert
            Assert.AreEqual(2, formCollection.Count);
            Assert.AreEqual("fooValue", formCollection["foo"]);
            Assert.AreEqual("barValue", formCollection["bar"]);
        }

        [TestMethod]
        public void ConstructorThrowsIfCollectionIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new FormCollection(null);
                }, "collection");
        }

        [TestMethod]
        public void CustomBinderBindModelReturnsFormCollection() {
            // Arrange
            NameValueCollection nvc = new NameValueCollection() { { "foo", "fooValue" }, { "bar", "barValue" } };
            IModelBinder binder = ModelBinders.Binders.GetBinder(typeof(FormCollection));

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.Form).Returns(nvc);

            // Act
            FormCollection formCollection = (FormCollection)binder.BindModel(mockControllerContext.Object, null);

            // Assert
            Assert.IsNotNull(formCollection, "BindModel() should have returned a FormCollection.");
            Assert.AreEqual(2, formCollection.Count);
            Assert.AreEqual("fooValue", nvc["foo"]);
            Assert.AreEqual("barValue", nvc["bar"]);
        }

        [TestMethod]
        public void CustomBinderBindModelThrowsIfControllerContextIsNull() {
            // Arrange
            IModelBinder binder = ModelBinders.Binders.GetBinder(typeof(FormCollection));

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void ToValueProviderWrapsCollectionInDictionary() {
            // Arrange
            FormCollection fc = new FormCollection() {
                { "foo", "fooValue0" },
                { "foo", "fooValue1" }
            };

            // Act
            IDictionary<string, ValueProviderResult> valueProvider;
            using (ValueProviderDictionaryTest.ReplaceCurrentCulture("fr-FR")) {
                valueProvider = fc.ToValueProvider();
            }

            // Assert
            Assert.IsNotNull(valueProvider);
            Assert.AreEqual(1, valueProvider.Count);

            ValueProviderResult vpResult = valueProvider["foo"];
            string[] rawValue = (string[])vpResult.RawValue;
            Assert.AreEqual(2, rawValue.Length);
            Assert.AreEqual("fooValue0", rawValue[0]);
            Assert.AreEqual("fooValue1", rawValue[1]);

            Assert.AreEqual("fooValue0,fooValue1", vpResult.AttemptedValue);
            Assert.AreEqual(CultureInfo.GetCultureInfo("fr-FR"), vpResult.Culture);
        }

    }
}
