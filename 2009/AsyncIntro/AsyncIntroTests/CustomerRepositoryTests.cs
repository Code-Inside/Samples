using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncIntro;
using System.Threading;

namespace AsyncIntroTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CustomerRepositoryTests
    {
        [TestMethod]
        public void GetCustomersAsync_Raised_GetCustomersAsyncCompleted_Event()
        {
            CustomerRepository rep = new CustomerRepository();
            AutoResetEvent reset = new AutoResetEvent(false);
            bool eventRaised = false;

            rep.GetCustomersAsyncCompleted += delegate(object sender, GenericEventArgs<List<Customer>> args)
            {
                reset.Set();
                eventRaised = true;
            };
            rep.GetCustomersAsync();

            reset.WaitOne();
            Assert.IsTrue(eventRaised);
        }
    }
}
