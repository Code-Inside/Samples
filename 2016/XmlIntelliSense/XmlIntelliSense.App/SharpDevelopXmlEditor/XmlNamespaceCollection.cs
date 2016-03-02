using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XmlIntelliSense.App.SharpDevelopXmlEditor
{
    public class XmlNamespaceCollection : Collection<XmlNamespace>
    {
        public XmlNamespaceCollection()
        {
        }

        public XmlNamespace[] ToArray()
        {
            List<XmlNamespace> namespaces = new List<XmlNamespace>(this);
            return namespaces.ToArray();
        }

        public string GetNamespaceForPrefix(string prefix)
        {
            foreach (XmlNamespace ns in this)
            {
                if (ns.Prefix == prefix)
                {
                    return ns.Name;
                }
            }
            return String.Empty;
        }

        public string GetPrefix(string namespaceToMatch)
        {
            foreach (XmlNamespace ns in this)
            {
                if (ns.Name == namespaceToMatch)
                {
                    return ns.Prefix;
                }
            }
            return String.Empty;
        }
    }
}