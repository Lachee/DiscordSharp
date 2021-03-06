﻿using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Thylacine.Rest.Authenticator
{
    class DiscordAuthenticator : IAuthenticator
    {
        private string token;

        public DiscordAuthenticator(string token) { this.token = token; }

        public void Authenticate(RestSharp.IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", "Bot " + token);
        }
    }
}
