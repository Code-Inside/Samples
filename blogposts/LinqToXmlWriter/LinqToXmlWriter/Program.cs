using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LinqToXmlWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            XElement outPutElement = new XElement("root", 
                                        new XElement("simpleItem",
                                            new XElement("title", "Code-Inside.de"),
                                            new XElement("subtitle", "Bester Blog der Welt!"),
                                            new XElement("authors",
                                                new XElement("name", "Robert Mühsig"),
                                                new XElement("name", "Oliver Guhr")
                                            )),
                                        new XElement("anotherItem",
                                            new XAttribute("id", 121),
                                            new XElement("foo", "bar")
                                            )
                                       );

            Console.Write(outPutElement.ToString());
            Console.ReadLine();
        }
    }
}
