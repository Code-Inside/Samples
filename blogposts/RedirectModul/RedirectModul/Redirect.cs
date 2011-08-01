using System.Web;
public class Redirect : IHttpModule
{
    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
        context.BeginRequest += new System.EventHandler(context_BeginRequest);
    }

    void context_BeginRequest(object sender, System.EventArgs e)
    {
        if(HttpContext.Current.Request.Url.LocalPath.ToLower() == "/google")
        {
            HttpContext.Current.Response.Redirect("http://google.de");
        }
    }

}
