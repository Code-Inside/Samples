using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace YourOwnConfigSection.Infrastructure
{
    public class CodeInsideConfigAuthorCollection : ConfigurationElementCollection
    {
        public CodeInsideConfigAuthor this[int index]
        {
            get
            {
                return base.BaseGet(index) as CodeInsideConfigAuthor;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CodeInsideConfigAuthor();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CodeInsideConfigAuthor)element).Name;
        } 
    }
}
