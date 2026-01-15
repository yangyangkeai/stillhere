// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using Microsoft.AspNetCore.Mvc;
using Nproj.StillHereApp.Common.Filter;
using Nproj.StillHereApp.Common.Utils;
using Nproj.StillHereApp.Model.Data;
using Nproj.StillHereApp.Model.Request.Web;

namespace Nproj.StillHereApp.WebApiController
{
    /// <summary>
    /// 通用Api
    /// </summary>
    public class CommApiController : SystemBase
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Init([FromBody] LoginParams @params)
        {
            var user = UserHelper.GetFromContext();
            if (user == null)
            {
                //没有登录，尝试自动登录
                try
                {
                    user = UserHelper.Login(@params.DeviceId);
                }
                catch (Exception e)
                {
                    return JsonResultHelper.Fail(e.Message);
                }
            }

            return JsonResultHelper.Success(new
            {
                user,
                hourThreshold = ConfigHelper.HourThresholdVal
            });
        }
    }
}