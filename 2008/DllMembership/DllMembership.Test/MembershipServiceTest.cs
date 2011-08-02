using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DllMembership.Lib;
using System.Web;
using System.Web.Security;

namespace DllMembership.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MembershipServiceTest
    {
        private MembershipService service;

        public MembershipServiceTest()
        {
            this.service = new MembershipService();
        }
        [TestMethod]
        public void MembershipService_Retrieve_MembershipUsers_Works()
        {
           List<User> col = service.GetUsers().ToList();
           Assert.IsNotNull(col);
           Assert.AreNotEqual(0, col.Count);

           foreach (User data in col)
           {
               Assert.AreNotEqual("", data.Name);
           }
        }

        [TestMethod]
        public void MembershipService_Users_Can_Login()
        {
            User data = service.Login("Robert", "Membership2008!");
            Assert.IsTrue(data.IsLoggedIn);
        }

        [TestMethod]
        public void MembershipService_Get_One_User()
        {
            User user = service.GetUser("Robert");
            
            Assert.AreEqual("Robert", user.Name);
        }
    }
}
