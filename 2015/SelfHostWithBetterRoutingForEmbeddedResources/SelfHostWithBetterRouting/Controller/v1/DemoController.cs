using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SelfHostWithBetterRouting.Controller.v1
{
    public class DemoController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("HelloWorld from DemoController");
            return response;
        }
    }
}
