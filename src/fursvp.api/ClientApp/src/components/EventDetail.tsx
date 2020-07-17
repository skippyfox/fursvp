import * as React from 'react';
import { Container, UncontrolledTooltip, ListGroup, ListGroupItem, Modal, ModalHeader, ModalBody, ModalFooter, Button, ListGroupItemText, ListGroupItemHeading } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps, Redirect } from 'react-router';
import { ApplicationState } from '../store';
import * as EventDetailStore from '../store/EventDetailStore';
import * as FursvpEventsStore from '../store/FursvpEvents';
import DateTime from './DateTime';
import { getStoredVerifiedEmail } from '../store/UserStore';
import { Member } from '../store/FursvpEvents';

// At runtime, Redux will merge together...
type EventDetailProps =
    EventDetailStore.EventDetailState // ... state we've requested from the Redux store
    & typeof EventDetailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{ eventId: string, memberId: string }>; // ... plus incoming routing parameters


class EventDetail extends React.PureComponent<EventDetailProps> {
    constructor(props : EventDetailProps) {
        super(props);

        this.toggleModal = this.toggleModal.bind(this);
        this.openNewMemberModal = this.openNewMemberModal.bind(this);
    }

    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    // This method is called when the route parameters change
    public componentDidUpdate() {
        this.ensureDataFetched();
    }

    private canEditMember(userEmail:string | undefined): boolean {

        if (this.props.modalMember === undefined) {
            return false;
        }

        if (userEmail === undefined) {
            return false;
        }

        if (this.props.modalMember.emailAddress == userEmail) {
            return true;
        }

        var author = this.getAuthor();

        if (author !== undefined && author.emailAddress == userEmail) {
            return true;
        }

        return false;
    }

    private getAuthor(): Member | undefined {
        if (this.props.fursvpEvent === undefined) {
            return undefined;
        }

        for (var member of this.props.fursvpEvent.members) {
            if (member.isAuthor) {
                return member;
            }
        }

        return undefined;
    }

    private renderViewOnlyModalContent(
        event: FursvpEventsStore.FursvpEvent,
        member: Member | undefined,
        responses: FursvpEventsStore.FormResponses[],
        userEmail: string | undefined)
        : React.ReactNode {

        if (member === undefined) {
            return <>
                <ModalHeader toggle={this.toggleModal}>Member Info Not Found</ModalHeader>
                <ModalBody>Sorry! We couldn't find the member info you're looking for.</ModalBody>
            </>;
        }

        return <>
            <ModalHeader toggle={this.toggleModal}>{member.name}</ModalHeader>
            <ModalBody>
                <ListGroup>
                    {member.emailAddress ? <ListGroupItem>✉{member.emailAddress}</ListGroupItem> : <></>}
                    <ListGroupItem>
                        {member.isOrganizer ? "⭐ Organizer" : ""}
                        {!member.isOrganizer && member.isAttending ? "🧑 Attending" : ""}
                        {!member.isOrganizer && !member.isAttending && member.isAuthor ? "⭐ Author" : ""}
                    </ListGroupItem>
                    {this.matchResponsesToPrompts(responses, event.form).map(response =>
                        <ListGroupItem key={response.promptId}>
                            <ListGroupItemHeading>{response.prompt}</ListGroupItemHeading>
                            {response.responses.map(individualResponse => <ListGroupItemText key={individualResponse}>{individualResponse}</ListGroupItemText>)}
                        </ListGroupItem>
                    )}
                    <ListGroupItem>✔<DateTime date={member.rsvpedAtLocal} timeZoneOffset={event.timeZoneOffset} id="eventDetail_memberModal_rsvpedAt" /></ListGroupItem>
                </ListGroup>
            </ModalBody>
            <ModalFooter>
                {userEmail === undefined
                    ? <Button color="primary" onClick={this.props.openLoginModal}>Log In To Edit</Button>
                    : <Button color="primary" onClick={this.props.openEditExistingMemberModal} disabled={!this.canEditMember(userEmail)}>Edit</Button>
                }
                {' '}<Button color="secondary" onClick={this.toggleModal}>Close</Button>
            </ModalFooter>
        </>;
    }

    private renderAddNewMemberModalContent(event: FursvpEventsStore.FursvpEvent) : React.ReactNode {
        return <>
            <ModalHeader toggle={this.toggleModal}><>Some name textbox here</></ModalHeader>
            <ModalBody>
                <ListGroup>
                    <>Some email address textbox here</>
                    {event.form.map(prompt =>
                        <ListGroupItem key={prompt.id}>
                            <ListGroupItemHeading>{prompt.prompt}</ListGroupItemHeading>
                            {prompt.behavior == 'Text'
                                ? <>Some textbox here</>
                                : <></>}
                            {prompt.behavior == 'Checkboxes'
                                ? prompt.options.map(option => <ListGroupItemText key={option}>{option}</ListGroupItemText>)
                                : <></>}
                            {prompt.behavior == 'Dropdown'
                                ? prompt.options.map(option => <ListGroupItemText key={option}>{option}</ListGroupItemText>)
                                : <></>}
                            }
                        </ListGroupItem>
                    )}
                </ListGroup>
            </ModalBody>
            <ModalFooter>
                <Button color="primary" onClick={this.toggleModal}>Add RSVP</Button>
                {' '}<Button color="secondary" onClick={this.toggleModal}>Cancel</Button>
            </ModalFooter>
        </>;
    }

    private renderEditMemberModalContent(event: FursvpEventsStore.FursvpEvent, member: Member | undefined) {
        if (member === undefined) {
            return this.renderAddNewMemberModalContent(event);
        }

        return <>
            <ModalHeader toggle={this.toggleModal}><>Some name textbox here</></ModalHeader>
            <ModalBody>
                <ListGroup>
                    <>Some email address textbox here</>
                    {event.form.map(prompt =>
                        <ListGroupItem key={prompt.id}>
                            <ListGroupItemHeading>{prompt.prompt}</ListGroupItemHeading>
                            {prompt.behavior == 'Text'
                                ? <>Some textbox here</>
                                : <></>}
                            {prompt.behavior == 'Checkboxes'
                                ? prompt.options.map(option => <ListGroupItemText key={option}>{option}</ListGroupItemText>)
                                : <></>}
                            {prompt.behavior == 'Dropdown'
                                ? prompt.options.map(option => <ListGroupItemText key={option}>{option}</ListGroupItemText>)
                                : <></>}
                            }
                        </ListGroupItem>
                    )}
                </ListGroup>
            </ModalBody>
            <ModalFooter>
                <Button color="primary" onClick={this.toggleModal}>Save Changes</Button>
                {' '}<Button color="secondary" onClick={this.toggleModal}>Cancel</Button>
                {' '}<Button outline color="danger" onClick={this.toggleModal}>Remove RSVP</Button>
            </ModalFooter>
        </>;
    }

    public render() {
        if (this.props.fursvpEvent !== undefined) {
            var event = this.props.fursvpEvent;
            var member = this.props.modalMember;
            var responses: FursvpEventsStore.FormResponses[] = this.props.modalMember !== undefined ? this.props.modalMember.responses : [];
            var userEmail = getStoredVerifiedEmail();

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
                        <span className="text-muted">Starts</span> <DateTime date={event.startsAtLocal} timeZoneOffset={event.timeZoneOffset} id="eventDetail_startsAt" />
                    </Container>
                    <Container>
                        <span className="text-muted">Ends</span> <DateTime date={event.endsAtLocal} timeZoneOffset={event.timeZoneOffset} id="eventDetail_endsAt" />
                    </Container>
                    <Container>
                        <span className="text-muted">Location</span> {event.location}
                    </Container>
                    <Container>
                        {event.otherDetails}
                    </Container>
                    <Container>
                        <ListGroup>
                            <ListGroupItem active tag="button" action onClick={this.openNewMemberModal}>
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
                        {this.props.modalIsInEditMode
                            ? this.renderEditMemberModalContent(event, member)
                            : this.renderViewOnlyModalContent(event, member, responses, userEmail)
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

    private showMember(member: FursvpEventsStore.Member) {
        this.props.history.push('/event/' + this.props.id + '/member/' + member.id);
        this.props.openModal(member);
    }

    private openNewMemberModal() {
        this.props.openNewMemberModal();
    }

    private toggleModal() {
        if (this.props.modalIsOpen && this.props.modalMember !== undefined) {
            this.props.history.push('/event/' + this.props.id);
            this.props.toggleModal();
        }
        else if (this.props.modalMember !== undefined) {
            this.props.history.push('/event/' + this.props.id + '/member/' + this.props.modalMember.id);
        }
        else {
            this.props.toggleModal();
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