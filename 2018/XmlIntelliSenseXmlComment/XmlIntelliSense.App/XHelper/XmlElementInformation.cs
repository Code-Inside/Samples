namespace XmlIntelliSense.App.XHelper
{
    /// <summary>
    /// Source: Sharpdevelop
    /// </summary>
    public class XmlElementInformation
    {
        XmlElementQualifiedNameCollection elements = new XmlElementQualifiedNameCollection();
        XmlNamespaceCollection namespacesInScope = new XmlNamespaceCollection();

        public XmlElementInformation()
        {
        }

        public string CurrentAttribute { get; set; }

        /// <summary>
        /// Gets the elements specifying the path.
        /// </summary>
        /// <remarks>The order of the elements determines the path.</remarks>
        public XmlElementQualifiedNameCollection Elements
        {
            get { return elements; }
        }

        public void AddElement(XmlElementQualifiedName elementName)
        {
            elements.Add(elementName);
        }

        public bool IsEmpty
        {
            get { return elements.IsEmpty; }
        }

        /// <summary>
        /// Compacts the path so it only contains the elements that are from 
        /// the namespace of the last element in the path. 
        /// </summary>
        /// <remarks>This method is used when we need to know the path for a
        /// particular namespace and do not care about the complete path.
        /// </remarks>
        public void Compact()
        {
            if (elements.HasItems)
            {
                XmlElementQualifiedName lastName = Elements.GetLast();
                int index = LastIndexNotMatchingNamespace(lastName.Namespace);
                if (index != -1)
                {
                    elements.RemoveFirst(index + 1);
                }
            }
        }

        public XmlNamespaceCollection NamespacesInScope
        {
            get { return namespacesInScope; }
        }

        public string GetNamespaceForPrefix(string prefix)
        {
            return namespacesInScope.GetNamespaceForPrefix(prefix);
        }

        /// <summary>
        /// An xml element path is considered to be equal if 
        /// each path item has the same name and namespace.
        /// </summary>
        public override bool Equals(object obj)
        {
            XmlElementInformation rhsPath = obj as XmlElementInformation;
            if (rhsPath == null)
            {
                return false;
            }

            return elements.Equals(rhsPath.elements);
        }

        public override int GetHashCode()
        {
            return elements.GetHashCode();
        }

        public override string ToString()
        {
            return elements.ToString();
        }

        public string GetRootNamespace()
        {
            return elements.GetRootNamespace();
        }

        /// <summary>
        /// Only updates those names without a namespace.
        /// </summary>
        public void SetNamespaceForUnqualifiedNames(string namespaceUri)
        {
            foreach (XmlElementQualifiedName name in elements)
            {
                if (!name.HasNamespace)
                {
                    name.Namespace = namespaceUri;
                }
            }
        }

        int LastIndexNotMatchingNamespace(string namespaceUri)
        {
            if (elements.Count > 1)
            {
                // Start the check from the last but one item.
                for (int i = elements.Count - 2; i >= 0; --i)
                {
                    XmlElementQualifiedName name = elements[i];
                    if (name.Namespace != namespaceUri)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}