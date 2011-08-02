using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace YourOwnConfigSection.Infrastructure
{
    public class CodeInsideConfig : ConfigurationSection
    {
        [ConfigurationProperty("webUrl", DefaultValue = "http://DEFAULT.de", IsRequired = true)]
        public string WebUrl
        {
            get
            {
                return this["webUrl"] as string;
            }
        }

        [ConfigurationProperty("startedOn", IsRequired = false)]
        public int StartedOn
        {
            get
            {
                return (int)this["startedOn"];
            }
        }

        [ConfigurationProperty("authors")]
        public CodeInsideConfigAuthorCollection Authors
        {
            get
            {
                return this["authors"] as CodeInsideConfigAuthorCollection;
            }
        }

        public static CodeInsideConfig GetConfig()
        {
            return ConfigurationSettings.GetConfig("codeInsideConfig") as CodeInsideConfig;
        } 
    }
}
