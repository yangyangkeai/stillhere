// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.Common.Subscribe
{
    /// <summary>
    /// 环境类型
    /// </summary>
    public static class EnvironmentType
    {
        /// <summary>
        /// 测试环境
        /// </summary>
        public const int ENV_DEBUG = 1;

        /// <summary>
        ///  正式环境
        /// </summary>
        public const int ENV_RELEASE = 4;
    }

    /// <summary>
    /// SubscribeAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        ///  Redis订阅
        /// </summary>
        public const string TYPE_REDIS = "REDIS";

        /// <summary>
        ///  RABBIT订阅
        /// </summary>
        public const string TYPE_RABBIT = "RABBIT";
        
        /// <summary>
        /// Channel  指定订阅频道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 实例  指定订阅实例
        /// </summary>
        public RedisTo Instance { get; set; } = RedisTo.Default;

        /// <summary>
        /// Channels  可以订阅多个频道,会优先使用单个的
        /// </summary>
        public string[] Channels { get; set; }

        /// <summary>
        /// Type  类型, 初始化时决定某个订阅程序只订阅哪个类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 所使用的环境
        /// </summary>
        public int Environment { get; set; } = EnvironmentType.ENV_RELEASE;

        /// <summary>
        /// 指定前缀
        /// </summary>
        /// <param name="channel">通道名</param>
        /// <param name="type">注册标识</param>
        /// <param name="instance">注册实例</param>
        /// <param name="environment">使用环境, 默认在 RELEASE 及 DEBUG 中有效</param>
        public SubscribeAttribute(string channel, string type, RedisTo instance = RedisTo.Default, int environment = EnvironmentType.ENV_RELEASE | EnvironmentType.ENV_DEBUG)
        {
            this.Channel = channel;
            this.Type = type;
            this.Instance = instance;
            this.Environment = environment;
        }

        /// <summary>
        /// 指定前缀
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="type"></param>
        public SubscribeAttribute(string[] channels, string type)
        {
            this.Channels = channels;
            this.Type = type;
        }
    }
}