using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client.Document;

namespace RavenHQ.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var documentStore = new DocumentStore { Url="https://1.ravenhq.com/databases/Robert0Muehsig-CodeInside", ApiKey="c4c3c135-e202-4a9e-b7d7-baa67298722a" })
			{
				documentStore.Initialize();

			    using(var session = documentStore.OpenSession())
			    {
			        var album = session.Load<Album>("albums/609");
                    ViewBag.Message = "Album Nr. 609 on RavenHQ cost about " + album.Price;
			    }


			}

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public class Album
    {
        public string AlbumArtUrl { get; set; }
        public string Id { get; set; }
        public double Price { get; set; }
    }

}
