using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace XmlIntelliSense.App.XHelper
{
    /// <summary>
    /// Combination of Sharpdevelop XmlEditor Code (https://github.com/icsharpcode/SharpDevelop/tree/master/src/AddIns/DisplayBindings/XmlEditor) &
    /// XSemmel https://xsemmel.codeplex.com
    /// 
    /// Copyright 2014 AlphaSierraPapa for the SharpDevelop team. SharpDevelop is distributed under the MIT license.
    /// XSemmel: Simplified BSD License (BSD)
    /// Copyright (c) 2007, F. Schnitzer
    /// All rights reserved.
    /// </summary>
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
        /// Gets path of the xml element start tag that the specified
        /// <paramref name="index"/> is currently inside.
        /// </summary>
        /// <remarks>If the index outside the start tag then an empty path
        /// is returned.</remarks>
        public static XmlElementInformation GetActiveElementStartPath(string xml, int index)
        {
            string elementText = GetActiveElementStartText(xml, index);
            if (elementText != null)
            {
                return GetActiveElementStartPath(xml, index, elementText);
            }
            return new XmlElementInformation();
        }

        /// <summary>
        /// Source: Sharpdevelop
        /// Gets the element name from the element start tag string.
        /// </summary>
        /// <param name="xml">This string must start at the
        /// element we are interested in.</param>
        static XmlElementQualifiedName GetElementName(string xml)
        {
            string name = String.Empty;

            // Find the end of the element name.
            xml = xml.Replace("\r\n", " ");
            int index = xml.IndexOf(' ');
            if (index > 0)
            {
                name = xml.Substring(1, index - 1);
            }
            else {
                name = xml.Substring(1);
            }

            return XmlElementQualifiedName.FromString(name);
        }

        /// <summary>
        /// Source: Sharpdevelop
        /// Gets the active element path given the element text.
        /// </summary>
        static XmlElementInformation GetActiveElementStartPath(string xml, int index, string elementText)
        {
            XmlElementQualifiedName elementName = GetElementName(elementText);
            if (elementName.IsEmpty)
            {
                return new XmlElementInformation();
            }

            XmlNamespace elementNamespace = GetElementNamespace(elementText);

            XmlElementInformation path = GetFullParentElementPath(xml.Substring(0, index));

            // Try to get a namespace for the active element's prefix.
            if (elementName.HasPrefix && !elementNamespace.HasName)
            {
                elementName.Namespace = path.GetNamespaceForPrefix(elementName.Prefix);
                elementNamespace.Name = elementName.Namespace;
                elementNamespace.Prefix = elementName.Prefix;
            }

            if (!elementNamespace.HasName)
            {
                if (path.Elements.Count > 0)
                {
                    XmlElementQualifiedName parentName = path.Elements[path.Elements.Count - 1];
                    elementNamespace.Name = parentName.Namespace;
                    elementNamespace.Prefix = parentName.Prefix;
                }
            }
            path.AddElement(new XmlElementQualifiedName(elementName.Name, elementNamespace));
            return path;
        }

        /// <summary>
        /// Source: Sharpdevelop
        /// Gets the parent element path based on the index position. This
        /// method does not compact the path so it will include all elements
        /// including those in another namespace in the path.
        /// </summary>
        static XmlElementInformation GetFullParentElementPath(string xml)
        {
            XmlElementInformation path = new XmlElementInformation();
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
                                        XmlElementQualifiedName elementName = new XmlElementQualifiedName(xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Prefix);
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


        /// <summary>
        /// Source: Sharpdevelop
        /// Gets the element namespace from the element start tag
        /// string.
        /// </summary>
        /// <param name="xml">This string must start at the
        /// element we are interested in.</param>
        static XmlNamespace GetElementNamespace(string xml)
        {
            XmlNamespace namespaceUri = new XmlNamespace();

            Match match = Regex.Match(xml, ".*?(xmlns\\s*?|xmlns:.*?)=\\s*?['\\\"](.*?)['\\\"]");
            if (match.Success)
            {
                namespaceUri.Name = match.Groups[2].Value;

                string xmlns = match.Groups[1].Value.Trim();
                int prefixIndex = xmlns.IndexOf(':');
                if (prefixIndex > 0)
                {
                    namespaceUri.Prefix = xmlns.Substring(prefixIndex + 1);
                }
            }

            return namespaceUri;
        }

        /// <summary>
        /// Source: SharpDevelop
        /// Gets the text of the xml element start tag that the index is
        /// currently inside.
        /// </summary>
        /// <returns>
        /// Returns the text up to and including the start tag &lt; character.
        /// </returns>
        static string GetActiveElementStartText(string xml, int index)
        {
            int elementStartIndex = GetActiveElementStartIndex(xml, index);
            if (elementStartIndex >= 0)
            {
                if (elementStartIndex < index)
                {
                    int elementEndIndex = GetActiveElementEndIndex(xml, index);
                    if (elementEndIndex >= index)
                    {
                        return xml.Substring(elementStartIndex, elementEndIndex - elementStartIndex);
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Source: Sharpdevelop
        /// Locates the index of the end tag character.
        /// </summary>
        /// <returns>
        /// Returns the index of the end tag character; otherwise
        /// -1 if no end tag character is found or a start tag
        /// character is found first.
        /// </returns>
        static int GetActiveElementEndIndex(string xml, int index)
        {
            int elementEndIndex = index;

            for (int i = index; i < xml.Length; ++i)
            {

                char currentChar = xml[i];
                if (currentChar == '>')
                {
                    elementEndIndex = i;
                    break;
                }
                else if (currentChar == '<')
                {
                    elementEndIndex = -1;
                    break;
                }
            }

            return elementEndIndex;
        }

        /// <summary>
        /// Source: Sharpdevelop
        /// Locates the index of the start tag &lt; character.
        /// </summary>
        /// <returns>
        /// Returns the index of the start tag character; otherwise
        /// -1 if no start tag character is found or a end tag
        /// &gt; character is found first.
        /// </returns>
        public static int GetActiveElementStartIndex(string xml, int index)
        {
            int elementStartIndex = -1;
            int currentIndex = index - 1;

            while (currentIndex > -1)
            {
                char currentChar = xml[currentIndex];
                if (currentChar == '<')
                {
                    elementStartIndex = currentIndex;
                    break;
                }
                else if (currentChar == '>')
                {
                    break;
                }

                --currentIndex;
            }

            return elementStartIndex;
        }



        /// <summary>
        /// Source: SharpDevelop
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XmlElementInformation GetParentElementPath(string xml)
        {
            XmlElementInformation path = new XmlElementInformation();
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
                                        XmlElementQualifiedName elementName = new XmlElementQualifiedName(xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Prefix);
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
