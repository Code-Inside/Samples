using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using WPAppStudio.Entities.Base;
using WPAppStudio.Repositories.Resources;

namespace WPAppStudio.Repositories.Base
{
    /// <summary>
    /// Implementation of a YouTube data source.
    /// </summary>
    public class YoutubeDataSource : IYoutubeDataSource
    {
        /// <summary>
        /// Gets the YouTube data from a specified URL.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>The data retrieved.</returns>
        public async Task<T> LoadRemote<T>(string url)
        {
            T result;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
                using (var responseStream = response.GetResponseStream())
                {
                    var atomns = XNamespace.Get("http://www.w3.org/2005/Atom");
                    var medians = XNamespace.Get("http://search.yahoo.com/mrss/");
                    var xml = XElement.Load(responseStream);
                    result = (T)(from entry in xml.Descendants(atomns.GetName("entry"))
                                 select new YouTubeVideo(atomns, entry, medians));
                }
            }
            catch (Exception e)
            {
                var error = string.Format("{0} {1}", AppResources.DataError ,e.ToString());
                Debug.WriteLine(error);
                throw new Exception(error, e);
            }
            return result;
        }
    }
}



