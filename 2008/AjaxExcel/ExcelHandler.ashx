<%@ WebHandler Language="C#" Class="ExcelHandler" %>

using System;
using System.Web;

public class ExcelHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {

        if (context.Request.Params["openXls"] == "true")
        {
            // display data
            if (context.Session["ExcelData"] != null)
            {
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.ContentEncoding = System.Text.Encoding.Default;
                context.Response.Write(context.Session["ExcelData"]);
            }
            else
            {
                context.Response.Write("session empty");
            }
        }
        else
        {
            // save data in session
            context.Response.ContentType = "application/vnd.ms-excel";
            string data = "";
            using (System.IO.StreamReader reader = new System.IO.StreamReader(context.Request.InputStream))
            {
                data = reader.ReadToEnd();
            }

            context.Session.Add("ExcelData", data);
        }
    }
    public bool IsReusable {
        get {
            return true;
        }
    }

}