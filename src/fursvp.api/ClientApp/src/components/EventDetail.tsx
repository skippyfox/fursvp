import * as React from 'react';
import {
    Button, Container, FormGroup, Input,
    Label, ListGroup, ListGroupItem, ListGroupItemText,
    Modal, ModalHeader, ModalBody, ModalFooter,
    Nav, NavItem, NavLink, TabContent, TabPane,
    UncontrolledTooltip
} from 'reactstrap';
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

const RsvpCheckbox = (props: { label: string, id: string }) => {
    const [field, meta] = useField({ id: props.id, name: props.id });
    return (
        <Container>
            <Label>
                <Input type="checkbox" {...field} id={props.id} name={props.id} checked={meta.value} />
                {props.label}
            </Label>
            {meta.touched && meta.error ? (
                <div className="error">{meta.error}</div>
            ) : null}
        </Container>
    );
};

const RsvpCheckboxGroup = (props: { options: string[], label: string, id: string }) => {    
    return (
        <>
            <Label>{props.label}</Label>
            {props.options.map(option => {
                const [field] = useField({ id: props.id, name: props.id, value: option, type: "checkbox" });
                return <Container key={props.id + option}><Input type="checkbox" name={props.id} value={option} {...field} />{' '}{option}</Container>;
            })}
        </>
    );
};

const getNewMemberInitialValues = (form: FursvpEventsStore.FormPrompt[]) => {
    var result : any = {
        newMemberName: "",
        newMemberEmail: "",
        newMemberIsAttending: true,
        newMemberIsOrganizer: false
    };

    for (let prompt of form) {
        if (prompt.behavior === "Checkboxes") {
            if (prompt.options.length === 1) {
                // Formik records a single checkbox as a boolean.
                result["newPrompt" + prompt.id] = false;
            }
            else {
                // Formik records a checkbox group as an array of strings.
                result["newPrompt" + prompt.id] = [];
            }
        }
        else {
            // Formik records a text input or dropdown selection as a string.
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
        this.openEditEventModal = this.openEditEventModal.bind(this);
        this.toggleEditEventModal = this.toggleEditEventModal.bind(this);
        this.setEditEventModalActiveTab = this.setEditEventModalActiveTab.bind(this);
    }

    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    // This method is called when the route parameters change
    public componentDidUpdate() {
        this.ensureDataFetched();
    }

    private canEditMember(): boolean {
        // TODO: These are business rules that belong in the domain layer. The results can be passed down in the Response object

        // No member to edit
        if (this.props.modalMember === undefined) {
            return false;
        }

        // User not logged in as someone in this list
        if (this.props.actingMember === undefined) {
            return false;
        }

        // User logged in and editing user's own entry
        if (this.props.modalMember.emailAddress === this.props.actingMember.emailAddress) {
            return true;
        }

        // User is event author
        if (this.props.actingMember.isAuthor) {
            return true;
        }

        // User is an event organizer, and member is not an author or organizer
        if (this.props.actingMember.isOrganizer && !this.props.modalMember.isAuthor && !this.props.modalMember.isOrganizer) {
            return true;
        }

        return false;
    }

    private canSetOrganizer(): boolean {
        // TODO: These are business rules that belong in the domain layer. The results can be passed down in the Response object

        // User is event author
        if (this.props.actingMember !== undefined && this.props.actingMember.isAuthor) {
            return true;
        }

        return false;
    }

    private canSetAttending(isOrganizerChecked : boolean): boolean {
        // TODO: These are business rules that belong in the domain layer. The results can be passed down in the Response object
        
        // User not logged in as someone in this list
        if (this.props.actingMember === undefined) {
            return false;
        }

        // An author can choose whether an organizer is attending
        if (this.props.actingMember.isAuthor && isOrganizerChecked) {            
            return true;
        }

        // An author or organizer can choose not to be attending
        if (this.userIsEditingOwnEntry() && (this.props.actingMember.isAuthor || this.props.actingMember.isOrganizer)) {            
            return true;
        }

        return false;
    }

    private userIsEditingOwnEntry() {
        // New member
        if (this.props.modalMember === undefined) {
            return false;
        }

        // User not logged in as someone in this list
        if (this.props.actingMember === undefined) {
            return false;
        }

        return this.props.actingMember.emailAddress === this.props.modalMember.emailAddress;
    }

    private canWithdrawRsvpWhenEditing(): boolean {
        return this.props.modalMember !== undefined && !this.props.modalMember.isAuthor;
    }

    private renderViewOnlyModalContent(
        event: FursvpEventsStore.FursvpEvent,
        member: Member | undefined,
        responses: FursvpEventsStore.FormResponses[])
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
                                <ListGroupItemText>
                                    {promptWithResponse.prompt.prompt}
                                    <ul>{promptWithResponse.responses.responses.map(individualResponse => <li key={individualResponse}>{individualResponse}</li>)}</ul>
                                </ListGroupItemText>
                            </ListGroupItem>
                            : <></>;
                        }
                    )}
                    <ListGroupItem>✔<DateTime date={member.rsvpedAtLocal} timeZoneOffset={event.timeZoneOffset} id="eventDetail_memberModal_rsvpedAt" /></ListGroupItem>
                </ListGroup>
            </ModalBody>
            <ModalFooter>
                {this.renderEditMemberButton()}
                {' '}<Button color="secondary" onClick={this.toggleModal}>Close</Button>
            </ModalFooter>
        </>;
    }

    private renderEditMemberButton() {

        // User is not logged in
        if (getStoredVerifiedEmail() === undefined) {
            return <Button color="primary" onClick={this.props.openLoginModal}>Log In To Edit</Button>;
        }

        if (this.canEditMember()) {
            return <Button color="primary" onClick={this.props.openEditExistingMemberModal}>Edit</Button>;
        }

        return <></>;
    }

    private renderAddNewMemberModalContent(event: FursvpEventsStore.FursvpEvent): React.ReactNode {
        return <Formik initialValues={getNewMemberInitialValues(event.form)} onSubmit={(values) => { this.addNewMember(values); }}>
            {formik => (
                <FormikForm translate={undefined}>
                    <ModalHeader toggle={this.toggleModal}>RSVP for {this.props.fursvpEvent ? this.props.fursvpEvent.name : ""}</ModalHeader>
                    <ModalBody>
                        <FormGroup><RsvpTextInput id="newMemberName" label="Name" required /></FormGroup>
                        <FormGroup><RsvpTextInput id="newMemberEmail" label="Email" required /></FormGroup>
                        {this.canSetOrganizer()
                            ? <FormGroup><RsvpCheckbox id="newMemberIsOrganizer" label="Is Organizer" /></FormGroup>
                            : <></>}
                        {this.canSetAttending(formik.values.newMemberIsOrganizer)
                            ? <FormGroup><RsvpCheckbox id="newMemberIsAttending" label="Is Attending" /></FormGroup>
                            : <></>}
                        {event.form.sort(x => x.sortOrder).map(prompt =>
                            <FormGroup key={prompt.id}>
                                {prompt.behavior === 'Text'
                                    ?
                                    <RsvpTextInput id={"newPrompt" + prompt.id} label={prompt.prompt} required={prompt.required} />
                                    : <></>}
                                {prompt.behavior === 'Checkboxes'
                                    ? <RsvpCheckboxGroup id={"newPrompt" + prompt.id} label={prompt.prompt} options={prompt.options} />
                                    : <></>}
                                {prompt.behavior === 'Dropdown'
                                    ? <RsvpDropdown label={prompt.prompt} id={"newPrompt" + prompt.id} required={prompt.required}>
                                        <option key="" value="">Select one...</option>
                                        <>{prompt.options.map(option => <option key={option}>{option}</option>)}</>
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
            )}
        </Formik>;
    }

    private renderEditMemberModalContent(
        event: FursvpEventsStore.FursvpEvent,
        member: Member | undefined
    ) {
        if (member === undefined) {
            return this.renderAddNewMemberModalContent(event);
        }

        return <Formik initialValues={this.getExistingMemberInitialValues(event.form, member)} onSubmit={(values) => { this.editExistingMember(member, values); }}>
            {formik => (
                <FormikForm translate={undefined}>
                    <ModalHeader toggle={this.toggleModal}>Edit RSVP for {this.props.fursvpEvent ? this.props.fursvpEvent.name : ""}</ModalHeader>
                    <ModalBody>
                        <FormGroup><RsvpTextInput id="editMemberName" label="Name" required /></FormGroup>
                        <FormGroup><RsvpTextInput id="editMemberEmail" label="Email" required /></FormGroup>
                        {this.canSetOrganizer()
                            ? <FormGroup><RsvpCheckbox id="editMemberIsOrganizer" label="Is Organizer" /></FormGroup>
                            : <></>}
                        {this.canSetAttending(formik.values.editMemberIsOrganizer)
                            ? <FormGroup><RsvpCheckbox id="editMemberIsAttending" label="Is Attending" /></FormGroup>
                            : <></>}

                        {event.form.sort(x => x.sortOrder).map(prompt =>
                            <FormGroup key={prompt.id}>
                                {prompt.behavior === 'Text'
                                    ?
                                    <RsvpTextInput id={"editPrompt" + prompt.id} label={prompt.prompt} required={prompt.required} />
                                    : <></>}
                                {prompt.behavior === 'Checkboxes'
                                    ? <RsvpCheckboxGroup id={"editPrompt" + prompt.id} label={prompt.prompt} options={prompt.options} />
                                    : <></>}
                                {prompt.behavior === 'Dropdown'
                                    ? <RsvpDropdown label={prompt.prompt} id={"editPrompt" + prompt.id} required={prompt.required}>
                                        <option key="" value="">Select one...</option>
                                        <>{prompt.options.map(option => <option key={option}>{option}</option>)}</>
                                    </RsvpDropdown>
                                    : <></>}
                            </FormGroup>
                        )}
                    </ModalBody>
                    <ModalFooter>
                        <Button type="submit" color="primary" disabled={this.props.isSaving}>Save Changes</Button>
                        {' '}<Button color="secondary" onClick={this.cancelEditMember} disabled={this.props.isSaving}>Cancel</Button>
                        {this.canWithdrawRsvpWhenEditing()
                            ? <>{' '}<Button outline color="danger" onClick={this.askForRemoveRsvpConfirmation} disabled={this.props.isSaving}>Remove RSVP</Button></>
                            : <></>}
                    </ModalFooter>
                </FormikForm>
            )}
        </Formik>;
    }

    private rsvpsAreClosed(event: FursvpEventsStore.FursvpEvent): boolean {
        if (!event.rsvpOpen) {
            return true;
        }

        if (event.rsvpClosesInMs !== null && event.rsvpClosesInMs <= 0) {
            return true;
        }

        return false;
    }

    private canAddRsvpWhenClosed(member: Member | undefined): boolean {
        if (member === undefined) {
            return false;
        }

        return member.isAuthor || member.isOrganizer;
    }

    private renderAddRsvpButtonContent(event: FursvpEventsStore.FursvpEvent, rsvpsAreClosed: boolean, canAddRsvpWhenClosed: boolean): React.ReactNode {
        if (rsvpsAreClosed && !canAddRsvpWhenClosed) {
            return <>RSVPs are not open at this time.</>;
        }

        return (
            <>
                Add an RSVP
                {!rsvpsAreClosed && event.rsvpClosesAtLocal != null
                    ? <><br /><small>RSVPs are open until <DateTime date={event.rsvpClosesAtLocal} timeZoneOffset={event.timeZoneOffset} id="eventDetailRsvpsCloseAt" /></small></>
                    : <></>}
                {rsvpsAreClosed && canAddRsvpWhenClosed
                    ? <><br /><small>RSVPs are open only to organizers as this time</small></>
                    : <></>}
            </>
        );
    }

    public render() {
        if (this.props.fursvpEvent !== undefined) {
            var event = this.props.fursvpEvent;
            var member = this.props.modalMember;
            var responses: FursvpEventsStore.FormResponses[] = this.props.modalMember !== undefined ? this.props.modalMember.responses : [];

            var rsvpsAreClosed = this.rsvpsAreClosed(event);
            var canAddRsvpWhenClosed = this.canAddRsvpWhenClosed(this.props.actingMember);

            let padlock = <></>;
            if (!event.isPublished) {
                padlock = <>
                    <span id="privateEventIndicator" role="img" aria-label="This event is visible only to you.">🔒</span>
                    <UncontrolledTooltip target="privateEventIndicator">This event is visible only to you.</UncontrolledTooltip>
                </>
            }

            return (
                <React.Fragment>
                    <h1 id="tabelLabel">
                        {event.name}
                        {padlock}
                        {this.props.actingMember !== undefined && (this.props.actingMember.isAuthor || this.props.actingMember.isOrganizer)
                            ? <>{' '}<Button color="primary" onClick={this.openEditEventModal}>Edit</Button></>
                            : <></>}
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
                            <ListGroupItem active tag="button" action onClick={this.openNewMemberModal} disabled={rsvpsAreClosed && !canAddRsvpWhenClosed}>
                                {this.renderAddRsvpButtonContent(event, rsvpsAreClosed, canAddRsvpWhenClosed)}                  
                            </ListGroupItem>
                            {event.members.map((member: FursvpEventsStore.Member) =>
                                <ListGroupItem key={member.id} tag="button" action onClick={this.showMember.bind(this, member)}>
                                    {this.memberTypeEmoji(member)}
                                    {member.name}
                                </ListGroupItem>
                            )}
                        </ListGroup>
                    </Container>
                    <Modal isOpen={this.props.memberModalIsOpen} toggle={this.toggleModal}>
                        {this.props.modalIsInEditMemberMode
                            ? this.renderEditMemberModalContent(event, member)
                            : this.renderViewOnlyModalContent(event, member, responses)
                        }
                    </Modal>
                    <Modal isOpen={this.props.editEventModalIsOpen} toggle={this.toggleEditEventModal}>
                        <ModalBody>
                            <Nav tabs>
                                <NavItem>
                                    <NavLink className={this.props.editEventModalActiveTab == 'editEventDetailsTab' ? "active" : ""} onClick={this.setEditEventModalActiveTab.bind(this, 'editEventDetailsTab')}>
                                        Details
                                    </NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink className={this.props.editEventModalActiveTab == 'editEventFormTab' ? "active" : ""} onClick={this.setEditEventModalActiveTab.bind(this, 'editEventFormTab')}>
                                        RSVP Form
                                    </NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink className={this.props.editEventModalActiveTab == 'editEventPublishTab' ? "active" : ""} onClick={this.setEditEventModalActiveTab.bind(this, 'editEventPublishTab')}>
                                        Publish
                                    </NavLink>
                                </NavItem>
                            </Nav>
                            <TabContent activeTab={this.props.editEventModalActiveTab}>
                                <TabPane tabId="editEventDetailsTab">
                                    Details
                                </TabPane>
                                <TabPane tabId="editEventFormTab">
                                    RSVP Form
                                </TabPane>
                                <TabPane tabId="editEventPublishTab">
                                    Publish
                                </TabPane>
                            </TabContent>
                        </ModalBody>
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
            editMemberEmail: member.emailAddress,
            editMemberIsAttending: member.isAttending,
            editMemberIsOrganizer: member.isOrganizer
        };

        var promptsWithResponses = this.joinResponsesToPrompts(member.responses, prompts);

        for (let item of promptsWithResponses) {
            if (item.prompt.behavior === "Checkboxes") {
                if (item.prompt.options.length === 1) {
                    // Formik records a single checkbox as a boolean.
                    result["editPrompt" + item.prompt.id] = item.responses !== undefined && item.responses.responses.length === 1 && item.responses.responses[0] === item.prompt.options[0];
                }
                else {
                    // Formik records a checkbox group as an array of strings.
                    result["editPrompt" + item.prompt.id] = item.responses ? item.responses.responses : [];
                }
            }
            else {
                // Formik stores textboxes and dropdown selections as a string.
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
                if (response.promptId === prompt.id) {
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

        var suffix = member.isAttending
            ? ""
            : " (not attending)";

        if (member.isOrganizer) {
            return this.toolTip("⭐", "Organizer" + suffix, id);
        }

        if (member.isAttending) {
            return <></>;
            //return this.toolTip("🧑", "Attending", id);
        }

        if (member.isAuthor) {
            return this.toolTip("⭐", "Author" + suffix, id);
        }

        return <></>;
    }

    private showMember(member: FursvpEventsStore.Member) {
        this.props.history.push('/event/' + this.props.id + '/member/' + member.id);
        this.props.openMemberModal(member);
    }

    private openEditEventModal() {
        this.props.history.push(`/event/${this.props.id}/edit`);
        this.props.openEditEventModal();
    }

    private toggleEditEventModal() {
        this.props.history.push(`/event/${this.props.id}`);
        this.props.toggleEditEventModal();
    }

    private openNewMemberModal() {
        this.props.openNewMemberModal();
    }

    private addNewMember(values: FormikValues) {
        this.props.addNewMember(values);
    }

    private editExistingMember(member : Member, values: FormikValues) {
        this.props.editExistingMember(member, values);
    }

    private toggleModal() {
        if (this.props.memberModalIsOpen && this.props.modalMember !== undefined) {
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
        this.props.requestFursvpEvent(this.props.match.params.eventId, this.props.match.params.memberId, this.props.match.path.search("edit") !== -1, this.props.history);
    }

    private setEditEventModalActiveTab(tabId: string) {
        this.props.setEditEventModalActiveTab(tabId);
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