// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

using Dapper;
using Nproj.StillHereApp.Common.Utils;

namespace TestProject;

public class DefaultTests
{
    [SetUp]
    public void Setup()
    {
#if DEBUG
        ConfigHelper.Init("appsettings.development.json");
        LogHelper.Init("nlog.config");
#else
        ConfigHelper.Init("appsettings.json");
        LogHelper.Init("nlog.config");
#endif
    }

    /// <summary>
    /// 测试数据库接入
    /// </summary>
    [Test]
    public void TestDb()
    {
        var conn = DbHelper.GetNewConnection();
        var id = conn.QueryFirstOrDefault<long>("SELECT Id from sys_user where Id=1");
        if (id > 0)
        {
            Assert.Pass();
            return;
        }

        Assert.Fail();
    }
}