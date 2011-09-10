using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryDataDash.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewBag.Data = "ImportantMessage";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
