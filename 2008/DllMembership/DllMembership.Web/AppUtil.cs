using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DllMembership.Lib;

namespace DllMembership.Web
{
    public static class AppUtil
    {
        public static User GetActiveUser()
        {
        if(HttpContext.Current.User.Identity.IsAuthenticated == false)
            {
                return new User() { IsLoggedIn = false };
            }
            else
            {
                MembershipService service = new MembershipService();
                User returnValue = service.GetUser(HttpContext.Current.User.Identity.Name);
                returnValue.IsLoggedIn = true;
                return returnValue;
            }
        }

        public static User Login(string username, string password)
        {
            MembershipService service = new MembershipService();
            User returnValue = service.Login(username, password);
            if (returnValue.IsLoggedIn)
            {
                FormsAuthentication.SetAuthCookie(returnValue.Name, true);
                returnValue.IsLoggedIn = true;
            }
            else
            {
                returnValue.IsLoggedIn = false;
            }

            return returnValue;
        }
    }
}
