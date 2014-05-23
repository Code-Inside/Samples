using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace imageuploader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        var values = new[]
        {
            new KeyValuePair<string, string>("Foo", "Bar"),
            new KeyValuePair<string, string>("More", "Less"),
        };

                        foreach (var keyValuePair in values)
                        {
                            content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                        }

                        var fileContent = new StreamContent(new FileStream(@"..\..\..\Test.jpg", FileMode.Open));
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "Test.jpg"
                        };
                        content.Add(fileContent);

                        var requestUri = "http://localhost:5916/api/upload";
                        var result = client.PostAsync(requestUri, content).Result;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
