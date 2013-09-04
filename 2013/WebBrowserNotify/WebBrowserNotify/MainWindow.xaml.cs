using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebBrowserNotify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var scripting = new ScriptingContext();
            scripting.NotifyInvoked += (sender, args) =>
                                           {
                                               this.Result.Text += args.Result;
                                           };

            this.Host.ObjectForScripting = scripting;
            this.Host.Navigate("http://localhost:40414/Index.html");
        }
    }

    [ComVisible(true)]
    public class ScriptingContext
    {
        public delegate void NotifyInvokedEventHandler(object sender, NotifyInvokedEventArgs e);

        public event NotifyInvokedEventHandler NotifyInvoked;

        public void notify(string result)
        {
            if (NotifyInvoked != null)
            {
                var args = new NotifyInvokedEventArgs { Result = result };
                NotifyInvoked(this, args);
            }
        }
    }

    public class NotifyInvokedEventArgs : EventArgs
    {
        public string Result { get; set; }

    }
}
