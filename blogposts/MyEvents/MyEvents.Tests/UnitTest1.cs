using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyEvents.ConsoleApp;
namespace MyEvents.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ConnectionManagerTest
    {
        public ConnectionManagerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void ConnectionManager_Raise_StateChanged_Event()
        {
            ConnectionManager man = new ConnectionManager();
            Assert.AreEqual(ConnectionStates.Disconnected, man.State);

            bool eventRaised = false;

            man.StateChanged += delegate(object sender, StateChangedEventArgs args)
            {
                eventRaised = true;
            };
            man.State = ConnectionStates.Connecting;

            Assert.IsTrue(eventRaised);
        }
    }
}
