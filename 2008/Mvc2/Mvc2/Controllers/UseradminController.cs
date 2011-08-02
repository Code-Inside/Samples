using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Mvc2.Filters;

namespace Mvc2.Controllers
{
    public class UseradminController : Controller
    {
        [RequiresRole(RoleToCheckFor="Admin")]
        public void Index()
        {
            MembershipUserCollection collection = Membership.GetAllUsers();
            RenderView("Index", collection);
        }

        [RequiresRole(RoleToCheckFor = "Admin")]
        public void Delete(string id)
        {
            Membership.DeleteUser(id);
            MembershipUserCollection collection = Membership.GetAllUsers();
            RenderView("Index", collection);
        }
    }
}
