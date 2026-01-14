/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

import Cookies from 'js-cookie'

const cookies = {}
const prefix='st_'
/**
 * @description 存储 cookie 值
 * @param {String} name cookie name
 * @param {String} value cookie value
 * @param {Object} setting cookie setting
 */
cookies.set = function (name = 'default', value = '', cookieSetting = {}) {
    let currentCookieSetting = {
        expires: 1
    }
    let key = "st_" + name;
    Object.assign(currentCookieSetting, cookieSetting)
    if (IS_TEST) {
        key = prefix + key;
    }
    Cookies.set(key, value, currentCookieSetting)
}

/**
 * @description 拿到 cookie 值
 * @param {String} name cookie name
 */
cookies.get = function (name = 'default') {
    let key = "st_" + name;
    if (IS_TEST) {
        key = prefix + key;
    }
    return Cookies.get(key)
}

/**
 * @description 拿到 cookie 全部的值
 */
cookies.getAll = function () {
    return Cookies.get()
}

/**
 * @description 删除 cookie
 * @param {String} name cookie name
 */
cookies.remove = function (name = 'default') {
    let key = "st_" + name;
    if (IS_TEST) {
        key = prefix + key;
    }
    return Cookies.remove(key)
}

/**
 * 保存所有的key
 * @type {{}}
 */
cookies.keys = {
    signKey: "signKey",
    token: "token",
    cipherText: "cipherText",
    source: "source",
}

export default cookies
