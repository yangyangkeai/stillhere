/**
 * Copyright (C) 2026 Tingyang Zhang
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see https://www.gnu.org/licenses/.
 */


/**
 * 错误显示页面, 一般是 500 404
 */
class ErrorPanel extends React.Component {
    render() {
        return (
            <div className="ns-content ml-auto error-content">
                <div className="error-panel ml-auto">
                    <div className={'icon icon-' + (this.props.icon && this.props.icon !== "" ? this.props.icon : "none")}/>
                    <div className="text" dangerouslySetInnerHTML={
                        {__html: this.props.text}
                    }>
                    </div>
                    {
                        this.props.btn ? (
                            <div className="btn">
                                <a href="#" onClick={(e) => {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    this.props.btn.call();
                                }}>{this.props.btn.text}</a>
                            </div>
                        ) : ""
                    }
                </div>
            </div>
        );
    }
}

export default ErrorPanel
