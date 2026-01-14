// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Nproj.StillHereApp.Common.Filter;
using Nproj.StillHereApp.Common.Middleware;
using Nproj.StillHereApp.Common.Middleware.MyWeb;
using Nproj.StillHereApp.Common.Utils;

namespace Nproj.StillHereApp.WebApp
{
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
#if DEBUG
            ConfigHelper.Init("appsettings.development.json");
            LogHelper.Init("nlog.config");
#else
            ConfigHelper.Init("appsettings.json");
            LogHelper.Init("nlog.config");
#endif
            //线程池
            ThreadPool.SetMinThreads(int.Parse(ConfigHelper.Get(ConfigHelper.KEY_MinWorkerThreads)), int.Parse(ConfigHelper.Get(ConfigHelper.KEY_MinCompletionPortThreads)));

            var builder = WebApplication.CreateBuilder(args);
#if !DEBUG
            builder.Logging.SetMinimumLevel(LogLevel.Error);
#endif
            //从环境变量中读取START_PORT
            var startPort = Environment.GetEnvironmentVariable("START_PORT");
            var url = ConfigHelper.Get(ConfigHelper.UseUrl);
            if (!string.IsNullOrEmpty(startPort))
            {
                url = url.Replace($":{startPort}", "");
                Console.WriteLine($"use url {url} start process");
            }
#if DEBUG
            //配置一下WebHost
            builder.WebHost.UseUrls(url).ConfigureKestrel(serverOptions => { serverOptions.ListenAnyIP(443, listenOptions => { listenOptions.UseHttps("ca.pfx", "123456"); }); });
#else
            //配置一下WebHost
            builder.WebHost.UseUrls(url);
#endif
#if DEBUG
            builder.Services.AddCors();
#endif
            builder.Services.AddControllers(options =>
            {
                options.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>();
                options.Filters.Add<ValidateModelAttribute>();
            }).AddNewtonsoftJson();
            builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<ValidateModelAttribute>();

            //build app
            var app = builder.Build();

            //一堆配置
            HttpContextHelper.Env = app.Environment;
            ServiceLocator.Instance = app.Services;

            //默认文件
            app.UseDefaultFiles();
#if !DEBUG
            app.UseMiddleware<MyStaticFileMiddleware>(); //自定义的, 主要是返回静态压缩文件
#endif
            var staticFileCacheDuration = ConfigHelper.Get(ConfigHelper.StaticFileCacheDuration);
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".apk"] = "application/vnd.android.package-archive";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx => { ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={staticFileCacheDuration}"); }
            });
#if DEBUG
            app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
#endif

            app.UseMiddleware<MyWeb>();
            app.UseMiddleware<EnableBufferingAndSaveToContext>();

            app.UseRouting();
            app.MapControllers();

            //跑起
            app.Run();
        }
    }
}