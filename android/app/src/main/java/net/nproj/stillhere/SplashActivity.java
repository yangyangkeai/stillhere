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

import android.os.Bundle;

import net.nproj.stillhere.i.RequestCallback;
import net.nproj.stillhere.model.http.BaseResponse;
import net.nproj.stillhere.model.http.req.InitRequest;
import net.nproj.stillhere.model.http.resp.ContextUser;
import net.nproj.stillhere.utils.Api;
import net.nproj.stillhere.utils.GlobalConstHelper;
import net.nproj.stillhere.utils.HttpHelper;
import net.nproj.stillhere.utils.JsonHelper;
import net.nproj.stillhere.utils.UserHelper;

/**
 * 启动页 Activity。
 * 负责启动时检查应用更新、下载并安装 APK（如果有新版本），以及初始化应用并跳转到主界面或登录页。
 */
public class SplashActivity extends BaseActivity {

    /**
     * Activity 创建入口，检查是否有新版本，如果需要则提示下载；否则继续初始化应用
     *
     * @param savedInstanceState 启动时传入的保存状态
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        init();
    }

    /**
     * 初始化应用
     */
    private void init() {
        var req = new InitRequest();
        req.deviceId = GlobalConstHelper.ANDROID_ID;
        //初始化网络
        HttpHelper.startPost(new HttpHelper.RequestConfig() {
            {
                data = req;
                path = Api.Comm_Init;
                callback = new RequestCallback() {
                    @Override
                    public void onSuccess(BaseResponse data, String json) {
                        if (data.data != null) {
                            var contextUser = JsonHelper.fromJson(data.data, ContextUser.class);
                            UserHelper.SetUser(contextUser);
                        }
                        //去MainActivity
                        startActivity(new android.content.Intent(SplashActivity.this, MainActivity.class));
                        // 关闭当前启动页
                        finish();
                    }
                };
            }
        });
    }
}
