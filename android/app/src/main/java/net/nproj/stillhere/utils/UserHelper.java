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

public class UserHelper {

    /**
     * 获取当前用户编号
     *
     * @return 用户编号
     */
    public static String GetNumber() {
        if (GlobalConstHelper.CONTEXT_USER != null) {
            return GlobalConstHelper.CONTEXT_USER.number;
        }
        return "";
    }

    /**
     * 将用户信息设置到全局变量中
     *
     * @param contextUser 用户信息对象
     */
    public static void SetUser(ContextUser contextUser) {
        if (contextUser == null) {
            return;
        }
        GlobalConstHelper.USER_TOKEN = contextUser.token;
        GlobalConstHelper.CONTEXT_USER = contextUser;
        //将token存入本地存储
        Helper.putStringToPrefs(GlobalConstHelper.PREFS_KEY_USER_TOKEN, contextUser.token);
    }

    /**
     * 清除全局变量中的用户信息
     */
    public static void ClearUser() {
        GlobalConstHelper.USER_TOKEN = "";
        GlobalConstHelper.CONTEXT_USER = null;
    }

    /**
     * 当前用户
     *
     * @return 用户信息对象
     */
    public static ContextUser GetUser() {
        return GlobalConstHelper.CONTEXT_USER;
    }

    /**
     * 设置昵称
     *
     * @param nickname 昵称
     */
    public static void SetNickname(String nickname) {
        if (GlobalConstHelper.CONTEXT_USER != null) {
            GlobalConstHelper.CONTEXT_USER.nickName = nickname;
        }
    }

    /**
     * 用户登出
     */
    public static void Logout() {
        ClearUser();
        //清除本地存储的token
        Helper.putStringToPrefs(GlobalConstHelper.PREFS_KEY_USER_TOKEN, "");
    }
}
