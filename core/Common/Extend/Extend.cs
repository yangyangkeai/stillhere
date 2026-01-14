// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nproj.StillHereApp.Common.Extend
{
    public static class Extend
    {
        /// <summary>
        /// list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToWhere(this IList<string> list)
        {
            var r = new StringBuilder();
            r.Append(" (");
            var index = 0;
            foreach (var s in list)
            {
                r.Append(s);
                if (index < list.Count - 1)
                {
                    r.Append(" or ");
                }

                index++;
            }

            r.Append(") ");
            return r.ToString();
        }

        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Merge(this IList<string> list)
        {
            if (list == null || !list.Any())
            {
                return "";
            }

            var sb = new StringBuilder();
            foreach (var s in list)
            {
                sb.Append(s);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string MergeToCss(this IList<string> list)
        {
            if (list == null || !list.Any())
            {
                return "";
            }

            var sb = new StringBuilder();
            foreach (var s in list)
            {
                sb.Append($"<link type=\"text/css\" href=\"{s}\" rel=\"stylesheet\">");
            }

            return sb.ToString();
        }

        /// <summary>
        /// MergeToScript
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string MergeToScript(this IList<string> list)
        {
            if (list == null || !list.Any())
            {
                return "";
            }

            var sb = new StringBuilder();
            foreach (var s in list)
            {
                sb.Append($"<script type=\"text/javascript\" src=\"{s}\"></script>");
            }

            return sb.ToString();
        }


        /// <summary>
        /// ToSqlIn
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToSqlIn(this IList<string> list)
        {
            //为啥要有个0呢, 因为 没有 跟 不限制这个条件是两回事
            //没有就查不到
            //不限制会得到所有数据
            if (list == null || !list.Any())
            {
                return "'0'";
            }

            var r = "'0'";
            foreach (var s in list)
            {
                r += $",'{s}'";
            }

            return r;
        }
    }
}