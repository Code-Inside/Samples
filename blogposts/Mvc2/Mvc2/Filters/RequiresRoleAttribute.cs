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
    /// Checks the User’s role using FormsAuthentication
    /// and throws and UnauthorizedAccessException if not authorized
    /// </summary>
    public class RequiresRoleAttribute : ActionFilterAttribute
    {

        public string RoleToCheckFor { get; set; }

        public override void OnActionExecuting(FilterExecutingContext filterContext)
        {
            //redirect if the user is not authenticated
            if (!String.IsNullOrEmpty(RoleToCheckFor))
            {

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
                else
                {
                    bool isAuthorized = filterContext.HttpContext.User.IsInRole(this.RoleToCheckFor);
                    if (!isAuthorized)
                        throw new UnauthorizedAccessException("You are not authorized to view this page");
                }
            }
            else
            {
                throw new InvalidOperationException("No Role Specified");
            }
        }
    }
}
