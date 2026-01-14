// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Filter.Web
{
    /// <summary>
    /// api授权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class WebAuthorFilter : BaseAuthorFilter
    {
        /// <summary>
        /// OnAuthorization
        /// </summary>
        /// <param name="context"></param>
        public override void OnAuthorization(AuthorizationFilterContext context)
        {
            //调父
            base.OnAuthorization(context);

            //请求信息
            HttpRequest request = context.HttpContext.Request;

            //浏览器特征
            var userAgent = HttpContextHelper.GetUa();
            if (string.IsNullOrEmpty(userAgent))
            {
                context.Result = JsonResultHelper.UrlExpire(3);
                return;
            }

            userAgent = AesHelper.DecryptUserAgent(userAgent);

            if (string.IsNullOrEmpty(userAgent))
            {
                context.Result = JsonResultHelper.UrlExpire(3);
                return;
            }

            //获取签名数据
            string timestamp = "", sign = "", random = "", ciphertext = "";

            if (request.Headers.ContainsKey("Ciphertext"))
            {
                ciphertext = HttpUtility.UrlDecode(request.Headers["Ciphertext"]);
            }

            //决定超时
            if (request.Headers.ContainsKey("Timestamp"))
            {
                timestamp = HttpUtility.UrlDecode(request.Headers["Timestamp"]);
            }

            if (string.IsNullOrEmpty(timestamp))
            {
                context.Result = JsonResultHelper.UrlExpire(1);
                return;
            }

            //决定唯一
            if (request.Headers.ContainsKey("Random"))
            {
                random = HttpUtility.UrlDecode(request.Headers["Random"]);
            }

            if (string.IsNullOrEmpty(random))
            {
                context.Result = JsonResultHelper.UrlExpire(2);
                return;
            }

            //决定签名
            if (request.Headers.ContainsKey("Sign"))
            {
                sign = HttpUtility.UrlDecode(request.Headers["Sign"]);
            }

            //生成动态签名key
            var signKey = AesHelper.DecryptSignKey(ciphertext, userAgent);
            if (string.IsNullOrEmpty(signKey))
            {
                signKey = AesHelper.GenerateSignKey(out ciphertext, userAgent, timestamp, random);
                context.Result = JsonResultHelper.UpdateSignKey(new
                {
                    key = signKey,
                    ciphertext = ciphertext
                });
                return;
            }

            HttpContextHelper.Set(HttpContextHelper.SignKey, signKey);

            //签名简单验证
            if (!AesHelper.CheckSign(sign, signKey, timestamp, random))
            {
                context.Result = JsonResultHelper.InvalidSignature();
                return;
            }

            //验证唯一性
            var url = SystemHelper.GetCurrentUrl();
            var urlKey = SystemHelper.Md5(random + url, SystemHelper.Md5Len16);
            if (RedisHelper.Exists(urlKey))
            {
                context.Result = JsonResultHelper.UrlExpire();
                return;
            }

            RedisHelper.Set(urlKey, "1", new TimeSpan(0, 50, 0));

            //判断timespan是否有效
            var requestTimestamp = 0L;
            var currentTimestamp = SystemHelper.ConvertDateTimeLong(DateTime.Now);
            var timestampConvertResult = long.TryParse(timestamp, out requestTimestamp);
            var timestampDiff = Math.Abs(currentTimestamp - requestTimestamp);
            var flag = timestampDiff > 40 * 60 * 1000; //超过指定分钟请求无效
            if (flag || !timestampConvertResult)
            {
                context.Result = JsonResultHelper.UrlExpire();
                return;
            }

            //读取并设置上下文用户
            var token = request.Headers.ContainsKey("Token") ? HttpUtility.UrlDecode(request.Headers["Token"]) : "";
            UserHelper.SetContextUserUseToken(token);
        }
    }
}