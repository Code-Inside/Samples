namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReflectedParameterBindingInfoTest {

        [TestMethod]
        public void BinderProperty() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasSingleModelBinderAttribute").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act
            IModelBinder binder = bindingInfo.Binder;

            // Assert
            Assert.IsInstanceOfType(binder, typeof(MyModelBinder));
        }

        [TestMethod]
        public void BinderPropertyThrowsIfMultipleBinderAttributesFound() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasMultipleModelBinderAttributes").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    IModelBinder binder = bindingInfo.Binder;
                },
                "The parameter 'p1' on method 'Void ParameterHasMultipleModelBinderAttributes(System.Object)' contains multiple attributes inheriting from CustomModelBinderAttribute.");
        }

        [TestMethod]
        public void ExcludeProperty() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasBindAttribute").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act
            ICollection<string> excludes = bindingInfo.Exclude;

            // Assert
            Assert.IsInstanceOfType(excludes, typeof(ReadOnlyCollection<string>));

            string[] excludesArray = excludes.ToArray();
            Assert.AreEqual(2, excludesArray.Length);
            Assert.AreEqual("excl_a", excludesArray[0]);
            Assert.AreEqual("excl_b", excludesArray[1]);
        }

        [TestMethod]
        public void ExcludePropertyReturnsEmptyArrayIfNoBindAttributeSpecified() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasNoBindAttributes").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act
            ICollection<string> excludes = bindingInfo.Exclude;

            // Assert
            Assert.IsNotNull(excludes);
            Assert.AreEqual(0, excludes.Count);
        }

        [TestMethod]
        public void IncludeProperty() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasBindAttribute").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act
            ICollection<string> includes = bindingInfo.Include;

            // Assert
            Assert.IsInstanceOfType(includes, typeof(ReadOnlyCollection<string>));

            string[] includesArray = includes.ToArray();
            Assert.AreEqual(2, includesArray.Length);
            Assert.AreEqual("incl_a", includesArray[0]);
            Assert.AreEqual("incl_b", includesArray[1]);
        }

        [TestMethod]
        public void IncludePropertyReturnsEmptyArrayIfNoBindAttributeSpecified() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasNoBindAttributes").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act
            ICollection<string> includes = bindingInfo.Include;

            // Assert
            Assert.IsNotNull(includes);
            Assert.AreEqual(0, includes.Count);
        }

        [TestMethod]
        public void PrefixProperty() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasBindAttribute").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act
            string prefix = bindingInfo.Prefix;

            // Assert
            Assert.AreEqual("some prefix", prefix);
        }

        [TestMethod]
        public void PrefixPropertyReturnsNullIfNoBindAttributeSpecified() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("ParameterHasNoBindAttributes").GetParameters()[0];
            ReflectedParameterBindingInfo bindingInfo = new ReflectedParameterBindingInfo(pInfo);

            // Act
            string prefix = bindingInfo.Prefix;

            // Assert
            Assert.IsNull(prefix);
        }

        private class MyController : Controller {
            public void ParameterHasBindAttribute(
                [Bind(Prefix = "some prefix", Include = "incl_a, incl_b", Exclude = "excl_a, excl_b")] object p1) {
            }
            public void ParameterHasNoBindAttributes(object p1) {
            }
            public void ParameterHasSingleModelBinderAttribute([ModelBinder(typeof(MyModelBinder))] object p1) {
            }
            public void ParameterHasMultipleModelBinderAttributes([MyCustomModelBinder, MyCustomModelBinder] object p1) {
            }
        }

        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
        private class MyCustomModelBinderAttribute : CustomModelBinderAttribute {
            public override IModelBinder GetBinder() {
                throw new NotImplementedException();
            }
        }

        private class MyModelBinder : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

    }
}
