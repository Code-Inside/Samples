namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class ReflectedEventPatternActionDescriptorTest {

        private readonly MethodInfo _setupMethod = typeof(ExecuteController).GetMethod("FooSetup");
        private readonly MethodInfo _completionMethod = typeof(ExecuteController).GetMethod("Foo");

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            string actionName = "SomeAction";
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act
            ReflectedEventPatternActionDescriptor ad = new ReflectedEventPatternActionDescriptor(_setupMethod, _completionMethod, actionName, cd);

            // Assert
            Assert.AreEqual(_setupMethod, ad.SetupMethod);
            Assert.AreEqual(_completionMethod, ad.CompletionMethod);
            Assert.AreEqual(actionName, ad.ActionName);
            Assert.AreEqual(cd, ad.ControllerDescriptor);
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsEmpty() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedEventPatternActionDescriptor(_setupMethod, _completionMethod, String.Empty, null);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedEventPatternActionDescriptor(_setupMethod, _completionMethod, null, null);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfCompletionMethodIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedEventPatternActionDescriptor(_setupMethod, null, null, null);
                }, "completionMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfControllerDescriptorIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedEventPatternActionDescriptor(_setupMethod, _completionMethod, "SomeAction", null);
                }, "controllerDescriptor");
        }

        [TestMethod]
        public void ConstructorThrowsIfSetupMethodIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedEventPatternActionDescriptor(null, null, null, null);
                }, "setupMethod");
        }

        [TestMethod]
        public void EndExecuteThrowsIfCalledPrematurely() {
            // Arrange
            ExecuteController controller = new ExecuteController();
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(controller);
            ControllerContext controllerContext = mockControllerContext.Object;

            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "id1", 42 }
            };

            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(_setupMethod, _completionMethod);

            // Act & assert
            controller.AsyncManager.OutstandingOperations.Increment();
            IAsyncResult asyncResult = ad.BeginExecute(controllerContext, parameters, null, null);
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ad.EndExecute(asyncResult);
                },
                @"EndExecute() was called prematurely.");
        }

        [TestMethod]
        public void Execute() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(new ExecuteController());
            ControllerContext controllerContext = mockControllerContext.Object;

            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "id1", 42 }
            };

            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(_setupMethod, _completionMethod);

            SignalContainer<object> resultContainer = new SignalContainer<object>();
            AsyncCallback callback = ar => {
                object o = ad.EndExecute(ar);
                resultContainer.Signal(o);
            };

            // Act
            ad.BeginExecute(controllerContext, parameters, callback, null);
            object retVal = resultContainer.Wait();

            // Assert
            Assert.AreEqual("Hello world: 42", retVal);
        }

        [TestMethod]
        public void ExecuteReplacesIncorrectNullableValuesWithNullOnActionMethod() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(new ExecuteController());
            ControllerContext controllerContext = mockControllerContext.Object;

            MethodInfo actionMethod = typeof(ExecuteController).GetMethod("FooWithException");
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "id1", 42 }
            };

            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(_setupMethod, actionMethod);

            SignalContainer<object> resultContainer = new SignalContainer<object>();
            AsyncCallback callback = ar => {
                object o = ad.EndExecute(ar);
                resultContainer.Signal(o);
            };

            // Act
            ad.BeginExecute(controllerContext, parameters, callback, null);
            object retVal = resultContainer.Wait();

            // Assert
            Assert.AreEqual("42", retVal);
        }

        [TestMethod]
        public void ExecuteReplacesIncorrectValueTypeValuesWithDefaultInstanceOnActionMethod() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(new ExecuteController());
            ControllerContext controllerContext = mockControllerContext.Object;

            MethodInfo actionMethod = typeof(ExecuteController).GetMethod("FooWithBool");
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "id1", 42 }
            };

            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(_setupMethod, actionMethod);

            SignalContainer<object> resultContainer = new SignalContainer<object>();
            AsyncCallback callback = ar => {
                object o = ad.EndExecute(ar);
                resultContainer.Signal(o);
            };

            // Act
            ad.BeginExecute(controllerContext, parameters, callback, null);
            object retVal = resultContainer.Wait();

            // Assert
            Assert.AreEqual("False42", retVal);
        }

        [TestMethod]
        public void ExecuteThrowsIfCalledSynchronously() {
            // Arrange
            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(null, null);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ad.Execute(null, null);
                },
                @"The action 'someName' cannot be called synchronously.");
        }

        [TestMethod]
        public void ExecuteThrowsIfControllerContextIsNull() {
            // Arrange
            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(null, null);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.BeginExecute(null, null, null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void ExecuteThrowsIfParametersIsNull() {
            // Arrange
            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(null, null);
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.BeginExecute(controllerContext, null, null, null);
                }, "parameters");
        }

        [TestMethod]
        public void GetCustomAttributes() {
            // Arrange
            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(_setupMethod, _completionMethod);

            // Act
            object[] attributes = ad.GetCustomAttributes(true /* inherit */);

            // Assert
            Assert.AreEqual(1, attributes.Length);
            Assert.IsInstanceOfType(attributes[0], typeof(AuthorizeAttribute));
        }

        [TestMethod]
        public void GetCustomAttributesFilterByType() {
            // Should not match attributes on the EndFoo() method, only the BeginFoo() method

            // Arrange
            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(_setupMethod, _completionMethod);

            // Act
            object[] attributes = ad.GetCustomAttributes(typeof(OutputCacheAttribute), true /* inherit */);

            // Assert
            Assert.AreEqual(0, attributes.Length);
        }

        [TestMethod]
        public void GetFilters() {
            // Arrange
            ControllerBase controller = new GetMemberChainSubderivedController();
            ControllerContext context = new Mock<ControllerContext>().Object;
            MethodInfo methodInfo = typeof(GetMemberChainSubderivedController).GetMethod("SomeVirtual");
            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(methodInfo, null /* completionMethod */);

            // Act
            FilterInfo filters = ad.GetFilters();

            // Assert
            Assert.AreEqual(2, filters.AuthorizationFilters.Count, "Wrong number of authorization filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.AuthorizationFilters[0]).Key);
            Assert.AreEqual("BaseMethod", ((KeyedFilterAttribute)filters.AuthorizationFilters[1]).Key);

            Assert.AreEqual(5, filters.ActionFilters.Count, "Wrong number of action filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.ActionFilters[0]).Key);
            Assert.AreEqual("BaseMethod", ((KeyedFilterAttribute)filters.ActionFilters[1]).Key);
            Assert.AreEqual("DerivedClass", ((KeyedFilterAttribute)filters.ActionFilters[2]).Key);
            Assert.AreEqual("SubderivedClass", ((KeyedFilterAttribute)filters.ActionFilters[3]).Key);
            Assert.AreEqual("SubderivedMethod", ((KeyedFilterAttribute)filters.ActionFilters[4]).Key);

            Assert.AreEqual(5, filters.ResultFilters.Count, "Wrong number of result filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.ResultFilters[0]).Key);
            Assert.AreEqual("BaseMethod", ((KeyedFilterAttribute)filters.ResultFilters[1]).Key);
            Assert.AreEqual("DerivedClass", ((KeyedFilterAttribute)filters.ResultFilters[2]).Key);
            Assert.AreEqual("SubderivedClass", ((KeyedFilterAttribute)filters.ResultFilters[3]).Key);
            Assert.AreEqual("SubderivedMethod", ((KeyedFilterAttribute)filters.ResultFilters[4]).Key);

            Assert.AreEqual(1, filters.ExceptionFilters.Count, "Wrong number of exception filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.ExceptionFilters[0]).Key);
        }

        [TestMethod]
        public void GetFiltersUsesMethodReflectedTypeRatherThanMethodDeclaringType() {
            // DevDiv 208062: Action filters specified on derived class won't run if the action method is on a base class

            // Arrange
            ControllerBase controller = new GetMemberChainDerivedController();
            ControllerContext context = new Mock<ControllerContext>().Object;
            MethodInfo methodInfo = typeof(GetMemberChainDerivedController).GetMethod("SomeVirtual");
            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(methodInfo, null /* completionMethod */);

            // Act
            FilterInfo filters = ad.GetFilters();

            // Assert
            Assert.AreEqual(2, filters.AuthorizationFilters.Count, "Wrong number of authorization filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.AuthorizationFilters[0]).Key);
            Assert.AreEqual("BaseMethod", ((KeyedFilterAttribute)filters.AuthorizationFilters[1]).Key);

            Assert.AreEqual(3, filters.ActionFilters.Count, "Wrong number of action filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.ActionFilters[0]).Key);
            Assert.AreEqual("BaseMethod", ((KeyedFilterAttribute)filters.ActionFilters[1]).Key);
            Assert.AreEqual("DerivedClass", ((KeyedFilterAttribute)filters.ActionFilters[2]).Key);

            Assert.AreEqual(3, filters.ResultFilters.Count, "Wrong number of result filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.ResultFilters[0]).Key);
            Assert.AreEqual("BaseMethod", ((KeyedFilterAttribute)filters.ResultFilters[1]).Key);
            Assert.AreEqual("DerivedClass", ((KeyedFilterAttribute)filters.ResultFilters[2]).Key);

            Assert.AreEqual(1, filters.ExceptionFilters.Count, "Wrong number of exception filters.");
            Assert.AreEqual("BaseClass", ((KeyedFilterAttribute)filters.ExceptionFilters[0]).Key);
        }

        [TestMethod]
        public void GetParametersWrapsParameterInfos() {
            // Arrange
            ParameterInfo pInfo = _setupMethod.GetParameters()[0];
            ReflectedEventPatternActionDescriptor ad = new ReflectedEventPatternActionDescriptor(_setupMethod, _completionMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

            // Act
            ParameterDescriptor[] pDescsFirstCall = ad.GetParameters();
            ParameterDescriptor[] pDescsSecondCall = ad.GetParameters();

            // Assert
            Assert.AreNotSame(pDescsFirstCall, pDescsSecondCall, "GetParameters() should return a new array on each invocation.");
            Assert.IsTrue(pDescsFirstCall.SequenceEqual(pDescsSecondCall), "Array elements were not equal.");
            Assert.AreEqual(1, pDescsFirstCall.Length);

            ReflectedParameterDescriptor pDesc = pDescsFirstCall[0] as ReflectedParameterDescriptor;

            Assert.IsNotNull(pDesc, "Parameter 0 should have been of type ReflectedParameterDescriptor.");
            Assert.AreSame(ad, pDesc.ActionDescriptor, "Parameter 0 Action did not match.");
            Assert.AreSame(pInfo, pDesc.ParameterInfo, "Parameter 0 ParameterInfo did not match.");
        }

        [TestMethod]
        public void GetSelectorsWrapsSelectorAttributes() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Mock<MethodInfo> mockMethod = new Mock<MethodInfo>();

            Mock<ActionMethodSelectorAttribute> mockAttr = new Mock<ActionMethodSelectorAttribute>();
            mockAttr.Expect(attr => attr.IsValidForRequest(controllerContext, mockMethod.Object)).Returns(true).Verifiable();
            mockMethod.Expect(m => m.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true)).Returns(new ActionMethodSelectorAttribute[] { mockAttr.Object });

            ReflectedEventPatternActionDescriptor ad = GetActionDescriptor(mockMethod.Object, null /* completionMethod */);

            // Act
            ICollection<ActionSelector> selectors = ad.GetSelectors();
            bool executedSuccessfully = selectors.All(s => s(controllerContext));

            // Assert
            Assert.AreEqual(1, selectors.Count);
            Assert.IsTrue(executedSuccessfully);
            mockAttr.Verify();
        }

        [TestMethod]
        public void IsDefined() {
            // Arrange
            ReflectedEventPatternActionDescriptor ad = new ReflectedEventPatternActionDescriptor(_setupMethod, _completionMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

            // Act
            bool isDefined = ad.IsDefined(typeof(AuthorizeAttribute), true /* inherit */);

            // Assert
            Assert.IsTrue(isDefined);
        }

        private static ReflectedEventPatternActionDescriptor GetActionDescriptor(MethodInfo setupMethod, MethodInfo actionMethod) {
            return new ReflectedEventPatternActionDescriptor(setupMethod, actionMethod, "someName", new Mock<ControllerDescriptor>().Object, false /* validateMethod */) {
                DispatcherCache = new MethodDispatcherCache()
            };
        }

        private class ExecuteController : Controller, IAsyncManagerContainer {
            private readonly AsyncManager _asyncHelper = new AsyncManager();
            private Func<object, string> _func;

            public AsyncManager AsyncManager {
                get {
                    return _asyncHelper;
                }
            }

            [Authorize]
            public void FooSetup(int id1) {
                _func = o => Convert.ToString(o, CultureInfo.InvariantCulture) + id1.ToString(CultureInfo.InvariantCulture);
                AsyncManager.Parameters["id2"] = "Hello world: ";
            }
            [OutputCache]
            public string Foo(string id2) {
                return _func(id2);
            }
            public string FooWithBool(bool id2) {
                return _func(id2);
            }
            public string FooWithException(Exception id2) {
                return _func(id2);
            }
        }

    }
}
