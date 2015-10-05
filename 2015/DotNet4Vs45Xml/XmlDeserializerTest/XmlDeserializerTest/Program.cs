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
    /// <summary>
    /// App = Compiled for .NET 4.0 and is running OK on .NET 4.5, but breaks on 4.0.
    /// 
    /// Kinda similar:
    /// http://stackoverflow.com/questions/14689305/serialization-breaks-in-net-4-5
    /// 
    /// .NET 4.5 == OK
    /// 
    /// .NET 4.0 == Exception
    /// 
    /// System.InvalidOperationException: XmlDeserializerTest.StaticContainerClass kann
    /// nicht serialisiert werden.Statische Typen können nicht als Parameter oder Rückg
    /// abewerte verwendet werden.
    /// bei System.Xml.Serialization.TypeScope.GetTypeDesc(Type type, MemberInfo sour
    /// ce, Boolean directReference, Boolean throwOnError)
    /// bei System.Xml.Serialization.TypeScope.GetTypeDesc(Type type, MemberInfo sour
    /// ce, Boolean directReference)
    /// bei System.Xml.Serialization.TypeScope.ImportTypeDesc(Type type, MemberInfo m
    /// emberInfo, Boolean directReference)
    /// bei System.Xml.Serialization.TypeScope.GetTypeDesc(Type type, MemberInfo sour
    /// ce, Boolean directReference, Boolean throwOnError)
    /// bei System.Xml.Serialization.ModelScope.GetTypeModel(Type type, Boolean direc
    /// tReference)
    /// bei System.Xml.Serialization.XmlReflectionImporter.ImportTypeMapping(Type typ
    /// e, XmlRootAttribute root, String defaultNamespace)
    /// bei System.Xml.Serialization.XmlSerializer..ctor(Type type, String defaultNam
    /// espace)
    /// bei XmlDeserializerTest.Program.Main(String[] args)
    /// </summary>
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
