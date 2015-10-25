using System.Xml.Serialization;

namespace XmlBaseTest.Elements
{
    public class ButtonElement : BaseElement
    {
        [XmlAttribute(AttributeName = "Label")]
        public string Label { get; set; }

        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }
    }
}