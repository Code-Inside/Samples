namespace System.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionMethodDispatcherTest {

        [TestMethod]
        public void ExecuteWithNormalActionMethod() {
            // Arrange
            DispatcherController controller = new DispatcherController();
            object[] parameters = new object[] { 5, "some string", new DateTime(2001, 1, 1) };
            MethodInfo methodInfo = typeof(DispatcherController).GetMethod("NormalAction");
            ActionMethodDispatcher dispatcher = new ActionMethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(controller, parameters);

            // Assert
            Assert.IsInstanceOfType(returnValue, typeof(string));
            string stringResult = (string)returnValue;
            Assert.AreEqual("Hello from NormalAction!", stringResult);

            Assert.AreEqual(5, controller._i);
            Assert.AreEqual("some string", controller._s);
            Assert.AreEqual(new DateTime(2001, 1, 1), controller._dt);
        }

        [TestMethod]
        public void ExecuteWithParameterlessActionMethod() {
            // Arrange
            DispatcherController controller = new DispatcherController();
            object[] parameters = new object[0];
            MethodInfo methodInfo = typeof(DispatcherController).GetMethod("ParameterlessAction");
            ActionMethodDispatcher dispatcher = new ActionMethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(controller, parameters);

            // Assert
            Assert.IsInstanceOfType(returnValue, typeof(int));
            int intResult = (int)returnValue;
            Assert.AreEqual(53, intResult);
        }

        [TestMethod]
        public void ExecuteWithStaticActionMethod() {
            // Arrange
            DispatcherController controller = new DispatcherController();
            object[] parameters = new object[0];
            MethodInfo methodInfo = typeof(DispatcherController).GetMethod("StaticAction");
            ActionMethodDispatcher dispatcher = new ActionMethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(controller, parameters);

            // Assert
            Assert.IsInstanceOfType(returnValue, typeof(int));
            int intResult = (int)returnValue;
            Assert.AreEqual(89, intResult);
        }

        [TestMethod]
        public void ExecuteWithVoidActionMethod() {
            // Arrange
            DispatcherController controller = new DispatcherController();
            object[] parameters = new object[] { 5, "some string", new DateTime(2001, 1, 1) };
            MethodInfo methodInfo = typeof(DispatcherController).GetMethod("VoidAction");
            ActionMethodDispatcher dispatcher = new ActionMethodDispatcher(methodInfo);

            // Act
            object returnValue = dispatcher.Execute(controller, parameters);

            // Assert
            Assert.IsNull(returnValue);
            Assert.AreEqual(5, controller._i);
            Assert.AreEqual("some string", controller._s);
            Assert.AreEqual(new DateTime(2001, 1, 1), controller._dt);
        }

        [TestMethod]
        public void MethodInfoProperty() {
            // Arrange
            MethodInfo original = typeof(object).GetMethod("ToString");
            ActionMethodDispatcher dispatcher = new ActionMethodDispatcher(original);

            // Act
            MethodInfo returned = dispatcher.MethodInfo;

            // Assert
            Assert.AreSame(original, returned);
        }

        private class DispatcherController : Controller {

            public int _i;
            public string _s;
            public DateTime _dt;

            public object NormalAction(int i, string s, DateTime dt) {
                VoidAction(i, s, dt);
                return "Hello from NormalAction!";
            }

            public int ParameterlessAction() {
                return 53;
            }

            public void VoidAction(int i, string s, DateTime dt) {
                _i = i;
                _s = s;
                _dt = dt;
            }

            public static int StaticAction() {
                return 89;
            }

        }

    }
}
