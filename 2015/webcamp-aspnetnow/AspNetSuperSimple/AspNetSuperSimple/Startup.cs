using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AspNetSuperSimple.Startup))]
namespace AspNetSuperSimple
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
