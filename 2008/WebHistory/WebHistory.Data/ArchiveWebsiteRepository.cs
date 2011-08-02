using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebHistory.Model;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace WebHistory.Data
{
    public class ArchiveWebsiteRepository : IWebsiteRepository
    {

        public Website GetWebsite(string url)
        {
            string archiveUrl = "http://web.archive.org/web/*/" + url;
            HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(archiveUrl);
            HttpWebResponse response = (HttpWebResponse)rq.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string html = reader.ReadToEnd();

            Website resultWebsite = new Website();
            resultWebsite.Url = url;

            MatchCollection matchs = Regex.Matches(html, @"<a.href=.http://web.archive.org/web/\d.*?</a>");
            foreach (Match match in matchs)
            {
                ArchiveWebsite archive = new ArchiveWebsite();
                archive.ArchiveUrl = Regex.Match(match.Value, @"http://web.archive.org/web/\d*").Value + "/" + url;
                archive.Date = DateTime.Parse(Regex.Match(match.Value, @"\w\w\w\s\d\d,\s\d\d\d\d").Value);
                resultWebsite.ArchiveWebsites.Add(archive);
            }

            return resultWebsite;
        }

    }
}
