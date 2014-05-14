using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using MvcRssSample.Results;

namespace MvcRssSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Feed(string id)
        {
            var items = new List<SyndicationItem>();

            for (int i = 0; i < 10; i++)
            {
                string feedTitle = "Test Title " + i;

                var helper = new UrlHelper(this.Request.RequestContext);
                var url = helper.Action("Index", "Home", new { }, Request.IsSecureConnection ? "https" : "http");

                var feedPackageItem = new SyndicationItem(feedTitle, "Test Description " + i, new Uri(url));
                feedPackageItem.PublishDate = DateTime.Now;
                items.Add(feedPackageItem);
            }

            return new RssResult("Demo Feed", items);
        }   

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}