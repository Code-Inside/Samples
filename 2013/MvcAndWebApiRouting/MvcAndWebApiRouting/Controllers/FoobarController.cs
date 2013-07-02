using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MvcAndWebApiRouting.Controllers
{
    public class FoobarController : ApiController
    {
        public string Get()
        {
            // Results in "http://localhost:9547/" (Default Controller Route)
            var urlForMvcController = Url.Link("Default", new {controller = "MvcWebsite", action = "Index"});

            // Results in "http://localhost:9547/AnotherMvcWebsite/Foobar"
            var urlForAnotherMvcController = Url.Link("Default",
                                                      new
                                                          {
                                                              controller = "AnotherMvcWebsite",
                                                              action = "Foobar"
                                                          });

            // Results in "http://localhost:9547/api/Foobarbuzz"
            var anotherWebApiController = Url.Link("DefaultApi", new {controller = "Foobarbuzz"});

            // Results in "/api/Foobarbuzz"
            var anotherWebApiControllerRoute = Url.Route("DefaultApi", new { controller = "Foobarbuzz" });

            return "Foobar";
        }
    }

    public class FoobarbuzzController : ApiController
    {
        public string Get()
        {
            
            return "Foobarbuzz";
        }
    }
}
