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
var UserStore_1 = require("./UserStore");
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
exports.actionCreators = {
    requestFursvpEvents: function () { return function (dispatch, getState) {
        // Only load data if it's something we don't already have (and are not already loading)
        var appState = getState();
        if (appState && appState.fursvpEvents) {
            var userEmail = UserStore_1.getStoredVerifiedEmail();
            var authToken = UserStore_1.getStoredAuthToken();
            var requestOptions = undefined;
            if (authToken !== undefined) {
                requestOptions = {
                    method: 'GET',
                    credentials: "include",
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + authToken
                    }
                };
            }
            fetch("api/event", requestOptions)
                .then(function (response) { return response.json(); })
                .then(function (data) {
                dispatch({ type: 'RECEIVE_FURSVP_EVENTS', events: data });
            });
            dispatch({ type: 'REQUEST_FURSVP_EVENTS', requestedAsUser: userEmail });
        }
    }; }
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var unloadedState = {
    events: [],
    isLoading: false,
    requestedAsUser: undefined
};
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return unloadedState;
    }
    var action = incomingAction;
    switch (action.type) {
        case 'REQUEST_FURSVP_EVENTS':
            return __assign(__assign({}, state), { isLoading: true, requestedAsUser: action.requestedAsUser });
        case 'RECEIVE_FURSVP_EVENTS':
            return __assign(__assign({}, state), { events: action.events, isLoading: false });
        case 'USER_LOGGED_OUT_ACTION':
            return __assign(__assign({}, state), { events: [], isLoading: true });
        default:
            return state;
    }
};
//# sourceMappingURL=FursvpEvents.js.map