using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayWithEffort;

namespace Testing
{
   [TestClass]
    public class SimpleTest
    {
        public DemoContext Context { get; set; }

        public DbConnection DbConnection { get; set; }

        public void FixEfProviderServicesProblem()
        {
            //The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
            //for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. 
            //Make sure the provider assembly is available to the running application. 
            //See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.

            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public bool IsInMemory { get; private set; }

        public void CreateInMemoryDb()
        {
            DbConnection = Effort.DbConnectionFactory.CreatePersistent(Guid.NewGuid().ToString());
            this.Context = new DemoContext(DbConnection);
            this.IsInMemory = true;
        }

        [TestMethod]
        public void Context_Created()
        {
            this.CreateInMemoryDb();
            Assert.IsNotNull(Context.Database);
        }


        [TestMethod]
        public void Invoke_Works()
        {

            this.CreateInMemoryDb();
            var test = this.Context.Blogs.ToList();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Save_Works()
        {

            this.CreateInMemoryDb();
            this.Context.Blogs.Add(Builder<Blog>.CreateNew().Build());
            this.Context.Blogs.Add(Builder<Blog>.CreateNew().Build());
            this.Context.Blogs.Add(Builder<Blog>.CreateNew().Build());
            this.Context.SaveChanges();

            using (new DemoContext(this.DbConnection))
            {
                Assert.IsTrue(this.Context.Blogs.Count() == 3);    
            }
        }
    }
}
