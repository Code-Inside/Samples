namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class AsyncActionDescriptorTest {

        private static readonly MethodInfo _convertIntToStringMethod = typeof(Convert).GetMethod("ToString", new Type[] { typeof(int) });
        private static readonly MethodInfo _convertStringToIntMethod = typeof(Convert).GetMethod("ToInt32", new Type[] { typeof(string) });
        private static readonly ParameterInfo _integerParameter = _convertIntToStringMethod.GetParameters()[0];
        private static readonly ParameterInfo _stringParameter = _convertStringToIntMethod.GetParameters()[0];

        [TestMethod]
        public void ExecuteMethodWrapsAsyncExecuteMethods() {
            // Arrange
            IAsyncResult asyncResult = new MockAsyncResult();
            object expectedValue = new object();
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            Mock<AsyncActionDescriptor> mockDescriptor = new Mock<AsyncActionDescriptor>() { CallBase = true };
            mockDescriptor.Expect(d => d.BeginExecute(controllerContext, parameters, null, null)).Returns(asyncResult);
            mockDescriptor.Expect(d => d.EndExecute(asyncResult)).Returns(expectedValue);
            ActionDescriptor descriptor = mockDescriptor.Object;

            // Act
            object returnedValue = descriptor.Execute(controllerContext, parameters);

            // Assert
            Assert.AreEqual(expectedValue, returnedValue);
        }

        [TestMethod]
        public void ExtractParameterFromDictionaryReturnsNullIfParameterIsNullAndNullableType() {
            // Arrange
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "value" , null }
            };

            // Act
            object returned = AsyncActionDescriptor.ExtractParameterFromDictionary(_stringParameter, parameters, _convertStringToIntMethod);

            // Assert
            Assert.IsNull(returned);
        }

        [TestMethod]
        public void ExtractParameterFromDictionaryReturnsParameterIfValid() {
            // Arrange
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "value" , 42 }
            };

            // Act
            object returned = AsyncActionDescriptor.ExtractParameterFromDictionary(_integerParameter, parameters, _convertIntToStringMethod);

            // Assert
            Assert.AreEqual(42, returned);
        }

        [TestMethod]
        public void ExtractParameterFromDictionaryThrowsIfParameterIsMissing() {
            // Arrange
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncActionDescriptor.ExtractParameterFromDictionary(_integerParameter, parameters, _convertIntToStringMethod);
                }, @"The parameters dictionary does not contain an entry for parameter 'value' of type 'System.Int32' for method 'System.String ToString(Int32)' in 'System.Convert'. The dictionary must contain an entry for each parameter, even parameters with null values.
Parameter name: parameters");
        }

        [TestMethod]
        public void ExtractParameterFromDictionaryThrowsIfParameterIsWrongType() {
            // Arrange
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "value" , 42 }
            };

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncActionDescriptor.ExtractParameterFromDictionary(_stringParameter, parameters, _convertStringToIntMethod);
                }, @"The parameters dictionary contains an invalid entry for parameter 'value' for method 'Int32 ToInt32(System.String)' in 'System.Convert'. The dictionary contains a value of type 'System.Int32', but the parameter requires a value of type 'System.String'.
Parameter name: parameters");
        }

        [TestMethod]
        public void ExtractParameterFromDictionaryThrowsIfRequiredParameterIsNull() {
            // Arrange
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "value" , null }
            };

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncActionDescriptor.ExtractParameterFromDictionary(_integerParameter, parameters, _convertIntToStringMethod);
                }, @"The parameters dictionary contains a null entry for parameter 'value' of non-nullable type 'System.Int32' for method 'System.String ToString(Int32)' in 'System.Convert'. To make a parameter optional its type should be either a reference type or a Nullable type.
Parameter name: parameters");
        }

        [TestMethod]
        public void GetActionMethodErrorMessageForInstanceMethodOnWrongTargetType() {
            // Arrange
            MethodInfo methodInfo = typeof(object).GetMethod("ToString");

            // Act
            string errorMessage = AsyncActionDescriptor.GetActionMethodErrorMessage(methodInfo);

            // Assert
            Assert.AreEqual(@"Cannot create a descriptor for instance method 'System.String ToString()' on type 'System.Object' since the type does not subclass ControllerBase.", errorMessage);
        }

        [TestMethod]
        public void GetActionMethodErrorMessageForInstanceMethodWithOpenGenericParameter() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("ActionWithOpenGenericParameter");

            // Act
            string errorMessage = AsyncActionDescriptor.GetActionMethodErrorMessage(methodInfo);

            // Assert
            Assert.AreEqual(@"Cannot call action method 'Void ActionWithOpenGenericParameter[T]()' on controller 'Microsoft.Web.Mvc.Test.AsyncActionDescriptorTest+MyController' since it is a generic method.", errorMessage);
        }

        [TestMethod]
        public void GetActionMethodErrorMessageForMethodWithOutParameter() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("ActionWithOutParameter");

            // Act
            string errorMessage = AsyncActionDescriptor.GetActionMethodErrorMessage(methodInfo);

            // Assert
            Assert.AreEqual(@"Cannot call action method 'Void ActionWithOutParameter(System.Object ByRef)' on controller 'Microsoft.Web.Mvc.Test.AsyncActionDescriptorTest+MyController' since the parameter 'System.Object& id' is passed by reference.", errorMessage);
        }

        [TestMethod]
        public void GetActionMethodErrorMessageForMethodWithRefParameter() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("ActionWithRefParameter");

            // Act
            string errorMessage = AsyncActionDescriptor.GetActionMethodErrorMessage(methodInfo);

            // Assert
            Assert.AreEqual(@"Cannot call action method 'Void ActionWithRefParameter(Int32 ByRef)' on controller 'Microsoft.Web.Mvc.Test.AsyncActionDescriptorTest+MyController' since the parameter 'Int32& id' is passed by reference.", errorMessage);
        }

        [TestMethod]
        public void GetActionMethodErrorMessageReturnsNullIfInstanceMethodIsValid() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("ValidAction");

            // Act
            string errorMessage = AsyncActionDescriptor.GetActionMethodErrorMessage(methodInfo);

            // Assert
            Assert.IsNull(errorMessage, "Error message should be null since method is valid.");
        }

        [TestMethod]
        public void GetActionMethodErrorMessageReturnsNullIfStaticMethodIsValid() {
            // Arrange
            MethodInfo methodInfo = typeof(object).GetMethod("ReferenceEquals");

            // Act
            string errorMessage = AsyncActionDescriptor.GetActionMethodErrorMessage(methodInfo);

            // Assert
            Assert.IsNull(errorMessage, "Error message should be null since method is valid.");
        }

        [TestMethod]
        public void GetAsyncManager() {
            // Arrange
            AsyncManager asyncHelper = new AsyncManager();

            Mock<Controller> mockController = new Mock<Controller>();
            mockController.As<IAsyncManagerContainer>().Expect(c => c.AsyncManager).Returns(asyncHelper);
            Controller controller = mockController.Object;

            // Act
            AsyncManager returned = AsyncActionDescriptor.GetAsyncManager(controller);

            // Assert
            Assert.AreEqual(asyncHelper, returned);
        }

        [TestMethod]
        public void GetAsyncManagerThrowsIfControllerIsNotAsyncManagerContainer() {
            // Arrange
            ControllerBase controller = new MyController();

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    AsyncActionDescriptor.GetAsyncManager(controller);
                },
                @"The controller of type 'Microsoft.Web.Mvc.Test.AsyncActionDescriptorTest+MyController' must subclass AsyncController or implement the IAsyncManagerContainer interface.");
        }

        private class MyController : Controller {
            public void ValidAction() {
            }
            public void ActionWithOpenGenericParameter<T>() {
            }
            public void ActionWithRefParameter(ref int id) {
            }
            public void ActionWithOutParameter(out object id) {
                id = null;
            }
        }

    }
}
