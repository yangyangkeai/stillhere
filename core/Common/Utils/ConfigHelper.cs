// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Nproj.StillHereApp.Common.Utils
{
    /// <summary>
    /// ConfigHelper
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// KEY_SmtpAccount
        /// </summary>
        public const string KEY_SmtpAccount = "Smtp:Account";

        /// <summary>
        /// KEY_SmtpName
        /// </summary>
        public const string KEY_SmtpName = "Smtp:Name";

        /// <summary>
        /// KEY_SmtpTitle
        /// </summary>
        public const string KEY_SmtpTitle = "Smtp:Title";

        /// <summary>
        /// KEY_SmtpBody
        /// </summary>
        public const string KEY_SmtpBody = "Smtp:Body";

        /// <summary>
        /// KEY_SmtpPwd
        /// </summary>
        public const string KEY_SmtpPwd = "Smtp:Pwd";

        /// <summary>
        /// KEY_SmtpServer
        /// </summary>
        public const string KEY_SmtpServer = "Smtp:Server";

        /// <summary>
        /// StaticFileCacheDuration
        /// </summary>
        public static string StaticFileCacheDuration = "Sys:StaticFileCacheDuration";

        /// <summary>
        /// DbConnection
        /// </summary>
        public static string DbConnection = "Db:Connection";

        /// <summary>
        /// DbConnectionReadonly
        /// </summary>
        public static string DbConnectionReadonly = "Db:ConnectionReadonly";

        /// <summary>
        /// MinWorkerThreads
        /// </summary>
        public const string KEY_MinWorkerThreads = "Sys:MinWorkerThreads";

        /// <summary>
        /// MinCompletionPortThreads
        /// </summary>
        public const string KEY_MinCompletionPortThreads = "Sys:MinCompletionPortThreads";

        /// <summary>
        /// LoginExpires
        /// </summary>
        public static string KEY_LoginExpires = "Sys:LoginExpires";

        /// <summary>
        /// LoginAutoExtension
        /// </summary>
        private static string KEY_LoginAutoExtension = "Sys:LoginAutoExtension";

        /// <summary>
        /// HourThreshold
        /// </summary>
        private static string KEY_HourThreshold = "Sys:HourThreshold";

        /// <summary>
        /// AesKey
        /// </summary>
        public const string AesKey = "Aes:Key";

        /// <summary>
        /// UseUrl
        /// </summary>
        public const string UseUrl = "Sys:UseUrl";

        /// <summary>
        /// WebAppid
        /// </summary>
        public const string WebAppid = "Sys:WebAppId";

        /// <summary>
        /// CookieDomain
        /// </summary>
        public const string CookieDomain = "Sys:CookieDomain";

        /// <summary>
        /// CookieSecure
        /// </summary>
        public const string CookieSecure = "Sys:CookieSecure";

        /// <summary>
        /// SecurityHost
        /// </summary>
        public const string SecurityHost = "Sys:SecurityHost";

        /// <summary>
        /// RedirectSecurityHost
        /// </summary>
        public const string RedirectSecurityHost = "Sys:RedirectSecurityHost";

        /// <summary>
        /// BlackIps
        /// </summary>
        public const string BlackIps = "Sys:BlackIps";

        /// <summary>
        /// _configuration
        /// </summary>
        private static IConfigurationRoot _configuration;

        /// <summary>
        /// 缓存字符串值
        /// </summary>
        private static Dictionary<string, string> _cacheDict = new Dictionary<string, string>();

        /// <summary>
        /// LoginAutoExtensionVal
        /// </summary>
        public static double LoginAutoExtensionVal = 0;

        /// <summary>
        /// 一个人超过多少小时未打卡则发送邮件通知
        /// </summary>
        public static int HourThresholdVal = 0;

        /// <summary>
        /// 替换环境变量
        /// </summary>
        /// <param name="str"></param>
        /// <param name="vars"></param>
        private static void ReplaceEnvVar(ref string str, Dictionary<string, string> vars = null)
        {
            //优先替换自定义变量
            if (vars != null)
            {
                int startIdx;
                while ((startIdx = str.IndexOf("${", System.StringComparison.Ordinal)) >= 0)
                {
                    var endIdx = str.IndexOf('}', startIdx + 2);
                    if (endIdx < 0) break;
                    var key = str.Substring(startIdx + 2, endIdx - startIdx - 2);
                    if (vars.ContainsKey(key))
                    {
                        var val = vars[key];
                        str = str.Substring(0, startIdx) + val + str.Substring(endIdx + 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                //再替换环境变量
                int startIdx;
                while ((startIdx = str.IndexOf("${", System.StringComparison.Ordinal)) >= 0)
                {
                    var endIdx = str.IndexOf('}', startIdx + 2);
                    if (endIdx < 0) break;
                    var key = str.Substring(startIdx + 2, endIdx - startIdx - 2);
                    var val = System.Environment.GetEnvironmentVariable(key) ?? string.Empty;
                    str = str.Substring(0, startIdx) + val + str.Substring(endIdx + 1);
                }
            }
        }


        /// <summary>
        /// Init
        /// </summary>
        public static void Init(string name)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(name, true, false)
                .AddEnvironmentVariables();
            _configuration = builder.Build();


            //这一块考虑初始化一些参数
            try
            {
                LoginAutoExtensionVal = double.Parse(Get(KEY_LoginAutoExtension));
            }
            catch
            {
                // ignored
            }

            try
            {
                HourThresholdVal = int.Parse(Get(KEY_HourThreshold));
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// GetObject
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetObject<T>(string key)
        {
            return _configuration.GetSection(key).Get<T>();
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>取到的值</returns>
        public static string Get(string key, Dictionary<string, string> vars = null)
        {
            if (_cacheDict.ContainsKey(key))
            {
                return _cacheDict[key];
            }

            var str = _configuration.GetValue<string>(key);
            //替换变量
            if (!string.IsNullOrWhiteSpace(str))
            {
                ReplaceEnvVar(ref str, vars);
                _cacheDict.TryAdd(key, str);
            }

            return str;
        }
    }
}