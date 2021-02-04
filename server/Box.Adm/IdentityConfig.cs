using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace Box.Adm
{

    // https://identityserver4.readthedocs.io/en/release/

    public class IdentityConfig
    {

        public string DEFAULT_CLIENT_URL { get; set; }
        public string IDENTITY_URL { get; set; }
        public string TOKEN_CERT_BLUEPRINT { get; set; }
        public string API_NAME { get; set; }
        public string API_SECRET { get; set; }

        public string OAUTH_GOOGLE_CLIENT_ID { get; set; }
        public string OAUTH_GOOGLE_CLIENT_SECRET { get; set; }

        public string OAUTH_MS_CLIENT_ID { get; set; }
        public string OAUTH_MS_CLIENT_SECRET { get; set; }

        public bool AUTO_CREATE_EXTERNAL_USERS { get; set;}

        /// <summary>
        /// The Identity resources.
        /// This resources are returned with the token id.
        /// </summary>
        public IdentityResource[] IdentityResources = new IdentityResource[] {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource() { Name = "role", UserClaims = new string[] { "role" } }
        };

        /// <summary>
        /// The APIs resources.
        /// This apis are served and protected by this server.
        /// </summary>
        public ApiResource[] ApiResources {
            get {
                return new ApiResource[] { new ApiResource(API_NAME) { UserClaims = new string[] { "role" } } };
            }
        }


        /// <summary>
        /// This clients are allowed to connect at this server.
        /// </summary>
        public Client[] Clients
        {
            get
            {
                return new Client[] {
            
                    // JavaScript Defautt Client
                    new Client
                    {
                        ClientId = "js",
                        ClientName = "JavaScript Client",
                        AllowedGrantTypes = GrantTypes.Implicit,


                        ClientSecrets = { new Secret(API_SECRET.Sha256()) },
                        
                        RequireConsent = false,
                        
                        AllowAccessTokensViaBrowser = true,
                        AlwaysIncludeUserClaimsInIdToken = true,

                        RedirectUris =           { DEFAULT_CLIENT_URL + "/index.html", DEFAULT_CLIENT_URL + "/silent-refresh.html" },
                        AllowedCorsOrigins =     { DEFAULT_CLIENT_URL },
                        PostLogoutRedirectUris = { DEFAULT_CLIENT_URL + "/bye" },

                        // this scopes can be used by this client
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,  // to login
                            IdentityServerConstants.StandardScopes.Profile, // to get name, etc..                            
                            IdentityServerConstants.StandardScopes.Email,   // email
                            "role",                                         // to get user roles
                            API_NAME,                                       // api
                        },

                        AccessTokenLifetime = 7200, // Access token life time in seconds
                        IdentityTokenLifetime = 7200 // Identity token life time in seconds
                    }
                };                
            }            
        }
    }
}
