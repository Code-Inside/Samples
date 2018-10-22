using Microsoft.Isam.Esent.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsentSample.Dictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            PersistentDictionary<string, string> dictionary = new PersistentDictionary<string, string>("Names");
            
            Console.WriteLine("What is your first name?");
            string firstName = Console.ReadLine();
            if (dictionary.ContainsKey(firstName))
            {
                Console.WriteLine("Welcome back {0} {1}", firstName, dictionary[firstName]);
            }
            else
            {
                Console.WriteLine("I don't know you, {0}. What is your last name?", firstName);
                dictionary[firstName] = Console.ReadLine();
            }
        }
    }
}
