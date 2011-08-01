namespace System.Web.Mvc.Ajax.Test {
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AjaxOptionsTest {

        [TestMethod]
        public void InsertionModeProperty() {
            // Arrange
            AjaxOptions options = new AjaxOptions();

            // Act & Assert
            MemberHelper.TestEnumProperty(options, "InsertionMode", InsertionMode.Replace, false);
        }

        [TestMethod]
        public void InsertionModePropertyExceptionText() {
            // Arrange
            AjaxOptions options = new AjaxOptions();

            // Act & Assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    options.InsertionMode = (InsertionMode)4;
                },
                "value",
                "The value '4' is outside the valid range of the enumeration type 'System.Web.Mvc.Ajax.InsertionMode'.\r\nParameter name: value");
        }

        [TestMethod]
        public void HttpMethodProperty() {
            // Arrange
            AjaxOptions options = new AjaxOptions();

            // Act & Assert
            MemberHelper.TestStringProperty(options, "HttpMethod", String.Empty, false, true);
        }

        [TestMethod]
        public void OnBeginProperty() {
            // Arrange
            AjaxOptions options = new AjaxOptions();

            // Act & Assert
            MemberHelper.TestStringProperty(options, "OnBegin", String.Empty, false, true);
        }

        [TestMethod]
        public void OnFailureProperty() {
            // Arrange
            AjaxOptions options = new AjaxOptions();

            // Act & Assert
            MemberHelper.TestStringProperty(options, "OnFailure", String.Empty, false, true);
        }

        [TestMethod]
        public void OnSuccessProperty() {
            // Arrange
            AjaxOptions options = new AjaxOptions();

            // Act & Assert
            MemberHelper.TestStringProperty(options, "OnSuccess", String.Empty, false, true);
        }

        [TestMethod]
        public void ToJavascriptStringWithEmptyOptions() {
            string s = (new AjaxOptions()).ToJavascriptString();
            Assert.AreEqual("{ insertionMode: Sys.Mvc.InsertionMode.replace }", s);
        }

        [TestMethod]
        public void ToJavascriptString() {
            // Arrange
            AjaxOptions options = new AjaxOptions {
                InsertionMode = InsertionMode.InsertBefore,
                Confirm = "confirm",
                HttpMethod = "POST",
                LoadingElementId = "loadingElement",
                UpdateTargetId = "someId",
                Url = "http://someurl.com",
                OnBegin = "some_begin_function",
                OnComplete = "some_complete_function",
                OnFailure = "some_failure_function",
                OnSuccess = "some_success_function",
            };

            // Act
            string s = options.ToJavascriptString();

            // Assert
            Assert.AreEqual("{ insertionMode: Sys.Mvc.InsertionMode.insertBefore, " +
                            "confirm: 'confirm', " +
                            "httpMethod: 'POST', " +
                            "loadingElementId: 'loadingElement', " +
                            "updateTargetId: 'someId', " +
                            "url: 'http://someurl.com', " + 
                            "onBegin: Function.createDelegate(this, some_begin_function), " +
                            "onComplete: Function.createDelegate(this, some_complete_function), " +
                            "onFailure: Function.createDelegate(this, some_failure_function), " +
                            "onSuccess: Function.createDelegate(this, some_success_function) }", s);
        }

        [TestMethod]
        public void ToJavascriptStringEscapesQuotesCorrectly() {
            // Arrange
            AjaxOptions options = new AjaxOptions {
                InsertionMode = InsertionMode.InsertBefore,
                Confirm = @"""confirm""",
                HttpMethod = "POST",
                LoadingElementId = "loading'Element'",
                UpdateTargetId = "someId",
                Url = "http://someurl.com",
                OnBegin = "some_begin_function",
                OnComplete = "some_complete_function",
                OnFailure = "some_failure_function",
                OnSuccess = "some_success_function",
            };

            // Act
            string s = options.ToJavascriptString();

            // Assert
            Assert.AreEqual("{ insertionMode: Sys.Mvc.InsertionMode.insertBefore, " +
                            @"confirm: '""confirm""', " +
                            "httpMethod: 'POST', " +
                            @"loadingElementId: 'loading\'Element\'', " +
                            "updateTargetId: 'someId', " +
                            "url: 'http://someurl.com', " +
                            "onBegin: Function.createDelegate(this, some_begin_function), " +
                            "onComplete: Function.createDelegate(this, some_complete_function), " +
                            "onFailure: Function.createDelegate(this, some_failure_function), " +
                            "onSuccess: Function.createDelegate(this, some_success_function) }", s);
        }

        [TestMethod]
        public void ToDictionaryWithOnlyUpdateTargetId() {
            // Arrange
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "someId" };

            // Act
            string s = options.ToJavascriptString();

            // Assert
            Assert.AreEqual("{ insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'someId' }", s);
        }

        [TestMethod]
        public void ToDictionaryWithUpdateTargetIdAndExplicitInsertionMode() {
            // Arrange
            AjaxOptions options = new AjaxOptions { InsertionMode = InsertionMode.InsertAfter, UpdateTargetId = "someId" };

            // Act
            string s = options.ToJavascriptString();

            // Assert
            Assert.AreEqual("{ insertionMode: Sys.Mvc.InsertionMode.insertAfter, updateTargetId: 'someId' }", s);
        }

        [TestMethod]
        public void UpdateTargetIdProperty() {
            // Arrange
            AjaxOptions options = new AjaxOptions();

            // Act & Assert
            MemberHelper.TestStringProperty(options, "UpdateTargetId", String.Empty, false, true);
        }

    }
}
