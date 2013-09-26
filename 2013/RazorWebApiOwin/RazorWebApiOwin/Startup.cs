using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using WebApiContrib.Formatting.Html.Configuration;
using WebApiContrib.Formatting.Html.Formatters;
using WebApiContrib.Formatting.Html.Locators;
using WebApiContrib.Formatting.Razor;

[assembly: OwinStartup(typeof(RazorWebApiOwin.Startup))]
namespace RazorWebApiOwin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.Formatters.Add(new HtmlMediaTypeViewFormatter());

            GlobalViews.DefaultViewParser = new RazorViewParser();
            GlobalViews.DefaultViewLocator = new RazorViewLocator();

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "{controller}");
            config.Formatters.Add(new HtmlMediaTypeViewFormatter());

            app.UseWebApi(config);

            app.UseWelcomePage();
        }
    }
}
