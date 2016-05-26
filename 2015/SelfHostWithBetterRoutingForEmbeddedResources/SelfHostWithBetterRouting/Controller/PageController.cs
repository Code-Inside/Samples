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

namespace SelfHostWithBetterRouting.Controller
{
    public class PageController : ApiController
    {
        private const string ResourcePath = "SelfHostWithBetterRouting.Pages{0}";

        public static Stream GetStream(string folderAndFileInProjectPath)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);
            return asm.GetManifestResourceStream(resource);
        }


        public HttpResponseMessage Get()
        {
             var virtualPathRoot = this.Request.GetRequestContext().VirtualPathRoot;
            string filename = this.Request.RequestUri.PathAndQuery;

            // happens if it is hosted in IIS
            if (virtualPathRoot != "/")
            {
                filename = filename.Replace(virtualPathRoot, string.Empty);
            }
            
            // input as /page-assets/js/scripts.js
            if (filename == "/")
            {
                filename = ".index.html";
            }
            // folders will be seen as "namespaces" - so replace / with the .
            filename = filename.Replace("/", ".");
            // resources can't be named with -, so it will be replaced with a _
            filename = filename.Replace("-", "_");

            var mimeType = System.Web.MimeMapping.GetMimeMapping(filename);

            var fileStream = GetStream(filename);

            if (fileStream != null)
            {
                var response = new HttpResponseMessage();
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                return response;
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

    }
}
