using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace JwtSampleApp
{
    // Code source is from this awesome blog: 
    // http://pfelix.wordpress.com/2012/11/27/json-web-tokens-and-the-new-jwtsecuritytokenhandler-class/
    class Program
    {
        static void Main(string[] args)
        {
            var securityKey = GetBytes("ThisIsAnImportantStringAndIHaveNoIdeaIfThisIsVerySecureOrNot!");

            var tokenHandler = new JwtSecurityTokenHandler();

            // Token Creation
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, "Pedro"),
                            new Claim(ClaimTypes.Role, "Author"), 
                        }),
                TokenIssuerName = "self",
                AppliesToAddress = "http://www.example.com",
                Lifetime = new Lifetime(now, now.AddMinutes(2)),
                SigningCredentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(securityKey),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Generate Token and return string
            var tokenString = tokenHandler.WriteToken(token);
            Console.WriteLine(tokenString);
            
            // Token Validation
            var validationParameters = new TokenValidationParameters()
            {
                AllowedAudience = "http://www.example.com",
                SigningToken = new BinarySecretSecurityToken(securityKey),
                ValidIssuer = "self"
            };

            // from Token to ClaimsPrincipal - easy!
            var principal = tokenHandler.ValidateToken(tokenString, validationParameters);

            Console.WriteLine(principal.Claims.Single(x => x.Type == ClaimTypes.Name).Value);

            Console.ReadLine();
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;

        }
    }
}
