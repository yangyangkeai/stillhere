// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Filter;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values.SelectMany(v => v.Errors);
#if DEBUG
            //打印出错误信息
            foreach (var error in errors)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("error:");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ResetColor();
                Console.WriteLine(" " + error.ErrorMessage);
            }
#endif
            //获取请求的内容
            var content = HttpContextHelper.ReadContent();
            //将errors连接成字符串
            var errorString = string.Join(";", errors.Select(e => e.ErrorMessage));
            LogHelper.Error($"请求参数错误：【{errorString}】，请求内容：【{content}】");
            context.Result = JsonResultHelper.InvalidParameter(errorString);
        }
    }
}