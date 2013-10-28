using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitterApiTest
{
    class Program
    {
        public static string oAuthConsumerKey = "9jD2zxpqPF3zHRLnMek7Q";
        public static string oAuthConsumerSecret = "yYSC6bRm50n2vvZOuVlSWtbk9B8lNImzjazyl5eYM";
        public static string oAuthUrl = "https://api.twitter.com/oauth2/token";
        public static string screenname = "robert0muehsig";

        public static async Task<string> GetTwitterAccessToken()
        {
            var client = new HttpClient();

            var authHeaderParameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(oAuthConsumerKey) + ":" +
                                                             Uri.EscapeDataString((oAuthConsumerSecret))));

            var postBody = "grant_type=client_credentials";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderParameter);

            var response = await client.PostAsync(oAuthUrl, new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded"));

            response.EnsureSuccessStatusCode();

            string oauthtoken = await response.Content.ReadAsStringAsync();
            var jToken = JToken.Parse(oauthtoken);
            var accessToken = jToken.SelectToken("access_token");

            return accessToken.Value<string>();
        }

        public static async Task<IEnumerable<string>> GetTwitterTimeline(string oauthToken)
        {
            var client = new HttpClient();

            var timelineFormat = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&include_rts=1&exclude_replies=1&count=5";
            var timelineUrl = string.Format(timelineFormat, screenname);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            var response = await client.GetAsync(timelineUrl);

            response.EnsureSuccessStatusCode();

            string timeline = await response.Content.ReadAsStringAsync();

            var jTimeline = JArray.Parse(timeline);
            var textNodes = jTimeline.Children()["text"];

            var textValues = textNodes.Values<string>();

            return textValues;
        }

        static void Main(string[] args)
        {
            var twitterAccessToken = GetTwitterAccessToken().Result;

            var timeline = GetTwitterTimeline(twitterAccessToken).Result;

            Console.WriteLine("Twitter Timeline from: " + screenname);
            foreach (var timelineItem in timeline)
            {
                Console.WriteLine(timelineItem);
            }

            Console.ReadLine();
        }
    }
}
