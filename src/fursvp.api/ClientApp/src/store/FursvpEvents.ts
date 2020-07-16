import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import { getStoredVerifiedEmail, getStoredAuthToken, UserLoggedInAction, UserLoggedOutAction, OpenLoginModalAction } from './UserStore';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface FursvpEventsState {
    isLoading: boolean;
    events: FursvpEvent[];
    requestedAsUser: string | undefined;
}

export interface FursvpEvent {
    id: string;
    version: number;
    startsAtUtc: string;
    startsAtLocal: string;
    endsAtUtc: string;
    endsAtLocal: string;
    timeZoneId: string;
    timeZoneOffset: string;
    members: Member[];
    form: FormPrompt[];
    name: string;
    otherDetails: string;
    location: string;
    rsvpOpen: boolean;
    rsvpClosesAtUtc: string | null;
    rsvpClosesAtLocal: string | null;
    isPublished: boolean;
}

export interface Member {
    id: string;
    emailAddress: string | null;
    name: string;
    isAttending: boolean;
    isOrganizer: boolean;
    isAuthor: boolean;
    responses: FormResponses[];
    rsvpedAtUtc: string;
    rsvpedAtLocal: string;
}

export interface FormPrompt {
    id: string;
    behavior: string;
    prompt: string;
    required: boolean;
    options: string[];
    sortOrder: number;
}

export interface FormResponses {
    promptId: string;
    responses: string[];
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestFursvpEventsAction {
    type: 'REQUEST_FURSVP_EVENTS';
    requestedAsUser: string | undefined;
}

interface ReceiveFursvpEventsAction {
    type: 'RECEIVE_FURSVP_EVENTS';
    events: FursvpEvent[];
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestFursvpEventsAction | ReceiveFursvpEventsAction
    | UserLoggedOutAction | UserLoggedInAction | OpenLoginModalAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestFursvpEvents: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();

        if (appState && appState.fursvpEvents) {

            var userEmail = getStoredVerifiedEmail();
            var authToken = getStoredAuthToken();

            var requestOptions: RequestInit | undefined = undefined;
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

            fetch(`api/event`, requestOptions)
                .then(response => response.json() as Promise<FursvpEvent[]>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_FURSVP_EVENTS', events: data });
                });

            dispatch({ type: 'REQUEST_FURSVP_EVENTS', requestedAsUser: userEmail });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: FursvpEventsState = {
    events: [],
    isLoading: false,
    requestedAsUser: undefined
};

export const reducer: Reducer<FursvpEventsState> = (state: FursvpEventsState | undefined, incomingAction: Action): FursvpEventsState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_FURSVP_EVENTS':
            return {
                ...state,
                isLoading: true,
                requestedAsUser: action.requestedAsUser
            };
        case 'RECEIVE_FURSVP_EVENTS':
            return {
                ...state,
                events: action.events,
                isLoading: false
            };
        case 'USER_LOGGED_OUT_ACTION':
            return {
                ...state,
                events: [],
                isLoading: true
            }
        default:
            return state;
    }
};
