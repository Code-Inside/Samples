using System.Xml.Serialization;

namespace XmlBaseTest.Elements
{
    public class TextBoxElement : BaseElement
    {
        [XmlAttribute(AttributeName = "MultiLines")]
        public int MultiLines { get; set; }
    }
}