using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace YamlSample
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoConfig sample = new DemoConfig();

            sample.SimpleItem = "Hello World!";

            sample.Foo = 1337;

            sample.SimpleList = new List<string>();
            sample.SimpleList.Add("Foobar 1");
            sample.SimpleList.Add("Foobar 2");
            sample.SimpleList.Add("Foobar 3");
            sample.SimpleList.Add("Foobar 4");

            sample.ComplexList = new List<DemoConfigSettingElement>();

            DemoConfigSettingElement element1 = new DemoConfigSettingElement();
            element1.Name = "Element 1";
            element1.Attributes = new List<string>();
            element1.Attributes.Add("Foobarbuzz 1");
            element1.Attributes.Add("Foobarbuzz 2");
            element1.Attributes.Add("Foobarbuzz 3");
            element1.Attributes.Add("Foobarbuzz 4");
            element1.Attributes.Add("Foobarbuzz 5");
            sample.ComplexList.Add(element1);


            DemoConfigSettingElement element2 = new DemoConfigSettingElement();
            element2.Name = "Element 2";
            element2.Attributes = new List<string>();
            element2.Attributes.Add("Foobarbuzzi 1");
            element2.Attributes.Add("Foobarbuzzi 2");
            element2.Attributes.Add("Foobarbuzzi 3");
            element2.Attributes.Add("Foobarbuzzi 4");
            element2.Attributes.Add("Foobarbuzzi 5");
            element2.Test = "Only defined in element2";
            sample.ComplexList.Add(element2);

            YamlDotNet.Serialization.Serializer serializer = new Serializer();
            StringWriter strWriter = new StringWriter();

            serializer.Serialize(strWriter, sample);
            serializer.Serialize(Console.Out, sample);

            using (TextWriter writer = File.CreateText("test.yml"))
            {
                writer.Write(strWriter.ToString());
            }

            using (TextReader reader = File.OpenText(@"test.yml"))
            {

                Deserializer deserializer = new Deserializer();
                var configFromFile = deserializer.Deserialize<DemoConfig>(reader);
            }


            Console.Read();

        }
    }

    public class DemoConfig
    {
        public int Foo { get; set; }
        public string SimpleItem { get; set; }
        public List<string> SimpleList { get; set; }
        public List<DemoConfigSettingElement> ComplexList { get; set; }
    }

    public class DemoConfigSettingElement
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public string Test { get; set; }
    }


}
