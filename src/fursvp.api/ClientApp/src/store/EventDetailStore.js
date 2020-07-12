"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
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
            if (eventId !== appState.targetEvent.id) {
                fetch("api/event/" + eventId)
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
                dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId });
            }
            else if (appState.targetEvent.fursvpEvent !== undefined && memberId !== undefined) {
                //Same event is already loaded, memberId provided
                var member = getMemberById(appState.targetEvent.fursvpEvent, memberId);
                if (member !== undefined) {
                    dispatch({ type: 'OPEN_MODAL_ACTION', member: member });
                }
                else {
                    //Handle member 404
                    dispatch({ type: 'OPEN_MODAL_ACTION', member: undefined });
                }
            }
            else if (appState.targetEvent.modalIsOpen) {
                //Same event is not yet loaded or member is not specified, and modal is open for some reason
                dispatch({ type: 'TOGGLE_MODAL_ACTION' });
            }
        }
    }; },
    toggleModal: function () { return function (dispatch, getState) {
        dispatch({ type: 'TOGGLE_MODAL_ACTION' });
    }; },
    openModal: function (member) { return function (dispatch, getState) {
        dispatch({ type: 'OPEN_MODAL_ACTION', member: member });
    }; }
};
var unloadedState = {
    fursvpEvent: undefined,
    isLoading: true,
    modalIsOpen: false,
    modalMember: undefined
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
            return {
                fursvpEvent: state.fursvpEvent,
                isLoading: true,
                id: action.id,
                modalIsOpen: state.modalIsOpen,
                modalMember: state.modalMember
            };
        case 'RECEIVE_FURSVP_EVENT':
            return {
                fursvpEvent: action.fursvpEvent,
                isLoading: false,
                id: action.id,
                modalIsOpen: state.modalIsOpen || action.member !== undefined,
                modalMember: action.member !== undefined ? action.member : state.modalMember
            };
        case 'TOGGLE_MODAL_ACTION':
            return {
                fursvpEvent: state.fursvpEvent,
                isLoading: state.isLoading,
                id: state.id,
                modalIsOpen: !state.modalIsOpen,
                modalMember: state.modalMember
            };
        case 'OPEN_MODAL_ACTION':
            return {
                fursvpEvent: state.fursvpEvent,
                isLoading: state.isLoading,
                id: state.id,
                modalIsOpen: true,
                modalMember: action.member
            };
        case 'FURSVP_EVENT_NOT_FOUND':
            return {
                fursvpEvent: state.fursvpEvent,
                isLoading: false,
                id: state.id,
                modalIsOpen: state.modalIsOpen,
                modalMember: state.modalMember
            };
        default:
            return state;
    }
};
//# sourceMappingURL=EventDetailStore.js.map