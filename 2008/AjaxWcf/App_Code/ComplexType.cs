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
using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// Summary description for ComplexType
/// </summary>
[DataContract]
public class ComplexType
{

	public ComplexType()
	{
	}
    [DataMember]
    public string Title { get; set; }
    [DataMember]
    public string Subtitle { get; set; }
    [DataMember]
    public int Number { get; set; }


    public static List<ComplexType> GetList()
    {
        return new List<ComplexType>() 
        {
            new ComplexType{ Subtitle = "hello", Number=5, Title="askjd"},
            new ComplexType{ Subtitle = "hello", Number=5, Title="askjd"},
            new ComplexType{ Subtitle = "hello", Number=5, Title="askjd"},
            new ComplexType{ Subtitle = "hello", Number=5, Title="askjd"},
            new ComplexType{ Subtitle = "hello", Number=5, Title="askjd"},
            new ComplexType{ Subtitle = "hello", Number=5, Title="askjd"}
        };

    }

    public static ComplexType GetOne()
    {
        return new ComplexType { Subtitle = "hello", Number = 5, Title = "askjd" };
    }
}
