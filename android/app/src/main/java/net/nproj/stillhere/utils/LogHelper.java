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

import android.util.Log;

public class LogHelper {
    /**
     * 打印调试日志
     *
     * @param message 日志消息
     */
    public static void Debug(String message) {
        Log.d("CHAT_SYSTEM", message);
    }

    /**
     * 打印错误日志
     * * @param message 日志消息
     */
    public static void Error(String message) {
        Log.e("CHAT_SYSTEM", message);
    }
}
