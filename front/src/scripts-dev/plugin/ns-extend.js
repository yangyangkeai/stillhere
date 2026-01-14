/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

(function () {

    //ReplaceAll
    String.prototype.replaceAll = function (s1, s2) {
        return this.replace(new RegExp(s1, "gm"), s2);
    }

//ba转换成return
    String.prototype.br2Return = function () {
        return this.replace(new RegExp("<br>", "gm"), "\n").replace(new RegExp("<br/>", "gm"), "\n").replace(new RegExp("<br />", "gm"), "\n");
    }

//换行转换成br
    String.prototype.return2Br = function () {
        return this.replace(/\r?\n/g, "<br />");
    }

//去掉前后空格
    String.prototype.trim = function () {
        return this.replace(/(^\s*)|(\s*$)/g, '')
    }
//验证电子邮件
    String.prototype.isEmail = function () {
        return /^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$/.test(this)
    }
//验证非空
    String.prototype.Empty = function () {
        if (this === '') {
            return true
        } else {
            return false
        }
    }
//验证只能是数字
    String.prototype.isNum = function () {
        return /^[0-9]*$/.test(this)
    }
//验证网址
    String.prototype.isUrl = function () {
        return /(http\:\/\/)?([\w.]+)(\/[\w- \.\/\?%&=]*)?/gi.test(this)
    }
//验证只能输入N位数字
    String.prototype.isLenNum = function (N) {
        var regExpStr = '^[0-9]{' + N + '}$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//只能输入N-M位数字
    String.prototype.isLen_LenNum = function (N, M) {
        var regExpStr = '^[0-9]{' + N + ',' + M + '}$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//至少输N位数字
    String.prototype.isLen_LenNum = function (N) {
        var regExpStr = '^[0-9]{' + N + ',}$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//只能输入英文
    String.prototype.isEn = function () {
        var regExpStr = '^[A-Za-z]+$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//只能输入字母,数字
    String.prototype.isEnNum = function () {
        var regExpStr = '^[A-Za-z0-9]+$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//验证用户名,只能以字母开头,只能有字母数字下滑线,至少 N位 最多M位
    String.prototype.isUser = function (N, M) {
        var regExpStr = '^[a-zA-Z]{1}([a-zA-Z0-9]|[_]){' + N + ',' + M + '}$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//只能有字母数字下滑线
    String.prototype.isEnNum_ = function () {
        var regExpStr = '^([a-zA-Z0-9]|[_])+$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//验证中国电话号码 区号-号码的形式,号码正文不得以0开始
    String.prototype.isPhone = function () {
        var regExpStr = '^[0-9]{3,4}-[1-9][0-9]{6,7}$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//验证中国邮编 六位数字
    String.prototype.isEamilCode = function () {
        var regExpStr = '^[0-9]{6}$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//验证QQ号 五位 - 十位数字 不能以 0 开头
    String.prototype.isQq = function () {
        var regExpStr = '^[1-9][0-9]{4,9}$'
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//验证中国手机号码 11位数字 以1开头
    String.prototype.isMobile = function () {
        var regExpStr = /^1[3|4|5|6|7|8|9][0-9]{9}$/
        var regObj = new RegExp(regExpStr)
        return regObj.test(this)
    }
//验证小数
    String.prototype.isDecimal = function () {
        return /^\d+(\.\d{1,2})?$/.test(this)
    }


    //------------------
//数组中包含一个元素
    Array.prototype.contain = function (val) {
        if (!val) {
            return true;
        }
        for (var i = 0; i < this.length; i++) {
            if (this[i] === val) {
                return true;
            }
        }
        return false;
    }

})()
