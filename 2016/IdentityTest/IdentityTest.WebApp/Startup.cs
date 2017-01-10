using System.Configuration;
using IdentityServer3.AccessTokenValidation;
using IdentityTest.WebApp;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace IdentityTest.WebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["Security.Authority"],
                RequiredScopes = new[] { "openid" }
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "cookies",
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions()
            {
                AuthenticationType = "oidc",
                SignInAsAuthenticationType = "cookies",
                Authority = ConfigurationManager.AppSettings["Security.Authority"],
                ClientId = "webapp",
                RedirectUri = ConfigurationManager.AppSettings["Security.RedirectUri"],
                ResponseType = "id_token",
                Scope = "openid all_claims"
            });
        }
    }
}
