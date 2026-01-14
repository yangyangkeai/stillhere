// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nproj.StillHereApp.Model.Response;

namespace Nproj.StillHereApp.Common.Utils
{
    /// <summary>
    /// JsonResultHelper
    /// </summary>
    public static class JsonResultHelper
    {
        /// <summary>
        /// 生成配置
        /// </summary>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            //yyyy-MM-ddTHH:mm:ss
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// 返回一个结果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonResult JsonResult(int code, string msg, object data = null)
        {
            var response = new StdResp()
            {
                Code = code,
                Msg = msg,
                Data = data
            };
            return new JsonResult(response, JsonSerializerSettings);
        }

        /// <summary>
        /// 直接输出json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static JsonResult JsonResult(object obj)
        {
            return new JsonResult(obj, JsonSerializerSettings);
        }

        /// <summary>
        /// 禁止访问
        /// </summary>
        /// <returns></returns>
        public static JsonResult NoAccess()
        {
            return JsonResult(10001, "禁止访问");
        }

        /// <summary>
        /// 超时
        /// </summary>
        /// <returns></returns>
        public static JsonResult Expire()
        {
            return JsonResult(10002, "超时");
        }

        /// <summary>
        /// 返回无效参数
        /// </summary>
        /// <returns></returns>
        public static JsonResult InvalidParameter(string msg = "无效参数")
        {
            return JsonResult(10003, msg);
        }

        /// <summary>
        /// Url过期
        /// </summary>
        /// <returns></returns>
        public static JsonResult UrlExpire(int code = 0)
        {
            return JsonResult(10004, $"Url过期, Code={code}");
        }

        /// <summary>
        /// 无效签名
        /// </summary>
        /// <returns></returns>
        public static JsonResult InvalidSignature()
        {
            return JsonResult(10005, "无效签名");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static JsonResult InvalidSignature(string msg)
        {
            return JsonResult(10006, msg);
        }

        /// <summary>
        /// 不存在的数据
        /// </summary>
        /// <returns></returns>
        public static JsonResult NotExist(string data = "不存在的数据")
        {
            return JsonResult(10007, data);
        }

        /// <summary>
        /// 无效用户
        /// </summary>
        /// <returns></returns>
        public static JsonResult InvalidUser()
        {
            return JsonResult(10008, "无效的账号");
        }

        /// <summary>
        /// 无效的验证码
        /// </summary>
        /// <returns></returns>
        public static JsonResult InvalidVerificationCode()
        {
            return JsonResult(10009, "无效验证码");
        }

        /// <summary>
        /// 需要登录
        /// </summary>
        /// <returns></returns>
        public static JsonResult RequiredLogin(object data = null)
        {
            return JsonResult(10010, "需要登录", data);
        }
        
        /// <summary>
        /// Mooc 课程没有权限
        /// </summary>
        /// <returns></returns>
        public static JsonResult NoPermission(object data = null)
        {
            return JsonResult(10012, "没有权限", data);
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <returns></returns>
        public static JsonResult Fail()
        {
            return JsonResult(10013, "操作失败");
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <returns></returns>
        public static JsonResult Fail(string msg)
        {
            return JsonResult(10013, msg);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <returns></returns>
        public static JsonResult Alert(string msg)
        {
            return JsonResult(10014, msg);
        }
        
        /// <summary>
        /// 无效密码
        /// </summary>
        /// <returns></returns>
        public static JsonResult InvalidPwd()
        {
            return JsonResult(10016, "无效密码");
        }

        /// <summary>
        /// 尝试超过最大限制
        /// </summary>
        /// <returns></returns>
        public static JsonResult TryOverflow()
        {
            return JsonResult(10017, "尝试超过最大限制");
        }
        
        /// <summary>
        /// NotAllow
        /// </summary>
        /// <returns></returns>
        public static JsonResult NotAllow(object data = null)
        {
            return JsonResult(10019, "不允许操作", data);
        }
        
        /// <summary>
        /// InvalidAccount
        /// </summary>
        /// <returns></returns>
        public static JsonResult InvalidAccount()
        {
            return JsonResult(10023, "无效的账号");
        }

        /// <summary>
        /// AccessDisabled
        /// </summary>
        /// <returns></returns>
        public static JsonResult AccountDisabled()
        {
            return JsonResult(10024, "当前账号被禁用");
        }

        /// <summary>
        /// InvalidToken
        /// </summary>
        /// <returns></returns>
        public static JsonResult InvalidToken()
        {
            return JsonResult(10025, "无效的Token");
        }
        
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <returns></returns>
        public static JsonResult Success(object data = null)
        {
            return JsonResult(0, "ok", data);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static JsonResult Success(string msg, object data)
        {
            return JsonResult(0, msg, data);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <returns></returns>
        public static JsonResult Error(string msg = "Error")
        {
            return JsonResult(10500, msg);
        }

        /// <summary>
        /// To404
        /// </summary>
        /// <returns></returns>
        public static JsonResult To404()
        {
            return JsonResult(10404, "");
        }

        /// <summary>
        /// 自定义, 到首页
        /// </summary>
        /// <returns></returns>
        public static JsonResult To400()
        {
            return JsonResult(10400, "");
        }
        
        /// <summary>
        /// 重定向
        /// </summary>
        /// <returns></returns>
        public static JsonResult Redirect(string msg = "", object data = null)
        {
            return JsonResult(10302, msg, data);
        }

        /// <summary>
        /// 更新签名key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonResult UpdateSignKey(object data)
        {
            return JsonResult(-1, "ok", data);
        }
        
        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <returns></returns>
        public static ActionResult NotImplemented()
        {
            return JsonResult(10501, "未实现");
        }
    }
}