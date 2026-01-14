/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

import Helper from "./utils/Helper";
import Config from "./utils/Config";
import cookies from "./utils/Cookies";
import storage from "./utils/Storage";
import $ from "jquery";
import ReactDOM from "react-dom"
import React from "react"
import FingerprintJS from '@fingerprintjs/fingerprintjs';

window.$ = $;
window.ReactDOM = ReactDOM;
window.React = React;

function main() {

    //const vConsole = new VConsole({theme: 'dark'});

    //调试器
    if (window.VConsole && (IS_DEV || IS_TEST)) {
        new window.VConsole();
    }

    //这个东西保存弹出的layer， 如果被弹过， 这里就会存在， 不再使用加载效果
    window.$lm = {};

    //全局通用工具类及配置
    let config = new Config();
    window.$config = config;
    React.Component.prototype.$config = config;

    window.$cookie = cookies;
    React.Component.prototype.$cookie = cookies;

    window.$storage = storage;
    React.Component.prototype.$storage = storage;

    let helper = new Helper();
    helper.regResizeListener();
    window.$helper = helper;
    React.Component.prototype.$helper = helper;

    //读取并设置fingerprint
    FingerprintJS.load().then(fp => {
        fp.get().then(result => {
            //console.log("fingerprint", result.visitorId);
            $config.setFingerprint(result.visitorId);
        });
    });

    //是否在微信里
    let ua = window.navigator.userAgent.toLowerCase();
    window.$ua = ua;
    if (ua.indexOf("micromessenger") !== -1) {
        window.$isWx = true;
        if (ua.indexOf("86.0.4240.99") !== -1) {
            //微信中内核过旧
            $("body").addClass("weixin")
        }
    }
    if (ua.indexOf("android") !== -1 || ua.indexOf("iphone") !== -1) {
        window.$isMobile = true;
    }
    if (ua.indexOf("ipad") !== -1 || ua.indexOf("iphone") !== -1) {
        window.$isIos = true;
    }
    console.log(ua)

    //获取执行参数
    let controller = $("meta[name='controller']").attr("content");
    let action = $("meta[name='action']").attr("content");
    let params = $("meta[name='params']").attr("content");
    if (!controller || !action || controller === '' || action === '') {
        controller = "error";
        action = "404";
    }
    //加载pageJs并执行
    helper.loadPage(controller, action, params, 0);

    //全局hover-btn
    $(document).on("touchstart", ".hover-btn", function () {
        $(this).addClass("hover");
    });
    $(document).on("touchend", ".hover-btn", function () {
        $(this).removeClass("hover");
    });

    //注册全局滚动
    $(window).scroll(function () {
        let top = $(window).scrollTop();
        let winHeight = $(window).height();
        let contentHeight = $(".ns-main").outerHeight(true);
        this.$config.scrollHandlers.forEach((item) => {
            item(top, winHeight, contentHeight);
        });
    });
}

$(document).ready(function () {
    main();
});

