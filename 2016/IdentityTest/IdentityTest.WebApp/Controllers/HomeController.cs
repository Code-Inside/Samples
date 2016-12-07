using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace IdentityTest.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [Authorize]
        public ActionResult Secure()
        {
            ViewBag.Title = "Secure Page";

            var claims = ClaimsPrincipal.Current.Claims;

            ViewBag.Claims = claims;

            return View();
        }
    }
}
