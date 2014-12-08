using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Live;

namespace LiveSample.Controllers
{
    public class AuthController : Controller
    {
        private static readonly string[] scopes =
        new string[] { 
            "wl.signin", 
            "wl.basic", 
            "wl.calendars" };

        private readonly LiveAuthClient authClient;

        public AuthController()
        {
            authClient = new LiveAuthClient("0000000044121F5D", "b6lolb9jiF-zaMtp8KXcRNJ7ACz37SuK", "http://blogpostsample.localtest.me/Auth/Redirect");
        }

        public async Task<ActionResult> Index()
        {
            LiveLoginResult loginStatus = await this.authClient.InitializeWebSessionAsync(HttpContext);
            switch (loginStatus.Status)
            {
                case LiveConnectSessionStatus.Expired:
                case LiveConnectSessionStatus.Unknown:
                    string reAuthUrl = authClient.GetLoginUrl(scopes);
                    return new RedirectResult(reAuthUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Redirect()
        {
            var result = await authClient.ExchangeAuthCodeAsync(HttpContext);
            if (result.Status == LiveConnectSessionStatus.Connected)
            {
                var client = new LiveConnectClient(this.authClient.Session);
                LiveOperationResult meResult = await client.GetAsync("me");
                LiveOperationResult mePicResult = await client.GetAsync("me/picture");
                LiveOperationResult calendarResult = await client.GetAsync("me/calendars");

                ViewBag.Name = meResult.Result["name"].ToString();
                ViewBag.PhotoLocation = mePicResult.Result["location"].ToString();
                ViewBag.CalendarJson = calendarResult.RawResult;
            }


            return View();
        }
    }
}