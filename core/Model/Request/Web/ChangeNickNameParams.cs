// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.ComponentModel.DataAnnotations;
using Nproj.StillHereApp.Model.Regular;

namespace Nproj.StillHereApp.Model.Request.Web;

public class ChangeNickNameParams
{
    /// <summary>
    /// Content
    /// </summary>
    [Required(ErrorMessage = "昵称不能为空")]
    [StringLength(30, ErrorMessage = "昵称长度不能超过30个字符")]
    public string Content { get; set; }
}