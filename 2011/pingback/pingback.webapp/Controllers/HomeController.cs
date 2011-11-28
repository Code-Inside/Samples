using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pingback.webapp.Controllers
{
    using System.Globalization;
    using System.Text.RegularExpressions;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult SendPingback()
        {
            // source must be available from the target url (e.g. localhost doesn´t work)
            string source = "http://www.bizzbingo.com/what-is/ravendb";

            // source must contain the target link... otherwise: Error.
            string target = "http://code-inside.de/blog-in/2011/11/05/use-ravendb-as-embedded-filebase/";
            Pingback.Send(new Uri(source), new Uri(target));
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
