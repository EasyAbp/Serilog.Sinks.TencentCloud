using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.TencentCloud
{
    public static class Time
    {
        public static string GetTencentCloudTimeStamp(this DateTime dateTime)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            long startTimeStamp = Convert.ToInt64((dateTime - dateStart).TotalSeconds);
            long endTimeStamp = Convert.ToInt64((dateTime.AddHours(1) - dateStart).TotalSeconds);
            return $"{startTimeStamp.ToString()};{endTimeStamp}";
        }
    }
}
