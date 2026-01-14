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
import android.content.Context;
import android.content.ContextWrapper;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.Looper;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import net.nproj.stillhere.R;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Date;

/**
 * 通用辅助类
 */
public class Helper {


    /**
     * 后台线程Handler
     */
    private static Handler networkHandler;

    /**
     * SharedPreferences名称
     */
    private static final String PREF_NAME = "app_prefs";

    /**
     * SharedPreferences实例
     */
    private static SharedPreferences prefs;

    /**
     * 静态构造
     */
    static {
        HandlerThread thread = new HandlerThread("backgroundHandler");
        thread.start();
        networkHandler = new Handler(thread.getLooper());
    }

    /**
     * 初始化SharedPreferences
     */
    public static void initPrefs(Context context) {
        if (prefs == null) {
            prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        }
    }

    /**
     * 保存字符串到SharedPreferences
     *
     * @param key   键
     * @param value 值
     */
    public static void putStringToPrefs(String key, String value) {
        prefs.edit().putString(key, value).apply();
    }

    /**
     * 读取字符串
     *
     * @param key 键
     */
    public static String getStringFromPrefs(String key) {
        return prefs.getString(key, "");
    }

    /**
     * 从Context中获取Activity
     *
     * @param context Context
     * @return Activity，如果无法获取则返回null
     */
    public static Activity getActivityFromContext(Context context) {
        if (context == null) return null;
        if (context instanceof Activity) {
            return (Activity) context;
        } else if (context instanceof ContextWrapper) {
            return getActivityFromContext(((ContextWrapper) context).getBaseContext());
        }
        return null;
    }

    /**
     * 在主线程运行指定的Runnable
     *
     * @param runnable    要运行的Runnable
     * @param delayMillis 延迟时间，单位毫秒
     */
    public static Handler runOnUiThreadDelayed(Runnable runnable, long delayMillis) {
        Handler handler = new Handler(Looper.getMainLooper());
        handler.postDelayed(runnable, delayMillis);
        return handler;
    }

    /**
     * 在后台网络线程运行指定的Runnable
     *
     * @param runnable 要运行的Runnable
     */
    public static void runOnNetworkDelayed(Runnable runnable, long delayMillis) {
        networkHandler.postDelayed(runnable, delayMillis);
    }

    /**
     * URL编码
     *
     * @param input 输入字符串
     * @return 编码后的字符串
     */
    public static String UrlEncode(String input) {
        try {
            return java.net.URLEncoder.encode(input, "UTF-8");
        } catch (Exception e) {
            return input;
        }
    }

    /**
     * 从Context中获取AppCompatActivity
     *
     * @param context Context
     * @return AppCompatActivity，如果无法获取则返回null
     */
    public static AppCompatActivity getAppCompatActivityFromContext(Context context) {
        if (context == null) return null;
        if (context instanceof AppCompatActivity) {
            return (AppCompatActivity) context;
        } else if (context instanceof ContextWrapper) {
            return getAppCompatActivityFromContext(((ContextWrapper) context).getBaseContext());
        }
        return null;
    }

    /**
     * 全局弹出Toast，默认显示时长3000ms
     *
     * @param context 任意Context
     * @param message 显示内容
     */
    public static void showToast(final Context context, final String message) {
        showToast(context, message, 3000);
    }

    /**
     * 全局弹出Toast
     *
     * @param context  任意Context
     * @param message  显示内容
     * @param duration 显示时长，单位ms 或 Toast.LENGTH_SHORT/Toast.LENGTH_LONG
     */
    public static void showToast(final Context context, final String message, final int duration) {
        if (context == null || message == null) return;
        Runnable show = new Runnable() {
            @Override
            public void run() {
                Context appContext = context.getApplicationContext();
                Toast toast;
                if (duration == Toast.LENGTH_SHORT || duration == Toast.LENGTH_LONG) {
                    toast = Toast.makeText(appContext, message, duration);
                } else {
                    toast = Toast.makeText(appContext, message, Toast.LENGTH_SHORT);
                    toast.setDuration(Toast.LENGTH_SHORT);
                }
                toast.show();
                // 如果自定义时长，手动延长显示
                if (duration != Toast.LENGTH_SHORT && duration != Toast.LENGTH_LONG) {
                    final Toast t = toast;
                    new Handler().postDelayed(new Runnable() {
                        @Override
                        public void run() {
                            t.cancel();
                        }
                    }, duration);
                }
            }
        };
        if (Looper.myLooper() == Looper.getMainLooper()) {
            show.run();
        } else {
            new Handler(Looper.getMainLooper()).post(show);
        }
    }

    /**
     * 全局弹出Toast，默认显示时长3000ms，无需传入Context
     *
     * @param message 显示内容
     */
    public static void showToast(final String message) {
        showToast(message, Toast.LENGTH_SHORT);
    }

    /**
     * 获取全局应用上下文
     *
     * @return 应用上下文
     */
    public static Context getAppContext() {
        return net.nproj.stillhere.MyApplication.getContext();
    }

    /**
     * 全局弹出Toast，无需传入Context
     *
     * @param message  显示内容
     * @param duration 显示时长，单位ms 或 Toast.LENGTH_SHORT/Toast.LENGTH_LONG
     */
    public static void showToast(final String message, final int duration) {
        Context appContext = getAppContext();
        if (appContext == null || message == null) return;
        runOnUiThreadDelayed(() -> {
            LayoutInflater inflater = LayoutInflater.from(appContext);
            View layout = inflater.inflate(net.nproj.stillhere.R.layout.toast_custom, null);
            TextView text = layout.findViewById(R.id.toast_text);
            text.setText(message);
            Toast toast;
            toast = Toast.makeText(appContext, message, duration);
            toast.setView(layout);
            toast.show();
        }, 0);
    }

    /**
     * 返回当前时间戳，单位毫秒 从1970年1月1日00:00:00起
     *
     * @return 时间戳
     */
    public static long getTimestampMs() {
        return System.currentTimeMillis();
    }

    /**
     * 计算字符串的MD5值
     *
     * @param input 输入字符串
     * @return MD5值
     */
    public static String md5(String input) {
        try {
            //获取MD5摘要器
            MessageDigest md = MessageDigest.getInstance("MD5");
            md.update(input.getBytes());
            byte[] digest = md.digest();
            // 转换为十六进制字符串
            StringBuilder sb = new StringBuilder();
            for (byte b : digest) {
                sb.append(String.format("%02x", b & 0xff));
            }
            return sb.toString();
        } catch (NoSuchAlgorithmException e) {
            // ignore
        }
        return "";
    }

    /**
     * 生成一个新的GUID
     *
     * @return GUID字符串
     */
    public static String getGuid() {
        return java.util.UUID.randomUUID().toString().replace("-", "").toLowerCase();
    }

    /**
     * 将文本复制到剪贴板
     *
     * @param message 要复制的文本
     */
    public static void copyText(String message) {
        Context context = getAppContext();
        if (context == null || message == null) return;
        android.content.ClipboardManager clipboard = (android.content.ClipboardManager) context.getSystemService(Context.CLIPBOARD_SERVICE);
        android.content.ClipData clip = android.content.ClipData.newPlainText("copied_text", message);
        if (clipboard != null) {
            clipboard.setPrimaryClip(clip);
            showToast("已复制到剪贴板");
        }
    }

    /**
     * 格式化日期
     *
     * @param date   日期
     * @param format 格式
     * @return 格式化后的字符串
     */
    public static String formatDate(Date date, String format) {
        java.text.SimpleDateFormat sdf = new java.text.SimpleDateFormat(format);
        return sdf.format(date);
    }

    /**
     * 将dp转换为px
     *
     * @param dp dp值
     * @return px值
     */
    public static int dpToPx(int dp) {
        float density = getAppContext().getResources().getDisplayMetrics().density;
        return Math.round(dp * density);
    }

    /**
     * 格式化日期
     * 如果是当天,显示 今天 小时:分钟
     * 否则显示 昨天 小时:分钟 或 前天 小时:分钟 或 月-日 小时:分钟
     *
     * @param date 日期对象
     * @return 格式化后的字符串
     */
    public static String formatDate(Date date) {

        if (date == null) {
            LogHelper.Error("formatDate: date is null");
            return "";
        }

        // 如果年份 <= 2000，返回空字符串（保持原逻辑）
        if (date.getYear() + 1900 <= 2000) {
            LogHelper.Error("formatDate: date year <= 2000 " + date.getYear());
            return "";
        }

        // 强制使用东八区
        //java.util.TimeZone tz = java.util.TimeZone.getTimeZone("Asia/Shanghai");

        java.util.Calendar cal = java.util.Calendar.getInstance();
        java.util.Calendar now = java.util.Calendar.getInstance();

        cal.setTime(date);

        java.util.TimeZone systemTz = java.util.TimeZone.getDefault();
        now.setTime(new Date());
        //要修正now， 计算systemTz与东八区的时差
        int offset = systemTz.getOffset(now.getTimeInMillis()) - java.util.TimeZone.getTimeZone("Asia/Shanghai").getOffset(now.getTimeInMillis());
        now.add(java.util.Calendar.MILLISECOND, -offset);

        // 计算“当天零点”用于天数差，避免跨年导致 dayDiff 为负数
        java.util.Calendar calStart = (java.util.Calendar) cal.clone();
        calStart.set(java.util.Calendar.HOUR_OF_DAY, 0);
        calStart.set(java.util.Calendar.MINUTE, 0);
        calStart.set(java.util.Calendar.SECOND, 0);
        calStart.set(java.util.Calendar.MILLISECOND, 0);

        java.util.Calendar nowStart = (java.util.Calendar) now.clone();
        nowStart.set(java.util.Calendar.HOUR_OF_DAY, 0);
        nowStart.set(java.util.Calendar.MINUTE, 0);
        nowStart.set(java.util.Calendar.SECOND, 0);
        nowStart.set(java.util.Calendar.MILLISECOND, 0);

        long diffMs = nowStart.getTimeInMillis() - calStart.getTimeInMillis();
        int dayDiff = (int) (diffMs / (24L * 60L * 60L * 1000L));

        String timePart = formatTwoDigits(cal.get(java.util.Calendar.HOUR_OF_DAY)) + ":" +
                formatTwoDigits(cal.get(java.util.Calendar.MINUTE));

        LogHelper.Debug("formatDate: dayDiff=" + dayDiff);

        if (dayDiff == 0) {
            return "今天 " + timePart;
        } else if (dayDiff == 1) {
            return "昨天 " + timePart;
        } else if (dayDiff == 2) {
            return "前天 " + timePart;
        } else {
            int yearNow = now.get(java.util.Calendar.YEAR);
            int yearDate = cal.get(java.util.Calendar.YEAR);

            if (yearNow == yearDate) {
                return formatTwoDigits(cal.get(java.util.Calendar.MONTH) + 1) + "-" +
                        formatTwoDigits(cal.get(java.util.Calendar.DAY_OF_MONTH)) + " " + timePart;
            } else {
                return yearDate + "-" +
                        formatTwoDigits(cal.get(java.util.Calendar.MONTH) + 1) + "-" +
                        formatTwoDigits(cal.get(java.util.Calendar.DAY_OF_MONTH)) + " " + timePart;
            }
        }

    }

    /**
     * 格式化两位数字
     *
     * @param number 数字
     * @return 格式化后的字符串
     */
    private static String formatTwoDigits(int number) {
        return number < 10 ? "0" + number : String.valueOf(number);
    }

    /**
     * 获取应用的当前版本号 (versionCode)
     *
     * @return 版本号整数，获取失败则返回 -1
     */
    public static int getVersionCode() {
        try {
            // 1. 获取 PackageInfo 对象
            PackageInfo pInfo = getAppContext().getPackageManager().getPackageInfo(getAppContext().getPackageName(), 0);

            // 2. 从 PackageInfo 中读取 versionCode
            // 注意：对于较新的 Android SDK (API 28/29+), getVersionCode() 已经被标记为 @Deprecated
            // 推荐使用 getLongVersionCode() 并转换为 int
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
                // 对于 API 28 (Android P) 及更高版本
                return (int) pInfo.getLongVersionCode();
            } else {
                // 对于旧版本
                return pInfo.versionCode;
            }
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
            return -1;
        }
    }

    /**
     * 显示软键盘
     *
     * @param editText 输入框
     */
    public static void showKeyboard(EditText editText) {
        editText.requestFocus();
        android.view.inputmethod.InputMethodManager imm = (android.view.inputmethod.InputMethodManager) getAppContext().getSystemService(Context.INPUT_METHOD_SERVICE);
        if (imm != null) {
            imm.showSoftInput(editText, android.view.inputmethod.InputMethodManager.SHOW_IMPLICIT);
        }
    }
}
