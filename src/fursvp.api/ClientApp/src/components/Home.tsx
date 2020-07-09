import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as FursvpEventsStore from '../store/FursvpEvents';

// At runtime, Redux will merge together...
type FursvpEventProps =
    FursvpEventsStore.FursvpEventsState // ... state we've requested from the Redux store
    & typeof FursvpEventsStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{ }>; // ... plus incoming routing parameters


class Home extends React.PureComponent<FursvpEventProps> {
    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    // This method is called when the route parameters change
    public componentDidUpdate() {
        this.ensureDataFetched();
    }

    public render() {
        return (
            <React.Fragment>
                <h1 id="tabelLabel">Upcoming Events</h1>
                <br />
                {this.renderEventsTable()}
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        this.props.requestFursvpEvents();
    }

    private renderEventsTable() {
        return (
            <>
                {this.props.events.map((event: FursvpEventsStore.FursvpEvent) =>
                    <div key={event.id} className="container-fluid">
                        <small>{event.startsAt} | {event.location} &nbsp;</small>
                        <span className="badge badge-info">{event.members.length}</span>
                        <p>
                            <h5>{event.name}</h5>
                        </p>
                    </div>
                )}
            </>
        );
    }
}

export default connect(
    (state: ApplicationState) => state.fursvpEvents, // Selects which state properties are merged into the component's props
    FursvpEventsStore.actionCreators // Selects which action creators are merged into the component's props
)(Home as any);