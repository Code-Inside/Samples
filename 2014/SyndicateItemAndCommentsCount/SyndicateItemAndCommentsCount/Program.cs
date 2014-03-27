using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SyndicateItemAndCommentsCount
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = XmlReader.Create("http://blog.codeinside.eu/feed");
            var feed = SyndicationFeed.Load(reader);

            foreach (var feedItem in feed.Items)
            {
                int commentCount = 0;

                Console.WriteLine(feedItem.Title.Text);

                foreach (SyndicationElementExtension extension in feedItem.ElementExtensions)
                {
                    XElement extensionElement = extension.GetObject<XElement>();

                    if (extensionElement.Name.LocalName == "comments" && extensionElement.Name.NamespaceName == "http://purl.org/rss/1.0/modules/slash/")
                    {
                        Console.WriteLine("Comments: " + extensionElement.Value);
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
