using System.Web;
using System.Web.Mvc;

namespace MvcControllerHttpContextTests.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = this.ControllerContext.HttpContext.Request.UserAgent;
            HttpCookie cookie = new HttpCookie("HelloTDDInCookie");
            this.Response.Cookies.Add(cookie);
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
