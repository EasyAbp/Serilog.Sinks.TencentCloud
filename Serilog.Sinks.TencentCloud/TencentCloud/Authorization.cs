using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.TencentCloud
{
    public class Authorization
    {
        private long lastEndTime = 0;

        private string authorizationString = "";
        public string SecretKey { get; set; }

        public string SecretId { get; set; }

        /// <summary>
        /// 获取请求Authorization头部的授权字符串
        /// </summary>
        /// <returns></returns>
        public string GetAuthorizationString()
        {
            var timeStr = DateTimeOffset.Now.GetTencentCloudTimeStamp();
            var time = timeStr.Split(";");
            var nowStartTime = Convert.ToInt64(time[0]);
            if(lastEndTime > nowStartTime)
            {
                return authorizationString;
            }
            lastEndTime = Convert.ToInt64(time[1]);
            var signature = Signature.GetSignature("post", "/structuredlog", timeStr, timeStr, SecretKey);
            authorizationString = $"q-sign-algorithm=sha1&q-ak={SecretId}&q-sign-time={timeStr}&q-key-time={timeStr}&q-header-list=&q-url-param-list=&q-signature={signature}";
            return authorizationString;
        }
    }
}
