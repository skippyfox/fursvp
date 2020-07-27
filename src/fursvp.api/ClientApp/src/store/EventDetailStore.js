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
var getMemberById = function (event, memberId) {
    if (memberId === undefined) {
        return undefined;
    }
    for (var _i = 0, _a = event.members; _i < _a.length; _i++) {
        var m = _a[_i];
        if (m.id === memberId) {
            return m;
        }
    }
    return undefined;
};
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
exports.actionCreators = {
    requestFursvpEvent: function (eventId, memberId) { return function (dispatch, getState) {
        // Only load data if it's something we don't already have (and are not already loading)
        var appState = getState();
        if (appState && appState.targetEvent) {
            var userEmail = UserStore_1.getStoredVerifiedEmail();
            if (eventId !== appState.targetEvent.id || appState.targetEvent.requestedAsUser !== userEmail) {
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
                fetch("api/event/" + eventId, requestOptions)
                    .then(function (response) {
                    if (!response.ok) {
                        throw new Error();
                    }
                    return response.json();
                })
                    .then(function (data) {
                    if (data === undefined) {
                        throw new Error();
                    }
                    dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: eventId, member: getMemberById(data, memberId) });
                })
                    .catch(function (err) {
                    dispatch({ type: 'FURSVP_EVENT_NOT_FOUND' });
                });
                dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });
            }
            else if (appState.targetEvent.fursvpEvent !== undefined && memberId !== undefined) {
                //Same event is already loaded, memberId provided
                var member = getMemberById(appState.targetEvent.fursvpEvent, memberId);
                if (member !== undefined) {
                    dispatch({ type: 'OPEN_MEMBER_MODAL_ACTION', member: member });
                }
                else {
                    //Handle member 404
                    dispatch({ type: 'OPEN_MEMBER_MODAL_ACTION', member: undefined });
                }
            }
            else if (appState.targetEvent.modalIsOpen && !appState.targetEvent.modalIsInEditMode) {
                //Same event is not yet loaded or member is not specified, and modal is open for some reason
                dispatch({ type: 'TOGGLE_MEMBER_MODAL_ACTION' });
            }
        }
    }; },
    toggleModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'TOGGLE_MEMBER_MODAL_ACTION' });
    }; },
    toggleRemoveRsvpModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'TOGGLE_REMOVE_RSVP_MODAL_ACTION' });
    }; },
    toggleRsvpRemovedModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'TOGGLE_RSVP_REMOVED_MODAL' });
    }; },
    cancelEditMember: function () { return function (dispatch, getState) {
        dispatch({ type: 'CANCEL_EDIT_MEMBER' });
    }; },
    askForRemoveRsvpConfirmation: function () { return function (dispatch, getState) {
        dispatch({ type: 'ASK_FOR_REMOVE_RSVP_CONFIRMATION' });
    }; },
    removeRsvp: function (eventId, memberId) { return function (dispatch, getState) {
        if (memberId === undefined) {
            return;
        }
        var authToken = UserStore_1.getStoredAuthToken();
        if (authToken === undefined) {
            return;
        }
        dispatch({ type: 'REMOVING_RSVP' });
        var deleteRequestOptions = {
            method: 'DELETE',
            credentials: "include",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            }
        };
        fetch("api/event/" + eventId + "/member/" + memberId, deleteRequestOptions)
            .then(function (response) {
            if (response.ok) {
                dispatch({ type: 'RSVP_REMOVED' });
            }
            else {
                throw new Error();
            }
        })
            .then(function () {
            var userEmail = UserStore_1.getStoredVerifiedEmail();
            dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });
            var getRequestOptions = {
                method: 'GET',
                credentials: "include",
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + authToken
                }
            };
            return fetch("api/event/" + eventId, getRequestOptions);
        })
            .then(function (response) {
            if (!response.ok) {
                throw new Error();
            }
            return response.json();
        })
            .then(function (data) {
            if (data === undefined) {
                throw new Error();
            }
            dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: eventId, member: getMemberById(data, memberId) });
        })
            .catch(function (err) {
            // Handle error
        });
    }; },
    openLoginModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_LOGIN_MODAL_ACTION' });
    }; },
    openModal: function (member) { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_MEMBER_MODAL_ACTION', member: member });
    }; },
    openNewMemberModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_NEW_MEMBER_MODAL' });
    }; },
    openEditExistingMemberModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_EDIT_EXISTING_MEMBER_MODAL' });
    }; },
    editExistingMember: function (member, values) { return function (dispatch, getState) {
        var state = getState();
        if (state.targetEvent === undefined) {
            return;
        }
        var authToken = UserStore_1.getStoredAuthToken();
        if (authToken === undefined) {
            return;
        }
        dispatch({ type: 'SAVING_MEMBER' });
        var eventForm = state.targetEvent.fursvpEvent ? state.targetEvent.fursvpEvent.form : undefined;
        var editRequestOptions = {
            method: 'PUT',
            credentials: "include",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            },
            body: JSON.stringify({
                "emailAddress": values.editMemberEmail,
                "name": values.editMemberName,
                "isOrganizer": values.editMemberIsOrganizer,
                "isAttending": values.editMemberIsAttending || (!values.editMemberIsOrganizer && !member.isAuthor),
                "formResponses": collectFormResponses(values, eventForm, "editPrompt")
            })
        };
        var eventId = state.targetEvent.id !== undefined ? state.targetEvent.id : "";
        fetch("api/event/" + eventId + "/member/" + member.id, editRequestOptions)
            .then(function (response) {
            if (response.ok) {
                dispatch({ type: 'MEMBER_EDITED' });
            }
            else {
                // Handle error
            }
        })
            .then(function () {
            var userEmail = UserStore_1.getStoredVerifiedEmail();
            dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });
            var getRequestOptions = {
                method: 'GET',
                credentials: "include",
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + authToken
                }
            };
            return fetch("api/event/" + eventId, getRequestOptions);
        })
            .then(function (response) {
            if (!response.ok) {
                throw new Error();
            }
            return response.json();
        })
            .then(function (data) {
            if (data === undefined) {
                throw new Error();
            }
            dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: eventId, member: undefined });
        })
            .catch(function (err) {
            // Handle error
        });
    }; },
    addNewMember: function (values) { return function (dispatch, getState) {
        var state = getState();
        if (state.targetEvent === undefined) {
            return;
        }
        dispatch({ type: 'SAVING_MEMBER' });
        var eventForm = state.targetEvent.fursvpEvent ? state.targetEvent.fursvpEvent.form : undefined;
        var requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                "emailAddress": values.newMemberEmail,
                "name": values.newMemberName,
                "isOrganizer": values.isOrganizer,
                "isAttending": values.isAttending || !values.isOrganizer,
                "formResponses": collectFormResponses(values, eventForm, "newPrompt")
            })
        };
        var eventId = state.targetEvent.id !== undefined ? state.targetEvent.id : "";
        fetch("api/event/" + eventId + "/member", requestOptions)
            .then(function (response) {
            if (response.ok) {
                dispatch({ type: 'NEW_MEMBER_ADDED' });
            }
            else {
                // Handle error
            }
        })
            .then(function () {
            var userEmail = UserStore_1.getStoredVerifiedEmail();
            var authToken = UserStore_1.getStoredAuthToken();
            dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });
            var getRequestOptions = {
                method: 'GET',
                credentials: "include",
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + authToken
                }
            };
            return fetch("api/event/" + eventId, getRequestOptions);
        })
            .then(function (response) {
            if (!response.ok) {
                throw new Error();
            }
            return response.json();
        })
            .then(function (data) {
            if (data === undefined) {
                throw new Error();
            }
            dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: eventId, member: undefined });
        })
            .catch(function (err) {
            // Handle error
        });
    }; }
};
function collectFormResponses(values, eventForm, keyPrefix) {
    if (eventForm === undefined) {
        return [];
    }
    console.log(values);
    var result = [];
    for (var _i = 0, eventForm_1 = eventForm; _i < eventForm_1.length; _i++) {
        var prompt = eventForm_1[_i];
        var responses = [];
        var key = keyPrefix + prompt.id;
        if (prompt.behavior === "Checkboxes") {
            if (prompt.options.length === 1) {
                // Formik records a single checkbox as a boolean
                if (values[key] && values[key] === true) {
                    responses.push(prompt.options[0]);
                }
            }
            else {
                // Formik records checkbox groups as a string array.
                responses = values[key];
            }
        }
        else if (values[key]) {
            // Formik records input text and dropdown selections as strings.
            responses.push(values[key]);
        }
        result.push({ promptId: prompt.id, responses: responses });
    }
    return result;
}
function getActingMember(memberList, emailAddress) {
    if (!emailAddress) {
        return undefined;
    }
    for (var _i = 0, memberList_1 = memberList; _i < memberList_1.length; _i++) {
        var member = memberList_1[_i];
        if (member.emailAddress === emailAddress) {
            return member;
        }
    }
    return undefined;
}
var unloadedState = {
    fursvpEvent: undefined,
    isLoading: true,
    modalIsOpen: false,
    modalMember: undefined,
    requestedAsUser: undefined,
    modalIsInEditMode: false,
    isSaving: false,
    isAskingForRemoveRsvpConfirmation: false,
    rsvpRemovedModalIsOpen: false,
    actingMember: undefined
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return unloadedState;
    }
    var action = incomingAction;
    switch (action.type) {
        case 'REQUEST_FURSVP_EVENT':
            return __assign(__assign({}, state), { isLoading: true, id: action.id, requestedAsUser: action.requestedAsUser });
        case 'RECEIVE_FURSVP_EVENT':
            return __assign(__assign({}, state), { fursvpEvent: __assign(__assign({}, action.fursvpEvent), { members: action.fursvpEvent.members.sort(function (m) { return new Date(m.rsvpedAtUtc).getTime(); }).reverse() }), isLoading: false, id: action.id, modalIsOpen: state.modalIsOpen || action.member !== undefined, modalMember: action.member !== undefined ? action.member : state.modalMember, actingMember: getActingMember(action.fursvpEvent.members, state.requestedAsUser) });
        case 'TOGGLE_MEMBER_MODAL_ACTION':
            return __assign(__assign({}, state), { modalIsOpen: !state.modalIsOpen, modalIsInEditMode: false, isAskingForRemoveRsvpConfirmation: false });
        case 'OPEN_MEMBER_MODAL_ACTION':
            return __assign(__assign({}, state), { modalIsOpen: true, modalMember: action.member });
        case 'FURSVP_EVENT_NOT_FOUND':
            return __assign(__assign({}, state), { isLoading: false });
        case 'USER_LOGGED_OUT_ACTION':
            return __assign(__assign({}, state), { modalMember: undefined, fursvpEvent: undefined, isLoading: true, actingMember: undefined });
        case 'OPEN_NEW_MEMBER_MODAL':
            return __assign(__assign({}, state), { modalIsOpen: true, modalMember: undefined, modalIsInEditMode: true });
        case 'OPEN_EDIT_EXISTING_MEMBER_MODAL':
            return __assign(__assign({}, state), { modalIsOpen: true, modalIsInEditMode: true });
        case 'SAVING_MEMBER':
            return __assign(__assign({}, state), { isSaving: true });
        case 'MEMBER_EDITED':
            return __assign(__assign({}, state), { isLoading: true, isSaving: false, modalIsInEditMode: false });
        case 'NEW_MEMBER_ADDED':
            return __assign(__assign({}, state), { isSaving: false, modalIsOpen: false, modalIsInEditMode: false });
        case 'ASK_FOR_REMOVE_RSVP_CONFIRMATION':
            return __assign(__assign({}, state), { isAskingForRemoveRsvpConfirmation: true });
        case 'REMOVING_RSVP':
            return __assign(__assign({}, state), { isAskingForRemoveRsvpConfirmation: false, isSaving: true });
        case 'RSVP_REMOVED':
            return __assign(__assign({}, state), { isLoading: true, isSaving: false, isAskingForRemoveRsvpConfirmation: false, rsvpRemovedModalIsOpen: true });
        case 'TOGGLE_RSVP_REMOVED_MODAL':
            return __assign(__assign({}, state), { modalIsOpen: false, modalMember: undefined, modalIsInEditMode: false, rsvpRemovedModalIsOpen: false });
        case 'TOGGLE_REMOVE_RSVP_MODAL_ACTION':
            return __assign(__assign({}, state), { isAskingForRemoveRsvpConfirmation: false });
        case 'CANCEL_EDIT_MEMBER':
            return __assign(__assign({}, state), { modalIsInEditMode: false });
        case 'NEW_EVENT_CREATED':
            return __assign(__assign({}, state), { fursvpEvent: action.event, isLoading: false, id: action.event.id, modalIsOpen: false, modalMember: undefined, requestedAsUser: action.requestedAsUser, actingMember: getActingMember(action.event.members, action.requestedAsUser) });
        default:
            return state;
    }
};
//# sourceMappingURL=EventDetailStore.js.map