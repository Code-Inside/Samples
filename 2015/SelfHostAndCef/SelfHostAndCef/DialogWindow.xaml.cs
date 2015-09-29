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
    public class SampleWebViewHostEventArgs : EventArgs
    {
        public string Json { get; set; }
    }
    public class SampleWebViewHost
    {
        public delegate void SampleWebViewHostEventHandler(object sender, SampleWebViewHostEventArgs e);

        public event SampleWebViewHostEventHandler SampleWebViewHostInvoked;
        public void done(string json)
        {
            if (SampleWebViewHostInvoked != null)
            {
                SampleWebViewHostEventArgs args = new SampleWebViewHostEventArgs();
                args.Json = json;
                SampleWebViewHostInvoked(this, args);
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

            var hostElement = new SampleWebViewHost();
            this.Browser.RegisterJsObject("sampleWebViewHost", hostElement);

            hostElement.SampleWebViewHostInvoked += HostElementSampleWebViewHostInvoked;
        }

        private void HostElementSampleWebViewHostInvoked(object sender, SampleWebViewHostEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("From Browser: " + e.Json);
                
                this.DialogResult = true;
                this.Close();
            });
        }   
    }
}
