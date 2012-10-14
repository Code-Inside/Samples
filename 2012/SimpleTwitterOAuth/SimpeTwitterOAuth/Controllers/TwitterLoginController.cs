using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.AspNet.Clients;

namespace SimpeTwitterOAuth.Controllers
{
    public class TwitterLoginController : Controller
    {
        // Callback after Twitter Login
        public ActionResult Callback()
        {
            DotNetOpenAuth.AspNet.Clients.TwitterClient client = new TwitterClient(ConfigurationManager.AppSettings["twitterConsumerKey"], ConfigurationManager.AppSettings["twitterConsumerSecret"]);

            var result = client.VerifyAuthentication(this.HttpContext);

            return RedirectToAction("Index", "Home");
        }

        // Point Login URL to this Action
        public ActionResult Login()
        {
            DotNetOpenAuth.AspNet.Clients.TwitterClient client = new TwitterClient(ConfigurationManager.AppSettings["twitterConsumerKey"], ConfigurationManager.AppSettings["twitterConsumerSecret"]);

            UrlHelper helper = new UrlHelper(this.ControllerContext.RequestContext);
            var result = helper.Action("Callback", "TwitterLogin", null, "http");

            client.RequestAuthentication(this.HttpContext, new Uri(result));

            return new EmptyResult();
        }
    }
}
