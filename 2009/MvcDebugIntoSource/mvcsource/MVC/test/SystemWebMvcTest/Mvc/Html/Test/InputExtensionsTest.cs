namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Globalization;
    using System.Web.Mvc.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InputExtensionsTest {

        [TestMethod]
        public void CheckBoxDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            string html = helper.CheckBox("baz", new { @checked = "checked", value = "false" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""baz"" name=""baz"" type=""checkbox"" value=""false"" />" +
                @"<input name=""baz"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxExplicitParametersOverrideDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = helper.CheckBox("foo", true /* isChecked */, new { @checked = "unchecked", value = "false" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""false"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxShouldNotCopyAttributesForHidden() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = helper.CheckBox("foo", true /* isChecked */, new { id = "myID" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""myID"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetCheckBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.CheckBox(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void CheckBoxWithInvalidBooleanThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetCheckBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectException<FormatException>(
                delegate {
                    helper.CheckBox("bar");
                },
                "String was not recognized as a valid Boolean.");
        }

        [TestMethod]
        public void CheckBoxCheckedWithOnlyName() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = helper.CheckBox("foo", true /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxShouldRespectModelStateAttemptedValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetCheckBoxViewData());
            helper.ViewData.ModelState.SetModelValue("foo", HtmlHelperTest.GetValueProviderResult("false", "false"));

            // Act
            string html = helper.CheckBox("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxWithOnlyName() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            string html = helper.CheckBox("foo");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxWithNameAndObjectAttribute() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            string html = helper.CheckBox("foo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxWithObjectAttribute() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = helper.CheckBox("foo", false /* isChecked */, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void CheckBoxWithAttributeDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = helper.CheckBox("foo", false /* isChecked */, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html);
        }

        [TestMethod]
        public void InputHelpersUseCurrentCultureToConvertValueParameter() {
            // Arrange
            DateTime dt = new DateTime(1900, 1, 1, 0, 0, 0);
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary { { "foo", dt } });

            var tests = new[] {
                // Hidden(name)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.Hidden("foo")) 
                },
                // Hidden(name, value)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.Hidden("foo", dt)) 
                },
                // Hidden(name, value, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.Hidden("foo", dt, null)) 
                },
                // Hidden(name, value, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.Hidden("foo", dt, new RouteValueDictionary())) 
                },

                // RadioButton(name, value)
                new { 
                    Html = @"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.RadioButton("foo", dt))
                },
                // RadioButton(name, value, isChecked)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.RadioButton("foo", dt, false))
                },
                // RadioButton(name, value, htmlAttributes)
                new { 
                    Html = @"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.RadioButton("foo", dt, null))
                },
                // RadioButton(name, value)
                new { 
                    Html = @"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.RadioButton("foo", dt, new RouteValueDictionary()))
                },
                // RadioButton(name, value, isChecked, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.RadioButton("foo", dt, false, null))
                },
                // RadioButton(name, value, isChecked, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.RadioButton("foo", dt, false, new RouteValueDictionary()))
                },

                // TextBox(name)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.TextBox("foo"))
                },
                // TextBox(name, value)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.TextBox("foo", dt))
                },
                // TextBox(name, value, hmtlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.TextBox("foo", dt, null))
                },
                // TextBox(name, value, hmtlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<string>(() => helper.TextBox("foo", dt, new RouteValueDictionary()))
                }
            };

            // Act && Assert
            using (HtmlHelperTest.ReplaceCulture("en-ZA", "en-US")) {
                foreach (var test in tests) {
                    Assert.AreEqual(test.Html, test.Action());
                }
            }
        }

        [TestMethod]
        public void HiddenWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Hidden(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void HiddenWithExplicitValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo", "DefaultFoo", null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithExplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo", "DefaultFoo", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithExplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo", "DefaultFoo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithExplicitValueNull() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo", (string)null /* value */, (object)null /* htmlAttributes */);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithImplicitValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithImplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithImplicitValueAndAttributesDictionaryReturnsEmptyValueIfNotFound() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("keyNotFound", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""keyNotFound"" name=""keyNotFound"" type=""hidden"" value="""" />", html);
        }

        [TestMethod]
        public void HiddenWithImplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithNameAndValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Hidden("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""fooValue"" />", html);
        }

        [TestMethod]
        public void HiddenWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Hidden(null /* name */);
                },
                "name");
        }

        [TestMethod]
        public void HiddenWithViewDataErrors() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewDataWithErrors());

            // Act
            string html = helper.Hidden("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""hidden"" value=""AttemptedValueFoo"" />", html);
        }

        [TestMethod]
        public void HiddenWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewDataWithErrors());

            // Act
            string html = helper.Hidden("foo", null, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""hidden"" value=""AttemptedValueFoo"" />", html);
        }

        [TestMethod]
        public void PasswordWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Password(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void PasswordDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", "Some Value", new { type = "fooType" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""fooType"" value=""Some Value"" />", html);
        }

        [TestMethod]
        public void PasswordExplicitParametersOverrideDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", "Some Value", new { value = "Another Value", name = "bar" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" value=""Some Value"" />", html);
        }

        [TestMethod]
        public void PasswordWithExplicitValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", "DefaultFoo", (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void PasswordWithExplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", "DefaultFoo", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""password"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void PasswordWithExplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", "DefaultFoo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""password"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void PasswordWithExplicitValueNull() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", (string)null /* value */, (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" />", html);
        }

        [TestMethod]
        public void PasswordWithImplicitValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" />", html);
        }

        [TestMethod]
        public void PasswordWithImplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""password"" />", html);
        }

        [TestMethod]
        public void PasswordWithImplicitValueAndAttributesDictionaryReturnsEmptyValueIfNotFound() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("keyNotFound", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""keyNotFound"" name=""keyNotFound"" type=""password"" />", html);
        }

        [TestMethod]
        public void PasswordWithImplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act
            string html = helper.Password("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""password"" />", html);
        }

        [TestMethod]
        public void PasswordWithNameAndValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.Password("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" value=""fooValue"" />", html);
        }

        [TestMethod]
        public void PasswordWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Password(null /* name */);
                },
                "name");
        }

        [TestMethod]
        public void PasswordWithViewDataErrors() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewDataWithErrors());

            // Act
            string html = helper.Password("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""password"" />", html);
        }

        [TestMethod]
        public void PasswordWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetPasswordViewDataWithErrors());

            // Act
            string html = helper.Password("foo", null, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""password"" />", html);
        }

        [TestMethod]
        public void RadioButtonDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("bar", "ViewDataBar", new { @checked = "chucked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input checked=""chucked"" id=""bar"" name=""bar"" type=""radio"" value=""ViewDataBar"" />", html);
        }

        [TestMethod]
        public void RadioButtonExplicitParametersOverrideDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("bar", "ViewDataBar", false, new { @checked = "checked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input id=""bar"" name=""bar"" type=""radio"" value=""ViewDataBar"" />", html);
        }

        [TestMethod]
        public void RadioButtonShouldRespectModelStateAttemptedValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());
            helper.ViewData.ModelState.SetModelValue("foo", HtmlHelperTest.GetValueProviderResult("ModelStateFoo", "ModelStateFoo"));

            // Act
            string html = helper.RadioButton("foo", "ModelStateFoo", false, new { @checked = "checked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ModelStateFoo"" />", html);
        }

        [TestMethod]
        public void RadioButtonValueParameterAlwaysRendered() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("foo", "ViewDataFoo");
            string html2 = helper.RadioButton("foo", "fooValue2");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ViewDataFoo"" />", html);
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""fooValue2"" />", html2);
        }

        [TestMethod]
        public void RadioButtonWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.RadioButton(String.Empty, null);
                },
                "name");
        }

        [TestMethod]
        public void RadioButtonWithNullValueThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    helper.RadioButton("foo", null);
                },
                "value");
        }

        [TestMethod]
        public void RadioButtonWithNameAndValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("foo", "ViewDataFoo");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithNameAndValueNotMatched() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""fooValue"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithNameValueUnchecked() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("bar", "barValue", false /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input id=""bar"" name=""bar"" type=""radio"" value=""barValue"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithNameValueChecked() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("bar", "barValue", true /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""bar"" name=""bar"" type=""radio"" value=""barValue"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithObjectAttribute() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("foo", "fooValue", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""radio"" value=""fooValue"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithAttributeDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("bar", "barValue", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""bar"" name=""bar"" type=""radio"" value=""barValue"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithValueUnchecked() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("foo", "bar", false /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""bar"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithValueAndObjectAttributeUnchecked() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("foo", "bar", false /* isChecked */, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""radio"" value=""bar"" />", html);
        }

        [TestMethod]
        public void RadioButtonWithValueAndAttributeDictionaryUnchecked() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            string html = helper.RadioButton("foo", "bar", false /* isChecked */, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""radio"" value=""bar"" />", html);
        }

        [TestMethod]
        public void TextBoxDictionaryOverridesImplicitValues() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", "DefaultFoo", new { type = "fooType" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""fooType"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxExplicitParametersOverrideDictionaryValues() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", "DefaultFoo", new { value = "Some other value" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithDotReplacementForId() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo.bar.baz", null);

            // Assert
            Assert.AreEqual(@"<input id=""foo_bar_baz"" name=""foo.bar.baz"" type=""text"" value="""" />", html);
        }

        [TestMethod]
        public void TextBoxWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.TextBox(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void TextBoxWithExplicitValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", "DefaultFoo", (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithExplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", "DefaultFoo", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithExplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", "DefaultFoo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithExplicitValueNull() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", (string)null /* value */, (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithImplicitValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithImplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithImplicitValueAndAttributesDictionaryReturnsEmptyValueIfNotFound() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("keyNotFound", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""keyNotFound"" name=""keyNotFound"" type=""text"" value="""" />", html);
        }

        [TestMethod]
        public void TextBoxWithImplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act
            string html = helper.TextBox("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.TextBox(null /* name */);
                },
                "name");
        }

        [TestMethod]
        public void TextBoxWithNameAndValue() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetHiddenViewData());

            // Act
            string html = helper.TextBox("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""fooValue"" />", html);
        }

        [TestMethod]
        public void TextBoxWithViewDataErrors() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewDataWithErrors());

            // Act
            string html = helper.TextBox("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""text"" value=""AttemptedValueFoo"" />", html);
        }

        [TestMethod]
        public void TextBoxWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper helper = HtmlHelperTest.GetHtmlHelper(GetTextBoxViewDataWithErrors());

            // Act
            string html = helper.TextBox("foo", null, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""text"" value=""AttemptedValueFoo"" />", html);
        }

        // CHECKBOX
        private static ViewDataDictionary GetCheckBoxViewData() {
            ViewDataDictionary viewData = new ViewDataDictionary { { "foo", true }, { "bar", "NotTrue" }, { "baz", false } };
            viewData.Model = new { foo = "ViewItemFoo", bar = "ViewItemBar", baz = "ViewItemBaz" };
            return viewData;
        }

        // HIDDEN
        private static ViewDataDictionary GetHiddenViewData() {
            return new ViewDataDictionary { { "foo", "ViewDataFoo" } };
        }

        private static ViewDataDictionary GetHiddenViewDataWithErrors() {
            ViewDataDictionary viewData = new ViewDataDictionary { { "foo", "ViewDataFoo" } };
            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = HtmlHelperTest.GetValueProviderResult("AttemptedValueFoo", "AttemptedValueFoo");

            return viewData;
        }

        // PASSWORD
        private static ViewDataDictionary GetPasswordViewData() {
            return new ViewDataDictionary { { "foo", "ViewDataFoo" } };
        }

        private static ViewDataDictionary GetPasswordViewDataWithErrors() {
            ViewDataDictionary viewData = new ViewDataDictionary { { "foo", "ViewDataFoo" } };
            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = HtmlHelperTest.GetValueProviderResult("AttemptedValueFoo", "AttemptedValueFoo");

            return viewData;
        }

        // RADIO
        private static ViewDataDictionary GetRadioButtonViewData() {
            ViewDataDictionary viewData = new ViewDataDictionary { { "foo", "ViewDataFoo" } };
            viewData.Model = new { foo = "ViewItemFoo", bar = "ViewItemBar" };
            ModelState modelState = new ModelState();
            modelState.Value = HtmlHelperTest.GetValueProviderResult("ViewDataFoo", "ViewDataFoo");
            viewData.ModelState["foo"] = modelState;

            return viewData;
        }

        // TEXTBOX
        private static readonly RouteValueDictionary _attributesDictionary = new RouteValueDictionary(new { baz = "BazValue" });
        private static readonly object _attributesObjectDictionary = new { baz = "BazObjValue" };

        private static ViewDataDictionary GetTextBoxViewData() {
            ViewDataDictionary viewData = new ViewDataDictionary { { "foo", "ViewDataFoo" } };
            viewData.Model = new { foo = "ViewItemFoo", bar = "ViewItemBar" };

            return viewData;
        }

        private static ViewDataDictionary GetTextBoxViewDataWithErrors() {
            ViewDataDictionary viewData = new ViewDataDictionary { { "foo", "ViewDataFoo" } };
            viewData.Model = new { foo = "ViewItemFoo", bar = "ViewItemBar" };
            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = HtmlHelperTest.GetValueProviderResult(new string[] { "AttemptedValueFoo" }, "AttemptedValueFoo");

            return viewData;
        }
    }
}
