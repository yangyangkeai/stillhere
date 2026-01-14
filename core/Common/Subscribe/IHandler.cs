// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using StackExchange.Redis;

namespace Nproj.StillHereApp.Common.Subscribe
{
    /// <summary>
    ///没啥事的情况下少用此功能
    /// </summary>
    interface IHandler
    {
        void Execute(RedisChannel channel, RedisValue value);
    }
}
