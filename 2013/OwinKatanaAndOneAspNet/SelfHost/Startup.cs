using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using SelfHost;

[assembly: OwinStartup(typeof(Startup))]

namespace SelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<LoggerMiddleware>();

            app.UseWelcomePage();
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
