namespace System.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelStateDictionaryTest {

        [TestMethod]
        public void AddModelErrorCreatesModelStateIfNotPresent() {
            // Arrange
            ModelStateDictionary dictionary = new ModelStateDictionary();

            // Act
            dictionary.AddModelError("some key", "some error");

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            ModelState modelState = dictionary["some key"];

            Assert.AreEqual(1, modelState.Errors.Count);
            Assert.AreEqual("some error", modelState.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void AddModelErrorThrowsIfKeyIsNull() {
            // Arrange
            ModelStateDictionary dictionary = new ModelStateDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    dictionary.AddModelError(null, (string)null);
                }, "key");
        }

        [TestMethod]
        public void AddModelErrorUsesExistingModelStateIfPresent() {
            // Arrange
            ModelStateDictionary dictionary = new ModelStateDictionary();
            dictionary.AddModelError("some key", "some error");
            Exception ex = new Exception();

            // Act
            dictionary.AddModelError("some key", ex);

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            ModelState modelState = dictionary["some key"];

            Assert.AreEqual(2, modelState.Errors.Count);
            Assert.AreEqual("some error", modelState.Errors[0].ErrorMessage);
            Assert.AreSame(ex, modelState.Errors[1].Exception);
        }

        [TestMethod]
        public void ConstructorThrowsIfDictionaryIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ModelStateDictionary((ModelStateDictionary)null);
                }, "dictionary");
        }

        [TestMethod]
        public void ConstructorWithDictionaryParameter() {
            // Arrange
            ModelStateDictionary oldDictionary = new ModelStateDictionary() {
                { "foo", new ModelState() { Value = HtmlHelperTest.GetValueProviderResult("bar", "bar") } }
            };

            // Act
            ModelStateDictionary newDictionary = new ModelStateDictionary(oldDictionary);

            // Assert
            Assert.AreEqual(1, newDictionary.Count);
            Assert.AreEqual("bar", newDictionary["foo"].Value.ConvertTo(typeof(string)));
        }

        [TestMethod]
        public void DictionaryInterface() {
            // Arrange
            DictionaryHelper<string, ModelState> helper = new DictionaryHelper<string, ModelState>() {
                Creator = () => new ModelStateDictionary(),
                Comparer = StringComparer.OrdinalIgnoreCase,
                SampleKeys = new string[] { "foo", "bar", "baz", "quux", "QUUX" },
                SampleValues = new ModelState[] { new ModelState(), new ModelState(), new ModelState(), new ModelState(), new ModelState() },
                ThrowOnKeyNotFound = false
            };

            // Act & assert
            helper.Execute();
        }

        [TestMethod]
        public void DictionaryIsSerializable() {
            // Arrange
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            ModelStateDictionary originalDict = new ModelStateDictionary();
            originalDict.AddModelError("foo", new InvalidOperationException("Some invalid operation."));
            originalDict.AddModelError("foo", new InvalidOperationException("Some other invalid operation."));
            originalDict.AddModelError("bar", "Some exception text.");
            originalDict.SetModelValue("baz", new ValueProviderResult("rawValue", "attemptedValue", CultureInfo.GetCultureInfo("fr-FR")));

            // Act
            formatter.Serialize(stream, originalDict);
            stream.Position = 0;
            ModelStateDictionary deserializedDict = formatter.Deserialize(stream) as ModelStateDictionary;

            // Assert
            Assert.IsNotNull(deserializedDict, "Dictionary was not deserialized correctly.");
            Assert.AreEqual(3, deserializedDict.Count, "Dictionary does not contain correct number of entries.");
            Assert.IsInstanceOfType(deserializedDict["FOO"].Errors[0].Exception, typeof(InvalidOperationException));
            Assert.AreEqual("Some invalid operation.", deserializedDict["FOO"].Errors[0].Exception.Message);
            Assert.IsInstanceOfType(deserializedDict["FOO"].Errors[1].Exception, typeof(InvalidOperationException));
            Assert.AreEqual("Some other invalid operation.", deserializedDict["FOO"].Errors[1].Exception.Message);
            Assert.AreEqual("Some exception text.", deserializedDict["BAR"].Errors[0].ErrorMessage);
            Assert.AreEqual("rawValue", deserializedDict["BAZ"].Value.RawValue);
            Assert.AreEqual("attemptedValue", deserializedDict["BAZ"].Value.AttemptedValue);
            Assert.AreEqual(CultureInfo.GetCultureInfo("fr-FR"), deserializedDict["BAZ"].Value.Culture);
        }

        [TestMethod]
        public void IsValidFieldReturnsFalseIfDictionaryDoesNotContainKey() {
            // Arrange
            ModelStateDictionary msd = new ModelStateDictionary();

            // Act
            bool isValid = msd.IsValidField("foo");

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValidFieldReturnsFalseIfKeyChildContainsErrors() {
            // Arrange
            ModelStateDictionary msd = new ModelStateDictionary();
            msd.AddModelError("foo.bar", "error text");

            // Act
            bool isValid = msd.IsValidField("foo");

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidFieldReturnsFalseIfKeyContainsErrors() {
            // Arrange
            ModelStateDictionary msd = new ModelStateDictionary();
            msd.AddModelError("foo", "error text");

            // Act
            bool isValid = msd.IsValidField("foo");

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidFieldReturnsTrueIfModelStateDoesNotContainErrors() {
            // Arrange
            ModelStateDictionary msd = new ModelStateDictionary() {
                { "foo", new ModelState() { Value = new ValueProviderResult(null, null, null) } }
            };

            // Act
            bool isValid = msd.IsValidField("foo");

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValidFieldThrowsIfKeyIsNull() {
            // Arrange
            ModelStateDictionary msd = new ModelStateDictionary();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    msd.IsValidField(null);
                }, "key");
        }

        [TestMethod]
        public void IsValidPropertyReturnsFalseIfErrors() {
            // Arrange
            ModelState errorState = new ModelState() { Value = HtmlHelperTest.GetValueProviderResult("quux", "quux") };
            errorState.Errors.Add("some error");
            ModelStateDictionary dictionary = new ModelStateDictionary() {
                { "foo", new ModelState() { Value = HtmlHelperTest.GetValueProviderResult("bar", "bar") } },
                { "baz", errorState }
            };

            // Act
            bool isValid = dictionary.IsValid;

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidPropertyReturnsTrueIfNoErrors() {
            // Arrange
            ModelStateDictionary dictionary = new ModelStateDictionary() {
                { "foo", new ModelState() { Value = HtmlHelperTest.GetValueProviderResult("bar", "bar") } },
                { "baz", new ModelState() { Value = HtmlHelperTest.GetValueProviderResult("quux", "bar") } }
            };

            // Act
            bool isValid = dictionary.IsValid;

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void MergeCopiesDictionaryEntries() {
            // Arrange
            ModelStateDictionary fooDict = new ModelStateDictionary() { { "foo", new ModelState() } };
            ModelStateDictionary barDict = new ModelStateDictionary() { { "bar", new ModelState() } };

            // Act
            fooDict.Merge(barDict);

            // Assert
            Assert.AreEqual(2, fooDict.Count);
            Assert.AreEqual(barDict["bar"], fooDict["bar"]);
        }

        [TestMethod]
        public void MergeDoesNothingIfParameterIsNull() {
            // Arrange
            ModelStateDictionary fooDict = new ModelStateDictionary() { { "foo", new ModelState() } };

            // Act
            fooDict.Merge(null);

            // Assert
            Assert.AreEqual(1, fooDict.Count);
            Assert.IsTrue(fooDict.ContainsKey("foo"));
        }

        [TestMethod]
        public void SetAttemptedValueCreatesModelStateIfNotPresent() {
            // Arrange
            ModelStateDictionary dictionary = new ModelStateDictionary();

            // Act
            dictionary.SetModelValue("some key", HtmlHelperTest.GetValueProviderResult("some value", "some value"));

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            ModelState modelState = dictionary["some key"];

            Assert.AreEqual(0, modelState.Errors.Count);
            Assert.AreEqual("some value", modelState.Value.ConvertTo(typeof(string)));
        }

        [TestMethod]
        public void SetAttemptedValueUsesExistingModelStateIfPresent() {
            // Arrange
            ModelStateDictionary dictionary = new ModelStateDictionary();
            dictionary.AddModelError("some key", "some error");
            Exception ex = new Exception();

            // Act
            dictionary.SetModelValue("some key", HtmlHelperTest.GetValueProviderResult("some value", "some value"));

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            ModelState modelState = dictionary["some key"];

            Assert.AreEqual(1, modelState.Errors.Count);
            Assert.AreEqual("some error", modelState.Errors[0].ErrorMessage);
            Assert.AreEqual("some value", modelState.Value.ConvertTo(typeof(string)));
        }

    }
}
