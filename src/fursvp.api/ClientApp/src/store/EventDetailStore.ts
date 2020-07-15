import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import { FursvpEvent, Member, FormPrompt, FormResponses } from './FursvpEvents';
import { getStoredVerifiedEmail, getStoredAuthToken, UserLoggedInAction, UserLoggedOutAction, OpenLoginModalAction } from './UserStore';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface EventDetailState {
    isLoading: boolean;
    id?: string;
    fursvpEvent: FursvpEvent | undefined;
    modalIsOpen: boolean;
    modalMember: Member | undefined;
    requestedAsUser: string | undefined;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestFursvpEventAction {
    type: 'REQUEST_FURSVP_EVENT';
    id: string;
    requestedAsUser: string | undefined;
}

interface ReceiveFursvpEventAction {
    type: 'RECEIVE_FURSVP_EVENT';
    fursvpEvent: FursvpEvent;
    id: string;
    member: Member | undefined;
}

interface ToggleModalAction {
    type: 'TOGGLE_MEMBER_MODAL_ACTION';
}

interface OpenModalAction {
    type: 'OPEN_MEMBER_MODAL_ACTION';
    member: Member | undefined;
}

interface FursvpEventNotFoundAction {
    type: 'FURSVP_EVENT_NOT_FOUND';
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestFursvpEventAction | ReceiveFursvpEventAction | ToggleModalAction | OpenModalAction | FursvpEventNotFoundAction
    | UserLoggedOutAction | UserLoggedInAction | OpenLoginModalAction;

const getMemberById = (event: FursvpEvent, memberId: string | undefined): Member | undefined => {
    if (memberId === undefined) {
        return undefined;
    }

    for (let m of event.members) {
        if (m.id === memberId) {
            return m;
        }
    }

    return undefined;
}

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
export const actionCreators = {
    requestFursvpEvent: (eventId: string, memberId: string | undefined): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState && appState.targetEvent) {

            var userEmail = getStoredVerifiedEmail();

            if (eventId !== appState.targetEvent.id || appState.targetEvent.requestedAsUser != userEmail) {

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

                fetch(`api/event/${eventId}`, requestOptions)
                    .then(response => {
                        if (!response.ok) {
                            throw new Error();
                        }

                        return response.json() as Promise<FursvpEvent>;
                    })
                    .then(data => {
                        if (data === undefined) {
                            throw new Error();
                        }
                        dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: eventId, member: getMemberById(data, memberId) });
                    })
                    .catch(err => {
                        dispatch({ type: 'FURSVP_EVENT_NOT_FOUND' });
                    });

                dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });
            }
            else if (appState.targetEvent.fursvpEvent !== undefined && memberId !== undefined) {
                //Same event is already loaded, memberId provided
                var member = getMemberById(appState.targetEvent.fursvpEvent, memberId);
                if (member !== undefined) {
                    dispatch({ type: 'OPEN_MEMBER_MODAL_ACTION', member });
                }
                else {
                    //Handle member 404
                    dispatch({ type: 'OPEN_MEMBER_MODAL_ACTION', member: undefined })
                }
            }
            else if (appState.targetEvent.modalIsOpen) {
                //Same event is not yet loaded or member is not specified, and modal is open for some reason
                dispatch({ type: 'TOGGLE_MEMBER_MODAL_ACTION' });
            }
        }
    },

    toggleModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'TOGGLE_MEMBER_MODAL_ACTION' });
    },

    openModal: (member: Member): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_MEMBER_MODAL_ACTION', member: member });
    }
};

const unloadedState: EventDetailState = {
    fursvpEvent: undefined,
    isLoading: true,
    modalIsOpen: false,
    modalMember: undefined,
    requestedAsUser: undefined
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
export const reducer: Reducer<EventDetailState> = (state: EventDetailState | undefined, incomingAction: Action): EventDetailState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_FURSVP_EVENT':
            return {
                ...state,
                isLoading: true,
                id: action.id,
                requestedAsUser: action.requestedAsUser
            };
        case 'RECEIVE_FURSVP_EVENT':
            return {
                ...state,
                fursvpEvent: action.fursvpEvent,
                isLoading: false,
                id: action.id,
                modalIsOpen: state.modalIsOpen || action.member !== undefined,
                modalMember: action.member !== undefined ? action.member : state.modalMember
            };
        case 'TOGGLE_MEMBER_MODAL_ACTION':
            return {
                ...state,
                modalIsOpen: !state.modalIsOpen,
            };
        case 'OPEN_MEMBER_MODAL_ACTION':
            return {
                ...state,
                modalIsOpen: true,
                modalMember: action.member
            };
        case 'FURSVP_EVENT_NOT_FOUND':
            return {
                ...state,
                isLoading: false
            }
        case 'USER_LOGGED_OUT_ACTION':
            return {
                ...state,
                modalMember: undefined,
                fursvpEvent: undefined,
                isLoading: true
            }
        default:
            return state;
    }
};
