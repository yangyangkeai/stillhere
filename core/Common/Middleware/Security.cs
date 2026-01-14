// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Middleware
{
    /// <summary>
    /// EnableBuffering 中间件
    /// </summary>
    public class Security
    {
        /// <summary>
        /// 代理 架构要求的
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// _ips
        /// </summary>
        private HashSet<string> _ips = new();

        /// <summary>
        /// _hosts
        /// </summary>
        private HashSet<string> _hosts = new();


        /// <summary>
        /// MSG_NO_ACCESS
        /// </summary>
        private const string MSG_NO_ACCESS = "No Access!";

        /// <summary>
        /// METHOD_POST
        /// </summary>
        private const string METHOD_POST = "POST";

        /// <summary>
        /// 需要排除掉的url
        /// </summary>
        private Regex _excludedUrlRegex;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next"></param>
        public Security(RequestDelegate next)
        {
            var blackIps = ConfigHelper.Get(ConfigHelper.BlackIps);
            var securityHost = ConfigHelper.Get(ConfigHelper.SecurityHost);
            if (!string.IsNullOrEmpty(blackIps))
            {
                _ips = blackIps.Split(",").ToHashSet();
            }

            if (!string.IsNullOrEmpty(securityHost))
            {
                _hosts = securityHost.Split(",").ToHashSet();
            }

            _excludedUrlRegex = new Regex(@"/open"); //使用|分隔多个

            _next = next;
        }

        /// <summary>
        /// 主调用方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Invoke(HttpContext context)
        {
            var ip = SystemHelper.GetClientIp();
            if (_ips.Contains(ip))
            {
                //黑了
                await context.Response.WriteAsync(MSG_NO_ACCESS);
                return;
            }
            
            var url = SystemHelper.GetCurrentUrl().ToLower();
            var host = SystemHelper.GetCurrentHost();
            if (context.Request.Method == METHOD_POST && !_excludedUrlRegex.IsMatch(url) && !_hosts.Contains(host))
            {
                //不合法的域名
                await context.Response.WriteAsync(MSG_NO_ACCESS);
                return;
            }

            await _next(context);
        }
    }
}