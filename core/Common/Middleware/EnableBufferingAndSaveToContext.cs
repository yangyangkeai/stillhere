// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Middleware
{
    /// <summary>
    /// EnableBuffering 中间件
    /// </summary>
    public class EnableBufferingAndSaveToContext
    {
        /// <summary>
        /// 代理 架构要求的
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next"></param>
        public EnableBufferingAndSaveToContext(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 主调用方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "POST")
            {
                var syncIoFeature = context.Features.Get<IHttpBodyControlFeature>();
                if (syncIoFeature != null)
                {
                    syncIoFeature.AllowSynchronousIO = true;
                }

                context.Request.EnableBuffering(500 * 1024);
                var stream = new MemoryStream();
                context.Request.Body.CopyTo(stream);
                context.Request.Body.Position = 0;
                if (stream.Length > 0)
                {
                    stream.Position = 0;
                    var sr = new StreamReader(stream, Encoding.UTF8);
                    var requestContent = sr.ReadToEnd();
                    HttpContextHelper.Set(HttpContextHelper.RequestContent, requestContent);
                }

                stream.Close();
                stream.Dispose();
            }

            await _next(context);
        }
    }
}