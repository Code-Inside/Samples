using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvcjcrop.Helper;

namespace mvcjcrop.Controllers
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

        public ImageResult PostPicture(int x, int y, int h, int w)
        {
            Image image = Image.FromFile(Path.Combine(this.Request.PhysicalApplicationPath,"Content/flowers.jpg"));
            Bitmap cropedImage = new Bitmap(w, h, image.PixelFormat);
            Graphics g = Graphics.FromImage(cropedImage);

            Rectangle rec = new Rectangle(0, 0,
                                w,
                                h);

            g.DrawImage(image, rec, x, y, w, h, GraphicsUnit.Pixel);
            image.Dispose();
            g.Dispose();

            return new ImageResult { Image = cropedImage, ImageFormat = ImageFormat.Jpeg }; 
        }
    }
}
