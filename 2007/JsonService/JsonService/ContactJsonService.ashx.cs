using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Serialization;
namespace JsonService
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ContactJsonService : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {      
            ContactListManager manager = new ContactListManager();
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string output = "var ContactList = " + serializer.Serialize(manager.GetAllContacts());

            context.Response.ContentType = "application/json";
            context.Response.Write(output);
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
