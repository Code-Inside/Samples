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
using System.Web.Mvc;

namespace Mvc2.Filters
{
    /// <summary>
    /// See: http://blog.wekeroad.com/2008/03/12/aspnet-mvc-securing-your-controller-actions/
    /// Checks the User’s authentication using FormsAuthentication
    /// and redirects to the Login Url for the application on fail
    /// </summary>
    public class RequiresAuthenticationAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(FilterExecutingContext filterContext)
        {

            //redirect if not authenticated
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // my simple Version
                filterContext.HttpContext.Response.Redirect("~/Login");

                // Robs "better" Version
                //use the current url for the redirect
                //string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;

                //send them off to the login page
                //string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                //string loginUrl = FormsAuthentication.LoginUrl + redirectUrl;
                //filterContext.HttpContext.Response.Redirect(loginUrl, true);

            }

        }
    }
}
