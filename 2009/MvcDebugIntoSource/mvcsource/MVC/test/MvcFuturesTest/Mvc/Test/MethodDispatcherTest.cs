namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class MethodDispatcherTest {

        [TestMethod]
        public void ExecuteWithNormalInstanceMethod() {
            // Arrange
            MethodContainer container = new MethodContainer();
            object[] parameters = new object[] { 5, "some string", new DateTime(2001, 1, 1) };
            MethodInfo methodInfo = typeof(MethodContainer).GetMethod("NormalMethod");
            MethodDispatcher dispatcher = new MethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(container, parameters);

            // Assert
            Assert.AreEqual("Hello from NormalMethod!", returnValue);

            Assert.AreEqual(5, container._i);
            Assert.AreEqual("some string", container._s);
            Assert.AreEqual(new DateTime(2001, 1, 1), container._dt);
        }

        [TestMethod]
        public void ExecuteWithParameterlessInstanceMethod() {
            // Arrange
            MethodContainer container = new MethodContainer();
            object[] parameters = new object[0];
            MethodInfo methodInfo = typeof(MethodContainer).GetMethod("ParameterlessMethod");
            MethodDispatcher dispatcher = new MethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(container, parameters);

            // Assert
            Assert.AreEqual(53, returnValue);
        }

        [TestMethod]
        public void ExecuteWithStaticMethod() {
            // Arrange
            MethodContainer container = new MethodContainer();
            object[] parameters = new object[0];
            MethodInfo methodInfo = typeof(MethodContainer).GetMethod("StaticMethod");
            MethodDispatcher dispatcher = new MethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(container, parameters);

            // Assert
            Assert.AreEqual(89, returnValue);
        }

        [TestMethod]
        public void ExecuteWithVoidInstanceMethod() {
            // Arrange
            MethodContainer container = new MethodContainer();
            object[] parameters = new object[] { 5, "some string", new DateTime(2001, 1, 1) };
            MethodInfo methodInfo = typeof(MethodContainer).GetMethod("VoidMethod");
            MethodDispatcher dispatcher = new MethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(container, parameters);

            // Assert
            Assert.IsNull(returnValue);
            Assert.AreEqual(5, container._i);
            Assert.AreEqual("some string", container._s);
            Assert.AreEqual(new DateTime(2001, 1, 1), container._dt);
        }

        [TestMethod]
        public void MethodInfoProperty() {
            // Arrange
            MethodInfo original = typeof(object).GetMethod("ToString");
            MethodDispatcher dispatcher = new MethodDispatcher(original);

            // Act
            MethodInfo returned = dispatcher.MethodInfo;

            // Assert
            Assert.AreSame(original, returned);
        }

        private class MethodContainer {

            public int _i;
            public string _s;
            public DateTime _dt;

            public object NormalMethod(int i, string s, DateTime dt) {
                VoidMethod(i, s, dt);
                return "Hello from NormalMethod!";
            }

            public int ParameterlessMethod() {
                return 53;
            }

            public void VoidMethod(int i, string s, DateTime dt) {
                _i = i;
                _s = s;
                _dt = dt;
            }

            public static int StaticMethod() {
                return 89;
            }

        }

    }
}
