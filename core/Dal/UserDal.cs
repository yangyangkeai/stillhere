// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using Dapper;
using Nproj.StillHereApp.IDal;
using Nproj.StillHereApp.Model.Db;

namespace Nproj.StillHereApp.Dal
{
    /// <summary>
    /// UserDal
    /// </summary>
    public class UserDal : BaseDal<User>, IDalUser
    {
        /// <summary>
        /// GetByUserName
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public User GetByDeviceId(string deviceId)
        {
            var sql = "SELECT * FROM sys_user WHERE DeviceId = @deviceId and DelFlag=0";
            var parameters = new { deviceId = deviceId };
            var user = Conn.QueryFirstOrDefault<User>(sql, parameters);
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// UpdateNickName
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nickName"></param>
        public void UpdateNickName(string number, string nickName)
        {
            var sql = "UPDATE sys_user SET NickName = @NickName WHERE Number = @Number";
            var parameters = new { NickName = nickName, Number = number };
            Conn.Execute(sql, parameters);
        }

        /// <summary>
        /// UpdateAvatar
        /// </summary>
        /// <param name="number"></param>
        /// <param name="email"></param>
        public void SetContactEmail(string number, string email)
        {
            var sql = "UPDATE sys_user SET ContactEmail = @email WHERE Number = @Number";
            var parameters = new { email = email, Number = number };
            Conn.Execute(sql, parameters);
        }

        /// <summary>
        /// UpdateLastCheckInTime
        /// </summary>
        /// <param name="number"></param>
        /// <param name="time"></param>
        public void UpdateLastCheckInTime(string number, DateTime time)
        {
            var sql = "UPDATE sys_user SET LastCheckInTime = @time, Status=0 WHERE Number = @Number";
            var parameters = new { time = time, Number = number };
            Conn.Execute(sql, parameters);
        }

        /// <summary>
        /// SetStatusByNumber
        /// </summary>
        /// <param name="number"></param>
        /// <param name="status"></param>
        public void SetStatusByNumber(string number, UserStatus status)
        {
            var sql = "UPDATE sys_user SET Status = @status WHERE Number = @Number";
            var parameters = new { status = (int)status, Number = number };
            Conn.Execute(sql, parameters);
        }
    }
}