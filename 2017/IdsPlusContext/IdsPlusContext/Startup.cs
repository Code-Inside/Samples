using System.Linq;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.EntityFramework;
using IdsPlusContext.IdSStuff;
using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup(typeof(IdsPlusContext.Startup))]

namespace IdsPlusContext
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var factory = new IdentityServerServiceFactory()
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());
            factory.UserService = new Registration<IUserService>(typeof(ExternalRegistrationUserService));

            // "Normal Context"
            using (var context = new TestDbContext("Data Source=...;Initial Catalog=...;User ID=...;Password...;MultipleActiveResultSets=True"))
            {
                var tenants = context.Tenants.ToList();
            }

            factory.RegisterOperationalServices(new EntityFrameworkServiceOptions() { ConnectionString = "Data Source=...;Initial Catalog=...;User ID=...;Password...;MultipleActiveResultSets=True", Schema = "IdS" });
            app.UseIdentityServer(new IdentityServerOptions
            {
                SiteName = "Embedded IdentityServer",
                SigningCertificate = Certificate.Load(),
                Factory = factory,
                RequireSsl = false,
                AuthenticationOptions = new AuthenticationOptions
                {
                    EnableLocalLogin = false,
                    IdentityProviders = ConfigureIdentityProviders

                }
            });


        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Sign-in with Google",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "...",
                ClientSecret = "..."
            });

            //var wsFederation = new WsFederationAuthenticationOptions
            //{
            //    AuthenticationType = "windows",
            //    Caption = "Windows - A",
            //    SignInAsAuthenticationType = signInAsType,
            //    MetadataAddress = ConfigurationManager.AppSettings["Security.WindowsAuthProviderAddress"],
            //    Wtrealm = "urn:idsrv3",
            //};
            //app.UseWsFederationAuthentication(wsFederation);
        }
    }
}
