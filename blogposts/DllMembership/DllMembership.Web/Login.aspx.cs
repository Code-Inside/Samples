using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace DllMembership.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DoLogin(object sender, EventArgs e)
        {
            DllMembership.Lib.User returnUser = AppUtil.Login(this.Username.Text, this.Password.Text);
            if (returnUser.IsLoggedIn == true)
            {
                Context.Response.Redirect("~/Default.aspx");
            }
            else
            {
                Error.Visible = true;
            }
        }
    }
}
