using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using Google.GData.Client;
using Google.GData.Extensions;

namespace YouTubeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Suchwort:");
            string searchTerm = Console.ReadLine();

            string uriSearchTerm = HttpUtility.UrlEncode(searchTerm);
            string url = "http://gdata.youtube.com/feeds/videos?q=" + uriSearchTerm;
            
            Console.WriteLine("Connection to YouTube - Searching: " + searchTerm);

            FeedQuery query = new FeedQuery("");
            Service service = new Service("youtube", "sample");
            query.Uri = new Uri(url);
            query.StartIndex = 0;
            query.NumberToRetrieve = 20;
            AtomFeed resultFeed = service.Query(query);

            foreach (AtomEntry entry in resultFeed.Entries)
            {
                Console.WriteLine("Title: " + entry.Title.Text);
                Console.WriteLine("Link: " + entry.AlternateUri.Content);
                Console.WriteLine("Tags:");
                foreach (AtomCategory cat in entry.Categories)
                {
                    Console.Write(cat.Term + ", ");
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
