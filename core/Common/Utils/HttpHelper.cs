// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using RestSharp;

namespace Nproj.StillHereApp.Common.Utils
{
    /// <summary>
    /// HttpHelper
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// _client
        /// </summary>
        private static RestClient _client = null;

        /// <summary>
        /// ContentType_Json
        /// </summary>
        public static string ContentType_Json = "application/json";

        static HttpHelper()
        {
            _client = new RestClient(options =>
            {
                options.Encoding = Encoding.UTF8;
                options.RemoteCertificateValidationCallback = (message, cert, chain, error) => true;
            });
        }

        /// <summary>
        /// 发送数据到指定的url, 服务器返回byte[]
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="data">要发送的数据</param>
        /// <param name="contentType">标准的ContentType值</param>
        /// <param name="dic">自定义请求头</param>
        /// <returns>>服务器响应的byte[]</returns>
        public static byte[] PostReadByte(string url, string data, string contentType = null, Dictionary<string, string> dic = null)
        {
            return SendDataToUrl<byte[]>(Encoding.UTF8.GetBytes(data), url, contentType, dic);
        }

        ///<summary>
        /// 发起一个Post请求, 获取服务器返回的字符串
        ///</summary>
        ///<param name="data">要Post的数据</param>
        ///<param name="url">目标url</param>
        /// <param name="contentType">标准的ContentType值</param>
        /// <param name="headers">自定义请求头</param>
        ///<returns>服务器响应的字符串信息</returns>
        public static string Post(string data, string url, string contentType = "", Dictionary<string, string> headers = null)
        {
            return SendDataToUrl<string>(Encoding.UTF8.GetBytes(data), url, contentType, headers);
        }

        /// <summary>
        /// 判断一个url能否正常访问
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool UrlIsReady(string url)
        {
            try
            {
                var request = new RestRequest(url, Method.Head);
                var response = _client.Execute(request);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                LogHelper.Error($"UrlIsOk Error, Url={url}, Msg={e.Message},Trace={e.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 从指定url下载一个文件
        /// </summary>
        /// <param name="url">要下载文件的url</param>
        /// <returns>返回的是一个文件的byte[]</returns>
        public static byte[] DownFile(string url)
        {
            var request = new RestRequest(url);
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.AddHeader("cache-control", " no-cache");
            return _client.DownloadData(request);
        }

        /// <summary>
        /// 从指定url下载一个文件
        /// </summary>
        /// <param name="url">要下载文件的url</param>
        /// <param name="filePath">一个相对当前项目的文件路径(包含文件名)</param>
        /// <returns>如果下载成功,返回的是一个文件的绝对地址. 否则, 返回空字符串</returns>
        public static string DownFile(string url, string filePath)
        {
            var bytes = DownFile(url);
            if (bytes == null || bytes.Length == 0)
            {
                return "";
            }

            var path = HttpContextHelper.GetPath(filePath);
            var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
            return path;
        }

        ///<summary>
        /// 发送数据到指定的url
        ///</summary>
        ///<param name="body">要send的数据</param>
        ///<param name="url">目标url</param>
        ///<param name="contentType">contentType</param>
        ///<param name="headers">contentType</param>
        ///<param name="method">默认就是post</param>
        /// <typeparam name="T">期望服务器响应, 注意只能接受返回string及bytes[]</typeparam>
        ///<returns>服务器响应, 注意只能接受返回string及bytes[]</returns>
        private static T SendDataToUrl<T>(byte[] body, string url, string contentType = "", Dictionary<string, string> headers = null, Method method = Method.Post)
        {
            var type = typeof(T);
            if (type != typeof(string) && type != typeof(byte[]))
            {
                throw new ArgumentException("T 只能是string或byte[]");
            }

            headers ??= new Dictionary<string, string>();

            var request = new RestRequest(url, method);
            foreach (var item in headers)
            {
                request.AddHeader(item.Key, item.Value);
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                request.AddHeader("Content-Type", contentType);
            }
            else
            {
                contentType = ContentType_Json;
            }

            request.AddBody(body, contentType);
            var response = _client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                switch (type)
                {
                    case var _ when type == typeof(string):
                        return (T)(object)response.Content;
                    case var _ when type == typeof(byte[]):
                        return (T)(object)response.RawBytes;
                }
            }

            LogHelper.Error($"SendDataToUrl Error. Url={url},Response={response.StatusCode}");
            throw new System.Exception($"SendDataToUrl Error. Url={url},Response={response.StatusCode}");
        }

        /// <summary>
        /// 发起一个Get请求, 获取服务器返回的字符串
        /// </summary>
        /// <param name="data">要发送的数据, 放在body中传输</param>
        /// <param name="url">目标url</param>
        /// <param name="contentType">标准的ContentType值</param>
        /// <param name="headers">自定义请求头</param>
        /// <returns>服务器响应的字符串信息</returns>
        public static string Get(string data, string url, string contentType = "", Dictionary<string, string> headers = null)
        {
            return SendDataToUrl<string>(Encoding.UTF8.GetBytes(data), url, contentType, headers, Method.Get);
        }

        /// <summary>
        ///  发起一个Get请求, 不往服务器发送任何数据, 纯获取服务器返回的字符串
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="header">自定义请求头</param>
        /// <returns>服务器响应的字符串信息</returns>
        public static string Get(string url, Dictionary<string, string> header = null)
        {
            return Get("", url);
        }
    }
}