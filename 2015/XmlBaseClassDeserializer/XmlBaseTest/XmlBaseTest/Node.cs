using System.Collections.Generic;
using System.Xml.Serialization;
using XmlBaseTest.Elements;

namespace XmlBaseTest
{
    public class Node
    {
        [XmlElement(typeof(LabelElement), ElementName = "Label")]
        [XmlElement(typeof(TextBoxElement), ElementName = "TextBox")]
        [XmlElement(typeof(ButtonElement), ElementName = "Button")]
        public List<BaseElement> Elements { get; set; }
    }
}