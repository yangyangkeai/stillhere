// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using Dapper;
using Nproj.StillHereApp.IDal;
using Nproj.StillHereApp.Model.Db;
using Nproj.StillHereApp.Model.Pager;

namespace Nproj.StillHereApp.Dal
{
    /// <summary>
    /// HistoryDal
    /// </summary>
    public class HistoryDal : BaseDal<History>, IDalHistory
    {
        /// <summary>
        /// GetToFront
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public IList<History> GetToApp(BasePager pager)
        {
            var where = " obj.DelFlag=0 and obj.UserNumber=@UserNumber ";
            var dp = new DynamicParameters();
            dp.Add("UserNumber", pager.Key);
            return GetByPager(pager, where, dp);
        }
    }
}