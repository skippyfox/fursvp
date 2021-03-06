import * as FursvpEvents from './FursvpEvents';
import * as EventDetail from './EventDetailStore';
import * as User from './UserStore';

// The top-level state object
export interface ApplicationState {
    fursvpEvents: FursvpEvents.FursvpEventsState | undefined;
    targetEvent: EventDetail.EventDetailState | undefined;
    user: User.UserState | undefined;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    fursvpEvents: FursvpEvents.reducer,
    targetEvent: EventDetail.reducer,
    user: User.reducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
