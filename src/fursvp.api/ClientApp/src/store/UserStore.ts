import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';

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
interface OpenLoginModalAction {
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

interface VerificationCodeWasSuccessfulAction {
    type: 'VERIFICATION_CODE_WAS_SUCCESSFUL_ACTION';
    emailAddress: string;
}

interface VerificationCodeWasUnsuccessfulAction {
    type: 'VERIFICATION_CODE_WAS_UNSUCCESSFUL_ACTION';
}

interface UserLoggedOutAction {
    type: 'USER_LOGGED_OUT_ACTION';
}

interface OpenUserInfoModalAction {
    type: 'OPEN_USER_INFO_MODAL_ACTION';
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = OpenLoginModalAction | ToggleLoginModalAction
    | VerificationEmailIsSendingAction | VerificationEmailWasSentAction | VerificationEmailDidNotSendAction
    | VerificationCodeIsSendingAction | VerificationCodeWasSuccessfulAction | VerificationCodeWasUnsuccessfulAction
    | UserLoggedOutAction | OpenUserInfoModalAction;

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
                dispatch({ type: 'VERIFICATION_CODE_WAS_SUCCESSFUL_ACTION', emailAddress });
                localStorage.setItem("verifiedEmail", emailAddress);
                localStorage.setItem("token", token);
            })
            .catch(error => {
                dispatch({ type: 'VERIFICATION_CODE_WAS_UNSUCCESSFUL_ACTION' })
            });
    },

    logOut: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        localStorage.removeItem("verifiedEmail");
        localStorage.removeItem("token");
        dispatch({ type: 'USER_LOGGED_OUT_ACTION' });
    }
};

function getVerifiedEmailFromLocalStorage(): string | undefined {
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
    verifiedEmail: getVerifiedEmailFromLocalStorage(),
    verificationCodeIsSending: false,
    userInfoModalIsOpen: false
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
export const reducer: Reducer<UserState> = (state: UserState | undefined, incomingAction: Action): UserState => {
    if (state === undefined) {
        return unloadedState;
    }

    console.log('User Reducer: ' + incomingAction.type);

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'TOGGLE_LOGIN_MODAL_ACTION':
            return {
                loginModalIsOpen: false,
                verificationEmailIsSending: state.verificationEmailIsSending,
                emailBeingVerified: state.emailBeingVerified,
                verifyModalIsOpen: false,
                verifiedEmail: state.verifiedEmail,
                verificationCodeIsSending: state.verificationCodeIsSending,
                userInfoModalIsOpen: false
            };
        case 'OPEN_LOGIN_MODAL_ACTION':
            return {
                loginModalIsOpen: true,
                verificationEmailIsSending: state.verificationEmailIsSending,
                emailBeingVerified: state.emailBeingVerified,
                verifyModalIsOpen: false,
                verifiedEmail: state.verifiedEmail,
                verificationCodeIsSending: state.verificationCodeIsSending,
                userInfoModalIsOpen: false
            };
        case 'VERIFICATION_EMAIL_IS_SENDING_ACTION':
            return {
                loginModalIsOpen: true,
                verificationEmailIsSending: true,
                emailBeingVerified: action.emailAddress,
                verifyModalIsOpen: false,
                verifiedEmail: state.verifiedEmail,
                verificationCodeIsSending: state.verificationCodeIsSending,
                userInfoModalIsOpen: false
            };
        case 'VERIFICATION_EMAIL_WAS_SENT_ACTION':
            return {
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                emailBeingVerified: state.emailBeingVerified,
                verifyModalIsOpen: true,
                verifiedEmail: state.verifiedEmail,
                verificationCodeIsSending: state.verificationCodeIsSending,
                userInfoModalIsOpen: false
            };
        case 'VERIFICATION_EMAIL_DID_NOT_SEND_ACTION':
            return {
                loginModalIsOpen: true,
                verificationEmailIsSending: false,
                emailBeingVerified: state.emailBeingVerified,
                verifyModalIsOpen: false,
                verifiedEmail: state.verifiedEmail,
                verificationCodeIsSending: state.verificationCodeIsSending,
                userInfoModalIsOpen: false
            }
        case 'VERIFICATION_CODE_IS_SENDING_ACTION':
            return {
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                emailBeingVerified: state.emailBeingVerified,
                verifyModalIsOpen: true,
                verifiedEmail: state.verifiedEmail,
                verificationCodeIsSending: true,
                userInfoModalIsOpen: false
            };
        case 'VERIFICATION_CODE_WAS_SUCCESSFUL_ACTION':
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
                loginModalIsOpen: false,
                verificationEmailIsSending: false,
                emailBeingVerified: state.emailBeingVerified,
                verifyModalIsOpen: true,
                verifiedEmail: state.verifiedEmail,
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
                loginModalIsOpen: false,
                verificationEmailIsSending: state.verificationEmailIsSending,
                emailBeingVerified: state.emailBeingVerified,
                verifyModalIsOpen: false,
                verifiedEmail: state.verifiedEmail,
                verificationCodeIsSending: state.verificationCodeIsSending,
                userInfoModalIsOpen: true
            }
        default:
            return state;
    }
};
