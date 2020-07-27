import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import { getStoredVerifiedEmail, getStoredAuthToken, UserLoggedOutAction, OpenLoginModalAction } from './UserStore';
import { ReceiveFursvpEventAction } from './EventDetailStore';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface FursvpEventsState {
    isLoading: boolean;
    events: FursvpEvent[];
    requestedAsUser: string | undefined;
    isCreateNewEventModalOpen: boolean;
    isSubmitting: boolean;
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
    rsvpClosesInMs: number | null;
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

export interface RequestFursvpEventsAction {
    type: 'REQUEST_FURSVP_EVENTS';
    requestedAsUser: string | undefined;
}

export interface ReceiveFursvpEventsAction {
    type: 'RECEIVE_FURSVP_EVENTS';
    events: FursvpEvent[];
}

interface ToggleCreateNewEventModalAction {
    type: 'TOGGLE_CREATE_NEW_EVENT_MODAL';
}

interface OpenCreateNewEventModalAction {
    type: 'OPEN_CREATE_NEW_EVENT_MODAL';
}

export interface NewEventCreatedAction {
    type: 'NEW_EVENT_CREATED',
    event: FursvpEvent,
    requestedAsUser: string
}

interface SubmittingNewEventAction {
    type: 'SUBMITTING_NEW_EVENT'
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestFursvpEventsAction | ReceiveFursvpEventsAction | UserLoggedOutAction
    | ToggleCreateNewEventModalAction | OpenCreateNewEventModalAction | OpenLoginModalAction
    | ReceiveFursvpEventAction | SubmittingNewEventAction | NewEventCreatedAction;

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
    },

    addEventButtonClicked: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_CREATE_NEW_EVENT_MODAL' });
    },

    toggleCreateNewEventModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'TOGGLE_CREATE_NEW_EVENT_MODAL' });
    },

    openLoginModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_LOGIN_MODAL_ACTION' });
    },

    createNewEvent: (values: any, actionOnSuccess: (event: FursvpEvent) => void): AppThunkAction<KnownAction> => (dispatch, getState) => {

        var authToken = getStoredAuthToken();
        var userEmail = getStoredVerifiedEmail();
        if (authToken === undefined || userEmail === undefined) {
            return;
        }

        var userEmailString: string = userEmail;

        dispatch({ type: 'SUBMITTING_NEW_EVENT' });

        var requestOptions : RequestInit = {
            method: 'POST',
            credentials: "include",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            },
            body: JSON.stringify({
                "authorName": values.authorName
            })
        };

        fetch(`api/event`, requestOptions)
            .then(response => response.json() as Promise<FursvpEvent>)
            .then(event => {
                dispatch({ type: 'NEW_EVENT_CREATED', event, requestedAsUser: userEmailString });

                if (actionOnSuccess) {
                    actionOnSuccess(event);
                }
            });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: FursvpEventsState = {
    events: [],
    isLoading: false,
    requestedAsUser: undefined,
    isCreateNewEventModalOpen: false,
    isSubmitting: false
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
            };
        case 'OPEN_CREATE_NEW_EVENT_MODAL':
            return {
                ...state,
                isCreateNewEventModalOpen: true
            };
        case 'TOGGLE_CREATE_NEW_EVENT_MODAL':
            return {
                ...state,
                isCreateNewEventModalOpen: false
            };
        case 'SUBMITTING_NEW_EVENT':
            return {
                ...state,
                isSubmitting: true
            }
        case 'NEW_EVENT_CREATED':
            return {
                ...state,
                isSubmitting: false
            }
        default:
            return state;
    }
};
