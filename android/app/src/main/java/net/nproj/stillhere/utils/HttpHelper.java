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

import net.nproj.stillhere.BuildConfig;
import net.nproj.stillhere.i.GetCallback;
import net.nproj.stillhere.i.RequestCallback;
import net.nproj.stillhere.model.http.BaseRequest;
import net.nproj.stillhere.model.http.BaseResponse;

import java.io.IOException;
import java.lang.reflect.Field;
import java.security.SecureRandom;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.Map;
import java.util.Objects;
import java.util.TreeMap;
import java.util.concurrent.TimeUnit;

import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSocketFactory;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import okhttp3.Interceptor;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

/**
 * 不安全的 OkHttpClient 工具类。
 * 用于创建信任所有证书的 OkHttpClient 实例，适合开发环境使用。
 */
class UnsafeOkHttpClient {
    /**
     * 获取信任所有证书的 OkHttpClient 实例。
     *
     * @return OkHttpClient 对象
     */
    public static OkHttpClient getUnsafeOkHttpClient(boolean addInterceptor) {
        try {
            // 创建一个信任所有证书的 TrustManager
            final TrustManager[] trustAllCerts = new TrustManager[]{
                    new X509TrustManager() {
                        @Override
                        public void checkClientTrusted(X509Certificate[] chain, String authType) throws CertificateException {
                        }

                        @Override
                        public void checkServerTrusted(X509Certificate[] chain, String authType) throws CertificateException {
                        }

                        @Override
                        public X509Certificate[] getAcceptedIssuers() {
                            return new X509Certificate[]{};
                        }
                    }
            };

            // 初始化 SSL 上下文
            final SSLContext sslContext = SSLContext.getInstance("TLS");
            sslContext.init(null, trustAllCerts, new SecureRandom());

            // 创建忽略验证的 socket 工厂
            final SSLSocketFactory sslSocketFactory = sslContext.getSocketFactory();

            OkHttpClient.Builder builder = new OkHttpClient.Builder();
            builder.sslSocketFactory(sslSocketFactory, (X509TrustManager) trustAllCerts[0]);
            builder.hostnameVerifier((hostname, session) -> true);

            var aaa = builder
                    .pingInterval(30, TimeUnit.SECONDS) // 自动发 WebSocket ping 帧
                    .connectTimeout(5, TimeUnit.SECONDS) // 连接超时
                    .readTimeout(5, TimeUnit.SECONDS)    // 读取超时
                    .writeTimeout(5, TimeUnit.SECONDS);   // 写入超时

            if (addInterceptor) {
                aaa.addInterceptor(new HttpHelper.GlobalInterceptor());
            }

            return aaa.build();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}

/**
 * 网络请求工具类。
 * 封装了全局拦截器、请求配置、POST请求等功能。
 */
public class HttpHelper {

    /**
     * 全局拦截器，添加公共请求头并处理公共响应。
     */
    static class GlobalInterceptor implements Interceptor {
        /**
         * 拦截并处理请求，添加公共头部信息。
         *
         * @param chain 请求链
         * @return 响应对象
         * @throws IOException IO异常
         */
        @Override
        public Response intercept(Chain chain) throws IOException {
            Request original = chain.request();
            Request.Builder builder = original.newBuilder();
            //添加公共请求头
            builder.header("Android-User-Agent", Helper.UrlEncode(AesHelper.GetAndroidId()));
            var uuid = Helper.getGuid();
            var timestamp = Helper.getTimestampMs();
            //决定全局签名
            var sign = Helper.md5(GlobalConstHelper.SIGN_KEY + "---" + timestamp + "---" + uuid);
            //决定用户
            var token = GlobalConstHelper.USER_TOKEN;

            builder.header("Sign", sign);
            builder.header("Token", token != null ? token : "");
            builder.header("Timestamp", String.valueOf(timestamp));
            builder.header("Random", uuid);
            builder.header("Ciphertext", Helper.UrlEncode(GlobalConstHelper.REQUEST_CIPHERTEXT));
            //----------------请求分界线----------------
            var response = chain.proceed(builder.build());
            return response;
        }
    }

    /**
     * 请求配置类。
     * 用于封装请求路径、数据、回调、重试次数等参数。
     */
    public static class RequestConfig {
        /**
         * 接口路径。
         */
        public String path;
        /**
         * 请求数据。
         */
        public BaseRequest data = new BaseRequest();
        /**
         * 请求回调。
         */
        public RequestCallback callback;
        /**
         * 重试次数。
         */
        public int retryCount = 0;
        /**
         * 加载提示文本。
         */
        public String loadingText;
        /**
         * 是否自动显示加载框。
         */
        public boolean autoShowLoading = false;
    }

    /**
     * 加载框处理器。
     */
    private static android.os.Handler loadingHandler;

    /**
     * OkHttpClient 实例。
     */
    private static final OkHttpClient client;

    /**
     * GET 请求的 OkHttpClient 实例。
     */
    private static final OkHttpClient getClient;

    /**
     * JSON内容类型。
     */
    private static final MediaType CONTENT_TYPE_JSON = MediaType.get("application/json; charset=utf-8");
    /**
     * 基础URL。
     */
    private static final String baseUrl = BuildConfig.API_BASE_URL;
    /**
     * 请求URL集合，用于限制短时间内重复请求。
     */
    private static final java.util.Set<String> urlSet = java.util.concurrent.ConcurrentHashMap.newKeySet();

    static {
        /*client = new OkHttpClient.Builder()
                .addInterceptor(new GlobalInterceptor())
                .connectTimeout(10, TimeUnit.SECONDS) // 连接超时
                .readTimeout(10, TimeUnit.SECONDS)    // 读取超时
                .writeTimeout(10, TimeUnit.SECONDS)   // 写入超时
                .build();*/
        //开发使用不安全的OkHttpClient
        client = UnsafeOkHttpClient.getUnsafeOkHttpClient(true);
        getClient = UnsafeOkHttpClient.getUnsafeOkHttpClient(false);
    }

    /**
     * 获取 OkHttpClient 实例。
     *
     * @return OkHttpClient 对象
     */
    public static OkHttpClient getClient() {
        return client;
    }

    /**
     * 获取完整的请求URL。
     *
     * @param apiPath 接口路径
     * @return 完整URL字符串
     */
    private static String getUrl(String apiPath) {
        //需要对path进行处理，如果不以/开头，则添加/
        if (!apiPath.startsWith("/")) {
            apiPath = "/" + apiPath;
        }
        return (baseUrl + apiPath).toLowerCase();
    }

    /**
     * 将URL添加到集合，并在延迟后移除。
     *
     * @param url 请求URL
     */
    private static void addUrlToSet(String url) {
        urlSet.add(url);
        //在700ms后将url从set中移除
        Helper.runOnNetworkDelayed(() -> {
            urlSet.remove(url);
        }, 700);
    }

    // language: java
    private static final java.util.Set<Class<?>> WRAPPERS = java.util.Set.of(
            Boolean.class, Byte.class, Character.class, Short.class,
            Integer.class, Long.class, Float.class, Double.class, Void.class
    );

    /**
     * 判断类型是否为基本类型、包装类型或字符串。
     *
     * @param cls 类型
     * @return 是否为基本类型、包装类型或字符串
     */
    private static boolean isPrimitiveOrWrapperOrString(Class<?> cls) {
        return cls.isPrimitive() || WRAPPERS.contains(cls) || cls == String.class;
    }

    /**
     * 启动POST请求（异步）。
     *
     * @param config 请求配置
     */
    public static void startPost(RequestConfig config) {
        /*new Thread(() -> {
            sendPost(config);
        }).start();*/
        Helper.runOnNetworkDelayed(() -> {
            sendPost(config);
        }, 0);
    }

    /**
     * 发送GET请求
     *
     * @param url      请求URL
     * @param callback 回调接口
     */
    public static void startGet(String url, GetCallback callback) {
        Helper.runOnNetworkDelayed(() -> {
            sendGet(url, callback);
        }, 0);
    }

    /**
     * 发送GET请求（同步）。
     *
     * @param url      请求URL
     * @param callback 回调接口
     */
    private static void sendGet(String url, GetCallback callback) {
        Request request = new Request.Builder()
                .url(url)
                .build();
        try (var response = getClient.newCall(request).execute()) {
            if (response.code() == 200) {
                var bodyBytes = response.body().bytes();
                if (callback != null) {
                    callback.onSuccess(bodyBytes);
                }
            } else {
                if (callback != null) {
                    callback.onFailure(response.code());
                }
            }
        } catch (IOException e) {
            if (callback != null) {
                callback.onFailure(-1);
            }
        }
    }

    /**
     * 发送POST请求（同步）。
     *
     * @param config 请求配置
     */
    public static void sendPost(RequestConfig config) {
        if (config == null || config.path == null) {
            return;
        }
        String url = getUrl(config.path);
        if (config.retryCount >= 15) {
            Helper.showToast("网络重试达到上限");
            return;
        }
        if (!GlobalConstHelper.HTTP_READY && !url.contains(Api.Comm_Init.toLowerCase())) {
            //在400ms后将retryCount加1重新请求
            Helper.runOnNetworkDelayed(() -> {
                config.retryCount++;
                sendPost(config);
            }, 400);
            return;
        }
        //用来限制短时间内同一接口访问次数
        if (urlSet.contains(url) && !url.contains(Api.Comm_Init.toLowerCase())) {
            if (config.callback != null) {
                config.callback.onIntercept();
            }
            return;
        }
        addUrlToSet(url);

        if (config.loadingText != null && !config.loadingText.trim().isEmpty()) {
            if (loadingHandler != null) {
                loadingHandler.removeCallbacksAndMessages(null);
                DialogManager.getInstance().hideLoading();
                loadingHandler = null;
            }
            DialogManager.getInstance().showLoading(config.loadingText.trim());
        } else {
            if (config.autoShowLoading) {
                if (loadingHandler != null) {
                    loadingHandler.removeCallbacksAndMessages(null);
                    DialogManager.getInstance().hideLoading();
                    loadingHandler = null;
                }
                loadingHandler = Helper.runOnUiThreadDelayed(() -> {
                    DialogManager.getInstance().showLoading(null);
                }, 700);
            }
        }
        if (config.data == null) {
            config.data = new BaseRequest();
        }
        config.data._mt = Helper.getTimestampMs();

        //排序参数
        var array = new TreeMap<String, Object>();
        Class<?> clazz = config.data.getClass();
        Field[] fields = clazz.getFields();
        for (Field field : fields) {
            field.setAccessible(true); // 允许访问private字段
            Object value = null;
            try {
                value = field.get(config.data);
            } catch (IllegalAccessException e) {
                // ignore
            }
            array.put(field.getName(), value);
        }
        //拼接
        StringBuilder str = new StringBuilder();
        for (Map.Entry<String, Object> entry : array.entrySet()) {
            if (Objects.equals(entry.getKey(), "signature") || entry.getKey().contains("Time") || entry.getKey().contains("Date")) {
                continue;
            }
            String val = "";
            Object v = entry.getValue();
            if (v == null) {
                val = "";
            } else if (isPrimitiveOrWrapperOrString(v.getClass())) {
                val = v.toString();
            } else if (v.getClass().isArray() || v instanceof java.util.Collection || v instanceof Map) {
                val = JsonHelper.toJson(v);
            } else {
                val = entry.getValue().toString();
            }
            if (val.length() <= 50) {
                str.append(entry.getKey()).append(val);
            }
        }
        //str = str.toLowerCase()
        //lper.Debug("Sign String: " + str.toString());
        //加入签名
        config.data.signature = Helper.md5(GlobalConstHelper.SIGN_KEY + "---" + str);
        //LogHelper.Debug("Signature: " + config.data.signature);

        //检测requestUrl中是否存在queryString
        if (config.retryCount > 0) {
            if (url.contains("?")) {
                url = url + "&_n=" + config.retryCount;
            } else {
                url = url + "?_n=" + config.retryCount;
            }
        }
        var json = JsonHelper.toJson(config.data);
        RequestBody body = RequestBody.create(json, CONTENT_TYPE_JSON);
        Request request = new Request.Builder()
                .url(url)
                .post(body)
                .build();
        //client.newCall(request).enqueue(config.callback);
        try (var response = client.newCall(request).execute()) {
            if (response.code() == 200) {
                var bodyString = response.body().string();
                var obj = JsonHelper.fromJson(bodyString, BaseResponse.class);
                if (loadingHandler != null) {
                    loadingHandler.removeCallbacksAndMessages(null);
                    loadingHandler = null;
                }
                DialogManager.getInstance().hideLoading();
                switch (obj.code) {
                    case -1: //未授权，Token无效
                        //key
                        //ciphertext
                        var data = obj.data.getAsJsonObject();
                        GlobalConstHelper.SIGN_KEY = data.get("key").getAsString();
                        GlobalConstHelper.REQUEST_CIPHERTEXT = data.get("ciphertext").getAsString();
                        GlobalConstHelper.HTTP_READY = true;
                        config.retryCount++;
                        //递归重试
                        sendPost(config);
                        break;
                    case 10010: //需要重新登录
                        break;
                    case 10005: //禁止访问，签名错误
                        GlobalConstHelper.SIGN_KEY = "";
                        GlobalConstHelper.REQUEST_CIPHERTEXT = "";
                        config.retryCount++;
                        //递归重试
                        sendPost(config);
                        break;
                    case 0: //成功
                        if (config.callback != null) {
                            config.callback.onSuccess(obj, bodyString);
                        }
                        break;
                    default:
                        if (config.callback != null) {
                            var r = config.callback.onFailure(obj);
                            if (!r) {
                                Helper.showToast(obj.msg);
                            }
                        } else {
                            if (obj.msg != null && !obj.msg.isEmpty()) {
                                Helper.showToast(obj.msg);
                            }
                        }
                        break;
                }
            } else {
                //toast error
                Helper.showToast("网络请求错误，状态码：" + response.code());
                //需要把loading框隐藏
                if (loadingHandler != null) {
                    loadingHandler.removeCallbacksAndMessages(null);
                    DialogManager.getInstance().hideLoading();
                    loadingHandler = null;
                }
                if (config.callback != null) {
                    config.callback.onError();
                }
            }
        } catch (IOException e) {
            //toast error
            Helper.showToast("网络请求异常：" + e.getMessage());
            //需要把loading框隐藏
            if (loadingHandler != null) {
                loadingHandler.removeCallbacksAndMessages(null);
                DialogManager.getInstance().hideLoading();
                loadingHandler = null;
            }
            if (config.callback != null) {
                config.callback.onError();
            }
        }
    }
}
