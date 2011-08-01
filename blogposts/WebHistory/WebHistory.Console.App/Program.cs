using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebHistory.Data;
using WebHistory.Model;
using WebHistory.Service;
using System.Windows.Forms;
using System.Drawing;

namespace WebHistory.Console.App
{
    
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Console.WriteLine("Create your own web history images.");
            System.Console.WriteLine("Type the URL (with http://...");
            string url = System.Console.ReadLine();
            System.Console.WriteLine(@"Save to location (e.g. C:\Images\)");
            string path = System.Console.ReadLine();
            IArchiveService service = new WebArchiveService();
            Website result = service.Load(url);
            System.Console.WriteLine("WebArchive Sites found: " + result.ArchiveWebsites.Count);
            WebBrowser wb = new WebBrowser();  
            int i = 0;
            foreach (ArchiveWebsite site in result.ArchiveWebsites)
            {
             i++;
             System.Console.WriteLine("Save image (Date " + site.Date.ToShortDateString() + ") number: " + i.ToString());
             wb.ScrollBarsEnabled = false;  
             wb.ScriptErrorsSuppressed = true;
             wb.Navigate(site.ArchiveUrl);  
             while (wb.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }     
             wb.Width = wb.Document.Body.ScrollRectangle.Width;  
             wb.Height = wb.Document.Body.ScrollRectangle.Height;  
  
             Bitmap bitmap = new Bitmap(wb.Width, wb.Height);  
             wb.DrawToBitmap(bitmap, new Rectangle(0, 0, wb.Width, wb.Height));  
             bitmap.Save(path + site.Date.Year.ToString() + "_" + site.Date.Month.ToString() + "_" + site.Date.Day.ToString() + ".bmp");  
            }
            wb.Dispose();

            System.Console.WriteLine(result.Url);
        }
    }
}
