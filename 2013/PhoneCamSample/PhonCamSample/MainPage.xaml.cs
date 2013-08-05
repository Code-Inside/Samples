using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using PhonCamSample.Resources;

namespace PhonCamSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        CameraCaptureTask cameraCaptureTask;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            cameraCaptureTask.Show();
        }

        private void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if(e.TaskResult == TaskResult.OK)
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(e.ChosenPhoto);
                Result.Source = image;
                MessageBox.Show("Foobar");
            }
        }


        private void ApplicationBarIconButtonApi_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CameraPreview.xaml", UriKind.Relative));
        }
    }


}