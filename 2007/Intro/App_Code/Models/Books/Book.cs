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

/// <summary>
/// Summary description for Book
/// </summary>
public class Book
{
	public Book()
	{

	}

    private string _title;
    private string _description;
    private string _picUrl;

    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
        }
    }

    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            _description = value;
        }
    }

    public string PicUrl
    {
        get
        {
            return _picUrl;
        }
        set
        {
            _picUrl = value;
        }
    }
}
