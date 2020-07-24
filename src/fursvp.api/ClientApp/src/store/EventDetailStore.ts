import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import { FursvpEvent, Member, FormPrompt, FormResponses } from './FursvpEvents';
import { getStoredVerifiedEmail, getStoredAuthToken, UserLoggedInAction, UserLoggedOutAction, OpenLoginModalAction } from './UserStore';
import { FormikValues } from 'formik';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface EventDetailState {
    isLoading: boolean;
    id?: string;
    fursvpEvent: FursvpEvent | undefined;
    modalIsOpen: boolean;
    modalMember: Member | undefined;
    requestedAsUser: string | undefined;
    modalIsInEditMode: boolean;
    isSaving: boolean;
    isAskingForRemoveRsvpConfirmation: boolean;
    rsvpRemovedModalIsOpen: boolean;
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

interface OpenNewMemberModalAction {
    type: 'OPEN_NEW_MEMBER_MODAL';
}

interface OpenEditExistingMemberModalAction {
    type: 'OPEN_EDIT_EXISTING_MEMBER_MODAL';
}

interface SavingMemberAction {
    type: 'SAVING_MEMBER';
}

interface NewMemberAddedAction {
    type: 'NEW_MEMBER_ADDED';
}

interface MemberEditedAction {
    type: 'MEMBER_EDITED';
}

interface AskForRemoveRsvpAction {
    type: 'ASK_FOR_REMOVE_RSVP_CONFIRMATION';
}

interface RemovingRsvpAction {
    type: 'REMOVING_RSVP';
}

interface RsvpRemovedAction {
    type: 'RSVP_REMOVED';
}

interface ToggleRsvpRemovedModalAction {
    type: 'TOGGLE_RSVP_REMOVED_MODAL';
}

interface ToggleRemoveRsvpModalAction {
    type: 'TOGGLE_REMOVE_RSVP_MODAL_ACTION';
}

interface CancelEditMemberAction {
    type: 'CANCEL_EDIT_MEMBER';
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestFursvpEventAction | ReceiveFursvpEventAction | ToggleModalAction | OpenModalAction | FursvpEventNotFoundAction
    | UserLoggedOutAction | UserLoggedInAction | OpenLoginModalAction
    | OpenNewMemberModalAction | OpenEditExistingMemberModalAction
    | SavingMemberAction | NewMemberAddedAction | MemberEditedAction | CancelEditMemberAction
    | AskForRemoveRsvpAction | RemovingRsvpAction | RsvpRemovedAction | ToggleRemoveRsvpModalAction | ToggleRsvpRemovedModalAction;

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
            else if (appState.targetEvent.modalIsOpen && !appState.targetEvent.modalIsInEditMode) {
                //Same event is not yet loaded or member is not specified, and modal is open for some reason
                dispatch({ type: 'TOGGLE_MEMBER_MODAL_ACTION' });
            }
        }
    },

    toggleModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'TOGGLE_MEMBER_MODAL_ACTION' });
    },

    toggleRemoveRsvpModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'TOGGLE_REMOVE_RSVP_MODAL_ACTION' });
    },

    toggleRsvpRemovedModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'TOGGLE_RSVP_REMOVED_MODAL' });
    },

    cancelEditMember: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'CANCEL_EDIT_MEMBER' });
    },
    
    askForRemoveRsvpConfirmation: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'ASK_FOR_REMOVE_RSVP_CONFIRMATION' });
    },

    removeRsvp: (eventId: string, memberId: string | undefined): AppThunkAction<KnownAction> => (dispatch, getState) => {
        if (memberId === undefined) {
            return;
        }

        var authToken = getStoredAuthToken();
        if (authToken === undefined) {
            return;
        }

        dispatch({ type: 'REMOVING_RSVP' });

        var deleteRequestOptions: RequestInit = {
            method: 'DELETE',
            credentials: "include",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            }
        };

        fetch(`api/event/${eventId}/member/${memberId}`, deleteRequestOptions)
            .then(response => {
                if (response.ok) {
                    dispatch({ type: 'RSVP_REMOVED' });
                }
                else {
                    throw new Error();
                }
            })
            .then(() => {
                var userEmail = getStoredVerifiedEmail();
                dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });

                var getRequestOptions: RequestInit = {
                    method: 'GET',
                    credentials: "include",
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + authToken
                    }
                };

                return fetch(`api/event/${eventId}`, getRequestOptions);
            })
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
                // Handle error
            });
    },

    openLoginModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_LOGIN_MODAL_ACTION' });
    },

    openModal: (member: Member): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_MEMBER_MODAL_ACTION', member: member });
    },

    openNewMemberModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_NEW_MEMBER_MODAL' });
    },

    openEditExistingMemberModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_EDIT_EXISTING_MEMBER_MODAL' });
    },

    editExistingMember: (memberId : string, values: FormikValues): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var state = getState();
        if (state.targetEvent === undefined) {
            return;
        }

        var authToken = getStoredAuthToken();
        if (authToken === undefined) {
            return;
        }

        dispatch({ type: 'SAVING_MEMBER' });

        var eventForm = state.targetEvent.fursvpEvent ? state.targetEvent.fursvpEvent.form : undefined;
        
        const editRequestOptions: RequestInit = {
            method: 'PUT',
            credentials: "include",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            },
            body: JSON.stringify({
                "isOrganizer": false, // TODO
                "isAttending": true, // TODO
                "emailAddress": values["editMemberEmail"],
                "name": values["editMemberName"],
                "formResponses": collectFormResponses(values, eventForm, "editPrompt")
            })
        };

        var eventId = state.targetEvent.id !== undefined ? state.targetEvent.id : "";

        fetch(`api/event/${eventId}/member/${memberId}`, editRequestOptions)
            .then(response => {
                if (response.ok) {
                    dispatch({ type: 'MEMBER_EDITED' });
                }
                else {
                    // Handle error
                }
            })
            .then(() => {
                var userEmail = getStoredVerifiedEmail();
                dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });

                var getRequestOptions: RequestInit = {
                    method: 'GET',
                    credentials: "include",
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + authToken
                    }
                };

                return fetch(`api/event/${eventId}`, getRequestOptions);
            })
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
                dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: eventId, member: undefined });
            })
            .catch(err => {
                // Handle error
            });
    },

    addNewMember: (values : FormikValues): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var state = getState();
        if (state.targetEvent === undefined) {
            return;
        }

        dispatch({ type: 'SAVING_MEMBER' });

        var eventForm = state.targetEvent.fursvpEvent ? state.targetEvent.fursvpEvent.form : undefined;

        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                "emailAddress": values["newMemberEmail"],
                "name": values["newMemberName"],
                "formResponses": collectFormResponses(values, eventForm, "newPrompt")
            })
        };

        var eventId = state.targetEvent.id !== undefined ? state.targetEvent.id : "";

        fetch(`api/event/${eventId}/member`, requestOptions)
            .then(response => {
                if (response.ok) {
                    dispatch({ type: 'NEW_MEMBER_ADDED' });
                }
                else {
                    // Handle error
                }
            })
            .then(() => {
                var userEmail = getStoredVerifiedEmail();
                var authToken = getStoredAuthToken();
                dispatch({ type: 'REQUEST_FURSVP_EVENT', id: eventId, requestedAsUser: userEmail });

                var getRequestOptions: RequestInit = {
                    method: 'GET',
                    credentials: "include",
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + authToken
                    }
                };

                return fetch(`api/event/${eventId}`, getRequestOptions);
            })
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
                dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: eventId, member: undefined });
            })
            .catch(err => {
                // Handle error
            });
    }
};

function collectFormResponses(values : FormikValues, eventForm : FormPrompt[] | undefined, keyPrefix : string): FormResponses[] {
    if (eventForm === undefined) {
        return [];
    }

    console.log(values);

    var result: FormResponses[] = [];

    for (var prompt of eventForm) {
        var responses : string[] = [];

        var key = keyPrefix + prompt.id;

        if (prompt.behavior == "Checkboxes") {
            for (var option in prompt.options) {
                if (values[key + option] && values[key + option] === true) {
                    responses.push(option);
                }
            }
        }
        else if (values[key]) {
            responses.push(values[key]);
        }

        result.push({ promptId: prompt.id, responses });
    }

    return result;
}

const unloadedState: EventDetailState = {
    fursvpEvent: undefined,
    isLoading: true,
    modalIsOpen: false,
    modalMember: undefined,
    requestedAsUser: undefined,
    modalIsInEditMode: false,
    isSaving: false,
    isAskingForRemoveRsvpConfirmation: false,
    rsvpRemovedModalIsOpen: false
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
                modalIsInEditMode: false
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
        case 'OPEN_NEW_MEMBER_MODAL':
            return {
                ...state,
                modalIsOpen: true,
                modalMember: undefined,
                modalIsInEditMode: true
            };
        case 'OPEN_EDIT_EXISTING_MEMBER_MODAL':
            return {
                ...state,
                modalIsOpen: true,
                modalIsInEditMode: true
            };
        case 'SAVING_MEMBER':
            return {
                ...state,
                isSaving: true
            };
        case 'MEMBER_EDITED':
            return {
                ...state,
                isLoading: true,
                isSaving: false,
                modalIsInEditMode: false
            };
        case 'NEW_MEMBER_ADDED':
            return {
                ...state,
                isSaving: false,
                modalIsOpen: false,
                modalIsInEditMode: false
            };
        case 'ASK_FOR_REMOVE_RSVP_CONFIRMATION':
            return {
                ...state,
                isAskingForRemoveRsvpConfirmation: true
            }
        case 'REMOVING_RSVP':
            return {
                ...state,
                isAskingForRemoveRsvpConfirmation: false,
                isSaving: true,
            }
        case 'RSVP_REMOVED':
            return {
                ...state,
                isLoading: true,
                isSaving: false,
                isAskingForRemoveRsvpConfirmation: false,
                rsvpRemovedModalIsOpen: true
            }
        case 'TOGGLE_RSVP_REMOVED_MODAL':
            return {
                ...state,
                modalIsOpen: false,
                modalMember: undefined,
                modalIsInEditMode: false,
                rsvpRemovedModalIsOpen: false
            }
        case 'TOGGLE_REMOVE_RSVP_MODAL_ACTION':
            return {
                ...state,
                isAskingForRemoveRsvpConfirmation: false
            }
        case 'TOGGLE_MEMBER_MODAL_ACTION':
            return {
                ...state,
                isAskingForRemoveRsvpConfirmation: false
            };
        case 'CANCEL_EDIT_MEMBER':
            return {
                ...state,
                modalIsInEditMode: false
            };
        default:
            return state;
    }
};
