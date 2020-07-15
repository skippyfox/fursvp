"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
Object.defineProperty(exports, "__esModule", { value: true });
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
exports.actionCreators = {
    toggleModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'TOGGLE_LOGIN_MODAL_ACTION' });
    }; },
    openLoginModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_LOGIN_MODAL_ACTION' });
    }; },
    openUserInfoModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_USER_INFO_MODAL_ACTION' });
    }; },
    sendVerificationEmail: function (emailAddress) { return function (dispatch, getState) {
        dispatch({ type: 'VERIFICATION_EMAIL_IS_SENDING_ACTION', emailAddress: emailAddress });
        var requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ "emailAddress": emailAddress })
        };
        fetch('api/auth/sendverificationcode', requestOptions)
            .then(function (response) {
            if (response && response.ok) {
                dispatch({ type: 'VERIFICATION_EMAIL_WAS_SENT_ACTION' });
            }
            else {
                dispatch({ type: 'VERIFICATION_EMAIL_DID_NOT_SEND_ACTION' });
            }
        });
    }; },
    checkVerificationCode: function (emailAddress, code) { return function (dispatch, getState) {
        if (emailAddress === undefined) {
            return;
        }
        dispatch({ type: 'VERIFICATION_CODE_IS_SENDING_ACTION' });
        var requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                "emailAddress": emailAddress,
                "verificationCode": code
            })
        };
        fetch('api/auth/verifyemail', requestOptions)
            .then(function (response) {
            if (!response.ok) {
                throw new Error(response.statusText);
            }
            return response.text();
        })
            .then(function (token) {
            dispatch({ type: 'USER_LOGGED_IN_ACTION', emailAddress: emailAddress });
            localStorage.setItem("verifiedEmail", emailAddress);
            localStorage.setItem("token", token);
        })
            .catch(function (error) {
            dispatch({ type: 'VERIFICATION_CODE_WAS_UNSUCCESSFUL_ACTION' });
        });
    }; },
    logOut: function () { return function (dispatch, getState) {
        localStorage.removeItem("verifiedEmail");
        localStorage.removeItem("token");
        dispatch({ type: 'USER_LOGGED_OUT_ACTION' });
    }; }
};
function getStoredAuthToken() {
    var token = localStorage.getItem("token");
    if (token == null) {
        return undefined;
    }
    return token;
}
exports.getStoredAuthToken = getStoredAuthToken;
function getStoredVerifiedEmail() {
    var email = localStorage.getItem("verifiedEmail");
    if (email == null) {
        return undefined;
    }
    return email;
}
exports.getStoredVerifiedEmail = getStoredVerifiedEmail;
var unloadedState = {
    loginModalIsOpen: false,
    verificationEmailIsSending: false,
    emailBeingVerified: undefined,
    verifyModalIsOpen: false,
    verifiedEmail: getStoredVerifiedEmail(),
    verificationCodeIsSending: false,
    userInfoModalIsOpen: false
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return unloadedState;
    }
    //console.log('User Reducer: ' + incomingAction.type);
    var action = incomingAction;
    switch (action.type) {
        case 'TOGGLE_LOGIN_MODAL_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: false, verifyModalIsOpen: false, userInfoModalIsOpen: false });
        case 'OPEN_LOGIN_MODAL_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: true, verifyModalIsOpen: false, userInfoModalIsOpen: false });
        case 'VERIFICATION_EMAIL_IS_SENDING_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: true, verificationEmailIsSending: true, emailBeingVerified: action.emailAddress, verifyModalIsOpen: false, userInfoModalIsOpen: false });
        case 'VERIFICATION_EMAIL_WAS_SENT_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: false, verificationEmailIsSending: false, verifyModalIsOpen: true, userInfoModalIsOpen: false });
        case 'VERIFICATION_EMAIL_DID_NOT_SEND_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: true, verificationEmailIsSending: false, verifyModalIsOpen: false, userInfoModalIsOpen: false });
        case 'VERIFICATION_CODE_IS_SENDING_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: false, verificationEmailIsSending: false, verifyModalIsOpen: true, verificationCodeIsSending: true, userInfoModalIsOpen: false });
        case 'USER_LOGGED_IN_ACTION':
            return {
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                emailBeingVerified: undefined,
                verifyModalIsOpen: false,
                verifiedEmail: action.emailAddress,
                verificationCodeIsSending: false,
                userInfoModalIsOpen: true
            };
        case 'VERIFICATION_CODE_WAS_UNSUCCESSFUL_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: false, verificationEmailIsSending: false, verifyModalIsOpen: true, verificationCodeIsSending: false, userInfoModalIsOpen: false });
        case 'USER_LOGGED_OUT_ACTION':
            return {
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                emailBeingVerified: undefined,
                verifyModalIsOpen: false,
                verifiedEmail: undefined,
                verificationCodeIsSending: false,
                userInfoModalIsOpen: true
            };
        case 'OPEN_USER_INFO_MODAL_ACTION':
            return __assign(__assign({}, state), { loginModalIsOpen: false, verifyModalIsOpen: false, userInfoModalIsOpen: true });
        default:
            return state;
    }
};
//# sourceMappingURL=UserStore.js.map