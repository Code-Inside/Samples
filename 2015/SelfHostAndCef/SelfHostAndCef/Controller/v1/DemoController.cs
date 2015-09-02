using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SelfHostAndCef.Controller.v1
{
    public class DemoController : ApiController
    {
        private const string ResourcePath = "SelfHostAndCef.HtmlSamples.{0}";
        public static string GetTestFileContent(string folderAndFileInProjectPath)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);

            using (var stream = asm.GetManifestResourceStream(resource))
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }
            return String.Empty;
        }

        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(GetTestFileContent("demo.html"));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

    }
}
