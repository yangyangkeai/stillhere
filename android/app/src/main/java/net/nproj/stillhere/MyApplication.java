/*
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

package net.nproj.stillhere;

import android.app.Application;
import android.content.Context;

import net.nproj.stillhere.utils.ActivityTracker;
import net.nproj.stillhere.utils.GlobalConstHelper;
import net.nproj.stillhere.utils.Helper;

/**
 * 自定义 Application 类，用于全局初始化和配置
 */
public class MyApplication extends Application {

    /**
     * 应用程序的全局上下文
     */
    private static Context appContext;

    /**
     * 应用程序创建时的入口方法
     */
    @Override
    public void onCreate() {
        super.onCreate();
        appContext = getApplicationContext();
        // 初始化 ActivityTracker
        ActivityTracker.getInstance().setup(this);
        //获取系统状态栏高度
        GlobalConstHelper.SYSTEM_STATUS_BAR_HEIGHT = getResources().getDimensionPixelSize(getResources().getIdentifier("status_bar_height", "dimen", "android"));
        //获取系统导航栏高度
        GlobalConstHelper.SYSTEM_NAVIGATION_BAR_HEIGHT = getResources().getDimensionPixelSize(getResources().getIdentifier("navigation_bar_height", "dimen", "android"));
        //读取安卓ID
        GlobalConstHelper.ANDROID_ID = android.provider.Settings.Secure.getString(getContentResolver(), android.provider.Settings.Secure.ANDROID_ID);
        //初始化SharedPreferences
        Helper.initPrefs(appContext);
        //还原出token
        GlobalConstHelper.USER_TOKEN = Helper.getStringFromPrefs(GlobalConstHelper.PREFS_KEY_USER_TOKEN);
    }

    /**
     * 获取应用程序的全局上下文
     * @return 应用程序的 Context 对象
     */
    public static Context getContext() {
        return appContext;
    }
}

