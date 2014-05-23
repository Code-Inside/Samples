using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace imageupload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
