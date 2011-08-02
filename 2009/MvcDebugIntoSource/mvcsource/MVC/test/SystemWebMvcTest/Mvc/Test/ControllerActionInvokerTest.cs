namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [CLSCompliant(false)]
    public class ControllerActionInvokerTest {

        [TestMethod]
        public void CreateActionResultWithActionResultParameterReturnsParameterUnchanged() {
            // Arrange
            ControllerActionInvokerHelper invoker = new ControllerActionInvokerHelper();
            ActionResult originalResult = new JsonResult();

            // Act
            ActionResult returnedActionResult = invoker.PublicCreateActionResult(null, null, originalResult);

            // Assert
            Assert.AreSame(originalResult, returnedActionResult);
        }

        [TestMethod]
        public void CreateActionResultWithNullParameterReturnsEmptyResult() {
            // Arrange
            ControllerActionInvokerHelper invoker = new ControllerActionInvokerHelper();

            // Act
            ActionResult returnedActionResult = invoker.PublicCreateActionResult(null, null, null);

            // Assert
            Assert.IsInstanceOfType(returnedActionResult, typeof(EmptyResult));
        }

        [TestMethod]
        public void CreateActionResultWithObjectParameterReturnsContentResult() {
            // Arrange
            ControllerActionInvokerHelper invoker = new ControllerActionInvokerHelper();
            object originalReturnValue = new CultureReflector();

            // Act
            ActionResult returnedActionResult = invoker.PublicCreateActionResult(null, null, originalReturnValue);

            // Assert
            Assert.IsInstanceOfType(returnedActionResult, typeof(ContentResult));
            ContentResult contentResult = (ContentResult)returnedActionResult;
            Assert.AreEqual("IVL", contentResult.Content);
        }

        [TestMethod]
        public void FindAction() {
            // Arrange
            EmptyController controller = new EmptyController();
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ActionDescriptor expectedAd = new Mock<ActionDescriptor>().Object;
            Mock<ControllerDescriptor> mockCd = new Mock<ControllerDescriptor>();
            mockCd.Expect(cd => cd.FindAction(controllerContext, "someAction")).Returns(expectedAd);

            // Act
            ActionDescriptor returnedAd = helper.PublicFindAction(controllerContext, mockCd.Object, "someAction");

            // Assert
            Assert.AreEqual(expectedAd, returnedAd, "Returned descriptor was incorrect.");
        }

        [TestMethod]
        public void FindActionDoesNotMatchConstructor() {
            // FindActionMethod() shouldn't match special-named methods like type constructors.

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, ".ctor");
            ActionDescriptor ad2 = helper.PublicFindAction(context, cd, "FindMethodController");

            // Assert
            Assert.IsNull(ad);
            Assert.IsNull(ad2);
        }

        [TestMethod]
        public void FindActionDoesNotMatchEvent() {
            // FindActionMethod() should skip methods that aren't publicly visible.

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "add_Event");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionDoesNotMatchInternalMethod() {
            // FindActionMethod() should skip methods that aren't publicly visible.

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "InternalMethod");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionDoesNotMatchMethodsDefinedOnControllerType() {
            // FindActionMethod() shouldn't match methods originally defined on the Controller type, e.g. Dispose().

            // Arrange
            Controller controller = new BlankController();
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(BlankController));
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            var methods = typeof(Controller).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // Act & Assert
            foreach (var method in methods) {
                bool wasFound = true;
                try {
                    ActionDescriptor ad = helper.PublicFindAction(context, cd, method.Name);
                    wasFound = (ad != null);
                }
                finally {
                    Assert.IsFalse(wasFound, "FindAction() should return false for methods defined on the Controller class: " + method);
                }
            }
        }

        [TestMethod]
        public void FindActionDoesNotMatchMethodsDefinedOnObjectType() {
            // FindActionMethod() shouldn't match methods originally defined on the Object type, e.g. ToString().

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "ToString");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionDoesNotMatchNonActionMethod() {
            // FindActionMethod() should respect the [NonAction] attribute.

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "NonActionMethod");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionDoesNotMatchOverriddenNonActionMethod() {
            // FindActionMethod() should trace the method's inheritance chain looking for the [NonAction] attribute.

            // Arrange
            Controller controller = new DerivedFindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(DerivedFindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "InternalMethod");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionDoesNotMatchPrivateMethod() {
            // FindActionMethod() should skip methods that aren't publicly visible.

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "PrivateMethod");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionDoesNotMatchProperty() {
            // FindActionMethod() shouldn't match special-named methods like property getters.

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "get_Property");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionDoesNotMatchProtectedMethod() {
            // FindActionMethod() should skip methods that aren't publicly visible.

            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "ProtectedMethod");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionIsCaseInsensitive() {
            // Arrange
            Controller controller = new FindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(FindMethodController));
            MethodInfo expectedMethodInfo = typeof(FindMethodController).GetMethod("ValidActionMethod");

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad1 = helper.PublicFindAction(context, cd, "validactionmethod");
            ActionDescriptor ad2 = helper.PublicFindAction(context, cd, "VALIDACTIONMETHOD");

            // Assert
            Assert.IsInstanceOfType(ad1, typeof(ReflectedActionDescriptor));
            Assert.AreSame(expectedMethodInfo, ((ReflectedActionDescriptor)ad1).MethodInfo, "MethodInfo for descriptor 1 was wrong.");
            Assert.IsInstanceOfType(ad2, typeof(ReflectedActionDescriptor));
            Assert.AreSame(expectedMethodInfo, ((ReflectedActionDescriptor)ad2).MethodInfo, "MethodInfo for descriptor 2 was wrong.");
        }

        [TestMethod]
        public void FindActionMatchesActionMethodWithClosedGenerics() {
            // FindActionMethod() should work with generic methods as long as there are no open types.

            // Arrange
            Controller controller = new GenericFindMethodController<int>();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(GenericFindMethodController<int>));
            MethodInfo expectedMethodInfo = typeof(GenericFindMethodController<int>).GetMethod("ClosedGenericMethod");

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "ClosedGenericMethod");

            // Assert
            Assert.IsInstanceOfType(ad, typeof(ReflectedActionDescriptor));
            Assert.AreSame(expectedMethodInfo, ((ReflectedActionDescriptor)ad).MethodInfo, "MethodInfo was wrong.");
        }

        [TestMethod]
        public void FindActionMatchesNewActionMethodsHidingNonActionMethods() {
            // FindActionMethod() should stop looking for [NonAction] in the method's inheritance chain when it sees
            // that a method in a derived class hides the a method in the base class.

            // Arrange
            Controller controller = new DerivedFindMethodController();
            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new ReflectedControllerDescriptor(typeof(DerivedFindMethodController));
            MethodInfo expectedMethodInfo = typeof(DerivedFindMethodController).GetMethod("DerivedIsActionMethod");

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ActionDescriptor ad = helper.PublicFindAction(context, cd, "DerivedIsActionMethod");

            // Assert
            Assert.IsInstanceOfType(ad, typeof(ReflectedActionDescriptor));
            Assert.AreSame(expectedMethodInfo, ((ReflectedActionDescriptor)ad).MethodInfo, "MethodInfo was wrong.");
        }

        [TestMethod]
        public void GetControllerDescriptor() {
            // Arrange
            EmptyController controller = new EmptyController();
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ControllerDescriptor cd = helper.PublicGetControllerDescriptor(controllerContext);

            // Assert
            Assert.IsInstanceOfType(cd, typeof(ReflectedControllerDescriptor));
            Assert.AreEqual(typeof(EmptyController), cd.ControllerType);
        }

        [TestMethod]
        public void GetFiltersWhereControllerDoesNotImplementFilterInterfaces() {
            // Arrange
            IActionFilter actionFilter = new Mock<IActionFilter>().Object;
            IResultFilter resultFilter = new Mock<IResultFilter>().Object;
            IAuthorizationFilter authFilter = new Mock<IAuthorizationFilter>().Object;
            IExceptionFilter exFilter = new Mock<IExceptionFilter>().Object;

            FilterInfo filterInfo = new FilterInfo();
            filterInfo.ActionFilters.Add(actionFilter);
            filterInfo.AuthorizationFilters.Add(authFilter);
            filterInfo.ExceptionFilters.Add(exFilter);
            filterInfo.ResultFilters.Add(resultFilter);

            Mock<ActionDescriptor> mockAd = new Mock<ActionDescriptor>();
            mockAd.Expect(ad => ad.GetFilters()).Returns(filterInfo);

            ControllerBase controller = new Mock<ControllerBase>().Object;
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            FilterInfo returnedFilterInfo = helper.PublicGetFilters(context, mockAd.Object);

            // Assert
            Assert.AreEqual(1, returnedFilterInfo.ActionFilters.Count);
            Assert.AreEqual(actionFilter, returnedFilterInfo.ActionFilters[0]);

            Assert.AreEqual(1, returnedFilterInfo.AuthorizationFilters.Count);
            Assert.AreEqual(authFilter, returnedFilterInfo.AuthorizationFilters[0]);

            Assert.AreEqual(1, returnedFilterInfo.ExceptionFilters.Count);
            Assert.AreEqual(exFilter, returnedFilterInfo.ExceptionFilters[0]);

            Assert.AreEqual(1, returnedFilterInfo.ResultFilters.Count);
            Assert.AreEqual(resultFilter, returnedFilterInfo.ResultFilters[0]);
        }

        [TestMethod]
        public void GetFiltersWhereControllerImplementsFilterInterfaces() {
            // Arrange
            IActionFilter actionFilter = new Mock<IActionFilter>().Object;
            IResultFilter resultFilter = new Mock<IResultFilter>().Object;
            IAuthorizationFilter authFilter = new Mock<IAuthorizationFilter>().Object;
            IExceptionFilter exFilter = new Mock<IExceptionFilter>().Object;

            FilterInfo filterInfo = new FilterInfo();
            filterInfo.ActionFilters.Add(actionFilter);
            filterInfo.AuthorizationFilters.Add(authFilter);
            filterInfo.ExceptionFilters.Add(exFilter);
            filterInfo.ResultFilters.Add(resultFilter);

            Mock<ActionDescriptor> mockAd = new Mock<ActionDescriptor>();
            mockAd.Expect(ad => ad.GetFilters()).Returns(filterInfo);

            EmptyController controller = new EmptyController();
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            FilterInfo returnedFilterInfo = helper.PublicGetFilters(context, mockAd.Object);

            // Assert
            Assert.AreEqual(2, returnedFilterInfo.ActionFilters.Count);
            Assert.AreEqual(controller, returnedFilterInfo.ActionFilters[0]);
            Assert.AreEqual(actionFilter, returnedFilterInfo.ActionFilters[1]);

            Assert.AreEqual(2, returnedFilterInfo.AuthorizationFilters.Count);
            Assert.AreEqual(controller, returnedFilterInfo.AuthorizationFilters[0]);
            Assert.AreEqual(authFilter, returnedFilterInfo.AuthorizationFilters[1]);

            Assert.AreEqual(2, returnedFilterInfo.ExceptionFilters.Count);
            Assert.AreEqual(controller, returnedFilterInfo.ExceptionFilters[0]);
            Assert.AreEqual(exFilter, returnedFilterInfo.ExceptionFilters[1]);

            Assert.AreEqual(2, returnedFilterInfo.ResultFilters.Count);
            Assert.AreEqual(controller, returnedFilterInfo.ResultFilters[0]);
            Assert.AreEqual(resultFilter, returnedFilterInfo.ResultFilters[1]);
        }

        [TestMethod]
        public void GetParameterValueAllowsAllSubpropertiesIfBindAttributeNotSpecified() {
            // Arrange
            CustomConverterController controller = new CustomConverterController();
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithoutBindAttribute = typeof(CustomConverterController).GetMethod("ParameterWithoutBindAttribute").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(paramWithoutBindAttribute, new Mock<ActionDescriptor>().Object);

            // Act
            object valueWithoutBindAttribute = helper.PublicGetParameterValue(controllerContext, pd);

            // Assert
            Assert.AreEqual("foo=True&bar=True", valueWithoutBindAttribute);
        }

        [TestMethod]
        public void GetParameterValueResolvesConvertersInCorrectOrderOfPrecedence() {
            // Order of precedence:
            //   1. Attributes on the parameter itself
            //   2. Query the global converter provider

            // Arrange
            CustomConverterController controller = new CustomConverterController();
            Dictionary<string, object> values = new Dictionary<string, object> { { "foo", "fooValue" } };
            ControllerContext controllerContext = GetControllerContext(controller, values);
            controller.ControllerContext = controllerContext;
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithOneConverter = typeof(CustomConverterController).GetMethod("ParameterHasOneConverter").GetParameters()[0];
            ReflectedParameterDescriptor pdOneConverter = new ReflectedParameterDescriptor(paramWithOneConverter, new Mock<ActionDescriptor>().Object);
            ParameterInfo paramWithNoConverters = typeof(CustomConverterController).GetMethod("ParameterHasNoConverters").GetParameters()[0];
            ReflectedParameterDescriptor pdNoConverters = new ReflectedParameterDescriptor(paramWithNoConverters, new Mock<ActionDescriptor>().Object);

            // Act
            object valueWithOneConverter = helper.PublicGetParameterValue(controllerContext, pdOneConverter);
            object valueWithNoConverters = helper.PublicGetParameterValue(controllerContext, pdNoConverters);

            // Assert
            Assert.AreEqual("foo_String", valueWithOneConverter);
            Assert.AreEqual("fooValue", valueWithNoConverters);
        }

        [TestMethod]
        public void GetParameterValueRespectsBindAttribute() {
            // Arrange
            CustomConverterController controller = new CustomConverterController();
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithBindAttribute = typeof(CustomConverterController).GetMethod("ParameterHasBindAttribute").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(paramWithBindAttribute, new Mock<ActionDescriptor>().Object);

            // Act
            object valueWithBindAttribute = helper.PublicGetParameterValue(controllerContext, pd);

            // Assert
            Assert.AreEqual("foo=True&bar=False", valueWithBindAttribute);
        }

        [TestMethod]
        public void GetParameterValueRespectsBindAttributePrefix() {
            // Arrange
            CustomConverterController controller = new CustomConverterController();
            Dictionary<string, object> values = new Dictionary<string, object> { { "foo", "fooValue" }, { "bar", "barValue" } };
            ControllerContext controllerContext = GetControllerContext(controller, values);
            controller.ControllerContext = controllerContext;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithFieldPrefix = typeof(CustomConverterController).GetMethod("ParameterHasFieldPrefix").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(paramWithFieldPrefix, new Mock<ActionDescriptor>().Object);

            // Act
            object parameterValue = helper.PublicGetParameterValue(controllerContext, pd);

            // Assert
            Assert.AreEqual("barValue", parameterValue);
        }

        [TestMethod]
        public void GetParameterValueRespectsBindAttributePrefixOnComplexType() {
            // Arrange
            CustomConverterController controller = new CustomConverterController();
            Dictionary<string, object> values = new Dictionary<string, object> { { "intprop", "123" }, { "stringprop", "hello" } };
            ControllerContext controllerContext = GetControllerContext(controller, values);
            controller.ControllerContext = controllerContext;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithFieldPrefix = typeof(CustomConverterController).GetMethod("ParameterHasPrefixAndComplexType").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(paramWithFieldPrefix, new Mock<ActionDescriptor>().Object);

            // Act
            MySimpleModel parameterValue = helper.PublicGetParameterValue(controllerContext, pd) as MySimpleModel;

            // Assert
            Assert.IsNull(parameterValue);
        }

        [TestMethod]
        public void GetParameterValueRespectsBindAttributeNullPrefix() {
            // Arrange
            CustomConverterController controller = new CustomConverterController();
            Dictionary<string, object> values = new Dictionary<string, object> { { "foo", "fooValue" }, { "bar", "barValue" } };
            ControllerContext controllerContext = GetControllerContext(controller, values);
            controller.ControllerContext = controllerContext;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithFieldPrefix = typeof(CustomConverterController).GetMethod("ParameterHasNullFieldPrefix").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(paramWithFieldPrefix, new Mock<ActionDescriptor>().Object);

            // Act
            object parameterValue = helper.PublicGetParameterValue(controllerContext, pd);

            // Assert
            Assert.AreEqual("fooValue", parameterValue);
        }

        [TestMethod]
        public void GetParameterValueRespectsBindAttributeNullPrefixOnComplexType() {
            // Arrange
            CustomConverterController controller = new CustomConverterController();
            Dictionary<string, object> values = new Dictionary<string, object> { { "intprop", "123" }, { "stringprop", "hello" } };
            ControllerContext controllerContext = GetControllerContext(controller, values);
            controller.ControllerContext = controllerContext;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithFieldPrefix = typeof(CustomConverterController).GetMethod("ParameterHasNoPrefixAndComplexType").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(paramWithFieldPrefix, new Mock<ActionDescriptor>().Object);

            // Act
            MySimpleModel parameterValue = helper.PublicGetParameterValue(controllerContext, pd) as MySimpleModel;

            // Assert
            Assert.IsNotNull(parameterValue);
            Assert.AreEqual(123, parameterValue.IntProp);
            Assert.AreEqual("hello", parameterValue.StringProp);
        }

        [TestMethod]
        public void GetParameterValueRespectsBindAttributeEmptyPrefix() {
            // Arrange
            CustomConverterController controller = new CustomConverterController();
            Dictionary<string, object> values = new Dictionary<string, object> { { "foo", "fooValue" }, { "bar", "barValue" }, { "intprop", "123" }, { "stringprop", "hello" } };
            ControllerContext controllerContext = GetControllerContext(controller, values);
            controller.ControllerContext = controllerContext;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo paramWithFieldPrefix = typeof(CustomConverterController).GetMethod("ParameterHasEmptyFieldPrefix").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(paramWithFieldPrefix, new Mock<ActionDescriptor>().Object);

            // Act
            MySimpleModel parameterValue = helper.PublicGetParameterValue(controllerContext, pd) as MySimpleModel;

            // Assert
            Assert.IsNotNull(parameterValue);
            Assert.AreEqual(123, parameterValue.IntProp);
            Assert.AreEqual("hello", parameterValue.StringProp);
        }

        [TestMethod]
        public void GetParameterValueReturnsNullIfCannotConvertNonRequiredParameter() {
            // Arrange
            Dictionary<string, object> dict = new Dictionary<string, object>() {
                { "id", DateTime.Now } // cannot convert DateTime to Nullable<int>
            };
            var controller = new ParameterTestingController();
            ControllerContext context = GetControllerContext(controller, dict);
            controller.ControllerContext = context;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            MethodInfo mi = typeof(ParameterTestingController).GetMethod("TakesNullableInt");
            ParameterInfo[] pis = mi.GetParameters();
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(pis[0], new Mock<ActionDescriptor>().Object);

            // Act
            object oValue = helper.PublicGetParameterValue(context, pd);

            // Assert
            Assert.IsNull(oValue);
        }

        [TestMethod]
        public void GetParameterValueReturnsNullIfNullableTypeValueNotFound() {
            // Arrange
            var controller = new ParameterTestingController();
            ControllerContext context = GetControllerContext(controller);
            controller.ControllerContext = context;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            MethodInfo mi = typeof(ParameterTestingController).GetMethod("TakesNullableInt");
            ParameterInfo[] pis = mi.GetParameters();
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(pis[0], new Mock<ActionDescriptor>().Object);

            // Act
            object oValue = helper.PublicGetParameterValue(context, pd);

            // Assert
            Assert.IsNull(oValue);
        }

        [TestMethod]
        public void GetParameterValueReturnsNullIfReferenceTypeValueNotFound() {
            // Arrange
            var controller = new ParameterTestingController();
            ControllerContext context = GetControllerContext(controller);
            controller.ControllerContext = context;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            MethodInfo mi = typeof(ParameterTestingController).GetMethod("Foo");
            ParameterInfo[] pis = mi.GetParameters();
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(pis[0], new Mock<ActionDescriptor>().Object);

            // Act
            object oValue = helper.PublicGetParameterValue(context, pd);

            // Assert
            Assert.IsNull(oValue);
        }

        [TestMethod]
        public void GetParameterValuesCallsGetParameterValue() {
            // Arrange
            ControllerBase controller = new ParameterTestingController();
            IDictionary<string, object> dict = new Dictionary<string, object>();
            ControllerContext context = GetControllerContext(controller);
            MethodInfo mi = typeof(ParameterTestingController).GetMethod("Foo");
            ReflectedActionDescriptor ad = new ReflectedActionDescriptor(mi, "Foo", new Mock<ControllerDescriptor>().Object);
            ParameterDescriptor[] pds = ad.GetParameters();

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetParameterValue(context, pds[0])).Returns("Myfoo").Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValue(context, pds[1])).Returns("Mybar").Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValue(context, pds[2])).Returns("Mybaz").Verifiable();
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            IDictionary<string, object> parameters = helper.PublicGetParameterValues(context, ad);

            // Assert
            Assert.AreEqual(3, parameters.Count);
            Assert.AreEqual("Myfoo", parameters["foo"]);
            Assert.AreEqual("Mybar", parameters["bar"]);
            Assert.AreEqual("Mybaz", parameters["baz"]);
            mockHelper.Verify();
        }

        [TestMethod]
        public void GetParameterValuesReturnsEmptyDictionaryForParameterlessMethod() {
            // Arrange
            var controller = new ParameterTestingController();
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            MethodInfo mi = typeof(ParameterTestingController).GetMethod("Parameterless");
            ReflectedActionDescriptor ad = new ReflectedActionDescriptor(mi, "Parameterless", new Mock<ControllerDescriptor>().Object);

            // Act
            IDictionary<string, object> parameters = helper.PublicGetParameterValues(context, ad);

            // Assert
            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void GetParameterValuesReturnsValuesForParametersInOrder() {
            // We need to hook into GetParameterValue() to make sure that GetParameterValues() is calling it.

            // Arrange
            var controller = new ParameterTestingController();
            Dictionary<string, object> dict = new Dictionary<string, object>() {
                { "foo", "MyFoo" },
                { "bar", "MyBar" },
                { "baz", "MyBaz" }
            };
            ControllerContext context = GetControllerContext(controller, dict);
            controller.ControllerContext = context;

            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            MethodInfo mi = typeof(ParameterTestingController).GetMethod("Foo");
            ReflectedActionDescriptor ad = new ReflectedActionDescriptor(mi, "Foo", new Mock<ControllerDescriptor>().Object);

            // Act
            IDictionary<string, object> parameters = helper.PublicGetParameterValues(context, ad);

            // Assert
            Assert.AreEqual(3, parameters.Count);
            Assert.AreEqual("MyFoo", parameters["foo"]);
            Assert.AreEqual("MyBar", parameters["bar"]);
            Assert.AreEqual("MyBaz", parameters["baz"]);
        }

        [TestMethod]
        public void GetParameterValueUsesControllerValueProviderAsValueProvider() {
            // Arrange
            ValueProviderDictionary valueProvider = new ValueProviderDictionary(null) {
                { "foo", new ValueProviderResult("fooValue", "fooValue", null) }
            };

            CustomConverterController controller = new CustomConverterController() { ValueProvider = valueProvider };
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            ParameterInfo parameter = typeof(CustomConverterController).GetMethod("ParameterHasNoConverters").GetParameters()[0];
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(parameter, new Mock<ActionDescriptor>().Object);

            // Act
            object parameterValue = helper.PublicGetParameterValue(controllerContext, pd);

            // Assert
            Assert.AreEqual("fooValue", parameterValue);
        }

        [TestMethod]
        public void InvokeAction() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;

            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            FilterInfo filterInfo = new FilterInfo();

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            MethodInfo methodInfo = typeof(object).GetMethod("ToString");
            ActionResult actionResult = new EmptyResult();
            ActionExecutedContext postContext = new ActionExecutedContext(context, ad, false /* canceled */, null /* exception */) {
                Result = actionResult
            };
            AuthorizationContext authContext = new AuthorizationContext(context);

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(context)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(context, cd, "SomeMethod")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(context, ad)).Returns(filterInfo).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(context, filterInfo.AuthorizationFilters, ad)).Returns(authContext).Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValues(context, ad)).Returns(parameters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionMethodWithFilters(context, filterInfo.ActionFilters, ad, parameters)).Returns(postContext).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResultWithFilters(context, filterInfo.ResultFilters, actionResult)).Returns((ResultExecutedContext)null).Verifiable();
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            bool retVal = helper.InvokeAction(context, "SomeMethod");
            Assert.IsTrue(retVal, "InvokeAction() should return True on success.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionCallsValidateRequestIfAsked() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            controller.ValidateRequest = true;

            ControllerContext context = GetControllerContext(controller, null, true /* throwOnRequestValidation */);
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            FilterInfo filterInfo = new FilterInfo();
            AuthorizationContext authContext = new AuthorizationContext(context);

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>();
            mockHelper.CallBase = true;
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(context)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(context, cd, "SomeMethod")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(context, ad)).Returns(filterInfo).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(context, filterInfo.AuthorizationFilters, ad)).Returns(authContext).Verifiable();
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            ExceptionHelper.ExpectException<HttpRequestValidationException>(
                delegate {
                    helper.InvokeAction(context, "SomeMethod");
                });

            // Assert
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereContinuationThrowsExceptionAndIsHandled() {
            // Arrange
            List<string> actions = new List<string>();
            MethodInfo mi = typeof(object).GetMethod("ToString");
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            Exception exception = new Exception();
            ActionDescriptor action = new Mock<ActionDescriptor>().Object;

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted");
                    Assert.AreSame(exception, filterContext.Exception, "Exception did not match.");
                    Assert.AreSame(action, filterContext.ActionDescriptor, "Descriptor did not match.");
                    Assert.IsFalse(filterContext.ExceptionHandled);
                    filterContext.ExceptionHandled = true;
                }
            };
            Func<ActionExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                throw exception;
            };

            ActionExecutingContext context = new ActionExecutingContext(GetControllerContext(new EmptyController()), action, parameters);

            // Act
            ActionExecutedContext result = ControllerActionInvoker.InvokeActionMethodFilter(filter, context, continuation);

            // Assert
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnActionExecuted", actions[2]);
            Assert.AreSame(exception, result.Exception, "Exception did not match.");
            Assert.AreSame(action, result.ActionDescriptor, "Descriptor did not match.");
            Assert.IsTrue(result.ExceptionHandled);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereContinuationThrowsExceptionAndIsNotHandled() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionDescriptor action = new Mock<ActionDescriptor>().Object;

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    Assert.IsNotNull(filterContext.Exception, "Exception property should have been set.");
                    Assert.AreEqual("Some exception message.", filterContext.Exception.Message);
                    Assert.AreSame(action, filterContext.ActionDescriptor, "Descriptor was incorrect.");
                    actions.Add("OnActionExecuted");
                }
            };
            Func<ActionExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                throw new Exception("Some exception message.");
            };

            ActionExecutingContext context = new ActionExecutingContext(GetControllerContext(new EmptyController()), action, parameters);

            // Act & Assert
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    ControllerActionInvoker.InvokeActionMethodFilter(filter, context, continuation);
                },
               "Some exception message.");
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnActionExecuted", actions[2]);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereContinuationThrowsThreadAbortException() {
            // Arrange
            List<string> actions = new List<string>();
            ActionResult actionResult = new EmptyResult();
            ActionDescriptor action = new Mock<ActionDescriptor>().Object;

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    Thread.ResetAbort();
                    actions.Add("OnActionExecuted");
                    Assert.IsNull(filterContext.Exception, "Exception property should not be set for ThreadAbortException.");
                    Assert.IsFalse(filterContext.ExceptionHandled);
                    Assert.AreSame(action, filterContext.ActionDescriptor, "Descriptor was incorrect.");
                }
            };
            Func<ActionExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                Thread.CurrentThread.Abort();
                return null;
            };

            ActionExecutingContext context = new ActionExecutingContext(new Mock<ControllerContext>().Object, action, new Dictionary<string, object>());

            // Act & Assert
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    ControllerActionInvoker.InvokeActionMethodFilter(filter, context, continuation);
                },
                "Thread was being aborted.");
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnActionExecuted", actions[2]);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereOnActionExecutingCancels() {
            // Arrange
            bool wasCalled = false;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            ActionResult actionResult = new EmptyResult();
            ActionDescriptor action = new Mock<ActionDescriptor>().Object;

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    Assert.IsFalse(wasCalled);
                    wasCalled = true;
                    filterContext.Result = actionResult;
                },
            };
            Func<ActionExecutedContext> continuation = delegate {
                Assert.Fail("The continuation should not be called.");
                return null;
            };

            ActionExecutingContext context = new ActionExecutingContext(GetControllerContext(new EmptyController()), action, parameters);

            // Act
            ActionExecutedContext result = ControllerActionInvoker.InvokeActionMethodFilter(filter, context, continuation);

            // Assert
            Assert.IsTrue(wasCalled);
            Assert.IsNull(result.Exception);
            Assert.IsTrue(result.Canceled);
            Assert.AreSame(actionResult, result.Result, "Result was incorrect.");
            Assert.AreSame(action, result.ActionDescriptor, "Descriptor was incorrect.");
        }

        [TestMethod]
        public void InvokeActionMethodFilterWithNormalControlFlow() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionDescriptor action = new Mock<ActionDescriptor>().Object;

            ActionExecutingContext preContext = new ActionExecutingContext(GetControllerContext(new EmptyController()), action, parameters);
            Mock<ActionExecutedContext> mockPostContext = new Mock<ActionExecutedContext>();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    Assert.AreSame(parameters, filterContext.ActionParameters);
                    Assert.IsNull(filterContext.Result);
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    Assert.AreEqual(mockPostContext.Object, filterContext);
                    actions.Add("OnActionExecuted");
                }
            };
            Func<ActionExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                return mockPostContext.Object;
            };

            // Act
            ActionExecutedContext result = ControllerActionInvoker.InvokeActionMethodFilter(filter, preContext, continuation);

            // Assert
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnActionExecuted", actions[2]);
            Assert.AreSame(result, mockPostContext.Object);
        }

        [TestMethod]
        public void InvokeActionInvokesExceptionFiltersAndExecutesResultIfExceptionHandled() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;

            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            FilterInfo filterInfo = new FilterInfo();

            Exception exception = new Exception();
            ActionResult actionResult = new EmptyResult();
            ExceptionContext exContext = new ExceptionContext(context, exception) {
                ExceptionHandled = true,
                Result = actionResult
            };

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(context)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(context, cd, "SomeMethod")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(context, ad)).Returns(filterInfo).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(context, filterInfo.AuthorizationFilters, ad)).Throws(exception).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(context, filterInfo.ExceptionFilters, exception)).Returns(exContext).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResult(context, actionResult)).Verifiable();
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            bool retVal = helper.InvokeAction(context, "SomeMethod");
            Assert.IsTrue(retVal, "InvokeAction() should return True on success.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionInvokesExceptionFiltersAndRethrowsExceptionIfNotHandled() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;

            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            FilterInfo filterInfo = new FilterInfo();

            Exception exception = new Exception();
            ExceptionContext exContext = new ExceptionContext(context, exception);

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(context)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(context, cd, "SomeMethod")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(context, ad)).Returns(filterInfo).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(context, filterInfo.AuthorizationFilters, ad)).Throws(exception).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(context, filterInfo.ExceptionFilters, exception)).Returns(exContext).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResult(context, It.IsAny<ActionResult>())).Callback(delegate {
                Assert.Fail("InvokeActionResult() shouldn't be called if the exception was unhandled by filters.");
            });
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            Exception thrownException = ExceptionHelper.ExpectException<Exception>(
                delegate {
                    helper.InvokeAction(context, "SomeMethod");
                });

            // Assert
            Assert.AreSame(exception, thrownException);
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionInvokesResultIfAuthorizationFilterReturnsResult() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;

            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            FilterInfo filterInfo = new FilterInfo();

            ActionResult actionResult = new EmptyResult();
            ActionExecutedContext postContext = new ActionExecutedContext(context, ad, false /* canceled */, null /* exception */) {
                Result = actionResult
            };
            AuthorizationContext authContext = new AuthorizationContext(context) { Result = actionResult };

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(context)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(context, cd, "SomeMethod")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(context, ad)).Returns(filterInfo).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(context, filterInfo.AuthorizationFilters, ad)).Returns(authContext).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResult(context, actionResult)).Verifiable();
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            bool retVal = helper.InvokeAction(context, "SomeMethod");
            Assert.IsTrue(retVal, "InvokeAction() should return True on success.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionMethod() {
            // Arrange
            EmptyController controller = new EmptyController();
            ControllerContext controllerContext = GetControllerContext(controller);
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            ActionResult expectedResult = new Mock<ActionResult>().Object;

            Mock<ActionDescriptor> mockAd = new Mock<ActionDescriptor>();
            mockAd.Expect(ad => ad.Execute(controllerContext, parameters)).Returns("hello world");

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicCreateActionResult(controllerContext, mockAd.Object, "hello world")).Returns(expectedResult);
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            ActionResult returnedResult = helper.PublicInvokeActionMethod(controllerContext, mockAd.Object, parameters);

            // Assert
            Assert.AreSame(expectedResult, returnedResult, "Returned result was not correct.");
        }

        [TestMethod]
        public void InvokeActionMethodWithFiltersOrdersFiltersCorrectly() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionResult actionResult = new EmptyResult();

            ActionFilterImpl filter1 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting1");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted1");
                }
            };
            ActionFilterImpl filter2 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting2");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted2");
                }
            };
            Func<ActionResult> continuation = delegate {
                actions.Add("Continuation");
                return new EmptyResult();
            };
            ControllerBase controller = new ContinuationController(continuation);
            ControllerContext context = GetControllerContext(controller);
            ActionDescriptor actionDescriptor = new ReflectedActionDescriptor(ContinuationController.GoMethod, "someName", new Mock<ControllerDescriptor>().Object);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            List<IActionFilter> filters = new List<IActionFilter>() { filter1, filter2 };

            // Act
            helper.PublicInvokeActionMethodWithFilters(context, filters, actionDescriptor, parameters);

            // Assert
            Assert.AreEqual(5, actions.Count);
            Assert.AreEqual("OnActionExecuting1", actions[0]);
            Assert.AreEqual("OnActionExecuting2", actions[1]);
            Assert.AreEqual("Continuation", actions[2]);
            Assert.AreEqual("OnActionExecuted2", actions[3]);
            Assert.AreEqual("OnActionExecuted1", actions[4]);
        }

        [TestMethod]
        public void InvokeActionMethodWithFiltersPassesArgumentsCorrectly() {
            // Arrange
            bool wasCalled = false;
            MethodInfo mi = ContinuationController.GoMethod;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionResult actionResult = new EmptyResult();
            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    Assert.AreSame(parameters, filterContext.ActionParameters);
                    Assert.IsFalse(wasCalled);
                    wasCalled = true;
                    filterContext.Result = actionResult;
                }
            };
            Func<ActionResult> continuation = delegate {
                Assert.Fail("Continuation should not be called.");
                return null;
            };
            ControllerBase controller = new ContinuationController(continuation);
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            ActionDescriptor actionDescriptor = new ReflectedActionDescriptor(ContinuationController.GoMethod, "someName", new Mock<ControllerDescriptor>().Object);
            List<IActionFilter> filters = new List<IActionFilter>() { filter };

            // Act
            ActionExecutedContext result = helper.PublicInvokeActionMethodWithFilters(context, filters, actionDescriptor, parameters);

            // Assert
            Assert.IsTrue(wasCalled);
            Assert.IsNull(result.Exception);
            Assert.IsFalse(result.ExceptionHandled);
            Assert.AreSame(actionResult, result.Result);
            Assert.AreSame(actionDescriptor, result.ActionDescriptor);
        }

        [TestMethod]
        public void InvokeActionPropagatesThreadAbortException() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;

            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            FilterInfo filterInfo = new FilterInfo();

            ActionResult actionResult = new EmptyResult();
            ActionExecutedContext postContext = new ActionExecutedContext(context, ad, false /* canceled */, null /* exception */) {
                Result = actionResult
            };
            AuthorizationContext authContext = new AuthorizationContext(context) { Result = actionResult };

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(context)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(context, cd, "SomeMethod")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(context, ad)).Returns(filterInfo).Verifiable();
            mockHelper
                .Expect(h => h.PublicInvokeAuthorizationFilters(context, filterInfo.AuthorizationFilters, ad))
                .Returns(
                    delegate(ControllerContext cc, IList<IAuthorizationFilter> f, ActionDescriptor a) {
                        Thread.CurrentThread.Abort();
                        return null;
                    });
            ControllerActionInvokerHelper helper = mockHelper.Object;

            bool wasAborted = false;

            // Act
            try {
                helper.InvokeAction(context, "SomeMethod");
            }
            catch (ThreadAbortException) {
                wasAborted = true;
                Thread.ResetAbort();
            }

            // Assert
            Assert.IsTrue(wasAborted, "The ThreadAbortException did not propagate correctly.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionResultWithFiltersPassesSameContextObjectToInnerFilters() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            ControllerContext context = GetControllerContext(controller);

            ResultExecutingContext storedContext = null;
            ActionResult result = new EmptyResult();
            List<IResultFilter> filters = new List<IResultFilter>() {
                new ActionFilterImpl() {
                    OnResultExecutingImpl = delegate(ResultExecutingContext ctx) {
                        storedContext = ctx;
                    },
                    OnResultExecutedImpl = delegate { }
                },
                new ActionFilterImpl() {
                    OnResultExecutingImpl = delegate(ResultExecutingContext ctx) {
                        Assert.AreSame(storedContext, ctx);
                    },
                    OnResultExecutedImpl = delegate { }
                },
            };
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            // Act
            ResultExecutedContext postContext = helper.PublicInvokeActionResultWithFilters(context, filters, result);

            // Assert
            Assert.AreSame(result, postContext.Result);
        }

        [TestMethod]
        public void InvokeActionReturnsFalseIfMethodNotFound() {
            // Arrange
            var controller = new BlankController();
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvoker invoker = new ControllerActionInvoker();

            // Act
            bool retVal = invoker.InvokeAction(context, "foo");

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void InvokeActionThrowsIfControllerContextIsNull() {
            // Arrange
            ControllerActionInvoker invoker = new ControllerActionInvoker();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    invoker.InvokeAction(null, "actionName");
                }, "controllerContext");
        }

        [TestMethod]
        public void InvokeActionWithEmptyActionNameThrows() {
            // Arrange
            var controller = new BasicMethodInvokeController();
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvoker invoker = new ControllerActionInvoker();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    invoker.InvokeAction(context, String.Empty);
                },
                "actionName");
        }

        [TestMethod]
        public void InvokeActionWithNullActionNameThrows() {
            // Arrange
            var controller = new BasicMethodInvokeController();
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvoker invoker = new ControllerActionInvoker();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    invoker.InvokeAction(context, null /* actionName */);
                },
                "actionName");
        }

        [TestMethod]
        public void InvokeActionWithResultExceptionInvokesExceptionFiltersAndExecutesResultIfExceptionHandled() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;

            ControllerContext context = GetControllerContext(controller);
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            FilterInfo filterInfo = new FilterInfo();
            AuthorizationContext authContext = new AuthorizationContext(context);

            Exception exception = new Exception();
            ActionResult actionResult = new EmptyResult();
            ActionExecutedContext postContext = new ActionExecutedContext(context, ad, false /* canceled */, null /* exception */) {
                Result = actionResult
            };
            ExceptionContext exContext = new ExceptionContext(context, exception) {
                ExceptionHandled = true,
                Result = actionResult
            };

            Mock<ControllerActionInvokerHelper> mockHelper = new Mock<ControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(context)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(context, cd, "SomeMethod")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(context, ad)).Returns(filterInfo).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(context, filterInfo.AuthorizationFilters, ad)).Returns(authContext).Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValues(context, ad)).Returns(parameters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionMethodWithFilters(context, filterInfo.ActionFilters, ad, parameters)).Returns(postContext).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResultWithFilters(context, filterInfo.ResultFilters, actionResult)).Throws(exception).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(context, filterInfo.ExceptionFilters, exception)).Returns(exContext).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResult(context, actionResult)).Verifiable();
            ControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            bool retVal = helper.InvokeAction(context, "SomeMethod");
            Assert.IsTrue(retVal, "InvokeAction() should return True on success.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeAuthorizationFilters() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            List<AuthorizationFilterHelper> callQueue = new List<AuthorizationFilterHelper>();
            AuthorizationFilterHelper filter1 = new AuthorizationFilterHelper(callQueue);
            AuthorizationFilterHelper filter2 = new AuthorizationFilterHelper(callQueue);
            IAuthorizationFilter[] filters = new IAuthorizationFilter[] { filter1, filter2 };

            // Act
            AuthorizationContext postContext = helper.PublicInvokeAuthorizationFilters(controllerContext, filters, ad);

            // Assert
            Assert.AreEqual(2, callQueue.Count);
            Assert.AreSame(filter1, callQueue[0]);
            Assert.AreSame(filter2, callQueue[1]);
        }

        [TestMethod]
        public void InvokeAuthorizationFiltersStopsExecutingIfResultProvided() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            ActionResult result = new EmptyResult();

            List<AuthorizationFilterHelper> callQueue = new List<AuthorizationFilterHelper>();
            AuthorizationFilterHelper filter1 = new AuthorizationFilterHelper(callQueue) { ShortCircuitResult = result };
            AuthorizationFilterHelper filter2 = new AuthorizationFilterHelper(callQueue);
            IAuthorizationFilter[] filters = new IAuthorizationFilter[] { filter1, filter2 };

            // Act
            AuthorizationContext postContext = helper.PublicInvokeAuthorizationFilters(controllerContext, filters, ad);

            // Assert
            Assert.AreSame(result, postContext.Result);
            Assert.AreEqual(1, callQueue.Count);
            Assert.AreSame(filter1, callQueue[0]);
        }

        [TestMethod]
        public void InvokeExceptionFilters() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            Exception exception = new Exception();
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            List<ExceptionFilterHelper> callQueue = new List<ExceptionFilterHelper>();
            ExceptionFilterHelper filter1 = new ExceptionFilterHelper(callQueue);
            ExceptionFilterHelper filter2 = new ExceptionFilterHelper(callQueue);
            IExceptionFilter[] filters = new IExceptionFilter[] { filter1, filter2 };

            // Act
            ExceptionContext postContext = helper.PublicInvokeExceptionFilters(controllerContext, filters, exception);

            // Assert
            Assert.AreSame(exception, postContext.Exception);
            Assert.IsFalse(postContext.ExceptionHandled);
            Assert.AreSame(filter1.ContextPassed, filter2.ContextPassed, "The same context should have been passed to each exception filter.");
            Assert.AreEqual(2, callQueue.Count);
            Assert.AreSame(filter1, callQueue[0]);
            Assert.AreSame(filter2, callQueue[1]);
        }

        [TestMethod]
        public void InvokeExceptionFiltersContinuesExecutingIfExceptionHandled() {
            // Arrange
            ControllerBase controller = new Mock<ControllerBase>().Object;
            Exception exception = new Exception();
            ControllerContext controllerContext = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();

            List<ExceptionFilterHelper> callQueue = new List<ExceptionFilterHelper>();
            ExceptionFilterHelper filter1 = new ExceptionFilterHelper(callQueue) { ShouldHandleException = true };
            ExceptionFilterHelper filter2 = new ExceptionFilterHelper(callQueue);
            IExceptionFilter[] filters = new IExceptionFilter[] { filter1, filter2 };

            // Act
            ExceptionContext postContext = helper.PublicInvokeExceptionFilters(controllerContext, filters, exception);

            // Assert
            Assert.AreSame(exception, postContext.Exception);
            Assert.IsTrue(postContext.ExceptionHandled, "The exception should have been handled.");
            Assert.AreSame(filter1.ContextPassed, filter2.ContextPassed, "The same context should have been passed to each exception filter.");
            Assert.AreEqual(2, callQueue.Count);
            Assert.AreSame(filter1, callQueue[0]);
            Assert.AreSame(filter2, callQueue[1]);
        }

        [TestMethod]
        public void InvokeResultFiltersOrdersFiltersCorrectly() {
            // Arrange
            List<string> actions = new List<string>();
            ActionFilterImpl filter1 = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    actions.Add("OnResultExecuting1");
                },
                OnResultExecutedImpl = delegate(ResultExecutedContext filterContext) {
                    actions.Add("OnResultExecuted1");
                }
            };
            ActionFilterImpl filter2 = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    actions.Add("OnResultExecuting2");
                },
                OnResultExecutedImpl = delegate(ResultExecutedContext filterContext) {
                    actions.Add("OnResultExecuted2");
                }
            };
            Action continuation = delegate {
                actions.Add("Continuation");
            };
            ActionResult actionResult = new ContinuationResult(continuation);
            ControllerBase controller = new Mock<ControllerBase>().Object;
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            List<IResultFilter> filters = new List<IResultFilter>() { filter1, filter2 };

            // Act
            helper.PublicInvokeActionResultWithFilters(context, filters, actionResult);

            // Assert
            Assert.AreEqual(5, actions.Count);
            Assert.AreEqual("OnResultExecuting1", actions[0]);
            Assert.AreEqual("OnResultExecuting2", actions[1]);
            Assert.AreEqual("Continuation", actions[2]);
            Assert.AreEqual("OnResultExecuted2", actions[3]);
            Assert.AreEqual("OnResultExecuted1", actions[4]);
        }

        [TestMethod]
        public void InvokeResultFiltersPassesArgumentsCorrectly() {
            // Arrange
            bool wasCalled = false;
            Action continuation = delegate {
                Assert.Fail("Continuation should not be called.");
            };
            ActionResult actionResult = new ContinuationResult(continuation);
            ControllerBase controller = new Mock<ControllerBase>().Object;
            ControllerContext context = GetControllerContext(controller);
            ControllerActionInvokerHelper helper = new ControllerActionInvokerHelper();
            ActionFilterImpl filter = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    Assert.AreSame(actionResult, filterContext.Result);
                    Assert.IsFalse(wasCalled);
                    wasCalled = true;
                    filterContext.Cancel = true;
                }
            };

            List<IResultFilter> filters = new List<IResultFilter>() { filter };

            // Act
            ResultExecutedContext result = helper.PublicInvokeActionResultWithFilters(context, filters, actionResult);

            // Assert
            Assert.IsTrue(wasCalled);
            Assert.IsNull(result.Exception);
            Assert.IsFalse(result.ExceptionHandled);
            Assert.AreSame(actionResult, result.Result);
        }

        [TestMethod]
        public void InvokeResultFilterWhereContinuationThrowsExceptionAndIsHandled() {
            // Arrange
            List<string> actions = new List<string>();
            ActionResult actionResult = new EmptyResult();
            Exception exception = new Exception();
            ActionFilterImpl filter = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    actions.Add("OnResultExecuting");
                },
                OnResultExecutedImpl = delegate(ResultExecutedContext filterContext) {
                    actions.Add("OnResultExecuted");
                    Assert.AreSame(actionResult, filterContext.Result);
                    Assert.AreSame(exception, filterContext.Exception);
                    Assert.IsFalse(filterContext.ExceptionHandled);
                    filterContext.ExceptionHandled = true;
                }
            };
            Func<ResultExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                throw exception;
            };

            Mock<ResultExecutingContext> mockResultExecutingContext = new Mock<ResultExecutingContext>() { DefaultValue = DefaultValue.Mock };
            mockResultExecutingContext.Expect(c => c.Result).Returns(actionResult);

            // Act
            ResultExecutedContext result = ControllerActionInvoker.InvokeActionResultFilter(filter, mockResultExecutingContext.Object, continuation);

            // Assert
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnResultExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnResultExecuted", actions[2]);
            Assert.AreSame(exception, result.Exception);
            Assert.IsTrue(result.ExceptionHandled);
            Assert.AreSame(actionResult, result.Result);
        }

        [TestMethod]
        public void InvokeResultFilterWhereContinuationThrowsExceptionAndIsNotHandled() {
            // Arrange
            List<string> actions = new List<string>();
            ActionFilterImpl filter = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    actions.Add("OnResultExecuting");
                },
                OnResultExecutedImpl = delegate(ResultExecutedContext filterContext) {
                    actions.Add("OnResultExecuted");
                }
            };
            Func<ResultExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                throw new Exception("Some exception message.");
            };

            // Act & Assert
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    ControllerActionInvoker.InvokeActionResultFilter(filter, new Mock<ResultExecutingContext>() { DefaultValue = DefaultValue.Mock }.Object, continuation);
                },
               "Some exception message.");
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnResultExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnResultExecuted", actions[2]);
        }

        [TestMethod]
        public void InvokeResultFilterWhereContinuationThrowsThreadAbortException() {
            // Arrange
            List<string> actions = new List<string>();
            ActionResult actionResult = new EmptyResult();

            Mock<ResultExecutingContext> mockPreContext = new Mock<ResultExecutingContext>() { DefaultValue = DefaultValue.Mock };
            mockPreContext.Expect(c => c.Result).Returns(actionResult);

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    actions.Add("OnResultExecuting");
                },
                OnResultExecutedImpl = delegate(ResultExecutedContext filterContext) {
                    Thread.ResetAbort();
                    actions.Add("OnResultExecuted");
                    Assert.AreSame(actionResult, filterContext.Result);
                    Assert.IsNull(filterContext.Exception);
                    Assert.IsFalse(filterContext.ExceptionHandled);
                }
            };
            Func<ResultExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                Thread.CurrentThread.Abort();
                return null;
            };

            // Act & Assert
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    ControllerActionInvoker.InvokeActionResultFilter(filter, mockPreContext.Object, continuation);
                },
                "Thread was being aborted.");
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnResultExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnResultExecuted", actions[2]);
        }

        [TestMethod]
        public void InvokeResultFilterWhereOnResultExecutingCancels() {
            // Arrange
            bool wasCalled = false;
            MethodInfo mi = typeof(object).GetMethod("ToString");
            object[] paramValues = new object[0];
            ActionResult actionResult = new EmptyResult();
            ActionFilterImpl filter = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    Assert.IsFalse(wasCalled);
                    wasCalled = true;
                    filterContext.Cancel = true;
                },
            };
            Func<ResultExecutedContext> continuation = delegate {
                Assert.Fail("The continuation should not be called.");
                return null;
            };

            Mock<ResultExecutingContext> mockResultExecutingContext = new Mock<ResultExecutingContext>() { DefaultValue = DefaultValue.Mock };
            mockResultExecutingContext.Expect(c => c.Result).Returns(actionResult);

            // Act
            ResultExecutedContext result = ControllerActionInvoker.InvokeActionResultFilter(filter, mockResultExecutingContext.Object, continuation);

            // Assert
            Assert.IsTrue(wasCalled);
            Assert.IsNull(result.Exception);
            Assert.IsTrue(result.Canceled);
            Assert.AreSame(actionResult, result.Result);
        }

        [TestMethod]
        public void InvokeResultFilterWithNormalControlFlow() {
            // Arrange
            List<string> actions = new List<string>();
            ActionResult actionResult = new EmptyResult();

            Mock<ResultExecutedContext> mockPostContext = new Mock<ResultExecutedContext>();
            mockPostContext.Expect(c => c.Result).Returns(actionResult);

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnResultExecutingImpl = delegate(ResultExecutingContext filterContext) {
                    Assert.AreSame(actionResult, filterContext.Result);
                    Assert.IsFalse(filterContext.Cancel);
                    actions.Add("OnResultExecuting");
                },
                OnResultExecutedImpl = delegate(ResultExecutedContext filterContext) {
                    Assert.AreEqual(mockPostContext.Object, filterContext);
                    actions.Add("OnResultExecuted");
                }
            };
            Func<ResultExecutedContext> continuation = delegate {
                actions.Add("Continuation");
                return mockPostContext.Object;
            };

            Mock<ResultExecutingContext> mockResultExecutingContext = new Mock<ResultExecutingContext>();
            mockResultExecutingContext.Expect(c => c.Result).Returns(actionResult);

            // Act
            ResultExecutedContext result = ControllerActionInvoker.InvokeActionResultFilter(filter, mockResultExecutingContext.Object, continuation);

            // Assert
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnResultExecuting", actions[0]);
            Assert.AreEqual("Continuation", actions[1]);
            Assert.AreEqual("OnResultExecuted", actions[2]);
            Assert.AreSame(result, mockPostContext.Object);
        }

        [TestMethod]
        public void InvokeMethodCallsOverriddenCreateActionResult() {
            // Arrange
            CustomResultInvokerController controller = new CustomResultInvokerController();
            ControllerContext context = GetControllerContext(controller);
            CustomResultInvoker helper = new CustomResultInvoker();
            MethodInfo mi = typeof(CustomResultInvokerController).GetMethod("ReturnCustomResult");
            ReflectedActionDescriptor ad = new ReflectedActionDescriptor(mi, "ReturnCustomResult", new Mock<ControllerDescriptor>().Object);
            IDictionary<string, object> parameters = new Dictionary<string, object>();

            // Act
            ActionResult actionResult = helper.PublicInvokeActionMethod(context, ad, parameters);

            // Assert (arg got passed to method + back correctly)
            Assert.IsInstanceOfType(actionResult, typeof(CustomResult));
            CustomResult customResult = (CustomResult)actionResult;
            Assert.AreEqual("abc123", customResult.ReturnValue);
        }

        private static ControllerContext GetControllerContext(ControllerBase controller) {
            return GetControllerContext(controller, null);
        }

        private static ControllerContext GetControllerContext(ControllerBase controller, IDictionary<string, object> values) {
            return GetControllerContext(controller, values, false);
        }

        private static ControllerContext GetControllerContext(ControllerBase controller, IDictionary<string, object> values, bool throwOnRequestValidation) {
            if (values != null) {
                ValueProviderDictionary valueProvider = new ValueProviderDictionary(null);
                foreach (var entry in values) {
                    valueProvider[entry.Key] = new ValueProviderResult(entry.Value, Convert.ToString(entry.Value), null);
                }
                controller.ValueProvider = valueProvider;
            }

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>() { DefaultValue = DefaultValue.Mock };

            mockControllerContext.Expect(c => c.HttpContext.Request.ValidateInput()).Callback(() => {
                if (!controller.ValidateRequest) {
                    Assert.Fail("ValidateRequest() should not be called if the controller opted out.");
                }
            });
            mockControllerContext.Expect(c => c.HttpContext.Request.RawUrl).Returns(() => {
                if (throwOnRequestValidation) {
                    throw new HttpRequestValidationException();
                }
                return "someRawUrl";
            });

            mockControllerContext.Expect(c => c.HttpContext.Session).Returns((HttpSessionStateBase)null);
            mockControllerContext.Expect(c => c.Controller).Returns(controller);
            return mockControllerContext.Object;
        }

        private class EmptyActionFilterAttribute : ActionFilterAttribute {
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

        private class ActionFilterImpl : IActionFilter, IResultFilter {

            public Action<ActionExecutingContext> OnActionExecutingImpl {
                get;
                set;
            }

            public void OnActionExecuting(ActionExecutingContext filterContext) {
                OnActionExecutingImpl(filterContext);
            }

            public Action<ActionExecutedContext> OnActionExecutedImpl {
                get;
                set;
            }

            public void OnActionExecuted(ActionExecutedContext filterContext) {
                OnActionExecutedImpl(filterContext);
            }

            public Action<ResultExecutingContext> OnResultExecutingImpl {
                get;
                set;
            }

            public void OnResultExecuting(ResultExecutingContext filterContext) {
                OnResultExecutingImpl(filterContext);
            }

            public Action<ResultExecutedContext> OnResultExecutedImpl {
                get;
                set;
            }

            public void OnResultExecuted(ResultExecutedContext filterContext) {
                OnResultExecutedImpl(filterContext);
            }

        }

        [KeyedActionFilter(Key = "BaseClass", Order = 0)]
        [KeyedAuthorizationFilter(Key = "BaseClass", Order = 0)]
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

        // This controller serves only to test vanilla method invocation - nothing exciting here
        private class BasicMethodInvokeController : Controller {
            public ActionResult ReturnsRenderView(object viewItem) {
                return View("ReturnsRenderView", viewItem);
            }
        }

        private class BlankController : Controller {
        }

        private sealed class CustomResult : ActionResult {
            public object ReturnValue {
                get;
                set;
            }

            public override void ExecuteResult(ControllerContext context) {
                throw new NotImplementedException();
            }
        }

        private sealed class CustomResultInvokerController : Controller {
            public object ReturnCustomResult() {
                return "abc123";
            }

        }

        private sealed class CustomResultInvoker : ControllerActionInvokerHelper {
            protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue) {
                return new CustomResult {
                    ReturnValue = actionReturnValue
                };
            }
        }

        private class ContinuationController : Controller {

            private Func<ActionResult> _continuation;

            public ContinuationController(Func<ActionResult> continuation) {
                _continuation = continuation;
            }

            public ActionResult Go() {
                return _continuation();
            }

            public static MethodInfo GoMethod {
                get {
                    return typeof(ContinuationController).GetMethod("Go");
                }
            }

        }

        private class ContinuationResult : ActionResult {

            private Action _continuation;

            public ContinuationResult(Action continuation) {
                _continuation = continuation;
            }

            public override void ExecuteResult(ControllerContext context) {
                _continuation();
            }

        }

        private class EmptyController : Controller {
        }

        // This controller serves to test the default action method matching mechanism
        private class FindMethodController : Controller {

            public ActionResult ValidActionMethod() {
                return null;
            }

            [NonAction]
            public virtual ActionResult NonActionMethod() {
                return null;
            }

            [NonAction]
            public ActionResult DerivedIsActionMethod() {
                return null;
            }

            public ActionResult MethodOverloaded() {
                return null;
            }

            public ActionResult MethodOverloaded(string s) {
                return null;
            }

            public void WrongReturnType() {
            }

            protected ActionResult ProtectedMethod() {
                return null;
            }

            private ActionResult PrivateMethod() {
                return null;
            }

            internal ActionResult InternalMethod() {
                return null;
            }

            public override string ToString() {
                // originally defined on Object
                return base.ToString();
            }

            public ActionResult Property {
                get {
                    return null;
                }
            }

#pragma warning disable 0067
            // CS0067: Event declared but never used. We use reflection to access this member.
            public event EventHandler Event;
#pragma warning restore 0067

        }

        private class DerivedFindMethodController : FindMethodController {

            public override ActionResult NonActionMethod() {
                return base.NonActionMethod();
            }

            // FindActionMethod() should accept this as a valid method since [NonAction] doesn't appear
            // in its inheritance chain.
            public new ActionResult DerivedIsActionMethod() {
                return base.DerivedIsActionMethod();
            }

        }

        // Similar to FindMethodController, but tests generics support specifically
        private class GenericFindMethodController<T> : Controller {

            public ActionResult ClosedGenericMethod(T t) {
                return null;
            }

            public ActionResult OpenGenericMethod<U>(U t) {
                return null;
            }

        }

        // Allows for testing parameter conversions, etc.
        private class ParameterTestingController : Controller {

            public ParameterTestingController() {
                Values = new Dictionary<string, object>();
            }

            public IDictionary<string, object> Values {
                get;
                private set;
            }

            public void Foo(string foo, string bar, string baz) {
                Values["foo"] = foo;
                Values["bar"] = bar;
                Values["baz"] = baz;
            }

            public void HasOutParam(out string foo) {
                foo = null;
            }

            public void HasRefParam(ref string foo) {
            }

            public void Parameterless() {
            }

            public void TakesInt(int id) {
                Values["id"] = id;
            }

            public ActionResult TakesNullableInt(int? id) {
                Values["id"] = id;
                return null;
            }

            public void TakesString(string id) {
            }

            public void TakesDateTime(DateTime id) {
            }

        }

        // Provides access to the protected members of ControllerActionInvoker
        public class ControllerActionInvokerHelper : ControllerActionInvoker {

            public ControllerActionInvokerHelper() {
                // set instance caches to prevent modifying global test application state
                DescriptorCache = new ControllerDescriptorCache();
            }

            public virtual ActionResult PublicCreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue) {
                return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
            }

            protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue) {
                return PublicCreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
            }

            public virtual ActionDescriptor PublicFindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {
                return base.FindAction(controllerContext, controllerDescriptor, actionName);
            }

            protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {
                return PublicFindAction(controllerContext, controllerDescriptor, actionName);
            }

            public virtual ControllerDescriptor PublicGetControllerDescriptor(ControllerContext controllerContext) {
                return base.GetControllerDescriptor(controllerContext);
            }

            protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext) {
                return PublicGetControllerDescriptor(controllerContext);
            }

            public virtual FilterInfo PublicGetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return base.GetFilters(controllerContext, actionDescriptor);
            }

            protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return PublicGetFilters(controllerContext, actionDescriptor);
            }

            public virtual object PublicGetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor) {
                return base.GetParameterValue(controllerContext, parameterDescriptor);
            }

            protected override object GetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor) {
                return PublicGetParameterValue(controllerContext, parameterDescriptor);
            }

            public virtual IDictionary<string, object> PublicGetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return base.GetParameterValues(controllerContext, actionDescriptor);
            }

            protected override IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return PublicGetParameterValues(controllerContext, actionDescriptor);
            }

            public virtual ActionResult PublicInvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters) {
                return base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
            }

            protected override ActionResult InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters) {
                return PublicInvokeActionMethod(controllerContext, actionDescriptor, parameters);
            }

            public virtual ActionExecutedContext PublicInvokeActionMethodWithFilters(ControllerContext controllerContext, IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters) {
                return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
            }

            protected override ActionExecutedContext InvokeActionMethodWithFilters(ControllerContext controllerContext, IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters) {
                return PublicInvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
            }

            public virtual void PublicInvokeActionResult(ControllerContext controllerContext, ActionResult actionResult) {
                base.InvokeActionResult(controllerContext, actionResult);
            }

            protected override void InvokeActionResult(ControllerContext controllerContext, ActionResult actionResult) {
                PublicInvokeActionResult(controllerContext, actionResult);
            }

            public virtual ResultExecutedContext PublicInvokeActionResultWithFilters(ControllerContext controllerContext, IList<IResultFilter> filters, ActionResult actionResult) {
                return base.InvokeActionResultWithFilters(controllerContext, filters, actionResult);
            }

            protected override ResultExecutedContext InvokeActionResultWithFilters(ControllerContext controllerContext, IList<IResultFilter> filters, ActionResult actionResult) {
                return PublicInvokeActionResultWithFilters(controllerContext, filters, actionResult);
            }

            public virtual AuthorizationContext PublicInvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor) {
                return base.InvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
            }

            protected override AuthorizationContext InvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor) {
                return PublicInvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
            }

            public virtual ExceptionContext PublicInvokeExceptionFilters(ControllerContext controllerContext, IList<IExceptionFilter> filters, Exception exception) {
                return base.InvokeExceptionFilters(controllerContext, filters, exception);
            }

            protected override ExceptionContext InvokeExceptionFilters(ControllerContext controllerContext, IList<IExceptionFilter> filters, Exception exception) {
                return PublicInvokeExceptionFilters(controllerContext, filters, exception);
            }

        }

        public class AuthorizationFilterHelper : IAuthorizationFilter {

            private IList<AuthorizationFilterHelper> _callQueue;
            public ActionResult ShortCircuitResult;

            public AuthorizationFilterHelper(IList<AuthorizationFilterHelper> callQueue) {
                _callQueue = callQueue;
            }

            public void OnAuthorization(AuthorizationContext filterContext) {
                _callQueue.Add(this);
                if (ShortCircuitResult != null) {
                    filterContext.Result = ShortCircuitResult;
                }
            }
        }

        public class ExceptionFilterHelper : IExceptionFilter {

            private IList<ExceptionFilterHelper> _callQueue;
            public bool ShouldHandleException;
            public ExceptionContext ContextPassed;

            public ExceptionFilterHelper(IList<ExceptionFilterHelper> callQueue) {
                _callQueue = callQueue;
            }

            public void OnException(ExceptionContext filterContext) {
                _callQueue.Add(this);
                if (ShouldHandleException) {
                    filterContext.ExceptionHandled = true;
                }
                ContextPassed = filterContext;
            }
        }

        private class CustomConverterController : Controller {

            public void ParameterWithoutBindAttribute([PredicateReflector] string someParam) {
            }

            public void ParameterHasBindAttribute([Bind(Include = "foo"), PredicateReflector] string someParam) {
            }

            public void ParameterHasFieldPrefix([Bind(Prefix = "bar")] string foo) {
            }

            public void ParameterHasNullFieldPrefix([Bind(Include = "whatever")] string foo) {
            }

            public void ParameterHasEmptyFieldPrefix([Bind(Prefix = "")] MySimpleModel foo) {
            }

            public void ParameterHasNoPrefixAndComplexType(MySimpleModel foo) {
            }

            public void ParameterHasPrefixAndComplexType([Bind(Prefix = "badprefix")]MySimpleModel foo) {
            }

            public void ParameterHasNoConverters(string foo) {
            }

            public void ParameterHasOneConverter([MyCustomConverter] string foo) {
            }

            public void ParameterHasTwoConverters([MyCustomConverter, MyCustomConverter] string foo) {
            }
        }

        public class MySimpleModel {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
        }

        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
        private class PredicateReflectorAttribute : CustomModelBinderAttribute {
            public override IModelBinder GetBinder() {
                return new MyConverter();
            }
            private class MyConverter : IModelBinder {
                public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                    string s = String.Format("foo={0}&bar={1}", bindingContext.PropertyFilter("foo"), bindingContext.PropertyFilter("bar"));
                    return s;
                }
            }
        }

        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
        private class MyCustomConverterAttribute : CustomModelBinderAttribute {
            public override IModelBinder GetBinder() {
                return new MyConverter();
            }
            private class MyConverter : IModelBinder {
                public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                    string s = bindingContext.ModelName + "_" + bindingContext.ModelType.Name;
                    return s;
                }
            }
        }

    }
}
