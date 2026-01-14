/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

import Frame from "../comm/Frame";

/**
 * 首页主体
 */
class Main extends React.Component {

    /**
     * 构造函数
     * @param props
     */
    constructor(props) {
        super(props);
        this.state = {}
    }

    /**
     * 第一次挂载后
     */
    componentDidMount() {
        this.#init();
    }

    /**
     * 初始化
     */
    #init() {

    }

    /**
     * 组件销毁时
     */
    componentWillUnmount() {

    }

    /**
     * render
     * return {*}
     */
    render() {
        return <Frame className="home">
            <div className="home-content">
                <div className="logo">
                    <img src={this.$helper.urlContent("~/images/logo.png")} width={184}/>
                    <div className="text">还在么</div>
                </div>
                <div className="beian" style={{
                    color: "#fff",
                }}>
                    © 2026 Tingyang Zhang. This project is licensed under the AGPL-3.0.
                </div>
            </div>
        </Frame>
    }

}

export default Main
