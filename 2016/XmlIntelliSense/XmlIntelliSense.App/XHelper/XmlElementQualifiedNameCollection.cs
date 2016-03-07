using System;
using System.Collections.ObjectModel;
using System.Text;

namespace XmlIntelliSense.App.XHelper
{
    /// <summary>
    /// Source: Sharpdevelop
    /// </summary>
    public class XmlElementQualifiedNameCollection : Collection<XmlElementQualifiedName>
    {
        public XmlElementQualifiedNameCollection()
        {
        }

        public XmlElementQualifiedNameCollection(XmlElementQualifiedNameCollection names)
        {
            AddRange(names);
        }

        public XmlElementQualifiedNameCollection(XmlElementQualifiedName[] names)
        {
            AddRange(names);
        }

        public bool HasItems
        {
            get { return Count > 0; }
        }

        public bool IsEmpty
        {
            get { return !HasItems; }
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    text.Append(" > ");
                }
                text.Append(this[i].ToString());
            }
            return text.ToString();
        }

        public void AddRange(XmlElementQualifiedName[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                Add(names[i]);
            }
        }

        public void AddRange(XmlElementQualifiedNameCollection names)
        {
            for (int i = 0; i < names.Count; i++)
            {
                Add(names[i]);
            }
        }

        public void RemoveLast()
        {
            if (HasItems)
            {
                RemoveAt(Count - 1);
            }
        }

        public void RemoveFirst()
        {
            if (HasItems)
            {
                RemoveFirst(1);
            }
        }

        public void RemoveFirst(int howMany)
        {
            if (howMany > Count)
            {
                howMany = Count;
            }

            while (howMany > 0)
            {
                RemoveAt(0);
                --howMany;
            }
        }

        public string GetLastPrefix()
        {
            if (HasItems)
            {
                XmlElementQualifiedName name = this[Count - 1];
                return name.Prefix;
            }
            return String.Empty;
        }

        public string GetNamespaceForPrefix(string prefix)
        {
            foreach (XmlElementQualifiedName name in this)
            {
                if (name.Prefix == prefix)
                {
                    return name.Namespace;
                }
            }
            return String.Empty;
        }

        public XmlElementQualifiedName GetLast()
        {
            if (HasItems)
            {
                return this[Count - 1];
            }
            return null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            XmlElementQualifiedNameCollection rhs = obj as XmlElementQualifiedNameCollection;
            if (rhs != null)
            {
                if (Count == rhs.Count)
                {
                    for (int i = 0; i < Count; ++i)
                    {
                        XmlElementQualifiedName lhsName = this[i];
                        XmlElementQualifiedName rhsName = rhs[i];
                        if (!lhsName.Equals(rhsName))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public string GetRootNamespace()
        {
            if (HasItems)
            {
                return this[0].Namespace;
            }
            return String.Empty;
        }
    }
}