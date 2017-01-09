using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IdentityModel;
using IdentityModel.Client;
using IdentityModel.Extensions;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Logging;
using IdentityTest.WpfClient.OidcStuff;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace IdentityTest.WpfClient
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var authority = "http://localhost:56482/";
            var options = new OidcClientOptions(
                authority: authority,
                clientId: "wpfapp",
                clientSecret: "secret",
                scope: "openid all_claims offline_access",
                webView: new WinFormsWebView(), 
                redirectUri: "http://localhost/wpf.hybrid");

            var client = new OidcClient(options);
           

            var state = await client.PrepareLoginAsync();
            
            var test = await client.LoginAsync(false);




        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            LoginWebView _login;
            _login = new LoginWebView();
            _login.Done += _login_Done;

            _login.Owner = this;

            var settings = new OidcSettings
            {
                Authority = "http://localhost:56482/",
                ClientId = "wpfapp",
                ClientSecret = "secret",
                RedirectUri = "http://localhost/wpf.hybrid",
                Scope = "openid all_claims offline_access",
                LoadUserProfile = false
            };

            await _login.LoginAsync(settings);
        }

        void _login_Done(object sender, LoginResult e)
        {
            if (e.Success)
            {
                var sb = new StringBuilder(128);

                foreach (var claim in e.User.Claims)
                {
                    sb.AppendLine($"{claim.Type}: {claim.Value}");
                }

                sb.AppendLine();

                sb.AppendLine($"Identity token: {e.IdentityToken}");
                sb.AppendLine($"Access token: {e.AccessToken}");
                sb.AppendLine($"Access token expiration: {e.AccessTokenExpiration}");
                sb.AppendLine($"Refresh token: {e?.RefreshToken ?? "none" }");
                string res = sb.ToString();
                // IdentityTextBox.Text = sb.ToString();
            }
            else
            {
                string error = e.ErrorMessage;
                //  IdentityTextBox.Text = e.ErrorMessage;
            }


        }
    }
}
