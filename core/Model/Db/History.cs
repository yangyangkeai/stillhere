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
    /// History
    /// </summary>
    [Table("sys_history")]
    [Serializable]
    public class History : DbBaseModel
    {
        /// <summary>
        /// UserNumber
        /// </summary>
        public string UserNumber { get; set; }
    }
}