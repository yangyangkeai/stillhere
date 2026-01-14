// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿using System;
using System.Reflection;
using Ninject;

namespace Nproj.StillHereApp.IDal
{
    public class DalFactory
    {

        /// <summary>
        /// Ninject 核心
        /// </summary>
        private static readonly IKernel _kernel;

        /// <summary>
        /// 构造方法
        /// </summary>
        static DalFactory()
        {
            _kernel = new StandardKernel();
            var idalAssemblyName = "Nproj.StillHereApp.IDal";
            var dalAssemblyName = "Nproj.StillHereApp.Dal";
            var idal = Assembly.Load(idalAssemblyName);
            var dal = Assembly.Load(dalAssemblyName);

            foreach (var type in idal.GetTypes())
            {
                //要是接口
                if (type.IsInterface)
                {

                    if (type.FullName.IndexOf("IDalBase", StringComparison.Ordinal) != -1)
                    {
                        continue;
                    }

                    var dalName = dalAssemblyName + type.FullName.Replace(idalAssemblyName, "").Replace(".IDal", ".") + "Dal";
                    var dalType = dal.GetType(dalName);
                    if (dalType != null)
                    {
                        _kernel.Bind(type).To(dalType).InSingletonScope();
                    }

                }

            }

        }

        /// <summary>
        /// 根据接口拿一个实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T : class
        {
            return _kernel.Get<T>();
        }

    }
}
