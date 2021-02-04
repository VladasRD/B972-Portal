using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Xml;
using Microsoft.IdentityModel.Tokens;

namespace Box.Common
{
    public class StartupHelper
    {

        /*
         * To create new certificates:
         * OPEN SLL
            https://benjii.me/2017/06/creating-self-signed-certificate-identity-server-azure/
            https://slproweb.com/products/Win32OpenSSL.html
            openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout identity.key -out identity.crt -subj "/CN=IdentityKey" -days 3650
            openssl pkcs12 -export -out identity.pfx -inkey identity.key -in identity.crt -certfile identity.crt
         */

        /// <summary>
        /// Gets the certificate key to sign the JWT tokens from the tokrn store.
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetCertificateFromStore(string certThumbPrint)
        {
            X509Certificate2 cert = null;
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    certThumbPrint,
                    false);
                // Get the first cert with the thumbprint
                if (certCollection.Count > 0)
                {
                    cert = certCollection[0];
                }
            }
            return cert;
        }

        /// <summary>
        /// Gets the certificate key to sign the JWT tokens from the server path.
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetCertificateFromPath(IHostingEnvironment host, string password = "box", string fileName = "identity.pfx")
        {
            return new X509Certificate2(Path.Combine(host.ContentRootPath, fileName), password);
        }

        /// <summary>
        /// Gets the asymetric key from a fixed hard-coded XML.
        /// </summary>
        /// <returns>The RSA Key</returns>
        public static RsaSecurityKey GetFixedRSAKey(string keyXml)
        {
            RsaSecurityKey key;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);
            FromXml(RSA, keyXml);
            key = new RsaSecurityKey(RSA);
            return key;
        }

        // see: https://github.com/dotnet/corefx/issues/23686
        private static void FromXml(RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }
    }
}
