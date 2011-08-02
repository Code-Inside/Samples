namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc.Internal;

    [TestClass]
    public class ExpressionHelperTest {
        [TestMethod]
        public void BuildRouteValueDictionaryWithNullExpressionThrowsArgumentNullException() {
            ExceptionHelper.ExpectArgumentNullException(
                () => ExpressionHelper.GetRouteValuesFromExpression<TestController>(null),
                "action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryWithNonMethodExpressionThrowsInvalidOperationException() {
            // Arrange
            Expression<Action<TestController>> expression = c => new TestController();

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => ExpressionHelper.GetRouteValuesFromExpression<TestController>(expression),
                "Expression must be a method call." + Environment.NewLine + "Parameter name: action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryWithoutControllerSuffixThrowsInvalidOperationException() {
            // Arrange
            Expression<Action<TestControllerNot>> index = (c => c.Index(123));

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => ExpressionHelper.GetRouteValuesFromExpression(index),
                "Controller name must end in 'Controller'." + Environment.NewLine + "Parameter name: action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryWithControllerBaseClassThrowsInvalidOperationException() {
            // Arrange
            Expression<Action<Controller>> index = (c => c.Dispose());

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => ExpressionHelper.GetRouteValuesFromExpression(index),
                "Cannot route to class named 'Controller'." + Environment.NewLine + "Parameter name: action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryAddsControllerNameToDictionary() {
            // Arrange
            Expression<Action<TestController>> index = (c => c.Index(123));

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(index);

            // Assert
            Assert.AreEqual("Test", rvd["Controller"]);
        }

        [TestMethod]
        public void BuildRouteValueDictionaryFromExpressionReturnsCorrectDictionary() {
            // Arrange
            Expression<Action<TestController>> index = (c => c.Index(123));

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(index);

            // Assert
            Assert.AreEqual("Test", rvd["Controller"]);
            Assert.AreEqual("Index", rvd["Action"]);
            Assert.AreEqual(123, rvd["page"]);
        }

        [TestMethod]
        public void BuildRouteValueDictionaryFromNonConstantExpressionReturnsCorrectDictionary() {
            // Arrange
            Expression<Action<TestController>> index = (c => c.About(Foo));

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(index);

            // Assert
            Assert.AreEqual("Test", rvd["Controller"]);
            Assert.AreEqual("About", rvd["Action"]);
            Assert.AreEqual("FooValue", rvd["s"]);
        }

        [TestMethod]
        public void GetInputNameFromPropertyExpressionReturnsPropertyName() {
            // Arrange
            Expression<Func<TestModel, int>> expression = m => m.IntProperty;

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("IntProperty", name);
        }

        [TestMethod]
        public void GetInputNameFromPropertyWithMethodCallExpressionReturnsPropertyName() {
            // Arrange
            Expression<Func<TestModel, string>> expression = m => m.IntProperty.ToString();

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("IntProperty", name);
        }

        [TestMethod]
        public void GetInputNameFromPropertyWithTwoMethodCallExpressionReturnsPropertyName() {
            // Arrange
            Expression<Func<TestModel, string>> expression = m => m.IntProperty.ToString().ToUpper();

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("IntProperty", name);
        }

        [TestMethod]
        public void GetInputNameFromExpressionWithTwoPropertiesUsesWholeExpression() {
            // Arrange
            Expression<Func<TestModel, int>> expression = m => m.StringProperty.Length;

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("StringProperty.Length", name);
        }

        public class TestController : Controller {
            public ActionResult Index(int page) {
                return null;
            }

            public string About(string s) {
                return "The value is " + s;
            }
        }

        public string Foo {
            get {
                return "FooValue";
            }
        }

        public class TestControllerNot : Controller {
            public ActionResult Index(int page) {
                return null;
            }
        }

        public class TestModel {
            public int IntProperty {
                get;
                set;
            }
            public string StringProperty {
                get;
                set;
            }
        }
    }
}
