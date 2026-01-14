// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nproj.StillHereApp.Common.Extend;
using Nproj.StillHereApp.Common.Middleware.MyWeb.Model;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Middleware.MyWeb
{
    /// <summary>
    /// 自定义的Web中间件
    /// </summary>
    public class MyWeb
    {
        /// <summary>
        /// 所有可用页面
        /// </summary>
        private Dictionary<string, PageMeta> _pages = new();

        /// <summary>
        /// 所有可用checker
        /// </summary>
        private Dictionary<string, IMyWebChecker> _checkers = new();

        /// <summary>
        /// 代理 架构要求的
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 匹配url
        /// </summary>
        private readonly Regex _regex = new Regex(@"/([a-z]*)[/]?([a-z0-9]*)[/]?([a-z0-9/]*)");

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next"></param>
        public MyWeb(RequestDelegate next)
        {
            LogHelper.Debug("初始化");

            //获取配置文件路径
            var pageJsonPath = HttpContextHelper.GetPath("/config/pages.json");
            var htmlFilePath = HttpContextHelper.GetPath("/config/html.html");

            //读入内容模板
            var htmlContent = File.ReadAllText(htmlFilePath);
            var pageJson = File.ReadAllText(pageJsonPath);

            //解析json,并填充_pages
            var config = JsonConvert.DeserializeObject<PageConfig>(pageJson);

            if (config != null)
            {
                //先进行通用处理
                htmlContent = htmlContent.Replace("\n", "").Replace("\r", "").Replace("${title}", config.Title)
                    .Replace("${keywords}", config.Keywords)
                    .Replace("${description}", config.Description)
                    .Replace("${commCss}", config.CommCss.MergeToCss())
                    .Replace("${commJs}", config.CommJs.MergeToScript())
                    .Replace("${commFootContent}", config.CommFootContent.Merge())
                    .Replace("${commHeadContent}", config.CommHeadContent.Merge());

                //处理每一个页面
                if (config.Pages.Any())
                {
                    foreach (var configPage in config.Pages)
                    {
                        configPage.Html = htmlContent.Replace("${pageCss}", configPage.Css.MergeToCss())
                            .Replace("${pageJs}", configPage.Js.MergeToScript())
                            .Replace("${headContent}", configPage.HeadContent.Merge());
                        _pages.Add($"{configPage.Controller}-{configPage.Action}", configPage);
                    }
                }
            }

            //扫所有的checker
            var assembly = Assembly.Load("Nproj.StillHereApp.Common");
            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace != null && type.Namespace.Contains("Nproj.StillHereApp.Middleware.MyWeb.Checker") && type.IsClass)
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length == 1 && !string.IsNullOrEmpty(interfaces[0].FullName) && interfaces[0].FullName.IndexOf("IMyWebChecker", StringComparison.Ordinal) != -1)
                    {
                        _checkers.Add(type.Name, (IMyWebChecker)assembly.CreateInstance(type.FullName));
                    }
                }
            }

            _next = next;
        }

        /// <summary>
        /// 主调用方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }

            if (
                context.Request.Method == "GET" &&
                !path.Contains("api") &&
                !path.Contains("open")
            )
            {
                #region 来源 name

                //name
                var name = context.Request.Query["name"].ToString();
                if (!string.IsNullOrEmpty(name) && RegularHelper.IsSource(name))
                {
                    CookieHelper.Set(CookieHelper.Source, name);
                }

                #endregion

                #region 确定用户是否登录

                var userId = 0L;
                var token = CookieHelper.Get(CookieHelper.Token);
                //验证登录情况
                var logged = false;
                if (!string.IsNullOrEmpty(token))
                {
                    userId = RedisHelper.Get<long>(RedisHelper.KEY_UserId + token);
                    if (userId > 0)
                    {
                        logged = true;
                    }
                }

                #endregion

                //自动生成一个SessionId
                var session = CookieHelper.Get(CookieHelper.Session);
                if (string.IsNullOrEmpty(session))
                {
                    CookieHelper.Set(CookieHelper.Session, SystemHelper.GetGuid());
                }

                //解析path匹配页面
                var match = _regex.Match(path);
                if (match.Success)
                {
                    var controller = match.Groups[1].Value;
                    //两个变量
                    var action = "";
                    var @params = "";

                    action = match.Groups[2].Value;
                    @params = match.Groups[3].Value;
                    if (string.IsNullOrEmpty(controller))
                    {
                        controller = "home";
                    }

                    if (string.IsNullOrEmpty(action))
                    {
                        action = "index";
                    }

                    var pageKey = $"{controller}-{action}";

                    //未找到404
                    if (!_pages.ContainsKey(pageKey))
                    {
                        controller = "error";
                        action = "404";
                        pageKey = $"{controller}-{action}";
                    }
                    else
                    {
                        //未登录需要登录的情况下
                        if (!logged && _pages[pageKey].RequireLogged)
                        {
                            //直接跳走(到登录页面)
                            context.Response.Redirect("/home/index?fu=" + HttpUtility.UrlEncode(SystemHelper.GetCurrentUrl()));
                            return;
                        }

                        //已登录但不能访问
                        if (logged && _pages[pageKey].LoggedNoAccess)
                        {
                            //直接跳到个人中心(我的信息)
                            context.Response.Redirect("/home/main");
                            return;
                        }
                    }

                    //做校验
                    if (_pages.ContainsKey(pageKey) && _pages[pageKey].Checkers != null && _pages[pageKey].Checkers.Any())
                    {
                        foreach (var checker in _pages[pageKey].Checkers)
                        {
                            if (_checkers.ContainsKey(checker))
                            {
                                var result = _checkers[checker].Check(userId, context);
                                if (!result)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    //输出可访问的页面
                    var html = "";
                    if (_pages.ContainsKey(pageKey))
                    {
                        html = _pages[pageKey].Html;
                    }

                    if (!string.IsNullOrEmpty(html))
                    {
                        html = html.Replace("${params}", @params).Replace("${controller}", controller).Replace("${action}", action);
                        context.Response.ContentType = "text/html;charset=utf-8";
                        await context.Response.WriteAsync(html);
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}