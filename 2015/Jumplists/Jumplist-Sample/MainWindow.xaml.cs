using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shell;

namespace Jumplist_Sample
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var di = new DirectoryInfo("C://");
            var jt = new JumpTask
            {
                ApplicationPath = "C:\\Windows\\notepad.exe",
                Arguments = "C://",
                Description = "Run at " + "C://",
                Title = di.Name,
                CustomCategory = "Hello World"
            };

            JumpList.AddToRecentCategory(jt);
        }
    }
}
