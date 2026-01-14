/*
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

package net.nproj.stillhere.utils;


import net.nproj.stillhere.model.http.resp.ContextUser;

/**
 * 常量帮助类
 */
public class GlobalConstHelper {

    /**
     * 当前登录用户
     */
    public static ContextUser CONTEXT_USER = null;

    /**
     * HTTP 请求是否已经就位
     */
    public static boolean HTTP_READY = false;

    /**
     * 状态栏高度
     */
    public static int SYSTEM_STATUS_BAR_HEIGHT = 0;

    /**
     * 导航栏高度
     */
    public static int SYSTEM_NAVIGATION_BAR_HEIGHT = 0;

    /**
     * 安卓ID
     */
    public static String ANDROID_ID = "";

    /**
     * 签名密钥 由后台动态生成分配
     */
    public static String SIGN_KEY = "";

    /**
     * 用户Token 用户登录后分配
     */
    public static String USER_TOKEN = "";

    /**
     * 请求密文 由后台动态生成分配
     */
    public static String REQUEST_CIPHERTEXT = "";

    /**
     * SharedPreferences键 - 用户Token
     */
    public static String PREFS_KEY_USER_TOKEN = "prefs_key_user_token";
}
