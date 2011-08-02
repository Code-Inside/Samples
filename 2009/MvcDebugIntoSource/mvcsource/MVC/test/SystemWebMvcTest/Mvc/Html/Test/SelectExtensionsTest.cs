namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Mvc.Test;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SelectExtensionsTest {
        private static readonly ViewDataDictionary _listBoxViewData = new ViewDataDictionary { { "foo", new[] { "Bravo" } } };
        private static readonly ViewDataDictionary _dropDownListViewData = new ViewDataDictionary { { "foo", "Bravo" } };

        private static ViewDataDictionary GetViewDataWithSelectList() {
            ViewDataDictionary viewData = new ViewDataDictionary();
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", "C");
            viewData["foo"] = selectList;
            viewData["foo.bar"] = selectList;
            return viewData;
        }

        [TestMethod]
        public void DropDownListUsesExplicitValueIfNotProvidedInViewData() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", "C");

            // Act
            string html = helper.DropDownList("foo", selectList, (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListUsesViewDataDefaultValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(_dropDownListViewData);
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), "Charlie");

            // Act
            string html = helper.DropDownList("foo", selectList, (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListUsesViewDataDefaultValueNoOptionLabel() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(_dropDownListViewData);
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), "Charlie");

            // Act
            string html = helper.DropDownList("foo", selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithAttributesDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            string html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.DropDownList(String.Empty, (SelectList)null /* selectList */, (string)null /* optionLabel */);
                },
                "name");
        }

        [TestMethod]
        public void DropDownListWithErrors() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(viewData);

            // Act
            string html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithErrorsAndCustomClass() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(viewData);

            // Act
            string html = helper.DropDownList("foo", selectList, null /* optionLabel */, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error foo-class"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.DropDownList(null /* name */, (SelectList)null /* selectList */, (string)null /* optionLabel */);
                },
                "name");
        }

        [TestMethod]
        public void DropDownListWithNullSelectListUsesViewData() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();
            helper.ViewData["foo"] = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            string html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                 @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithObjectDictionary() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            ViewDataDictionary viewData = new ViewDataDictionary();
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(viewData);

            // Act
            string html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndSelectList() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            string html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndSelectListNoOptionLabel() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            string html = helper.DropDownList("foo", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndEmptyOptionLabel() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            string html = helper.DropDownList("foo", selectList, String.Empty /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option value=""""></option>
<option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndTitle() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            string html = helper.DropDownList("foo", selectList, "[Select Something]", HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option value="""">[Select Something]</option>
<option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListUsesViewDataSelectList() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithSelectList());

            // Act
            string html = helper.DropDownList("foo", (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListUsesModelState() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            ViewDataDictionary viewData = GetViewDataWithErrors();
            viewData["foo"] = selectList;
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(viewData);

            // Act
            string html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListUsesViewDataSelectListNoOptionLabel() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithSelectList());

            // Act
            string html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithDotReplacementForId() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithSelectList());

            // Act
            string html = helper.DropDownList("foo.bar");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo_bar"" name=""foo.bar""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithIEnumerableSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleIEnumerableObjects() } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithIEnumerableSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", "123456789" } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.DropDownList("foo", MultiSelectListTest.GetSampleIEnumerableObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithListOfSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", "123456789" } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.DropDownList("foo", MultiSelectListTest.GetSampleListObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithListOfSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleListObjects() } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void DropDownListWithNullViewDataValueThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    helper.DropDownList("foo", (string)null /* optionLabel */);
                },
                "There is no ViewData item with the key 'foo' of type 'IEnumerable<SelectListItem>'.");
        }

        [TestMethod]
        public void DropDownListWithWrongViewDataTypeValueThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary { { "foo", 123 } });

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    helper.DropDownList("foo", (string)null /* optionLabel */);
                },
                "The ViewData item with the key 'foo' is of type 'System.Int32' but needs to be of type 'IEnumerable<SelectListItem>'.");
        }

        [TestMethod]
        public void ListBoxUsesExplicitValueIfNotProvidedInViewData() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", new[] { "A", "C" });

            // Act
            string html = helper.ListBox("foo", selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxUsesViewDataDefaultValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(_listBoxViewData);
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            string html = helper.ListBox("foo", selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithErrors() {
            // Arrange
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(viewData);
            MultiSelectList list = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            string html = helper.ListBox("foo", list);

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithErrorsAndCustomClass() {
            // Arrange
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(viewData);
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            string html = helper.ListBox("foo", selectList, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error foo-class"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithNameOnly() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();
            helper.ViewData["foo"] = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            string html = helper.ListBox("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithAttributesDictionary() {
            // Arrange
            ViewDataDictionary viewData = new ViewDataDictionary();
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            //viewData["foo"] = selectList;
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(viewData);

            // Act
            string html = helper.ListBox("foo", selectList, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithAttributesDictionaryAndMultiSelectList() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            string html = helper.ListBox("foo", selectList, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.ListBox(String.Empty, (MultiSelectList)null /* selectList */);
                },
                "name");
        }

        [TestMethod]
        public void ListBoxWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.ListBox(null /* name */, (MultiSelectList)null /* selectList */);
                },
                "name");
        }

        [TestMethod]
        public void ListBoxWithNullSelectListUsesViewData() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();
            helper.ViewData["foo"] = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            string html = helper.ListBox("foo", null);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithObjectDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            string html = helper.ListBox("foo", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithIEnumerableSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleIEnumerableObjects() } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.ListBox("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithIEnumerableSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", "123456789" } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.ListBox("foo", MultiSelectListTest.GetSampleIEnumerableObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithListOfSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", new string[] { "123456789", "111111111" } } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.ListBox("foo", MultiSelectListTest.GetSampleListObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void ListBoxWithListOfSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleListObjects() } };
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act
            string html = helper.ListBox("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html);
        }

        [TestMethod]
        public void SelectHelpersUseCurrentCultureToConvertValues() {
            // Arrange
            HtmlHelper defaultValueHelper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary { 
            { "foo", new [] {new DateTime(1900, 1, 1, 0, 0, 1)}},
            { "bar", new DateTime(1900, 1, 1, 0, 0, 1)} });
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();
            SelectList selectList = new SelectList(GetSampleCultureAnonymousObjects(), "Date", "FullWord", new DateTime(1900, 1, 1, 0, 0, 0));

            var tests = new[] {
                // DropDownList(name, selectList, optionLabel)
                new { 
                    Html = @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<string>(() => helper.DropDownList("foo", selectList, (string)null))
                },
                // DropDownList(name, selectList, optionLabel) (With default value selected from ViewData)
                new { 
                    Html = @"<select id=""bar"" name=""bar""><option value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option selected=""selected"" value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<string>(() => defaultValueHelper.DropDownList("bar", selectList, (string)null))
                },

                // ListBox(name, selectList)
                new { 
                    Html = @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<string>(() => helper.ListBox("foo", selectList))
                },

                // ListBox(name, selectList) (With default value selected from ViewData)
                new { 
                    Html = @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option selected=""selected"" value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<string>(() => defaultValueHelper.ListBox("foo", selectList))
                }
            };

            // Act && Assert
            using (HtmlHelperTest.ReplaceCulture("en-ZA", "en-US")) {
                foreach (var test in tests) {
                    Assert.AreEqual(test.Html, test.Action());
                }
            }
        }

        private static ViewDataDictionary GetViewDataWithErrors() {
            ViewDataDictionary viewData = new ViewDataDictionary { { "foo", "ViewDataFoo" } };
            viewData.Model = new { foo = "ViewItemFoo", bar = "ViewItemBar" };

            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = new ValueProviderResult(new string[] { "Bravo", "Charlie" }, "Bravo", CultureInfo.InvariantCulture);

            return viewData;
        }

        internal static IEnumerable GetSampleCultureAnonymousObjects() {
            return new[] {
                new { Date = new DateTime(1900, 1, 1, 0, 0, 0), FullWord = "Alpha" },
                new { Date = new DateTime(1900, 1, 1, 0, 0, 1), FullWord = "Bravo" },
                new { Date = new DateTime(1900, 1, 1, 0, 0, 2), FullWord = "Charlie" }
            };
        }
    }
}
