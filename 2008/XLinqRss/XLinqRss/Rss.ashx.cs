using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;

namespace XLinqRss
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Rss : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            XDocument document = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement("rss",
                                        new XAttribute("version", "2.0"),
                                        new XElement("channel", this.CreateElements())
                                       ));

            context.Response.ContentType = "text/xml";
            document.Save(context.Response.Output);
            context.Response.End();
        }

        private IEnumerable<XElement> CreateElements()
        {
            List<XElement> list = new List<XElement>();

            for (int i = 1; i < 100; i++)
            {
                XElement itemElement = new XElement("item",
                                            new XElement("title", i),
                                            new XElement("link", "http://code-inside.de")
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
