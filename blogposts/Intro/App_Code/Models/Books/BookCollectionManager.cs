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
/// Summary description for BookcollectionManager
/// </summary>
public class BookCollectionManager
{
	public BookCollectionManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public BookCollection GetBookCollection(string query, int page)
    {
        BookCollection returnCollection = new BookCollection();

        for(int i = 0; i < 25; i++)
        {
            Book newBook = new Book();
            newBook.Title = "Buch " + i.ToString();
            newBook.Description = "Beschreibung" + i.ToString();
            newBook.PicUrl = "PicUrl " + i.ToString();

            returnCollection.Add(newBook);
        }

        return returnCollection;
    }
}
