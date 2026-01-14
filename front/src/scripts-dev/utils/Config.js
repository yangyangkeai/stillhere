/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

import Apis from "./Apis";

class Config {
    //当前浏览器指纹
    #fingerprint = "";

    constructor() {
        this.apis = Apis
        if (IS_DEV) {

        }
        if (IS_TEST) {

        }
        this.data = {
            webTitle: "还在么",
            securityHost: "local.xxx.com,xxx.com"  //不带端口
        }
        this.scrollHandlers = new Set();
    }

    /**
     * 设置指纹
     * @param fingerprint
     */
    setFingerprint(fingerprint) {
        this.#fingerprint = fingerprint;
    }

    /**
     * 获取指纹
     * @returns {string}
     */
    getFingerprint() {
        return this.#fingerprint;
    }
}

export default Config
