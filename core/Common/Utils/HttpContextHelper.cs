// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Nproj.StillHereApp.Common.Utils
{
    public enum AppEnvironment
    {
        Pc,
        MiniProg
    }

    public static class HttpContextHelper
    {
        /// <summary>
        /// 当前的环境， 由程序启动时决定
        /// </summary>
        public static AppEnvironment CurrentEnvironment = AppEnvironment.Pc;

        /// <summary>
        /// _baseDirectory
        /// </summary>
        private static string _baseDirectory;

        /// <summary>
        /// 获取当前HttpCurrent
        /// </summary>
        /// <returns></returns>
        public static HttpContext GetCurrent()
        {
            var ica = ServiceLocator.GetService<IHttpContextAccessor>();
            return ica?.HttpContext;
        }

        /// <summary>
        /// User
        /// </summary>
        public const string User = "User";
        
        /// <summary>
        /// 上下文中存放事务的key
        /// </summary>
        public const string DbTransaction = "DbTransaction";

        /// <summary>
        /// DbConn
        /// </summary>
        public const string DbConn = "DbConn";

        /// <summary>
        /// CurrentUrl
        /// </summary>
        public const string CurrentUrl = "CurrentUrl";


        /// <summary>
        /// RequestContent
        /// </summary>
        public const string RequestContent = "RequestContent";

        /// <summary>
        /// Action
        /// </summary>
        public const string Action = "Action";

        /// <summary>
        /// Controller
        /// </summary>
        public const string Controller = "Controller";

        /// <summary>
        /// SignKey
        /// </summary>
        public const string SignKey = "SignKey";

        /// <summary>
        /// Token
        /// </summary>
        public const string Token = "Token";
        
        /// <summary>
        /// Session
        /// </summary>
        public const string Session = "Session";

        /// <summary>
        /// Source
        /// </summary>
        public const string Source = "Source";

        /// <summary>
        /// UserAgent
        /// </summary>
        public const string UserAgent = "UserAgent";

        static HttpContextHelper()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            _baseDirectory = path.Substring(0, path.Length - 1);
        }

        /// <summary>
        /// Env
        /// </summary>
        public static IWebHostEnvironment Env { get; set; }

        /// <summary>
        /// 从当前上下文中读取信息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="current">key</param>
        /// <returns></returns>
        public static T Get<T>(string key,
            HttpContext current = null)
        {
            if (current == null)
            {
                current = GetCurrent();
            }

            if (current != null)
            {
                return (T)GetCurrent().Items[key];
            }

            return ThreadContext.Get<T>(key);
        }

        /// <summary>
        ///  设置一个值到当前上下文中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="current"></param>
        public static void Set(string key, object value,
            HttpContext current = null)
        {
            if (current == null)
            {
                current = GetCurrent();
            }

            if (current != null)
            {
                current.Items[key] = value;
            }

            ThreadContext.Set(key, value);
        }

        /// <summary>
        /// 获取文件本地系统中的绝对路径,
        /// </summary>
        /// <param name="path">/开头(/即代表当前根目录/../xxx.jpg可以上一层), 相对当前web项目所在目录</param>
        /// <param name="createdDir">created</param>
        /// <returns></returns>
        public static string GetPath(string path, bool createdDir = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("输入路径无效");
            }

            if (path[0] != '/')
            {
                path += "/" + path;
            }

            //兼容
            path = path.Replace("\\", "/");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //如果是windows, 需要将path中的 / 变成 \
                path = path.Replace("/", "\\");
            }

            var result = $"{_baseDirectory}{path}";
            var dir = Path.GetDirectoryName(result);
            if (dir != null)
            {
                if (!Directory.Exists(dir) && createdDir)
                {
                    Directory.CreateDirectory(dir);
                }
            }

            return result;
        }

        /// <summary>
        /// ReadContent
        /// </summary>
        /// <returns></returns>
        public static string ReadContent()
        {
            return Get<string>(RequestContent);
        }

        /// <summary>
        /// GetUa
        /// </summary>
        /// <returns></returns>
        public static string GetUa()
        {
            var request = GetCurrent().Request;
            var ua = request.Headers.ContainsKey("Android-User-Agent") ? request.Headers["Android-User-Agent"].ToString() : "";
            if (string.IsNullOrWhiteSpace(ua))
            {
                ua = request.Headers.TryGetValue("User-Agent", out var header) ? header.ToString().ToLower() : "";
            }

            return ua;
        }

        /// <summary>
        /// 读取query
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetQuery(string key)
        {
            var current = GetCurrent();
            if (current != null)
            {
                return current.Request.Query[key];
            }

            return "";
        }
    }
}