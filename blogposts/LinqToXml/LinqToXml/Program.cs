using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqToXml
{
    class Program
    {
        static void Main(string[] args)
        {
            XDocument xmlDoc = XDocument.Load("../../Sample.xml");

            Console.WriteLine();
            Console.WriteLine("Simple: ");
            var simpleQuery = from simpleItem in xmlDoc.Descendants("Item")
                              select simpleItem;

            foreach (XElement element in simpleQuery)
            {
                Console.WriteLine(element.Value);
            }

            Console.WriteLine();
            Console.WriteLine("Simple + WHERE: ");
            var simpleWhereQuery = from simpleItem in xmlDoc.Descendants("SimpleItems").Descendants("Item")
                                   where Convert.ToInt32(simpleItem.Element("Number").Value) > 55
                                   select simpleItem;

            foreach (XElement element in simpleWhereQuery)
            {
                Console.WriteLine(element.Value);
            }

            Console.WriteLine();
            Console.WriteLine("Complex: ");
            var complexQuery = from complexItem in xmlDoc.Descendants("ComplexItems").Descendants("Item")
                                   where complexItem.Attribute("marketplace").Value == "fr" &&
                                   Convert.ToInt32(complexItem.Element("Number").Value) > 20
                                   select complexItem;

            foreach (XElement element in complexQuery)
            {
                Console.WriteLine(element.Value);
            }
             Console.ReadLine();
        }
    }
}
