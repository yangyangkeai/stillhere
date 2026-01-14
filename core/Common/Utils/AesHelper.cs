// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace Nproj.StillHereApp.Common.Utils
{
    /// <summary>
    /// AES加密解密帮助类
    /// </summary>
    public class AesHelper
    {
        /// <summary>
        /// 默认密钥
        /// </summary>
        private static string _key = ConfigHelper.Get(ConfigHelper.AesKey);

        /// <summary>
        /// 默认IV
        /// </summary>
        private static byte[] _iv = new byte[16]
        {
            1, 2, 3, 4, 5, 6, 7, 8,
            9, 10, 11, 12, 13, 14, 15, 16
        };

        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="str">需要加密字符串</param>  
        /// <returns>加密后字符串</returns>  
        public static string Encrypt(string str)
        {
            return Encrypt(str, _key);
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="str">需要解密字符串</param>  
        /// <returns>解密后字符串</returns>  
        public static string Decrypt(string str)
        {
            return Decrypt(str, _key);
        }

        /// <summary>
        /// Hex字符串转字节数组
        /// </summary>
        /// <param name="hex">Hex字符串</param>
        /// <returns></returns>
        private static byte[] HexStringToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return Array.Empty<byte>();
            }

            // 去掉空格
            hex = hex.Replace(" ", "");

            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string must have even length.");
            }

            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2)
            {
                // 每两个字符转换为一个字节
                var byteValue = hex.Substring(i, 2);
                bytes[i / 2] = Convert.ToByte(byteValue, 16);
            }

            return bytes;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="key">32位密钥</param>
        /// <returns>加密后的字符串</returns>
        private static string Encrypt(string str, string key)
        {
            byte[] encrypted;
            var keyArray = HexStringToBytes(key);
            using (var aes = Aes.Create())
            {
                aes.Key = keyArray;
                aes.IV = _iv;
                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(str);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <param name="key">32位密钥</param>
        /// <returns>解密后的字符串</returns>
        private static string Decrypt(string str, string key)
        {
            var keyArray = HexStringToBytes(key);
            var strArray = Convert.FromBase64String(str);

            string res = null;
            using var aes = Aes.Create();
            aes.Key = keyArray;
            aes.IV = _iv;

            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            using var msDecrypt = new MemoryStream(strArray);
            using var csDecrypt = new CryptoStream(msDecrypt, decrypt, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            res = srDecrypt.ReadToEnd();

            return res;
        }

        /// <summary>
        /// TryDecrypt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TryDecrypt(string str)
        {
            try
            {
                return Decrypt(str);
            }
            catch (Exception e)
            {
                LogHelper.Error($"AesHelper TryDecrypt Error, Msg={e.Message},Str={str}");
                return "";
            }
        }

        /// <summary>
        /// DecryptSignKey
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string DecryptSignKey(string ciphertext, string userAgent)
        {
            if (string.IsNullOrEmpty(ciphertext))
            {
                return "";
            }

            var result = TryDecrypt(ciphertext);

            if (string.IsNullOrEmpty(result))
            {
                return "";
            }

            var arr = result.Split(':');

            if (arr.Length != 2)
            {
                return "";
            }

            var currentTimestamp = SystemHelper.ConvertDateTimeLong(DateTime.Now);
            var timestampConvertResult = long.TryParse(arr[0], out var requestTimestamp);
            var timestampDiff = Math.Abs(currentTimestamp - requestTimestamp);
            var flag = timestampDiff > 120 * 60 * 1000; //一个动态key最长120分钟有效期
            if (flag || !timestampConvertResult)
            {
                return "";
            }

            //0 time
            //1 rand
            return SystemHelper.Md5(userAgent + arr[0] + arr[1]);
        }

        /// <summary>
        /// GenerateSignKey
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="userAgent"></param>
        /// <param name="timestamp"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static string GenerateSignKey(out string ciphertext, string userAgent, string timestamp, string random)
        {
            ciphertext = Encrypt(timestamp + ":" + random);
            return SystemHelper.Md5(userAgent + timestamp + random);
        }

        /// <summary>
        /// CheckSign
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="key"></param>
        /// <param name="timestamp"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static bool CheckSign(string sign, string key, string timestamp, string random)
        {
            return sign.ToLower() == SystemHelper.Md5(key + "---" + timestamp + "---" + random).ToLower();
        }

        /// <summary>
        /// DecryptUserAgent
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string DecryptUserAgent(string userAgent)
        {
            userAgent = SystemHelper.UrlDecode(userAgent);
            var str = TryDecrypt(userAgent);
            //将str按':'分割，取第一个部分作为结果返回
            var arr = str.Split(':');
            if (arr.Length > 0)
            {
                return arr[0];
            }

            return "";
        }
    }
}