using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSessionStateServer.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(this.Session["Foobar"] != null) {
                ViewData["Message"] = "Session: " + this.Session["Foobar"];
            }
            else {
                ViewData["Message"] = "Session empty";
            }
            return View();
        }

        public ActionResult SetSession()
        {
            this.Session["Foobar"] = "Yeah - " + DateTime.Now.ToShortTimeString() + ":" + DateTime.Now.Second.ToString();
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
