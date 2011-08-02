<%@ WebHandler Language="C#" Class="PdfHandler" %>

using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using java.io;
using org.xml.sax;
using org.apache.fop.apps;
using System.IO;


[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class PdfHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/pdf";                     
        FileInputStream input = new FileInputStream(context.Request.PhysicalApplicationPath+"helloworld.fo");
        InputSource source = new InputSource(input);

        java.io.ByteArrayOutputStream output = new ByteArrayOutputStream();

        Driver driver = new Driver(source, output);
        driver.setRenderer(Driver.RENDER_PDF);
        driver.run();
        output.close();

        sbyte[] Pdf = output.toByteArray();
        BinaryWriter bw = new BinaryWriter(context.Response.OutputStream);           
        for (int i = 0; i < Pdf.Length; i++)
        {
            bw.Write(Pdf[i]);
        }

        bw.Close();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}

