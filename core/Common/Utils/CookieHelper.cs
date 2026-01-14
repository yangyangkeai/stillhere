// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using Microsoft.AspNetCore.Http;

namespace Nproj.StillHereApp.Common.Utils
{
    public class CookieHelper
    {
        /// <summary>
        /// Session
        /// </summary>
        public static string Session = "session_id";

        /// <summary>
        /// Token
        /// </summary>
        public static string Token = "st_token";

        /// <summary>
        /// Source
        /// </summary>
        public static string Source = "st_source";

        /// <summary>
        /// Position
        /// </summary>
        public static string Position = "st_position";
        

        /// <summary>
        /// 从cookie中拿到一个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            var current = HttpContextHelper.GetCurrent();
            if (current == null)
            {
                return "";
            }

            if (current.Request.Cookies.ContainsKey(key))
            {
                var source = current.Request.Cookies[key];
                if (string.IsNullOrEmpty(source))
                {
                    return "";
                }

                var result = source;
                if (key != Token)
                {
                    result = AesHelper.TryDecrypt(source);
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Split(':')[0];
                    }
                }

                return result;
            }

            return "";
        }


        /// <summary>
        /// GetSource
        /// </summary>
        /// <returns></returns>
        public static string GetSource()
        {
            var r = Get(Source);
            if (string.IsNullOrEmpty(r))
            {
                r = "default";
            }

            return r;
        }

        /// <summary>
        /// GetPosition
        /// </summary>
        /// <returns></returns>
        public static string GetPosition()
        {
            var r = Get(Position);
            if (string.IsNullOrEmpty(r))
            {
                r = "default";
            }
            return r;
        }

        /// <summary>
        /// 往cookie中设置一个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encrypt"></param>
        /// <returns></returns>
        public static void Set(string key, string value, bool encrypt = true)
        {
            var result = "";

            if (encrypt)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    value = $"{value}:{SystemHelper.DateTimeToUnixTime(DateTime.Now)}";
                    try
                    {
                        result = AesHelper.Encrypt(value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else
            {
                result = value;
            }

            HttpContextHelper.GetCurrent().Response.Cookies.Append(key, result, new CookieOptions()
            {
                //Domain = ConfigHelper.Get(ConfigHelper.CookieDomain),
                Path = "/",
                Secure = ConfigHelper.Get(ConfigHelper.CookieSecure) == "1"
            });
        }


        /// <summary>
        /// 往cookie中设置一个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static void Set(string key, string value, DateTime expires)
        {
            var result = "";

            if (!string.IsNullOrEmpty(value))
            {
                value = $"{value}:{SystemHelper.DateTimeToUnixTime(DateTime.Now)}";
                try
                {
                    result = AesHelper.Encrypt(value);
                }
                catch
                {
                    // ignored
                }
            }

            HttpContextHelper.GetCurrent().Response.Cookies.Append(key, result, new CookieOptions()
            {
                Domain = ConfigHelper.Get(ConfigHelper.CookieDomain),
                Path = "/",
                Expires = expires,
                Secure = ConfigHelper.Get(ConfigHelper.CookieSecure) == "1"
            });
        }

        /// <summary>
        /// 清掉一个key
        /// </summary>
        /// <param name="key"></param>
        public static void Clear(string key)
        {
            HttpContextHelper.GetCurrent().Response.Cookies.Delete(key);
        }
    }
}