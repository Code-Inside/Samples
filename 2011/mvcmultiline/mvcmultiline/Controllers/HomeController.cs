using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvcmultiline.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult Multiline(string input)
        {
            ViewBag.MultilineRaw = input;

            List<string> eachLine = input.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ViewBag.MultilineSplitted = eachLine;

            return View("Index");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
