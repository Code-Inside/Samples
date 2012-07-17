using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication15.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult Test(string name, List<Foobar> foobars)
        {
            return View("Index");
        }

        public ActionResult About()
        {
            return View();
        }
    }


    public class Foobar
    {
        public string Buzz { get; set; }

        public int Foo { get; set; }

        public List<Foobar> Children { get; set; } 
    }
}
