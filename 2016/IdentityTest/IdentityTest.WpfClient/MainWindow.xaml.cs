using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json.Linq;
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

        private string accessToken = "";
        private string refreshToken = "";

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var client = OidcClient();

                var test = await client.LoginAsync(true);

                accessToken = test.AccessToken;
                refreshToken = test.RefreshToken;
                Result.Text = "Success!";
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("AccessToken: " + test.AccessToken);
                sb.AppendLine("RefreshToken: " + test.RefreshToken);
                sb.AppendLine("AccessTokenExpiration: " + test.AccessTokenExpiration);

                sb.AppendLine("Claims... ");

                foreach (var claim in test.Claims)
                {
                    sb.AppendLine("Claim: " + claim.Type + " - " + claim.Value);
                }

                Detailed.Text = sb.ToString();
            }
            catch (Exception exc)
            {
                Result.Text = "Error: " + exc.Message;
                Detailed.Text = "";
            }

        }

        private static OidcClient OidcClient()
        {
            var authority = ConfigurationManager.AppSettings["Security.Authority"];
            var options = new OidcClientOptions(
                authority: authority,
                clientId: "wpfapp", 
                clientSecret: "secret",
                scope: "openid all_claims offline_access",
                webView: new WinFormsWebView(),
                redirectUri: "something://localhost/wpf.hybrid");

            var client = new OidcClient(options);
            client.Options.UseFormPost = true;

            return client;
        }

        private async void ButtonApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new HttpClient();
                client.SetBearerToken(accessToken);

                var json = await client.GetStringAsync(ConfigurationManager.AppSettings["Security.WebAppClientUrl"] + "api/values");
                var test = JArray.Parse(json).ToString();

                Api.Text = test;
            }
            catch (Exception exc)
            {
                Api.Text = "Error " + exc.Message;
            }

        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            var client = OidcClient();

            var result = await client.RefreshTokenAsync(this.refreshToken);

            if (result.Success)
            {
                this.accessToken = result.AccessToken;
                this.refreshToken = result.RefreshToken;
            }
           
        }
    }
}
