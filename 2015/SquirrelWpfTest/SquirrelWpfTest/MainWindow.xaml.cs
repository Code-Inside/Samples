using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Squirrel;

namespace SquirrelWpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

       private async void ButtonUpdateQuestion_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var mgr = new UpdateManager(ConfigurationManager.AppSettings["UpdatePath"]))
                {
                    var release = await mgr.CheckForUpdate();
                    this.UpdateQuestionResult.Text = "Possible Update: " + release.FutureReleaseEntry.Version.ToString();
                }
            }
            catch (Exception test)
            {
                this.UpdateQuestionResult.Text = test.Message;
            }
        }

        private async void ButtonUpdateDo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var mgr = new UpdateManager(ConfigurationManager.AppSettings["UpdatePath"]))
                {
                    var release = await mgr.UpdateApp();
                   
                    this.UpdateDoResult.Text = release.EntryAsString;
                }
            }
            catch (Exception test)
            {
                this.UpdateDoResult.Text = "Updated to: " + test.Message;
            }
        }
    }
}
