using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlBaseTest
{
    public class Root
    {
        [XmlElement(ElementName = "Node")]
        public List<Node> Nodes { get; set; }
    }
}