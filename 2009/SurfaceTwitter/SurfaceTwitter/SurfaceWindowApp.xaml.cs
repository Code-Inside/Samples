using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using SurfaceTwitter.Services;
using System.Net;
using System.Security.Authentication;
using SurfaceTwitter.Model;
using System.Collections.ObjectModel;
using Path = System.IO.Path;

namespace SurfaceTwitter
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindowApp : SurfaceWindow
    {
        /// <summary>
        /// Twitter Service for communication through twitter
        /// </summary>
        private ITwitterService _twitterService;

        /// <summary>
        /// Dispatcher for UI Threading timed
        /// </summary>
        private DispatcherTimer _dispatcherTimer;

        /// <summary>
        /// The most recent stroke (used for undo operations).
        /// </summary>
        private Stroke mostRecentStroke;

        /// <summary>
        /// Tweets
        /// </summary>
        private ObservableCollection<Tweet> Tweets { get; set; }

        #region StandardMethods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindowApp()
        {
            InitializeComponent();

            // Add handlers for Application activation events
            AddActivationHandlers();

            this.Tweets = new ObservableCollection<Tweet>();
            this.TweetsList.DataContext = this.Tweets;
            this.InitTimer();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for Application activation events
            RemoveActivationHandlers();
        }

        /// <summary>
        /// Adds handlers for Application activation events.
        /// </summary>
        private void AddActivationHandlers()
        {
            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;
        }

        /// <summary>
        /// Removes handlers for Application activation events.
        /// </summary>
        private void RemoveActivationHandlers()
        {
            // Unsubscribe from surface application activation events
            ApplicationLauncher.ApplicationActivated -= OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed -= OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated -= OnApplicationDeactivated;
        }

        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
        #endregion

        #region Drawing Pad Specific Code

        /// <summary>
        /// Toggles the edit mode of a SurfaceInkCanvas between EraseByPoint and Ink.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="args">The arguments for the event.</param>
        private void EditModeClicked(object sender, RoutedEventArgs args)
        {
            SurfaceButton button = (SurfaceButton)sender;
            if (DrawingPadCanvas.EditingMode == SurfaceInkEditingMode.Ink)
            {
                DrawingPadCanvas.EditingMode = SurfaceInkEditingMode.EraseByPoint;

                // Load the new button image
                button.Content = LoadImageFromPath("Resources\\Erase.png");
            }
            else
            {
                DrawingPadCanvas.EditingMode = SurfaceInkEditingMode.Ink;

                // Load the new button image
                button.Content = LoadImageFromPath("Resources\\Draw.png");
            }
        }

        /// <summary>
        /// Handles the OnStrokeCollected event for SurfaceInkCanvas.
        /// </summary>
        /// <param name="sender">The SurfaceInkCanvas that raised the event.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs args)
        {
            mostRecentStroke = args.Stroke;
        }

        /// <summary>
        /// Handles the click event for the undo button.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">The arguments for the event.</param>
        void UndoClicked(object sender, RoutedEventArgs e)
        {
            if (mostRecentStroke != null)
            {
                DrawingPadCanvas.Strokes.Remove(mostRecentStroke);
                mostRecentStroke = null;
            }
        }

        /// <summary>
        /// Handles the tap event for the current color indicator.
        /// </summary>
        /// <param name="sender">The color wheel.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnCurrentColorTap(object sender, ContactEventArgs args)
        {
            // Replace the current color botton with the color wheel
            ColorWheel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the tap event for the color wheel.
        /// </summary>
        /// <param name="sender">The color wheel.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnColorWheelContactTap(object sender, ContactEventArgs args)
        {
            // Set the color on the CurrentColor indicator and on the SurfaceInkCanvas
            Color color = GetPixelColor(args.Contact);

            // Black means the user touched the transparent part of the wheel. In that 
            // case, leave the color set to its current value
            if (color != Colors.Black)
            {
                DrawingPadCanvas.DefaultDrawingAttributes.Color = color;
                CurrentColor.Fill = new SolidColorBrush(color);
            }

            // Set editing mode to ink
            DrawingPadCanvas.EditingMode = SurfaceInkEditingMode.Ink;

            // Update the button image
            EditModeButton.Content = LoadImageFromPath("Resources\\Draw.png");

            // Replace the color wheel with the current color button
            ColorWheel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handles the ContactDownEvent for the color wheel and the current color indicator.
        /// </summary>
        /// <param name="sender">The color wheel that raised the event.</param>
        /// <param name="args">The arguments for the event.</param>
        private void OnColorSelectionPreviewContactDown(object sender, ContactEventArgs args)
        {
            // Capture the contact and handle the event 
            IInputElement element = sender as IInputElement;
            if (element != null && Contacts.CaptureContact(args.Contact, element))
            {
                args.Handled = true;
            }
        }

        //==========================================================//
        /// <summary>
        /// Gets the color of a specific pixel.
        /// </summary>
        /// <param name="pt">The point from which to get a color.</param>
        /// <returns>The color of the point.</returns>
        private System.Windows.Media.Color GetPixelColor(Contact contact)
        {
            // Translate the point according to whatever transforms are on the color wheel.
            Point rawPoint = contact.GetPosition(ColorWheel);
            Point transformedPoint = ColorWheel.RenderTransform.Transform(rawPoint);

            // The point is outside the color wheel. Return black.
            if (transformedPoint.X < 0 || transformedPoint.X >= ColorWheel.Source.Width ||
                transformedPoint.Y < 0 || transformedPoint.Y >= ColorWheel.Source.Height)
            {
                return Colors.Black;
            }

            // The point is inside the color wheel. Find the color at the point.
            CroppedBitmap cb = new CroppedBitmap(ColorWheel.Source as BitmapSource, new Int32Rect((int)transformedPoint.X, (int)transformedPoint.Y, 1, 1));
            byte[] pixels = new byte[4];
            cb.CopyPixels(pixels, 4, 0);
            return Color.FromRgb(pixels[2], pixels[1], pixels[0]);
        }

        /// <summary>
        /// Loads an image.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Image LoadImageFromPath(string path)
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            Image image = new Image();
            image.Source = (ImageSource)converter.ConvertFromString(path);
            return image;
        }

        #endregion Drawing Pad Specific Code

        #region MyHandlers
        /// <summary>
        /// Eventhandler for Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NetworkCredential cred = new NetworkCredential(this.Username.Text, this.Password.Password);
                this._twitterService = new TwitterService(cred);
                this.Login.Visibility = Visibility.Hidden;

                this.ReloadTweets();
                this.TweetsList.Visibility = Visibility.Visible;
                this.Writer.Visibility = Visibility.Visible;
            }
            catch (AuthenticationException)
            {
                this.HandleAuthenticationException();
            }

        }

        private void ReloadTweets()
        {
            if (this._twitterService != null && this._twitterService.IsAuthenticated)
            {
                foreach (Tweet tweet in this._twitterService.GetTweets().ToList())
                {
                    if (this.Tweets.Count(x => x.Id == tweet.Id) == 0)
                    {

                        var insertbefor = this.Tweets.FirstOrDefault(x => x.CreatedOn < tweet.CreatedOn);
                        if (insertbefor == null)
                            this.Tweets.Add(tweet);
                        else
                            this.Tweets.Insert(this.Tweets.IndexOf(insertbefor), tweet);
                    }
                }

            }
        }

        private void InitTimer()
        {
            this._dispatcherTimer = new DispatcherTimer();
            this._dispatcherTimer.Interval = TimeSpan.FromMinutes(1);
            this._dispatcherTimer.Tick += new EventHandler(delegate(object s, EventArgs a)
            {
                this.ReloadTweets();
            });
            this._dispatcherTimer.Start();
        }

        private void Tweet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string imagePath = "";

                if (mostRecentStroke != null)
                {
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)this.DrawingPadCanvas.Width, (int)DrawingPadCanvas.Height, 96d, 96d, PixelFormats.Default);
                    rtb.Render(DrawingPadCanvas);
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    imagePath = Path.GetTempFileName() + ".jpg";
                    FileStream fs = File.Open(imagePath, FileMode.Create);
                    encoder.Save(fs);
                    fs.Close();
                }

                if (imagePath == "") this._twitterService.SendTweet(this.InputTweet.Text);
                else this._twitterService.SendTweet(this.InputTweet.Text, imagePath);
            }
            catch (AuthenticationException)
            {
                this.HandleAuthenticationException();
            }
        }

        private void HandleAuthenticationException()
        {
            this.Username.Style = (Style)FindResource("Error");
            UserNotifications.RequestNotification("Authentication Failed",
                                                  "Wrong Username/Email and password combination.",
                                                  TimeSpan.FromSeconds(6));
        }

        #endregion
    }
}