// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("apiOne", "API One"),
                new ApiResource("apiTwo", "API Two"),
                new ApiResource("apiThree", "API Three")
            };
        
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // machine to machine client
                new Client
                {
                    ClientId = "externalApp1",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("externalApp1secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "apiOne", "apiTwo" }
                },
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "MvcApp",
                    ClientSecrets = { new Secret("MvcAppSecret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:8001" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:8001" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
        
    }
}