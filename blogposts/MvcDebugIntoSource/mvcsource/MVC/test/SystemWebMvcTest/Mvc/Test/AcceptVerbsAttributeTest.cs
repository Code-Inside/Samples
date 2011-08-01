namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AcceptVerbsAttributeTest {

        private const string _invalidEnumFormatString = @"The enum '{0}' did not produce the correct array.
Expected: {1}
Actual: {2}";

        [TestMethod]
        public void ConstructorThrowsIfVerbsIsEmpty() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new AcceptVerbsAttribute(new string[0]);
                }, "verbs");
        }

        [TestMethod]
        public void ConstructorThrowsIfVerbsIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new AcceptVerbsAttribute((string[])null);
                }, "verbs");
        }

        [TestMethod]
        public void EnumToArray() {
            // Arrange
            IDictionary<string, HttpVerbs> enumValues = EnumToDictionary<HttpVerbs>();
            var allCombinations = EnumerableToCombinations(enumValues);

            // Act & assert
            foreach (var combination in allCombinations) {
                // generate all the names + values in this combination
                List<string> aggrNames = new List<string>();
                HttpVerbs aggrValues = (HttpVerbs)0;
                foreach (var entry in combination) {
                    aggrNames.Add(entry.Key);
                    aggrValues |= entry.Value;
                }

                // get the resulting array
                string[] array = AcceptVerbsAttribute.EnumToArray(aggrValues);
                var aggrNamesOrdered = aggrNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase);
                var arrayOrdered = array.OrderBy(name => name, StringComparer.OrdinalIgnoreCase);
                bool match = aggrNamesOrdered.SequenceEqual(arrayOrdered, StringComparer.OrdinalIgnoreCase);

                if (!match) {
                    string message = String.Format(_invalidEnumFormatString, aggrValues,
                        aggrNames.Aggregate((a, b) => a + ", " + b),
                        array.Aggregate((a, b) => a + ", " + b));
                    Assert.Fail(message);
                }
            }
        }

        [TestMethod]
        public void IsValidForRequestReturnsFalseIfHttpVerbIsNotInVerbsCollection() {
            // Arrange
            AcceptVerbsAttribute attr = new AcceptVerbsAttribute("get", "post");
            ControllerContext context = GetControllerContextWithHttpVerb("HEAD");

            // Act
            bool result = attr.IsValidForRequest(context, null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidForRequestReturnsTrueIfHttpVerbIsInVerbsCollection() {
            // Arrange
            AcceptVerbsAttribute attr = new AcceptVerbsAttribute("get", "post");
            ControllerContext context = GetControllerContextWithHttpVerb("POST");

            // Act
            bool result = attr.IsValidForRequest(context, null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidForRequestThrowsIfControllerContextIsNull() {
            // Arrange
            AcceptVerbsAttribute attr = new AcceptVerbsAttribute("get", "post");

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.IsValidForRequest(null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void VerbsPropertyFromEnumConstructor() {
            // Arrange
            AcceptVerbsAttribute attr = new AcceptVerbsAttribute(HttpVerbs.Get | HttpVerbs.Post);

            // Act
            ReadOnlyCollection<string> collection = attr.Verbs as ReadOnlyCollection<string>;

            // Assert
            Assert.IsNotNull(collection, "Verbs property should have returned read-only collection.");
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual("GET", collection[0]);
            Assert.AreEqual("POST", collection[1]);
        }

        [TestMethod]
        public void VerbsPropertyFromStringArrayConstructor() {
            // Arrange
            AcceptVerbsAttribute attr = new AcceptVerbsAttribute("get", "post");

            // Act
            ReadOnlyCollection<string> collection = attr.Verbs as ReadOnlyCollection<string>;

            // Assert
            Assert.IsNotNull(collection, "Verbs property should have returned read-only collection.");
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual("get", collection[0]);
            Assert.AreEqual("post", collection[1]);
        }

        private static ControllerContext GetControllerContextWithHttpVerb(string httpVerb) {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.HttpMethod).Returns(httpVerb);
            return mockControllerContext.Object;
        }

        private static IDictionary<string, TEnum> EnumToDictionary<TEnum>() {
            // Arrange
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            return values.ToDictionary(value => Enum.GetName(typeof(TEnum), value), value => value);
        }

        private static IEnumerable<ICollection<T>> EnumerableToCombinations<T>(IEnumerable<T> elements) {
            List<T> allElements = elements.ToList();

            int maxCount = 1 << allElements.Count;
            for (int idxCombination = 0; idxCombination < maxCount; idxCombination++) {
                List<T> thisCollection = new List<T>();
                for (int idxBit = 0; idxBit < 32; idxBit++) {
                    bool bitActive = (((uint)idxCombination >> idxBit) & 1) != 0;
                    if (bitActive) {
                        thisCollection.Add(allElements[idxBit]);
                    }
                }
                yield return thisCollection;
            }
        }

    }
}
