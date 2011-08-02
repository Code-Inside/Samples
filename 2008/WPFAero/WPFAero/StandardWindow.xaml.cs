using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFAero
{
    /// <summary>
    /// Interaction logic for StandardWindow.xaml
    /// </summary>
    public partial class StandardWindow : Window
    {
        public StandardWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            AeroWindow window = new AeroWindow();
            window.Show();
            this.Close();
        }
    }
}
