using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OwinWithWebApi.Startup))]

namespace OwinWithWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<LoggerMiddleware>();

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "{controller}");
            app.UseWebApi(config);

            app.UseHandlerAsync((req, res) =>
            {
                res.ContentType = "text/plain";
                return res.WriteAsync("Hello World!");
            });
        }
    }

    public class HomeController : ApiController
    {
        public int[] GetValues()
        {
            return new[] {1, 2, 3};
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
