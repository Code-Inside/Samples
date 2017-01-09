/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer3.Host.Config
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "WPF WebView Client Sample",
                    ClientId = "wpf.webview.client",
                    Flow = Flows.Implicit,

                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Roles,
                        Constants.StandardScopes.Address,
                        "read",
                        "write"
                    },

                    ClientUri = "https://identityserver.io",

                    RequireConsent = false,
                    AllowRememberConsent = false,

                    RedirectUris = new List<string>
                    {
                        "oob://localhost/wpf.webview.client",
                    },
                },
                new Client
                {
                    ClientId = "webapp",
                    ClientName = "WebApp Demo",
                    Flow = Flows.Implicit,
                    RedirectUris = new List<string>()
                    {
                      "http://localhost:57884/"
                    },
                    AllowedScopes = new List<string>
                    {   "openid",  "all_claims"
                    }
                },
                new Client
                {
                    ClientId = "wpfapp",
                    ClientName = "WPFApp Demo",
                    Flow = Flows.HybridWithProofKey,
                    ClientSecrets = new List<Secret>() { new Secret("secret".Sha256()) },
                 AllowedScopes = new List<string>
                 {   "openid",  "all_claims", "offline_access"
                 },
                     RequireConsent = false,
                     RedirectUris = new List<string>() { "http://localhost/wpf.hybrid" }
                },



            };
        }
    }
}