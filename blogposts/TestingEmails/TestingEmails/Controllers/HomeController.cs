using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace TestingEmails.Controllers
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

        public ActionResult SendMail()
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage("from@test.de", "to@test.de");
            mailMessage.Subject = "Test-Subject";
            mailMessage.Body = "Test-Body";
            smtpClient.Send(mailMessage);

            return View("Index");
        }
    }
}
