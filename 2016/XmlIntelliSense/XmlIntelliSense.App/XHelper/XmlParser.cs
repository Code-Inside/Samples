using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlIntelliSense.App.SharpDevelopXmlEditor;

namespace XmlIntelliSense.App.XHelper
{
    public static class XmlParser
    {
        /// <summary>
        /// Source: https://xsemmel.codeplex.com
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string GetElementAtCursor(string xml, int offset)
        {
            if (offset == xml.Length)
            {
                offset--;
            }
            int startIdx = xml.LastIndexOf('<', offset);
            if (startIdx < 0) return null;

            if (startIdx < xml.Length && xml[startIdx + 1] == '/')
            {
                startIdx = startIdx + 1;
            }

            int endIdx1 = xml.IndexOf(' ', startIdx);
            if (endIdx1 == -1 /*|| endIdx1 > offset*/) endIdx1 = int.MaxValue;

            int endIdx2 = xml.IndexOf('>', startIdx);
            if (endIdx2 == -1 /*|| endIdx2 > offset*/)
            {
                endIdx2 = int.MaxValue;
            }
            else
            {
                if (endIdx2 < xml.Length && xml[endIdx2 - 1] == '/')
                {
                    endIdx2 = endIdx2 - 1;
                }
            }

            int endIdx = Math.Min(endIdx1, endIdx2);
            if (endIdx2 > 0 && endIdx2 < int.MaxValue && endIdx > startIdx)
            {
                return xml.Substring(startIdx + 1, endIdx - startIdx - 1);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Source: https://xsemmel.codeplex.com
        /// Liefert true falls das Element beim offset ein schließendes Element ist,
        /// also &lt;/x&gt; oder &lt;x/&gt;
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="offset"></param>
        /// <param name="elementName">optional, elementName = GetElementAtCursor(xml, offset)</param>
        /// <returns></returns>
        public static bool IsClosingElement(string xml, int offset, string elementName = null)
        {
            if (elementName == null)
            {
                elementName = GetElementAtCursor(xml, offset);
            }
            else
            {
                Debug.Assert(GetElementAtCursor(xml, offset) == elementName);
            }

            if (offset >= xml.Length || offset < 0)
            {
                return false;
            }
            int idxOpen = xml.LastIndexOf('<', offset);
            if (idxOpen < 0)
            {
                return false;
            }

            int idxClose = xml.LastIndexOf('>', offset);
            if (idxClose > 0)
            {
                if (idxClose > idxOpen && idxClose < offset - 1)
                {
                    return false;
                }
            }

            string prefix = xml.Substring(idxOpen, offset - idxOpen);
            if (prefix.Contains("/"))
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// Source: SharpDevelop
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XmlElementPath GetParentElementPath(string xml)
        {
            XmlElementPath path = new XmlElementPath();
            IDictionary<string, string> namespacesInScope = null;
            using (StringReader reader = new StringReader(xml))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(reader))
                {
                    try
                    {
                        xmlReader.XmlResolver = null; // prevent XmlTextReader from loading external DTDs
                        while (xmlReader.Read())
                        {
                            switch (xmlReader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    if (!xmlReader.IsEmptyElement)
                                    {
                                        QualifiedName elementName = new QualifiedName(xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Prefix);
                                        path.AddElement(elementName);
                                    }
                                    break;
                                case XmlNodeType.EndElement:
                                    path.Elements.RemoveLast();
                                    break;
                            }
                        }
                    }
                    catch (XmlException)
                    {
                        namespacesInScope = xmlReader.GetNamespacesInScope(XmlNamespaceScope.All);
                    }
                }
            }

            // Add namespaces in scope for the last element read.
            if (namespacesInScope != null)
            {
                foreach (KeyValuePair<string, string> ns in namespacesInScope)
                {
                    path.NamespacesInScope.Add(new XmlNamespace(ns.Key, ns.Value));
                }
            }

            return path;
        }
    }
}
