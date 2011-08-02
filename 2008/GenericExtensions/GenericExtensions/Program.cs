using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericExtensions
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(2);
            intList.Add(3);

            List<int> newIntList = new List<int>();
            newIntList.Add(4);
            newIntList.Add(5);

            intList.Add(newIntList);

            foreach (int myInt in intList)
            {
                Console.WriteLine(myInt); // Should be 1,2,3,4,5
            }
           
            Console.ReadLine(); 
        }
    }
}
