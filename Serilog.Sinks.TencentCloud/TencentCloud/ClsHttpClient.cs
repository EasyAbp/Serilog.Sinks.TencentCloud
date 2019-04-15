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
        private readonly HttpClient client;
        private readonly Authorization authorization;

        public ClsHttpClient() => client = new HttpClient();

        public ClsHttpClient(Authorization authorization)
        {
            this.authorization = authorization;
            client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            
        }
        public void Dispose() => client.Dispose();

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization.GetAuthorizationString());
            return client.PostAsync(requestUri, content);
        }
    }
}
