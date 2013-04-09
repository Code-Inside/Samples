using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Xml;
using Microsoft.IdentityModel.Tokens.JWT;

namespace AcsAzureAdMvc.Controllers
{
    /// <summary>
    /// Based on this sample: http://code.msdn.microsoft.com/AAL-Native-Application-to-fd648dcf/sourcecode?fileId=62849&pathId=697488104
    /// </summary>
    public class AcsAuthorizationTokenValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token">= Token</param>
        /// <param name="allowedAudience">= Azure ACS Namespace</param>
        /// <param name="issuers">= RPs in ACS</param>
        /// <returns></returns>
        public static ClaimsPrincipal RetrieveClaims(string token, string issuer, List<string> allowedAudiences)
        {
            JWTSecurityTokenHandler tokenHandler = new JWTSecurityTokenHandler();

            // Set the expected properties of the JWT token in the TokenValidationParameters 
            TokenValidationParameters validationParameters = new TokenValidationParameters()
                                                                 {
                                                                     AllowedAudiences = allowedAudiences,
                                                                     ValidIssuer = issuer,
                                                                     SigningToken = new X509SecurityToken(new X509Certificate2(GetSigningCertificate(issuer + "FederationMetadata/2007-06/FederationMetadata.xml")))
                                                                 };

            return tokenHandler.ValidateToken(token, validationParameters); 
        }

        private static byte[] GetSigningCertificate(string metadataAddress)
        {
            if (metadataAddress == null)
            {
                throw new ArgumentNullException(metadataAddress);
            }

            using (XmlReader metadataReader = XmlReader.Create(metadataAddress))
            {
                MetadataSerializer serializer = new MetadataSerializer()
                                                    {
                                                        CertificateValidationMode = X509CertificateValidationMode.None
                                                    };

                EntityDescriptor metadata = serializer.ReadMetadata(metadataReader) as EntityDescriptor;

                if (metadata != null)
                {
                    SecurityTokenServiceDescriptor stsd = metadata.RoleDescriptors.OfType<SecurityTokenServiceDescriptor>().First();

                    if (stsd != null)
                    {
                        X509RawDataKeyIdentifierClause clause = stsd.Keys.First().KeyInfo.OfType<X509RawDataKeyIdentifierClause>().First();

                        if (clause != null)
                        {
                            return clause.GetX509RawData();
                        }
                        throw new Exception("The SecurityTokenServiceDescriptor in the metadata does not contain the Signing Certificate in the <X509Certificate> element");
                    }
                    throw new Exception("The Federation Metadata document does not contain a SecurityTokenServiceDescriptor");
                }
                throw new Exception("Invalid Federation Metadata document");
            }
        } 
    }
}