using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace OpenID.Controllers
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

        public ActionResult OpenID()
        {
            ViewData["message"] = "You are not logged in";
            var openid = new OpenIdRelyingParty();

            IAuthenticationResponse response = openid.GetResponse();
            if (response != null && response.Status == AuthenticationStatus.Authenticated)
                ViewData["message"] = "Success! Identifier: " + response.ClaimedIdentifier;

            return View("OpenID");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OpenID(string openid_identifier)
        {
            var openid = new OpenIdRelyingParty();
            IAuthenticationRequest request = openid.CreateRequest(Identifier.Parse(openid_identifier));

            return request.RedirectingResponse.AsActionResult();
        }
    }
    
}
