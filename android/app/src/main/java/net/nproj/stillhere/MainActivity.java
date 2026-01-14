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

import android.animation.AnimatorInflater;
import android.content.Context;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.view.animation.DecelerateInterpolator;
import android.view.animation.OvershootInterpolator;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.TextView;

import net.nproj.stillhere.i.ActionCallback;
import net.nproj.stillhere.i.RequestCallback;
import net.nproj.stillhere.model.http.BaseResponse;
import net.nproj.stillhere.model.http.req.ContentRequest;
import net.nproj.stillhere.model.http.resp.ContextUser;
import net.nproj.stillhere.utils.Api;
import net.nproj.stillhere.utils.Helper;
import net.nproj.stillhere.utils.HttpHelper;
import net.nproj.stillhere.utils.JsonHelper;
import net.nproj.stillhere.utils.LogHelper;
import net.nproj.stillhere.utils.UserHelper;

public class MainActivity extends BaseActivity {

    /**
     * 当前用户
     */
    private ContextUser currentUser;

    /**
     * 昵称编辑框
     */
    private EditText editTextName;

    /**
     * 联系邮箱编辑框
     */
    private  EditText editTextMail;

    /**
     * 重写触摸事件分发以实现点击 EditText 之外区域隐藏键盘和清除焦点
     * @param ev 触摸事件
     * @return 是否消费该事件
     */
    @Override
    public boolean dispatchTouchEvent(MotionEvent ev) {
        if (ev.getAction() == MotionEvent.ACTION_DOWN) {
            View v = getCurrentFocus();
            if (isShouldHideInput(v, ev)) {
                // 1. 隐藏键盘
                InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
                if (imm != null) {
                    assert v != null;
                    imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
                }
                // 2. 清除焦点
                assert v != null;
                v.clearFocus();
            }
        }
        return super.dispatchTouchEvent(ev);
    }

    /**
     * 判断点击位置是否在 EditText 之外
     */
    public boolean isShouldHideInput(View v, MotionEvent event) {
        if ((v instanceof EditText)) {
            int[] leftTop = {0, 0};
            // 获取输入框在屏幕上的位置
            v.getLocationInWindow(leftTop);
            int left = leftTop[0];
            int top = leftTop[1];
            int bottom = top + v.getHeight();
            int right = left + v.getWidth();

            // 如果点击坐标在 EditText 矩形区域外，则返回 true
            return !(event.getX() > left && event.getX() < right
                    && event.getY() > top && event.getY() < bottom);
        }
        return false;
    }

    /**
     * Activity 创建入口。
     * @param savedInstanceState 从系统恢复的上次保存状态；若无则为 null
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        var rootView = findViewById(R.id.root_element);
        this.setSize(rootView);

        if (UserHelper.GetUser() != null) {

            //获取当前用户
            currentUser = UserHelper.GetUser();

            //查出控件
            editTextName = findViewById(R.id.edit_text_name);
            TextView textViewName = findViewById(R.id.text_view_name);
            var editNameButton = findViewById(R.id.button_edit_name);
            editNameButton.setOnClickListener(v -> {
                //显示昵称编辑框
                editTextName.setText(currentUser.nickName);
                editTextName.setVisibility(View.VISIBLE);
                editTextName.requestFocus();
                textViewName.setVisibility(View.GONE);
                editNameButton.setVisibility(View.GONE);
            });

            editTextName.setOnFocusChangeListener((v, hasFocus) -> {
                if (!hasFocus) {
                    String newName = editTextName.getText().toString().trim();
                    if (newName.isEmpty()) {
                        Helper.showToast("未设置昵称！");
                        return;
                    }

                    if (newName.equals(currentUser.nickName)) {
                        editTextName.setVisibility(View.GONE);

                        textViewName.setText(newName);
                        textViewName.setVisibility(View.VISIBLE);

                        editNameButton.setVisibility(View.VISIBLE);
                        return;
                    }

                    updateNickName(newName, new ActionCallback() {
                        @Override
                        public void Execute() {
                            currentUser.nickName = newName;

                            editTextName.setVisibility(View.GONE);

                            textViewName.setText(newName);
                            textViewName.setVisibility(View.VISIBLE);

                            editNameButton.setVisibility(View.VISIBLE);
                        }
                    });
                }
            });

            editTextMail = findViewById(R.id.edit_text_mail);
            TextView textViewMail = findViewById(R.id.text_view_mail);
            var editMailButton = findViewById(R.id.button_edit_mail);
            editMailButton.setOnClickListener(v -> {
                //显示邮箱编辑框
                editTextMail.setText(currentUser.contactEmail);
                editTextMail.setVisibility(View.VISIBLE);
                editTextMail.requestFocus();
                textViewMail.setVisibility(View.GONE);
                editMailButton.setVisibility(View.GONE);
            });

            editTextMail.setOnFocusChangeListener((v, hasFocus) -> {
                if (!hasFocus) {
                    String newMail = editTextMail.getText().toString().trim();
                    if (newMail.isEmpty()) {
                        Helper.showToast("未设置联系邮箱！");
                        return;
                    }

                    if (newMail.equals(currentUser.contactEmail)) {
                        editTextMail.setVisibility(View.GONE);

                        textViewMail.setText(newMail);
                        textViewMail.setVisibility(View.VISIBLE);

                        editMailButton.setVisibility(View.VISIBLE);
                        return;
                    }

                    updateContactEmail(newMail, new ActionCallback() {
                        @Override
                        public void Execute() {
                            currentUser.contactEmail = newMail;

                            editTextMail.setVisibility(View.GONE);

                            textViewMail.setText(newMail);
                            textViewMail.setVisibility(View.VISIBLE);

                            editMailButton.setVisibility(View.VISIBLE);
                        }
                    });
                }
            });

            //判断如何显示昵称
            if (currentUser.nickName == null || currentUser.nickName.isEmpty()) {
                editTextName.setVisibility(View.VISIBLE);
            } else {
                textViewName.setText(currentUser.nickName);
                textViewName.setVisibility(View.VISIBLE);
                editNameButton.setVisibility(View.VISIBLE);
            }

            //判断如何显示邮箱
            if (currentUser.contactEmail == null || currentUser.contactEmail.isEmpty()) {
                editTextMail.setVisibility(View.VISIBLE);
            } else {
                textViewMail.setText(currentUser.contactEmail);
                textViewMail.setVisibility(View.VISIBLE);
                editMailButton.setVisibility(View.VISIBLE);
            }

            //判断是否显示最后打卡时间
            showLastCheckInTime();
        }

        //背景大的圈闪动
        var bgCircle1 = findViewById(R.id.bg_circle1);
        var anim = AnimatorInflater.loadAnimator(this, R.animator.blink);
        anim.setTarget(bgCircle1);
        anim.start();

        //打卡按钮
        var btn = findViewById(R.id.btn_check_in);
        btn.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        v.animate()
                                .scaleX(0.95f)
                                .scaleY(0.95f)
                                .setDuration(100)
                                .setInterpolator(new DecelerateInterpolator())
                                .start();
                        break;

                    case MotionEvent.ACTION_UP:
                    case MotionEvent.ACTION_CANCEL:
                        v.animate()
                                .scaleX(1f)
                                .scaleY(1f)
                                .setDuration(150)
                                .setInterpolator(new OvershootInterpolator())
                                .withEndAction(new Runnable() {
                                    @Override
                                    public void run() {
                                        if (event.getAction() == MotionEvent.ACTION_UP) {
                                            v.performClick();
                                        }
                                    }
                                })
                                .start();
                        break;
                }
                return true;
            }
        });
        btn.setOnClickListener(v -> {
            var user = UserHelper.GetUser();
            if (user == null) {
                Helper.showToast("用户未登录，无法打卡！");
                return;
            }
            if (currentUser.nickName == null || currentUser.nickName.isEmpty()) {
                Helper.showToast("请先设置昵称！");
                return;
            }
            if (currentUser.contactEmail == null || currentUser.contactEmail.isEmpty()) {
                Helper.showToast("请先设置联系邮箱！");
                return;
            }
            //执行打卡
            checkIn(new ActionCallback() {
                @Override
                public void Execute() {
                    Helper.showToast("打卡成功！");
                }
            });
        });
    }

    /**
     * 显示最后打卡时间
     */
    private void showLastCheckInTime() {
        if (currentUser.lastCheckInTime != null) {
            TextView textViewLastCheckIn = findViewById(R.id.text_view_last_checkin);
            var dateStr = Helper.formatDate(currentUser.lastCheckInTime);
            LogHelper.Debug("最后打卡时间字符串：" + dateStr);
            if (!dateStr.isEmpty()) {
                textViewLastCheckIn.setText("最后打卡：" + dateStr);
                textViewLastCheckIn.setVisibility(View.VISIBLE);
            }
        }
    }

    /**
     * 请求服务器打卡
     *
     * @param actionCallback 打卡成功回调
     */
    private void checkIn(ActionCallback actionCallback) {

        HttpHelper.startPost(new HttpHelper.RequestConfig() {
            {
                path = Api.User_CheckIn;
                callback = new RequestCallback() {
                    @Override
                    public void onSuccess(BaseResponse data, String json) {
                        if (data.data != null) {
                            var contextUser = JsonHelper.fromJson(data.data, ContextUser.class);
                            LogHelper.Debug("打卡响应JSON：" + data.data);
                            UserHelper.SetUser(contextUser);
                            currentUser = contextUser;

                        }
                        Helper.runOnUiThreadDelayed(() -> {
                            //更新最后打卡时间显示
                            showLastCheckInTime();
                            if (actionCallback != null) {
                                actionCallback.Execute();
                            }
                        }, 0);
                    }
                };
            }
        });
    }

    /**
     * 请求服务器更新联系邮箱
     */
    private void updateContactEmail(String email, ActionCallback actionCallback) {
        var req = new ContentRequest();
        req.content = email;

        HttpHelper.startPost(new HttpHelper.RequestConfig() {
            {
                data = req;
                path = Api.User_SetContactEmail;
                callback = new RequestCallback() {
                    @Override
                    public void onSuccess(BaseResponse data, String json) {
                        Helper.showToast("联系邮箱已更新！");
                        Helper.runOnUiThreadDelayed(() -> {
                            if (actionCallback != null) {
                                actionCallback.Execute();
                            }
                        }, 0);
                    }
                };
            }
        });
    }

    /**
     * 请求服务器更新昵称
     */
    private void updateNickName(String name, ActionCallback actionCallback) {
        var req = new ContentRequest();
        req.content = name;

        HttpHelper.startPost(new HttpHelper.RequestConfig() {
            {
                data = req;
                path = Api.User_SetNickName;
                callback = new RequestCallback() {
                    @Override
                    public void onSuccess(BaseResponse data, String json) {
                        Helper.showToast("昵称已更新！");
                        Helper.runOnUiThreadDelayed(() -> {
                            if (actionCallback != null) {
                                actionCallback.Execute();
                            }
                        }, 0);
                    }
                };
            }
        });
    }
}