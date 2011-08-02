using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreeTier.Data.DataAccess.DemoRepository;
using ThreeTier.Data.DataAccess;
using ThreeTier.Data;
namespace ThreeTier.Tests.Data
{
    /// <summary>
    /// Summary description for DemoUserRepository
    /// </summary>
    [TestClass]
    public class DemoUserRepositoryTest
    {
        IUserRepository rep;

        [TestMethod]
        public void DemoUserRepository_GetUsers_Should_Return_Values()
        {
            rep = new DemoUserRepository();
            List<User> users = rep.GetUsers().ToList();
            Assert.IsNotNull(users);
            Assert.AreNotEqual(0, users.Count);
        }

        [TestMethod]
        public void DemoUserRepository_GetUser_Should_Return_Null_On_Invalid_User()
        {
            rep = new DemoUserRepository();
            User user = rep.GetUser("hallo");
            Assert.IsNull(user);
        }

        [TestMethod]
        public void DemoUserRepository_GetUser_Should_Return_Data_On_Valid_User()
        {
            rep = new DemoUserRepository();
            User user = rep.GetUser("Robert");
            Assert.IsNotNull(user);
            Assert.AreEqual("Robert", user.Login);
        }       
    }
}
