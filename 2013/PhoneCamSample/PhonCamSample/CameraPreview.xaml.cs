using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Devices;
using Microsoft.Phone.Controls;

namespace PhonCamSample
{
    public partial class CameraPreview : PhoneApplicationPage
    {
        private PhotoCamera cam;

        public CameraPreview()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            VideoCanvas.Visibility = Visibility.Visible;
            Result.Visibility = Visibility.Collapsed;
            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary))
            {
                cam = new PhotoCamera(CameraType.Primary);
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureCompleted);
                videoBrush.SetSource(cam);
                previewTransform.Rotation = cam.Orientation;
            }
        }

        private void cam_CaptureCompleted(object sender, ContentReadyEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
                                            {
                                                BitmapImage tempImage = new BitmapImage();
                                                tempImage.SetSource(RotateStream(e.ImageStream, Convert.ToInt32(cam.Orientation)));

                                                Result.Source = tempImage;
                                                Result.Visibility = Visibility.Visible;
                                                VideoCanvas.Visibility = Visibility.Collapsed;
                                            });

        }

        /// <summary>
        /// Picture needs to be rotated - dunno why (because of the ImageRotateTransform... sucks...)
        /// Origin: http://timheuer.com/blog/archive/2010/09/23/working-with-pictures-in-camera-tasks-in-windows-phone-7-orientation-rotation.aspx
        /// </summary>
        private Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return stream;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                cam.Dispose();

                cam.CaptureImageAvailable -= cam_CaptureCompleted;
            }
        }

        private void like_Click(object sender, EventArgs e)
        {
            cam.CaptureImage();
        }

    }
}