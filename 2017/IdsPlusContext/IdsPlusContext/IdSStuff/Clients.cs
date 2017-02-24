using System.Collections.Generic;
using System.Configuration;
using IdentityServer3.Core.Models;

namespace IdsPlusContext.IdSStuff
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "webapp",
                    ClientName = "WebApp Demo",
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>()
                    {
                        ConfigurationManager.AppSettings["Security.WebAppClientUrl"]
                    },
                    AllowedScopes = new List<string>
                    {   "openid",  "all_claims"
                    },
                    RequireConsent = false,
                },
                new Client
                {
                    ClientName = "WPFApp Demo",
                    ClientId = "wpfapp",
                    Flow = Flows.HybridWithProofKey,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = new List<string>
                    {
                        "something://localhost/wpf.hybrid"
                    },

                    AllowedScopes = new List<string>
                    {   "openid",  "all_claims", "offline_access"
                    },
                    RequireConsent = false,

                    AccessTokenType = AccessTokenType.Reference
                },



            };
        }
    }
}