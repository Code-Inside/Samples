using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using MyToolkit.Controls;
using MyToolkit.Multimedia;

namespace WPAppStudio.Controls
{
    /// <summary>
    /// YouTubePlayer
    /// </summary>
    public partial class YouTubePlayer : UserControl
    {
        private static string _videoId;
        private YouTubeButton _youTubeButton;

        public static readonly DependencyProperty VideoIdProperty =
            DependencyProperty.Register(
            "VideoId",
            typeof(string),
            typeof(YouTubePlayer),
            new PropertyMetadata("YouTubePlayer", OnVideoIdPropertyChanged));

        public string VideoId
        {
            get { return (string)GetValue(VideoIdProperty); }
            set { SetValue(VideoIdProperty, value); }
        }

        private static void OnVideoIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (YouTubePlayer)d;
            var value = (string)e.NewValue;
            _videoId = value;
            source.SetVideo(value);
        }

        public YouTubePlayer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create Video Player.
        /// </summary>
        /// <param name="video"></param>
        private void SetVideo(string video)
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
            {
                var interfaceType = profile.NetworkAdapter.IanaInterfaceType;

                if (interfaceType == 71)
                {
                    //Wifi
                    _youTubeButton = new YouTubeButton {YouTubeID = _videoId};
                    _youTubeButton.Click += youTubeButton_Click;

                    LayoutRoot.Children.Clear();
                    LayoutRoot.Children.Add(_youTubeButton);
                }
                else
                {
                    //Other
                    var web = new WebBrowser()
                        {
                            Height = 600,
                            IsScriptEnabled = true
                        };

                    web.NavigateToString(string.Format("<!doctype html>" +
                                                       "<html><head><title></title></head><body style=background-color:black;>" +
                                                       "<iframe height=\"600\" src=\"http://www.youtube.com/embed/{0}\" width=\"1000\"></iframe>" +
                                                       "</body></html>", video));

                    LayoutRoot.Children.Clear();
                    LayoutRoot.Children.Add(web);
                }
            }
        }

        private void youTubeButton_Click(object sender, RoutedEventArgs re)
        {
            YouTube.Play(_videoId, true, YouTubeQuality.Quality480P, e =>
            {
                if (e != null)
                    MessageBox.Show(e.Message);
            });
        }
    }
}
