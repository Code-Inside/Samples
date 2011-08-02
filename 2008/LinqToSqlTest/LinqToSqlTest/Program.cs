using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace LinqToSqlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use a standard connection string
            DataContext db = new DataContext(@"Data Source=REMAN-NOTEBOOK\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True");
            Table<Customer> Customers = db.GetTable<Customer>();

            // Logging
            db.Log = Console.Out;

            // CREATE
            Customer newCostumer = new Customer();
            newCostumer.City = "Dresden";
            newCostumer.Address = "Riesaer Str. 5";
            newCostumer.CustomerID = "DDMMS";
            newCostumer.CompanyName = "T-Systems MMS";

            Customers.InsertOnSubmit(newCostumer);
            db.SubmitChanges();
            
            // READ
            var custs =
                from c in Customers
                where c.City == "Dresden"
                select c;

            // Debugging - Console.WriteLine
            foreach (var cust in custs)
            {
                Console.WriteLine("ID={0}, City={1}, Address={2}", cust.CustomerID, cust.City, cust.Address);
            }
            
            // UPDATE
            var upObjects = from test in Customers
                             where test.CompanyName == "T-Systems MMS"
                             select test;

            upObjects.First().Address = "Straße";
            db.SubmitChanges();

            // Debugging - Console.WriteLine
            foreach (var cust in custs)
            {
                Console.WriteLine("ID={0}, City={1}, Address={2}", cust.CustomerID, cust.City, cust.Address);
            }

            // DEL
            var delObjects = from test in Customers 
                             where test.CompanyName == "T-Systems MMS"
                             select test;

            Customers.DeleteOnSubmit(delObjects.First());
            db.SubmitChanges();
            
            Console.ReadLine();
        }
    }
}
