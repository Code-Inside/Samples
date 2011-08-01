using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcFileUpload.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            ViewData["Message"] = file.FileName + " - " + file.ContentLength.ToString();

            return View("Index");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
