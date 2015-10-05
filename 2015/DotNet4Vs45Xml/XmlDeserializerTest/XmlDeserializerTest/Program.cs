using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlDeserializerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            StaticContainerClass.XmlContainer toBeDeserialized = new StaticContainerClass.XmlContainer();

            toBeDeserialized.Entries = new List<StaticContainerClass.XmlContainerEntry>();
            toBeDeserialized.Entries.Add(new StaticContainerClass.XmlContainerEntry() { Id = Guid.NewGuid()});


            try
            {
                using (XmlTextWriter writer = new XmlTextWriter("test.xml", Encoding.UTF8))
                {
                    var serializer = new XmlSerializer(typeof(StaticContainerClass.XmlContainer));
                    serializer.Serialize(writer, toBeDeserialized);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }
    }

    public static class StaticContainerClass
    {
        [XmlType(AnonymousType = true)]
        [XmlRoot(Namespace = "", IsNullable = false, ElementName = "container")]
        public class XmlContainer
        {
            public XmlContainer()
            {
                Entries = new List<XmlContainerEntry>();
            }

            [XmlElement("entry")]
            public List<XmlContainerEntry> Entries { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class XmlContainerEntry
        {
            [XmlAttribute("id")]
            public Guid Id { get; set; }
        }
        
    }
}
