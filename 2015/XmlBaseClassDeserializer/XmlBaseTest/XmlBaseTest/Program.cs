using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlBaseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream readFileStream = new FileStream(@"test.xml", FileMode.Open, FileAccess.Read, FileShare.Read);

            XmlSerializer serializer = new XmlSerializer(typeof(Root));
            var test = serializer.Deserialize(readFileStream);

        }
    }
}
