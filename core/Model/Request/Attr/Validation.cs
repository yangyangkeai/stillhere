// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;

namespace Nproj.StillHereApp.Model.Request.Attr
{
    /// <summary>
    /// 自定义的验证规则
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Validation : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="required"></param>
        /// <param name="regex">可以没有</param>
        /// <param name="msg"></param>
        public Validation(bool required, string regex = null, string msg = null)
        {
            Required = required;
            Regex = regex;
            Msg = msg;
        }

        /// <summary>
        /// 必须
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 正则
        /// </summary>
        public string Regex { get; set; }

        /// <summary>
        /// Msg
        /// </summary>
        public string Msg { get; set; }
    }
}