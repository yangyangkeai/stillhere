// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Nproj.StillHereApp.Common.Utils;
using Nproj.StillHereApp.IDal;
using Nproj.StillHereApp.Model.Db;
using Nproj.StillHereApp.Model.Pager;

namespace Nproj.StillHereApp.Dal
{
    /// <summary>
    /// dal基类
    /// </summary>
    /// <typeparam name="TModel">模型</typeparam>
    public class BaseDal<TModel> : IDalBase<TModel> where TModel : DbBaseModel
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        protected IDbConnection Conn => DbHelper.GetCurrentConnection();

        /// <summary>
        /// 只读数据库连接
        /// </summary>
        /// protected IDbConnection ReadonlyConn => DbHelper.GetCurrentConnection(type: DbHelperConnectType.Readonly);
        /// <summary>
        /// 表名称
        /// </summary>
        protected string TableName = string.Empty;

        /// <summary>
        /// 可排序的
        /// </summary>
        protected bool Sorted = false;

        /// <summary>
        /// 默认构造方法, 用来确定tablename
        /// </summary>
        protected BaseDal()
        {
            //表名(以下代码判断有风险, 随后再更新, 这是老代码)
            var attributes = typeof(TModel).GetCustomAttributesData();
            var sortProp = typeof(TModel).GetProperty("Sort");
            if (sortProp != null)
            {
                Sorted = true;
            }

            foreach (var attribute in attributes)
            {
                if (attribute.ConstructorArguments.Count > 0)
                {
                    TableName = attribute.ConstructorArguments[0].Value.ToString();
                }
            }
        }

        /// <summary>
        /// 执行一条sql语句更新数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="wheres"></param>
        /// <returns></returns>
        public int Update(string values, string wheres)
        {
            var sql = "UPDATE " + TableName + " SET " + values + " WHERE " + wheres;
            return Conn.Execute(sql);
        }

        /// <summary>
        /// 获取分页数据, 复杂版本, 支持join
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="groupBy"></param>
        /// <param name="where">查询条件</param>
        /// <param name="param">参数</param>
        /// <param name="join">连接</param>
        /// <param name="orderBy">排序</param>
        /// <param name="select">选择什么</param>
        /// <param name="tableIsSubQuery"></param>
        /// <returns></returns>
        public IList<TP> GetByPager<TP>(BasePager pager, string where = "", object param = null, string select = "", string join = "", string orderBy = "", string groupBy = "", bool tableIsSubQuery = false)
        {
            return DbHelper.GetByPager<TP>(TableName, pager, where, param, select, join, orderBy, tableIsSubQuery, groupBy);
            //return DbHelper.GetByPager<TP>(tableName, pager, where, param, select, join, orderby, groupby, tableIsSubQuery);
        }

        /// <summary>
        /// 获取分页数据, 复杂版本, 支持join
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="groupBy"></param>
        /// <param name="pager"></param>
        /// <param name="where">查询条件</param>
        /// <param name="param">参数</param>
        /// <param name="join">连接</param>
        /// <param name="orderBy">排序</param>
        /// <param name="select">选择什么</param>
        /// <param name="tableIsSubQuery"></param>
        /// <returns></returns>
        public IList<TP> GetByPager<TP>(string tableName, BasePager pager, string where = "", object param = null, string select = "",
            string join = "", string orderBy = "", string groupBy = "", bool tableIsSubQuery = false)
        {
            return DbHelper.GetByPager<TP>(tableName, pager, where, param, select, join, orderBy, tableIsSubQuery, groupBy);
            //return DbHelper.GetByPager<TP>(tableName, pager, where, param, select, join, orderby, groupby, tableIsSubQuery);
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
        /// <param name="groupBy">按什么分组</param>
        /// <param name="tableIsSubQuery">table是一个子查询</param>
        /// <returns></returns>
        public IList<TModel> GetByPager(BasePager pager, string where = "", object param = null, string select = "", string join = "", string orderBy = "", string groupBy = "", bool tableIsSubQuery = false)
        {
            return GetByPager<TModel>(pager, where, param, select, join, orderBy, groupBy, tableIsSubQuery);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <returns></returns>
        public TModel GetFirst()
        {
            var sql = $"select * from {TableName} where DelFlag=0 order by Id desc limit 0,1";
            return Conn.QueryFirstOrDefault<TModel>(sql);
        }


        /// <summary>
        /// 更新排序值
        /// </summary>
        /// <returns></returns>
        public int UpdateSort(long id, long val)
        {
            return Conn.Execute($"update {TableName} set Sort={val} where Id={id}");
        }

        /// <summary>
        /// 获取最大的排序值
        /// </summary>
        /// <returns></returns>
        public long GetMaxSort()
        {
            try
            {
                return Conn.QuerySingleOrDefault<long>("select max(Sort) from " + TableName);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 按条件查数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="param"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public IList<TModel> GetListByCondition(string @where, string orderBy, object param = null, long top = 0)
        {
            //语句
            var sql = "select " + (top > 0 ? "top " + top : "") + " * from " + TableName + " as obj";
            if (!string.IsNullOrEmpty(@where))
            {
                sql += " where " + @where;
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " order by " + orderBy;
            }

            return Conn.Query<TModel>(sql, param).ToList();
        }

        /// <summary>
        /// 按条件获取一条
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public TModel GetSingleByCondition(string @where, string orderBy, object param = null)
        {
            //语句
            var sql = "select * from " + TableName + " as obj limit 0,1";
            if (!string.IsNullOrEmpty(@where))
            {
                sql += " where " + @where;
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " order by " + orderBy;
            }

            return Conn.QueryFirstOrDefault<TModel>(sql, param);
        }

        /// <summary>
        /// 统计数据总数
        /// </summary>
        /// <returns></returns>
        public long GetCount()
        {
            var sql = "select count(*) from " + TableName + " where DelFlag=0";
            return Conn.QuerySingle<long>(sql);
        }

        /// <summary>
        /// 统计数据总数, 带条件及参数
        /// </summary>
        /// <returns></returns>
        public long GetCount(string where, object para = null)
        {
            var sql = "select count(*) from " + TableName + " as obj where " + where;
            return Conn.QuerySingle<long>(sql, para);
        }

        /// <summary>
        /// 按条件获取数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public IList<TModel> GetByWhere(string @where, object para = null)
        {
            var sql = $"select * from {TableName} as obj where " + where;
            return Conn.Query<TModel>(sql, para).ToList();
        }

        /// <summary>
        /// BatDelete
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int BatDelete(IList<long> ids)
        {
            var sql = "update " + TableName + " set DelFlag=1 where ID in @ids";
            return Conn.Execute(sql, new { ids });
        }

        /// <summary>
        /// GetToManagerList
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public virtual IList<TModel> GetToManagerList(BasePager pager)
        {
            return GetByPager(pager, "DelFlag=0", orderBy: Sorted ? "Sort desc,CreateTime desc" : "CreateTime desc");
        }

        /// <summary>
        /// 获取整个表中所有数据
        /// </summary>
        /// <returns></returns>
        public IList<TModel> GetAll(bool delDelFlag = true)
        {
            var sql = "select * from " + TableName;
            if (delDelFlag)
                sql += " where DelFlag=0";
            if (Sorted)
            {
                sql += " order by Sort desc, CreateTime desc";
            }
            else
            {
                sql += " order by CreateTime desc";
            }

            return Conn.Query<TModel>(sql).ToList();
        }

        /// <summary>
        /// 按id获取某一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hasDelFlag">为false将忽略DelFlag参数</param>
        /// <returns></returns>
        public virtual TModel GetById(long id, bool hasDelFlag = true)
        {
            var sql = "select * from " + TableName + " where ID=" + id;
            if (hasDelFlag)
            {
                sql += " and DelFlag=0";
            }

            return Conn.QueryFirstOrDefault<TModel>(sql);
        }

        /// <summary>
        /// 按id获取某一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hasDelFlag"></param>
        /// <returns></returns>
        public virtual TModel GetById(string id, bool hasDelFlag = true)
        {
            var sql = "select * from " + TableName + " where StrId=@id";
            var param = new DynamicParameters();
            param.Add("id", id);
            return Conn.QueryFirstOrDefault<TModel>(sql, param);
        }

        /// <summary>
        /// 插入一个模型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual long? Insert(TModel obj)
        {
            return Conn.Insert(obj, GetCurrentTrans());
        }

        /// <summary>
        /// 按id删除一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ignoreVer"></param>
        /// <returns></returns>
        public int DeleteById(long id, bool ignoreVer = false)
        {
            var sql = "update " + TableName + " set DelFlag=1,UpdateTime=@now where ID=" + id;

            return Conn.Execute(sql, new { now = DateTime.Now });
        }

        /// <summary>
        /// 按id删除一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(long id)
        {
            var sql = "delete " + TableName + " where ID=" + id;
            return Conn.Execute(sql);
        }

        /// <summary>
        /// 按id还原一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RestoreById(long id)
        {
            var sql = "update " + TableName + " set DelFlag=0 where ID=" + id;
            return Conn.Execute(sql);
        }

        /// <summary>
        /// 按id还原一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="published"></param>
        /// <returns></returns>
        public int PublishedById(long id, int published)
        {
            var sql = "update " + TableName + " set Published=" + published + " where ID=" + id;
            return Conn.Execute(sql);
        }

        /// <summary>
        /// 更新一个模型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Update(TModel obj)
        {
            obj.UpdateTime = DateTime.Now;
            //检测到拥有快照功能 
            if (obj is IVer ver)
            {
                //当前删除
                var r1 = Conn.Execute($"update {TableName} set DelFlag=1 where Id={obj.Id}");
                if (r1 > 0)
                {
                    ver.Ver++;
                    var r2 = Insert(obj);
                    return r2 > 0 ? 1 : 0;
                }

                return 0;
            }

            return Conn.Update(obj, GetCurrentTrans()) ? 1 : 0;
        }

        /// <summary>
        /// 开启一个事务, 并保存在上下文中
        /// </summary>
        protected void BeginTransaction()
        {
            DbHelper.GetCurrentTransaction();
        }

        /// <summary>
        /// 获取当前上下文中的事务
        /// </summary>
        /// <returns></returns>
        protected IDbTransaction GetCurrentTrans()
        {
            return DbHelper.GetCurrentTransaction(false);
        }
    }
}