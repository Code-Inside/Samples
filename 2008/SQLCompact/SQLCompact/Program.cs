using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLCompact
{
    class Program
    {
        static void Main(string[] args)
        {
            Database context = new Database(@"Data Source=C:\Users\rmu\Documents\Visual Studio 2008\Projects\Blogposts\SQLCompact\SQLCompact\Database.sdf");
            context.Companies.InsertOnSubmit(new Company { Id = Guid.NewGuid(), Name = "Code-Inside" });
            context.SubmitChanges();
        }
    }
}
