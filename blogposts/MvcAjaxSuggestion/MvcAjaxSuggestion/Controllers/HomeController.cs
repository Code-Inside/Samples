using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAjaxSuggestion.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public JsonResult Search(string q, int limit)
        {
            List<string> result = new List<string>();

            for(int i = 0; i <= limit; i++)
            {
                result.Add(q + "xxx...");
            }

            return Json(result);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
