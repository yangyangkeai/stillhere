# StillHere · 还在么

StillHere 是一个极简的开源打卡应用，用于**每日确认“我还在”**。

用户每天进入应用，点击一次按钮即可完成打卡；
一旦在设定时间内未打卡，系统会自动向预设的紧急联系人发送提醒邮件，提示可能存在异常情况。

## 核心特性

- 🟢 **一键打卡**：每天一次，点一下即可
- ⏰ **超时未打卡检测**：可配置检测周期
- 📧 **紧急联系人通知**：通过 Email 发送异常提醒
- 🔒 **极简 & 克制**：不追踪、不定位、不收集多余数据
- 🌱 **完全开源**：代码透明，可自行部署
- 🚧 **持续演进**：更多功能将随项目迭代逐步加入

## 效果预览

<p align="left">
  <img src="assets/20260115_013839_stillhere.gif" width="360" />
</p>

## 技术架构

StillHere 采用前后端分离架构，不同平台使用原生技术栈实现。

```text
stillhere/
├── core/               # .NET程序
│   ├── WebApp/         # API服务
│   ├── App.SendMails/  # 邮件发送服务
├── android/            # 原生安卓程序
├── assets/             # MD文档引用的资源
├── front/              # 一个基于React渲染的Web前端项目，目前仅有一个欢迎首页。可基于此项目扩展官网。
├── resource/           # 一些资源文件
├── LICENSE             # 开源协议
├── LICENSE_HEADER      # 代码版权声明头
└── README.md           # 项目说明
```

### 后端

- **.NET 10 Web API**。
- **MySQL 8**：核心业务数据存储。
- **Redis**：缓存/简单消息收发。
- **Email 通知**：依赖SMTP服务发送邮件。

### 前端
一个基于React渲染的Web前端项目（自定义开发架构），后端项目跑起来后，浏览器直接访问就会显示这里的页面。目前实现了一个首页及404页面。 

### 客户端

- **Android**：原生 Android 应用（Java）
- **iOS**：原生 iOS 应用（开发中）

客户端主要功能：

- 每日打卡
- 状态展示
- 紧急联系人配置

## 本地运行指南

### 一、环境准备

请确保已安装/拥有以下依赖/服务：

- .NET SDK **10.x**
- MySQL **8.x**
- Redis **2.x 或以上**
- Android Studio（仅Android客户端需要）
- Visual Studio 2026或以上版本（仅开发后端API时需要）

### 二、数据库（MySQL 8）

创建数据库并执行 resource/db/ddl.sql 脚本初始化数据库。

```sql
CREATE DATABASE still_here CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;
mysql -u root -p still_here < resource/db/ddl.sql
```

找到.NET项目中的appsettings.development.json/appsettings.json修改数据库连接字符串。或根据连接字符串中的变量名配置系统环境变量。变量名解释如下：

- P_ST_DB 数据库名称
- P_ST_DB_SOURCE 数据库服务地址
- P_ST_DB_USER 数据库登录用户名
- P_ST_DB_PWD 数据库登录密码

### 三、Redis

找到.NET项目中的appsettings.development.json/appsettings.json中的Redis部分修改Redis连接地址。

### 四、编译前端

请确保已安装以下依赖：

- Node v22+
- 全局 Gulp 3.x
- Yarn 1.22+

```bash
cd front\\src
yarn install
gulp build.dev
```

### 五、运行后端

```bash
cd core\\WebApp
dotnet run
```

- 运行后会监听本地443端口，如果端口被占用会报错。
- 本地运行会使用域名https://st.local.nproj.net/，请在自己的HOSTS文件中将此域名解析到本机局域网IP上。
- 注意如果证书不被信任请导入resource/web_ca/yang_dev_ca.crt，或修改使用的域名，并修改core/WebApp/ca.pfx为自己信任的开发证书。
- 一切就绪，用浏览器访问可以看到首页。

HOSTS配置：

```bash
192.168.x.x st.local.nproj.net
```

### 六、运行 Android 客户端

- 使用Android Studio打开android/目录。
- build.gradle.kts中修改API地址为本地后端地址。（如果没有修改域名可以忽略）
- 运行到模拟器或真机即可。（请注意真机是否可访问本地运行的后端程序）

### 七、邮件发送（App.SendMails）

此程序目前还处于初级阶段，它是一个独立的控制台应用程序，线上部署时应使用cron服务托管调度。本地预览直接运行它就可以看到效果。
SMTP环境变量：

- P_ST_SMTP_ACCOUNT SMTP账号
- P_ST_SMTP_PWD SMTP密码
- P_ST_SMTP_SERVER SMTP服务器地址

## 线上部署

本项目是一套 **完整、可直接用于线上环境部署的程序**，并非 Demo 或示例代码。
可部署于：

- 云服务器（阿里云 / 腾讯云 / AWS / 自建服务器）
- Docker / 容器化环境
- 私有化内网环境

### 部署说明

由于不同部署环境在以下方面可能存在差异：

- 网络与防火墙策略
- 邮件服务配置
- 数据库与 Redis 高可用方案
- 定时任务 / 后台任务调度方式
- 域名、HTTPS、反向代理配置

README 中仅提供通用的本地运行说明，
**实际线上部署请根据自身环境进行合理调整。**

### 技术支持与联系

如果你希望将 StillHere 部署到线上环境，或在部署过程中遇到任何技术问题，欢迎与我联系交流。

- 可通过 GitHub Issue 提交问题


## License

本项目采用 **GNU Affero General Public License v3.0（AGPL-3.0）** 开源协议。

你可以自由使用、修改和部署本项目。
但如果你将本项目 **部署为线上服务并对外提供访问**，
则 **必须公开你所使用的完整源代码（包括你的修改部分）**。

完整协议内容请参见 [LICENSE](./LICENSE) 文件。

## Project Philosophy

愿大家每天点下按钮时，都是美好、平安一天。

我一直都在。
