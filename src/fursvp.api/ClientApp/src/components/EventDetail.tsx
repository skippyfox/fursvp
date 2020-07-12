import * as React from 'react';
import { Container, UncontrolledTooltip, ListGroup, ListGroupItem, Modal, ModalHeader, ModalBody, ModalFooter, Button, ListGroupItemText, ListGroupItemHeading } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps, Redirect } from 'react-router';
import { ApplicationState } from '../store';
import * as EventDetailStore from '../store/EventDetailStore';
import * as FursvpEventsStore from '../store/FursvpEvents';
import DateTime from './DateTime';

// At runtime, Redux will merge together...
type EventDetailProps =
    EventDetailStore.EventDetailState // ... state we've requested from the Redux store
    & typeof EventDetailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{ eventId: string, memberId: string }>; // ... plus incoming routing parameters


class EventDetail extends React.PureComponent<EventDetailProps> {
    constructor(props : EventDetailProps) {
        super(props);

        this.toggleModal = this.toggleModal.bind(this);
    }

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
            var event = this.props.fursvpEvent;
            var member = this.props.modalMember;
            var responses: FursvpEventsStore.FormResponses[] = this.props.modalMember !== undefined ? this.props.modalMember.responses : [];

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
                        <ListGroup>
                            <ListGroupItem active tag="button" action>
                                Add an RSVP
                            </ListGroupItem>
                            {event.members.map((member: FursvpEventsStore.Member) =>
                                <ListGroupItem key={member.id} tag="button" action onClick={this.showMember.bind(this, member)}>
                                    {this.memberTypeEmoji(member)}
                                    {member.name}
                                </ListGroupItem>
                            )}
                        </ListGroup>
                    </Container>
                    <Modal isOpen={this.props.modalIsOpen} toggle={this.toggleModal}>
                        {member !== undefined ?
                            <>
                                <ModalHeader toggle={this.toggleModal}>{member.name}</ModalHeader>
                                <ModalBody>
                                    <ListGroup>
                                        {member.emailAddress ? <ListGroupItem>✉{member.emailAddress}</ListGroupItem> : <></>}
                                        <ListGroupItem>✔<DateTime date={member.rsvpedAt} timeZoneId={event.timeZoneId} id="eventDetail_memberModal_rsvpedAt" /></ListGroupItem>
                                        {this.matchResponsesToPrompts(responses, event.form).map(response =>
                                            <ListGroupItem>
                                                <ListGroupItemHeading>{response.prompt}</ListGroupItemHeading>
                                                {response.responses.map(individualResponse => <ListGroupItemText>{individualResponse}</ListGroupItemText>)}
                                            </ListGroupItem>
                                        )}
                                    </ListGroup>
                                </ModalBody>
                                <ModalFooter>
                                    <Button color="primary" onClick={this.toggleModal}>Edit</Button>{' '}
                                    <Button color="secondary" onClick={this.toggleModal}>Close</Button>
                                </ModalFooter>
                            </>
                            :
                            <>
                                <ModalHeader toggle={this.toggleModal}>Member Info Not Found</ModalHeader>
                                <ModalBody>Sorry! We couldn't find the member info you're looking for.</ModalBody>
                            </>
                        }
                    </Modal>
                </React.Fragment>
            );
        }
        else if (this.props.isLoading) {
            return (
                <React.Fragment>
                    (Loading)
                </React.Fragment>
            );
        }
        else {
            //Not loading and no event defined means we 404ed
            return <Redirect to="/" />;
        }
    }

    private matchResponsesToPrompts(responses: FursvpEventsStore.FormResponses[], prompts: FursvpEventsStore.FormPrompt[]): (FursvpEventsStore.FormResponses & FursvpEventsStore.FormPrompt)[] {
        var result: (FursvpEventsStore.FormResponses & FursvpEventsStore.FormPrompt)[] = [];
        for (let r of responses) {
            for (let p of prompts) {
                if (r.promptId == p.id) {
                    result.push({
                        id: p.id,
                        behavior: p.behavior,
                        options: p.options,
                        sortOrder: p.sortOrder,
                        prompt: p.prompt,
                        required: p.required,
                        promptId: r.promptId,
                        responses: r.responses
                    });
                }
            }
        }
        return result;
    }

    private toolTip(text: string, toolTipText: string, id: string): JSX.Element {
        return <>
            <span id={id}>{text}</span>
            <UncontrolledTooltip target={id}>{toolTipText}</UncontrolledTooltip>
        </>;
    }

    private memberTypeEmoji(member: FursvpEventsStore.Member): JSX.Element {
        var id = "member_" + member.id;

        if (member.isOrganizer) {
            return this.toolTip("⭐", "Organizer", id);
        }

        if (member.isAttending) {
            return <></>;
            //return this.toolTip("🧑", "Attending", id);
        }

        if (member.isAuthor) {
            return this.toolTip("⭐", "Author", id);
        }

        return <></>;
    }

    private emailToolTip(member: FursvpEventsStore.Member): JSX.Element {
        if (member.emailAddress === undefined || member.emailAddress === null) {
            return <></>;
        }

        return this.toolTip("✉", member.emailAddress, "emailAddress_" + member.id);
    }

    private showMember(member: FursvpEventsStore.Member) {
        this.props.history.push('/event/' + this.props.id + '/member/' + member.id);
        this.props.openModal(member);
    }

    private toggleModal() {
        if (this.props.modalIsOpen) {
            this.props.history.push('/event/' + this.props.id);
        }
        else if (this.props.modalMember !== undefined) {
            this.props.history.push('/event/' + this.props.id + '/member/' + this.props.modalMember.id);
        }
        else {
            return;
        }
    }

    private ensureDataFetched() {
        this.props.requestFursvpEvent(this.props.match.params.eventId, this.props.match.params.memberId);
    }
}

export default connect(
    (state: ApplicationState) => state.targetEvent, // Selects which state properties are merged into the component's props
    EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail as any);