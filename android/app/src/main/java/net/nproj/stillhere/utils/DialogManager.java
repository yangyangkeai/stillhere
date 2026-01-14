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
import android.app.Dialog;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.TextView;

import net.nproj.stillhere.R;
import net.nproj.stillhere.model.http.BaseResponse;

/**
 * 对话框管理器，负责显示自定义弹窗
 */
public class DialogManager {
    /**
     * 单例实例
     */
    private static DialogManager instance;

    /**
     * 加载中对话框实例
     */
    private static Dialog loadingDialog;

    /**
     * 私有构造方法，防止外部实例化
     */
    private DialogManager() {
    }

    /**
     * 获取 DialogManager 单例实例
     *
     * @return DialogManager 实例
     */
    public static DialogManager getInstance() {
        if (instance == null) {
            instance = new DialogManager();
        }
        return instance;
    }

    /**
     * 按钮点击事件监听接口
     */
    public interface OnAlertDialogActionListener {
        /**
         * 确认按钮点击回调
         *
         * @return 如果返回true则关闭对话框，返回false则不关闭
         */
        boolean onConfirm();

        /**
         * 取消按钮点击回调（可选实现）
         */
        default void onCancel() {
        }

        /**
         * 关闭按钮点击回调
         */
        default void onClose() {

        }
    }

    /**
     * 弹窗配置类
     */
    public static class AlertDialogConfig {

        /**
         * 图标文本
         */
        private String iconText;

        /**
         * 标题
         */
        private String title;

        /**
         * 内容消息
         */
        private String message;

        /**
         * 确认按钮文本
         */
        private String okBtnText;

        /**
         * 取消按钮文本
         */
        private String cancelBtnText;

        /**
         * 按钮事件监听器
         */
        OnAlertDialogActionListener listener;

        /**
         * 构造方法（带全部参数）
         *
         * @param title         标题
         * @param message       内容
         * @param okBtnText     确认按钮文本
         * @param iconText      图标文本
         * @param cancelBtnText 取消按钮文本
         * @param listener      事件监听器
         */
        public AlertDialogConfig(String title, String message, String okBtnText, String iconText, String cancelBtnText, OnAlertDialogActionListener listener) {
            this.title = title;
            this.message = message;
            this.okBtnText = okBtnText;
            this.cancelBtnText = cancelBtnText;
            this.iconText = iconText;
            this.listener = listener;
        }

        /**
         * 构造方法（无图标和取消按钮）
         *
         * @param title     标题
         * @param message   内容
         * @param okBtnText 确认按钮文本
         * @param listener  事件监听器
         */
        public AlertDialogConfig(String title, String message, String okBtnText, OnAlertDialogActionListener listener) {
            this.title = title;
            this.message = message;
            this.okBtnText = okBtnText;
            this.listener = listener;
        }
    }

    /**
     * 隐藏加载中对话框
     */
    public void hideLoading() {
        if (loadingDialog != null && loadingDialog.isShowing()) {
            loadingDialog.dismiss();
            loadingDialog = null;
        }
    }

    /**
     * 显示加载中对话框
     *
     * @param text 加载文本
     */
    public void showLoading(String text) {
        if (text == null || text.trim().isEmpty()) {
            text = "处理中...";
        }
        final String finalText = text;
        Activity currentActivity = ActivityTracker.getInstance().getCurrentActivity();
        if (currentActivity == null || currentActivity.isFinishing() || currentActivity.isDestroyed()) {
            // 没有可用的 Activity，无法显示
            return;
        }
        if (loadingDialog != null && loadingDialog.isShowing()) {
            // 已经显示，关闭后重新显示
            loadingDialog.dismiss();
            loadingDialog = null;
        }
        currentActivity.runOnUiThread(new Runnable() {

            @Override
            public void run() {
                // 1. 创建 Dialog 实例
                loadingDialog = new Dialog(currentActivity);

                // 2. 移除标题栏和设置透明背景 (实现自定义UI的关键)
                loadingDialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
                loadingDialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

                // 3. 引入自定义布局
                View dialogView = LayoutInflater.from(currentActivity).inflate(net.nproj.stillhere.R.layout.dialog_loading, null);
                loadingDialog.setContentView(dialogView);
                TextView loadingText = dialogView.findViewById(R.id.loading);
                loadingText.setText(finalText);

                // 可选：设置点击外部不消失
                loadingDialog.setCanceledOnTouchOutside(false);

                // 5. 显示 Dialog
                loadingDialog.show();
            }
        });
    }

    /**
     * 显示一个Alert弹窗
     *
     * @param config 弹窗配置
     */
    public void showAlert(final AlertDialogConfig config) {

        if (config == null) {
            // 配置为空，无法显示
            return;
        }

        Activity currentActivity = ActivityTracker.getInstance().getCurrentActivity();
        if (currentActivity == null || currentActivity.isFinishing() || currentActivity.isDestroyed()) {
            // 没有可用的 Activity，无法显示
            return;
        }

        // 确保在主线程中执行 UI 操作
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // 1. 创建 Dialog 实例
                final Dialog dialog = new Dialog(currentActivity);

                // 2. 移除标题栏和设置透明背景 (实现自定义UI的关键)
                dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
                dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

                // 3. 引入自定义布局
                View dialogView = LayoutInflater.from(currentActivity).inflate(R.layout.dialog_alert, null);
                dialog.setContentView(dialogView);

                // 4. 绑定视图和设置点击事件
                Button okBtn = dialogView.findViewById(R.id.ok_btn);
                Button cancelBtn = dialogView.findViewById(R.id.cancel_btn);
                ImageButton closeBtn = dialogView.findViewById(R.id.close_btn);
                TextView title = dialogView.findViewById(R.id.title);
                TextView content = dialogView.findViewById(R.id.content);
                LinearLayout icon = dialogView.findViewById(R.id.icon);
                TextView iconText = dialogView.findViewById(R.id.icon_text);

                if (config.iconText != null) {
                    icon.setVisibility(View.VISIBLE);
                    iconText.setText(config.iconText);
                } else {
                    icon.setVisibility(View.GONE);
                }

                if (config.title != null) {
                    title.setText(config.title);
                }

                if (config.message != null) {
                    content.setText(config.message);
                }

                if (config.okBtnText != null) {
                    okBtn.setText(config.okBtnText);
                }

                if (cancelBtn != null && config.cancelBtnText != null) {
                    cancelBtn.setVisibility(View.VISIBLE);
                    cancelBtn.setText(config.cancelBtnText);
                } else if (cancelBtn != null) {
                    cancelBtn.setVisibility(View.GONE);
                }

                if (config.listener != null) {
                    okBtn.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View v) {
                            //dialog.dismiss();
                            if (config.listener != null) {
                                var r = config.listener.onConfirm();
                                if (r) {
                                    dialog.dismiss();
                                }
                            }
                        }
                    });

                    if (cancelBtn != null && config.cancelBtnText != null) {
                        cancelBtn.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                dialog.dismiss();
                                if (config.listener != null) {
                                    config.listener.onCancel();
                                }
                            }
                        });
                    }

                    closeBtn.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View v) {
                            dialog.dismiss();
                            if (config.listener != null) {
                                config.listener.onClose();
                            }
                        }
                    });

                }

                // 可选：设置点击外部不消失
                dialog.setCanceledOnTouchOutside(false);

                // 5. 显示 Dialog
                dialog.show();

                // 可选：调整 Dialog 宽度
                /*dialog.getWindow().setLayout(
                        ViewGroup.LayoutParams.WRAP_CONTENT,
                        ViewGroup.LayoutParams.WRAP_CONTENT
                );*/
            }
        });
    }
}
