// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿

using System;

namespace Nproj.StillHereApp.Model.Db
{
    /// <summary>
    /// 可排序的模型, 有排序值
    /// </summary>
    [Serializable]
    public class SortedModel : DbBaseModel
    {
        /// <summary>
        ///     排序值
        /// </summary>
        public long Sort { get; set; }
    }
}
