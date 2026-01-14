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
/// 通用配置
/// </summary>
public class PageConfig
{
    
    /// <summary>
    /// 默认标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 默认关键字
    /// </summary>
    public string Keywords { get; set; }

    /// <summary>
    /// 默认描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 通用的css, 仅css路径
    /// </summary>
    public IList<string> CommCss { get; set; }

    /// <summary>
    /// 通用的js, 仅js路径
    /// </summary>
    public IList<string> CommJs { get; set; }

    /// <summary>
    /// 通用的底部Html元素
    /// </summary>
    public IList<string> CommFootContent { get; set; }

    /// <summary>
    /// 通用的头部Html元素
    /// </summary>
    public IList<string> CommHeadContent { get; set; }

    /// <summary>
    /// 页面配置
    /// </summary>
    public IList<PageMeta> Pages { get; set; }
}