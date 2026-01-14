// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Middleware
{
    public class MyStaticFileMiddleware
    {
        /// <summary>
        /// 代理 架构要求的
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// StaticFileCacheDuration
        /// </summary>
        private string _staticFileCacheDuration;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next"></param>
        public MyStaticFileMiddleware(RequestDelegate next)
        {
            _encodingRegex = new Regex(@"gzip");
            _next = next;
            _staticFileCacheDuration = ConfigHelper.Get(ConfigHelper.StaticFileCacheDuration);
        }

        /// <summary>
        /// _contentTypes
        /// </summary>
        private readonly Dictionary<string, string> _contentTypes = new()
        {
            { ".js", "application/javascript" },
            { ".css", "text/css" },
        };

        /// <summary>
        /// METHOD_GET
        /// </summary>
        private const string METHOD_GET = "GET";


        /// <summary>
        /// 需要排除掉的url
        /// </summary>
        private Regex _encodingRegex;

        /// <summary>
        /// 主调用方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Invoke(HttpContext context)
        {
            if (context.Request.Method == METHOD_GET && context.Request.Headers.ContainsKey("Accept-Encoding") && _encodingRegex.IsMatch(context.Request.Headers["Accept-Encoding"]))
            {
                var path = context.Request.Path.ToString().ToLower();
                var extName = Path.GetExtension(path);
                if (_contentTypes.TryGetValue(extName, out var type))
                {
                    //text/css
                    //application/javascript
                    var physicsPath = HttpContextHelper.GetPath($"/wwwroot/{path}.gz", false);
                    try
                    {
                        await using var fileContent = File.OpenRead(physicsPath);
                        context.Response.Headers.Add("Content-Encoding", "gzip");
                        context.Response.Headers.Add("Cache-Control", $"public, max-age={_staticFileCacheDuration}");
                        context.Response.Headers.Add("Content-Type", type);
                        await StreamCopyOperation.CopyToAsync(fileContent, context.Response.Body, fileContent.Length, (int)fileContent.Length, context.Response.HttpContext.RequestAborted);
                        return;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }


            await _next(context);
        }
    }
}