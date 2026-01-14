/*
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

package net.nproj.stillhere.i;

import net.nproj.stillhere.model.http.BaseResponse;
import net.nproj.stillhere.utils.Helper;
import net.nproj.stillhere.utils.LogHelper;

public interface RequestCallback {
    /**
     * 请求成功回调
     *
     * @param data 解析后的响应数据
     * @param json 原始响应内容
     */
    default void onSuccess(net.nproj.stillhere.model.http.BaseResponse data, String json) {
    }

    /**
     * 当请求频繁时调用，可用于拦截请求
     */
    default void onIntercept() {
    }

    /**
     * 发生错误时调用
     */
    default void onError() {
    }

    /**
     * 请求失败回调
     * 如果已经对错误进行了处理，返回true
     *
     * @param data 解析后的响应数据
     * @return 是否已经处理错误
     */
    default boolean onFailure(BaseResponse data) {
        switch (data.code) {
            case 10500: //服务器异常
                Helper.showToast(data.msg);
                LogHelper.Error(data.msg);
                return true;
        }
        return false;
    }
}
