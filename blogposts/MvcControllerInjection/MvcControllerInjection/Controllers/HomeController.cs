using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcControllerInjection.Models;

namespace MvcControllerInjection.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private IFooService _fooService;

        public HomeController(IFooService fooService)
        {
            this._fooService = fooService;
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!" + this._fooService.Bar();

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
