using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.TencentCloud
{
    public class ClsHttpClient : IHttpClient
    {
        private readonly IHttpClientFactory factory;
        private readonly Authorization authorization;

        public ClsHttpClient(Authorization authorization, IHttpClientFactory httpClientFactory)
        {
            this.authorization = authorization;
            factory = httpClientFactory;
        }
        
        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization.GetAuthorizationString());
            return client.PostAsync(requestUri, content);
        }
    }
}
