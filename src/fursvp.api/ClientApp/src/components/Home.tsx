import * as React from 'react';
import { Badge, ListGroup, ListGroupItem, ListGroupItemHeading, ListGroupItemText, UncontrolledTooltip } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as FursvpEventsStore from '../store/FursvpEvents';
import DateTime from './DateTime';

// At runtime, Redux will merge together...
type FursvpEventProps =
    FursvpEventsStore.FursvpEventsState // ... state we've requested from the Redux store
    & typeof FursvpEventsStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters


class Home extends React.PureComponent<FursvpEventProps> {
    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public render() {
        return (
            <React.Fragment>
                <h1 id="tabelLabel">Upcoming Events</h1>
                <br />
                {this.listEvents()}
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        this.props.requestFursvpEvents();
    }

    private listEvents() {
        return (
            <ListGroup>
                <ListGroupItem active tag="button" action>
                    Add an event
                </ListGroupItem>
                {this.props.events.map((event: FursvpEventsStore.FursvpEvent) => {

                    let padlock = <></>;
                    if (!event.isPublished) {
                        padlock = <>
                            <span id="privateEventIndicator" role="img" aria-label="This event is visible only to you.">🔒</span>
                            <UncontrolledTooltip target="privateEventIndicator">This event is visible only to you.</UncontrolledTooltip>
                        </>
                    }

                    return <ListGroupItem key={event.id} tag="button" action onClick={this.showEvent.bind(this, event)}>
                        <ListGroupItemHeading>{event.name}{padlock}&nbsp;<Badge color="info">{event.members.length}</Badge></ListGroupItemHeading>
                        <ListGroupItemText>
                            <DateTime date={event.startsAtLocal} timeZoneOffset={event.timeZoneOffset} id={"home_startsAt_" + event.id} />
                            <br />{event.location}
                        </ListGroupItemText>
                    </ListGroupItem>
                }
                )}
            </ListGroup>
        );
    }

    private showEvent(event: FursvpEventsStore.FursvpEvent) {
        this.props.history.push('/event/' + event.id);
    }
}

export default connect(
    (state: ApplicationState) => state.fursvpEvents, // Selects which state properties are merged into the component's props
    FursvpEventsStore.actionCreators // Selects which action creators are merged into the component's props
)(Home as any);