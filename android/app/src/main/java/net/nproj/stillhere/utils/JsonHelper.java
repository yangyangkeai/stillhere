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

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonElement;
import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;
import java.util.List;

/**
 * JsonHelper 是一个用于 JSON 序列化和反序列化的工具类。
 * <p>
 * 该类封装了对 Gson 库的调用，提供了对象与 JSON 字符串之间的相互转换方法。
 * </p>
 */
public class JsonHelper {
    /**
     * Gson 实例，用于实际的序列化和反序列化操作。
     */
    private static final Gson GSON = new GsonBuilder().setDateFormat("yyyy-MM-dd HH:mm:ss").create();


    /**
     * 将对象序列化为 JSON 字符串。
     *
     * @param obj 需要序列化的对象
     * @return 对象对应的 JSON 字符串
     */
    public static String toJson(Object obj) {
        // 假设使用某个 JSON 库进行序列化
        return GSON.toJson(obj);
    }

    /**
     * 将 JSON 字符串反序列化为指定类型的对象。
     *
     * @param json  JSON 字符串
     * @param clazz 目标对象的类类型
     * @param <T>   目标对象的类型
     * @return 反序列化后的对象
     */
    public static <T> T fromJson(String json, Class<T> clazz) {
        if (json == null || json.isEmpty()) {
            return null;
        }
        // 假设使用某个 JSON 库进行反序列化
        return GSON.fromJson(json, clazz);
    }

    /**
     * 将 JsonElement 反序列化为指定类型的对象。
     *
     * @param json  JSON 元素
     * @param clazz 目标对象的类类型
     * @param <T>   目标对象的类型
     * @return 反序列化后的对象
     */
    public static <T> T fromJson(JsonElement json, Class<T> clazz) {
        // 假设使用某个 JSON 库进行反序列化
        return GSON.fromJson(json, clazz);
    }

    /**
     * 将JsonElement（数组）转换为指定类型的List
     *
     * @param element JsonElement 对象（必须是一个 JSON 数组）
     * @param clazz   元素类型的Class
     * @param <T>     元素类型
     * @return List<T> 对象，如果element不是数组或为空，则返回空列表
     */
    public static <T> List<T> fromJsonArray(JsonElement element, Class<T> clazz) {
        if (element == null || !element.isJsonArray()) {
            return java.util.Collections.emptyList();
        }

        Type listType = TypeToken.getParameterized(List.class, clazz).getType();
        return GSON.fromJson(element, listType);
    }
}
