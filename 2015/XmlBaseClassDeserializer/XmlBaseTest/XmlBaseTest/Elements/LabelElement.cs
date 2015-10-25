using System.Xml.Serialization;

namespace XmlBaseTest.Elements
{
    public class LabelElement : BaseElement
    {
        [XmlText]
        public string Content { get; set; }
    }
}