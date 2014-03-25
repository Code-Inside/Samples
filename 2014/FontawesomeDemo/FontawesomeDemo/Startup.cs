using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FontawesomeDemo.Startup))]
namespace FontawesomeDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
