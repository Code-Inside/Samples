using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadRss
{
    class Program
    {
        static void Main(string[] args)
        {
            RssService service = new RssService();
            List<RssItem> resultTwitter = service.GetItems("http://twitter.com/statuses/user_timeline/14109602.rss");
            
            Console.WriteLine("[Twitter]");
            foreach (RssItem item in resultTwitter)
            {
                Console.WriteLine("---");
                Console.WriteLine(item.Title);
                Console.WriteLine(item.Message);
                Console.WriteLine(item.PublishedOn.ToShortDateString());
                Console.WriteLine(item.Url);
                Console.WriteLine("---");
            }

            List<RssItem> resultBlog = service.GetItems("http://feeds2.feedburner.com/Code-insideBlog");
            
            Console.WriteLine("[Blog]");
            foreach (RssItem item in resultBlog)
            {
                Console.WriteLine("---");
                Console.WriteLine(item.Title);
                Console.WriteLine(item.Message);
                Console.WriteLine(item.PublishedOn.ToShortDateString());
                Console.WriteLine(item.Url);
                Console.WriteLine("---");
            }
        

            Console.ReadLine();
        }
    }
}
