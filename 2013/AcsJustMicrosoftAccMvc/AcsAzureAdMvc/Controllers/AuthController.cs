using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace AcsAzureAdMvc.Controllers
{
    /// <summary>
    /// Everything based on WS-Federation: http://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html
    /// </summary>
    public class AuthController : Controller
    {
        public ActionResult Index()
        {
            // Microsoft Account Login

            // 2 Option - use hosted Loginpage from ACS or host your own...

            // based on Azure ACS Portal - Login Page Integration - Option 1:
            // Problem: wfresh=0 not supported - logout is :/
            return new RedirectResult("https://codeinside.accesscontrol.windows.net:443/v2/wsfederation?wa=wsignin1.0&wtrealm=urn:sample:wifless");

            // Option 2: Use the link from the JSON - but needed more coding.
            // But wfresh=0 is supported!
            //return new RedirectResult("https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=https%3a%2f%2faccesscontrol.windows.net%2f&wreply=https%3a%2f%2fcodeinside.accesscontrol.windows.net%2fv2%2fwsfederation&wp=MBI_FED_SSL&wctx=cHI9d3NmZWRlcmF0aW9uJnJtPXVybiUzYXNhbXBsZSUzYXdpZmxlc3Mmcnk9aHR0cCUzYSUyZiUyZmxvY2FsaG9zdCUzYTg4MTclMmZBdXRoJTJmQ2FsbGJhY2s1&wfresh=0");
        }

        [ValidateInput(false)]
        public ActionResult Callback(string wresult, string wa)
        {
            if (wa == "wsignin1.0")
            {
                var wrappedToken = XDocument.Parse(wresult);
                var binaryToken = wrappedToken.Root.Descendants("{http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd}BinarySecurityToken").First();
                var tokenBytes = Convert.FromBase64String(binaryToken.Value);
                var token = Encoding.UTF8.GetString(tokenBytes);

                var result = AcsAuthorizationTokenValidator.RetrieveClaims(token,
                                                                          "https://codeinside.accesscontrol.windows.net/",
                                                                          new List<string>() { "urn:sample:wifless" });

                var name = result.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier);

                FormsAuthentication.SetAuthCookie(name.Value, false);
            }
            else if (wa == "wsignoutcleanup1.0")
            {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

    }
}
