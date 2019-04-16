using Google.Protobuf;
using ProtoBuf;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Parsing;
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

            foreach (var logEvent in logEvents)
            {
                var log = new global::TencentCloud.Cls.Log();
                try
                {
                    var contents = GetLogEventContents(logEvent);
                    log.Contents.AddRange(contents);
                    log.Time = logEvent.Timestamp.ToUnixTimeMilliseconds();
                }
                catch (Exception e)
                {
                    LogNonFormattableEvent(logEvent, e);
                }
                logGroup.Logs.Add(log);
            }
            var logGroupList = new global::TencentCloud.Cls.LogGroupList();
            logGroupList.LogGroupList_.Add(logGroup);

            return logGroupList.ToByteArray();
        }

        private List<global::TencentCloud.Cls.Log.Types.Content> GetLogEventContents(LogEvent logEvent)
        {
            var contents = new List<global::TencentCloud.Cls.Log.Types.Content>();
            var sw1 = new StringWriter();
            contents.Add(new global::TencentCloud.Cls.Log.Types.Content
            {
                Key = "Timestamp",
                Value = logEvent.Timestamp.ToString("o")
            });
            contents.Add(new global::TencentCloud.Cls.Log.Types.Content
            {
                Key = "Level",
                Value = logEvent.Level.ToString()
            });
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, sw1);
            contents.Add(new global::TencentCloud.Cls.Log.Types.Content
            {
                Key = "MessageTemplate",
                Value = sw1.ToString()
            });
            sw1.Flush();
            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            JsonValueFormatter.WriteQuotedJsonString(message, sw1);
            contents.Add(new global::TencentCloud.Cls.Log.Types.Content
            {
                Key = "RenderedMessage",
                Value = sw1.ToString()
            });
            sw1.Flush();
            if (logEvent.Exception != null)
            {
                JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), sw1);
                contents.Add(new global::TencentCloud.Cls.Log.Types.Content
                {
                    Key = "Exception",
                    Value = sw1.ToString()
                });
            }

            if (logEvent.Properties.Count != 0)
            {
                var sw = new StringWriter();
                WriteProperties(logEvent.Properties, sw);
                contents.Add(new global::TencentCloud.Cls.Log.Types.Content
                {
                    Key = "Properties",
                    Value = sw.ToString()
                });
            }
            // Better not to allocate an array in the 99.9% of cases where this is false
            var tokensWithFormat = logEvent.MessageTemplate.Tokens
                .OfType<PropertyToken>()
                .Where(pt => pt.Format != null);
            // ReSharper disable once PossibleMultipleEnumeration
            if (tokensWithFormat.Any())
            {
                var output = new StringWriter();
                // ReSharper disable once PossibleMultipleEnumeration
                WriteRenderings(tokensWithFormat.GroupBy(pt => pt.PropertyName), logEvent.Properties, output);
                contents.Add(new global::TencentCloud.Cls.Log.Types.Content
                {
                    Key = "Renderings",
                    Value = output.ToString()
                });
            }
            return contents;

        }
        private static void WriteProperties(
            IReadOnlyDictionary<string, LogEventPropertyValue> properties,
            TextWriter output)
        {
            output.Write("{");

            var precedingDelimiter = "";

            foreach (var property in properties)
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                new JsonValueFormatter().Format(property.Value, output);
            }

            output.Write('}');
        }

        private static void WriteRenderings(
            IEnumerable<IGrouping<string, PropertyToken>> tokensWithFormat,
            IReadOnlyDictionary<string, LogEventPropertyValue> properties,
            TextWriter output)
        {
            output.Write("{");

            var rdelim = "";
            foreach (var ptoken in tokensWithFormat)
            {
                output.Write(rdelim);
                rdelim = ",";

                JsonValueFormatter.WriteQuotedJsonString(ptoken.Key, output);
                output.Write(":[");

                var fdelim = "";
                foreach (var format in ptoken)
                {
                    output.Write(fdelim);
                    fdelim = ",";

                    output.Write("{\"Format\":");
                    JsonValueFormatter.WriteQuotedJsonString(format.Format, output);

                    output.Write(",\"Rendering\":");
                    var sw = new StringWriter();
                    format.Render(properties, sw);
                    JsonValueFormatter.WriteQuotedJsonString(sw.ToString(), output);
                    output.Write('}');
                }

                output.Write(']');
            }

            output.Write('}');
        }


        private static void LogNonFormattableEvent(LogEvent logEvent, Exception e)
        {
            SelfLog.WriteLine(
                "Event at {0} with message template {1} could not be formatted into JSON and will be dropped: {2}",
                logEvent.Timestamp.ToString("o"),
                logEvent.MessageTemplate.Text,
                e);
        }
    }
}
