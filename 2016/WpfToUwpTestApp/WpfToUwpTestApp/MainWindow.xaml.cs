using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;

namespace WpfToUwpTestApp
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

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

            key.CreateSubKey("WpfToUwpTestApp");
            key = key.OpenSubKey("WpfToUwpTestApp", true);


            key.CreateSubKey("ItWorks");
            key = key.OpenSubKey("ItWorks", true);

            key.SetValue("ItWorks", "true");
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string appFolder = System.IO.Path.Combine(roaming, "WpfToUwpTestApp");

            string file = System.IO.Path.Combine(appFolder, "Test.txt");

            if (Directory.Exists(appFolder) == false)
            {
                Directory.CreateDirectory(appFolder);
            }

            File.WriteAllText(file, "Hello World!");
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.google.com");
        }
    }
}
