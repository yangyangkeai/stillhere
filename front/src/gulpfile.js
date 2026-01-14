/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

const gulp = require('gulp');
const cssnano = require('gulp-cssnano');
const less = require('gulp-less');
const replace = require('gulp-replace');
const clean = require('gulp-clean');
const fs = require("fs");
const path = require("path");
const babelParser = require("@babel/parser");
const webpack = require('webpack-stream');
const named = require('vinyl-named');
const gzip = require('gulp-gzip');
const webpack2 = require('webpack');
const brotli = require('gulp-brotli');
const zlib = require("zlib")
const oblique = path.normalize("/")

/**
 * 抓错误并输出
 * @param error
 */
function swallowError(error) {
    console.error(error.toString())
    this.emit('end')
}

/**
 * 处理合并css
 */
let compileLess = function (dist) {
    if (!dist) {
        dist = "css"
    }
    return function (done) {
        console.log("compileLess");
        //编译less
        let time = new Date().getTime();
        gulp.src('less/main.less')
            .pipe(less())
            .pipe(replace('../../images/', '../images/'))
            .pipe(replace('.png', '.png?v=' + time))
            .pipe(replace('.gif', '.gif?v=' + time))
            .pipe(replace('.jpg', '.jpg?v=' + time))
            .pipe(cssnano({zindex: false, autoprefixer: false}))
            .on("error", swallowError)
            .pipe(gulp.dest(dist)).on("end", () => {
            if (done) {
                done();
            }
        });
    }
};

/**
 * 传入一个路径, 将对应的路径或文件复制到.net core下
 * @param path
 */
let copyFileToNetCore = function (path) {
    gulp.src(path).pipe(gulp.dest("../../core/WebApp/wwwroot")).on("error", swallowError);
}

/**
 * 从.net core的wwwroot目录下,删除指定文件或文件夹
 */
let deleteFromNetCore = function (path) {
    gulp.src(`../../core/WebApp/wwwroot/${path}`, {read: false}).pipe(clean({
        force: true,
        allowEmpty: true
    })).on("error", swallowError);
}


let fileList = [];
let filePath = 'scripts-dev';
let entrance = {
    "scripts-dev": 1/*,
    "scripts-dev\\pages": 1*/
}
entrance[`scripts-dev${oblique}pages`] = 1;
let output = {};
/**
 * 这一块是返回一个目录下的所有文件
 * @param folder
 * @param callback
 */
let readDirRecur = function (folder, callback) {
    fs.readdir(folder, function (err, files) {
        var count = 0
        var checkEnd = function () {
            ++count === files.length && callback()
        }

        files.forEach(function (file) {
            var fullPath = folder + '/' + file;

            fs.stat(fullPath, function (err, stats) {
                if (stats) {
                    if (stats.isDirectory()) {
                        return readDirRecur(fullPath, checkEnd);
                    } else {
                        /*not use ignore files*/
                        if (file[0] === '.') {

                        } else {
                            fileList.push(fullPath.replace(/\//g, oblique))
                        }
                        checkEnd()
                    }
                }
            })
        })
        //为空时直接回调
        files.length === 0 && callback()
    })
}


/**
 * 依赖分析的主方法
 * @param index
 * @param call
 */
let parseJs = function (index, call) {
    if (!fileList[index]) {
        if (call) {
            call();
        }
        return;
    }
    let currentFilePath = fileList[index];
    let dir = path.dirname(currentFilePath);
    //var name = path.basename(currentFilePath);
    //console.log("--------------------------------------------");
    //console.log("currentFilePath=" + currentFilePath);
    //console.log("dir=" + dir);
    //console.log("name=" + name);
    fs.readFile(currentFilePath, 'utf-8', function (err, data) {
        let result = null;
        try {
            result = babelParser.parse(data, {
                sourceType: "module",
                plugins: [
                    "jsx",
                    "flow",
                    "classPrivateProperties",
                    "classPrivateMethods",
                    "classProperties"
                ]
            });
        } catch (e) {
            console.log("解析依赖发生错误", e)
        }
        if (result) {
            for (let i = 0; i < result.program.body.length; i++) {
                let node = result.program.body[i];
                if (node.type === "ImportDeclaration") {
                    if (node.source && node.source.value) {
                        if (node.source.value.indexOf(".") !== -1) {
                            //let outputPath = path.normalize(dir + "\\" + node.source.value + ".js");
                            let outputPath = path.normalize(dir + oblique + node.source.value + ".js");
                            if (!output[outputPath]) {
                                output[outputPath] = {};
                            }
                            //只有为2的才会被检索出来进行编译
                            //为2的时候,  当前文件所在目录在入口白名单路径中
                            output[outputPath][currentFilePath] = entrance[dir] ? 2 : 1;
                            //console.log(path.normalize(outputPath));
                        }

                    }
                }
            }
            //console.log("--------------------------------------------");
            if (index === fileList.length - 1) {
                if (call) {
                    call();
                }
            } else {
                parseJs(++index, call);
            }
        }

    });
}

/**
 * 启动依赖分析
 * @param call
 */
let analysisImport = function (call) {
    console.log("分析依赖")
    //置空
    fileList.length = 0;
    readDirRecur(filePath, function (filePath) {
        parseJs(0, call);
    })
}

/**
 * 通一个传入的文件路径, 返回依赖的page
 * @param path
 * @param arr
 */
let getTopReferences = function (path, arr) {
    let dependent = output[path];
    if (dependent) {
        for (let key in dependent) {
            if (dependent[key] === 2) {
                arr.push(key);
            } else {
                getTopReferences(key, arr);
            }
        }
    }
}

/**
 * 重命名, 将大驼峰变成 -...-
 * @param name
 * @returns {*}
 */
function convertFileName(name) {
    var encounterNum = false;
    var r = name.replace(/[A-Z0-9]/g, function (rs, index) {
        var n = parseInt(rs);
        if (n >= 0 && n <= 9) {
            if (name.length === index + 1) {
                return rs;
            }
            //console.log(a, b, c)
            if (!encounterNum) {
                encounterNum = true;
                return "-" + rs.toLowerCase();
            } else {
                return rs;
            }
        }
        return "-" + rs.toLowerCase();
    });
    if (r.indexOf("-") === 0) {
        r = r.substring(1);
    }
    return r;
}

/**
 *
 * @param path
 * @param isTest 测试环境
 * @returns {(function(*): void)|*}
 */
let buildJs = function (path, isTest, isDev) {
    if (!isTest) {
        isTest = false;
    }
    if (!isDev) {
        isDev = false;
    }
    return function (done) {
        console.log("buildJs...", path)
        compileJs(path, "dist/scripts", isDev, isTest, done)
    }
}
/**
 * 使用webpack打包js, path可以是一个字符串, 也可以是数组
 * @param path
 * @param dist   文件编译后放哪里, 没有前后/
 * @param isDev  编译开发版
 * @param isTest 编译测试版
 * @param done  完成回调
 * @constructor
 */
let compileJs = function (path, dist, isDev, isTest, done) {
    if (isDev === undefined) {
        isDev = true;
    }
    if (isTest === undefined) {
        isTest = false;
    }
    if (!dist) {
        dist = "scripts"
    }

    const webpackPlugins = ["@babel/plugin-proposal-class-properties", "@babel/plugin-proposal-private-methods", [
        "conditional-compile", {
            define: {
                IS_DEV: isDev,
                IS_TEST: isTest
            }
        }
    ]];
    if (!isDev) {
        webpackPlugins.push(
            ["transform-remove-console", {"exclude": []}]
        )
    }
    gulp.src(path, {base: "scripts-dev"})
        .pipe(named(function (file) {
            let localPath = file.history[0].replace(file._cwd, "");
            if (localPath.indexOf("pages") !== -1) {
                return `pages${oblique}` + convertFileName(localPath.substring(localPath.lastIndexOf(oblique) + 1).replace(".js", ""))
            }
            return localPath.substring(localPath.lastIndexOf(oblique) + 1).replace(".js", "")
        }))
        .pipe(webpack(
            {
                performance: {
                    //生成的文件最大体积 字节
                    maxAssetSize: 1024 * 800,
                    maxEntrypointSize: 1024 * 800,
                },
                plugins: [
                    new webpack2.IgnorePlugin({
                        resourceRegExp: /^\.\/locale$/,
                        contextRegExp: /moment$/,
                    })
                ],
                module: {
                    rules: [
                        {
                            test: /\.m?js$/,
                            exclude: /(node_modules|bower_components)/,
                            use: {
                                loader: 'babel-loader',
                                options: {
                                    presets: ['@babel/preset-env', "@babel/preset-react"],
                                    plugins: webpackPlugins
                                }
                            }
                        }
                    ]
                },
                mode: isDev ? "development" : "production"
            }
        ))
        .on("error", swallowError)
        .pipe(gulp.dest(dist)).on("end", () => {
        if (done) {
            done();
        }
    });
}

/**
 * 使用一个js路径开始编译js
 */
let startCompileJs = function (path) {
    //path = path.replace(/\\/g, "/");
    let dir = path.substring(0, path.lastIndexOf(oblique));
    //scripts-dev/pages    //如果dir是这个, 直接编译当前文件
    //scripts-dev          //如果dir是这个, 就直接编译main
    if (entrance[dir]) {
        //直接编译当前文件
        console.log("准备编译", path)
        compileJs(path)
    } else {
        //剩下的先分析依赖, 再编译
        analysisImport(function () {
            console.log(Object.keys(output).length)
            const arr = [];
            getTopReferences(path, arr);
            console.log("得到需要编译的文件", arr)
            if (arr && arr.length > 0) {
                compileJs(arr)
            }
        })
    }
}

let jsWatchM = null;

/**
 * 监听任务, 开发时使用
 */
function watch() {

    //监听css,编译并处理
    gulp.watch('less/**/*.less', gulp.series(compileLess()));

    //创建监听器,当下面的文件发生变动时, 执行复制操作
    let fileWatcher = gulp.watch([
        "css/*",
        "images/*",
        "images/**",
        "fonts/*",
        "fonts/**",
        "scripts/*",
        "scripts/**",
    ]);
    fileWatcher.on('change', function (path) {
        copyFileToNetCore(path)
        console.log(`File ${path} was changed`);
    });
    fileWatcher.on('add', function (path) {
        copyFileToNetCore(path);
        console.log(`File ${path} was added`);
    });
    fileWatcher.on('addDir', function (path) {
        copyFileToNetCore(path);
        console.log(`Dir ${path} was added`);
    });
    fileWatcher.on('unlink', function (path) {
        deleteFromNetCore(path);
        console.log(`File ${path} was unlink`);
    });

    //创建监听器, 监听所有js变更
    let jsWatcher = gulp.watch("scripts-dev/**/*.js");
    jsWatcher.on("change", function (path) {
        //console.log(path)
        clearTimeout(jsWatchM);
        jsWatchM = setTimeout(() => {
            try {
                startCompileJs(path)
                console.log(`File ${path} was changed`);
            } catch (e) {
                console.log(e)
            }
        }, 600);
    });
    jsWatcher.on('add', function (path) {
        //分析依赖
        analysisImport(function () {
        })
    });
    jsWatcher.on('unlink', function (path) {
        //分析依赖
        analysisImport(function () {
        })
    });
}

/**
 * 清理dist
 * @param done
 */
let clear = function (done) {
    console.log("clear....");
    gulp.src([
        "dist/***"
    ], {read: false})
        .pipe(clean({force: true, allowEmpty: true}))
        .pipe(gulp.dest("."))
        .on("end", () => {
            if (done) {
                done();
            }
        });
}


/**
 * 将资源文件复制到dist
 * @param done
 */
let copyToDist = function (done) {
    console.log("copyToDist....");
    gulp.src([
        'images/**',
        'css/**',
        'scripts/**',
        'apk/**',
        'favicon.ico',
        'ver.txt',
    ], {base: "."}).pipe(gulp.dest("dist")).on("end", () => {
        if (done) {
            done();
        }
    });
}

/**
 * copyToNetCore
 * @param done
 */
let copyToNetCore = function (done) {
    console.log("copyToNetCore....");
    gulp.src([
        'dist/**'
    ], {base: "./dist"}).pipe(gulp.dest("../../core/WebApp/wwwroot")).on("end", () => {
        if (done) {
            done();
        }
    });
}

/**
 * gzpi压缩文件
 */
let gzipCompress = function (done) {
    console.log("gzip....");
    gulp.src([
        "dist/scripts/**/*.js",
        "dist/css/**/*.css",
    ], {base: "."})
        .pipe(gzip({
            gzipOptions: {level: 9}
        }))
        .pipe(gulp.dest("./")).on("end", () => {
        console.log("gzip ok");
        done();
    });
}

/**
 * brotli
 * @param done
 */
let brCompress = function (done) {
    console.log("brotli....");
    gulp.src([
        "dist/scripts/**/*.js",
        "dist/css/**/*.css",
    ], {base: "."})
        .pipe(brotli.compress({
            level: 11,
            params: {
                [zlib.constants.BROTLI_PARAM_MODE]: zlib.constants.BROTLI_MODE_TEXT,
                [zlib.constants.BROTLI_PARAM_QUALITY]: zlib.constants.BROTLI_MAX_QUALITY,
            }
        }))
        .pipe(gulp.dest("./")).on("end", () => {
        console.log("brotli ok");
        done();
    });
}

//gulp.task("build", gulp.series(clear,copyToDist, compileLess("dist/css")));
gulp.task("build", gulp.series(clear, copyToDist, compileLess("dist/css"), buildJs("scripts-dev/main.js", false, false), buildJs("scripts-dev/pages/*.js", false, false), gzipCompress, brCompress));
gulp.task("build.debug", gulp.series(clear, copyToDist, compileLess("dist/css"), buildJs("scripts-dev/main.js", false, true), buildJs("scripts-dev/pages/*.js", false, true), gzipCompress, brCompress));
gulp.task("br", gulp.series(brCompress));
gulp.task("gzip", gulp.series(gzipCompress));
gulp.task("build.test", gulp.series(clear, copyToDist, compileLess("dist/css"), buildJs("scripts-dev/main.js", true, false), buildJs("scripts-dev/pages/*.js", true, false), gzipCompress, brCompress));
gulp.task("build.dev", gulp.series(clear, copyToDist, compileLess("dist/css"), buildJs("scripts-dev/main.js", false, true), buildJs("scripts-dev/pages/*.js", false, true), gzipCompress, brCompress, copyToNetCore));
exports.watch = watch;
