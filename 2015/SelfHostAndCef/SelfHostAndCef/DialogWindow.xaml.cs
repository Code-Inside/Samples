using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SelfHostAndCef
{
    public class OneOffixxWebViewHostEventArgs : EventArgs
    {
        public string Json { get; set; }
    }
    public class OneOffixxWebViewHost
    {
        public delegate void OneOffixxWebViewHostEventHandler(object sender, OneOffixxWebViewHostEventArgs e);

        public event OneOffixxWebViewHostEventHandler OneOffixxWebViewHostInvoked;
        public void done(string json)
        {
            if (OneOffixxWebViewHostInvoked != null)
            {
                OneOffixxWebViewHostEventArgs args = new OneOffixxWebViewHostEventArgs();
                args.Json = json;
                OneOffixxWebViewHostInvoked(this, args);
            }
        }
    }


    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    /// 
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
            this.Browser.Address = "http://localhost:9000/api/v1/Demo";

            var hostElement = new OneOffixxWebViewHost();
            this.Browser.RegisterJsObject("oneoffixxWebViewHost", hostElement);

            hostElement.OneOffixxWebViewHostInvoked += HostElement_OneOffixxWebViewHostInvoked;
        }

        private void HostElement_OneOffixxWebViewHostInvoked(object sender, OneOffixxWebViewHostEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                this.DialogResult = true;
                this.Close();
            });
        }   
    }
}
