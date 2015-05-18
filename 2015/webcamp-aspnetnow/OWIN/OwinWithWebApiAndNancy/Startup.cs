using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Nancy;
using Nancy.Owin;
using Owin;
using System.Diagnostics;

[assembly: OwinStartup(typeof(OwinWithWebApiAndNancy.Startup))]

namespace OwinWithWebApiAndNancy
{
    /// <summary>
    /// /... => Nancy 404 Error Page
    /// /nancy = Nancy Module
    /// /api/home => WebApi
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<LoggerMiddleware>();

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}");
            app.UseWebApi(config);

            app.UseNancy(new NancyOptions());
        }
    }

    public class HomeController : ApiController
    {
        public int[] GetValues()
        {
            return new[] { 1, 2, 3 };
        }
    }

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/nancy/"] = x =>
            {
                var env = (IDictionary<string, object>)Context.Items[Nancy.Owin.NancyMiddleware.RequestEnvironmentKey];

                var requestBody = (Stream)env["owin.RequestBody"];
                var requestHeaders = (IDictionary<string, string[]>)env["owin.RequestHeaders"];
                var requestMethod = (string)env["owin.RequestMethod"];
                var requestPath = (string)env["owin.RequestPath"];
                var requestPathBase = (string)env["owin.RequestPathBase"];
                var requestProtocol = (string)env["owin.RequestProtocol"];
                var requestQueryString = (string)env["owin.RequestQueryString"];
                var requestScheme = (string)env["owin.RequestScheme"];

                var responseBody = (Stream)env["owin.ResponseBody"];
                var responseHeaders = (IDictionary<string, string[]>)env["owin.ResponseHeaders"];

                var owinVersion = (string)env["owin.Version"];
                var cancellationToken = (CancellationToken)env["owin.CallCancelled"];

                var uri = (string)env["owin.RequestScheme"] + "://" + requestHeaders["Host"].First() +
                  (string)env["owin.RequestPathBase"] + (string)env["owin.RequestPath"];

                if (env["owin.RequestQueryString"] != "")
                    uri += "?" + (string)env["owin.RequestQueryString"];

                return string.Format("{0} {1}", requestMethod, uri);
            };
        }
    }




    public class LoggerMiddleware : OwinMiddleware
    {
        public LoggerMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            Trace.TraceInformation("Middleware begin");
            await this.Next.Invoke(context);
            Trace.TraceInformation("Middleware end");
        }
    }
}
