// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Data;
using Dapper;
using MimeKit;
using Nproj.StillHereApp.Common.Utils;
using Nproj.StillHereApp.IDal;
using Nproj.StillHereApp.Model.Db;

namespace App.SendMails;

/// <summary>
/// 此程序由系统cron定时触发执行。每隔10分钟执行一次。
/// 处理最后一次打卡时间超过指定时间(小时)的用户，发送邮件通知好友。
/// </summary>
class Program
{
    /// <summary>
    /// 配置
    /// 超过多少小时未打卡则发送邮件通知
    /// </summary>
    private static readonly int HourThreshold = ConfigHelper.HourThresholdVal;

    /// <summary>
    /// 程序入口
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
#if DEBUG
        ConfigHelper.Init("appsettings.development.json");
        LogHelper.Init("nlog.config");
#else
        ConfigHelper.Init("appsettings.json");
        LogHelper.Init("nlog.config");
#endif
        //conn
        var conn = DbHelper.GetNewConnection(save: true);
        var now = DateTime.Now.Date.AddHours(0 - HourThreshold); //两天前
        LogHelper.Debug($"开始处理{now}之前未打卡的用户邮件通知");
        //sql
        var sql = @"SELECT Number,ContactEmail,NickName from sys_user where DelFlag=0 and Status=0 and LastCheckInTime is not null and LastCheckInTime<=@LastCheckInTime order by LastCheckInTime asc";
        var list = conn.Query<(string, string, string)>(sql, new { LastCheckInTime = now }).ToList();
        LogHelper.Debug("查询到需要发送邮件的用户数量：" + list.Count);
        foreach (var item in list)
        {
            try
            {
                LogHelper.Debug($"给用户{item.Item1}发送邮件通知到{item.Item2}");
                var program = new Program();
                program.SendMail(item.Item3, item.Item2);
                LogHelper.Debug($"给用户{item.Item1}发送邮件通知到{item.Item2}成功");
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                DalFactory.GetInstance<IDalUser>().SetStatusByNumber(item.Item1, UserStatus.Notified);
                LogHelper.Debug($"更新用户{item.Item1}状态为已通知成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"给用户{item.Item1}发送邮件通知到{item.Item2}失败，Msg：{ex.Message}。StackTrace：{ex.StackTrace}");
            }
        }

        DbHelper.CloseCurrentConnection();
        LogHelper.Debug("处理完成");
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="name">我自己的名称</param>
    /// <param name="to">发送给谁</param>
    void SendMail(string name, string to)
    {
        var title = ConfigHelper.Get(ConfigHelper.KEY_SmtpTitle);
        var body = ConfigHelper.Get(ConfigHelper.KEY_SmtpBody, new Dictionary<string, string>()
        {
            { "name", name }
        });
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(ConfigHelper.Get(ConfigHelper.KEY_SmtpName), ConfigHelper.Get(ConfigHelper.KEY_SmtpAccount)));
        message.To.Add(new MailboxAddress(to.Substring(0, to.IndexOf("@", StringComparison.Ordinal)), to));
        message.Subject = title;
        message.Body = new TextPart("html")
        {
            Text = body
        };
        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.Connect(ConfigHelper.Get(ConfigHelper.KEY_SmtpServer), 465, MailKit.Security.SecureSocketOptions.Auto);
        client.Authenticate(ConfigHelper.Get(ConfigHelper.KEY_SmtpAccount), ConfigHelper.Get(ConfigHelper.KEY_SmtpPwd));
        client.Send(message);
        client.Disconnect(true);
    }
}