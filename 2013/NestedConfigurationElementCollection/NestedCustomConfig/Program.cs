using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestedCustomConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            // LINQ & config demo
            var authors = from CodeInsideConfigAuthor author in CodeInsideConfig.GetConfig().Authors
                          select author;

            foreach (var codeInsideConfigAuthor in authors)
            {
                Console.WriteLine(codeInsideConfigAuthor.Name);
                Console.WriteLine("Foobar-Topics:");

                var topicsWithFoobar = from CodeInsideConfigTopic topic in codeInsideConfigAuthor.Topics
                                    where topic.Name.Contains("Foobar")
                                    select topic;


                foreach (CodeInsideConfigTopic topics in topicsWithFoobar)
                {
                    Console.WriteLine(topics.Name);
                }
            }

            Console.ReadLine();
        }
    }

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

    public class CodeInsideConfigAuthorCollection : ConfigurationElementCollection
    {
        // define "author" as child-element-name (and not add...)
        public CodeInsideConfigAuthorCollection()
        {
            AddElementName = "author";
        }

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

        [ConfigurationProperty("topics")]
        public CodeInsideConfigTopicCollection Topics
        {
            get
            {
                return this["topics"] as CodeInsideConfigTopicCollection;
            }
        }

    }

    public class CodeInsideConfigTopicCollection : ConfigurationElementCollection
    {
        public CodeInsideConfigTopic this[int index]
        {
            get
            {
                return base.BaseGet(index) as CodeInsideConfigTopic;
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
            return new CodeInsideConfigTopic();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CodeInsideConfigTopic)element).Name;
        }
    }

    public class CodeInsideConfigTopic : ConfigurationElement
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
