// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nproj.StillHereApp.Common.Utils
{
    public static class SystemHelper
    {
        /// <summary>
        ///     Md5 32位加密
        /// </summary>
        public const int Md5Len32 = 32;

        /// <summary>
        ///     Md5 16位加密
        /// </summary>
        public const int Md5Len16 = 16;

        /// <summary>
        /// Random
        /// </summary>
        public static readonly Random Random = new Random();

        /// <summary>
        /// Letters  Mod 62
        /// </summary>
        public static readonly string[] Letters = new[]
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", //10
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", //26
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" //26
        };

        /// <summary>
        /// XmlToDict
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Dictionary<string, string> XmlToDict(string xml)
        {
            //创建xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var dict = new Dictionary<string, string>();
            foreach (var xmlDocChildNode in xmlDoc.FirstChild.ChildNodes)
            {
                var element = (XmlElement)xmlDocChildNode;
                dict.Add(element.Name, element.InnerText);
            }

            return dict;
        }
        
        /// <summary>
        /// 将 Unix 时间, 变成C#时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>    
        /// DateTime时间格式转换为Unix时间戳格式    
        /// </summary>    
        /// <param name="time"> DateTime时间格式</param>    
        /// <returns>Unix时间戳格式</returns>    
        public static int DateTimeToUnixTime(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// DateTime时间格式转换为13位带毫秒的Unix时间戳
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long ConvertDateTimeLong(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static int TimeStamp()
        {
            return DateTimeToUnixTime(DateTime.Now);
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            return Regex.IsMatch(ip,
                @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){6}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^::([\da-fA-F]{1,4}:){0,4}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:):([\da-fA-F]{1,4}:){0,3}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){2}:([\da-fA-F]{1,4}:){0,2}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){3}:([\da-fA-F]{1,4}:){0,1}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){4}:((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){7}[\da-fA-F]{1,4}$|^:((:[\da-fA-F]{1,4}){1,6}|:)$|^[\da-fA-F]{1,4}:((:[\da-fA-F]{1,4}){1,5}|:)$|^([\da-fA-F]{1,4}:){2}((:[\da-fA-F]{1,4}){1,4}|:)$|^([\da-fA-F]{1,4}:){3}((:[\da-fA-F]{1,4}){1,3}|:)$|^([\da-fA-F]{1,4}:){4}((:[\da-fA-F]{1,4}){1,2}|:)$|^([\da-fA-F]{1,4}:){5}:([\da-fA-F]{1,4})?$|^([\da-fA-F]{1,4}:){6}:$");
        }

        /// <summary>
        ///     将一个字符串, 执行md5加密
        /// </summary>
        /// <param name="str">要加密的串</param>
        /// <param name="len">Md5Len32 or Md5Len16</param>
        /// <param name="type">Md5Len32 or Md5Len16</param>
        /// <returns>返回加密后的串, 小写形式</returns>
        public static string Md5(string str, int len = Md5Len32, byte type = 0)
        {
            var md5 = new MD5CryptoServiceProvider();
            var result = type == 0 ? Encoding.UTF8.GetBytes(str) : Encoding.Unicode.GetBytes(str);
            var output = md5.ComputeHash(result);
            var value = BitConverter.ToString(output).Replace("-", "").ToLower();
            if (len == 32)
            {
                return value;
            }

            value = value.Substring(8, 16);
            return value;
        }


        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Sha1(string str)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_in = Encoding.UTF8.GetBytes(str);
            byte[] bytes_out = sha1.ComputeHash(bytes_in);
            sha1.Dispose();
            string result = BitConverter.ToString(bytes_out);
            result = result.Replace("-", "");
            return result;
        }
        
        /// <summary>
        /// 获取当前请求的url
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUrl()
        {
            //var url = "";
            var current = HttpContextHelper.GetCurrent();

            var url = HttpContextHelper.Get<string>(HttpContextHelper.CurrentUrl, current);
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }

            var request = current.Request;
            var proto = current.Request.Headers["X-Client-Proto"];
            if (string.IsNullOrWhiteSpace(proto))
            {
                proto = current.Request.Headers["X-Forwarded-Proto"];
            }

            if (string.IsNullOrWhiteSpace(proto))
            {
                proto = request.Scheme;
            }

            var host = request.Host.ToUriComponent();
            var path = current.Request.Headers["X-Original-URL"];
            if (string.IsNullOrWhiteSpace(path))
            {
                path = request.Path.ToUriComponent();
            }

            var queryString = request.QueryString.ToUriComponent();

            url = proto + "://" + host + path + queryString;
            HttpContextHelper.Set(HttpContextHelper.CurrentUrl, url, current);
            return url;
        }


        /// <summary>
        /// 当前时间的日期部分
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDate()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0);
        }


        /// <summary>
        /// 用时间形式返回一个编号
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="len">生成的数字长度, 默认是8</param>
        /// <returns></returns>
        public static string GenNumber(string fix = "", short len = 8)
        {
            var prefix = fix;
            if (len <= 0) return prefix;
            using (var rng = RandomNumberGenerator.Create())
            {
                if (len == 1)
                {
                    var b = new byte[1];
                    rng.GetBytes(b);
                    int d = (b[0] % 9) + 1; // 1-9
                    return prefix + d.ToString();
                }

                var bytes = new byte[8];
                rng.GetBytes(bytes);
                ulong maxTail = Convert.ToUInt64(Math.Pow(10, len - 1));
                ulong tail = BitConverter.ToUInt64(bytes, 0) % maxTail;

                var b2 = new byte[1];
                rng.GetBytes(b2);
                int firstDigit = (b2[0] % 9) + 1; // 1-9

                return prefix + firstDigit.ToString() + String.Format("{0:D" + (len - 1) + "}", tail);
            }
        }

        /// <summary>
        /// 按指定长度生成一个纯数字字符
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GenNumberByLen(short len)
        {
            var result = new StringBuilder();
            for (var i = 0; i < len; i++)
            {
                result.Append(Random.Next(0, 10));
            }

            return result.ToString();
        }

        /// <summary>
        /// GenNumber2
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GenNumber2(string fix = "", short len = 8)
        {
            var result = new StringBuilder();
            result.Append(fix);
            for (var i = 0; i < len; i++)
            {
                result.Append(Letters[Random.Next(0, 1001) % Letters.Length]);
            }

            return result.ToString();
        }


        /// <summary>
        /// 返回0-9之间的数字
        /// 包含start, 不包含end
        /// </summary>
        /// <returns></returns>
        public static int GetRandom(int start = 0, int end = 10)
        {
            return new Random().Next(start, end);
        }

        /// <summary>
        /// GetRefererUrl
        /// </summary>
        /// <returns></returns>
        public static string GetRefererUrl()
        {
            var headers = HttpContextHelper.GetCurrent().Request.Headers;
            if (headers.ContainsKey("Referer"))
            {
                return headers["Referer"];
            }

            if (headers.ContainsKey("referer"))
            {
                return headers["referer"];
            }

            return "";
        }

        /// <summary>
        /// GetGuid
        /// </summary>
        /// <returns></returns>
        public static string GetGuid(byte len = 32)
        {
            var g = Guid.NewGuid().ToString().ToLower().Replace("-", "");
            if (len == 16)
            {
                return g.Substring(8, 16);
            }

            if (len == 8)
            {
                return g.Substring(8, 8);
            }

            return g;
        }

        /// <summary>
        /// CheckFromUrl
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CheckFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            var hosts = ConfigHelper.Get(ConfigHelper.RedirectSecurityHost);
            if (string.IsNullOrEmpty(hosts))
            {
                return false;
            }

            var host = new Uri(url).Host;
            return hosts.Split(',').Contains(host);
        }

        /// <summary>
        /// ConvertImagePath
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string ConvertImagePath(string image, int width, int height)
        {
            if (string.IsNullOrEmpty(image) || image.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                return "";
            }

            var extName = image.Substring(image.LastIndexOf(".", StringComparison.Ordinal));
            var embed = "_" + width + "_" + height + "_auto";
            if (image.IndexOf("_", StringComparison.Ordinal) == -1)
            {
                return image.Substring(0, image.IndexOf(".", StringComparison.Ordinal)) + embed + extName;
            }

            return image.Substring(0, image.IndexOf("_", StringComparison.Ordinal)) + embed + extName;
        }


        /// <summary>
        ///     返回当前访问者的客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            return GetIp();
        }


        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsIpv4(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetIp()
        {
            string userHostAddress = "";
            var httpXForwardedFor = HttpContextHelper.GetCurrent().Request.Headers["X-Forwarded-For"].ToString();
            //如果客户端使用了代理服务器，则利用HTTP_X_FORWARDED_FOR找到客户端IP地址
            if (!string.IsNullOrEmpty(httpXForwardedFor))
            {
                if (httpXForwardedFor.Contains(":"))
                {
                    userHostAddress = httpXForwardedFor.Split(':')[0].Trim();
                }
                else if (httpXForwardedFor.Contains(","))
                {
                    userHostAddress = httpXForwardedFor.Split(',')[0].Trim();
                }
                else
                {
                    userHostAddress = httpXForwardedFor.Trim();
                }
            }

            //前两者均失败，则利用Request.UserHostAddress属性获取IP地址，但此时无法确定该IP是客户端IP还是代理IP
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContextHelper.GetCurrent().Connection.RemoteIpAddress.ToString();
            }

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && IsIpv4(userHostAddress))
            {
                return userHostAddress;
            }

            return "127.0.0.1";
        }

        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        /// <summary>
        /// UrlDecode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// HtmlEncode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HtmlEncode(string url)
        {
            return HttpUtility.HtmlEncode(url);
        }

        /// <summary>
        /// HtmlDecode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HtmlDecode(string url)
        {
            return HttpUtility.HtmlDecode(url);
        }

        /// <summary>
        /// DeserializeObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string value)
        {
            //value = HtmlDecode(value);
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// SerializeObject
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// GetCurrentHost
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHost()
        {
            var url = GetRefererUrl();
            /*if (string.IsNullOrEmpty(url))
            {
                url = GetCurrentUrl();
            }*/
            if (!string.IsNullOrEmpty(url))
            {
                return new Uri(url).Host;
            }

            return "";
        }

        /// <summary>
        /// 用来生成一个导出的文件名
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetExportFileName(string dir, out string path)
        {
            path = "/front/Files/Export/" + dir;
            var guide = Guid.NewGuid().ToString();
            path += "/" + guide + ".xlsx";
            return HttpContextHelper.GetPath(path);
        }

        /// <summary>
        /// 用来生成一个导出的文件名
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetExportFileName_Custom_MD5_Key(string key, string dir, out string path)
        {
            path = "/front/Files/Export/" + dir;
            path += "/" + Md5(key) + ".xlsx";
            return HttpContextHelper.GetPath(path);
        }
        
        /// <summary>
        /// 清除html代码
        /// </summary>
        /// <param name="html">html字符串</param>
        /// <returns>清理后的纯文本</returns>
        public static string ClearHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            //删除脚本   
            html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            html = Regex.Replace(html, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"-->", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<!--.*", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            html = html.Replace("<", "");
            html = html.Replace(">", "");
            html = html.Replace("\r\n", "");
            html = HttpUtility.HtmlEncode(html).Trim();
            return html;
        }

        /// <summary>
        /// PrintHeader
        /// </summary>
        /// <param name="request"></param>
        public static void PrintHeader(HttpRequest request)
        {
            foreach (var item in request.Headers)
            {
                LogHelper.Debug($"{item.Key}={item.Value}");
            }
        }

        /// <summary>
        /// PrintQuery
        /// </summary>
        /// <param name="request"></param>
        public static void PrintQuery(HttpRequest request)
        {
            foreach (var item in request.Query)
            {
                LogHelper.Debug($"{item.Key}={item.Value}");
            }
        }
        
        /// <summary>
        /// 将一个毫秒数转换成 00:00:00 格式的字符串
        /// </summary>
        /// <param name="duration"> 持续时长 毫秒 </param>
        /// <returns></returns>
        public static string ConvertMsToHms(long duration)
        {
            var ts = TimeSpan.FromMilliseconds(duration);
            return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
        
        /// <summary>
        /// 将HH:mm:ss.ms格式的字符串转换成毫秒数
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static long ConvertTimeToMs(string duration)
        {
            try
            {
                var ts = TimeSpan.Parse(duration);
                return (long)ts.TotalMilliseconds;
            }
            catch (Exception e)
            {
                LogHelper.Error($"ConvertTimeToMs Error=>{e.Message},Stack=>{e.StackTrace}");
            }

            return 0;
        }

        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="localDataVoiceItem"></param>
        /// <returns></returns>
        public static T Copy<T>(object localDataVoiceItem)
        {
            var json = JsonConvert.SerializeObject(localDataVoiceItem);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将一个毫秒转换成分钟
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int MsToMin(int value)
        {
            return (int)Math.Floor(value / 60000.0);
        }

        /// <summary>
        ///  解析时间字符串，失败则返回当前时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string str)
        {
            if (!DateTime.TryParse(str, out var createTime))
            {
                createTime = DateTime.Now;
            }

            return createTime;
        }
    }
}