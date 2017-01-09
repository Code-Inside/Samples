using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;

namespace IdentityTest.IdServerHost
{
    //public class WindowsGrantValidator : ICustomGrantValidator
    //{
    //    private IUserService _users;

    //    public WindowsGrantValidator(IUserService users)
    //    {
    //        _users = users;
    //    }

    //    public string GrantType
    //    {
    //        get
    //        {
    //            return "windows";
    //        }
    //    }

    //    public async Task<CustomGrantValidationResult> ValidateAsync(ValidatedTokenRequest request)
    //    {
    //        var param = request.Raw.Get("win_token");
    //        if (string.IsNullOrWhiteSpace(param))
    //        {
    //            return await Task.FromResult(new CustomGrantValidationResult("Missing parameter win_token."));
    //        }

    //        var principal = TryValidateToken(param, request.Options.SigningCertificate);

    //        var nameIdentifierClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

    //        if (nameIdentifierClaim == null)
    //        {
    //            return await Task.FromResult(new CustomGrantValidationResult("Missing NameIdentifier claim in win_token."));
    //        }

    //        var authenticationResult = await AuthenticateUserAsync(nameIdentifierClaim.Value, principal.Claims);

    //        var customGrantResult = new CustomGrantValidationResult()
    //        {
    //            IsError = authenticationResult.IsError,
    //            Error = authenticationResult.ErrorMessage,
    //            ErrorDescription = authenticationResult.ErrorMessage,
    //            Principal = authenticationResult.User
    //        };

    //        return await Task.FromResult(customGrantResult);

    //    }

    //    private ClaimsPrincipal TryValidateToken(string token, X509Certificate2 signingCert)
    //    {
    //        var tokenHandler = new JwtSecurityTokenHandler();

    //        var tokenValidationParams = new TokenValidationParameters
    //        {
    //            ValidAudience = "urn:idsrv3",
    //            ValidIssuer = "urn:windowsauthentication",
    //            IssuerSigningKey = new X509SecurityKey(signingCert)
    //        };

    //        Microsoft.IdentityModel.Tokens.SecurityToken securityToken;
    //        var claimsPrincipial = tokenHandler.ValidateToken(token, tokenValidationParams, out securityToken);

    //        return claimsPrincipial;
    //    }

    //    private async Task<AuthenticateResult> AuthenticateUserAsync(string providerId, IEnumerable<Claim> claimsFromExternalProvider)
    //    {
    //        var nameClaim = claimsFromExternalProvider.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Name);

    //        var claims = new List<Claim>();
    //        if (nameClaim != null)
    //        {
    //            claims.Add(nameClaim);
    //        }

    //        var context = new ExternalAuthenticationContext()
    //        {
    //            ExternalIdentity = new ExternalIdentity()
    //            {
    //                Provider = "windows",
    //                ProviderId = providerId,
    //                Claims = claims
    //            },
    //            SignInMessage = new SignInMessage()
    //        };

    //        await _users.AuthenticateExternalAsync(context);

    //        return context.AuthenticateResult;
    //    }
    //}
}