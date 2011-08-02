using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace YourOwnConfigSection.Infrastructure
{
    public class CodeInsideConfigAuthor : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

    } 
}
