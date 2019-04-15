using Google.Protobuf;
using ProtoBuf;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Serilog.Sinks.TencentCloud.Sinks.Http.BatchFormatters
{
    public class ClsFormatter : IBatchFormatter
    {
        public byte[] Format(IEnumerable<LogEvent> logEvents)
        {
            if (logEvents == null) throw new ArgumentNullException(nameof(logEvents));

            var logGroup = new global::TencentCloud.Cls.LogGroup();
            var stream = new StringWriter();
            foreach (var logEvent in logEvents)
            {
                var log = new global::TencentCloud.Cls.Log();
                var contents = logEvent.Properties.Select(p =>
                {
                    p.Value.Render(stream);
                    var value = stream.ToString();
                    stream.Flush();
                    return new global::TencentCloud.Cls.Log.Types.Content() {
                        Key = p.Key,
                        Value = value
                    };
                });
                log.Contents.AddRange(contents);
                log.Time = logEvent.Timestamp.ToUnixTimeMilliseconds();

                logGroup.Logs.Add(log);
            }
            stream.Dispose();
            var logGroupList = new global::TencentCloud.Cls.LogGroupList();
            logGroupList.LogGroupList_.Add(logGroup);

            return logGroupList.ToByteArray();
        }
    }
}
