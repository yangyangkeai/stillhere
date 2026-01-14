// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using Nproj.StillHereApp.Model.Db;

namespace Nproj.StillHereApp.IDal
{
    /// <summary>
    /// User dal接口
    /// </summary>
    public interface IDalUser : IDalBase<User>
    {
        /// <summary>
        /// GetByUserName
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        User GetByDeviceId(string deviceId);
        
        /// <summary>
        /// UpdateNickName
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nickName"></param>
        void UpdateNickName(string number, string nickName);

        /// <summary>
        /// UpdateAvatar
        /// </summary>
        /// <param name="number"></param>
        /// <param name="email"></param>
        void SetContactEmail(string number, string email);

        /// <summary>
        /// UpdateLastCheckInTime
        /// </summary>
        /// <param name="number"></param>
        /// <param name="time"></param>
        void UpdateLastCheckInTime(string number, DateTime time);

        /// <summary>
        /// SetStatusByNumber
        /// </summary>
        /// <param name="number"></param>
        /// <param name="status"></param>
        void SetStatusByNumber(string number, UserStatus status);
    }
}