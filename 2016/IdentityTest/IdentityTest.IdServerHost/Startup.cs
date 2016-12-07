using Configuration;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Host.Config;
using IdentityTest.IdServerHost;
using Microsoft.Owin;
using Microsoft.Owin.Security.WsFederation;
using Owin;

[assembly: OwinStartup(typeof(IdentityTest.Startup))]

namespace IdentityTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var factory = new IdentityServerServiceFactory()
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());
            factory.UserService = new Registration<IUserService>(typeof(ExternalRegistrationUserService));

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
            var wsFederation = new WsFederationAuthenticationOptions
            {
                AuthenticationType = "windows",
                Caption = "Windows",
                SignInAsAuthenticationType = signInAsType,
                
                MetadataAddress = "http://localhost:58773",
                Wtrealm = "urn:idsrv3"
            };
            app.UseWsFederationAuthentication(wsFederation);
        }
    }
}
