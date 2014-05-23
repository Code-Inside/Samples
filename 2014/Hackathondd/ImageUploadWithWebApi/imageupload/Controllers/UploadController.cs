using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace imageupload.Controllers
{
    public class UploadController : ApiController
    {
        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            return await UseMultipartFormDataStream();
        }

        private async Task<HttpResponseMessage> UseMultipartFormDataStream()
        {
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            MultipartFormDataContent mpfdc = new MultipartFormDataContent();

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName;
                    Trace.WriteLine(filename);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                    mpfdc.Add(new ByteArrayContent(File.ReadAllBytes(file.LocalFileName)), "File", filename);
                }
                var response = Request.CreateResponse();
                response.Content = mpfdc;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}