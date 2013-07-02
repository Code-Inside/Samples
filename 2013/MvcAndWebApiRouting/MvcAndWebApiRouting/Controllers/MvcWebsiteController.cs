using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAndWebApiRouting.Controllers
{
    public class MvcWebsiteController : Controller
    {
        //
        // GET: /MvcWebsite/

        public ActionResult Index()
        {
            ViewBag.UrlToAnotherMvcWebsiteController = Url.Action("Foobar", "AnotherMvcWebsite");
            ViewBag.AbsoluteUrlToAnotherMvcWebsiteController = Url.Action("Foobar", "AnotherMvcWebsite", null, "http");
            ViewBag.WebApiController = Url.RouteUrl("DefaultApi", 
                                                new { httproute = "", controller = "Foobar" });

            ViewBag.AbsoluteUrlToWebApiController = Url.RouteUrl("DefaultApi",
                                                new { httproute = "", controller = "Foobar" }, "http");


            return View();
        }

    }
}
