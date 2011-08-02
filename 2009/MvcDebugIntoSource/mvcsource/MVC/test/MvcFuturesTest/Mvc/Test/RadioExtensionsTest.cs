namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class RadioExtensionsTest {
        [TestMethod]
        public void RadioButtonListNothingSelected() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList", GetRadioButtonListData(false));

            // Assert
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListItemSelected() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList", GetRadioButtonListData(true));

            // Assert
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input checked=""checked"" id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListItemSelectedWithValueFromViewData() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(new ViewDataDictionary(new { foolist = "bar" }));

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList", GetRadioButtonListData(false));

            // Assert
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input checked=""checked"" id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListWithObjectAttributes() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList", GetRadioButtonListData(true), new { attr1 = "value1" });

            // Assert
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input attr1=""value1"" checked=""checked"" id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListWithDictionaryAttributes() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList", GetRadioButtonListData(true), new RouteValueDictionary(new { attr1 = "value1" }));

            // Assert
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input attr1=""value1"" checked=""checked"" id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListNothingSelectedWithSelectListFromViewData() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(GetRadioButtonListViewData(false));

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList");

            // Assert
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListItemSelectedWithSelectListFromViewData() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(GetRadioButtonListViewData(true));

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList");

            // Assert
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input checked=""checked"" id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListWithObjectAttributesWithSelectListFromViewData() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(GetRadioButtonListViewData(true));

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList", new { attr1 = "value1" });

            // Assert
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input attr1=""value1"" checked=""checked"" id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        [TestMethod]
        public void RadioButtonListWithDictionaryAttributesWithSelectListFromViewData() {
            // Arrange
            HtmlHelper htmlHelper = TestHelper.GetHtmlHelper(GetRadioButtonListViewData(true));

            // Act
            string[] html = htmlHelper.RadioButtonList("FooList", new RouteValueDictionary(new { attr1 = "value1" }));

            // Assert
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""foo"" />", html[0]);
            Assert.AreEqual(@"<input attr1=""value1"" id=""FooList"" name=""FooList"" type=""radio"" value=""bar"" />", html[1]);
            Assert.AreEqual(@"<input attr1=""value1"" checked=""checked"" id=""FooList"" name=""FooList"" type=""radio"" value=""baz"" />", html[2]);
        }

        private static SelectList GetRadioButtonListData(bool selectBaz) {
            List<RadioItem> list = new List<RadioItem>();
            list.Add(new RadioItem { Text = "text-foo", Value = "foo" });
            list.Add(new RadioItem { Text = "text-bar", Value = "bar" });
            list.Add(new RadioItem { Text = "text-baz", Value = "baz" });
            return new SelectList(list, "value", "TEXT", selectBaz ? "baz" : "something-else");
        }

        private static ViewDataDictionary GetRadioButtonListViewData(bool selectBaz) {
            ViewDataDictionary viewData = new ViewDataDictionary();
            viewData["FooList"] = GetRadioButtonListData(selectBaz);
            return viewData;
        }

        private class RadioItem {
            public string Text {
                get;
                set;
            }

            public string Value {
                get;
                set;
            }
        }
    }
}
