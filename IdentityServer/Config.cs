// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId()
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
                }
            };
        
    }
}