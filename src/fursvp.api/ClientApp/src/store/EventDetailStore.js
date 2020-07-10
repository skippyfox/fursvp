"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
exports.actionCreators = {
    requestFursvpEvent: function (id) { return function (dispatch, getState) {
        // Only load data if it's something we don't already have (and are not already loading)
        var appState = getState();
        if (appState && appState.targetEvent) {
            fetch("api/event/" + id)
                .then(function (response) { return response.json(); })
                .then(function (data) {
                dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data });
            });
            dispatch({ type: 'REQUEST_FURSVP_EVENT' });
        }
    }; }
};
// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var unloadedState = { fursvpEvent: undefined, isLoading: false };
exports.reducer = function (state, incomingAction) {
    if (state === undefined) {
        return unloadedState;
    }
    var action = incomingAction;
    switch (action.type) {
        case 'REQUEST_FURSVP_EVENT':
            return {
                fursvpEvent: state.fursvpEvent,
                isLoading: true
            };
        case 'RECEIVE_FURSVP_EVENT':
            return {
                fursvpEvent: action.fursvpEvent,
                isLoading: false
            };
        default:
            return state;
    }
};
//# sourceMappingURL=EventDetailStore.js.map