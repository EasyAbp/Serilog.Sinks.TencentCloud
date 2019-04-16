using Microsoft.Extensions.DependencyInjection;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Http;
using Serilog.Sinks.TencentCloud;
using Serilog.Sinks.TencentCloud.Sinks.Http;
using Serilog.Sinks.TencentCloud.Sinks.Http.BatchFormatters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Serilog
{/// <summary>
 /// Adds the WriteTo.TencentCloud() extension method to
 /// <see cref="LoggerConfiguration"/>.
 /// </summary>
    public static class LoggerSinkConfigurationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBaseUri">Like ap-guangzhou.cls.myqcloud.com/</param>
        /// <param name="topicId">日志主题 ID </param>
        /// <param name="secretId">腾讯云API调用授权所需secretId</param>
        /// <param name="secretKey">腾讯云API调用授权所需secretKey</param>
        /// <returns></returns>
        public static LoggerConfiguration TencentCloud(
               this LoggerSinkConfiguration sinkConfiguration,
               string requestBaseUri,
               string topicId,
               string secretId,
               string secretKey,
               int batchPostingLimit = 1000,
               int? queueLimit = null,
               TimeSpan? period = null,
               ClsFormatter clsFormatter = null,
               LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
               IHttpClient httpClient = null)
        {
            var authorization = new Authorization()
            {
                SecretId = secretId,
                SecretKey = secretKey,
            };

            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

            // Default values
            period = period ?? TimeSpan.FromSeconds(2);
            clsFormatter = clsFormatter ?? new ClsFormatter();

            var httpClientFactory = GetHttpClientFactory();
            httpClient = httpClient ?? new ClsHttpClient(authorization, httpClientFactory);

            var sink = queueLimit != null
                ? new TencentCloudSink($"http://{requestBaseUri}/structuredlog?topic_id={topicId}", batchPostingLimit, period.Value, queueLimit.Value, httpClient, clsFormatter)
                : new TencentCloudSink($"http://{requestBaseUri}/structuredlog?topic_id={topicId}", batchPostingLimit, period.Value, httpClient, clsFormatter );

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        private static IHttpClientFactory GetHttpClientFactory()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var provider = serviceCollection.BuildServiceProvider();
            var httpClientFactory = provider.GetService<IHttpClientFactory>();
            return httpClientFactory;
        }
    }
}
