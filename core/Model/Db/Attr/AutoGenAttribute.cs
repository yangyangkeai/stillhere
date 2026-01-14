// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;

namespace Nproj.StillHereApp.Model.Db.Attr;

public enum AutoGenType
{
    /// <summary>
    /// 一个32位的Guid
    /// </summary>
    Guid,

    /// <summary>
    /// 一个有前缀的数字
    /// </summary>
    Number,

    /// <summary>
    ///     纯数字
    /// </summary>
    PureNumber
}

[AttributeUsage(AttributeTargets.Property)]
public class AutoGenAttribute : Attribute
{
    public AutoGenType Type { get; set; }
    public string Prefix { get; set; }
    public short Length { get; set; }

    public AutoGenAttribute(AutoGenType type, string prefix = null, short length = -1)
    {
        Type = type;
        Prefix = prefix;
        Length = length;
    }
}