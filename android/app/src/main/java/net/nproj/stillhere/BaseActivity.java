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

import android.view.View;
import android.widget.FrameLayout;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowCompat;
import androidx.core.view.WindowInsetsCompat;

import net.nproj.stillhere.utils.GlobalConstHelper;

/**
 * Activity基类，提供通用功能
 */
public class BaseActivity extends AppCompatActivity {

    /**
     * 设置界面大小，使内容扩展到系统栏下面，并处理安全区的 padding。
     *
     * @param rootView 根布局视图
     */
    protected void setSize(View rootView) {
        var window = getWindow();
        // 1️⃣ 让内容扩展到系统栏下面（状态栏 + 导航栏）
        WindowCompat.setDecorFitsSystemWindows(window, false);
        // 3️⃣ 监听系统安全区（状态栏 + 导航栏）并设置padding
        ViewCompat.setOnApplyWindowInsetsListener(rootView, (v, insets) -> {
            // 获取系统的所有缩进（包括状态栏、导航栏、键盘）
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            // 只应用状态栏和导航栏的缩进，完全不处理 IME (键盘)
            // 这样键盘弹起时，布局就不会产生任何移动
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return WindowInsetsCompat.CONSUMED;
        });

    }
}
