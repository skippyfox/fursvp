import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import { RequestFursvpEventAction, ReceiveFursvpEventAction } from './EventDetailStore';
import { FursvpEvent, RequestFursvpEventsAction, ReceiveFursvpEventsAction } from './FursvpEvents'

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface UserState {
    loginModalIsOpen: boolean;
    verificationEmailIsSending: boolean;
    emailBeingVerified: string | undefined;
    verifyModalIsOpen: boolean;
    verifiedEmail: string | undefined;
    verificationCodeIsSending: boolean;
    userInfoModalIsOpen: boolean;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
export interface OpenLoginModalAction {
    type: 'OPEN_LOGIN_MODAL_ACTION';
}

interface ToggleLoginModalAction {
    type: 'TOGGLE_LOGIN_MODAL_ACTION';
}

interface VerificationEmailIsSendingAction {
    type: 'VERIFICATION_EMAIL_IS_SENDING_ACTION';
    emailAddress: string;
}

interface VerificationEmailWasSentAction {
    type: 'VERIFICATION_EMAIL_WAS_SENT_ACTION';
}

interface VerificationEmailDidNotSendAction {
    type: 'VERIFICATION_EMAIL_DID_NOT_SEND_ACTION';
}

interface VerificationCodeIsSendingAction {
    type: 'VERIFICATION_CODE_IS_SENDING_ACTION';
}

export interface UserLoggedInAction {
    type: 'USER_LOGGED_IN_ACTION';
    emailAddress: string;
}

interface VerificationCodeWasUnsuccessfulAction {
    type: 'VERIFICATION_CODE_WAS_UNSUCCESSFUL_ACTION';
}

export interface UserLoggedOutAction {
    type: 'USER_LOGGED_OUT_ACTION';
}

interface OpenUserInfoModalAction {
    type: 'OPEN_USER_INFO_MODAL_ACTION';
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = OpenLoginModalAction | ToggleLoginModalAction
    | VerificationEmailIsSendingAction | VerificationEmailWasSentAction | VerificationEmailDidNotSendAction
    | VerificationCodeIsSendingAction | UserLoggedInAction | VerificationCodeWasUnsuccessfulAction
    | UserLoggedOutAction | OpenUserInfoModalAction
    | RequestFursvpEventAction | ReceiveFursvpEventAction
    | RequestFursvpEventsAction | ReceiveFursvpEventsAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
export const actionCreators = {
    toggleModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'TOGGLE_LOGIN_MODAL_ACTION' });
    },

    openLoginModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_LOGIN_MODAL_ACTION' });
    },

    openUserInfoModal: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'OPEN_USER_INFO_MODAL_ACTION' });
    },

    sendVerificationEmail: (emailAddress: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'VERIFICATION_EMAIL_IS_SENDING_ACTION', emailAddress });

        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ "emailAddress": emailAddress })
        };

        fetch('api/auth/sendverificationcode', requestOptions)
            .then(response => {
                if (response && response.ok) {
                    dispatch({ type: 'VERIFICATION_EMAIL_WAS_SENT_ACTION' });
                }
                else {
                    dispatch({ type: 'VERIFICATION_EMAIL_DID_NOT_SEND_ACTION'})
                }
            });
    },

    checkVerificationCode: (emailAddress: string | undefined, code: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        if (emailAddress === undefined) {
            return;
        }

        dispatch({ type: 'VERIFICATION_CODE_IS_SENDING_ACTION' });

        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                "emailAddress": emailAddress,
                "verificationCode": code
            })
        };

        fetch('api/auth/verifyemail', requestOptions)
            .then(response => {
                if (!response.ok) {
                    throw new Error(response.statusText);
                }

                return response.text();
            })
            .then(token => {
                dispatch({ type: 'USER_LOGGED_IN_ACTION', emailAddress });

                localStorage.setItem("verifiedEmail", emailAddress);
                localStorage.setItem("token", token);

                {
                    dispatch({ type: 'REQUEST_FURSVP_EVENTS', requestedAsUser: emailAddress });

                    var getRequestAllEventsOptions: RequestInit = {
                        method: 'GET',
                        credentials: "include",
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + token
                        }
                    };

                    fetch(`api/event`, getRequestAllEventsOptions)
                        .then(response => response.json() as Promise<FursvpEvent[]>)
                        .then(data => {
                            dispatch({ type: 'RECEIVE_FURSVP_EVENTS', events: data });
                        });
                }

                var state = getState();
                if (state.targetEvent && state.targetEvent.id) {
                    dispatch({ type: 'REQUEST_FURSVP_EVENT', id: state.targetEvent.id, requestedAsUser: emailAddress });

                    var getRequestEventOptions: RequestInit = {
                        method: 'GET',
                        credentials: "include",
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + token
                        }
                    };

                    return fetch(`api/event/${state.targetEvent.id}`, getRequestEventOptions)
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

                            if (state.targetEvent && state.targetEvent.id) {
                                dispatch({ type: 'RECEIVE_FURSVP_EVENT', fursvpEvent: data, id: state.targetEvent.id, member: undefined });
                            }
                        });
                }
            })
            .catch(error => {
                dispatch({ type: 'VERIFICATION_CODE_WAS_UNSUCCESSFUL_ACTION' })
            });
    },

    logOut: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        localStorage.removeItem("verifiedEmail");
        localStorage.removeItem("token");
        dispatch({ type: 'USER_LOGGED_OUT_ACTION' });
        
        const requestOptions : RequestInit = {
            method: 'DELETE',
            credentials: "include",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + getStoredAuthToken()
            }
        };

        fetch("api/auth/logout", requestOptions);

        dispatch({ type: 'REQUEST_FURSVP_EVENTS', requestedAsUser: undefined });
        
        fetch(`api/event`)
            .then(response => response.json() as Promise<FursvpEvent[]>)
            .then(data => {
                dispatch({ type: 'RECEIVE_FURSVP_EVENTS', events: data });
            });
    }
};

export function getStoredAuthToken(): string | undefined {
    var token = localStorage.getItem("token");

    if (token == null) {
        return undefined;
    }

    return token;
}

export function getStoredVerifiedEmail(): string | undefined {
    var email = localStorage.getItem("verifiedEmail");

    if (email == null) {
        return undefined;
    }

    return email;
}

const unloadedState: UserState = {
    loginModalIsOpen: false,
    verificationEmailIsSending: false,
    emailBeingVerified: undefined,
    verifyModalIsOpen: false,
    verifiedEmail: getStoredVerifiedEmail(),
    verificationCodeIsSending: false,
    userInfoModalIsOpen: false
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
export const reducer: Reducer<UserState> = (state: UserState | undefined, incomingAction: Action): UserState => {
    if (state === undefined) {
        return unloadedState;
    }

    //console.log('User Reducer: ' + incomingAction.type);

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'TOGGLE_LOGIN_MODAL_ACTION':
            return {
                ...state,
                loginModalIsOpen: false,
                verifyModalIsOpen: false,
                userInfoModalIsOpen: false
            };
        case 'OPEN_LOGIN_MODAL_ACTION':
            return {
                ...state,
                loginModalIsOpen: true,
                verifyModalIsOpen: false,
                userInfoModalIsOpen: false
            };
        case 'VERIFICATION_EMAIL_IS_SENDING_ACTION':
            return {
                ...state,
                loginModalIsOpen: true,
                verificationEmailIsSending: true,
                emailBeingVerified: action.emailAddress,
                verifyModalIsOpen: false,
                userInfoModalIsOpen: false
            };
        case 'VERIFICATION_EMAIL_WAS_SENT_ACTION':
            return {
                ...state,
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                verifyModalIsOpen: true,
                userInfoModalIsOpen: false
            };
        case 'VERIFICATION_EMAIL_DID_NOT_SEND_ACTION':
            return {
                ...state,
                loginModalIsOpen: true,
                verificationEmailIsSending: false,
                verifyModalIsOpen: false,
                userInfoModalIsOpen: false
            }
        case 'VERIFICATION_CODE_IS_SENDING_ACTION':
            return {
                ...state,
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                verifyModalIsOpen: true,
                verificationCodeIsSending: true,
                userInfoModalIsOpen: false
            };
        case 'USER_LOGGED_IN_ACTION':
            return {
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                emailBeingVerified: undefined,
                verifyModalIsOpen: false,
                verifiedEmail: action.emailAddress,
                verificationCodeIsSending: false,
                userInfoModalIsOpen: true
            };
        case 'VERIFICATION_CODE_WAS_UNSUCCESSFUL_ACTION':
            return {
                ...state,
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                verifyModalIsOpen: true,
                verificationCodeIsSending: false,
                userInfoModalIsOpen: false
            }
        case 'USER_LOGGED_OUT_ACTION':
            return {
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                emailBeingVerified: undefined,
                verifyModalIsOpen: false,
                verifiedEmail: undefined,
                verificationCodeIsSending: false,
                userInfoModalIsOpen: true
            }
        case 'OPEN_USER_INFO_MODAL_ACTION':
            return {
                ...state,
                loginModalIsOpen: false,
                verifyModalIsOpen: false,
                userInfoModalIsOpen: true
            }
        default:
            return state;
    }
};
