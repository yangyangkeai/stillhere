/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

import "../plugin/ns-extend"
import MD5 from 'md5'
import urlParse from 'url-parse'
import query from 'query-string'
import moment from 'moment'

import 'moment/locale/zh-cn'
import AlertDialog from "./dialog/AlertDialog";

moment.locale("zh-cn")


class Helper {

    /**
     * 是否已经注册完了resize事件, 只能注册一次
     * @type {boolean}
     */
    #resizeReged = false;

    /**
     * 构造方法
     */
    constructor(config) {
        //这个是为了防止按钮高频率点击
        this._mts = {};
        this._mtsMax = 500;
    }

    /**
     * clearHtml
     * @param str
     * @returns {*}
     */
    clearHtml(str) {
        if (!str) {
            return "";
        }
        return str.replace(/<[^>]+>/g, "");
    }

    /**
     * md5
     * @param str
     * @returns {*}
     */
    md5(str) {
        return MD5(str)
    }

    /**
     *
     */
    setTitle(title, addGlobalTitle) {
        if (addGlobalTitle === undefined) {
            addGlobalTitle = true;
        }
        console.log("setTitle", title, addGlobalTitle);
        $("title").html((title ? title + (addGlobalTitle ? " | " : "") : "") + (addGlobalTitle ? window.$config.data.webTitle : ""));
    }

    /**
     * sToMin
     */
    sToMin(duration) {
        duration = parseInt(duration * 1000);
        if (!duration || duration <= 0) {
            return 0;
        }
        return parseInt(duration / 1000 / 60);
    }

    /**
     * getControllerName
     */
    getControllerName() {
        //console.log($("meta[name='controller']").attr("content"));
        return $("meta[name='controller']").attr("content");
    }

    /**
     * getActionName
     * @returns {*|jQuery}
     */
    getActionName() {
        //console.log($("meta[name='controller']").attr("content"));
        return $("meta[name='action']").attr("content");
    }

    /**
     * formatFileLength
     */
    formatFileLength(length) {
        if (length < 1024) {
            return length + "B";
        }
        if (length < 1024 * 1024) {
            return (length / 1024).toFixed(2) + "KB";
        }
        if (length < 1024 * 1024 * 1024) {
            return (length / 1024 / 1024).toFixed(2) + "MB";
        }
        if (length < 1024 * 1024 * 1024 * 1024) {
            return (length / 1024 / 1024 / 1024).toFixed(2) + "GB";
        }
    }

    /**
     * closeLayer
     * @param callback 关闭后的回调
     * @param deleteOutEl 是否删除外层元素
     * @param id 删除的id
     */
    closeLayer(callback, deleteOutEl, id) {
        console.log("closeLayer已被调用", id)
        if (!id) {
            id = "ns-layer-out";
        }
        let element = document.getElementById(id);
        if (element) {
            console.log("closeLayer存在原来的, 先尝试关闭再调用")
            $(element).find(".nlc").removeClass("show");
            //console.log($(element).find("nlc"))
            if (deleteOutEl) {
                $(element).removeClass("show");
            }
            setTimeout(() => {
                ReactDOM.unmountComponentAtNode(element);
                if (deleteOutEl) {
                    element.remove();
                }
                if (callback) {
                    callback();
                }
            }, 110);
        } else {
            console.log("closeLayer直接回调")
            if (callback) {
                callback();
            }
        }

    }

    /**
     * closeLoading
     */
    closeLoading() {
        $(".custom-loading").remove();
    }

    /**
     * 时间差 ms
     * @param time1 大
     * @param time2  小
     * @returns {string}
     */
    datetimeDiff(time1, time2) {
        return moment(time1).diff(moment(time2));
    }

    /**
     * http://momentjs.cn/docs/#/get-set/
     * formatDateTime
     * @param time
     * @param fmt
     */
    formatDateTime(time, fmt) {
        if (!fmt) {
            fmt = "YYYY-MM-DD HH:mm"
        }
        let m = null;
        if (typeof (time) === "number") {
            m = moment(time * 1000);
        } else {
            m = moment(time);
        }
        if (m.year() === moment("1970-01-01 08:00:00").year()) {
            return "";
        }
        return m.format(fmt);
    }


    /**
     *formatNum
     */
    formatNum(num) {
        if (typeof (num) === "number") {
            let s = num.toString();
            if (s.length === 1) {
                return "0" + s;
            }
            return s.toString();
        }
        return num;
    }

    /**
     * getQueryValues
     */
    getQueryValues() {
        return query.parse(location.search);
    }


    /**
     * copy
     */
    copy(obj) {
        return JSON.parse(JSON.stringify(obj));
    }

    /**
     * 没事就是一个测试
     */
    test() {
        console.log("hello world!");
    }

    /**
     *
     * @param msg
     */
    success(msg) {
        //console.log("toast", msg)
        setTimeout(() => {
            antd.message.success(msg);
        }, 60);
    }

    /**
     *
     * @param msg
     */
    toast(msg) {
        //console.log("toast", msg)
        setTimeout(() => {
            antd.message.warning(msg);
        }, 60);
    }

    /**
     * 显示错误
     * @param msg
     */
    error(msg) {
        console.log("error", msg)
        antd.message.error(msg);
    }

    /**
     * loading
     * @param msg
     */
    loading(msg) {
        antd.message.loading(msg);
    }

    /**
     * getTimestamp
     * @returns {number}
     */
    getTimestamp() {
        return new Date().getTime();
    }

    /**
     * getParams
     */
    getParams() {
        return $("meta[name='params']").attr("content");
    }

    /**
     * getUUid
     * @returns {string}
     */
    getUUid() {
        let s = []
        let hexDigits = '0123456789abcdef'
        for (let i = 0; i < 36; i++) {
            s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1)
        }
        s[14] = '4'  // bits 12-15 of the time_hi_and_version field to 0010
        s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1)  // bits 6-7 of the clock_seq_hi_and_reserved to 01
        s[8] = s[13] = s[18] = s[23] = '-'
        let uuid = s.join('')
        return uuid.toLowerCase()
    }

    /**
     * 注册浏览器resize事件
     */
    regResizeListener() {
        if (this.#resizeReged) {
            this.#resizeReged = true;
            return;
        }
        this.#setSize(0);
        $(window).resize(() => {
            this.#setSize(0);
        });
    }

    /**
     * 从当前上下文环境中运行一个页面
     */
    runPage(params, count) {
        if (count >= 3) {
            return;
        }
        if (window.page) {
            window.page.main(params);
        } else {
            count++;
            setTimeout(() => {
                this.runPage(params, count)
            }, 100);
        }
    }

    /**
     * 加载一个页面
     * @param controller
     * @param action
     */
    loadPage(controller, action, params, count) {
        //console.log(count);
        if (count >= 5) {
            return;
        }
        let url = `${this.getBasePath()}scripts/pages/${controller}-${action}.js?t=${this.getTimestamp()}`;
        $.ajax({
            url: url,
            dataType: "script",
            success: () => {
                this.runPage(params, 0);
            },
            error: () => {
                this.loadPage("error", "404", params, ++count)
            }
        });
    }

    /**
     * getBasePath
     */
    getBasePath() {
        let path = $("body").attr("data-base");
        //console.log(path);
        if (!path) {
            path = "/"
        }
        return path;
    }

    /**
     * 获取当前年份
     */
    getCurrentYear() {
        return new Date().getFullYear()
    }

    /**
     *
     * @param url
     */
    urlContent(url) {
        let base = this.getBasePath();
        url = url.replace("~/", base);
        url = this.appendName(url);
        return url;
    }

    /**
     *
     * @param s
     * @returns {string}
     */
    urlEncode(s) {
        return encodeURIComponent(s);
    }

    /**
     * htmlEncode
     * @param str
     * @returns {string|Url|boolean}
     */
    htmlEncode(str) {
        let hex = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'];
        let preescape = str;
        let escaped = "";
        for (let i = 0; i < preescape.length; i++) {
            let p = preescape.charAt(i);
            escaped = escaped + escapeCharx(p);
        }

        return escaped;

        function escapeCharx(original) {
            let found = true;
            let thechar = original.charCodeAt(0);
            switch (thechar) {
                case 10:
                    return "<br/>";
                    break; //newline
                case 32:
                    return "&nbsp;";
                    break; //space
                case 34:
                    return "&quot;";
                    break; //"
                case 38:
                    return "&amp;";
                    break; //&
                case 39:
                    return "&#x27;";
                    break; //'
                case 47:
                    return "&#x2F;";
                    break; // /
                case 60:
                    return "&lt;";
                    break; //<
                case 62:
                    return "&gt;";
                    break; //>
                case 198:
                    return "&AElig;";
                    break;
                case 193:
                    return "&Aacute;";
                    break;
                case 194:
                    return "&Acirc;";
                    break;
                case 192:
                    return "&Agrave;";
                    break;
                case 197:
                    return "&Aring;";
                    break;
                case 195:
                    return "&Atilde;";
                    break;
                case 196:
                    return "&Auml;";
                    break;
                case 199:
                    return "&Ccedil;";
                    break;
                case 208:
                    return "&ETH;";
                    break;
                case 201:
                    return "&Eacute;";
                    break;
                case 202:
                    return "&Ecirc;";
                    break;
                case 200:
                    return "&Egrave;";
                    break;
                case 203:
                    return "&Euml;";
                    break;
                case 205:
                    return "&Iacute;";
                    break;
                case 206:
                    return "&Icirc;";
                    break;
                case 204:
                    return "&Igrave;";
                    break;
                case 207:
                    return "&Iuml;";
                    break;
                case 209:
                    return "&Ntilde;";
                    break;
                case 211:
                    return "&Oacute;";
                    break;
                case 212:
                    return "&Ocirc;";
                    break;
                case 210:
                    return "&Ograve;";
                    break;
                case 216:
                    return "&Oslash;";
                    break;
                case 213:
                    return "&Otilde;";
                    break;
                case 214:
                    return "&Ouml;";
                    break;
                case 222:
                    return "&THORN;";
                    break;
                case 218:
                    return "&Uacute;";
                    break;
                case 219:
                    return "&Ucirc;";
                    break;
                case 217:
                    return "&Ugrave;";
                    break;
                case 220:
                    return "&Uuml;";
                    break;
                case 221:
                    return "&Yacute;";
                    break;
                case 225:
                    return "&aacute;";
                    break;
                case 226:
                    return "&acirc;";
                    break;
                case 230:
                    return "&aelig;";
                    break;
                case 224:
                    return "&agrave;";
                    break;
                case 229:
                    return "&aring;";
                    break;
                case 227:
                    return "&atilde;";
                    break;
                case 228:
                    return "&auml;";
                    break;
                case 231:
                    return "&ccedil;";
                    break;
                case 233:
                    return "&eacute;";
                    break;
                case 234:
                    return "&ecirc;";
                    break;
                case 232:
                    return "&egrave;";
                    break;
                case 240:
                    return "&eth;";
                    break;
                case 235:
                    return "&euml;";
                    break;
                case 237:
                    return "&iacute;";
                    break;
                case 238:
                    return "&icirc;";
                    break;
                case 236:
                    return "&igrave;";
                    break;
                case 239:
                    return "&iuml;";
                    break;
                case 241:
                    return "&ntilde;";
                    break;
                case 243:
                    return "&oacute;";
                    break;
                case 244:
                    return "&ocirc;";
                    break;
                case 242:
                    return "&ograve;";
                    break;
                case 248:
                    return "&oslash;";
                    break;
                case 245:
                    return "&otilde;";
                    break;
                case 246:
                    return "&ouml;";
                    break;
                case 223:
                    return "&szlig;";
                    break;
                case 254:
                    return "&thorn;";
                    break;
                case 250:
                    return "&uacute;";
                    break;
                case 251:
                    return "&ucirc;";
                    break;
                case 249:
                    return "&ugrave;";
                    break;
                case 252:
                    return "&uuml;";
                    break;
                case 253:
                    return "&yacute;";
                    break;
                case 255:
                    return "&yuml;";
                    break;
                case 162:
                    return "&cent;";
                    break;
                case '\r':
                    break;
                default:
                    found = false;
                    break;
            }
            return original;
        }
    }

    /**
     * 打开一个新的页面
     * @param url 链接
     * @param blank 是否新窗口
     */
    openUrl(url, blank) {

        //ios专用
        if (window.$isIos) {
            window.location.href = url
            //window.location.replace(url);
            return;
        }

        if ($("#redirect-link").length <= 0) {
            $("body").append("<a href='#' id='redirect-link'></a>");
        }
        if (blank) {
            $("#redirect-link").attr("target", "_blank");
        }
        $("#redirect-link").attr("href", url);
        document.getElementById("redirect-link").click();
    }

    /**
     * scrollTo
     */
    scrollTo(hash, count) {
        if (count === undefined) {
            count = -1;
        }
        let offset = $(hash).offset();
        if (offset !== $('body,html').scrollTop()) {
            if (offset) {
                $('body,html').animate({scrollTop: offset.top - 30}, 200);
            }
        }
        if (count >= 0 && count <= 2) {
            setTimeout(() => {
                this.scrollTo(hash, ++count)
            }, 300);
        }
    }

    /**
     *  urlParse
     * @param url
     * @returns {Url}
     */
    parseUrl(url) {
        if (!url) {
            url = window.location.href;
        }
        return urlParse(url, true);
    }

    /**
     * 不带安全校验的跳转
     */
    redirect(url) {
        if (!url) {
            return;
        }
        if (url === "/" || url === "~/") {
            this.redirect_home()
            return;
        }
        if ($("#redirect-link").length <= 0) {
            $("body").append("<a href='#' id='redirect-link'></a>");
        }
        url = this.urlContent(url);
        $("#redirect-link").attr("href", url);
        document.getElementById("redirect-link").click();
    }

    /**
     * 带安全校验的跳转
     */
    redirect_security(url, addFromUrl, addname) {

        if (!url) {
            return;
        }

        if (url === "/" || url === "~/") {
            this.redirect_home()
            return;
        }

        if ($("#redirect-link").length <= 0) {
            $("body").append("<a href='#' id='redirect-link'></a>");
        }

        if (url.toLowerCase().indexOf("http://") === -1 && url.toLowerCase().indexOf("https://") === -1) {
            url = this.urlContent(url);
        }

        if (addFromUrl && location.href.toLowerCase().indexOf("fu=") === -1) {
            let fromUrl = this.urlEncode(location.href);
            if (url.indexOf("?") === -1) {
                url += "?fu=" + fromUrl;
            } else {
                url += "&fu=" + fromUrl;
            }
        }
        //补充name参数
        // if (addname && location.href.toLowerCase().indexOf("name=") === -1) {
        //     url = this.appendName(url);
        // }
        //安全验证
        if (url.toLowerCase().indexOf("http://") !== -1 || url.toLowerCase().indexOf("https://") !== -1) {
            console.log("检测到绝对地址引用");
            let arr = window.$config.data.securityHost.split(",");
            for (let i = 0; i < arr.length; i++) {
                let c = arr[i];
                console.log("检测安全合法性");
                let urlObject = this.parseUrl(url);
                if (urlObject.hostname === c) {
                    $("#redirect-link").attr("href", url);
                    document.getElementById("redirect-link").click();
                    console.log("同意执行跳转, 并已经完成跳转引导" + ":" + url);
                    return;
                }
            }
            console.log("未同意,回首页!");
            $("#redirect-link").attr("href", this.urlContent("~/"));
            document.getElementById("redirect-link").click();
        } else {
            //console.log("检测到内部地址引用");
            $("#redirect-link").attr("href", url);
            document.getElementById("redirect-link").click();
            console.log("执行跳转, 并已经完成跳转引导" + ":" + url);
            //window.location.href = url;
        }
    }

    /**
     * 补充name参数
     * @param url
     * @returns {*}
     */
    appendName(url) {
        //检测当前页面是否存在name=xxx，如果存在需要带上这个参数和值
        var name = this.parseUrl().query.name;
        if (name) {
            if (url.indexOf("?") === -1) {
                url += "?name=" + name;
            } else {
                url += "&name=" + name;
            }
        }

        return url;
    }

    /**
     * 回到首页,直接replace
     */
    redirect_home() {
        window.location.replace(this.urlContent("~/"));
    }

    /**
     * 用来处理底部空间大小
     */
    #setSize(count) {
        if (count >= 50) {
            return false;
        }
        if ($(".ns-main").length > 0) {
            if ($(".ns-main").width() <= 980) {
                $(".ns-main").css("padding-bottom", 0).css("visibility", "visible");
            } else {
                $(".ns-main").css("padding-bottom", $(".bottom").outerHeight()).css("visibility", "visible");
            }
            setTimeout(() => {
                this.#setSize(count++);
            }, 200);
        } else {
            setTimeout(() => {
                this.#setSize(count++);
            }, 100);
        }
    }


    /**
     * allowClick
     * @param name
     */
    allowClick(name) {
        let t = new Date().getTime();
        if (this._mts[name] && t - this._mts[name] < this._mtsMax) {
            return false;
        }
        this._mts[name] = t;
        return true;
    }

    /**
     *
     */
    showUnknownError() {
        this.toast("发生未知错误")
    }


    /**
     * getCurrentHost
     */
    getCurrentHost() {
        return this.parseUrl(window.location.href).host;
    }

    /**
     * getLayerOutEle
     * @param className
     * @param id
     * @returns {HTMLElement}
     */
    getLayerOutEle(className, id) {
        console.log("getLayerOutEle")
        if (!id) {
            id = "ns-layer-out";
        }
        let el = document.getElementById(id);
        if (!el) {
            console.log("没有ns-layer-out, 创建一个")
            el = document.createElement("div");
            el.id = id;
            $("#react-content").get(0).append(el)
            setTimeout(() => {
                $(el).addClass("show")
            }, 50);
        }
        if (className) {
            $(el).addClass(className);
        } else {
            let className = el.className;
            let arr = className.split(" ");
            for (let i = 0; i < arr.length; i++) {
                if (arr[i].indexOf("cus-") === 0) {
                    $(el).removeClass(arr[i]);
                }
            }
            /* $(el).removeClass(function (index, className) {
                 if (className.indexOf("cus-") === 0) {
                     console.log(className)
                     return className;
                 }
                 return null;
             });*/
        }
        return el;
    }

    /**
     * 在屏幕中间显示一个自定义的loading
     * @param mask 有一个毛玻璃的背景
     */
    showLoading(mask) {
        console.log("showLoading", mask);
        if ($(".custom-loading").length === 0) {
            let className = "custom-loading";
            if (mask) {
                className += " mask";
            }
            $(".ns-content").append($(`<div class='${className}'><span></span><span></span><span></span></div>`));
            setTimeout(() => {
                $(".custom-loading").addClass("show")
            }, 70);
        }
    }

    /**
     * 弹一个通用的提示框
     * @param config
     */
    showAlertLayer(config) {
        if (config && config.dontCloseOther) {
            if (!config._elId) {
                config._elId = `ns-layer-out-${this.getUUid()}`;
            }
            let element = this.getLayerOutEle("ns-layer-out", config._elId);
            ReactDOM.render(<AlertDialog config={config}/>, element);
            return;
        }
        this.closeLayer(() => {
            let element = this.getLayerOutEle();
            ReactDOM.render(<AlertDialog config={config}/>, element);
        });
    }
}

export default Helper
