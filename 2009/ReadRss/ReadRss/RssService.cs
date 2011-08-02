using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ReadRss
{
    public class RssService
    {
        public List<RssItem> GetItems(string feedUrl)
        {
            XDocument doc = XDocument.Load(feedUrl);
            return (from x in doc.Descendants("channel").Descendants("item")
                         select new RssItem()
                         {
                             Title = x.Descendants("title").Single().Value,
                             Message = x.Descendants("description").Single().Value,
                             Url = x.Descendants("link").Single().Value,
                             PublishedOn = DateTime.Parse(x.Descendants("pubDate").Single().Value)
                         }).ToList();

        }
    }
}
