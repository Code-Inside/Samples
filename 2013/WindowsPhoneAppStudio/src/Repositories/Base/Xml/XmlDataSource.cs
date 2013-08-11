using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using WPAppStudio.Repositories.Resources;

namespace WPAppStudio.Repositories.Base
{
    /// <summary>
    /// Implementation of a data source based on XML.
    /// </summary>
    public class XmlDataSource : IXmlDataSource
    {
        /// <summary>
        /// Gets the XML data from a specified URL.
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
                    var nt = new NameTable();

                    var nsmgr = new XmlNamespaceManager(nt);
                    nsmgr.AddNamespace("georss", "http://www.w3.org/2001/XMLSchema-instance");
                    var context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                    var xset = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};
                    var rdr = XmlReader.Create(responseStream, xset, context);
					var rss10Reader = new Rss10FeedFormatter();
                    SyndicationFeed feed;
					try
					{
						feed = SyndicationFeed.Load(rdr);
					}
					catch (Exception ex)
					{
						Debug.WriteLine("{0} {1}, {2} {3} {4}", AppResources.RssError, url, AppResources.Error, ex);
						rss10Reader.ReadFrom(rdr);
						feed = rss10Reader.Feed;
					}
					result = (T)Convert.ChangeType(feed, typeof(T));
                }
            }
            catch (Exception ex)
            {
				var error = string.Format("{0} {1}. {2} {3}", AppResources.RssError, url, AppResources.Error, ex);
                Debug.WriteLine(error);
                throw new Exception(error, ex);
            }
            return result;
        }
    }
}
