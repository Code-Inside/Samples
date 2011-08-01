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
    public class ReflectedDelegatePatternActionDescriptorTest {

        private readonly MethodInfo _actionMethod = typeof(ExecuteController).GetMethod("Foo");

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            string actionName = "SomeAction";
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act
            ReflectedDelegatePatternActionDescriptor ad = new ReflectedDelegatePatternActionDescriptor(_actionMethod, actionName, cd);

            // Assert
            Assert.AreEqual(_actionMethod, ad.ActionMethod);
            Assert.AreEqual(actionName, ad.ActionName);
            Assert.AreEqual(cd, ad.ControllerDescriptor);
        }

        [TestMethod]
        public void ConstructorThrowsIfActionMethodDoesNotReturnParameterlessDelegateType() {
            // Arrange
            MethodInfo actionMethod = typeof(ExecuteController).GetMethod("MethodReturnTypeIsNotParameterlessDelegate");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedDelegatePatternActionDescriptor(actionMethod, null, null);
                },
                @"Method 'Void MethodReturnTypeIsNotParameterlessDelegate()' is not valid for this descriptor. The method must return a parameterless delegate type.
Parameter name: actionMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfActionMethodIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedDelegatePatternActionDescriptor(null, null, null);
                }, "actionMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsEmpty() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedDelegatePatternActionDescriptor(_actionMethod, String.Empty, null);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedDelegatePatternActionDescriptor(_actionMethod, null, null);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfControllerDescriptorIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedDelegatePatternActionDescriptor(_actionMethod, "SomeAction", null);
                }, "controllerDescriptor");
        }

        [TestMethod]
        public void EndExecuteThrowsIfCalledPrematurely() {
            // Arrange
            ExecuteController controller = new ExecuteController();
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(controller);
            ControllerContext controllerContext = mockControllerContext.Object;

            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "id", 42 }
            };

            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(_actionMethod);

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
                { "id", 42 }
            };

            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(_actionMethod);

            SignalContainer<object> resultContainer = new SignalContainer<object>();
            AsyncCallback callback = ar => {
                object o = ad.EndExecute(ar);
                resultContainer.Signal(o);
            };

            // Act
            ad.BeginExecute(controllerContext, parameters, callback, null);
            object retVal = resultContainer.Wait();

            // Assert
            Assert.AreEqual(84, retVal);
        }

        [TestMethod]
        public void ExecuteThrowsIfActionMethodReturnsNull() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(new ExecuteController());
            ControllerContext controllerContext = mockControllerContext.Object;

            MethodInfo actionMethod = typeof(ExecuteController).GetMethod("FooReturnsNull");
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(actionMethod);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ad.BeginExecute(controllerContext, parameters, null, null);
                },
                @"Method 'System.Func`1[System.Int32] FooReturnsNull()' returned null. The method must return a value.");
        }

        [TestMethod]
        public void ExecuteThrowsIfCalledSynchronously() {
            // Arrange
            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(null);

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
            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor( null);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.BeginExecute(null, null, null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void ExecuteThrowsIfParametersIsNull() {
            // Arrange
            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor( null);
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
            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(_actionMethod);

            // Act
            object[] attributes = ad.GetCustomAttributes(true /* inherit */);

            // Assert
            Assert.AreEqual(1, attributes.Length);
            Assert.IsInstanceOfType(attributes[0], typeof(AuthorizeAttribute));
        }

        [TestMethod]
        public void GetCustomAttributesFilterByType() {
            // Arrange
            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(_actionMethod);

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
            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(methodInfo);

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
            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(methodInfo);

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
            ParameterInfo pInfo = _actionMethod.GetParameters()[0];
            ReflectedDelegatePatternActionDescriptor ad = new ReflectedDelegatePatternActionDescriptor(_actionMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

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

            ReflectedDelegatePatternActionDescriptor ad = GetActionDescriptor(mockMethod.Object);

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
            ReflectedDelegatePatternActionDescriptor ad = new ReflectedDelegatePatternActionDescriptor(_actionMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

            // Act
            bool isDefined = ad.IsDefined(typeof(AuthorizeAttribute), true /* inherit */);

            // Assert
            Assert.IsTrue(isDefined);
        }

        private static ReflectedDelegatePatternActionDescriptor GetActionDescriptor(MethodInfo actionMethod) {
            return new ReflectedDelegatePatternActionDescriptor(actionMethod, "someName", new Mock<ControllerDescriptor>().Object, false /* validateMethod */) {
                DispatcherCache = new MethodDispatcherCache()
            };
        }

        private class ExecuteController : Controller, IAsyncManagerContainer {
            private readonly AsyncManager _asyncHelper = new AsyncManager();

            public AsyncManager AsyncManager {
                get {
                    return _asyncHelper;
                }
            }

            [Authorize]
            public Func<int> Foo(int id) {
                return () => id * 2;
            }
            public Func<int> FooReturnsNull() {
                return null;
            }
            public void MethodReturnTypeIsNotParameterlessDelegate() {
            }
        }

    }
}
