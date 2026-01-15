// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using Microsoft.AspNetCore.Mvc;
using Nproj.StillHereApp.Common.Filter.Web;
using Nproj.StillHereApp.Common.Utils;
using Nproj.StillHereApp.IDal;
using Nproj.StillHereApp.Model.Db;
using Nproj.StillHereApp.Model.Request.Web;

namespace Nproj.StillHereApp.WebApiController
{
    /// <summary>
    /// 用户相关
    /// </summary>
    [RequiredLogin]
    public class UserController : SystemBase
    {
        /// <summary>
        /// 修改昵称(改自己)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetNickName([FromBody] ChangeNickNameParams @params)
        {
            var user = UserHelper.GetFromContext();
            DalFactory.GetInstance<IDalUser>().UpdateNickName(user.Number, @params.Content);
            user.NickName = @params.Content;
            UserHelper.SetCacheToRedis(user);
            return JsonResultHelper.Success();
        }

        /// <summary>
        /// 修改联系人邮箱(改自己)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetContactEmail([FromBody] SetContactEmail @params)
        {
            var user = UserHelper.GetFromContext();
            DalFactory.GetInstance<IDalUser>().SetContactEmail(user.Number, @params.Content);
            user.ContactEmail = @params.Content;
            UserHelper.SetCacheToRedis(user);
            return JsonResultHelper.Success();
        }

        /// <summary>
        /// 修改联系人邮箱(改自己)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckIn()
        {
            var user = UserHelper.GetFromContext();
            if (RedisHelper.Exists(user.Number))
            {
                return JsonResultHelper.Fail("今天已经打过卡了，明天再来吧！");
            }
            
            RedisHelper.Set(user.Number, "1", new TimeSpan(0, ConfigHelper.HourThresholdVal, 0));

            var history = new History()
            {
                UserNumber = user.Number,
                CreateTime = DateTime.Now
            };
            DalFactory.GetInstance<IDalHistory>().Insert(history);
            DalFactory.GetInstance<IDalUser>().UpdateLastCheckInTime(user.Number, history.CreateTime);
            user.LastCheckInTime = history.CreateTime;
            UserHelper.SetCacheToRedis(user);
            return JsonResultHelper.Success(user);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Logout()
        {
            var token = HttpContextHelper.Get<string>(HttpContextHelper.Token);
            UserHelper.Logout(token);
            return JsonResultHelper.Success();
        }
    }
}