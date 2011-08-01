using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTabboxen.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index(string id)
        {
            if (id == string.Empty)
            {
                id = "Tab1";
            }

            // AJAX Request
            if(Request.IsAjaxRequest())
            {
                return View(id);
            }
            
            // Normal GET Request
            ViewData["ActivTab"] = id;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        //public ViewResult Tab1()
        //{
        //    return View();
        //}

        //public ViewResult Tab2()
        //{
        //    return View();
        //}

        //public ViewResult Tab3()
        //{
        //    return View();
        //}

    }
}
