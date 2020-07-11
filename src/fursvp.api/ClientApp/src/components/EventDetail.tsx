import * as React from 'react';
import { Badge, Container, UncontrolledTooltip } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as EventDetailStore from '../store/EventDetailStore';
import * as FursvpEventsStore from '../store/FursvpEvents';
import DateTime from './DateTime';

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

    public toolTip(text: string, toolTipText: string, id: string): JSX.Element {
        return <>
            <span id={id}>{text}</span>
            <UncontrolledTooltip target={id}>{toolTipText}</UncontrolledTooltip>
        </>;
    }

    public memberTypeEmoji(member: FursvpEventsStore.Member) : JSX.Element {
        var id = "member_" + member.id;

        if (member.isOrganizer) {
            return this.toolTip("⭐", "Organizer", id);
        }

        if (member.isAttending) {
            return this.toolTip("🧑", "Attending", id);
        }

        if (member.isAuthor) {
            return this.toolTip("⭐", "Author", id);
        }

        return <></>;
    }

    public emailToolTip(member: FursvpEventsStore.Member): JSX.Element {
        if (member.emailAddress === undefined || member.emailAddress === null) {
            return <></>;
        }

        return this.toolTip("✉", member.emailAddress, "emailAddress_" + member.id);
    }

    public render() {
        if (this.props.fursvpEvent !== undefined) {
            var event = this.props.fursvpEvent;

            let padlock = <></>;
            if (!event.isPublished) {
                padlock = <>
                    <span id="privateEventIndicator" role="img" aria-label="private">🔒</span>
                    <UncontrolledTooltip target="privateEventIndicator">Private Event</UncontrolledTooltip>
                    </>
            }

            return (
                <React.Fragment>
                    <h1 id="tabelLabel">                        
                        {event.name}{padlock}
                    </h1>
                    <Container>
                        <span className="text-muted">Starts</span> <DateTime date={event.startsAt} timeZoneId={event.timeZoneId} id="eventDetail_startsAt" />
                    </Container>
                    <Container>
                        <span className="text-muted">Ends</span> <DateTime date={event.endsAt} timeZoneId={event.timeZoneId} id="eventDetail_endsAt" />
                    </Container>
                    <Container>
                        <span className="text-muted">Location</span> {event.location}
                    </Container>
                    <Container>
                        {event.otherDetails}
                    </Container>
                    <Container>
                        {event.members.map((member: FursvpEventsStore.Member) =>
                            <div key={member.id} className="container-fluid">
                                {this.memberTypeEmoji(member)}
                                {member.name}
                                {this.emailToolTip(member)}
                            </div>
                        )}
                    </Container>
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