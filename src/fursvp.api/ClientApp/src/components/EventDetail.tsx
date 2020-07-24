import * as React from 'react';
import { Container, UncontrolledTooltip, ListGroup, ListGroupItem, Modal, ModalHeader, ModalBody, ModalFooter, Button, ListGroupItemText, ListGroupItemHeading, Form, FormGroup, Label, Input } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps, Redirect } from 'react-router';
import { ApplicationState } from '../store';
import * as EventDetailStore from '../store/EventDetailStore';
import * as FursvpEventsStore from '../store/FursvpEvents';
import DateTime from './DateTime';
import { getStoredVerifiedEmail } from '../store/UserStore';
import { Member } from '../store/FursvpEvents';
import { Formik, Form as FormikForm, useField, FormikValues } from 'formik';

const RsvpTextInput = (props : {label: string, id: string, required: boolean}) => {
    const [field, meta] = useField({ id: props.id, required: props.required, name: props.id});
    return (
        <>
            <Label htmlFor={props.id}>{props.label}</Label>
            <Input {...field} id={props.id} required={props.required} name={props.id} />
            {meta.touched && meta.error ? (
                <div className="error">{meta.error}</div>
            ) : null}
        </>
    );
};

const RsvpDropdown = (props: { children: JSX.Element[], label: string, id: string, required: boolean }) => {
    const [field, meta] = useField({ id: props.id, required: props.required, name: props.id, children: props.children });
    return (
        <>
            <Label htmlFor={props.id}>{props.label}</Label>
            <Input type="select" id={props.id} name={props.id} required={props.required} children={props.children} {...field} />
            {meta.touched && meta.error ? (
                <div className="error">{meta.error}</div>
            ) : null}
        </>
    );
};

const RsvpCheckboxes = (props: { options: string[], id: string }) => {
    const [field, meta] = useField({ id: props.id, name: props.id });
    return (
        <Label check id={props.id}>
            {props.options.map(option => <><Input id={props.id + option} key={option} type="checkbox" />{' '}{option}</>)}
        </Label>
    );
};

const getNewMemberInitialValues = (form: FursvpEventsStore.FormPrompt[]) => {
    var result : any = {
        newMemberName: "",
        newMemberEmail: ""
    };

    for (let prompt of form) {
        if (prompt.behavior == "Checkboxes") {
            for (let option of prompt.options) {
                result["newPrompt" + prompt.id + option] = "";
            }
        }
        else {
            result["newPrompt" + prompt.id] = "";
        }
    }

    return result;
}

// At runtime, Redux will merge together...
type EventDetailProps =
    EventDetailStore.EventDetailState // ... state we've requested from the Redux store
    & typeof EventDetailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{ eventId: string, memberId: string }>; // ... plus incoming routing parameters

class EventDetail extends React.PureComponent<EventDetailProps> {
    constructor(props : EventDetailProps) {
        super(props);

        this.toggleModal = this.toggleModal.bind(this);
        this.toggleRemoveRsvpModal = this.toggleRemoveRsvpModal.bind(this);
        this.removeRsvp = this.removeRsvp.bind(this);
        this.askForRemoveRsvpConfirmation = this.askForRemoveRsvpConfirmation.bind(this);
        this.openNewMemberModal = this.openNewMemberModal.bind(this);
        this.addNewMember = this.addNewMember.bind(this);
        this.editExistingMember = this.editExistingMember.bind(this);
        this.toggleRsvpRemovedModal = this.toggleRsvpRemovedModal.bind(this);
        this.cancelEditMember = this.cancelEditMember.bind(this);
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
                    {this.joinResponsesToPrompts(responses, event.form).sort(x => x.prompt.sortOrder).map(promptWithResponse => {
                        return promptWithResponse.responses !== undefined
                            ? <ListGroupItem key={promptWithResponse.prompt.id}>
                                <ListGroupItemHeading>{promptWithResponse.prompt.prompt}</ListGroupItemHeading>
                                {promptWithResponse.responses.responses.map(individualResponse => <ListGroupItemText key={individualResponse}>{individualResponse}</ListGroupItemText>)}
                            </ListGroupItem>
                            : <></>;
                        }
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

    private renderAddNewMemberModalContent(event: FursvpEventsStore.FursvpEvent): React.ReactNode {
        return <Formik initialValues={getNewMemberInitialValues(event.form)} onSubmit={(values, { setSubmitting }) => { this.addNewMember(values); }}>
            <FormikForm translate={undefined}>
                <ModalHeader toggle={this.toggleModal}>RSVP for {this.props.fursvpEvent ? this.props.fursvpEvent.name : ""}</ModalHeader>
                <ModalBody>
                    <FormGroup>
                        <RsvpTextInput id="newMemberName" label="Name" required />
                    </FormGroup>
                    <FormGroup>
                        <RsvpTextInput id="newMemberEmail" label="Email" required />
                    </FormGroup>
                    {event.form.sort(x => x.sortOrder).map(prompt =>
                        <FormGroup key={prompt.id} check={prompt.behavior == 'Checkboxes'}>
                            {prompt.behavior == 'Text'
                                ?
                                <RsvpTextInput id={"newPrompt" + prompt.id} label={prompt.prompt} required={prompt.required} />
                                : <></>}
                            {prompt.behavior == 'Checkboxes'
                                ? <RsvpCheckboxes id={"newPrompt" + prompt.id} options={prompt.options} />
                                : <></>}
                            {prompt.behavior == 'Dropdown'
                                ? <RsvpDropdown label={prompt.prompt} id={"newPrompt" + prompt.id} required={prompt.required}>
                                    {prompt.options.map(option => <option key={option}>{option}</option>)}
                                </RsvpDropdown>
                                : <></>}
                        </FormGroup>
                    )}
                </ModalBody>
                <ModalFooter>
                    <Button type="submit" color="primary" disabled={this.props.isSaving}>Add RSVP</Button>
                    {' '}<Button color="secondary" onClick={this.toggleModal} disabled={this.props.isSaving}>Cancel</Button>
                </ModalFooter>
            </FormikForm>
        </Formik>;
    }

    private renderEditMemberModalContent(
        event: FursvpEventsStore.FursvpEvent,
        member: Member | undefined,
        responses: FursvpEventsStore.FormResponses[],
    ) {
        if (member === undefined) {
            return this.renderAddNewMemberModalContent(event);
        }

        return <Formik initialValues={this.getExistingMemberInitialValues(event.form, member)} onSubmit={(values, { setSubmitting }) => { this.editExistingMember(member.id, values); }}>
            <FormikForm translate={undefined}>
                <ModalHeader toggle={this.toggleModal}>Edit RSVP for {this.props.fursvpEvent ? this.props.fursvpEvent.name : ""}</ModalHeader>
                <ModalBody>
                    <FormGroup>
                        <RsvpTextInput id="editMemberName" label="Name" required />
                    </FormGroup>
                    <FormGroup>
                        <RsvpTextInput id="editMemberEmail" label="Email" required />
                    </FormGroup>
                    {event.form.sort(x => x.sortOrder).map(prompt =>
                        <FormGroup key={prompt.id} check={prompt.behavior == 'Checkboxes'}>
                            {prompt.behavior == 'Text'
                                ?
                                <RsvpTextInput id={"editPrompt" + prompt.id} label={prompt.prompt} required={prompt.required} />
                                : <></>}
                            {prompt.behavior == 'Checkboxes'
                                ? <RsvpCheckboxes id={"editPrompt" + prompt.id} options={prompt.options} />
                                : <></>}
                            {prompt.behavior == 'Dropdown'
                                ? <RsvpDropdown label={prompt.prompt} id={"editPrompt" + prompt.id} required={prompt.required}>
                                    {prompt.options.map(option => <option key={option}>{option}</option>)}
                                </RsvpDropdown>
                                : <></>}
                        </FormGroup>
                    )}
                </ModalBody>
                <ModalFooter>
                    <Button type="submit" color="primary" disabled={this.props.isSaving}>Save Changes</Button>
                    {' '}<Button color="secondary" onClick={this.cancelEditMember} disabled={this.props.isSaving}>Cancel</Button>
                    {' '}<Button outline color="danger" onClick={this.askForRemoveRsvpConfirmation} disabled={this.props.isSaving}>Remove RSVP</Button>
                </ModalFooter>
            </FormikForm>
        </Formik>;
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
                            ? this.renderEditMemberModalContent(event, member, responses)
                            : this.renderViewOnlyModalContent(event, member, responses, userEmail)
                        }
                    </Modal>
                    <Modal isOpen={this.props.isAskingForRemoveRsvpConfirmation} toggle={this.toggleRemoveRsvpModal}>
                        <ModalHeader>Remove RSVP?</ModalHeader>
                        <ModalBody>Please confirm that you wish to withdraw {member !== undefined ? member.name : "this member"}'s RSVP from this event.</ModalBody>
                        <ModalFooter>
                            <Button color="danger" onClick={() => this.removeRsvp(event.id, member !== undefined ? member.id : undefined)}>Remove RSVP</Button>
                            {' '}<Button color="secondary" onClick={this.toggleRemoveRsvpModal}>Cancel</Button>
                        </ModalFooter>
                    </Modal>
                    <Modal isOpen={this.props.rsvpRemovedModalIsOpen} toggle={this.toggleRsvpRemovedModal}>
                        <ModalHeader>RSVP Removed</ModalHeader>
                        <ModalBody>This RSVP has been removed.</ModalBody>
                        <ModalFooter>
                            <Button color="primary" onClick={this.toggleRsvpRemovedModal}>Close</Button>
                        </ModalFooter>
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

    private getExistingMemberInitialValues(prompts: FursvpEventsStore.FormPrompt[], member: FursvpEventsStore.Member) {
        var result: any = {
            editMemberName: member.name,
            editMemberEmail: member.emailAddress
        };

        var promptsWithResponses = this.joinResponsesToPrompts(member.responses, prompts);

        for (let item of promptsWithResponses) {
            if (item.prompt.behavior == "Checkboxes") {
                for (let option of item.prompt.options) {
                    result["editPrompt" + item.prompt.id + option] = false;

                    if (item.responses !== undefined) {
                        for (let response of item.responses.responses) {
                            if (response == option) {
                                result["editPrompt" + item.prompt.id + option] = true;
                                break;
                            }
                        }
                    }
                }
            }
            else {
                result["editPrompt" + item.prompt.id] = item.responses !== undefined && item.responses.responses.length > 0 ? item.responses.responses[0] : "";
            }
        }

        return result;
    }

    private joinResponsesToPrompts(responses: FursvpEventsStore.FormResponses[], prompts: FursvpEventsStore.FormPrompt[]): PromptWithResponses[] {
        var result: PromptWithResponses[] = [];

        for (let prompt of prompts) {

            var responsesForPrompt: FursvpEventsStore.FormResponses | undefined = undefined;

            for (let response of responses) {
                if (response.promptId == prompt.id) {
                    responsesForPrompt = response;
                    break;
                }
            }

            result.push({ prompt: prompt, responses: responsesForPrompt });
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

    private addNewMember(values: FormikValues) {
        this.props.addNewMember(values);
    }

    private editExistingMember(memberId : string, values: FormikValues) {
        this.props.editExistingMember(memberId, values);
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

    private cancelEditMember() {
        this.props.cancelEditMember();
    }

    private toggleRemoveRsvpModal() {
        this.props.toggleRemoveRsvpModal();
    }

    private toggleRsvpRemovedModal() {
        this.props.history.push('/event/' + this.props.id);
        this.props.toggleRsvpRemovedModal();
    }

    private removeRsvp(eventId:string, memberId:string | undefined) {
        this.props.removeRsvp(eventId, memberId);
    }

    private askForRemoveRsvpConfirmation() {
        this.props.askForRemoveRsvpConfirmation();
    }

    private ensureDataFetched() {
        this.props.requestFursvpEvent(this.props.match.params.eventId, this.props.match.params.memberId);
    }
}

interface PromptWithResponses {
    prompt: FursvpEventsStore.FormPrompt,
    responses: FursvpEventsStore.FormResponses | undefined
}

export default connect(
    (state: ApplicationState) => state.targetEvent, // Selects which state properties are merged into the component's props
    EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail as any);