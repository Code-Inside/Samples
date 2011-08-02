using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRequestValidation.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string htmlInput)
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";
            ViewData["HtmlInput"] = htmlInput;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
