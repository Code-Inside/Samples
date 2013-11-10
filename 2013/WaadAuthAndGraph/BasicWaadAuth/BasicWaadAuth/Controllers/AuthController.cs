using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Auth10.WindowsAzureActiveDirectory;
using Auth10.WindowsAzureActiveDirectory.Infrastructure;
using Newtonsoft.Json.Linq;

namespace BasicWaadAuth.Controllers
{
    public class AuthController : Controller
    {
        private string AzureAdAppClientId;
        private string AzureAdAppClientSecret;
        private string AzureAdAppUri;
        private IssuingAuthority AzureAdAuthroAuthority;

        public AuthController()
        {
            AzureAdAppUri = "http://localhost:47828/";
            AzureAdAppClientId = "515e8337-2a81-421a-bf76-cbc78ff89288";
            AzureAdAppClientSecret = "sYEVBHHMM4kQNv2NOT6x0c55sogupaknnr3gdX9cptg=";

            // Fix for ID4175 & WIF10201  http://www.cloudidentity.com/blog/2013/02/08/multitenant-sts-and-token-validation-4/
            AzureAdAuthroAuthority = new IssuingAuthority("WAAD");
            // Issuer = Azure Ad Tenant 
            AzureAdAuthroAuthority.Issuers.Add("https://sts.windows.net/3351acfe-7e1b-4e9b-b587-f34bfa2e128a/");
            AzureAdAuthroAuthority.Thumbprints.Add("3464C5BDD2BE7F2B6112E2F08E9C0024E33D9FE0");

            // Thumbprint can be read via this code:
            // ia = ValidatingIssuerNameRegistry.GetIssuingAuthority("https://login.windows.net/TENANTID/FederationMetadata/2007-06/FederationMetadata.xml");

        }

        public ActionResult Index()
        {
            // Develop Multitenant apps http://msdn.microsoft.com/en-us/library/windowsazure/dn151789.aspx - before "https://login.windows.net/3351acfe-7e1b-4e9b-b587-f34bfa2e128a/";
            string issuer = "https://login.windows.net/common/";

            // returnUrl can be passed in wctx parameter with rm=0&id=passive&ru=%2fHome%2fAbout
            var redirectUrl = string.Format(@"{0}wsfed?wa=wsignin1.0&wtrealm={1}", issuer, Url.Encode(AzureAdAppUri));

            return Redirect(redirectUrl);
        }


        [ValidateInput(false)]
        public async Task<ActionResult> Callback(string wresult, string wa, string wctx)
        {
            // http://www.tecsupra.com/blog/system-identitymodel-manually-parsing-the-saml-token/
            var wrappedToken = XDocument.Parse(wresult);
            var requestedSecurityToken = wrappedToken.Root.Descendants("{http://schemas.xmlsoap.org/ws/2005/02/trust}RequestedSecurityToken").First();
            var asssertion = requestedSecurityToken.DescendantNodes().First();

            var xmlTextReader = asssertion.CreateReader();

            var securityTokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();

            // Fix for ID1032 http://blog.fabse.net/2013/01/10/id1032-at-least-one-audienceuri-must-be-specified-2/
            securityTokenHandlers.Configuration.AudienceRestriction.AllowedAudienceUris.Add(new Uri(AzureAdAppUri));
            securityTokenHandlers.Configuration.CertificateValidationMode = X509CertificateValidationMode.None;
            securityTokenHandlers.Configuration.CertificateValidator = X509CertificateValidator.None;

            securityTokenHandlers.Configuration.IssuerNameRegistry = new ValidatingIssuerNameRegistry(AzureAdAuthroAuthority);

            SecurityToken token = securityTokenHandlers.ReadToken(xmlTextReader);

            var viewModel = new CallbackViewModel();

            var claimsIdentity = securityTokenHandlers.ValidateToken(token);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            viewModel.Claims = claimsPrincipal.Claims.ToList();

            var tenantId =
                claimsPrincipal.Claims.Single(x => x.Type == "http://schemas.microsoft.com/identity/claims/tenantid")
                               .Value;

            var waadRequest = new HttpClient();

            string postData = "grant_type=client_credentials";
            postData += "&resource=" + HttpUtility.UrlEncode("https://graph.windows.net");
            postData += "&client_id=" + HttpUtility.UrlEncode(AzureAdAppClientId);
            postData += "&client_secret=" + HttpUtility.UrlEncode(AzureAdAppClientSecret);
            var waadRequestContent = new StringContent(postData, System.Text.Encoding.ASCII, "application/x-www-form-urlencoded");

            string postUrl = string.Format("https://login.windows.net/{0}/oauth2/token?api-version=1.0", tenantId);

            var waadResult = await waadRequest.PostAsync(postUrl, waadRequestContent);

            waadResult.EnsureSuccessStatusCode();

            var result = await waadResult.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(result);
            var accessToken = jObject.SelectToken("access_token");

            var graph = new DirectoryGraph(tenantId, accessToken.Value<string>());

            string nextPageUrl;

            var user = graph.GetUsers(out nextPageUrl);

            viewModel.Users = user;

            return View(viewModel);
        }

    }

    public class CallbackViewModel
    {
        public List<Claim> Claims { get; set; }
        public List<User> Users { get; set; }
    }
}