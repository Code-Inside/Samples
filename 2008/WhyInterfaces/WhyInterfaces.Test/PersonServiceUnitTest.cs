using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhyInterfaces.Service;
using WhyInterfaces.Test.Stubs;

namespace WhyInterfaces.Test
{
    /// <summary>
    /// Summary description for PersonServiceUnitTest
    /// </summary>
    [TestClass]
    public class PersonServiceUnitTest
    {
        [TestMethod]
        public void PersonService_ReturnPersons_Work_With_Authentication()
        {
            TestPersonDataProvider provider = new TestPersonDataProvider();
            PersonService srv = new PersonService(new TrueTestAuthenticationService(), provider);
            Assert.AreEqual(provider.InMemoryPersonCollection.Count, srv.GetPersons().Count);
        }

        [TestMethod]
        public void PersonService_Should_Return_Null_Without_Authentication()
        {
            TestPersonDataProvider provider = new TestPersonDataProvider();
            PersonService srv = new PersonService(new FalseTestAuthenticationService(), provider);
            Assert.IsNull(srv.GetPersons());
        }
    }
}
