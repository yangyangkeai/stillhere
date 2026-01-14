// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿namespace Nproj.StillHereApp.Model.Pager
{
    /// <summary>
    /// BasePager
    /// </summary>
    public class BasePager
    {
        /// <summary>
        /// 每页大小
        /// </summary>
        private long _pageSize = 20;

        /// <summary>
        /// 当前页码
        /// </summary>
        private long _currentPage = 1;

        /// <summary>
        /// 当前码 默认 1
        /// </summary>
        public long CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                if (_currentPage < 1)
                {
                    _currentPage = 1;
                }
            }
        }

        /// <summary>
        ///  每页大小
        /// </summary>
        public long PageSize
        {
            get => _pageSize;
            set
            {
                //不能超过50条
                _pageSize = value <= 50 ? value : 50;
                //也不能小于10条
                if (_pageSize < 10)
                {
                    _pageSize = 10;
                }
            }
        }

        /// <summary>
        /// 总页码 此参数不必传入, 由系统自行计算
        /// </summary>
        public long TotalPage => TotalRecord / PageSize + 1;

        /// <summary>
        ///  表示一个id
        /// </summary>
        public long? Id { get; set; } = null;

        /// <summary>
        ///  表示一个key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 一般情况下代表一个编号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalRecord { get; set; } = -1;

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 排序类型
        /// </summary>
        public string SortProp { get; set; }

        /// <summary>
        /// 排序类型
        /// </summary>
        public short? SortValue { get; set; } = null;
    }
}