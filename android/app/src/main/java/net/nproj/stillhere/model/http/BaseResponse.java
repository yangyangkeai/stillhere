/*
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

package net.nproj.stillhere.model.http;

import com.google.gson.JsonElement;

/**
 * 基础响应类，封装通用响应字段
 */
public class BaseResponse {
    /**
     * 响应代码
     */
    public int code;

    /**
     * 响应消息
     */
    public String msg;

    /**
     * 响应数据
     */
    public JsonElement data;
}
