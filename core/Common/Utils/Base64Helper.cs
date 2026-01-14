// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;

namespace Nproj.StillHereApp.Common.Utils
{
    /// <summary>
    /// Base64Helper
    /// </summary>
    public class Base64Helper
    {
        /// <summary>
        /// GetString
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string GetString(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return "";
            }

            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// GetBytes
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string base64)
        {
            return Convert.FromBase64String(base64);
        }
    }
}