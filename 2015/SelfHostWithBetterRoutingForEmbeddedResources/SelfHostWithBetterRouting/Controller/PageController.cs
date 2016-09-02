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

        public static string GetStreamContent(string folderAndFileInProjectPath)
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
            var virtualPathRoot = this.Request.GetRequestContext().VirtualPathRoot;
            string filename = this.Request.RequestUri.PathAndQuery;

            // happens if it is hosted in IIS
            if (virtualPathRoot != "/")
            {
                filename = filename.Replace(virtualPathRoot, string.Empty);
            }
            
            // input as /page-assets/js/scripts.js
            if (filename == "/" || filename == "")
            {
                filename = ".index.html";
            }

            // folders will be seen as "namespaces" - so replace / with the .
            filename = filename.Replace("/", ".");
            // resources can't be named with -, so it will be replaced with a _
            filename = filename.Replace("-", "_");

            var mimeType = System.Web.MimeMapping.GetMimeMapping(filename);

            var fileStreamContent = GetStreamContent(filename);

            if (string.IsNullOrWhiteSpace(fileStreamContent))
            {
                throw new Exception(string.Format("Can't find embedded file for '{0}'", filename));
            }

            if (virtualPathRoot != "/")
            {
                fileStreamContent = fileStreamContent.Replace("~/", virtualPathRoot + "/");
            }
            else
            {
                fileStreamContent = fileStreamContent.Replace("~/", virtualPathRoot);
            }

            var response = new HttpResponseMessage();
            response.Content = new StringContent(fileStreamContent);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return response;
        }

    }
}
