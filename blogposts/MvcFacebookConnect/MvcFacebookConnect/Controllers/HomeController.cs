using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Session;
using System.Configuration;
using Facebook.Rest;

namespace MvcFacebookConnect.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ConnectSession session = new ConnectSession(ConfigurationManager.AppSettings["Facebook_API_Key"], ConfigurationManager.AppSettings["Facebook_API_Secret"]);
            if(session.IsConnected())
            {
                Api facebook = new Api(session);

                ViewData["Message"] = "Hello, " + facebook.Users.GetInfo().name;
            }
            else
            {
                ViewData["Message"] = "Login with Facebook!";
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult XdReceiver()
        {
            return View();
        }
    }
}
