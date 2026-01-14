// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Nproj.StillHereApp.Common.Utils
{
    /// <summary>
    /// 连接到哪
    /// </summary>
    public enum RedisTo
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,
    }

    public static class RedisHelper
    {
        /// <summary>
        /// 自定义的Json序列化类
        /// </summary>
        class IgnoreJsonIgnoreCamelCaseContractResolver : Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
        {
            protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                // 忽略 JsonIgnore 属性的影响
                property.Ignored = false;

                return property;
            }
        }

        private static Dictionary<string, string> _connects = new();
        private static Dictionary<string, string> _fixs = new();
        private static Dictionary<string, ConnectionMultiplexer> _instances = new();
        
        public static string KEY_UserId = "UserId";
        public static string KEY_User = "User";
        
        /// <summary>
        /// 序列化配置
        /// </summary>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new IgnoreJsonIgnoreCamelCaseContractResolver()
            {
                IgnoreSerializableAttribute = true
            },
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        /// <summary>
        /// RedisHelper
        /// </summary>
        static RedisHelper()
        {
            var redisIpMap = Environment.GetEnvironmentVariable("REDIS_IP_MAP");
            var redisIpMapArr = redisIpMap?.Split('_');
            if (redisIpMapArr == null || redisIpMapArr.Length != 2)
            {
                redisIpMapArr = null;
            }

            var ps = typeof(RedisTo).GetEnumNames();
            foreach (var name in ps)
            {
                var conn = ConfigHelper.Get("Redis:" + name);
                var fix = ConfigHelper.Get("Redis:" + name + "Fix");
                if (string.IsNullOrEmpty(conn))
                {
                    continue;
                }

                if (redisIpMapArr != null)
                {
                    conn = conn.Replace(redisIpMapArr[0], redisIpMapArr[1]);
                }

                Console.WriteLine($"Redis conn={conn},fix={fix}");

                var instance = ConnectionMultiplexer.Connect(conn);
                if (_connects.ContainsKey(name))
                {
                    _connects[name] = conn;
                }
                else
                {
                    _connects.Add(name, conn);
                }

                if (_fixs.ContainsKey(name))
                {
                    _fixs[name] = fix;
                }
                else
                {
                    _fixs.Add(name, fix);
                }

                if (_instances.ContainsKey(name))
                {
                    _instances[name] = instance;
                }
                else
                {
                    _instances.Add(name, instance);
                }
            }
        }
        
        /// <summary>
        /// GetIncrValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static long GetIncrValue(string key, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            return Convert.ToInt64(GetDatabase(redisTo).StringGet(key));
        }

        /// <summary>
        /// Subscribe
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="redisTo"></param>
        public static void Unsubscribe(string channel, RedisTo redisTo = RedisTo.Default)
        {
            //LogHelper.Debug("Subscribe=>" + channel + "," + redisTo);
            ISubscriber sub = GetInstance(redisTo).GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Init
        /// </summary>
        public static void Init(bool enableSubscription = false, string rssType = "RSS")
        {
        }

        /// <summary>
        /// GetInstance
        /// </summary>
        public static ConnectionMultiplexer GetInstance(RedisTo redisTo = RedisTo.Default)
        {
            var key = redisTo.ToString();
            if (_instances.ContainsKey(key) && _instances[key] != null && _instances[key].IsConnected)
            {
                return _instances[key];
            }

            if (_instances.ContainsKey(key) && _instances[key] != null && _instances[key].IsConnecting)
            {
                while (_instances[key].IsConnecting)
                {
                    Thread.Sleep(100);
                }

                return _instances[key];
            }

            var conn = _connects[key];
            var instance = ConnectionMultiplexer.Connect(conn);
            if (_instances.ContainsKey(key))
            {
                _instances[key] = instance;
            }
            else
            {
                _instances.Add(key, instance);
            }

            return instance;
        }

        /// <summary>
        /// GetDatabase
        /// </summary>
        /// <returns></returns>
        public static IDatabase GetDatabase(RedisTo redisTo = RedisTo.Default)
        {
            return GetInstance(redisTo).GetDatabase();
        }

        /// <summary>
        /// 获取一个加了前缀的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static string GetKey(string key, RedisTo redisTo = RedisTo.Default)
        {
            return MergeKey(key, redisTo);
        }

        /// <summary>
        /// 加锁, 返回true加锁成功
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <param name="perpetual"></param>
        /// <returns></returns>
        public static bool Lock(string key, RedisTo redisTo = RedisTo.Default, bool perpetual = false)
        {
            key = MergeKey(key, redisTo);
            if (perpetual)
            {
                return GetDatabase(redisTo).LockTake(key, ConfigHelper.Get(ConfigHelper.WebAppid), TimeSpan.MaxValue);
            }

            return GetDatabase(redisTo).LockTake(key, ConfigHelper.Get(ConfigHelper.WebAppid), new TimeSpan(0, 0, 10));
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static bool LockRelease(string key, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            return GetDatabase(redisTo).LockRelease(key, ConfigHelper.Get(ConfigHelper.WebAppid));
        }

        /// <summary>
        /// 这里的 MergeKey 用来拼接 Key 的前缀，具体不同的业务模块使用不同的前缀。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        private static string MergeKey(string key, RedisTo redisTo = RedisTo.Default)
        {
            var fix = "";

            if (!key.Contains("GF_"))
            {
                var dk = redisTo.ToString();
                if (_fixs.ContainsKey(dk))
                {
                    fix = _fixs[dk];
                }
            }

            return fix + key;
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static T Get<T>(string key, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            try
            {
                return Deserialize<T>(GetDatabase(redisTo).StringGet(key));
            }
            catch (Exception e)
            {
                LogHelper.Error($"Redis get error. e={e.Message}");
            }

            return default;
        }


        /// <summary>
        /// Get
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="second"></param>
        /// <param name="redisTo"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(string key, Func<T> callback, int second = 60, RedisTo redisTo = RedisTo.Default)
        {
            if (Exists($"{key}_null"))
            {
                return default(T);
            }

            var result = Get<T>(key);
            if (result == null)
            {
                result = callback();
                if (result == null)
                {
                    Set($"{key}_null", 1, new TimeSpan(0, 0, second));
                }
                else
                {
                    Set(key, result, new TimeSpan(0, 0, second));
                }
            }

            return result;
        }


        /// <summary>
        /// 执行一个命令,返回以行为单位的字符串数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IList<string> Execute(string cmd, object[] args)
        {
            var r = GetDatabase().Execute(cmd, args);
            if (r == null)
            {
                return new List<string>();
            }

            if (r.IsNull)
            {
                return new List<string>();
            }

            if (r.Type == ResultType.MultiBulk)
            {
                var r1 = (RedisResult[])r;
                var ret = new List<string>();
                foreach (var redisResult in r1)
                {
                    ret.Add(redisResult.ToString());
                }

                return ret;
            }

            return new List<string>();
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static object Get(string key, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            return Deserialize<object>(GetDatabase(redisTo).StringGet(key));
        }

        /// <summary>
        /// 更新一个key的过期时间 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire"></param>
        /// <param name="useMerge"></param>
        /// <param name="redisTo"></param>
        public static void UpdateExpire(string key, TimeSpan expire, bool useMerge = true, RedisTo redisTo = RedisTo.Default)
        {
            if (useMerge)
            {
                key = MergeKey(key, redisTo);
            }

            GetDatabase(redisTo).KeyExpire(key, expire);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="redisTo"></param>
        public static void Set(string key, object value, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            GetDatabase(redisTo).StringSet(key, Serialize(value));
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <param name="redisTo"></param>
        public static void Set(string key, object value, TimeSpan expiry, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            GetDatabase(redisTo).StringSet(key, Serialize(value), expiry);
        }

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static bool Exists(string key, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            return GetDatabase(redisTo).KeyExists(key);
        }

        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <param name="mergeKey"></param>
        /// <returns></returns>
        public static bool Remove(string key, RedisTo redisTo = RedisTo.Default, bool mergeKey = true)
        {
            if (mergeKey)
            {
                key = MergeKey(key, redisTo);
            }

            return GetDatabase(redisTo).KeyDelete(key);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        private static string Serialize(object @object)
        {
            if (@object == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(@object, JsonSerializerSettings);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// SecurityExecute
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public static void SecurityExecute(string key, System.Action action)
        {
            while (!Lock(key))
            {
                Thread.Sleep(100);
            }

            try
            {
                action();
            }
            catch (Exception e)
            {
                LogHelper.Error($"SecurityExecute Error e={e.Message}, ed={e.StackTrace}");
            }

            LockRelease(key);
        }

        /// <summary>
        /// 当作消息代理中间件使用
        /// 消息组建中,重要的概念便是生产者,消费者,消息中间件。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static long Publish(string channel, string message, RedisTo redisTo = RedisTo.Default)
        {
            ISubscriber sub = GetInstance(redisTo).GetSubscriber();
            return sub.Publish($"{channel}", message);
        }

        /// <summary>
        /// 在消费者端得到该消息并输出
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static void Subscribe(string channel, Action<RedisChannel, RedisValue> handler,
            RedisTo redisTo = RedisTo.Default)
        {
            //LogHelper.Debug("Subscribe=>" + channel + "," + redisTo);
            ISubscriber sub = GetInstance(redisTo).GetSubscriber();
            sub.Subscribe(channel, handler);
        }

        /// <summary>
        /// LeftPush
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="redisTo"></param>
        public static long LeftPush(string key, object value, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            var db = GetDatabase(redisTo);
            return db.ListLeftPush(key, Serialize(value));
        }

        /// <summary>
        /// RightPop
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static T RightPop<T>(string key, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            return Deserialize<T>(GetDatabase(redisTo).ListRightPop(key));
        }

        /// <summary>
        /// INCR
        /// </summary>
        /// <param name="key"></param>
        /// <param name="v"></param>
        /// <param name="redisTo"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public static long INCR(string key, long v = 1, TimeSpan? expire = null, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            var db = GetDatabase(redisTo);
            var r = db.StringIncrement(key, v);
            if (expire != null)
            {
                UpdateExpire(key, expire.Value, false);
            }

            return r;
        }

        /// <summary>
        /// 获取一个key的过期剩余时长
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <returns></returns>
        public static TimeSpan? Ttl(string key, RedisTo redisTo = RedisTo.Default)
        {
            key = MergeKey(key, redisTo);
            var db = GetDatabase(redisTo);
            return db.KeyTimeToLive(key);
        }

        /// <summary>
        /// LeftRange
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redisTo"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<T>? LeftRange<T>(string key, RedisTo redisTo = RedisTo.Default, int start = 0, int stop = -1)
        {
            key = MergeKey(key, redisTo);
            var db = GetDatabase(redisTo);
            var list = db.ListRange(key, start, stop);
            return list.Length != 0 ? list.Select(item => Deserialize<T>(item)).ToList() : null;
        }
    }
}