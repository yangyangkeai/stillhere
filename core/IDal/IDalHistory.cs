// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using Nproj.StillHereApp.Model.Db;
using Nproj.StillHereApp.Model.Pager;

namespace Nproj.StillHereApp.IDal
{
    /// <summary>
    /// History dal接口
    /// </summary>
    public interface IDalHistory : IDalBase<History>
    {
        /// <summary>
        /// GetToFront
        /// </summary>
        /// <param name="pager">pager</param>
        /// <returns></returns>
        IList<History> GetToApp(BasePager pager);
    }
}