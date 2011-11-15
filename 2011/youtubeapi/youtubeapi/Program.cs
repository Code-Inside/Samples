using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace youtubeapi
{
    using Google.YouTube;

    class Program
    {
        static void Main(string[] args)
        {
            Uri youTubeLink = new Uri("http://www.youtube.com/watch?v=ItqQ2EZziB8");
            var parameters = System.Web.HttpUtility.ParseQueryString(youTubeLink.Query);
            var videoId = parameters["v"];

            Uri youTubeApi = new Uri(string.Format("http://gdata.youtube.com/feeds/api/videos/{0}", videoId));
            YouTubeRequestSettings settings = new YouTubeRequestSettings(null, null);

            YouTubeRequest request = new YouTubeRequest(settings);
            var result = request.Retrieve<Video>(youTubeApi);

            Console.WriteLine(result.Title);
            Console.WriteLine(result.Description);
            Console.WriteLine(result.ViewCount);

            Console.ReadLine();
        }
    }
}
