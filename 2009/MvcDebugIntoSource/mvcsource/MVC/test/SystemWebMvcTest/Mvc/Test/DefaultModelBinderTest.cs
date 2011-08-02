namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [CLSCompliant(false)]
    public class DefaultModelBinderTest {

        [TestMethod]
        public void BindComplexElementalModelReturnsIfOnModelUpdatingReturnsFalse() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            MyModel model = new MyModel() { ReadWriteProperty = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelType = typeof(MyModel)
            };

            Mock<DefaultModelBinderHelper> mockHelper = new Mock<DefaultModelBinderHelper>() { CallBase = true };
            mockHelper.Expect(b => b.PublicOnModelUpdating(controllerContext, It.IsAny<ModelBindingContext>())).Returns(false);
            mockHelper.Expect(b => b.PublicGetModelProperties(controllerContext, It.IsAny<ModelBindingContext>())).Never();
            mockHelper.Expect(b => b.PublicBindProperty(controllerContext, It.IsAny<ModelBindingContext>(), It.IsAny<PropertyDescriptor>())).Never();
            DefaultModelBinderHelper helper = mockHelper.Object;

            // Act
            helper.BindComplexElementalModel(controllerContext, bindingContext, model);

            // Assert
            Assert.AreEqual(3, model.ReadWriteProperty, "Model should not have been updated.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void BindComplexModelCanBindArrays() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(int[]),
                PropertyFilter = _ => false,
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "foo[0]", null }, { "foo[1]", null }, { "foo[2]", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(newModel, typeof(int[]));
            int[] newIntArray = (int[])newModel;

            Assert.AreEqual(3, newIntArray.Length, "Model is not of correct length.");
            Assert.AreEqual(0, newIntArray[0]);
            Assert.AreEqual(1, newIntArray[1]);
            Assert.AreEqual(2, newIntArray[2]);
        }

        [TestMethod]
        public void BindComplexModelCanBindCollections() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelType = typeof(IList<int>),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "foo[0]", null }, { "foo[1]", null }, { "foo[2]", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(newModel, typeof(IList<int>));
            IList<int> modelAsList = (IList<int>)newModel;

            Assert.AreEqual(3, modelAsList.Count, "Model is not of correct length.");
            Assert.AreEqual(0, modelAsList[0]);
            Assert.AreEqual(1, modelAsList[1]);
            Assert.AreEqual(2, modelAsList[2]);
        }

        [TestMethod]
        public void BindComplexModelCanBindDictionaries() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelType = typeof(IDictionary<int, string>),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(new ModelBindingContext().PropertyFilter, bc.PropertyFilter, "PropertyFilter should not have been set.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10;
                    });

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(string), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockIntBinder.Object },
                    { typeof(string), mockStringBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(newModel, typeof(IDictionary<int, string>));
            IDictionary<int, string> modelAsDictionary = (IDictionary<int, string>)newModel;

            Assert.AreEqual(3, modelAsDictionary.Count, "Model is not of correct length.");
            Assert.AreEqual("10Value", modelAsDictionary[10]);
            Assert.AreEqual("11Value", modelAsDictionary[11]);
            Assert.AreEqual("12Value", modelAsDictionary[12]);
        }

        [TestMethod]
        public void BindComplexModelCanBindObjects() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelType = typeof(ModelWithoutBindAttribute),
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindComplexModelReturnsNullArrayIfNoValuesProvided() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(int[]),
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "foo", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(null, bindingContext);

            // Assert
            Assert.IsNull(newModel, "Method should have returned null.");
        }

        [TestMethod]
        public void BindComplexModelWhereModelTypeContainsBindAttribute() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithBindAttribute model = new ModelWithBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelType = typeof(ModelWithBindAttribute),
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual("FooPreValue", model.Foo, "Foo property shouldn't have been updated since it was in the exclusion list.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindComplexModelWhereModelTypeDoesNotContainBindAttribute() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelType = typeof(ModelWithoutBindAttribute),
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindModelCanBindObjects() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelType = typeof(ModelWithoutBindAttribute),
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindModelCanBindSimpleTypes() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(int),
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "foo", new ValueProviderResult("42", "42", null) } }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(null, bindingContext);

            // Assert
            Assert.AreEqual(42, updatedModel);
        }

        [TestMethod]
        public void BindModelReturnsNullIfKeyNotFound() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(int),
                ValueProvider = new Dictionary<string, ValueProviderResult>()
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedModel = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(returnedModel);
        }

        [TestMethod]
        public void BindModelThrowsIfBindingContextIsNull() {
            // Arrange
            DefaultModelBinder binder = new DefaultModelBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "bindingContext");
        }

        [TestMethod]
        public void BindModelWithPrefix() {
            // Arrange
            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "prefix",
                ModelType = typeof(ModelWithoutBindAttribute),
                ValueProvider = new ValueProviderDictionary(null) {
                    { "prefix.foo", new ValueProviderResult("FooPostValue", "FooPostValue", null) },
                    { "prefix.bar", new ValueProviderResult("BarPostValue", "BarPostValue", null) }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(null, bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindModelWithPrefixAndFallback() {
            // Arrange
            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                FallbackToEmptyPrefix = true,
                Model = model,
                ModelName = "prefix",
                ModelType = typeof(ModelWithoutBindAttribute),
                ValueProvider = new ValueProviderDictionary(null) {
                    { "foo", new ValueProviderResult("FooPostValue", "FooPostValue", null) },
                    { "bar", new ValueProviderResult("BarPostValue", "BarPostValue", null) }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(null, bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindModelWithPrefixReturnsNullIfFallbackNotSpecifiedAndValueProviderContainsNoEntries() {
            // Arrange
            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "prefix",
                ModelType = typeof(ModelWithoutBindAttribute),
                ValueProvider = new ValueProviderDictionary(null) {
                    { "foo", new ValueProviderResult("FooPostValue", "FooPostValue", null) },
                    { "bar", new ValueProviderResult("BarPostValue", "BarPostValue", null) }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(updatedModel);
        }

        [TestMethod]
        public void BindModelReturnsNullIfSimpleTypeNotFound() {
            // DevDiv 216165: ModelBinders should not try and instantiate simple types

            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "prefix",
                ModelType = typeof(string),
                ValueProvider = new ValueProviderDictionary(null) {
                    { "prefix.foo", new ValueProviderResult("foo", "foo", null) },
                    { "prefix.bar", new ValueProviderResult("bar", "bar", null) }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(updatedModel);
        }

        [TestMethod]
        public void BindPropertyCanUpdateComplexReadOnlyProperties() {
            // Arrange
            // the Customer type contains a single read-only Address property
            Customer model = new Customer();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "Address", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Address address = (Address)bc.Model;
                        address.Street = "1 Microsoft Way";
                        address.Zip = "98052";
                        return address;
                    });

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["Address"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(Address), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(null, bindingContext, pd);

            // Assert
            Assert.AreEqual("1 Microsoft Way", model.Address.Street, "Property should have been updated.");
            Assert.AreEqual("98052", model.Address.Zip, "Property should have been updated.");
        }

        [TestMethod]
        public void BindPropertyDoesNothingIfValueProviderContainsNoEntryForProperty() {
            // Arrange
            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ValueProvider = new Dictionary<string, ValueProviderResult>()
            };

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicBindProperty(null, bindingContext, pd);

            // Assert
            Assert.AreEqual(3, model.IntReadWrite, "Property should not have been changed.");
        }

        [TestMethod]
        public void BindProperty() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "IntReadWrite", new ValueProviderResult("42", "42", null) }
                }
            };

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];

            Mock<DefaultModelBinderHelper> mockHelper = new Mock<DefaultModelBinderHelper>() { CallBase = true };
            mockHelper.Expect(b => b.PublicOnPropertyValidating(controllerContext, bindingContext, pd, 42)).Returns(true).Verifiable();
            mockHelper.Expect(b => b.PublicSetProperty(controllerContext, bindingContext, pd, 42)).Verifiable();
            mockHelper.Expect(b => b.PublicOnPropertyValidated(controllerContext, bindingContext, pd, 42)).Verifiable();
            DefaultModelBinderHelper helper = mockHelper.Object;

            // Act
            helper.PublicBindProperty(controllerContext, bindingContext, pd);

            // Assert
            mockHelper.Verify();
        }

        [TestMethod]
        public void BindPropertyReturnsIfOnPropertyValidatingReturnsFalse() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "IntReadWrite", new ValueProviderResult("42", "42", null) }
                }
            };

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];

            Mock<DefaultModelBinderHelper> mockHelper = new Mock<DefaultModelBinderHelper>() { CallBase = true };
            mockHelper.Expect(b => b.PublicOnPropertyValidating(controllerContext, bindingContext, pd, 42)).Returns(false);
            mockHelper.Expect(b => b.PublicSetProperty(controllerContext, bindingContext, pd, 42)).Never();
            mockHelper.Expect(b => b.PublicOnPropertyValidated(controllerContext, bindingContext, pd, 42)).Never();
            DefaultModelBinderHelper helper = mockHelper.Object;

            // Act
            helper.PublicBindProperty(controllerContext, bindingContext, pd);

            // Assert
            Assert.AreEqual(3, model.IntReadWrite, "Property should not have been changed.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void BindPropertySetsPropertyToNullIfUserLeftTextEntryFieldBlankForOptionalValue() {
            // Arrange
            MyModel2 model = new MyModel2() { NullableIntReadWrite = 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "NullableIntReadWrite", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder.Expect(b => b.BindModel(null, It.IsAny<ModelBindingContext>())).Returns((object)null);

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["NullableIntReadWrite"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int?), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(null, bindingContext, pd);

            // Assert
            Assert.AreEqual(0, bindingContext.ModelState.Count, "Should not have been an error.");
            Assert.AreEqual(null, model.NullableIntReadWrite, "Property should not have been updated.");
        }

        [TestMethod]
        public void BindPropertyUpdatesPropertyOnFailureIfInnerBinderReturnsNonNullObject() {
            // Arrange
            MyModel2 model = new MyModel2() { IntReadWriteNonNegative = 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "IntReadWriteNonNegative", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(null, It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        bc.ModelState.AddModelError("IntReadWriteNonNegative", "Some error text.");
                        return 4;
                    });

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWriteNonNegative"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(null, bindingContext, pd);

            // Assert
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("IntReadWriteNonNegative"), "Error should have propagated.");
            Assert.AreEqual(1, bindingContext.ModelState["IntReadWriteNonNegative"].Errors.Count, "Wrong number of errors.");
            Assert.AreEqual("Some error text.", bindingContext.ModelState["IntReadWriteNonNegative"].Errors[0].ErrorMessage, "Wrong error text.");
            Assert.AreEqual(4, model.IntReadWriteNonNegative, "Property should have been updated.");
        }

        [TestMethod]
        public void BindPropertyUpdatesPropertyOnSuccess() {
            // Arrange
            // Effectively, this is just testing updating a single property named "IntReadWrite"
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "foo",
                ModelState = new ModelStateDictionary() { { "blah", new ModelState() } },
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "foo.IntReadWrite", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(3, bc.Model, "Original model was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "Model type was not forwarded correctly.");
                        Assert.AreEqual("foo.IntReadWrite", bc.ModelName, "Model name was not forwarded correctly.");
                        Assert.AreEqual(new ModelBindingContext().PropertyFilter, bc.PropertyFilter, "Property filter property should not have been set.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return 4;
                    });

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(controllerContext, bindingContext, pd);

            // Assert
            Assert.AreEqual(4, model.IntReadWrite, "Property should have been updated.");
        }

        [TestMethod]
        public void BindSimpleModelCanReturnArrayTypes() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(42, null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(int[])
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.IsInstanceOfType(returnedValue, typeof(int[]), "Returned value was of incorrect type.");

            int[] returnedValueAsIntArray = (int[])returnedValue;
            Assert.AreEqual(1, returnedValueAsIntArray.Length);
            Assert.AreEqual(42, returnedValueAsIntArray[0]);
        }

        [TestMethod]
        public void BindSimpleModelCanReturnCollectionTypes() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(new string[] { "42", "82" }, null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(IEnumerable<int>)
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.IsInstanceOfType(returnedValue, typeof(IEnumerable<int>), "Returned value was of incorrect type.");
            List<int> returnedValueAsList = ((IEnumerable<int>)returnedValue).ToList();

            Assert.AreEqual(2, returnedValueAsList.Count);
            Assert.AreEqual(42, returnedValueAsList[0]);
            Assert.AreEqual(82, returnedValueAsList[1]);
        }

        [TestMethod]
        public void BindSimpleModelCanReturnElementalTypes() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult("42", null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(int)
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.AreEqual(42, returnedValue);
        }

        [TestMethod]
        public void BindSimpleModelCanReturnStrings() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(new object[] { "42" }, null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(string)
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.AreEqual("42", returnedValue);
        }

        [TestMethod]
        public void BindSimpleModelChecksValueProviderResultRawValueType() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(new MemoryStream(), null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(Stream)
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.AreEqual(result, bindingContext.ModelState["foo"].Value, "ModelState should have been set.");
            Assert.AreSame(result.RawValue, returnedValue, "Should have returned the RawValue since it was of the correct type.");
        }

        [TestMethod]
        public void BindSimpleModelPropagatesErrorsOnFailure() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult("invalid", null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ModelType = typeof(int)
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.IsFalse(bindingContext.ModelState.IsValidField("foo"), "Foo should be an invalid field.");
            Assert.IsInstanceOfType(bindingContext.ModelState["foo"].Errors[0].Exception, typeof(InvalidOperationException));
            Assert.AreEqual("The parameter conversion from type 'System.String' to type 'System.Int32' failed. See the inner exception for more information.", bindingContext.ModelState["foo"].Errors[0].Exception.Message);
            Assert.IsNull(returnedValue, "Should have returned null on failure.");
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstance() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(Guid));

            // Assert
            Assert.AreEqual(Guid.Empty, modelObj);
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericICollection() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(ICollection<Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(ICollection<Guid>));
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericIDictionary() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(IDictionary<string, Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(IDictionary<string, Guid>));
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericIEnumerable() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(IEnumerable<Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(ICollection<Guid>), "We must actually create an ICollection<> when asked to create an IEnumerable<>.");
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericIList() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(IList<Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(IList<Guid>));
        }

        [TestMethod]
        public void CreateSubIndexNameReturnsPrefixPlusIndex() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubIndexName("somePrefix", 2);

            // Assert
            Assert.AreEqual("somePrefix[2]", newName);
        }

        [TestMethod]
        public void CreateSubPropertyNameReturnsPrefixPlusPropertyName() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubPropertyName("somePrefix", "someProperty");

            // Assert
            Assert.AreEqual("somePrefix.someProperty", newName);
        }

        [TestMethod]
        public void CreateSubPropertyNameReturnsPropertyNameIfPrefixIsEmpty() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubPropertyName(String.Empty, "someProperty");

            // Assert
            Assert.AreEqual("someProperty", newName);
        }

        [TestMethod]
        public void CreateSubPropertyNameReturnsPropertyNameIfPrefixIsNull() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubPropertyName(null, "someProperty");

            // Assert
            Assert.AreEqual("someProperty", newName);
        }

        [TestMethod]
        public void GetModelPropertiesFiltersNonUpdateableProperties() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelType = typeof(PropertyTestingModel),
                PropertyFilter = new BindAttribute() { Exclude = "Blacklisted" }.IsPropertyAllowed
            };

            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            PropertyDescriptorCollection properties = helper.PublicGetModelProperties(null, bindingContext);

            // Assert
            Assert.IsNotNull(properties["StringReadWrite"], "StringReadWrite: Read+write string properties are updateable.");
            Assert.IsNull(properties["StringReadOnly"], "StringReadOnly: Read-only string properties are not updateable.");
            Assert.IsNotNull(properties["IntReadWrite"], "IntReadWrite: Read+write ValueType properties are updateable.");
            Assert.IsNull(properties["IntReadOnly"], "IntReadOnly: Read-only string properties are not updateable.");
            Assert.IsNotNull(properties["ArrayReadWrite"], "ArrayReadWrite: Read+write array properties are updateable.");
            Assert.IsNull(properties["ArrayReadOnly"], "ArrayReadOnly: Read-only array properties are not updateable.");
            Assert.IsNotNull(properties["AddressReadWrite"], "AddressReadWrite: Read+write complex properties are updateable.");
            Assert.IsNotNull(properties["AddressReadOnly"], "AddressReadOnly: Read-only complex properties are updateable.");
            Assert.IsNotNull(properties["Whitelisted"], "Whitelisted: Whitelisted properties are updateable.");
            Assert.IsNull(properties["Blacklisted"], "Blacklisted: Blacklisted properties are not updateable.");
            Assert.AreEqual(6, properties.Count, "Incorrect number of properties returned.");
        }

        [TestMethod]
        public void OnModelUpdatedDoesNothingIfModelDoesNotImplementIDataErrorInfo() {
            // Arrange
            MyModel model = new MyModel();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicOnModelUpdated(null, bindingContext);

            // Assert
            Assert.IsTrue(bindingContext.ModelState.IsValidField("themodel.readwriteproperty"), "ModelState should not have been changed.");
        }

        [TestMethod]
        public void OnModelUpdatedDoesNothingIfModelImplementsIDataErrorInfoAndIsValid() {
            // Arrange
            DataErrorInfoProvider model = new DataErrorInfoProvider();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicOnModelUpdated(null, bindingContext);

            // Assert
            Assert.IsTrue(bindingContext.ModelState.IsValidField("themodel"), "ModelState should not have been changed.");
        }

        [TestMethod]
        public void OnModelUpdatedRecordsErrorIfModelImplementsIDataErrorInfoAndIsInvalid() {
            // Arrange
            DataErrorInfoProvider model = new DataErrorInfoProvider() { ErrorText = "Some error message." };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicOnModelUpdated(null, bindingContext);

            // Assert
            Assert.AreEqual("Some error message.", bindingContext.ModelState["themodel"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void OnModelUpdatingReturnsTrue() {
            // By default, this method does nothing, so we just want to make sure it returns true

            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            bool returned = helper.PublicOnModelUpdating(null, null);

            // Arrange
            Assert.IsTrue(returned);
        }

        [TestMethod]
        public void OnPropertyValidatedDoesNothingIfPropertyDoesNotImplementIDataErrorInfo() {
            // Arrange
            MyModel model = new MyModel();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicOnPropertyValidated(null, bindingContext, property, 42);

            // Assert
            Assert.IsTrue(bindingContext.ModelState.IsValidField("themodel.readwriteproperty"), "ModelState should not have been changed.");
        }

        [TestMethod]
        public void OnPropertyValidatedDoesNothingIfPropertyImplementsIDataErrorInfoAndIsValid() {
            // Arrange
            DataErrorInfoProvider model = new DataErrorInfoProvider();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicOnPropertyValidated(null, bindingContext, property, 42);

            // Assert
            Assert.IsTrue(bindingContext.ModelState.IsValidField("themodel.readwriteproperty"), "ModelState should not have been changed.");
        }

        [TestMethod]
        public void OnPropertyValidatedRecordsErrorIfPropertyImplementsIDataErrorInfoAndIsInvalid() {
            // Arrange
            DataErrorInfoProvider model = new DataErrorInfoProvider();
            model.Errors["readwriteproperty"] = "Some error message.";

            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicOnPropertyValidated(null, bindingContext, property, 42);

            // Assert
            Assert.AreEqual("Some error message.", bindingContext.ModelState["themodel.readwriteproperty"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void OnPropertyValidatingReturnsFalseAndCreatesValueRequiredErrorIfNecessary() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            bool returned = helper.PublicOnPropertyValidating(null, bindingContext, property, null);

            // Assert
            Assert.IsFalse(returned, "Value should not have passed validation.");
            Assert.AreEqual("A value is required.", bindingContext.ModelState["theModel.ReadWriteProperty"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void OnPropertyValidatingReturnsFalseIfModelIsAlreadyInvalid() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "theModel"
            };
            bindingContext.ModelState.AddModelError("themodel.readwriteproperty.stuff", "An error.");

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            bool returned = helper.PublicOnPropertyValidating(null, bindingContext, property, null);

            // Assert
            Assert.IsFalse(returned, "Value should not have passed validation.");
            Assert.IsFalse(bindingContext.ModelState.ContainsKey("theModel.ReadWriteProperty"), "Shouldn't have modified ModelState if already contained errors.");
        }

        [TestMethod]
        public void OnPropertyValidatingReturnsTrueOnSuccess() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            bool returned = helper.PublicOnPropertyValidating(null, bindingContext, property, 42);

            // Assert
            Assert.IsTrue(returned, "Value should have passed validation.");
            Assert.AreEqual(0, bindingContext.ModelState.Count);
        }

        [TestMethod]
        public void SetPropertyCapturesAnyExceptionThrown() {
            // Arrange
            MyModelWithBadPropertySetter model = new MyModelWithBadPropertySetter();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(model)["BadInt"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicSetProperty(null, bindingContext, property, 42);

            // Assert
            Assert.AreEqual(@"No earthly integer is valid for this method.
Parameter name: value", bindingContext.ModelState["theModel.BadInt"].Errors[0].Exception.Message);
        }

        [TestMethod]
        public void SetPropertyDoesNothingIfPropertyIsReadOnly() {
            // Arrange
            MyModel model = new MyModel();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(model)["ReadOnlyProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicSetProperty(null, bindingContext, property, 42);

            // Assert
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState should remain untouched.");
        }

        [TestMethod]
        public void SetPropertySuccess() {
            // Arrange
            MyModelWithBadPropertySetter model = new MyModelWithBadPropertySetter();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(model)["NormalInt"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicSetProperty(null, bindingContext, property, 42);

            // Assert
            Assert.AreEqual(42, model.NormalInt);
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState should remain untouched.");
        }

        [TestMethod]
        public void UpdateCollectionCreatesDefaultEntriesForInvalidElements() {
            // Arrange
            List<int> model = new List<int>() { 4, 5, 6, 7, 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "foo",
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "foo[0]", null }, { "foo[1]", null }, { "foo[2]", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                        return (fooIdx == 1) ? (object)null : fooIdx;
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateCollection(null, bindingContext, typeof(int));

            // Assert
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("foo[1]"), "Conversion should have failed.");
            Assert.AreEqual("A value is required.", bindingContext.ModelState["foo[1]"].Errors[0].ErrorMessage, "Error message did not propagate correctly.");
            Assert.AreEqual(0, model[0]);
            Assert.AreEqual(0, model[1]);
            Assert.AreEqual(2, model[2]);
        }

        [TestMethod]
        public void UpdateCollectionReturnsModifiedCollectionOnSuccess() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            List<int> model = new List<int>() { 4, 5, 6, 7, 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new Dictionary<string, ValueProviderResult>() { { "foo[0]", null }, { "foo[1]", null }, { "foo[2]", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateCollection(controllerContext, bindingContext, typeof(int));

            // Assert
            Assert.AreSame(model, updatedModel, "Should have updated the provided model object.");
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual(0, model[0]);
            Assert.AreEqual(1, model[1]);
            Assert.AreEqual(2, model[2]);
        }

        [TestMethod]
        public void UpdateCollectionReturnsNullIfZeroIndexNotFound() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ValueProvider = new Dictionary<string, ValueProviderResult>()
            };
            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.UpdateCollection(null, bindingContext, typeof(object));

            // Assert
            Assert.IsNull(updatedModel, "Method should return null if no values exist as part of the request.");
        }

        [TestMethod]
        public void UpdateDictionaryCreatesDefaultEntriesForInvalidValues() {
            // Arrange
            Dictionary<string, int> model = new Dictionary<string, int>{
                { "one", 1 },
                { "two", 2 }
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "foo",
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                        return (fooIdx == 1) ? (object)null : fooIdx;
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockStringBinder.Object },
                    { typeof(int), mockIntBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateDictionary(null, bindingContext, typeof(string), typeof(int));

            // Assert
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("foo[1].value"), "Conversion should have failed.");
            Assert.AreEqual("A value is required.", bindingContext.ModelState["foo[1].value"].Errors[0].ErrorMessage, "Error message did not propagate correctly.");
            Assert.AreEqual(0, model["10Value"]);
            Assert.AreEqual(0, model["11Value"]);
            Assert.AreEqual(2, model["12Value"]);
        }

        [TestMethod]
        public void UpdateDictionaryReturnsModifiedDictionaryOnSuccess() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            Dictionary<int, string> model = new Dictionary<int, string>{
                { 1, "one" },
                { 2, "two" }
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(new ModelBindingContext().PropertyFilter, bc.PropertyFilter, "PropertyFilter should not have been set.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10;
                    });

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(string), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockIntBinder.Object },
                    { typeof(string), mockStringBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateDictionary(controllerContext, bindingContext, typeof(int), typeof(string));

            // Assert
            Assert.AreSame(model, updatedModel, "Should have updated the provided model object.");
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual("10Value", model[10]);
            Assert.AreEqual("11Value", model[11]);
            Assert.AreEqual("12Value", model[12]);
        }

        [TestMethod]
        public void UpdateDictionaryReturnsNullIfNoValidElementsFound() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ValueProvider = new Dictionary<string, ValueProviderResult>()
            };
            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.UpdateDictionary(null, bindingContext, typeof(object), typeof(object));

            // Assert
            Assert.IsNull(updatedModel, "Method should return null if no values exist as part of the request.");
        }

        [TestMethod]
        public void UpdateDictionarySkipsInvalidKeys() {
            // Arrange
            Dictionary<int, string> model = new Dictionary<int, string>{
                { 1, "one" },
                { 2, "two" }
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                Model = model,
                ModelName = "foo",
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                        return (fooIdx == 1) ? (object)null : fooIdx;
                    });

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockIntBinder.Object },
                    { typeof(string), mockStringBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateDictionary(null, bindingContext, typeof(int), typeof(string));

            // Assert
            Assert.AreEqual(2, model.Count, "Model is not of correct length.");
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("foo[1].key"), "Conversion should have failed.");
            Assert.AreEqual("A value is required.", bindingContext.ModelState["foo[1].key"].Errors[0].ErrorMessage, "Error message did not propagate correctly.");
            Assert.AreEqual("10Value", model[0]);
            Assert.AreEqual("12Value", model[2]);
        }

        [ModelBinder(typeof(DefaultModelBinder))]
        private class MyModel {
            public int ReadOnlyProperty {
                get { return 4; }
            }
            public int ReadWriteProperty { get; set; }
            public int ReadWriteProperty2 { get; set; }
        }

        private class DataErrorInfoProvider : IDataErrorInfo {
            private Dictionary<string, string> _errors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            public string ErrorText { get; set; }
            public Dictionary<string, string> Errors {
                get {
                    return _errors;
                }
            }

            #region IDataErrorInfo Members
            string IDataErrorInfo.Error {
                get {
                    return ErrorText ?? String.Empty;
                }
            }
            string IDataErrorInfo.this[string columnName] {
                get {
                    string thisError;
                    Errors.TryGetValue(columnName, out thisError);
                    return thisError ?? String.Empty;
                }
            }
            #endregion
        }

        private class MyModelWithBadPropertySetter {
            public int NormalInt { get; set; }
            public int BadInt {
                get {
                    return 0;
                }
                set {
                    throw new ArgumentOutOfRangeException("value", "No earthly integer is valid for this method.");
                }
            }
        }

        private class MyClassWithoutConverter {
        }

        [Bind(Exclude = "Alpha,Echo")]
        private class MyOtherModel {
            public string Alpha { get; set; }
            public string Bravo { get; set; }
            public string Charlie { get; set; }
            public string Delta { get; set; }
            public string Echo { get; set; }
            public string Foxtrot { get; set; }
        }

        public class Customer {
            private Address _address = new Address();
            public Address Address {
                get {
                    return _address;
                }
            }
        }

        public class Address {
            public string Street { get; set; }
            public string Zip { get; set; }
        }

        public class IntegerContainer {
            public int Integer { get; set; }
            public int? NullableInteger { get; set; }
        }

        [TypeConverter(typeof(CultureAwareConverter))]
        public class StringContainer {
            public StringContainer(string value) {
                Value = value;
            }
            public string Value {
                get;
                private set;
            }
        }

        private class CultureAwareConverter : TypeConverter {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
                return (sourceType == typeof(string));
            }
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
                return (destinationType == typeof(string));
            }
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
                string stringValue = value as string;
                if (stringValue == null || stringValue.Length < 3) {
                    throw new Exception("Value must have at least 3 characters.");
                }
                return new StringContainer(AppendCultureName(stringValue, culture));
            }
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
                StringContainer container = value as StringContainer;
                if (container.Value == null || container.Value.Length < 3) {
                    throw new Exception("Value must have at least 3 characters.");
                }

                return AppendCultureName(container.Value, culture);
            }

            private static string AppendCultureName(string value, CultureInfo culture) {
                string cultureName = (!String.IsNullOrEmpty(culture.Name)) ? culture.Name : culture.ThreeLetterWindowsLanguageName;
                return value + " (" + cultureName + ")";
            }
        }

        [ModelBinder(typeof(MyStringModelBinder))]
        private class MyStringModel {
            public string Value { get; set; }
        }

        private class MyStringModelBinder : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                MyStringModel castModel = bindingContext.Model as MyStringModel;
                if (castModel != null) {
                    castModel.Value += "_Update";
                }
                else {
                    castModel = new MyStringModel() { Value = bindingContext.ModelName + "_Create" };
                }
                return castModel;
            }
        }

        public class DefaultModelBinderHelper : DefaultModelBinder {
            public virtual void PublicBindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property) {
                base.BindProperty(controllerContext, bindingContext, property);
            }
            protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property) {
                PublicBindProperty(controllerContext, bindingContext, property);
            }
            public virtual object PublicCreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType) {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
            protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType) {
                return PublicCreateModel(controllerContext, bindingContext, modelType);
            }
            public virtual PropertyDescriptorCollection PublicGetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return base.GetModelProperties(controllerContext, bindingContext);
            }
            protected override PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return PublicGetModelProperties(controllerContext, bindingContext);
            }
            public string PublicCreateSubIndexName(string prefix, int indexName) {
                return CreateSubIndexName(prefix, indexName);
            }
            public string PublicCreateSubPropertyName(string prefix, string propertyName) {
                return CreateSubPropertyName(prefix, propertyName);
            }
            public virtual bool PublicOnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return base.OnModelUpdating(controllerContext, bindingContext);
            }
            protected override bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return PublicOnModelUpdating(controllerContext, bindingContext);
            }
            public virtual void PublicOnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                base.OnModelUpdated(controllerContext, bindingContext);
            }
            protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                PublicOnModelUpdated(controllerContext, bindingContext);
            }
            public virtual bool PublicOnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                return base.OnPropertyValidating(controllerContext, bindingContext, property, value);
            }
            protected override bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                return PublicOnPropertyValidating(controllerContext, bindingContext, property, value);
            }
            public virtual void PublicOnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                base.OnPropertyValidated(controllerContext, bindingContext, property, value);
            }
            protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                PublicOnPropertyValidated(controllerContext, bindingContext, property, value);
            }
            public virtual void PublicSetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                base.SetProperty(controllerContext, bindingContext, property, value);
            }
            protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                PublicSetProperty(controllerContext, bindingContext, property, value);
            }
        }

        private class MyModel2 {

            private int _intReadWriteNonNegative;

            public int IntReadOnly { get { return 4; } }
            public int IntReadWrite { get; set; }
            public int IntReadWriteNonNegative {
                get {
                    return _intReadWriteNonNegative;
                }
                set {
                    if (value < 0) {
                        throw new ArgumentOutOfRangeException("value", "Value must be non-negative.");
                    }
                    _intReadWriteNonNegative = value;
                }
            }
            public int? NullableIntReadWrite { get; set; }

        }

        [Bind(Exclude = "Foo")]
        private class ModelWithBindAttribute : ModelWithoutBindAttribute {
        }

        private class ModelWithoutBindAttribute {
            public string Foo { get; set; }
            public string Bar { get; set; }
            public string Baz { get; set; }
        }

        private class PropertyTestingModel {
            public string StringReadWrite { get; set; }
            public string StringReadOnly { get; private set; }
            public int IntReadWrite { get; set; }
            public int IntReadOnly { get; private set; }
            public object[] ArrayReadWrite { get; set; }
            public object[] ArrayReadOnly { get; private set; }
            public Address AddressReadWrite { get; set; }
            public Address AddressReadOnly { get; private set; }
            public string Whitelisted { get; set; }
            public string Blacklisted { get; set; }
        }

    }
}
