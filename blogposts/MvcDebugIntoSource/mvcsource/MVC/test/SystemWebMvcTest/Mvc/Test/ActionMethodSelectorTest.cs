namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionMethodSelectorTest {

        [TestMethod]
        public void AliasedMethodsProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);

            // Act
            ActionMethodSelector selector = new ActionMethodSelector(controllerType);

            // Assert
            Assert.AreEqual(2, selector.AliasedMethods.Length);

            List<MethodInfo> sortedAliasedMethods = selector.AliasedMethods.OrderBy(methodInfo => methodInfo.Name).ToList();
            Assert.AreEqual("Bar", sortedAliasedMethods[0].Name);
            Assert.AreEqual("FooRenamed", sortedAliasedMethods[1].Name);
        }

        [TestMethod]
        public void ControllerTypeProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            ActionMethodSelector selector = new ActionMethodSelector(controllerType);

            // Act & Assert
            Assert.AreSame(controllerType, selector.ControllerType);
        }

        [TestMethod]
        public void FindActionMethodReturnsMatchingMethodIfOneMethodMatches() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            ActionMethodSelector selector = new ActionMethodSelector(controllerType);

            // Act
            MethodInfo matchedMethod = selector.FindActionMethod(null, "OneMatch");

            // Assert
            Assert.AreEqual("OneMatch", matchedMethod.Name, "Method named OneMatch() should have matched.");
            Assert.AreEqual(typeof(string), matchedMethod.GetParameters()[0].ParameterType, "Method overload OneMatch(string) should have matched.");
        }

        [TestMethod]
        public void FindActionMethodReturnsMethodWithActionSelectionAttributeIfMultipleMethodsMatchRequest() {
            // DevDiv Bugs 212062: If multiple action methods match a request, we should match only the methods with an
            // [ActionMethod] attribute since we assume those methods are more specific.

            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            ActionMethodSelector selector = new ActionMethodSelector(controllerType);

            // Act
            MethodInfo matchedMethod = selector.FindActionMethod(null, "ShouldMatchMethodWithSelectionAttribute");

            // Assert
            Assert.AreEqual("MethodHasSelectionAttribute1", matchedMethod.Name);
        }

        [TestMethod]
        public void FindActionMethodReturnsNullIfNoMethodMatches() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            ActionMethodSelector selector = new ActionMethodSelector(controllerType);

            // Act
            MethodInfo matchedMethod = selector.FindActionMethod(null, "ZeroMatch");

            // Assert
            Assert.IsNull(matchedMethod, "No method should have matched.");
        }

        [TestMethod]
        public void FindActionMethodThrowsIfMultipleMethodsMatch() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            ActionMethodSelector selector = new ActionMethodSelector(controllerType);

            // Act & veriy
            ExceptionHelper.ExpectException<AmbiguousMatchException>(
                delegate {
                    selector.FindActionMethod(null, "TwoMatch");
                },
                @"The current request for action 'TwoMatch' on controller type 'SelectionAttributeController' is ambiguous between the following action methods:
Void TwoMatch2() on type System.Web.Mvc.Test.ActionMethodSelectorTest+SelectionAttributeController
Void TwoMatch() on type System.Web.Mvc.Test.ActionMethodSelectorTest+SelectionAttributeController");
        }

        [TestMethod]
        public void NonAliasedMethodsProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);

            // Act
            ActionMethodSelector selector = new ActionMethodSelector(controllerType);

            // Assert
            Assert.AreEqual(1, selector.NonAliasedMethods.Count);

            List<MethodInfo> sortedMethods = selector.NonAliasedMethods["foo"].OrderBy(methodInfo => methodInfo.GetParameters().Length).ToList();
            Assert.AreEqual("Foo", sortedMethods[0].Name);
            Assert.AreEqual(0, sortedMethods[0].GetParameters().Length);
            Assert.AreEqual("Foo", sortedMethods[1].Name);
            Assert.AreEqual(typeof(string), sortedMethods[1].GetParameters()[0].ParameterType);
        }

        private class MethodLocatorController : Controller {

            public void Foo() {
            }

            public void Foo(string s) {
            }

            [ActionName("Foo")]
            public void FooRenamed() {
            }

            [ActionName("Bar")]
            public void Bar() {
            }

            [ActionName("PrivateVoid")]
            private void PrivateVoid() {
            }


            protected void ProtectedVoidAction() {
            }

            public static void StaticMethod() {
            }

            // ensure that methods inheriting from Controller or a base class are not matched
            [ActionName("Blah")]
            protected override void ExecuteCore() {
                throw new NotImplementedException();
            }

            public string StringProperty {
                get;
                set;
            }

#pragma warning disable 0067
            public event EventHandler<EventArgs> SomeEvent;
#pragma warning restore 0067

        }

        private class SelectionAttributeController : Controller {

            [Match(false)]
            public void OneMatch() {
            }

            public void OneMatch(string s) {
            }

            public void TwoMatch() {
            }

            [ActionName("TwoMatch")]
            public void TwoMatch2() {
            }

            [Match(true), ActionName("ShouldMatchMethodWithSelectionAttribute")]
            public void MethodHasSelectionAttribute1() {
            }

            [ActionName("ShouldMatchMethodWithSelectionAttribute")]
            public void MethodDoesNotHaveSelectionAttribute1() {
            }

            private class MatchAttribute : ActionMethodSelectorAttribute {
                private bool _match;
                public MatchAttribute(bool match) {
                    _match = match;
                }
                public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
                    return _match;
                }
            }
        }

    }
}
