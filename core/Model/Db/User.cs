// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nproj.StillHereApp.Model.Db
{
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 异常 保留
        /// </summary>
        Abnormal = 1,

        /// <summary>
        /// 已通知
        /// </summary>
        Notified = 1024
    }

    /// <summary>
    /// User
    /// </summary>
    [Table("sys_user")]
    [Serializable]
    public class User : DbBaseModel
    {
        /// <summary>
        /// 用户的状态
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 系统内唯一的编号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        ///  LoginId
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// 最后签到时间
        /// </summary>
        public DateTime LastCheckInTime { get; set; }
    }
}