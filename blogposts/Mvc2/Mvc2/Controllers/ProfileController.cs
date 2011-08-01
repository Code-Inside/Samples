using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Mvc2.Filters;

namespace Mvc2.Controllers
{
    public class ProfileController : Controller
    {

        [RequiresAuthentication]
        public void Index()
        {
            MembershipUser currentlyUser = Membership.GetUser();
            ViewData["Login"] = currentlyUser.UserName;
            ViewData["Email"] = currentlyUser.Email;

            RenderView("Index", ViewData);
        }

        [RequiresAuthentication]
        public void Edit()
        {
            if (this.Request.Form.Count > 0)
            {
                MembershipUser currentlyUser = Membership.GetUser();
                currentlyUser.Email = this.Request.Form["Email"];

                if (this.Request.Form["OldPassword"] != "" && this.Request.Form["NewPassword"] != "")
                {
                    currentlyUser.ChangePassword(this.Request.Form["OldPassword"], this.Request.Form["NewPassword"]);
                }
                Membership.UpdateUser(currentlyUser);
                RedirectToAction("Index");
            }
            else
            {
                MembershipUser currentlyUser = Membership.GetUser();
                ViewData["Login"] = currentlyUser.UserName;
                ViewData["Email"] = currentlyUser.Email;

                RenderView("Edit", ViewData);
            }
        }
    }
}
