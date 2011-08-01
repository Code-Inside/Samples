using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MvcConfig.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = WebConfigurationManager.AppSettings["HelloWorldKey"];

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
