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

import android.util.Base64;

import java.nio.charset.StandardCharsets;

import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

/**
 * AES 加密解密工具类
 */
public class AesHelper {

    /**
     * AES 密钥（16、24、32 字节分别对应 AES-128、AES-192、AES-256）
     */
    private static final String KEY = "4A9C8B2D5E1F0A3B6C9D8E7F1A2B3C4D5E6F7A8B9C0D1E2F"; //AES-192

    /**
     * 初始化向量（16 字节）
     */
    private static final byte[] IV = new byte[]{
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
    };

    /**
     * 加密转换模式
     */
    private static final String TRANSFORMATION = "AES/CBC/PKCS5Padding";

    /**
     * 加密算法
     */
    private static final String ALGORITHM = "AES";

    /**
     * 将十六进制字符串转换为字节数组
     *
     * @param hex 十六进制字符串
     * @return 字节数组
     */
    private static byte[] hexStringToBytes(String hex) {
        if (hex == null || hex.isEmpty()) {
            return new byte[0];
        }

        // 去掉可能的空格
        hex = hex.replaceAll("\\s", "");

        int len = hex.length();
        if (len % 2 != 0) {
            throw new IllegalArgumentException("Invalid hex string, length must be even");
        }

        byte[] result = new byte[len / 2];

        for (int i = 0; i < len; i += 2) {
            // 每两个字符表示一个字节
            String byteStr = hex.substring(i, i + 2);
            result[i / 2] = (byte) Integer.parseInt(byteStr, 16);
        }

        return result;
    }

    /**
     * AES 加密
     *
     * @param plainText 原始明文
     * @return Base64 编码的密文
     */
    public static String encrypt(String plainText) {
        try {
            Cipher cipher = Cipher.getInstance(TRANSFORMATION);
            SecretKeySpec keySpec = new SecretKeySpec(hexStringToBytes(KEY), ALGORITHM);
            IvParameterSpec ivSpec = new IvParameterSpec(IV);
            cipher.init(Cipher.ENCRYPT_MODE, keySpec, ivSpec);

            byte[] encrypted = cipher.doFinal(plainText.getBytes(StandardCharsets.UTF_8));
            return Base64.encodeToString(encrypted, Base64.NO_WRAP);
        } catch (Exception e) {
            //throw new RuntimeException("AES 加密失败", e);
            LogHelper.Error("AES 加密失败: " + e.getMessage());
            return "";
        }
    }


    /**
     * AES 解密
     *
     * @param cipherText Base64 编码的密文
     * @return 解密后的明文
     */
    public static String decrypt(String cipherText) {
        try {
            Cipher cipher = Cipher.getInstance(TRANSFORMATION);
            SecretKeySpec keySpec = new SecretKeySpec(hexStringToBytes(KEY), ALGORITHM);
            IvParameterSpec ivSpec = new IvParameterSpec(IV);
            cipher.init(Cipher.DECRYPT_MODE, keySpec, ivSpec);

            //byte[] decoded = Base64.getDecoder().decode(cipherText);
            byte[] decoded = Base64.decode(cipherText, Base64.NO_WRAP);
            byte[] decrypted = cipher.doFinal(decoded);
            return new String(decrypted, StandardCharsets.UTF_8);
        } catch (Exception e) {
            //throw new RuntimeException("AES 解密失败", e);
            return "";
        }
    }

    /**
     * 获取一个加了密的Android ID 这个东西只能用一次
     *
     * @return 加密后的Android ID
     */
    public static String GetAndroidId() {
        var str = GlobalConstHelper.ANDROID_ID + ":" + Helper.getGuid() + ":" + Helper.getTimestampMs();
        return AesHelper.encrypt(str);
    }
}
