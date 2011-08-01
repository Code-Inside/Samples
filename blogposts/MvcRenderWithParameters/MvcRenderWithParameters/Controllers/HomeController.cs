using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRenderWithParameters.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult News(int? count)
        {
            // Default Value
            int newsCount = 5;

            // Check if Parameter has value
            if(count.HasValue) {
                newsCount = count.Value;
            }

            // Prepare News
            List<string> newsEntry = new List<string>();

            for(int i = 0; i <= newsCount; i++)
            {
                newsEntry.Add("Test " + i);
            }

            ViewData["News"] = newsEntry;

            return View();
        }
    }
}
