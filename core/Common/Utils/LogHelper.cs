// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using NLog;
using NLog.Web;

namespace Nproj.StillHereApp.Common.Utils
{
    public static class LogHelper
    {
        /// <summary>
        /// _logger
        /// </summary>
        private static Logger _logger;

        /// <summary>
        /// 默认异步写入
        /// </summary>
        private static bool _asyn = true;

        /// <summary>
        /// Tag_Main
        /// </summary>
        private static string Tag_Main = "MAIN";

        /// <summary>
        /// Tag_Js
        /// </summary>
        public static string Tag_Js = "JS";

        /// <summary>
        /// 指定配置文件启动log
        /// </summary>
        /// <param name="config"></param>
        /// <param name="asyn">使用异步方式写入</param>
        public static void Init(string config, bool asyn = true)
        {
            StartLog(config);
            _asyn = asyn;
            return;
        }

        private static void StartLog(string config)
        {
            _logger = NLogBuilder.ConfigureNLog($"config/{config}").GetCurrentClassLogger();
            Debug("日志系统启动!");
        }

        /// <summary>
        /// GetLogEventInfo
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static LogEventInfo GetLogEventInfo(string msg, string tag = "")
        {
            LogEventInfo log = new LogEventInfo();
            //log.Properties["content"] = msg;
            log.Properties["tag"] = string.IsNullOrEmpty(tag) ? Tag_Main : tag;
            log.Message = msg;
            return log;
        }

        /// <summary>
        ///     记录调试信息
        /// </summary>
        /// <param name="msg">记录内容</param>
        /// <param name="tag"></param>
        public static void Debug(string msg, string tag = "")
        {
            if (_asyn)
            {
                System.Threading.Tasks.Task.Run(() => { _logger.Debug(GetLogEventInfo(msg, tag)); });
                return;
            }

            _logger.Debug(GetLogEventInfo(msg, tag));
        }

        /// <summary>
        ///     记录错误信息
        /// </summary>
        /// <param name="msg">记录内容</param>
        /// <param name="tag"></param>
        public static void Error(string msg, string tag = "")
        {
            if (_asyn)
            {
                System.Threading.Tasks.Task.Run(() => { _logger.Error(GetLogEventInfo(msg, tag)); });
                return;
            }

            _logger.Error(GetLogEventInfo(msg, tag));
        }

        /// <summary>
        ///     记录提示信息
        /// </summary>
        /// <param name="msg">记录内容</param>
        /// <param name="tag"></param>
        public static void Info(string msg, string tag = "")
        {
            if (_asyn)
            {
                System.Threading.Tasks.Task.Run(() => { _logger.Info(GetLogEventInfo(msg, tag)); });
                return;
            }

            _logger.Info(GetLogEventInfo(msg, tag));
        }

        /// <summary>
        ///     记录警告信息
        /// </summary>
        /// <param name="msg">记录内容</param>
        /// <param name="tag"></param>
        public static void Warn(string msg, string tag = "")
        {
            if (_asyn)
            {
                System.Threading.Tasks.Task.Run(() => { _logger.Warn(GetLogEventInfo(msg, tag)); });
                return;
            }

            _logger.Warn(GetLogEventInfo(msg, tag));
        }
    }
}