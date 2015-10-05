using System;
using System.Collections.Generic;
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
            FileStream readFileStream = new FileStream(@"C:\Users\Robert\Documents\visual studio 2015\Projects\XmlBaseTest\XmlBaseTest\test.xml", FileMode.Open, FileAccess.Read, FileShare.Read);

            XmlSerializer serializer = new XmlSerializer(typeof(View));
            var test = serializer.Deserialize(readFileStream);

        }
    }

    public interface IAmBindable
    {
        string Bind { get; set; }
    }

    public class Row : IAmBindable
    {
        [XmlAttribute(AttributeName = "Bind")]
        public string Bind { get; set; }

        [XmlElement(typeof(Label))]
        [XmlElement(typeof(TextBox))]
        [XmlElement(typeof(Button))]
        public List<BaseControl> Controls { get; set; }
    }

    public class BaseControl
    {
        [XmlAttribute(AttributeName = "ColumnSpan")]
        public string ColumnSpan { get; set; }
    }

    public class Label : BaseControl, IAmBindable
    {
        [XmlText]
        public string Content { get; set; }

        [XmlAttribute(AttributeName = "Bind")]
        public string Bind { get; set; }
    }

    public class TextBox : BaseControl, IAmBindable
    {
        [XmlAttribute(AttributeName = "Bind")]
        public string Bind { get; set; }
    }

    public class Button : BaseControl, IAmBindable
    {
        [XmlAttribute(AttributeName = "Label")]
        public string Label { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "TargetView")]
        public string TargetView { get; set; }

        [XmlAttribute(AttributeName = "Bind")]
        public string Bind { get; set; }
    }

    public class View
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "Row")]
        public List<Row> Rows { get; set; }
    }
}
