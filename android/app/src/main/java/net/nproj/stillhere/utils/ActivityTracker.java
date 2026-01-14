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

import android.app.Activity;
import android.app.Application;
import android.os.Bundle;

import java.lang.ref.WeakReference;

/**
 * Activity 跟踪器，监听应用中 Activity 的生命周期变化，
 * 并提供获取当前前台 Activity 的功能。
 */
public class ActivityTracker implements Application.ActivityLifecycleCallbacks {

    /**
     * 单例实例
     */
    private static ActivityTracker instance;

    /**
     * 当前前台 Activity 的弱引用，防止内存泄漏
     */
    private WeakReference<Activity> currentActivityRef;

    /**
     * 私有构造函数，防止外部实例化
     */
    private ActivityTracker() {
    }

    /**
     * 获取单例实例
     * @return ActivityTracker 实例
     */
    public static ActivityTracker getInstance() {
        if (instance == null) {
            instance = new ActivityTracker();
        }
        return instance;
    }

    /**
     * 设置并注册 Activity 生命周期回调
     * @param application 应用的 Application 对象
     */
    public void setup(Application application) {
        application.registerActivityLifecycleCallbacks(this);
    }

    /**
     * 获取当前前台 Activity
     * @return 当前前台 Activity 对象，若无则返回 null
     */
    public Activity getCurrentActivity() {
        return currentActivityRef != null ? currentActivityRef.get() : null;
    }

    /**
     * Activity 创建时回调
     * @param activity  被创建的 Activity
     * @param savedInstanceState 保存的状态
     */
    @Override
    public void onActivityCreated(Activity activity, Bundle savedInstanceState) {
        // 不做特殊处理
    }

    /**
     * Activity 启动时回调
     * @param activity 被启动的 Activity
     */
    @Override
    public void onActivityStarted(Activity activity) {
        // 不做特殊处理
    }

    /**
     * Activity 恢复到前台时回调
     * @param activity 被恢复的 Activity
     */
    @Override
    public void onActivityResumed(Activity activity) {
        // Activity 恢复到前台时，更新当前 Activity 引用
        currentActivityRef = new WeakReference<>(activity);
    }

    /**
     * Activity 暂停时回调
     * @param activity 被暂停的 Activity
     */
    @Override
    public void onActivityPaused(Activity activity) {
        // 如果暂停的 Activity 是当前持有的，则清空引用
        if (currentActivityRef != null && currentActivityRef.get() == activity) {
            currentActivityRef.clear();
        }
    }

    /**
     * Activity 停止时回调
     * @param activity 被停止的 Activity
     */
    @Override
    public void onActivityStopped(Activity activity) {
        // 不做特殊处理
    }

    /**
     * Activity 保存实例状态时回调
     * @param activity 被保存状态的 Activity
     * @param outState 保存的状态数据
     */
    @Override
    public void onActivitySaveInstanceState(Activity activity, Bundle outState) {
        // 不做特殊处理
    }

    /**
     * Activity 销毁时回调
     * @param activity 被销毁的 Activity
     */
    @Override
    public void onActivityDestroyed(Activity activity) {
        // 不做特殊处理
    }
}
