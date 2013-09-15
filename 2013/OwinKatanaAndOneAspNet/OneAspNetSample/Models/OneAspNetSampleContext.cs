using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OneAspNetSample.Models
{
    public class OneAspNetSampleContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<OneAspNetSample.Models.OneAspNetSampleContext>());

        public OneAspNetSampleContext() : base("name=OneAspNetSampleContext")
        {
        }

        public System.Data.Entity.DbSet<OneAspNetSample.Models.Product> Product { get; set; }

    }
}