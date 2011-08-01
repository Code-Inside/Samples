using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcLinkingDoItRight.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult Blog(string year, string month, string day)
        {
            ViewData["Message"] = year + " " + month + " " + day;

            return View("Index");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
