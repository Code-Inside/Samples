using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Owin.Hosting;

namespace SelfHostAndCef
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            string baseAddress = "http://localhost:9000/";

            webApp = WebApp.Start<Startup>(url: baseAddress);
           
        }

        public IDisposable webApp { get; set; }

        ~App()
        {
            webApp.Dispose();
        }
    }
}
