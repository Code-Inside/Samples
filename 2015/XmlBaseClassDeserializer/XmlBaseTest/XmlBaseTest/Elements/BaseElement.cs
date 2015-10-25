using System.Xml.Serialization;

namespace XmlBaseTest.Elements
{
    public abstract class BaseElement
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }
}