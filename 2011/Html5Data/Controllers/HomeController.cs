using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Html5Data.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ShoppingCart cart = new ShoppingCart();
            return View(cart);
        }

        [HttpPost]
        public ActionResult Update(string price)
        {
            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Products()
        {
            List<Item> items = new List<Item>();
            items.Add(new Item() { Name = "Produkt 1", Price = 6.99});
            items.Add(new Item() { Name = "Produkt 2", Price = 12.99 });
            items.Add(new Item() { Name = "Produkt 3", Price = 2.99 });
            items.Add(new Item() { Name = "Produkt 4", Price = 0.99 });

            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }

    public class ShoppingCart
    {
        public double Sum { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
