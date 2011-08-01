namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class AsyncActionMethodSelectorTest {

        [TestMethod]
        public void AliasedMethodsProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);

            // Act
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Assert
            Assert.AreEqual(3, selector.AliasedMethods.Length);

            List<MethodInfo> sortedAliasedMethods = selector.AliasedMethods.OrderBy(methodInfo => methodInfo.Name).ToList();
            Assert.AreEqual("Bar", sortedAliasedMethods[0].Name);
            Assert.AreEqual("BeginAsyncResultPatternWithRename2", sortedAliasedMethods[1].Name);
            Assert.AreEqual("FooRenamed", sortedAliasedMethods[2].Name);
        }

        [TestMethod]
        public void ControllerTypeProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & Assert
            Assert.AreSame(controllerType, selector.ControllerType);
        }

        [TestMethod]
        public void FindActionMethodDoesNotMatchBeginMethod() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "BeginAsyncResultPattern");

            // Assert
            Assert.IsNull(creator, "No method should have matched.");
        }

        [TestMethod]
        public void FindActionMethodDoesNotMatchCompletionMethod() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "EventPatternCompleted");

            // Assert
            Assert.IsNull(creator, "No method should have matched.");
        }

        [TestMethod]
        public void FindActionMethodDoesNotMatchEndMethod() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "EndAsyncResultPattern");

            // Assert
            Assert.IsNull(creator, "No method should have matched.");
        }

        [TestMethod]
        public void FindActionMethodReturnsMatchingMethodIfOneMethodMatches() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "OneMatch");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedActionDescriptor));
            ReflectedActionDescriptor castActionDescriptor = (ReflectedActionDescriptor)actionDescriptor;

            Assert.AreEqual("OneMatch", castActionDescriptor.MethodInfo.Name, "Method named OneMatch() should have matched.");
            Assert.AreEqual(typeof(string), castActionDescriptor.MethodInfo.GetParameters()[0].ParameterType, "Method overload OneMatch(string) should have matched.");
        }

        [TestMethod]
        public void FindActionMethodReturnsMethodWithActionSelectionAttributeIfMultipleMethodsMatchRequest() {
            // DevDiv Bugs 212062: If multiple action methods match a request, we should match only the methods with an
            // [ActionMethod] attribute since we assume those methods are more specific.

            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "ShouldMatchMethodWithSelectionAttribute");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedActionDescriptor));
            ReflectedActionDescriptor castActionDescriptor = (ReflectedActionDescriptor)actionDescriptor;

            Assert.AreEqual("MethodHasSelectionAttribute1", castActionDescriptor.MethodInfo.Name);
        }

        [TestMethod]
        public void FindActionMethodReturnsNullIfNoMethodMatches() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "ZeroMatch");

            // Assert
            Assert.IsNull(creator, "No method should have matched.");
        }

        [TestMethod]
        public void FindActionMethodThrowsIfMultipleMethodsMatch() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & veriy
            ExceptionHelper.ExpectException<AmbiguousMatchException>(
                delegate {
                    selector.FindActionMethod(null, "TwoMatch");
                },
                @"The current request for action 'TwoMatch' on controller type 'SelectionAttributeController' is ambiguous between the following action methods:
Void TwoMatch2() on type Microsoft.Web.Mvc.Test.AsyncActionMethodSelectorTest+SelectionAttributeController
Void TwoMatch() on type Microsoft.Web.Mvc.Test.AsyncActionMethodSelectorTest+SelectionAttributeController");
        }

        [TestMethod]
        public void FindActionMethodWithAsyncPatternType() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "AsyncResultPattern");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedAsyncPatternActionDescriptor));
            ReflectedAsyncPatternActionDescriptor castActionDescriptor = (ReflectedAsyncPatternActionDescriptor)actionDescriptor;
            
            Assert.AreEqual("BeginAsyncResultPattern", castActionDescriptor.BeginMethod.Name);
            Assert.AreEqual("EndAsyncResultPattern", castActionDescriptor.EndMethod.Name);
        }

        [TestMethod]
        public void FindActionMethodWithAsyncPatternTypeThrowsIfEndMethodIsAmbiguous() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & assert
            ExceptionHelper.ExpectException<AmbiguousMatchException>(
                delegate {
                    selector.FindActionMethod(null, "AsyncResultPatternWithConflict");
                },
                @"Lookup for method 'EndAsyncResultPatternWithRename2' on controller type 'MethodLocatorController' failed because of an ambiguity between the following methods:
Void EndAsyncResultPatternWithRename2(System.IAsyncResult) on type Microsoft.Web.Mvc.Test.AsyncActionMethodSelectorTest+MethodLocatorController
Void EndAsyncResultPatternWithRename2() on type Microsoft.Web.Mvc.Test.AsyncActionMethodSelectorTest+MethodLocatorController");
        }

        [TestMethod]
        public void FindActionMethodWithAsyncPatternTypeThrowsIfEndMethodNotFound() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    selector.FindActionMethod(null, "AsyncResultPatternEndNotFound");
                },
                @"Could not locate a method named 'EndAsyncResultPatternEndNotFound' on controller type 'Microsoft.Web.Mvc.Test.AsyncActionMethodSelectorTest+MethodLocatorController'.");
        }

        [TestMethod]
        public void FindActionMethodWithDelegatePatternType() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "DelegatePattern");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedDelegatePatternActionDescriptor));
            ReflectedDelegatePatternActionDescriptor castActionDescriptor = (ReflectedDelegatePatternActionDescriptor)actionDescriptor;

            Assert.AreEqual("DelegatePattern", castActionDescriptor.ActionMethod.Name);
        }

        [TestMethod]
        public void FindActionMethodWithEventPatternType() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindActionMethod(null, "EventPattern");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedEventPatternActionDescriptor));
            ReflectedEventPatternActionDescriptor castActionDescriptor = (ReflectedEventPatternActionDescriptor)actionDescriptor;

            Assert.AreEqual("EventPattern", castActionDescriptor.SetupMethod.Name);
            Assert.AreEqual("EventPatternCompleted", castActionDescriptor.CompletionMethod.Name);
        }

        [TestMethod]
        public void NonAliasedMethodsProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);

            // Act
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Assert
            Assert.AreEqual(5, selector.NonAliasedMethods.Count);

            List<MethodInfo> sortedMethods = selector.NonAliasedMethods["foo"].OrderBy(methodInfo => methodInfo.GetParameters().Length).ToList();
            Assert.AreEqual("Foo", sortedMethods[0].Name);
            Assert.AreEqual(0, sortedMethods[0].GetParameters().Length);
            Assert.AreEqual("Foo", sortedMethods[1].Name);
            Assert.AreEqual(typeof(string), sortedMethods[1].GetParameters()[0].ParameterType);

            Assert.AreEqual(1, selector.NonAliasedMethods["AsyncResultPattern"].Count());
            Assert.AreEqual("BeginAsyncResultPattern", selector.NonAliasedMethods["AsyncResultPattern"].First().Name);
            Assert.AreEqual(1, selector.NonAliasedMethods["AsyncResultPatternEndNotFound"].Count());
            Assert.AreEqual("BeginAsyncResultPatternEndNotFound", selector.NonAliasedMethods["AsyncResultPatternEndNotFound"].First().Name);
            Assert.AreEqual(1, selector.NonAliasedMethods["DelegatePattern"].Count());
            Assert.AreEqual(1, selector.NonAliasedMethods["EventPattern"].Count());
            Assert.AreEqual("EventPattern", selector.NonAliasedMethods["EventPattern"].First().Name);
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

            public IAsyncResult BeginAsyncResultPattern(AsyncCallback callback, object state) {
                throw new NotImplementedException();
            }

            public void EndAsyncResultPattern(IAsyncResult asyncResult) {
            }

            public IAsyncResult BeginAsyncResultPatternEndNotFound(AsyncCallback callback, object state) {
                throw new NotImplementedException();
            }

            [ActionName("AsyncResultPatternWithConflict")]
            public IAsyncResult BeginAsyncResultPatternWithRename2(AsyncCallback callback, object state) {
                throw new NotImplementedException();
            }

            public void EndAsyncResultPatternWithRename2(IAsyncResult asyncResult) {
            }

            public void EndAsyncResultPatternWithRename2() {
            }

            public void EventPattern() {
            }

            public void EventPatternCompleted() {
            }

            public Action DelegatePattern() {
                throw new NotImplementedException();
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
