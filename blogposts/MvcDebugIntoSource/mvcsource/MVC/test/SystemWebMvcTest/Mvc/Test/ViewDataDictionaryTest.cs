namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ViewDataDictionaryTest {

        [TestMethod]
        public void ConstructorThrowsIfDictionaryIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewDataDictionary((ViewDataDictionary)null);
                }, "dictionary");
        }

        [TestMethod]
        public void ConstructorWithViewDataDictionaryCopiesModelAndModelState() {
            // Arrange
            ViewDataDictionary originalVdd = new ViewDataDictionary();
            object model = new object();
            originalVdd.Model = model;
            originalVdd["foo"] = "bar";
            originalVdd.ModelState.AddModelError("key", "error");

            // Act
            ViewDataDictionary newVdd = new ViewDataDictionary(originalVdd);

            // Assert
            Assert.AreEqual(model, newVdd.Model);
            Assert.IsTrue(newVdd.ModelState.ContainsKey("key"));
            Assert.AreEqual("error", newVdd.ModelState["key"].Errors[0].ErrorMessage);
            Assert.AreEqual("bar", newVdd["foo"]);
        }

        [TestMethod]
        public void DictionaryInterface() {
            // Arrange
            DictionaryHelper<string, object> helper = new DictionaryHelper<string, object>() {
                Creator = () => new ViewDataDictionary(),
                Comparer = StringComparer.OrdinalIgnoreCase,
                SampleKeys = new string[] { "foo", "bar", "baz", "quux", "QUUX" },
                SampleValues = new object[] { 42, "string value", new DateTime(2001, 1, 1), new object(), 32m },
                ThrowOnKeyNotFound = false
            };

            // Act & assert
            helper.Execute();
        }

        [TestMethod]
        public void EvalReturnsSimplePropertyValue() {
            var obj = new { Foo = "Bar" };
            ViewDataDictionary vdd = new ViewDataDictionary(obj);

            Assert.AreEqual("Bar", vdd.Eval("Foo"));
        }

        [TestMethod]
        public void EvalWithModelAndDictionaryPropertyEvaluatesDictionaryValue() {
            var obj = new { Foo = new Dictionary<string, object> { { "Bar", "Baz" } } };
            ViewDataDictionary vdd = new ViewDataDictionary(obj);

            Assert.AreEqual("Baz", vdd.Eval("Foo.Bar"));
        }

        [TestMethod]
        public void EvalEvaluatesDictionaryThenModel() {
            var obj = new { Foo = "NotBar" };
            ViewDataDictionary vdd = new ViewDataDictionary(obj);
            vdd.Add("Foo", "Bar");

            Assert.AreEqual("Bar", vdd.Eval("Foo"));
        }

        [TestMethod]
        public void EvalReturnsValueOfCompoundExpressionByFollowingObjectPath() {
            var obj = new { Foo = new { Bar = "Baz" } };
            ViewDataDictionary vdd = new ViewDataDictionary(obj);

            Assert.AreEqual("Baz", vdd.Eval("Foo.Bar"));
        }

        [TestMethod]
        public void EvalReturnsNullIfExpressionDoesNotMatch() {
            var obj = new { Foo = new { Biz = "Baz" } };
            ViewDataDictionary vdd = new ViewDataDictionary(obj);

            Assert.AreEqual(null, vdd.Eval("Foo.Bar"));
        }

        [TestMethod]
        public void EvalReturnsValueJustAdded() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", "Blah");

            Assert.AreEqual("Blah", vdd.Eval("Foo"));
        }

        [TestMethod]
        public void EvalWithCompoundExpressionReturnsIndexedValue() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo.Bar", "Baz");

            Assert.AreEqual("Baz", vdd.Eval("Foo.Bar"));
        }

        [TestMethod]
        public void EvalWithCompoundExpressionReturnsPropertyOfAddedObject() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", new { Bar = "Baz" });

            Assert.AreEqual("Baz", vdd.Eval("Foo.Bar"));
        }

        [TestMethod]
        public void EvalWithCompoundIndexExpressionReturnsEval() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo.Bar", new { Baz = "Quux" });

            Assert.AreEqual("Quux", vdd.Eval("Foo.Bar.Baz"));
        }

        [TestMethod]
        public void EvalWithCompoundIndexAndCompoundExpressionReturnsValue() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo.Bar", new { Baz = new { Blah = "Quux" } });

            Assert.AreEqual("Quux", vdd.Eval("Foo.Bar.Baz.Blah"));
        }

        /// <summary>
        /// Make sure that dict["foo.bar"] gets chosen before dict["foo"]["bar"]
        /// </summary>
        [TestMethod]
        public void EvalChoosesValueInDictionaryOverOtherValue() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", new { Bar = "Not Baz" });
            vdd.Add("Foo.Bar", "Baz");

            Assert.AreEqual("Baz", vdd.Eval("Foo.Bar"));
        }

        /// <summary>
        /// Make sure that dict["foo.bar"]["baz"] gets chosen before dict["foo"]["bar"]["baz"]
        /// </summary>
        [TestMethod]
        public void EvalChoosesCompoundValueInDictionaryOverOtherValues() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", new { Bar = new { Baz = "Not Quux" } });
            vdd.Add("Foo.Bar", new { Baz = "Quux" });

            Assert.AreEqual("Quux", vdd.Eval("Foo.Bar.Baz"));
        }

        /// <summary>
        /// Make sure that dict["foo.bar"]["baz"] gets chosen before dict["foo"]["bar.baz"]
        /// </summary>
        [TestMethod]
        public void EvalChoosesCompoundValueInDictionaryOverOtherValuesWithCompoundProperty() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", new Person());
            vdd.Add("Foo.Bar", new { Baz = "Quux" });

            Assert.AreEqual("Quux", vdd.Eval("Foo.Bar.Baz"));
        }

        [TestMethod]
        public void EvalThrowsIfExpressionIsEmpty() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    vdd.Eval(String.Empty);
                }, "expression");
        }

        [TestMethod]
        public void EvalThrowsIfExpressionIsNull() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    vdd.Eval(null);
                }, "expression");
        }

        [TestMethod]
        public void EvalWithCompoundExpressionAndDictionarySubExpressionChoosesDictionaryValue() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", new Dictionary<string, object> { { "Bar", "Baz" } });

            Assert.AreEqual("Baz", vdd.Eval("Foo.Bar"));
        }

        [TestMethod]
        public void EvalWithDictionaryAndNoMatchReturnsNull() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", new Dictionary<string, object> { { "NotBar", "Baz" } });

            object result = vdd.Eval("Foo.Bar");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void EvalWithNestedDictionariesEvalCorrectly() {
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("Foo", new Dictionary<string, object> { { "Bar", new Hashtable { { "Baz", "Quux" } } } });

            Assert.AreEqual("Quux", vdd.Eval("Foo.Bar.Baz"));
        }

        [TestMethod]
        public void EvalFormatWithNullValueReturnsEmptyString() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();

            // Act
            string formattedValue = vdd.Eval("foo", "for{0}mat");

            // Assert
            Assert.AreEqual<string>(String.Empty, formattedValue);
        }

        [TestMethod]
        public void EvalFormatWithEmptyFormatReturnsViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["foo"] = "value";

            // Act
            string formattedValue = vdd.Eval("foo", "");

            // Assert
            Assert.AreEqual<string>("value", formattedValue);
        }

        [TestMethod]
        public void EvalFormatWithFormatReturnsFormattedViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["foo"] = "value";

            // Act
            string formattedValue = vdd.Eval("foo", "for{0}mat");

            // Assert
            Assert.AreEqual<string>("forvaluemat", formattedValue);
        }

        [TestMethod]
        public void EvalPropertyNamedModel() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["Title"] = "Home Page";
            vdd["Message"] = "Welcome to ASP.NET MVC!";
            vdd.Model = new TheQueryStringParam {
                Name = "The Name",
                Value = "The Value",
                Model = "The Model",
            };

            // Act
            object o = vdd.Eval("Model");

            // Assert
            Assert.AreEqual("The Model", o);
        }

        [TestMethod]
        public void EvalSubPropertyNamedValueInModel() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["Title"] = "Home Page";
            vdd["Message"] = "Welcome to ASP.NET MVC!";
            vdd.Model = new TheQueryStringParam {
                Name = "The Name",
                Value = "The Value",
                Model = "The Model",
            };

            // Act
            object o = vdd.Eval("Value");

            // Assert
            Assert.AreEqual("The Value", o);
        }

        public class TheQueryStringParam {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Model { get; set; }
        }

        public class Person : CustomTypeDescriptor {
            public override PropertyDescriptorCollection GetProperties() {
                return new PropertyDescriptorCollection(new PersonPropertyDescriptor[] { new PersonPropertyDescriptor() });
            }
        }

        public class PersonPropertyDescriptor : PropertyDescriptor {
            public PersonPropertyDescriptor()
                : base("Bar.Baz", null) {
            }

            public override object GetValue(object component) {
                return "Quux";
            }

            public override bool CanResetValue(object component) {
                return false;
            }

            public override Type ComponentType {
                get {
                    return typeof(Person);
                }
            }

            public override bool IsReadOnly {
                get {
                    return false;
                }
            }

            public override Type PropertyType {
                get {
                    return typeof(string);
                }
            }

            public override void ResetValue(object component) {
            }

            public override void SetValue(object component, object value) {
            }

            public override bool ShouldSerializeValue(object component) {
                return true;
            }
        }
    }
}
