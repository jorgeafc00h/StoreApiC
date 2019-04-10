using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreApi.Identity
{
    public class Config
    {
        public static IEnumerable<Client> Clients
        {
            get
            {
                return new List<Client>
                {
                        new Client
                        {
                            ClientId = "spa",
                            AllowedGrantTypes = GrantTypes.Implicit,
                            AllowAccessTokensViaBrowser = true,
                            RedirectUris = {
                                $"{BaseUrl}/callback.html",
                                $"{BaseUrl}/popup.html",
                                $"{BaseUrl}/silent.html"
                            },
                            PostLogoutRedirectUris = { $"{BaseUrl}/index.html" },
                            AllowedScopes = { "openid", "profile", "email", "api1" },
                            AllowedCorsOrigins = { BaseUrl },
                        },
                        new Client
                        {
                            ClientId = "ro.client",
                            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                            ClientSecrets =
                            {
                                new Secret("secret".Sha256())
                            },
                            AllowedScopes = { "api1" }
                        },

                };
            }
        }

        public static Client ResourceOwnerClient
        {
            get
            {
                return new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                            {
                                new Secret("secret".Sha256())
                            },
                    AllowedScopes = { "api1" }
                };
            }
        }

        public static IEnumerable<IdentityResource> IdentityResources = new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
             
        };

        public static IEnumerable<ApiResource> Apis = new List<ApiResource>
        {
            new ApiResource("api1", "StoreApiCoding",
                new[]{ JwtClaimTypes.Id,JwtClaimTypes.Name,JwtClaimTypes.FamilyName,JwtClaimTypes.Role})
        };

        public static string BaseUrl { get; set; } = "http://localhost:49557/";// "https://localhost:44373/";

        
    }
}
