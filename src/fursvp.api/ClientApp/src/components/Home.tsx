import * as React from 'react';
import { Badge, ListGroup, ListGroupItem, ListGroupItemHeading, ListGroupItemText, UncontrolledTooltip, Modal, ModalHeader, ModalFooter, ModalBody, Button, FormGroup, Label, Input } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as FursvpEventsStore from '../store/FursvpEvents';
import DateTime from './DateTime';
import { Formik, Form as FormikForm, useField, FormikValues } from 'formik';
import { getStoredVerifiedEmail } from '../store/UserStore';

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
                {this.createNewEventModal()}
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        this.props.requestFursvpEvents();
    }

    private createNewEventModal() {
        return (
            <Formik initialValues={{authorName: ""}} onSubmit={(values) => { this.createNewEvent.bind(this, values); }}>
                <Modal isOpen={this.props.isCreateNewEventModalOpen} toggle={this.toggleCreateNewEventModal.bind(this)}>
                    <FormikForm translate={undefined}>
                        <ModalHeader>Create New Event</ModalHeader>
                        <ModalBody>
                            <Label id="authorNameLabel" htmlFor="authorName">Your Name</Label>
                            <UncontrolledTooltip target="authorNameLabel">This is your display name as the author of this event.</UncontrolledTooltip>
                            <Input id="authorName" required name="authorName" />
                        </ModalBody>
                        <ModalFooter>
                            {getStoredVerifiedEmail() === undefined
                                ? <Button color="primary" onClick={this.props.openLoginModal}>Log In To Create New Event</Button>
                                : <Button color="primary" type="submit" disabled={this.props.isSubmitting}>Create New Event</Button>}
                            {' '}<Button color="secondary" onClick={this.toggleCreateNewEventModal.bind(this)} disabled={this.props.isSubmitting}>Cancel</Button>
                        </ModalFooter>
                    </FormikForm>
                </Modal>
            </Formik>
        );
    }

    private listEvents() {
        return (
            <ListGroup>
                <ListGroupItem active tag="button" action onClick={this.addEventButtonClicked.bind(this)}>
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

    private toggleCreateNewEventModal() {
        this.props.toggleCreateNewEventModal();
    }

    private addEventButtonClicked() {
        this.props.addEventButtonClicked();
    }

    private createNewEvent(values : any) {
        this.props.createNewEvent(values, this.showEvent);
    }
}

export default connect(
    (state: ApplicationState) => state.fursvpEvents, // Selects which state properties are merged into the component's props
    FursvpEventsStore.actionCreators // Selects which action creators are merged into the component's props
)(Home as any);