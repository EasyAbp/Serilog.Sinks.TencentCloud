using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Serilog.Sinks.TencentCloud
{
    public class ClsHttpClient : IHttpClient
    {
        private readonly Authorization authorization;
        private readonly HttpClient client;

        public ClsHttpClient(Authorization authorization)
        {
            this.authorization = authorization;
            client = new HttpClient();
        }
        
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization.GetAuthorizationString());
            return await client.PostAsync(requestUri, content, cancellationToken);
        }
    }
}
