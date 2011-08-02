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
    /// Interaction logic for Aero.xaml
    /// </summary>
    public partial class AeroWindow : Window
    {
        public AeroWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // This can't be done any earlier than the SourceInitialized event:
            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            StandardWindow window = new StandardWindow();
            window.Show();
            this.Close();
        }
    }
}
