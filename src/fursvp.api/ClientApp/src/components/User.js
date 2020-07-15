"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var reactstrap_1 = require("reactstrap");
var react_redux_1 = require("react-redux");
var UserStore = require("../store/UserStore");
var react_router_dom_1 = require("react-router-dom");
var User = /** @class */ (function (_super) {
    __extends(User, _super);
    function User(props) {
        var _this = _super.call(this, props) || this;
        _this.sendVerificationCode = _this.sendVerificationCode.bind(_this);
        _this.sendVerificationEmail = _this.sendVerificationEmail.bind(_this);
        return _this;
    }
    User.prototype.render = function () {
        return (React.createElement(React.Fragment, null,
            this.props.verifiedEmail == undefined
                ? React.createElement(reactstrap_1.NavLink, { tag: react_router_dom_1.Link, onClick: this.props.openLoginModal, className: "text-dark", to: "#" }, "Log In")
                : React.createElement(reactstrap_1.NavLink, { tag: react_router_dom_1.Link, onClick: this.props.openUserInfoModal, className: "text-dark", to: "#" }, "Account"),
            React.createElement(reactstrap_1.Modal, { isOpen: this.props.loginModalIsOpen, toggle: this.props.toggleModal },
                React.createElement(reactstrap_1.ModalHeader, { toggle: this.props.toggleModal }, "Log In"),
                React.createElement(reactstrap_1.ModalBody, null,
                    React.createElement(reactstrap_1.Form, null,
                        React.createElement(reactstrap_1.FormGroup, null,
                            React.createElement(reactstrap_1.Label, { for: "exampleEmail" }, "Email"),
                            React.createElement(reactstrap_1.Input, { type: "email", name: "email", id: "loginModalEmailAddress", disabled: this.props.verificationEmailIsSending })))),
                React.createElement(reactstrap_1.ModalFooter, null,
                    React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.sendVerificationEmail, disabled: this.props.verificationEmailIsSending }, "Send Verification Email"),
                    ' ',
                    React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.props.toggleModal, disabled: this.props.verificationEmailIsSending }, "Cancel"))),
            React.createElement(reactstrap_1.Modal, { isOpen: this.props.verifyModalIsOpen, toggle: this.props.toggleModal },
                React.createElement(reactstrap_1.ModalHeader, { toggle: this.props.toggleModal }, "Verify Email"),
                React.createElement(reactstrap_1.ModalBody, null,
                    "Enter the verification code we sent to ",
                    React.createElement("code", null, this.props.emailBeingVerified),
                    ".",
                    React.createElement(reactstrap_1.Form, null,
                        React.createElement(reactstrap_1.FormGroup, null,
                            React.createElement(reactstrap_1.Label, { for: "exampleEmail" }, "Verification Code"),
                            React.createElement(reactstrap_1.Input, { type: "email", name: "email", id: "verifyModalCode", disabled: this.props.verificationCodeIsSending })))),
                React.createElement(reactstrap_1.ModalFooter, null,
                    React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.sendVerificationCode, disabled: this.props.verificationCodeIsSending }, "Verify"),
                    ' ',
                    React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.props.toggleModal, disabled: this.props.verificationCodeIsSending }, "Cancel"))),
            React.createElement(reactstrap_1.Modal, { isOpen: this.props.userInfoModalIsOpen, toggle: this.props.toggleModal },
                React.createElement(reactstrap_1.ModalHeader, { toggle: this.props.toggleModal }, this.props.verifiedEmail === undefined ? "Not Logged In" : "Logged In"),
                React.createElement(reactstrap_1.ModalBody, null, this.props.verifiedEmail === undefined
                    ? React.createElement(React.Fragment, null, "You are not logged in.")
                    : React.createElement(React.Fragment, null,
                        "You are verified as ",
                        React.createElement("code", null, this.props.verifiedEmail),
                        ".")),
                React.createElement(reactstrap_1.ModalFooter, null,
                    React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.props.toggleModal, disabled: this.props.verificationCodeIsSending }, "Continue"),
                    ' ',
                    this.props.verifiedEmail === undefined ?
                        React.createElement(reactstrap_1.Button, { color: "warning", onClick: this.props.openLoginModal, disabled: this.props.verificationCodeIsSending }, "Log In")
                        :
                            React.createElement(reactstrap_1.Button, { outline: true, color: "danger", onClick: this.props.logOut, disabled: this.props.verificationCodeIsSending }, "Log Out")))));
    };
    User.prototype.sendVerificationEmail = function () {
        var emailAddress = document.getElementById('loginModalEmailAddress').value;
        this.props.sendVerificationEmail(emailAddress);
    };
    User.prototype.sendVerificationCode = function () {
        var code = document.getElementById('verifyModalCode').value;
        this.props.checkVerificationCode(this.props.emailBeingVerified, code);
    };
    return User;
}(React.PureComponent));
exports.default = react_redux_1.connect(function (state) { return state.user; }, // Selects which state properties are merged into the component's props
UserStore.actionCreators // Selects which action creators are merged into the component's props
)(User);
//# sourceMappingURL=User.js.map