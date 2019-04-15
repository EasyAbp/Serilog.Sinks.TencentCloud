using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Http;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.TencentCloud.Sinks.Http
{
    public class TencentCloudSink : PeriodicBatchingSink
    {
        private const string ContentType = "application/x-protobuf";

        private readonly string requestUri;
        private readonly IBatchFormatter clsFormatter;

        private IHttpClient client;
        public TencentCloudSink(string requestUri, int batchSizeLimit, TimeSpan period, IHttpClient client, IBatchFormatter clsFormatter) 
            : base(batchSizeLimit, period)
        {
            this.requestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
            this.clsFormatter = clsFormatter ?? throw new ArgumentNullException(nameof(clsFormatter));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public TencentCloudSink(string requestUri, int batchSizeLimit, TimeSpan period, int queueLimit, IHttpClient client, IBatchFormatter clsFormatter) 
            : base(batchSizeLimit, period, queueLimit)
        {
            this.requestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
            this.clsFormatter = clsFormatter ?? throw new ArgumentNullException(nameof(clsFormatter));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }


        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> logEvents)
        {
            var logs = clsFormatter.Format(logEvents);

            var content = new ByteArrayContent(logs); //new StringContent(logs);
            content.Headers.Clear();
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            var result = await client
                .PostAsync(requestUri, content)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                throw new LoggingFailedException($"Received failed result {result.StatusCode} when posting events to {requestUri}");
            }
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">
        /// If true, called because the object is being disposed; if false, the object is being
        /// disposed from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                client?.Dispose();
                client = null;
            }
        }
    }
}
