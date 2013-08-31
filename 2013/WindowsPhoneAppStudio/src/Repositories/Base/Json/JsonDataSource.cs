using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WPAppStudio.Repositories.Resources;

namespace WPAppStudio.Repositories.Base
{
    /// <summary>
    /// Implementation of a data source based on JSON.
    /// </summary>
    public class JsonDataSource : IJsonDataSource
    {
        /// <summary>
        /// Gets the JSON data from a specified URL.
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
                request.Method = "GET";
                request.Accept = "application/json";

                var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

                using (var responseStream = response.GetResponseStream())
                {
                    using (var output = new MemoryStream())
                    {
                        await responseStream.CopyToAsync(output);
                        var data = output.ToArray();
                        var responseBody = Encoding.UTF8.GetString(data, 0, data.Length);
                        try
                        {
                            var dataSource = JsonConvert.DeserializeObject<T>(responseBody);
                            result = dataSource;
                        }
                        catch (Exception e)
                        {
                            var error = string.Format("{0} {1}. {2} {3}", AppResources.DeserializeJSONError, url, AppResources.Error, e.ToString());
                            Debug.WriteLine(error);
                            throw new Exception(error, e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var error = string.Format("{0} {1}. {2} {3}", AppResources.DataError, url, AppResources.Error, e.ToString());
                Debug.WriteLine(error);
				throw new Exception(error, e);
            }

            return result;
        }

        /// <summary>
        /// Gets the JSON data from a specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The data retrieved as dynamic.</returns>
        public async Task<JArray> LoadRemote(string url)
        {
            JArray dynObj;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Accept = "application/json";

                var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

                using (var responseStream = response.GetResponseStream())
                {
                    using (var output = new MemoryStream())
                    {
                        await responseStream.CopyToAsync(output);
                        var data = output.ToArray();
                        var responseBody = Encoding.UTF8.GetString(data, 0, data.Length);
                        try
                        {
                            dynObj = (JArray)JsonConvert.DeserializeObject(responseBody);
                        }
                        catch (Exception e)
                        {
							var error = string.Format("{0} {1}. {2} {3}", AppResources.DeserializeJSONError, url, AppResources.Error, e.ToString());
                            Debug.WriteLine(error);
                            throw new Exception(error, e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var error = AppResources.DataError;
                Debug.WriteLine(error);
                throw new Exception(error, e);
            }

            return dynObj;
        }
    }
}
