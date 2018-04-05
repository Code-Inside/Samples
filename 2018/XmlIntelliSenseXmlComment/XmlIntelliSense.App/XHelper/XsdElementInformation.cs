using System.Collections.Generic;

namespace XmlIntelliSense.App.XHelper
{
    public class XsdElementInformation
    {
        public XsdElementInformation()
        {
            Attributes = new List<XsdAttributeInformation>();
            Elements = new List<XsdElementInformation>();
        }

        public bool IsRoot { get; set; }
        public string Name { get; set; }
        public string XPathLikeKey { get; set; }
        public string DataType { get; set; }
        public List<XsdAttributeInformation> Attributes { get; set; }

        public List<XsdElementInformation> Elements { get; set; }
    }
}