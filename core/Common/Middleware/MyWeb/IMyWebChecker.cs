// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using Microsoft.AspNetCore.Http;

namespace Nproj.StillHereApp.Common.Middleware.MyWeb;

/// <summary>
/// 校验器接口
/// </summary>
public interface IMyWebChecker
{
    /// <summary>
    /// 校验执行的方法
    /// </summary>
    /// <param name="userId">当前用户id</param>
    /// <param name="context">当前请求上下文</param>
    /// <returns></returns>
    public bool Check(long userId, HttpContext context);
}