// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System.Collections.Generic;
using Nproj.StillHereApp.Model.Db;
using Nproj.StillHereApp.Model.Pager;

namespace Nproj.StillHereApp.IDal
{
    /// <summary>
    /// dal接口基类
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IDalBase<TModel> where TModel : DbBaseModel
    {
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int BatDelete(IList<long> ids);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        IList<TModel> GetToManagerList(BasePager pager);

        /// <summary>
        /// 获取整个表中所有数据
        /// </summary>
        /// <returns></returns>
        IList<TModel> GetAll(bool flag = true);

        /// <summary>
        /// 获取第一条记录
        /// </summary>
        /// <returns></returns>
        TModel GetFirst();

        /// <summary>
        /// 更新排序值
        /// </summary>
        /// <returns></returns>
        int UpdateSort(long id, long val);

        /// <summary>
        /// 获取最大的排序值
        /// </summary>
        /// <returns></returns>
        long GetMaxSort();

        /// <summary>
        /// 按条件获取数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="param"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        IList<TModel> GetListByCondition(string where, string orderBy, object param = null, long top = 0);

        /// <summary>
        /// 按条件获取一条
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        TModel GetSingleByCondition(string where, string orderBy, object param = null);

        /// <summary>
        /// 统计数据总数
        /// </summary>
        /// <returns></returns>
        long GetCount();

        /// <summary>
        /// 统计数据总数, 带条件及参数
        /// </summary>
        /// <returns></returns>
        long GetCount(string where, object para = null);

        /// <summary>
        /// 按条件获取数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        IList<TModel> GetByWhere(string where, object para = null);

        /// <summary>
        /// 按id获取某一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hasDelFlag">为false将忽略DelFlag参数</param>
        TModel GetById(long id, bool hasDelFlag = true);

        /// <summary>
        /// 按id获取某一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hasDelFlag">为false将忽略DelFlag参数</param>
        /// <returns></returns>
        TModel GetById(string id, bool hasDelFlag = true);

        /// <summary>
        /// 插入一个模型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        long? Insert(TModel obj);

        /// <summary>
        /// 按id删除一条数据(DelFlag=1)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ignoreVer"></param>
        /// <returns></returns>
        int DeleteById(long id, bool ignoreVer = false);

        /// <summary>
        /// 按id删除一条数据(物理删除)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(long id);

        /// <summary>
        /// 按id还原一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int RestoreById(long id);

        /// <summary>
        /// 按id还原一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int PublishedById(long id, int published);

        /// <summary>
        /// 更新一个模型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int Update(TModel obj);

        /// <summary>
        /// 执行一条sql语句更新数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="wheres"></param>
        /// <returns></returns>
        int Update(string values, string wheres);

        /// <summary>
        /// 获取分页数据, 复杂版本, 支持join
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="where">查询条件</param>
        /// <param name="param">参数</param>
        /// <param name="join">连接</param>
        /// <param name="orderBy">排序</param>
        /// <param name="select">选择什么</param>
        /// <param name="tableIsSubQuery"></param>
        /// <returns></returns>
        IList<TP> GetByPager<TP>(BasePager pager, string where = "", object param = null, string select = "", string join = "", string orderBy = "", string groupBy = "", bool tableIsSubQuery = false);

        /// <summary>
        /// 获取分页数据, 复杂版本, 支持join
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="where">查询条件</param>
        /// <param name="param">参数</param>
        /// <param name="join">连接</param>
        /// <param name="orderBy">排序</param>
        /// <param name="select">选择什么</param>
        /// <param name="tableIsSubQuery"></param>
        /// <returns></returns>
        IList<TP> GetByPager<TP>(string tableName, BasePager pager, string where = "", object param = null, string select = "", string join = "", string orderBy = "", string groupBy = "", bool tableIsSubQuery = false);

        /// <summary>
        /// 获取分页数据, 复杂版本, 支持join
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="where">查询条件</param>
        /// <param name="param">参数</param>
        /// <param name="join">连接</param>
        /// <param name="orderBy">排序</param>
        /// <param name="select">选择什么</param>
        /// <param name="tableIsSubQuery"></param>
        /// <returns></returns>
        IList<TModel> GetByPager(BasePager pager, string where = "", object param = null, string select = "", string join = "", string orderBy = "", string groupBy = "", bool tableIsSubQuery = false);
    }
}