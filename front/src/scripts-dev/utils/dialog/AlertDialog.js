/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */

import DialogBase from "./DialogBase";

class AlertDialog extends DialogBase {

    //当前的所有属性
    #props = null;

    //当前的配置
    #config = null;

    constructor(props) {
        super(props);
        this.#props = props;
        this.#config = props.config;
        //合并状态
        this.state = Object.assign(this.state, {});
    }

    /**
     * 第一次挂载后
     * 注意: 被DialogBase的componentDidMount接管并调用
     */
    onReady() {

    }

    /**
     * 渲染
     * @returns {JSX.Element}
     */
    render() {
        return (<div className="ns-layer alert-layer">
            <div className={`nlc ${this.state.show ? 'show' : ''} ${(this.#config.onOk || this.#config.onCancel) ? 'has-btn' : ''}`}>
                <div className="nlc-inner" style={this.#config.style && this.#config.style.bg ? {
                    backgroundColor: this.#config.style.bg,
                } : null}>
                    {
                        this.#config.title && this.#config.title !== '' ? <div className="title">
                            {
                                this.#config.title
                            }
                        </div> : null
                    }
                    <div className="content" style={this.#config.style && this.#config.style.content ? {
                        color: this.#config.style.content,
                    } : null}>
                        <div className="nlc-content" dangerouslySetInnerHTML={
                            {__html: this.#config.content}
                        }>
                        </div>
                    </div>
                    {
                        this.#config.onOk || this.#config.onCancel ? <div className="btns">
                            {
                                this.#config.onOk ? <a href="#" className="btn btn-ok hover-btn" onClick={(e) => {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    if (!this.$helper.allowClick("ok-btn")) {
                                        return;
                                    }
                                    let result = true;
                                    if (this.#config.onOk) {
                                        result = this.#config.onOk();
                                    }
                                    if (result !== false) {
                                        this.$helper.closeLayer(null, true, this.#config._elId);
                                    }
                                }}>
                                    {
                                        this.#config.okText ? this.#config.okText : "确定"
                                    }
                                </a> : null
                            }
                            {
                                this.#config.onCancel ? <a href="#" className="btn btn-cancel" onClick={(e) => {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    if (!this.$helper.allowClick("cancel-btn")) {
                                        return;
                                    }
                                    let result = true;
                                    if (this.#config.onCancel) {
                                        result = this.#config.onCancel();
                                    }
                                    if (result !== false) {
                                        this.$helper.closeLayer(null, true, this.#config._elId);
                                    }
                                }}>
                                    {
                                        this.#config.cancelText ? this.#config.cancelText : "取消"
                                    }
                                </a> : null
                            }
                        </div> : null
                    }
                </div>
            </div>
        </div>);
    }
}

export default AlertDialog
