// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using Microsoft.AspNetCore.Mvc.Filters;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Filter
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 全局捕获异常
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            //0. 回滚事务
            DbHelper.RollBackTrans();
            DbHelper.RollBackTrans(DbHelperConnectType.Readonly);
            //1. 关闭数据库
            DbHelper.CloseCurrentConnection();
            DbHelper.CloseCurrentConnection(DbHelperConnectType.Readonly);
#if DEBUG
            context.Result = JsonResultHelper.Error(context.Exception.Message);
#else
            context.Result = JsonResultHelper.Error("服务器出现异常");
#endif
            LogHelper.Error($"全局异常消息:" + context.Exception.Message);
            LogHelper.Error($"全局异常堆栈:" + context.Exception.StackTrace);
        }
    }
}