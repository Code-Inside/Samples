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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibWithWpfStuff;

namespace CakeExampleWithWpf
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var testList = new List<string>();
            testList.Add("hallo welt");
            testList.Add("hello world");

            MessageBox.Show(LibWithoutWpfStuff.UsingJsonNet.ToJson(testList));
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            LibWithWpfStuff.TestView window = new TestView();
            window.Show();
        }
    }
}
