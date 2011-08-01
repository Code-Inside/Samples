using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Mvc2.Filters;

namespace Mvc2.Controllers
{
    public class LoginController : Controller
    {
        public void Index()
        {
            RenderView("Index");
        }

        public void Login()
        {
            string login = this.Request.Form["Login"];
            string password = this.Request.Form["Password"];

            if (Membership.ValidateUser(login, password))
            {
                FormsAuthentication.SetAuthCookie(login, false);
                RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Failur"] = true;
                RenderView("Index", ViewData);
            }
        }
        
        [RequiresAuthentication]
        public void Logout()
        {
            this.HttpContext.Session.Abandon();
            this.HttpContext.Session.Clear();
            FormsAuthentication.SignOut();
            RedirectToAction("Index", "Home");
        }

        public void Register()
        {
            if (this.Request.Form.Count > 0)
            {
                string login = this.Request.Form["Login"];
                string email = this.Request.Form["Email"];
                string password = this.Request.Form["Password"];

                if (Membership.GetUser(login) == null)
                {
                    MembershipUser createdUser = Membership.CreateUser(login, password, email);
                    FormsAuthentication.SetAuthCookie(login, true);
                    RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["Failur"] = true;
                    RenderView("Register", ViewData);
                }

            }
            else
            {
                RenderView("Register");
            }
        }

        public void PasswordRecovery()
        {
            if (this.Request.Form.Count > 0)
            {
                string login = this.Request.Form["Login"];
                MembershipUser requestUser = Membership.GetUser(login);
                if (requestUser != null)
                {
                    ViewData["NewPassword"] = requestUser.ResetPassword();
                    RenderView("PasswordRecovery", ViewData);
                }
                else
                {
                    ViewData["Failur"] = true;
                    RenderView("PasswordRecovery", ViewData);
                }

            }
            else
            {
                RenderView("PasswordRecovery");
            }
        }
    }
}
