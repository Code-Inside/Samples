using System.Web.Http;
using WebApiContrib.Formatting.Html;

namespace RazorWebApiOwin.Api
{
    // http://weblogs.asp.net/fredriknormen/archive/2012/07/17/asp-net-web-api-and-using-razor-the-next-step.aspx
    public class HomeController : ApiController
    {
        public Value GetValues()
        {
            // Option via return type: return new View("Home", null);
            return new Value() { Numbers = new int[] { 1, 2, 3 } };
        }
    }

    [View("Home")]
    public class Value
    {
        public int[] Numbers { get; set; }
    }
}