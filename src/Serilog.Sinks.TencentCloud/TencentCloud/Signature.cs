﻿using NETCore.Encrypt.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.TencentCloud
{
    public static class Signature
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">HTTP 请求使用的方法，小写字母，如 get、post等</param>
        /// <param name="uri">HTTP 请求的资源名称，不包含 query string 部分，如 /logset</param>
        /// <returns>HttpRequestInfo</returns>
        private static string GetHttpRequestInfo(string method, string uri)
        {
            return $"{method}\n{uri}\n\n\n";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q_sign_algorithm">签名算法，目前仅支持 sha1</param>
        /// <param name="q_sign_time">签名有效起止时间，Unix时间戳，以秒为单位，;分隔</param>
        /// <param name="httpRequestInfo">HttpRequestInfo</param>
        /// <returns></returns>
        private static string GetStringToSign(string q_sign_algorithm, string q_sign_time, string httpRequestInfo)
        {
            return $"{q_sign_algorithm}\n{q_sign_time}\n{httpRequestInfo.SHA1()}\n".ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q_key_time">Unix时间戳，以秒为单位，;分隔</param>
        /// <param name="secretKey">腾讯云API的SecretKey</param>
        /// <returns></returns>
        private static string GetSignKey(string q_key_time, string secretKey)
        {
            return q_key_time.HMACSHA1(secretKey).ToLower();
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="stringToSign">StringToSign</param>
        /// <param name="signKey">SignKey</param>
        /// <returns></returns>
        private static string GetSignature(string stringToSign, string signKey)
        {
            return stringToSign.HMACSHA1(signKey).ToLower();
        }

        /// <summary>
        /// 获取签名字符串
        /// </summary>
        /// <param name="method">HTTP 请求使用的方法，小写字母，如 get、post等</param>
        /// <param name="uri">HTTP 请求的资源名称，不包含 query string 部分，如 /logset</param>
        /// <param name="q_sign_algorithm">签名算法，目前仅支持 sha1</param>
        /// <param name="q_sign_time">签名有效起止时间，Unix时间戳，以秒为单位，;分隔</param>
        /// <param name="q_key_time">Unix时间戳，以秒为单位，;分隔</param>
        /// <param name="secretKey">腾讯云API的SecretKey</param>
        /// <returns>Signature签名字符串</returns>
        public static string GetSignature(string method, string uri, string q_sign_time, string q_key_time, string secretKey)
        {
            var httpRequestInfo = GetHttpRequestInfo(method, uri);
            var stringToSign = GetStringToSign("sha1", q_sign_time, httpRequestInfo);
            var signKey = GetSignKey(q_key_time, secretKey);

            return GetSignature(stringToSign, signKey);
        }
    }
}
