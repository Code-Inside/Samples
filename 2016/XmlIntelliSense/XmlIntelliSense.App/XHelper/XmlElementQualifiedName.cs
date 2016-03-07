using System;
using System.Xml;

namespace XmlIntelliSense.App.XHelper
{
    /// <summary>
    /// Source: Sharpdevelop
    /// </summary>
    public class XmlElementQualifiedName
    {
        System.Xml.XmlQualifiedName xmlQualifiedName = System.Xml.XmlQualifiedName.Empty;
        string prefix = String.Empty;

        public XmlElementQualifiedName()
        {
        }

        public XmlElementQualifiedName(string name, string namespaceUri)
            : this(name, namespaceUri, String.Empty)
        {
        }

        public XmlElementQualifiedName(string name, XmlNamespace ns)
            : this(name, ns.Name, ns.Prefix)
        {
        }

        public XmlElementQualifiedName(string name, string namespaceUri, string prefix)
        {
            xmlQualifiedName = new System.Xml.XmlQualifiedName(name, namespaceUri);
            this.prefix = prefix;
        }

        public static bool operator ==(XmlElementQualifiedName lhs, XmlElementQualifiedName rhs)
        {
            object lhsObject = (object)lhs;
            object rhsObject = (object)rhs;
            if ((lhsObject != null) && (rhsObject != null))
            {
                return lhs.Equals(rhs);
            }
            else if ((lhsObject == null) && (rhsObject == null))
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(XmlElementQualifiedName lhs, XmlElementQualifiedName rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// A qualified name is considered equal if the namespace and 
        /// name are the same.  The prefix is ignored.
        /// </summary>
        public override bool Equals(object obj)
        {
            XmlElementQualifiedName xmlElementQualifiedName = obj as XmlElementQualifiedName;
            if (xmlElementQualifiedName != null)
            {
                return this.xmlQualifiedName.Equals(xmlElementQualifiedName.xmlQualifiedName);
            }
            else {
                System.Xml.XmlQualifiedName name = obj as System.Xml.XmlQualifiedName;
                if (name != null)
                {
                    return this.xmlQualifiedName.Equals(name);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return xmlQualifiedName.GetHashCode();
        }

        public bool IsEmpty
        {
            get { return xmlQualifiedName.IsEmpty && String.IsNullOrEmpty(prefix); }
        }

        /// <summary>
        /// Gets the namespace of the qualified name.
        /// </summary>
        public string Namespace
        {
            get { return xmlQualifiedName.Namespace; }
            set { xmlQualifiedName = new System.Xml.XmlQualifiedName(xmlQualifiedName.Name, value); }
        }

        public bool HasNamespace
        {
            get { return xmlQualifiedName.Namespace.Length > 0; }
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        public string Name
        {
            get { return xmlQualifiedName.Name; }
            set { xmlQualifiedName = new System.Xml.XmlQualifiedName(value, xmlQualifiedName.Namespace); }
        }

        /// <summary>
        /// Gets the namespace prefix used.
        /// </summary>
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        public bool HasPrefix
        {
            get { return !String.IsNullOrEmpty(prefix); }
        }

        public override string ToString()
        {
            string qualifiedName = GetPrefixedName();
            if (HasNamespace)
            {
                return String.Concat(qualifiedName, " [", xmlQualifiedName.Namespace, "]");
            }
            return qualifiedName;
        }

        public string GetPrefixedName()
        {
            if (String.IsNullOrEmpty(prefix))
            {
                return xmlQualifiedName.Name;
            }
            return prefix + ":" + xmlQualifiedName.Name;
        }

        public static XmlElementQualifiedName FromString(string name)
        {
            if (name == null)
            {
                return new XmlElementQualifiedName();
            }

            int index = name.IndexOf(':');
            if (index >= 0)
            {
                return CreateFromNameWithPrefix(name, index);
            }
            return new XmlElementQualifiedName(name, String.Empty);
        }

        static XmlElementQualifiedName CreateFromNameWithPrefix(string name, int index)
        {
            string prefix = name.Substring(0, index);
            name = name.Substring(index + 1);
            return new XmlElementQualifiedName(name, String.Empty, prefix);
        }
    }
}