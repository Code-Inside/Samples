using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcRenderToString.Results
{
    public class ExcelResult : ActionResult
    {
        public string FileName { get; set; }
        public string Content { get; set; }

        public ExcelResult(string filename, string content)
        {
            this.FileName = filename;
            this.Content = content;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            WriteFile(this.FileName, "application/ms-excel", this.Content);
        }

        private static void WriteFile(string fileName, string contentType, string content)
        {

            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            context.Response.Charset = "";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = contentType;
            context.Response.Write(content);
            context.Response.End();
        }
    }
}
