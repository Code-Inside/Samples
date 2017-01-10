using System;
using System.Collections.Generic;
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

        private string token = "";

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var authority = "http://localhost:56482/";
                var options = new OidcClientOptions(
                    authority: authority,
                    clientId: "wpfapp",
                    clientSecret: "secret",
                    scope: "openid all_claims",
                    webView: new WinFormsWebView(),

                    redirectUri: "http://localhost/wpf.hybrid");

                var client = new OidcClient(options);
                client.Options.UseFormPost = true;

                var state = await client.PrepareLoginAsync();

                var test = await client.LoginAsync(true);

                token = test.AccessToken;

                Result.Text = "Success!";
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("AccessToken: " + test.AccessToken);
                sb.AppendLine("RefreshToken: " + test.RefreshToken);

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

        private async void ButtonApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new HttpClient();
                client.SetBearerToken(token);

                var json = await client.GetStringAsync("http://localhost:57884/api/values");
                var test = JArray.Parse(json).ToString();

                Api.Text = test;
            }
            catch (Exception exc)
            {
                Api.Text = "Error " + exc.Message;
            }

        }
    }
}
