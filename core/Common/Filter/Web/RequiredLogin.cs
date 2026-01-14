// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using Microsoft.AspNetCore.Mvc.Filters;
using Nproj.StillHereApp.Common.Utils;
using Nproj.StillHereApp.Model.Data;

namespace Nproj.StillHereApp.Common.Filter.Web
{
    /// <summary> 
    /// 验证用户登陆 
    /// </summary>
    public class RequiredLogin : ActionFilterAttribute
    {
        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = HttpContextHelper.Get<ContextUser>(HttpContextHelper.User);
            if (user == null)
            {
                context.Result = JsonResultHelper.RequiredLogin();
                return;
            }
        }
    }
}