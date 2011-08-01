using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;

namespace MediaRssSample
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class MediaRss : IHttpHandler
    {
        XNamespace media = "http://search.yahoo.com/mrss";
        XNamespace atom = "http://www.w3.org/2005/Atom";
        public void ProcessRequest(HttpContext context)
        {
           

            XDocument document = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement("rss",
                                        new XAttribute("version", "2.0"),
                                        new XAttribute(XNamespace.Xmlns + "media", media),
                                        new XAttribute(XNamespace.Xmlns + "atom", atom),
                                        new XElement("channel", this.CreateElements())
                                       ));

            context.Response.ContentType = "text/xml";
            document.Save(context.Response.Output);
            context.Response.End();
        }

        private IEnumerable<XElement> CreateElements()
        {
            List<XElement> list = new List<XElement>();

            for(int i = 1; i < 100; i++)
            {
                XElement itemElement = new XElement("item",
                                            new XElement("title", i),
                                            new XElement("link", "Code-Inside.de"),
                                            new XElement(media + "thumbnail", 
                                                new XAttribute("url", "http://code-inside.de/blog/wp-content/uploads/image-thumb" + i + ".png")),
                                            new XElement(media + "content",
                                                new XAttribute("url", "http://code-inside.de/blog/wp-content/uploads/image-thumb" + i + ".png"))
                                       );
                list.Add(itemElement);
            }

            return list;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
