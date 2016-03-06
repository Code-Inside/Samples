using System.Collections.Generic;

namespace XmlIntelliSense.App.XHelper
{
    public class XsdInformationElement
    {
        public XsdInformationElement()
        {
            Attributes = new List<XsdInformationAttribute>();
            Elements = new List<XsdInformationElement>();
        }

        public bool IsRoot { get; set; }
        public string Name { get; set; }
        public string XPathLikeKey { get; set; }
        public string DataType { get; set; }
        public List<XsdInformationAttribute> Attributes { get; set; }

        public List<XsdInformationElement> Elements { get; set; }
    }
}