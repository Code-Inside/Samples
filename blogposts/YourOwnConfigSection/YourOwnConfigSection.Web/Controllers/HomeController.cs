using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YourOwnConfigSection.Infrastructure;

namespace YourOwnConfigSection.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";
            CodeInsideConfig config = CodeInsideConfig.GetConfig();
            ViewData["webUrl"] = config.WebUrl;
            ViewData["startedOn"] = config.StartedOn;

            string authors = "";
            foreach (CodeInsideConfigAuthor author in config.Authors)
            {
                authors += author.Name + ",";    
            }

            ViewData["authors"] = authors;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
