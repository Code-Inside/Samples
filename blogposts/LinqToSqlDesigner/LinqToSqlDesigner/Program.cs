using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace LinqToSqlDesigner
{
    class Program
    {
        static void Main(string[] args)
        {
            NorthwindDataContext context = new NorthwindDataContext();
            Table<Customer> customerTable = context.GetTable<Customer>();

            var cust = from c in customerTable
                       where c.City == "London"
                       select c;

            foreach(Customer customer in cust)
            {
                Console.WriteLine(customer.CompanyName);
                if(customer.Orders.Count > 0)
                {
                    Console.WriteLine("Customer Orders");
                    foreach(Order order in customer.Orders)
                    {
                        if(order.Order_Details.Count > 0)
                        {
                            Console.WriteLine(" - " + order.Order_Details[0].Product.ProductName);
                        }
                    }

                }
            }

            Console.ReadLine();
        }
    }
}
