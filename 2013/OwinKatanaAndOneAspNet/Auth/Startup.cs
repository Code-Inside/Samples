using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Auth.Startup))]
namespace Auth
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}
