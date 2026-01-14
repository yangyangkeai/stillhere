/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

/**
 * 本地存储操作类
 * */
class Storage {

    /**
     * constructor
     */
    constructor() {
        this.keys = {
            signKey: "signKey",
            token: "token",
            cipherText: "cipherText",
            user: "user"
        }
    }

    /**
     * mergeKey
     */
    mergeKey(key) {
        return "st_" + key
    }


    /**
     * getFromSession
     */
    getFromSession(key) {
        try {
            return JSON.parse(sessionStorage.getItem(this.mergeKey(key)))
        } catch (e) {
            var val = sessionStorage.getItem(this.mergeKey(key));
            if (!val || val === "") {
                return null;
            }
            return val
        }
    }

    /**
     *
     * @param key
     * @param value
     */
    setToSession(key, value) {
        if (!value) {
            sessionStorage[this.mergeKey(key)] = ""
        }
        if (typeof value === 'string') {
            sessionStorage[this.mergeKey(key)] = value
        }
        if (typeof value === 'object') {
            sessionStorage[this.mergeKey(key)] = JSON.stringify(value)
        }
    }

    /**
     * remvoe
     * @param key
     */
    remove(key) {
        localStorage.removeItem(this.mergeKey(key));
    }

    /**
     * 设置一个值到本地存储
     * */
    set(key, value) {
        if (!value) {
            localStorage[this.mergeKey(key)] = ""
        }
        if (typeof value === 'string') {
            localStorage[this.mergeKey(key)] = value
        }
        if (typeof value === 'object') {
            localStorage[this.mergeKey(key)] = JSON.stringify(value)
        }
    }

    /**
     * 从本地存储中拿一个值
     * */
    get(key) {
        try {
            return JSON.parse(localStorage.getItem(this.mergeKey(key)))
        } catch (e) {
            var val = localStorage.getItem(this.mergeKey(key));
            if (!val || val === "") {
                return null;
            }
            return val
        }
    }
}

export default new Storage()
