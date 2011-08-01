namespace System.Web.Mvc.Test {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MultiSelectListTest {

        [TestMethod]
        public void Constructor1SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];

            // Act
            MultiSelectList multiSelect = new MultiSelectList(items);

            // Assert
            Assert.AreSame(items, multiSelect.Items);
            Assert.IsNull(multiSelect.DataValueField);
            Assert.IsNull(multiSelect.DataTextField);
            Assert.IsNull(multiSelect.SelectedValues);
        }

        [TestMethod]
        public void Constructor2SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];
            IEnumerable selectedValues = new object[0];

            // Act
            MultiSelectList multiSelect = new MultiSelectList(items, selectedValues);

            // Assert
            Assert.AreSame(items, multiSelect.Items);
            Assert.IsNull(multiSelect.DataValueField);
            Assert.IsNull(multiSelect.DataTextField);
            Assert.AreSame(selectedValues, multiSelect.SelectedValues);
        }

        [TestMethod]
        public void Constructor3SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];

            // Act
            MultiSelectList multiSelect = new MultiSelectList(items, "SomeValueField", "SomeTextField");

            // Assert
            Assert.AreSame(items, multiSelect.Items);
            Assert.AreEqual("SomeValueField", multiSelect.DataValueField);
            Assert.AreEqual("SomeTextField", multiSelect.DataTextField);
            Assert.IsNull(multiSelect.SelectedValues);
        }

        [TestMethod]
        public void Constructor4SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];
            IEnumerable selectedValues = new object[0];

            // Act
            MultiSelectList multiSelect = new MultiSelectList(items, "SomeValueField", "SomeTextField", selectedValues);

            // Assert
            Assert.AreSame(items, multiSelect.Items);
            Assert.AreEqual("SomeValueField", multiSelect.DataValueField);
            Assert.AreEqual("SomeTextField", multiSelect.DataTextField);
            Assert.AreSame(selectedValues, multiSelect.SelectedValues);
        }

        [TestMethod]
        public void ConstructorWithNullItemsThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new MultiSelectList(null /* items */, "dataValueField", "dataTextField", null /* selectedValues */);
                }, "items");
        }

        [TestMethod]
        public void GetListItemsThrowsOnBindingFailure() {
            // Arrange
            MultiSelectList multiSelect = new MultiSelectList(GetSampleFieldObjects(),
                "Text", "Value", new string[] { "A", "C", "T" });

            // Assert
            ExceptionHelper.ExpectHttpException(
                delegate {
                    IList<SelectListItem> listItems = multiSelect.GetListItems();
                }, "DataBinding: 'System.Web.Mvc.Test.MultiSelectListTest+Item' does not contain a property with the name 'Text'.", 500);
        }

        [TestMethod]
        public void GetListItemsWithoutValueField() {
            // Arrange
            MultiSelectList multiSelect = new MultiSelectList(GetSampleStrings());

            // Act
            IList<SelectListItem> listItems = multiSelect.GetListItems();

            // Assert
            Assert.AreEqual(3, listItems.Count);
            Assert.IsNull(listItems[0].Value);
            Assert.AreEqual("Alpha", listItems[0].Text);
            Assert.IsFalse(listItems[0].Selected);
            Assert.IsNull(listItems[1].Value);
            Assert.AreEqual("Bravo", listItems[1].Text);
            Assert.IsFalse(listItems[1].Selected);
            Assert.IsNull(listItems[2].Value);
            Assert.AreEqual("Charlie", listItems[2].Text);
            Assert.IsFalse(listItems[2].Selected);
        }

        [TestMethod]
        public void GetListItemsWithoutValueFieldWithSelections() {
            // Arrange
            MultiSelectList multiSelect = new MultiSelectList(GetSampleStrings(), new string[] { "Alpha", "Charlie", "Tango" });

            // Act
            IList<SelectListItem> listItems = multiSelect.GetListItems();

            // Assert
            Assert.AreEqual(3, listItems.Count);
            Assert.IsNull(listItems[0].Value);
            Assert.AreEqual("Alpha", listItems[0].Text);
            Assert.IsTrue(listItems[0].Selected);
            Assert.IsNull(listItems[1].Value);
            Assert.AreEqual("Bravo", listItems[1].Text);
            Assert.IsFalse(listItems[1].Selected);
            Assert.IsNull(listItems[2].Value);
            Assert.AreEqual("Charlie", listItems[2].Text);
            Assert.IsTrue(listItems[2].Selected);
        }

        [TestMethod]
        public void GetListItemsWithValueField() {
            // Arrange
            MultiSelectList multiSelect = new MultiSelectList(GetSampleAnonymousObjects(), "Letter", "FullWord");

            // Act
            IList<SelectListItem> listItems = multiSelect.GetListItems();

            // Assert
            Assert.AreEqual(3, listItems.Count);
            Assert.AreEqual("A", listItems[0].Value);
            Assert.AreEqual("Alpha", listItems[0].Text);
            Assert.IsFalse(listItems[0].Selected);
            Assert.AreEqual("B", listItems[1].Value);
            Assert.AreEqual("Bravo", listItems[1].Text);
            Assert.IsFalse(listItems[1].Selected);
            Assert.AreEqual("C", listItems[2].Value);
            Assert.AreEqual("Charlie", listItems[2].Text);
            Assert.IsFalse(listItems[2].Selected);
        }

        [TestMethod]
        public void GetListItemsWithValueFieldWithSelections() {
            // Arrange
            MultiSelectList multiSelect = new MultiSelectList(GetSampleAnonymousObjects(),
                "Letter", "FullWord", new string[] { "A", "C", "T" });

            // Act
            IList<SelectListItem> listItems = multiSelect.GetListItems();

            // Assert
            Assert.AreEqual(3, listItems.Count);
            Assert.AreEqual("A", listItems[0].Value);
            Assert.AreEqual("Alpha", listItems[0].Text);
            Assert.IsTrue(listItems[0].Selected);
            Assert.AreEqual("B", listItems[1].Value);
            Assert.AreEqual("Bravo", listItems[1].Text);
            Assert.IsFalse(listItems[1].Selected);
            Assert.AreEqual("C", listItems[2].Value);
            Assert.AreEqual("Charlie", listItems[2].Text);
            Assert.IsTrue(listItems[2].Selected);
        }

        [TestMethod]
        public void IEnumerableWithAnonymousObjectsAndTextValueFields() {
            // Arrange
            MultiSelectList multiSelect = new MultiSelectList(GetSampleAnonymousObjects(),
                "Letter", "FullWord", new string[] { "A", "C", "T" });

            // Act
            IEnumerator enumerator = multiSelect.GetEnumerator();
            enumerator.MoveNext();
            SelectListItem firstItem = enumerator.Current as SelectListItem;
            SelectListItem lastItem = null;

            while (enumerator.MoveNext()) {
                lastItem = enumerator.Current as SelectListItem;
            }

            // Assert
            Assert.IsTrue(firstItem != null);
            Assert.AreEqual("Alpha", firstItem.Text);
            Assert.AreEqual("A", firstItem.Value);
            Assert.IsTrue(firstItem.Selected);

            Assert.IsTrue(lastItem != null);
            Assert.AreEqual("Charlie", lastItem.Text);
            Assert.AreEqual("C", lastItem.Value);
            Assert.IsTrue(lastItem.Selected);
        }


        internal static IEnumerable GetSampleAnonymousObjects() {
            return new[] {
                new { Letter = 'A', FullWord = "Alpha" },
                new { Letter = 'B', FullWord = "Bravo" },
                new { Letter = 'C', FullWord = "Charlie" }
            };
        }

        internal static IEnumerable GetSampleFieldObjects() {
            return new[] {
                new Item { Text = "A", Value = "Alpha" },
                new Item { Text = "B", Value = "Bravo" },
                new Item { Text = "C", Value = "Charlie" }
            };
        }

        internal static List<SelectListItem> GetSampleListObjects() {
            List<SelectListItem> list = new List<SelectListItem>();
            string selectedSSN = "111111111";

            foreach (Person person in GetSamplePeople()) {
                list.Add(new SelectListItem {
                    Text = person.FirstName,
                    Value = person.SSN,
                    Selected = String.Equals(person.SSN, selectedSSN)
                });
            }
            return list;
        }

        internal static IEnumerable<SelectListItem> GetSampleIEnumerableObjects() {
            Person[] people = GetSamplePeople();

            string selectedSSN = "111111111";
            IEnumerable<SelectListItem> list = from person in people
                                               select new SelectListItem {
                                                   Text = person.FirstName,
                                                   Value = person.SSN,
                                                   Selected = String.Equals(person.SSN, selectedSSN)
                                               };
            return list;
        }

        internal static IEnumerable GetSampleStrings() {
            return new string[] { "Alpha", "Bravo", "Charlie" };
        }

        internal static Person[] GetSamplePeople() {
            return new Person[] {
                new Person {
                    FirstName = "John",
                    SSN="123456789"
                }, 
                new Person {
                    FirstName = "Jane",
                    SSN="987654321"
                },
                new Person {
                    FirstName = "Joe",
                    SSN="111111111"
                }
            };
        }

        internal class Item {
            public string Text;
            public string Value;
        }

        internal class Person {
            public string FirstName {
                get;
                set;
            }

            public string SSN {
                get;
                set;
            }
        }
    }
}
