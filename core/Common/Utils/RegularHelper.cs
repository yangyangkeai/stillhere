// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Text.RegularExpressions;

namespace Nproj.StillHereApp.Common.Utils
{
    public static class RegularHelper
    {
        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="strTelephone"></param>
        /// <returns></returns>
        public static bool IsTelephone(string strTelephone)
        {
            return Regex.IsMatch(strTelephone, @"^(\d{3,4}-)?\d{6,8}$");
        }

        /// <summary>
        /// IsName
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsName(string str)
        {
            return Regex.IsMatch(str, "^[\\u4e00-\\u9fa5a-zA-Z0-9 ]*$");
        }

        /// <summary>
        /// 验证手机号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobile(string mobile)
        {
            return Regex.IsMatch(mobile, @"^(0|86|17951)?1[23456789]\d{9}$");
            //1[345789]\d{9}$
        }

        /// <summary>
        /// 验证身份证号
        /// </summary>
        /// <param name="strIdcard"></param>
        /// <returns></returns>
        public static bool IsIDcard(string strIdcard)
        {
            return Regex.IsMatch(strIdcard, @"(^\d{18}$)|(^\d{15}$)");
        }

        /// <summary>
        /// 验证是否是数字, 可以有负号, 小数点
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsDecimal(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }

            Regex regex = new Regex(@"^(-)?\d+(\.\d+)?$");
            if (regex.IsMatch(number))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 验证是否是数字,只能是纯数字 不能有负号, 小数点
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumber(string strNumber)
        {
            return Regex.IsMatch(strNumber, @"^[0-9]*$");
        }

        /// <summary>
        /// 验证邮编
        /// </summary>
        /// <param name="strPostalcode"></param>
        /// <returns></returns>
        public static bool IsPostalcode(string strPostalcode)
        {
            return Regex.IsMatch(strPostalcode, @"^\d{6}$");
        }

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static bool IsEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?");
        }

        /// <summary>
        /// 验证Url
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static bool IsUrl(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^[A-Za-z]+://[A-Za-z0-9-_]+\.[A-Za-z0-9-_%&?/.=]+$");
        }

        /// <summary>
        /// 校验字符串, 过了返回true
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool ChkString(string title, int min = -1, int max = -1)
        {
            if (string.IsNullOrEmpty(title))
            {
                return false;
            }

            if (min > -1 && title.Length < min)
            {
                return false;
            }

            if (max > -1 && title.Length > max)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// IsTime
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsTime(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]{2}:[0-9]{2}$");
        }

        /// <summary>
        /// IsAToZ
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsAToZ(string str)
        {
            return Regex.IsMatch(str, @"^[A-Z]$");
        }

        /// <summary>
        /// 剔除所有的空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSpace(string str)
        {
            return Regex.Replace(str, @"\s", "");
        }

        /// <summary>
        /// IsSource
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsSource(string source)
        {
            return Regex.IsMatch(source, @"^[A-Za-z0-9_-]{1,45}$");
        }

        /// <summary>
        /// 只能包含字母和数字还有中划线
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool LetterAndNumberAndGang(string source)
        {
            return Regex.IsMatch(source, @"^[A-Za-z0-9-]+$");
        }

        /// <summary>
        /// IsTaskId 32位的数字和小写字母组合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsTaskId(string id)
        {
            return Regex.IsMatch(id, @"^[a-z0-9]{32}$");
        }
    }
}