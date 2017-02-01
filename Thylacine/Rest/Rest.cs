﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Serializers;
using RestSharp.Authenticators;
using Thylacine.Rest.Authenticator;
using Thylacine.Rest.Payloads;
using Thylacine.Models;
using RestSharp.Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thylacine.Exceptions;

namespace Thylacine.Rest
{
    //TODO: Make a IRestClient interface to allow modulation
    public class RestClient
    {
        private RestSharp.RestClient client;
        private string token;

        public RestClient(string token)
        {
            this.token = token;

            this.client = new RestSharp.RestClient("https://discordapp.com/api");
            this.client.Authenticator = new DiscordAuthenticator(this.token);
        }


        private IRestResponse Send(string resource, Method method, object payload)
        {
            RestRequest request = new RestRequest(resource, method);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(payload);

            var response = client.Execute(request);
            switch(response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Created:
                case System.Net.HttpStatusCode.NoContent:
                    return response;

                case System.Net.HttpStatusCode.Forbidden:
                case System.Net.HttpStatusCode.BadRequest:
                    DiscordError err = JsonConvert.DeserializeObject<DiscordError>(response.Content);
                    throw new DiscordRestException(err.Code, err.Message);

                default:
                    throw new Exception("A HTTP status error has occured while sending a REST call to discord. Status Code: " + (int)response.StatusCode);
            }
        }

        /// <summary>
        /// Sends a IRestPayload to discord.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public void SendPayload(IRestPayload payload)
        {
            Send(payload.Request, payload.Method, payload.Payload);
        }

        /// <summary>
        /// Sends a IRestPayload to discord.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        public T SendPayload<T>(IRestPayload payload) where T : new()
        {
            IRestResponse response = Send(payload.Request, payload.Method, payload.Payload);            
            return JsonConvert.DeserializeObject<T>(response.Content);
        }



        private struct DiscordError
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
