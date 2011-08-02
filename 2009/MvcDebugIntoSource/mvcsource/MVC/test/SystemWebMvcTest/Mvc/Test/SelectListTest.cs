namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Web.TestUtil;
    using System.Web.Mvc;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class SelectListTest {

        [TestMethod]
        public void Constructor1SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];

            // Act
            SelectList selectList = new SelectList(items);

            // Assert
            Assert.AreSame(items, selectList.Items);
            Assert.IsNull(selectList.DataValueField);
            Assert.IsNull(selectList.DataTextField);
            Assert.IsNull(selectList.SelectedValues);
            Assert.IsNull(selectList.SelectedValue);
        }

        [TestMethod]
        public void Constructor2SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];
            object selectedValue = new object();

            // Act
            SelectList selectList = new SelectList(items, selectedValue);
            List<object> selectedValues = selectList.SelectedValues.Cast<object>().ToList();

            // Assert
            Assert.AreSame(items, selectList.Items);
            Assert.IsNull(selectList.DataValueField);
            Assert.IsNull(selectList.DataTextField);
            Assert.AreSame(selectedValue, selectList.SelectedValue);
            Assert.AreEqual(1, selectedValues.Count);
            Assert.AreSame(selectedValue, selectedValues[0]);
        }

        [TestMethod]
        public void Constructor3SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];

            // Act
            SelectList selectList = new SelectList(items, "SomeValueField", "SomeTextField");

            // Assert
            Assert.AreSame(items, selectList.Items);
            Assert.AreEqual("SomeValueField", selectList.DataValueField);
            Assert.AreEqual("SomeTextField", selectList.DataTextField);
            Assert.IsNull(selectList.SelectedValues);
            Assert.IsNull(selectList.SelectedValue);
        }

        [TestMethod]
        public void Constructor4SetsProperties() {
            // Arrange
            IEnumerable items = new object[0];
            object selectedValue = new object();

            // Act
            SelectList selectList = new SelectList(items, "SomeValueField", "SomeTextField", selectedValue);
            List<object> selectedValues = selectList.SelectedValues.Cast<object>().ToList();

            // Assert
            Assert.AreSame(items, selectList.Items);
            Assert.AreEqual("SomeValueField", selectList.DataValueField);
            Assert.AreEqual("SomeTextField", selectList.DataTextField);
            Assert.AreSame(selectedValue, selectList.SelectedValue);
            Assert.AreEqual(1, selectedValues.Count);
            Assert.AreSame(selectedValue, selectedValues[0]);
        }
    }
}
