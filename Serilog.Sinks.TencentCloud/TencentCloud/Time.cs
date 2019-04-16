using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.TencentCloud
{
    public static class Time
    {
        public static string GetTencentCloudTimeStamp(this DateTimeOffset offset)
        {
            var endOffset = offset.AddHours(1);
            return $"{offset.ToUnixTimeSeconds()};{endOffset.ToUnixTimeSeconds()}";
        }
    }
}
