import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import { FursvpEvent, Member, FormPrompt, FormResponses } from './FursvpEvents';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface EventDetailState {
    isLoading: boolean;
    fursvpEvent: FursvpEvent | undefined;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestFursvpEventAction {
    type: 'REQUEST_FURSVP_EVENT';
}

interface ReceiveFursvpEventAction {
    type: 'RECEIVE_FURSVP_EVENT';
    fursvpEvent: FursvpEvent;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestFursvpEventAction | ReceiveFursvpEventAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestFursvpEvent: (id: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState && appState.targetEvent) {
            fetch(`api/event/${id}`)
                .then(response => response.json() as Promise<FursvpEvent>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data });
                });

            dispatch({ type: 'REQUEST_FURSVP_EVENT' });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: EventDetailState = { fursvpEvent: undefined, isLoading: false };

export const reducer: Reducer<EventDetailState> = (state: EventDetailState | undefined, incomingAction: Action): EventDetailState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
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
