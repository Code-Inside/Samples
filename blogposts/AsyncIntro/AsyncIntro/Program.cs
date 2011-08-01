using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncIntro
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomerRepository rep = new CustomerRepository();

            Console.WriteLine("Sync: A | Async: B");
            string choice = Console.ReadLine();

            if(choice.ToLower() == "a")
            {
                DisplayCustomers(rep.GetCustomers());
            }
            if (choice.ToLower() == "b")
            {
                rep.GetCustomersAsyncCompleted += new EventHandler<GenericEventArgs<List<Customer>>>(rep_GetCustomersAsyncCompleted);
                rep.GetCustomersAsync();
            }

            Console.ReadLine();
        }

        static void rep_GetCustomersAsyncCompleted(object sender, GenericEventArgs<List<Customer>> e)
        {
            DisplayCustomers(e.EventArgs);
        }

        static void DisplayCustomers(List<Customer> customers)
        {
            Console.WriteLine("Finished:");
            foreach (Customer customer in customers)
            {
                Console.WriteLine("+++");
                Console.WriteLine("Customer Name: " + customer.Name);
                Console.WriteLine("Customer Address: " + customer.Address);
                Console.WriteLine("Customer ID: " + customer.Id.ToString());
            }
        }
    }


}
