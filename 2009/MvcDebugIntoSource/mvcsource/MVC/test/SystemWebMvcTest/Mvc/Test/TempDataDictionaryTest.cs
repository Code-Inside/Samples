namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
   
    [TestClass]
    public class TempDataDictionaryTest {
        
        [TestMethod]
        public void TempDataCanBeSerialized() {
            // Arrange
            TempDataDictionary tempData = new TempDataDictionary();
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binFormatter = new BinaryFormatter();

            tempData["Key1"] = "Value1";
            tempData["Key2"] = "Value2";
            tempData["Key3"] = "Value3";

            // Act            
            memStream.Seek(0, SeekOrigin.Begin);
            binFormatter.Serialize(memStream, tempData);
            memStream.Seek(0, SeekOrigin.Begin);
            TempDataDictionary deserializedTempData = binFormatter.Deserialize(memStream, null) as TempDataDictionary;
            object value = deserializedTempData["KEY3"];

            // Assert
            Assert.AreEqual(deserializedTempData["Key1"], "Value1");
            Assert.AreEqual(deserializedTempData["Key2"], "Value2");
            Assert.AreEqual(deserializedTempData["Key3"], "Value3");
            Assert.AreSame(deserializedTempData["Key3"], value);
        }

        [TestMethod]
        public void CompareIsOrdinalIgnoreCase() {
            // Arrange
            TempDataDictionary tempData = new TempDataDictionary();
            object item = new object();

            // Act
            tempData["Foo"] = item;
            object value = tempData["FOO"];

            // Assert
            Assert.AreSame(item, value);
        }

        [TestMethod]
        public void TempDataIsADictionary() {
            // Arrange
            TempDataDictionary tempData = new TempDataDictionary();

            // Act
            tempData["Key1"] = "Value1";
            tempData.Add("Key2", "Value2");
            ((ICollection<KeyValuePair<string, object>>)tempData).Add(new KeyValuePair<string,object>("Key3", "Value3"));

            // Assert (IDictionary)
            Assert.AreEqual(3, tempData.Count, "tempData should contain 3 items");
            Assert.IsTrue(tempData.Remove("Key1"), "The key should be present");
            Assert.IsFalse(tempData.Remove("Key4"), "The key should not be present");
            Assert.IsTrue(tempData.ContainsValue("Value2"), "The value should be present");
            Assert.IsFalse(tempData.ContainsValue("Value1"), "The value should not be present");
            Assert.IsNull(tempData["Key6"], "The key should not be present");

            IEnumerator tempDataEnumerator = tempData.GetEnumerator();
            tempDataEnumerator.Reset();
            while (tempDataEnumerator.MoveNext()) {
                KeyValuePair<string, object> pair = (KeyValuePair<string, object>)tempDataEnumerator.Current;
                Assert.IsTrue(((ICollection<KeyValuePair<string, object>>)tempData).Contains(pair), "The key/value pair should be present");
            }
            
            // Assert (ICollection)
            foreach (string key in tempData.Keys) {
                Assert.IsTrue(((ICollection<KeyValuePair<string, object>>)tempData).Contains(new KeyValuePair<string, object>(key, tempData[key])), "The key/value pair should be present");
            }

            foreach (string value in tempData.Values) {
                Assert.IsTrue(tempData.ContainsValue(value));
            }

            foreach (string key in ((IDictionary<string, object>)tempData).Keys) {
                Assert.IsTrue(tempData.ContainsKey(key), "The key should be present");
            }

            foreach (string value in ((IDictionary<string, object>)tempData).Values) {
                Assert.IsTrue(tempData.ContainsValue(value));
            }

            KeyValuePair<string, object>[] keyValuePairArray = new KeyValuePair<string, object>[tempData.Count];
            ((ICollection<KeyValuePair<string, object>>)tempData).CopyTo(keyValuePairArray, 0);

            Assert.IsFalse(((ICollection<KeyValuePair<string, object>>)tempData).IsReadOnly, "The dictionary should not be read-only.");
            
            Assert.IsFalse(((ICollection<KeyValuePair<string, object>>)tempData).Remove(new KeyValuePair<string, object>("Key5", "Value5")), "The key/value pair should not be present");

            IEnumerator<KeyValuePair<string, object>> keyValuePairEnumerator = ((ICollection<KeyValuePair<string, object>>)tempData).GetEnumerator();
            keyValuePairEnumerator.Reset();
            while (keyValuePairEnumerator.MoveNext()) {
                KeyValuePair<string, object> pair = keyValuePairEnumerator.Current;
                Assert.IsTrue(((ICollection<KeyValuePair<string, object>>)tempData).Contains(pair), "The key/value pair should be present");
            }

            // Act
            tempData.Clear();

            // Assert
            Assert.AreEqual(0, tempData.Count, "tempData should not contain any items");

            IEnumerator y = ((IEnumerable)tempData).GetEnumerator();
            while (y.MoveNext()) {
                Assert.Fail("There should not be any elements in tempData");
            }
        }

        [TestMethod]
        public void TempDataDictionaryCreatesEmptyDictionaryIfProviderReturnsNull() {
            // Arrange
            TempDataDictionary tempDataDictionary = new TempDataDictionary();
            NullTempDataProvider provider = new NullTempDataProvider();

            // Act
            tempDataDictionary.Load(null /* controllerContext */, provider);

            // Assert
            Assert.AreEqual(tempDataDictionary.Count, 0);
        }

        internal class NullTempDataProvider : ITempDataProvider {
            public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values) {
            }

            public IDictionary<string, object> LoadTempData(ControllerContext controllerContext) {
                return null;
            }
        }
    }
}
