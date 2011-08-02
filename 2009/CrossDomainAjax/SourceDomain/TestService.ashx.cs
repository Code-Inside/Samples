using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.SessionState;

namespace SourceDomain
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    public class TestService : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["RequestCounter"] == null)
            { context.Session["RequestCounter"] = 1; }
            else
            {
                context.Session["RequestCounter"] = Convert.ToInt32(context.Session["RequestCounter"]) + 1;
            }

            context.Response.ContentType = "text/plain";

            string response = context.Request.Params["jsonp_callback"];
            response += "({\"response\":\"" + context.Session["RequestCounter"]  + " requests startet\"});";
            context.Response.Write(response);
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
