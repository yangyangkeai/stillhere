// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using Newtonsoft.Json;

namespace Nproj.StillHereApp.Model.Data
{
    /// <summary>
    /// Label
    /// </summary>
    [Serializable]
    public class Label
    {
        /// <summary>
        ///  比较规则
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Label)obj;
            return Id == other.Id;
        }

        /// <summary>
        /// id的hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Val
        /// </summary>
        public string Val { get; set; }

        /// <summary>
        /// ExtData
        /// </summary>
        public string ExtData { get; set; }

        /// <summary>
        /// 用来排序
        ///</summary>
        [JsonIgnore]
        public long Sort { get; set; }

        /// <summary>
        /// 用来排序
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; set; }
    }
}