using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lib;

namespace MvcLibConfig.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = ReadFromConfig.Get();

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
