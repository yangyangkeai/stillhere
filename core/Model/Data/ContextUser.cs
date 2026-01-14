// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using Nproj.StillHereApp.Model.Db;

namespace Nproj.StillHereApp.Model.Data;

[Serializable]
public class ContextUser
{
    /// <summary>
    /// 用户的唯一标识
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 用户的唯一编号
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// NickName
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 用户的Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// ContactEmail
    /// </summary>
    public string ContactEmail { get; set; }
    
    /// <summary>
    /// LastCheckInTime
    /// </summary>
    public DateTime LastCheckInTime { get; set; }
}