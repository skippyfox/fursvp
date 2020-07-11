import * as React from 'react';
import { Badge } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as EventDetailStore from '../store/EventDetailStore';

// At runtime, Redux will merge together...
type EventDetailProps =
    EventDetailStore.EventDetailState // ... state we've requested from the Redux store
    & typeof EventDetailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{ id: string }>; // ... plus incoming routing parameters


class EventDetail extends React.PureComponent<EventDetailProps> {
    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    // This method is called when the route parameters change
    public componentDidUpdate() {
        this.ensureDataFetched();
    }

    public render() {
        if (this.props.fursvpEvent !== undefined) {
            return (
                <React.Fragment>
                    <h1 id="tabelLabel">{this.props.fursvpEvent.name}</h1>
                    <br />
                    <div key={this.props.fursvpEvent.id} className="container-fluid">
                        <small>{this.props.fursvpEvent.startsAt} | {this.props.fursvpEvent.location} &nbsp;</small>
                        <Badge color="info">{this.props.fursvpEvent.members.length}</Badge>
                    </div>
                </React.Fragment>
            );
        }
        else {
            return (
                <React.Fragment>
                    (Loading)
                </React.Fragment>
            );
        }
    }

    private ensureDataFetched() {
        this.props.requestFursvpEvent(this.props.match.params.id);
    }
}

export default connect(
    (state: ApplicationState) => state.targetEvent, // Selects which state properties are merged into the component's props
    EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail as any);