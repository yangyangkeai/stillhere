// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Filter
{
    /// <summary>
    /// 字符串比较
    /// </summary>
    public class StringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return String.CompareOrdinal(x, y);
        }
    }

    /// <summary> 
    /// 验证post数据签名
    /// </summary>
    public class ParamSignFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            if (request.Method == "POST")
            {
                var signature = "";
                var dic = new Dictionary<string, string>();
                var body = HttpContextHelper.ReadContent();
                if (string.IsNullOrEmpty(body))
                {
                    LogHelper.Error($"ParamSignFilter: body is null. url={SystemHelper.GetCurrentUrl()},rurl={SystemHelper.GetRefererUrl()}");
                    context.Result = JsonResultHelper.InvalidSignature("数据校验失败 1");
                    return;
                }

                var requestData = (JObject)JsonConvert.DeserializeObject(body);
                foreach (var token in requestData)
                {
                    if (token.Value == null)
                    {
                        continue;
                    }

                    //跳过sign
                    if (token.Key == "signature")
                    {
                        signature = token.Value.ToString();
                    }
                    else
                    {
                        if (token.Value is JArray || token.Value is JObject)
                        {
                            dic.Add(token.Key, token.Value.ToString(Formatting.None));
                        }
                        else
                        {
                            if (token.Value.Type == JTokenType.Date)
                            {
                                dic.Add(token.Key, ((DateTime)token.Value).ToString("yyyy-MM-ddTHH:mm:ss"));
                            }
                            else
                            {
                                dic.Add(token.Key, token.Value.ToString());
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(signature))
                {
                    context.Result = JsonResultHelper.InvalidSignature("数据校验失败 2");
                    return;
                }

                //排序
                var newDic = dic.OrderBy(p => p.Key, new StringComparer()).ToDictionary(p => p.Key, o => o.Value);
                //组织
                var str = newDic.Aggregate("", (current, item) => current + (item.Key + item.Value));
                //验证签名
                var singKey = HttpContextHelper.Get<string>(HttpContextHelper.SignKey);
                if (signature.ToLower() != SystemHelper.Md5(singKey + "---" + str).ToLower())
                {
                    context.Result = JsonResultHelper.InvalidSignature("数据校验失败 3");
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}