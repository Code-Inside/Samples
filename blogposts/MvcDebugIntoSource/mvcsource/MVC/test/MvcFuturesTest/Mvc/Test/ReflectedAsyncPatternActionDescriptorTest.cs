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
    public class ReflectedAsyncPatternActionDescriptorTest {

        private readonly MethodInfo _validBeginMethod = typeof(MyController).GetMethod("BeginFooValid");
        private readonly MethodInfo _validEndMethod = typeof(MyController).GetMethod("EndFooValid");

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            string actionName = "SomeAction";
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act
            ReflectedAsyncPatternActionDescriptor ad = new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, actionName, cd);

            // Assert
            Assert.AreEqual(_validBeginMethod, ad.BeginMethod);
            Assert.AreEqual(_validEndMethod, ad.EndMethod);
            Assert.AreEqual(actionName, ad.ActionName);
            Assert.AreEqual(cd, ad.ControllerDescriptor);
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsEmpty() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, String.Empty, null);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, null, null);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfBeginMethodHasIncorrectParameterCount() {
            // Arrange
            MethodInfo beginMethod = typeof(MyController).GetMethod("BeginFooTooFewParameters");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(beginMethod, null, null, null);
                }, @"Method 'System.IAsyncResult BeginFooTooFewParameters()' is not a valid begin method. A begin method must take an AsyncCallback and Object as its last two parameters, and it must return an IAsyncResult.
Parameter name: beginMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfBeginMethodHasIncorrectParameterTypes() {
            // Arrange
            MethodInfo beginMethod = typeof(MyController).GetMethod("BeginFooWrongParameters");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(beginMethod, null, null, null);
                }, @"Method 'System.IAsyncResult BeginFooWrongParameters(Int32, Int32)' is not a valid begin method. A begin method must take an AsyncCallback and Object as its last two parameters, and it must return an IAsyncResult.
Parameter name: beginMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfBeginMethodHasIncorrectReturnType() {
            // Arrange
            MethodInfo beginMethod = typeof(MyController).GetMethod("BeginFooWrongReturnType");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(beginMethod, null, null, null);
                }, @"Method 'Void BeginFooWrongReturnType(System.AsyncCallback, System.Object)' is not a valid begin method. A begin method must take an AsyncCallback and Object as its last two parameters, and it must return an IAsyncResult.
Parameter name: beginMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfBeginMethodIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(null, null, null, null);
                }, "beginMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfControllerDescriptorIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, "SomeAction", null);
                }, "controllerDescriptor");
        }

        [TestMethod]
        public void ConstructorThrowsIfEndMethodHasIncorrectParameterCount() {
            // Arrange
            MethodInfo endMethod = typeof(MyController).GetMethod("EndFooWrongParameterCount");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, endMethod, null, null);
                }, @"Method 'Void EndFooWrongParameterCount(Int32, Int32)' is not a valid end method. An end method must take an IAsyncResult as its only parameter.
Parameter name: endMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfEndMethodHasIncorrectParameterType() {
            // Arrange
            MethodInfo endMethod = typeof(MyController).GetMethod("EndFooWrongParameterType");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, endMethod, null, null);
                }, @"Method 'Void EndFooWrongParameterType(Int32)' is not a valid end method. An end method must take an IAsyncResult as its only parameter.
Parameter name: endMethod");
        }

        [TestMethod]
        public void ConstructorThrowsIfEndMethodIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, null, null, null);
                }, "endMethod");
        }

        [TestMethod]
        public void Execute() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(new ExecuteController());
            ControllerContext controllerContext = mockControllerContext.Object;

            MethodInfo beginMethod = typeof(ExecuteController).GetMethod("BeginMethod");
            MethodInfo endMethod = typeof(ExecuteController).GetMethod("EndMethod");
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "id", 42 }
            };

            ReflectedAsyncPatternActionDescriptor ad = GetActionDescriptor(beginMethod, endMethod);

            SignalContainer<object> resultContainer = new SignalContainer<object>();
            AsyncCallback callback = ar => {
                object o = ad.EndExecute(ar);
                resultContainer.Signal(o);
            };

            // Act
            ad.BeginExecute(controllerContext, parameters, callback, null);
            object retVal = resultContainer.Wait();

            // Assert
            Assert.AreEqual(42, retVal);
        }

        [TestMethod]
        public void ExecuteThrowsIfControllerContextIsNull() {
            // Arrange
            ReflectedAsyncPatternActionDescriptor ad = GetActionDescriptor(null, null);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.BeginExecute(null, null, null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void ExecuteThrowsIfParametersIsNull() {
            // Arrange
            ReflectedAsyncPatternActionDescriptor ad = GetActionDescriptor(null, null);
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
            ReflectedAsyncPatternActionDescriptor ad = new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

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
            ReflectedAsyncPatternActionDescriptor ad = new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

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
            ReflectedAsyncPatternActionDescriptor ad = GetActionDescriptor(methodInfo, null /* endMethod */);

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
            ReflectedAsyncPatternActionDescriptor ad = GetActionDescriptor(methodInfo, null /* endMethod */);

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
            ParameterInfo pInfo = typeof(MyController).GetMethod("BeginFooValid").GetParameters()[0];
            ReflectedAsyncPatternActionDescriptor ad = new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

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

            ReflectedAsyncPatternActionDescriptor ad = GetActionDescriptor(mockMethod.Object, null /* endMethod */);

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
            ReflectedAsyncPatternActionDescriptor ad = new ReflectedAsyncPatternActionDescriptor(_validBeginMethod, _validEndMethod, "SomeAction", new Mock<ControllerDescriptor>().Object);

            // Act
            bool isDefined = ad.IsDefined(typeof(AuthorizeAttribute), true /* inherit */);

            // Assert
            Assert.IsTrue(isDefined);
        }

        private static ReflectedAsyncPatternActionDescriptor GetActionDescriptor(MethodInfo beginMethod, MethodInfo endMethod) {
            return new ReflectedAsyncPatternActionDescriptor(beginMethod, endMethod, "someName", new Mock<ControllerDescriptor>().Object, false /* validateMethod */) {
                DispatcherCache = new MethodDispatcherCache()
            };
        }

        private class ExecuteController : Controller {
            private Func<int, int> _identityFunc = id => id;
            public IAsyncResult BeginMethod(int id, AsyncCallback callback, object state) {
                return _identityFunc.BeginInvoke(id, callback, state);
            }
            public int EndMethod(IAsyncResult asyncResult) {
                return _identityFunc.EndInvoke(asyncResult);
            }
        }

        private class MyController : Controller {
            public IAsyncResult BeginFooTooFewParameters() {
                throw new NotImplementedException();
            }
            public IAsyncResult BeginFooWrongParameters(int a, int b) {
                throw new NotImplementedException();
            }
            public void BeginFooWrongReturnType(AsyncCallback callback, object state) {
                throw new NotImplementedException();
            }
            [Authorize]
            public IAsyncResult BeginFooValid(int id, AsyncCallback callback, object state) {
                throw new NotImplementedException();
            }
            public void EndFooWrongParameterCount(int a, int b) {
                throw new NotImplementedException();
            }
            public void EndFooWrongParameterType(int a) {
                throw new NotImplementedException();
            }
            [OutputCache]
            public void EndFooValid(IAsyncResult asyncResult) {
                throw new NotImplementedException();
            }
        }

    }
}
