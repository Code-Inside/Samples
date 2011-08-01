using DoNot.Fear.UnitTests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DoNot.Fear.UnitTests.Test
{
    
    
    /// <summary>
    ///This is a test class for DataManagerTest and is intended
    ///to contain all DataManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DataManagerTest
    {
        [TestMethod()]
        public void DataManager_ConnectToData_IsTrue()
        {
            DataManager man = new DataManager();
            Assert.IsTrue(man.ConnectToData());
        }

        [TestMethod()]
        public void DataManager_GetData_IsNotNull()
        {
            DataManager man = new DataManager();
            Assert.IsNotNull(man.GetData());
        }

        [TestMethod()]
        public void DataManager_GetData_CheckForZero()
        {
            DataManager man = new DataManager();
            List<int> result = man.GetData();
            foreach (int number in result)
            {
                Assert.AreNotEqual(0, number);
            }
        }

    }
}
