// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using Nproj.StillHereApp.Model.Pager;

namespace Nproj.StillHereApp.Common.Utils
{
    /// <summary>
    /// 数据库连接类型
    /// </summary>
    public enum DbHelperConnectType
    {
        /// <summary>
        /// Writable
        /// </summary>
        Writable,

        /// <summary>
        /// Readonly
        /// </summary>
        Readonly
    }

    public static class DbHelper
    {
        /// <summary>
        /// GetConnectionStr
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetConnectionStr(DbHelperConnectType type)
        {
            switch (type)
            {
                case DbHelperConnectType.Writable:
                    return ConfigHelper.Get(ConfigHelper.DbConnection);
                case DbHelperConnectType.Readonly:
                    return ConfigHelper.Get(ConfigHelper.DbConnectionReadonly);
            }

            throw new System.Exception("无效的数据库连接类型");
        }

        /// <summary>
        /// 取得一个新的连接
        /// </summary>
        /// <returns></returns>
        /// <param name="type"></param>
        /// <param name="save"></param>
        public static IDbConnection GetNewConnection(DbHelperConnectType type = DbHelperConnectType.Writable, bool save = false)
        {
            IDbConnection conn = new MySqlConnection(GetConnectionStr(type));
            if (save)
            {
                var key = HttpContextHelper.DbConn + type;
                HttpContextHelper.Set(key, conn);
            }

            conn.Open();
            return conn;
        }


        /// <summary>
        /// 获取当前上下文中的数据库连接, 要是没有, 则返回新的
        /// </summary>
        /// <param name="create">如果为true会创建</param>
        /// <param name="type">如果为true会创建</param>
        /// <returns></returns>
        public static IDbConnection GetCurrentConnection(bool create = true, DbHelperConnectType type = DbHelperConnectType.Writable)
        {
            IDbConnection conn = HttpContextHelper.Get<IDbConnection>(HttpContextHelper.DbConn + type);
            if (conn == null)
            {
                if (create)
                {
                    return GetNewConnection(type, true);
                }

                return null;
            }

            return conn;
        }


        /// <summary>
        /// 获取分页数据, 复杂版本, 支持join
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="where">查询条件</param>
        /// <param name="param">参数</param>
        /// <param name="join">连接</param>
        /// <param name="orderBy">排序</param>
        /// <param name="select">选择什么</param>
        /// <param name="tableName">表 </param>
        /// <param name="tableIsSubQuery">表为子查寻</param>
        /// <param name="groupBy">分组</param>
        /// <param name="type">分组</param>
        /// <returns></returns>
        public static IList<TP> GetByPager<TP>(string tableName, BasePager pager, string where = "", object param = null, string select = "", string join = "", string orderBy = "", bool tableIsSubQuery = false, string groupBy = "", DbHelperConnectType type = DbHelperConnectType.Readonly)
        {
            var baseSql = " from " + tableName + (tableIsSubQuery ? "" : " as obj ") +
                          (!string.IsNullOrEmpty(join) ? join : "") + " " +
                          (!string.IsNullOrEmpty(where) ? "where " + where : "") + " ";

            baseSql += string.IsNullOrEmpty(groupBy) ? "" : " GROUP BY " + groupBy + " ";

            var orderBySql = (!string.IsNullOrEmpty(orderBy)
                ? "order by " + orderBy
                : "order by obj.CreateTime desc");

            var countSql = "";

            if (string.IsNullOrEmpty(groupBy))
            {
                countSql = "select count(*) " + baseSql;
            }
            else
            {
                countSql = "select count(*) from (select " + groupBy + " " + baseSql + ") as sub";
            }

            var count = GetCurrentConnection(type: type).QueryFirstOrDefault<long>(countSql, param);
            pager.TotalRecord = count;

            if (count > 0)
            {
                if (pager.CurrentPage > pager.TotalPage)
                {
                    pager.CurrentPage = pager.TotalPage;
                }

                var dataSql = "select " + (!string.IsNullOrEmpty(select) ? select : "obj.*") + baseSql + orderBySql + " limit " + ((pager.CurrentPage - 1) * pager.PageSize) + " , " + (pager.PageSize);
                var list = GetCurrentConnection(type: type).Query<TP>(dataSql, param).ToList();
                return list;
            }

            return new List<TP>();
        }


        /// <summary>
        /// 获取当前的事务(整个请求全局有效)
        /// </summary>
        /// <param name="createNew"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbTransaction GetCurrentTransaction(bool createNew = true, DbHelperConnectType type = DbHelperConnectType.Writable)
        {
            IDbTransaction transaction = HttpContextHelper.Get<IDbTransaction>(HttpContextHelper.DbTransaction + type);
            if (transaction == null && createNew)
            {
                if (GetCurrentConnection(true, type).State != ConnectionState.Open)
                {
                    GetCurrentConnection(false, type).Open();
                }

                transaction = GetCurrentConnection(false, type).BeginTransaction();
                HttpContextHelper.Set(HttpContextHelper.DbTransaction + type, transaction);
            }

            return transaction;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public static void SubmitTrans(DbHelperConnectType type = DbHelperConnectType.Writable)
        {
            IDbTransaction transaction = HttpContextHelper.Get<IDbTransaction>(HttpContextHelper.DbTransaction + type);
            if (transaction != null)
            {
                transaction.Commit();
                HttpContextHelper.Set(HttpContextHelper.DbTransaction + type, null);
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public static void RollBackTrans(DbHelperConnectType type = DbHelperConnectType.Writable)
        {
            IDbTransaction transaction = HttpContextHelper.Get<IDbTransaction>(HttpContextHelper.DbTransaction + type);
            if (transaction != null)
            {
                transaction.Rollback();
                HttpContextHelper.Set(HttpContextHelper.DbTransaction + type, null);
            }
        }

        /// <summary>
        /// 执行一条更新语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <param name="type"></param>
        public static int ExecuteNonQuery(string sql, object paras, DbHelperConnectType type = DbHelperConnectType.Writable)
        {
            return GetCurrentConnection(type: type).Execute(sql, paras);
        }

        /// <summary>
        /// 执行一条查询语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <param name="type"></param>
        public static IDataReader ExecuteQuery(string sql, object paras, DbHelperConnectType type = DbHelperConnectType.Readonly)
        {
            return GetNewConnection(type).ExecuteReader(sql, paras);
        }

        /// <summary>
        /// 关闭当前上下文中的有效数据库连接
        /// </summary>
        /// <param name="type"></param>
        public static void CloseCurrentConnection(DbHelperConnectType type = DbHelperConnectType.Writable)
        {
            var conn = GetCurrentConnection(false, type);
            //LogHelper.Debug("关闭数据库连接, conn==null " + (conn == null));
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 执行一行语句, 返回datareader
        /// </summary>
        /// <returns></returns>
        public static IDataReader ExecuteReaderBySql(string sql, DynamicParameters param = null, DbHelperConnectType type = DbHelperConnectType.Readonly)
        {
            var conn = GetCurrentConnection(type: type);
            var result = conn.ExecuteReader(sql, param);
            return result;
        }
    }
}