using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace DynVCard
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class VCardService : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            VCard myVCard = new VCard();
            myVCard.FirstName = "Robert";
            myVCard.LastName = "Mühsig";
            myVCard.Role = "Blogger";

            context.Response.ContentType = "text/x-vcard";
            context.Response.ContentEncoding = System.Text.Encoding.Default;
            context.Response.Write(myVCard.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
