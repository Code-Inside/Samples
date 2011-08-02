using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentInterfaces.Services;
using FluentInterfaces.Data;
namespace FluentInterfaces.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PersonService srv = new PersonService();
            List<Person> resultList = srv.GetPersons(20, "R").ToList();
            foreach (Person result in resultList)
            {
                Console.WriteLine("Name {0} - Age {1}", result.Name, result.Age);
            }

            Console.Read();
        }
    }
}
