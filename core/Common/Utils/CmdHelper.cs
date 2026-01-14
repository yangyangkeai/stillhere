// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Diagnostics;
using System.Threading;

namespace Nproj.StillHereApp.Common.Utils;

public delegate void CallBack(string output);

public class CmdHelper
{
    /// <summary>
    /// 执行一个外部命令
    /// </summary>
    /// <param name="command"></param>
    /// <param name="params"></param>
    /// <param name="callBack">回调方法</param>
    public static void RunCmd(string command, string @params, CallBack callBack = null)
    {
        var process = new Process();
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = @params;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.ErrorDataReceived += (sender, output) =>
        {
            if (!string.IsNullOrEmpty(output.Data) && callBack != null)
            {
                callBack(output.Data);
            }
        };
        process.Start();
        process.BeginErrorReadLine();
        process.WaitForExit();
        process.Close();
        Thread.Sleep(2 * 1000);
    }
}