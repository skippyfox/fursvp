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
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var UserStore_1 = require("./UserStore");
var luxon_1 = require("luxon");
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
    }; },
    addEventButtonClicked: function () { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_CREATE_NEW_EVENT_MODAL' });
    }; },
    toggleCreateNewEventModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'TOGGLE_CREATE_NEW_EVENT_MODAL' });
    }; },
    openLoginModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_LOGIN_MODAL_ACTION' });
    }; },
    createNewEvent: function (values, history) { return function (dispatch, getState) {
        var authToken = UserStore_1.getStoredAuthToken();
        var userEmail = UserStore_1.getStoredVerifiedEmail();
        if (authToken === undefined || userEmail === undefined) {
            return;
        }
        var userEmailString = userEmail;
        dispatch({ type: 'SUBMITTING_NEW_EVENT' });
        var requestOptions = {
            method: 'POST',
            credentials: "include",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            },
            body: JSON.stringify({
                "authorName": values.authorName,
                "timeZoneId": luxon_1.DateTime.local().zoneName
            })
        };
        fetch("api/event", requestOptions)
            .then(function (response) { return response.json(); })
            .then(function (event) {
            dispatch({ type: 'NEW_EVENT_CREATED', event: event, requestedAsUser: userEmailString });
            if (history) {
                history.push("/event/" + event.id + "/edit");
            }
        });
    }; }
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var unloadedState = {
    events: [],
    isLoading: false,
    requestedAsUser: undefined,
    isCreateNewEventModalOpen: false,
    isSubmitting: false
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
        case 'OPEN_CREATE_NEW_EVENT_MODAL':
            return __assign(__assign({}, state), { isCreateNewEventModalOpen: true });
        case 'TOGGLE_CREATE_NEW_EVENT_MODAL':
            return __assign(__assign({}, state), { isCreateNewEventModalOpen: false });
        case 'SUBMITTING_NEW_EVENT':
            return __assign(__assign({}, state), { isSubmitting: true });
        case 'NEW_EVENT_CREATED':
            return __assign(__assign({}, state), { isSubmitting: false, events: __spreadArrays([action.event], state.events) });
        default:
            return state;
    }
};
//# sourceMappingURL=FursvpEvents.js.map