// Copyright (C) 2026 Tingyang Zhang
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.

﻿namespace Nproj.StillHereApp.Model.Regular;

public class RegularConst
{
    public const string USERNUMBER = @"^UR\d{6,8}$";
    public const string USERNUMBER_MSG = "用户编号格式不正确";

    public const string PWD = @"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$";
    public const string PWD_MSG = "密码格式不正确,至少6位字母数字组合";

    public const string MOBILE = @"^(0|86|17951)?1[23456789]\d{9}$";
    public const string MOBILE_MSG = "手机号格式不正确";

    public const string NUMBER_CODE = @"^\d{6}$";
    public const string NUMBER_CODE_MSG = "验证码格式不正确";
    
    public const string NUMBER_LOGIN_ID = @"^\d{6,8}$";
    public const string NUMBER_LOGIN_ID_MSG = "登录账号格式不正确";

    public const string NUMBER_CODE2 = @"^[a-zA-Z0-9]{4}$";
    public const string NUMBER_CODE2_MSG = "验证码格式不正确";

    public const string COMPANY_NUMBER = @"^\d{19}$";
    public const string COMPANY_NUMBER_MSG = "公司编号格式不正确";

    public const string COMPANY_NAME = @"^[\u4e00-\u9fa5a-zA-Z0-9_\- ()（）]{1,200}$";
    public const string COMPANY_NAME_MSG = "只能是中文、英文、数字、下划线、中划线、空格、括号";

    public const string COMPANY_CREDIT_CODE = @"^[A-Z0-9]{18}$";
    public const string COMPANY_CREDIT_CODE_MSG = "社会信用代码格式不正确";

    public const string PERSON_NAME = @"^[\u4e00-\u9fa5a-zA-Z]{1,20}$";
    public const string PERSON_NAME_MSG = "姓名格式不正确";

    public const string ADDRESS = @"^[\u4e00-\u9fa5a-zA-Z0-9_\- ()（）]{1,200}$";
    public const string ADDRESS_MSG = "地址格式不正确";

    public const string FILE_PATH = @"^[a-z0-9/._]+$";
    public const string FILE_PATH_MSG = "文件路径格式不正确";

    public const string WEBSITE = "^(http:\\/\\/|https:\\/\\/)?((www\\.)?[a-zA-Z0-9-_.]+\\.[a-zA-Z]+)(:\\d+)?(\\/[a-zA-Z\\d.\\-_]*)*([a-zA-Z.!@#$%&=-_'\":,?\\d*)(]*)?$";
    public const string WEBSITE_MSG = "网址格式不正确";

    public const string CITY = @"^[\u4e00-\u9fa5/]+$";
    public const string CITY_MSG = "城市格式不正确";

    public const string TEL = @"^((\d{3,4}-|\d{3,4})?\d{7,8})|(\d{3}-\d{3}-\d{4})|(\d{3}-\d{4}-\d{3})$";
    public const string TEL_MSG = "电话格式不正确";

    public const string APPID = @"^wx[a-zA-Z0-9]{16}$";
    public const string APPID_MSG = "AppId格式不正确";

    public const string LETTER_NUMBER = @"^[a-zA-Z0-9]+$";
    public const string LETTER_NUMBER_MSG = "只能是字母和数字";

    public const string LETTER = @"^[a-z]+$";
    public const string LETTER_MSG = "只能是小写字母";

    public const string MAIL = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
    public const string MAIL_MSG = "邮箱格式不正确";

    public const string VARIABLE = @"^[$_a-zA-Z][$_a-zA-Z0-9]*$";
    public const string VARIABLE_MSG = "只能是字母和数字";

    public const string USERNAME = @"^[a-zA-Z0-9_]{1,20}$";
    public const string USERNAME_MSG = "用户名格式不正确";
}