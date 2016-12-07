using Configuration;
using IdentityServer.WindowsAuthentication.Configuration;
using IdentityServer.WindowsAuthentication.Services;
using IdentityTest.WinAuth;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace IdentityTest.WinAuth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWindowsAuthenticationService(new WindowsAuthenticationOptions
            {
                IdpReplyUrl = "http://localhost:56482/was",
                SigningCertificate = Certificate.Load(),
                EnableOAuth2Endpoint = false,
            });
        }
    }
}
