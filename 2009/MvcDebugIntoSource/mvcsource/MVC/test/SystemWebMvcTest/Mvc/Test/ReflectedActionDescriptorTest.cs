namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ReflectedActionDescriptorTest {

        private static readonly MethodInfo _int32EqualsIntMethod = typeof(int).GetMethod("Equals", new Type[] { typeof(int) });

        [TestMethod]
        public void ConstructorSetsActionNameProperty() {
            // Arrange
            string name = "someName";

            // Act
            ReflectedActionDescriptor ad = new ReflectedActionDescriptor(new Mock<MethodInfo>().Object, "someName", new Mock<ControllerDescriptor>().Object, false /* validateMethod */);

            // Assert
            Assert.AreEqual(name, ad.ActionName);
        }

        [TestMethod]
        public void ConstructorSetsControllerDescriptorProperty() {
            // Arrange
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act
            ReflectedActionDescriptor ad = new ReflectedActionDescriptor(new Mock<MethodInfo>().Object, "someName", cd, false /* validateMethod */);

            // Assert
            Assert.AreSame(cd, ad.ControllerDescriptor);
        }

        [TestMethod]
        public void ConstructorSetsMethodInfoProperty() {
            // Arrange
            MethodInfo methodInfo = new Mock<MethodInfo>().Object;

            // Act
            ReflectedActionDescriptor ad = new ReflectedActionDescriptor(methodInfo, "someName", new Mock<ControllerDescriptor>().Object, false /* validateMethod */);

            // Assert
            Assert.AreSame(methodInfo, ad.MethodInfo);
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsEmpty() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedActionDescriptor(new Mock<MethodInfo>().Object, "", new Mock<ControllerDescriptor>().Object);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfActionNameIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedActionDescriptor(new Mock<MethodInfo>().Object, null, new Mock<ControllerDescriptor>().Object);
                }, "actionName");
        }

        [TestMethod]
        public void ConstructorThrowsIfControllerDescriptorIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedActionDescriptor(new Mock<MethodInfo>().Object, "someName", null);
                }, "controllerDescriptor");
        }

        [TestMethod]
        public void ConstructorThrowsIfMethodInfoHasRefParameters() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("MethodHasRefParameter");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedActionDescriptor(methodInfo, "someName", new Mock<ControllerDescriptor>().Object);
                },
                @"Cannot call action method 'Void MethodHasRefParameter(Int32 ByRef)' on controller 'System.Web.Mvc.Test.ReflectedActionDescriptorTest+MyController' since the parameter 'Int32& i' is passed by reference.
Parameter name: methodInfo");
        }

        [TestMethod]
        public void ConstructorThrowsIfMethodInfoHasOutParameters() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("MethodHasOutParameter");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedActionDescriptor(methodInfo, "someName", new Mock<ControllerDescriptor>().Object);
                },
                @"Cannot call action method 'Void MethodHasOutParameter(Int32 ByRef)' on controller 'System.Web.Mvc.Test.ReflectedActionDescriptorTest+MyController' since the parameter 'Int32& i' is passed by reference.
Parameter name: methodInfo");
        }

        [TestMethod]
        public void ConstructorThrowsIfMethodInfoIsInstanceMethodOnNonControllerBaseType() {
            // Arrange
            MethodInfo methodInfo = typeof(string).GetMethod("Clone");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedActionDescriptor(methodInfo, "someName", new Mock<ControllerDescriptor>().Object);
                },
                @"Cannot create a descriptor for instance method 'System.Object Clone()' on type 'System.String' since the type does not subclass ControllerBase.
Parameter name: methodInfo");
        }

        [TestMethod]
        public void ConstructorThrowsIfMethodInfoIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedActionDescriptor(null, "someName", new Mock<ControllerDescriptor>().Object);
                }, "methodInfo");
        }

        [TestMethod]
        public void ConstructorThrowsIfMethodInfoIsOpenGenericType() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("OpenGenericMethod");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedActionDescriptor(methodInfo, "someName", new Mock<ControllerDescriptor>().Object);
                },
                @"Cannot call action method 'Void OpenGenericMethod[T]()' on controller 'System.Web.Mvc.Test.ReflectedActionDescriptorTest+MyController' since it is a generic method.
Parameter name: methodInfo");
        }

        [TestMethod]
        public void ExecuteCallsMethodInfoOnSuccess() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(new ConcatController());
            Dictionary<string, object> parameters = new Dictionary<string, object>(){
                { "a", "hello " },
                { "b", "world" }
            };

            ReflectedActionDescriptor ad = GetActionDescriptor(typeof(ConcatController).GetMethod("Concat"));

            // Act
            object result = ad.Execute(mockControllerContext.Object, parameters);

            // Assert
            Assert.AreEqual("hello world", result);
        }

        [TestMethod]
        public void ExecuteThrowsIfControllerContextIsNull() {
            // Arrange
            ReflectedActionDescriptor ad = GetActionDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.Execute(null, new Dictionary<string, object>());
                }, "controllerContext");
        }

        [TestMethod]
        public void ExecuteThrowsIfParametersContainsNullForNonNullableParameter() {
            // Arrange
            ReflectedActionDescriptor ad = GetActionDescriptor(_int32EqualsIntMethod);
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "obj", null } };

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ad.Execute(new Mock<ControllerContext>().Object, parameters);
                },
                @"The parameters dictionary contains a null entry for parameter 'obj' of non-nullable type 'System.Int32' for method 'Boolean Equals(Int32)' in 'System.Int32'. To make a parameter optional its type should be either a reference type or a Nullable type.
Parameter name: parameters");
        }

        [TestMethod]
        public void ExecuteThrowsIfParametersContainsValueOfWrongTypeForParameter() {
            // Arrange
            ReflectedActionDescriptor ad = GetActionDescriptor(_int32EqualsIntMethod);
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "obj", "notAnInteger" } };

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ad.Execute(new Mock<ControllerContext>().Object, parameters);
                },
                @"The parameters dictionary contains an invalid entry for parameter 'obj' for method 'Boolean Equals(Int32)' in 'System.Int32'. The dictionary contains a value of type 'System.String', but the parameter requires a value of type 'System.Int32'.
Parameter name: parameters");
        }

        [TestMethod]
        public void ExecuteThrowsIfParametersIsMissingAValue() {
            // Arrange
            ReflectedActionDescriptor ad = GetActionDescriptor(_int32EqualsIntMethod);
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ad.Execute(new Mock<ControllerContext>().Object, parameters);
                },
                @"The parameters dictionary does not contain an entry for parameter 'obj' of type 'System.Int32' for method 'Boolean Equals(Int32)' in 'System.Int32'. The dictionary must contain an entry for each parameter, even parameters with null values.
Parameter name: parameters");
        }

        [TestMethod]
        public void ExecuteThrowsIfParametersIsNull() {
            // Arrange
            ReflectedActionDescriptor ad = GetActionDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.Execute(new Mock<ControllerContext>().Object, null);
                }, "parameters");
        }

        [TestMethod]
        public void GetCustomAttributesCallsMethodInfoGetCustomAttributes() {
            // Arrange
            object[] expected = new object[0];
            Mock<MethodInfo> mockMethod = new Mock<MethodInfo>();
            mockMethod.Expect(mi => mi.GetCustomAttributes(true)).Returns(expected);
            ReflectedActionDescriptor ad = GetActionDescriptor(mockMethod.Object);

            // Act
            object[] returned = ad.GetCustomAttributes(true);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void GetCustomAttributesWithAttributeTypeCallsMethodInfoGetCustomAttributes() {
            // Arrange
            object[] expected = new object[0];
            Mock<MethodInfo> mockMethod = new Mock<MethodInfo>();
            mockMethod.Expect(mi => mi.GetCustomAttributes(typeof(ObsoleteAttribute), true)).Returns(expected);
            ReflectedActionDescriptor ad = GetActionDescriptor(mockMethod.Object);

            // Act
            object[] returned = ad.GetCustomAttributes(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void GetFilters() {
            // Arrange
            ControllerBase controller = new GetMemberChainSubderivedController();
            ControllerContext context = new Mock<ControllerContext>().Object;
            MethodInfo methodInfo = typeof(GetMemberChainSubderivedController).GetMethod("SomeVirtual");
            ReflectedActionDescriptor ad = GetActionDescriptor(methodInfo);

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
            ReflectedActionDescriptor ad = GetActionDescriptor(methodInfo);

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
            ParameterInfo pInfo0 = typeof(ConcatController).GetMethod("Concat").GetParameters()[0];
            ParameterInfo pInfo1 = typeof(ConcatController).GetMethod("Concat").GetParameters()[1];
            ReflectedActionDescriptor ad = GetActionDescriptor(typeof(ConcatController).GetMethod("Concat"));

            // Act
            ParameterDescriptor[] pDescsFirstCall = ad.GetParameters();
            ParameterDescriptor[] pDescsSecondCall = ad.GetParameters();

            // Assert
            Assert.AreNotSame(pDescsFirstCall, pDescsSecondCall, "GetParameters() should return a new array on each invocation.");
            Assert.IsTrue(pDescsFirstCall.SequenceEqual(pDescsSecondCall), "Array elements were not equal.");
            Assert.AreEqual(2, pDescsFirstCall.Length);

            ReflectedParameterDescriptor pDesc0 = pDescsFirstCall[0] as ReflectedParameterDescriptor;
            ReflectedParameterDescriptor pDesc1 = pDescsFirstCall[1] as ReflectedParameterDescriptor;

            Assert.IsNotNull(pDesc0, "Parameter 0 should have been of type ReflectedParameterDescriptor.");
            Assert.AreSame(ad, pDesc0.ActionDescriptor, "Parameter 0 Action did not match.");
            Assert.AreSame(pInfo0, pDesc0.ParameterInfo, "Parameter 0 ParameterInfo did not match.");
            Assert.IsNotNull(pDesc1, "Parameter 1 should have been of type ReflectedParameterDescriptor.");
            Assert.AreSame(ad, pDesc1.ActionDescriptor, "Parameter 1 Action did not match.");
            Assert.AreSame(pInfo1, pDesc1.ParameterInfo, "Parameter 1 ParameterInfo did not match.");
        }

        [TestMethod]
        public void GetSelectorsWrapsSelectorAttributes() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Mock<MethodInfo> mockMethod = new Mock<MethodInfo>();

            Mock<ActionMethodSelectorAttribute> mockAttr = new Mock<ActionMethodSelectorAttribute>();
            mockAttr.Expect(attr => attr.IsValidForRequest(controllerContext, mockMethod.Object)).Returns(true).Verifiable();
            mockMethod.Expect(m => m.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true)).Returns(new ActionMethodSelectorAttribute[] { mockAttr.Object });

            ReflectedActionDescriptor ad = GetActionDescriptor(mockMethod.Object);

            // Act
            ICollection<ActionSelector> selectors = ad.GetSelectors();
            bool executedSuccessfully = selectors.All(s => s(controllerContext));

            // Assert
            Assert.AreEqual(1, selectors.Count);
            Assert.IsTrue(executedSuccessfully);
            mockAttr.Verify();
        }

        [TestMethod]
        public void IsDefinedCallsMethodInfoIsDefined() {
            // Arrange
            Mock<MethodInfo> mockMethod = new Mock<MethodInfo>();
            mockMethod.Expect(mi => mi.IsDefined(typeof(ObsoleteAttribute), true)).Returns(true);
            ReflectedActionDescriptor ad = GetActionDescriptor(mockMethod.Object);

            // Act
            bool isDefined = ad.IsDefined(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.IsTrue(isDefined);
        }

        [TestMethod]
        public void TryCreateDescriptorReturnsDescriptorOnSuccess() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("StaticMethod");
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act
            ReflectedActionDescriptor ad = ReflectedActionDescriptor.TryCreateDescriptor(methodInfo, "someName", cd);

            // Assert
            Assert.IsNotNull(ad, "Descriptor should have been created successfully.");
            Assert.AreSame(methodInfo, ad.MethodInfo);
            Assert.AreEqual("someName", ad.ActionName);
            Assert.AreSame(cd, ad.ControllerDescriptor);
        }

        [TestMethod]
        public void TryCreateDescriptorReturnsNullOnFailure() {
            // Arrange
            MethodInfo methodInfo = typeof(MyController).GetMethod("OpenGenericMethod");
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act
            ReflectedActionDescriptor ad = ReflectedActionDescriptor.TryCreateDescriptor(methodInfo, "someName", cd);

            // Assert
            Assert.IsNull(ad, "Descriptor should not have been returned.");
        }

        private static ReflectedActionDescriptor GetActionDescriptor() {
            return GetActionDescriptor(new Mock<MethodInfo>().Object);
        }

        private static ReflectedActionDescriptor GetActionDescriptor(MethodInfo methodInfo) {
            return new ReflectedActionDescriptor(methodInfo, "someName", new Mock<ControllerDescriptor>().Object, false /* validateMethod */) {
                DispatcherCache = new ActionMethodDispatcherCache()
            };
        }

        private class ConcatController : Controller {

            public string Concat(string a, string b) {
                return a + b;
            }

        }

        [KeyedActionFilter(Key = "BaseClass", Order = 0)]
        [KeyedAuthorizationFilter(Key = "BaseClass", Order = 0)]
        [KeyedExceptionFilter(Key = "BaseClass", Order = 0)]
        private class GetMemberChainController : Controller {

            [KeyedActionFilter(Key = "BaseMethod", Order = 0)]
            [KeyedAuthorizationFilter(Key = "BaseMethod", Order = 0)]
            public virtual void SomeVirtual() {
            }

        }

        [KeyedActionFilter(Key = "DerivedClass", Order = 1)]
        private class GetMemberChainDerivedController : GetMemberChainController {

        }

        [KeyedActionFilter(Key = "SubderivedClass", Order = 2)]
        private class GetMemberChainSubderivedController : GetMemberChainDerivedController {

            [KeyedActionFilter(Key = "SubderivedMethod", Order = 2)]
            public override void SomeVirtual() {
            }

        }

        private abstract class KeyedFilterAttribute : FilterAttribute {
            public string Key {
                get;
                set;
            }
        }

        private class KeyedAuthorizationFilterAttribute : KeyedFilterAttribute, IAuthorizationFilter {
            public void OnAuthorization(AuthorizationContext filterContext) {
                throw new NotImplementedException();
            }
        }

        private class KeyedExceptionFilterAttribute : KeyedFilterAttribute, IExceptionFilter {
            public void OnException(ExceptionContext filterContext) {
                throw new NotImplementedException();
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
        private class KeyedActionFilterAttribute : KeyedFilterAttribute, IActionFilter, IResultFilter {
            public void OnActionExecuting(ActionExecutingContext filterContext) {
                throw new NotImplementedException();
            }
            public void OnActionExecuted(ActionExecutedContext filterContext) {
                throw new NotImplementedException();
            }
            public void OnResultExecuting(ResultExecutingContext filterContext) {
                throw new NotImplementedException();
            }

            public void OnResultExecuted(ResultExecutedContext filterContext) {
                throw new NotImplementedException();
            }
        }

        private class MyController : Controller {

            public static void StaticMethod() {
            }

            public void OpenGenericMethod<T>() {
            }

            public void MethodHasOutParameter(out int i) {
                i = 0;
            }

            public void MethodHasRefParameter(ref int i) {
            }

        }

    }
}
