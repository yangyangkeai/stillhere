// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using Nproj.StillHereApp.Common.ModelHelper;
using Nproj.StillHereApp.IDal;
using Nproj.StillHereApp.Model.Data;
using Nproj.StillHereApp.Model.Db;

namespace Nproj.StillHereApp.Common.Utils;

/// <summary>
/// UserHelper
/// </summary>
public class UserHelper
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    public static ContextUser Login(string deviceId)
    {
        //如果在公众平台中, 一定有这个
        var user = GetUserFromDb(deviceId);
        if (user != null)
        {
            Login(user);
        }

        return user;
    }

    /// <summary>
    /// LogOut
    /// </summary>
    /// <param name="token"></param>
    public static void Logout(string token)
    {
        ClearContextUserCache(token);
        RedisHelper.Remove(RedisHelper.KEY_UserId + token);
        CookieHelper.Clear(CookieHelper.Token);
    }

    /// <summary>
    /// ClearContextUserCache
    /// </summary>
    /// <param name="userToken"></param>
    public static void ClearContextUserCache(string userToken)
    {
        RedisHelper.Remove(RedisHelper.KEY_User + userToken);
    }

    /// <summary>
    /// GetUserFromDb
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <returns></returns>
    private static ContextUser GetUserFromDb(string deviceId)
    {
        ContextUser user = null;
        var dbUser = DalFactory.GetInstance<IDalUser>().GetByDeviceId(deviceId);
        if (dbUser == null)
        {
            //直接创建一个用户
            dbUser = new User()
            {
                DeviceId = deviceId,
                Number = SystemHelper.GenNumber(NumberFixHelper.SYS_User)
            };
           var id= DalFactory.GetInstance<IDalUser>().Insert(dbUser);
            dbUser.Id = id.HasValue ? id.Value : 0;
        }

        /*
        if (dbUser.IsLock == 1)
        {
            throw new Exception("用户已锁定");
        }
        */

        user = new ContextUser()
        {
            Id = dbUser.Id,
            NickName = dbUser.NickName,
            Number = dbUser.Number,
            ContactEmail = dbUser.ContactEmail,
            LastCheckInTime = dbUser.LastCheckInTime
        };

        return user;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="user"></param>
    private static void Login(ContextUser user)
    {
        var token = HttpContextHelper.Get<string>(HttpContextHelper.Token);
        user.Token = SetToken(user, token);
        //user.Token = SetToken(user);
        //用户加入缓存
        SetCacheToRedis(user);
        //token设置到cookie中
        CookieHelper.Set(CookieHelper.Token, user.Token, false);
    }

    /// <summary>
    /// 生成一个token
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static string SetToken(ContextUser user, string token)
    {
        if (user == null)
        {
            return "";
        }

        var expires = int.Parse(ConfigHelper.Get(ConfigHelper.LoginExpires)); //这个是分钟
        token = string.IsNullOrWhiteSpace(token) ? SystemHelper.Md5(user.Id.ToString() + SystemHelper.DateTimeToUnixTime(DateTime.Now)) : token;
        RedisHelper.Set(RedisHelper.KEY_UserId + token, user.Id, new TimeSpan(0, expires, 0));
        //token校验加进去
        var ua = HttpContextHelper.Get<string>(HttpContextHelper.UserAgent);
        if (string.IsNullOrEmpty(ua))
        {
            ua = HttpContextHelper.GetUa();
        }

        var code = SystemHelper.Md5($"{token}{ua}");
        RedisHelper.Set(token, code, new TimeSpan(0, expires, 0));
        return token;
    }


    /// <summary>
    /// GetFromRedis
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static ContextUser GetFromRedis(long userId, string token)
    {
        if (userId <= 0)
        {
            return null;
        }

        ContextUser user = null;
        try
        {
            user = RedisHelper.Get<ContextUser>(RedisHelper.KEY_User + token);
        }
        catch
        {
            // ignored
        }

        if (user == null)
        {
            ContextUser contextUser = null;
            try
            {
                var dbUser = DalFactory.GetInstance<IDalUser>().GetById(userId);
                if (dbUser != null)
                {
                    contextUser = new ContextUser()
                    {
                        Id = dbUser.Id,
                        NickName = dbUser.NickName,
                        Number = dbUser.Number,
                        ContactEmail = dbUser.ContactEmail,
                        LastCheckInTime = dbUser.LastCheckInTime,
                        Token = token
                    };
                    SetToken(contextUser, token);
                }
            }
            catch
            {
                // ignored
            }

            if (contextUser == null)
            {
                return null;
            }

            user = contextUser;

            SetCacheToRedis(contextUser);
        }

        return user;
    }

    /// <summary>
    /// GetFromContext
    /// </summary>
    /// <returns></returns>
    public static ContextUser GetFromContext()
    {
        return HttpContextHelper.Get<ContextUser>(HttpContextHelper.User);
    }

    /// <summary>
    /// GetTokenTtl
    /// </summary>
    /// <param name="token"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int GetTokenTtl(string token, out string key)
    {
        key = RedisHelper.KEY_UserId + token;
        var ttl = RedisHelper.Ttl(key);
        if (ttl != null)
        {
            return Convert.ToInt32(ttl.Value.TotalMinutes);
        }

        return 0;
    }

    /// <summary>
    /// token续期
    /// </summary>
    /// <param name="token"></param>
    public static void TokenExtension(string token)
    {
        var ttl = GetTokenTtl(token, out var key);
        if (ttl > 0 && ttl <= ConfigHelper.LoginAutoExtensionVal)
        {
            var expires = int.Parse(ConfigHelper.Get(ConfigHelper.LoginExpires)); //这个是分钟
            //自动续期
            RedisHelper.UpdateExpire(key, new TimeSpan(0, expires, 0));
            RedisHelper.UpdateExpire(token, new TimeSpan(0, expires, 0));
        }
    }

    /// <summary>
    /// SetCacheToRedis
    /// </summary>
    /// <param name="user"></param>
    public static void SetCacheToRedis(ContextUser user)
    {
        if (user == null)
        {
            return;
        }

        RedisHelper.Set(RedisHelper.KEY_User + user.Token, user, new TimeSpan(0, 30, 0));
    }

    /// <summary>
    /// SetContextUserUseToken
    /// </summary>
    /// <param name="token"></param>
    public static bool SetContextUserUseToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            HttpContextHelper.Set(HttpContextHelper.Token, token);
            //从缓存, 得到用户id
            var userId = RedisHelper.Get<long>(RedisHelper.KEY_UserId + token);
            if (userId > 0)
            {
                //token续期
                TokenExtension(token);
                //从缓存读用户
                var user = GetFromRedis(userId, token);
                HttpContextHelper.Set(HttpContextHelper.User, user);
                return true;
            }
        }

        return false;
    }
}