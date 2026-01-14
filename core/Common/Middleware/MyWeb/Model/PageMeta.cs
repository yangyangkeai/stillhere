// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;

namespace Nproj.StillHereApp.Common.Middleware.MyWeb.Model;

/// <summary>
/// 页面配置
/// </summary>
public class PageMeta
{
    /// <summary>
    /// 页面使用的Html模板
    /// </summary>
    public string Html { get; set; }

    /// <summary>
    /// 页面使用的css, 仅css路径
    /// </summary>
    public IList<string> Css { get; set; }

    /// <summary>
    /// 页面使用的js, 仅js路径
    /// </summary>
    public IList<string> Js { get; set; }

    /// <summary>
    /// 当前页面中头部放置的其他Html元素
    /// </summary>
    public IList<string> HeadContent { get; set; }

    /// <summary>
    /// 当前页面的Controller
    /// </summary>
    public string Controller { get; set; }

    /// <summary>
    /// 当前页面的Action
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// 如果需要登录才能访问的话就设置为true 
    /// </summary>
    public bool RequireLogged { get; set; } = false;

    /// <summary>
    /// 如果登录后页面不可访问, 就设置为true
    /// </summary>
    public bool LoggedNoAccess { get; set; } = false;

    /// <summary>
    /// 指定一个校验类, 访问当前页面时会去执行这里的验证
    /// </summary>
    public IList<string> Checkers { get; set; }
}