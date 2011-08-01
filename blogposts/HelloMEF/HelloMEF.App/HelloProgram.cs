using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using HelloMEF.App;
using System.IO;
using System.Reflection;

namespace HelloMEF.App
{
    public class HelloProgram
    {
        [Import(typeof(IHelloService))]
        public List<IHelloService> Services { get; set; }

        public HelloProgram()
        {
            this.Services = new List<IHelloService>();

            if (!Directory.Exists("PlugIns"))
            {
                Directory.CreateDirectory("PlugIns");
            }

            AggregatingComposablePartCatalog catalog = new AggregatingComposablePartCatalog();
            catalog.Catalogs.Add(new AttributedAssemblyPartCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new DirectoryPartCatalog("PlugIns"));
            
            CompositionContainer container = new CompositionContainer(catalog.CreateResolver());
            container.AddPart(this);
            container.Compose();
        }

        public void WriteHelloGreetings()
        {
            Console.WriteLine();
            Console.WriteLine("Writing Greetings...");

            foreach (IHelloService srv in Services)
            {
                Console.WriteLine(srv.GetHelloMessage());
            }

            Console.WriteLine("... powered by MEF");
        }
    }

    [Export(typeof(IHelloService))]
    public class MEFHelloService : IHelloService
    {
        #region IHelloService Members

        public string GetHelloMessage()
        {
            return "Hello MEF!";
        }

        #endregion
    }

}
