-- 用户表
CREATE TABLE `sys_user`
(
    `Id`              bigint      NOT NULL AUTO_INCREMENT,                                        -- 主键
    `CreateTime`      datetime    NOT NULL DEFAULT CURRENT_TIMESTAMP,                             -- 创建时间
    `UpdateTime`      datetime    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, -- 更新时间
    `DelFlag`         bit(1)      NOT NULL DEFAULT b'0',                                          -- 删除标志 0-未删除 1-已删除
    `Number`          varchar(12) NOT NULL,                                                       -- 用户编号 UR000000001
    `DeviceId`        varchar(32) NOT NULL,                                                       -- 设备ID
    `NickName`        varchar(30) NULL     DEFAULT NULL,                                          -- 用户名称
    `ContactEmail`    varchar(50) NULL     DEFAULT NULL,                                          -- 用户邮箱
    `LastCheckInTime` datetime    NULL     DEFAULT NULL,                                          -- 最后签到时间
    `Status`          smallint    NOT NULL DEFAULT 0,                                             -- 用户状态 0-正常 1-异常 1024-已通知
    PRIMARY KEY (`Id`),
    INDEX `idx_sys_user_device_id` (`DeviceId`)
);

-- 历史记录表
CREATE TABLE `sys_history`
(
    `Id`         bigint      NOT NULL AUTO_INCREMENT,                                        -- 主键
    `CreateTime` datetime    NOT NULL DEFAULT CURRENT_TIMESTAMP,                             -- 创建时间
    `UpdateTime` datetime    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, -- 更新时间
    `DelFlag`    bit(1)      NOT NULL DEFAULT b'0',                                          -- 删除标志 0-未删除 1-已删除
    `UserNumber` varchar(12) NOT NULL,                                                       -- 用户编号 UR000000001
    PRIMARY KEY (`Id`),
    INDEX `idx_sys_history_user_number` (`UserNumber`)
);