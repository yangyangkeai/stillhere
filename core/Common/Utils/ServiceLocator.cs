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
    public static class ServiceLocator
    {

        /// <summary>
        /// Instance
        /// </summary>
        public static IServiceProvider Instance { get; set; }

        /// <summary>
        /// GetService
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            if (Instance == null)
            {
                return default(T);
            }
            return (T)Instance.GetService(typeof(T));
        }
    }
}
