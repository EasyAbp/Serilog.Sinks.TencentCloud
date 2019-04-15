using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.TencentCloud.Sinks.Http
{
    public interface IBatchFormatter
    {
        /// <summary>
        /// Format the log events
        /// </summary>
        /// <param name="logEvents">
        /// The events to format.
        /// </param>
        byte[] Format(IEnumerable<LogEvent> logEvents);
    }
}
