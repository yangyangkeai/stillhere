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

namespace Nproj.StillHereApp.WebApiController
{
    /// <summary>
    /// 完全开放
    /// </summary>
    [Route("api/[controller]/[action]/{id?}")]
    [CommFilter]
    [ExceptionFilter]
    public class OpenController : ControllerBase
    {
        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Test()
        {
            return JsonResultHelper.Success(new
            {
                ip = SystemHelper.GetClientIp(),
                ip2 = Request.Headers["X-Forwarded-For"].ToString(),
                ip3 = Request.Headers["x-Real-IP"].ToString(),
            });
        }
    }
}