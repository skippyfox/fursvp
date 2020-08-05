"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var reactstrap_1 = require("reactstrap");
var react_redux_1 = require("react-redux");
var react_router_1 = require("react-router");
var EventDetailStore = require("../store/EventDetailStore");
var DateTime_1 = require("./DateTime");
var UserStore_1 = require("../store/UserStore");
var formik_1 = require("formik");
var pickers_1 = require("@material-ui/pickers");
var RsvpTextInput = function (props) {
    var _a = formik_1.useField({ id: props.id, required: props.required, name: props.id }), field = _a[0], meta = _a[1];
    return (React.createElement(React.Fragment, null,
        React.createElement(reactstrap_1.Label, { htmlFor: props.id }, props.label),
        React.createElement(reactstrap_1.Input, __assign({}, field, { id: props.id, required: props.required, name: props.id })),
        meta.touched && meta.error ? (React.createElement("div", { className: "error" }, meta.error)) : null));
};
var RsvpDropdown = function (props) {
    var _a = formik_1.useField({ id: props.id, required: props.required, name: props.id, children: props.children }), field = _a[0], meta = _a[1];
    return (React.createElement(React.Fragment, null,
        React.createElement(reactstrap_1.Label, { htmlFor: props.id }, props.label),
        React.createElement(reactstrap_1.Input, __assign({ type: "select", id: props.id, name: props.id, required: props.required, children: props.children }, field)),
        meta.touched && meta.error ? (React.createElement("div", { className: "error" }, meta.error)) : null));
};
var RsvpCheckbox = function (props) {
    var _a = formik_1.useField({ id: props.id, name: props.id }), field = _a[0], meta = _a[1];
    return (React.createElement(reactstrap_1.Container, null,
        React.createElement(reactstrap_1.Label, null,
            React.createElement(reactstrap_1.Input, __assign({ type: "checkbox" }, field, { id: props.id, name: props.id, checked: meta.value })),
            props.label),
        meta.touched && meta.error ? (React.createElement("div", { className: "error" }, meta.error)) : null));
};
var RsvpCheckboxGroup = function (props) {
    return (React.createElement(React.Fragment, null,
        React.createElement(reactstrap_1.Label, null, props.label),
        props.options.map(function (option) {
            var field = formik_1.useField({ id: props.id, name: props.id, value: option, type: "checkbox" })[0];
            return React.createElement(reactstrap_1.Container, { key: props.id + option },
                React.createElement(reactstrap_1.Input, __assign({ type: "checkbox", name: props.id, value: option }, field)),
                ' ',
                option);
        })));
};
var RsvpDateTimePicker = function (props) {
    var setFieldValue = formik_1.useFormikContext().setFieldValue;
    var field = formik_1.useField({ id: props.id, name: props.id })[0];
    return (React.createElement(pickers_1.DateTimePicker, __assign({}, field, props, { onChange: function (val) {
            setFieldValue(field.name, val);
        } })));
};
var getNewMemberInitialValues = function (form) {
    var result = {
        newMemberName: "",
        newMemberEmail: "",
        newMemberIsAttending: true,
        newMemberIsOrganizer: false
    };
    for (var _i = 0, form_1 = form; _i < form_1.length; _i++) {
        var prompt_1 = form_1[_i];
        if (prompt_1.behavior === "Checkboxes") {
            if (prompt_1.options.length === 1) {
                // Formik records a single checkbox as a boolean.
                result["newPrompt" + prompt_1.id] = false;
            }
            else {
                // Formik records a checkbox group as an array of strings.
                result["newPrompt" + prompt_1.id] = [];
            }
        }
        else {
            // Formik records a text input or dropdown selection as a string.
            result["newPrompt" + prompt_1.id] = "";
        }
    }
    return result;
};
var EventDetail = /** @class */ (function (_super) {
    __extends(EventDetail, _super);
    function EventDetail(props) {
        var _this = _super.call(this, props) || this;
        _this.toggleModal = _this.toggleModal.bind(_this);
        _this.toggleRemoveRsvpModal = _this.toggleRemoveRsvpModal.bind(_this);
        _this.removeRsvp = _this.removeRsvp.bind(_this);
        _this.askForRemoveRsvpConfirmation = _this.askForRemoveRsvpConfirmation.bind(_this);
        _this.openNewMemberModal = _this.openNewMemberModal.bind(_this);
        _this.addNewMember = _this.addNewMember.bind(_this);
        _this.editExistingMember = _this.editExistingMember.bind(_this);
        _this.toggleRsvpRemovedModal = _this.toggleRsvpRemovedModal.bind(_this);
        _this.cancelEditMember = _this.cancelEditMember.bind(_this);
        _this.openEditEventModal = _this.openEditEventModal.bind(_this);
        _this.toggleEditEventModal = _this.toggleEditEventModal.bind(_this);
        _this.setEditEventModalActiveTab = _this.setEditEventModalActiveTab.bind(_this);
        return _this;
    }
    // This method is called when the component is first added to the document
    EventDetail.prototype.componentDidMount = function () {
        this.ensureDataFetched();
    };
    // This method is called when the route parameters change
    EventDetail.prototype.componentDidUpdate = function () {
        this.ensureDataFetched();
    };
    EventDetail.prototype.canEditMember = function () {
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
    };
    EventDetail.prototype.canSetOrganizer = function () {
        // TODO: These are business rules that belong in the domain layer. The results can be passed down in the Response object
        // User is event author
        if (this.props.actingMember !== undefined && this.props.actingMember.isAuthor) {
            return true;
        }
        return false;
    };
    EventDetail.prototype.canSetAttending = function (isOrganizerChecked) {
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
    };
    EventDetail.prototype.userIsEditingOwnEntry = function () {
        // New member
        if (this.props.modalMember === undefined) {
            return false;
        }
        // User not logged in as someone in this list
        if (this.props.actingMember === undefined) {
            return false;
        }
        return this.props.actingMember.emailAddress === this.props.modalMember.emailAddress;
    };
    EventDetail.prototype.canWithdrawRsvpWhenEditing = function () {
        return this.props.modalMember !== undefined && !this.props.modalMember.isAuthor;
    };
    EventDetail.prototype.renderViewOnlyModalContent = function (event, member, responses) {
        if (member === undefined) {
            return React.createElement(React.Fragment, null,
                React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal }, "Member Info Not Found"),
                React.createElement(reactstrap_1.ModalBody, null, "Sorry! We couldn't find the member info you're looking for."));
        }
        return React.createElement(React.Fragment, null,
            React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal }, member.name),
            React.createElement(reactstrap_1.ModalBody, null,
                React.createElement(reactstrap_1.ListGroup, null,
                    member.emailAddress ? React.createElement(reactstrap_1.ListGroupItem, null,
                        "\u2709",
                        member.emailAddress) : React.createElement(React.Fragment, null),
                    React.createElement(reactstrap_1.ListGroupItem, null,
                        member.isOrganizer ? "⭐ Organizer" : "",
                        !member.isOrganizer && member.isAttending ? "🧑 Attending" : "",
                        !member.isOrganizer && !member.isAttending && member.isAuthor ? "⭐ Author" : ""),
                    this.joinResponsesToPrompts(responses, event.form).sort(function (x) { return x.prompt.sortOrder; }).map(function (promptWithResponse) {
                        return promptWithResponse.responses !== undefined
                            ? React.createElement(reactstrap_1.ListGroupItem, { key: promptWithResponse.prompt.id },
                                React.createElement(reactstrap_1.ListGroupItemText, null, promptWithResponse.prompt.prompt),
                                React.createElement("ul", null, promptWithResponse.responses.responses.map(function (individualResponse) { return React.createElement("li", { key: individualResponse }, individualResponse); })))
                            : React.createElement(React.Fragment, null);
                    }),
                    React.createElement(reactstrap_1.ListGroupItem, null,
                        "\u2714",
                        React.createElement(DateTime_1.default, { date: member.rsvpedAtLocal, timeZoneOffset: event.timeZoneOffset, id: "eventDetail_memberModal_rsvpedAt" })))),
            React.createElement(reactstrap_1.ModalFooter, null,
                this.renderEditMemberButton(),
                ' ',
                React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.toggleModal }, "Close")));
    };
    EventDetail.prototype.renderEditMemberButton = function () {
        // User is not logged in
        if (UserStore_1.getStoredVerifiedEmail() === undefined) {
            return React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.props.openLoginModal }, "Log In To Edit");
        }
        if (this.canEditMember()) {
            return React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.props.openEditExistingMemberModal }, "Edit");
        }
        return React.createElement(React.Fragment, null);
    };
    EventDetail.prototype.renderAddNewMemberModalContent = function (event) {
        var _this = this;
        return React.createElement(formik_1.Formik, { initialValues: getNewMemberInitialValues(event.form), onSubmit: function (values) { _this.addNewMember(values); } }, function (formik) { return (React.createElement(formik_1.Form, { translate: undefined },
            React.createElement(reactstrap_1.ModalHeader, { toggle: _this.toggleModal },
                "RSVP for ",
                _this.props.fursvpEvent ? _this.props.fursvpEvent.name : ""),
            React.createElement(reactstrap_1.ModalBody, null,
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(RsvpTextInput, { id: "newMemberName", label: "Name", required: true })),
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(RsvpTextInput, { id: "newMemberEmail", label: "Email", required: true })),
                _this.canSetOrganizer()
                    ? React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpCheckbox, { id: "newMemberIsOrganizer", label: "Is Organizer" }))
                    : React.createElement(React.Fragment, null),
                _this.canSetAttending(formik.values.newMemberIsOrganizer)
                    ? React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpCheckbox, { id: "newMemberIsAttending", label: "Is Attending" }))
                    : React.createElement(React.Fragment, null),
                event.form.sort(function (x) { return x.sortOrder; }).map(function (prompt) {
                    return React.createElement(reactstrap_1.FormGroup, { key: prompt.id },
                        prompt.behavior === 'Text'
                            ?
                                React.createElement(RsvpTextInput, { id: "newPrompt" + prompt.id, label: prompt.prompt, required: prompt.required })
                            : React.createElement(React.Fragment, null),
                        prompt.behavior === 'Checkboxes'
                            ? React.createElement(RsvpCheckboxGroup, { id: "newPrompt" + prompt.id, label: prompt.prompt, options: prompt.options })
                            : React.createElement(React.Fragment, null),
                        prompt.behavior === 'Dropdown'
                            ? React.createElement(RsvpDropdown, { label: prompt.prompt, id: "newPrompt" + prompt.id, required: prompt.required },
                                React.createElement("option", { key: "", value: "" }, "Select one..."),
                                React.createElement(React.Fragment, null, prompt.options.map(function (option) { return React.createElement("option", { key: option }, option); })))
                            : React.createElement(React.Fragment, null));
                })),
            React.createElement(reactstrap_1.ModalFooter, null,
                React.createElement(reactstrap_1.Button, { type: "submit", color: "primary", disabled: _this.props.isSaving }, "Add RSVP"),
                ' ',
                React.createElement(reactstrap_1.Button, { color: "secondary", onClick: _this.toggleModal, disabled: _this.props.isSaving }, "Cancel")))); });
    };
    EventDetail.prototype.renderEditMemberModalContent = function (event, member) {
        var _this = this;
        if (member === undefined) {
            return this.renderAddNewMemberModalContent(event);
        }
        return React.createElement(formik_1.Formik, { initialValues: this.getExistingMemberInitialValues(event.form, member), onSubmit: function (values) { _this.editExistingMember(member, values); } }, function (formik) { return (React.createElement(formik_1.Form, { translate: undefined },
            React.createElement(reactstrap_1.ModalHeader, { toggle: _this.toggleModal },
                "Edit RSVP for ",
                _this.props.fursvpEvent ? _this.props.fursvpEvent.name : ""),
            React.createElement(reactstrap_1.ModalBody, null,
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(RsvpTextInput, { id: "editMemberName", label: "Name", required: true })),
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(RsvpTextInput, { id: "editMemberEmail", label: "Email", required: true })),
                _this.canSetOrganizer()
                    ? React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpCheckbox, { id: "editMemberIsOrganizer", label: "Is Organizer" }))
                    : React.createElement(React.Fragment, null),
                _this.canSetAttending(formik.values.editMemberIsOrganizer)
                    ? React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpCheckbox, { id: "editMemberIsAttending", label: "Is Attending" }))
                    : React.createElement(React.Fragment, null),
                event.form.sort(function (x) { return x.sortOrder; }).map(function (prompt) {
                    return React.createElement(reactstrap_1.FormGroup, { key: prompt.id },
                        prompt.behavior === 'Text'
                            ?
                                React.createElement(RsvpTextInput, { id: "editPrompt" + prompt.id, label: prompt.prompt, required: prompt.required })
                            : React.createElement(React.Fragment, null),
                        prompt.behavior === 'Checkboxes'
                            ? React.createElement(RsvpCheckboxGroup, { id: "editPrompt" + prompt.id, label: prompt.prompt, options: prompt.options })
                            : React.createElement(React.Fragment, null),
                        prompt.behavior === 'Dropdown'
                            ? React.createElement(RsvpDropdown, { label: prompt.prompt, id: "editPrompt" + prompt.id, required: prompt.required },
                                React.createElement("option", { key: "", value: "" }, "Select one..."),
                                React.createElement(React.Fragment, null, prompt.options.map(function (option) { return React.createElement("option", { key: option }, option); })))
                            : React.createElement(React.Fragment, null));
                })),
            React.createElement(reactstrap_1.ModalFooter, null,
                React.createElement(reactstrap_1.Button, { type: "submit", color: "primary", disabled: _this.props.isSaving }, "Save Changes"),
                ' ',
                React.createElement(reactstrap_1.Button, { color: "secondary", onClick: _this.cancelEditMember, disabled: _this.props.isSaving }, "Cancel"),
                _this.canWithdrawRsvpWhenEditing()
                    ? React.createElement(React.Fragment, null,
                        ' ',
                        React.createElement(reactstrap_1.Button, { outline: true, color: "danger", onClick: _this.askForRemoveRsvpConfirmation, disabled: _this.props.isSaving }, "Remove RSVP"))
                    : React.createElement(React.Fragment, null)))); });
    };
    EventDetail.prototype.rsvpsAreClosed = function (event) {
        if (!event.rsvpOpen) {
            return true;
        }
        if (event.rsvpClosesInMs !== null && event.rsvpClosesInMs <= 0) {
            return true;
        }
        return false;
    };
    EventDetail.prototype.canAddRsvpWhenClosed = function (member) {
        if (member === undefined) {
            return false;
        }
        return member.isAuthor || member.isOrganizer;
    };
    EventDetail.prototype.renderAddRsvpButtonContent = function (event, rsvpsAreClosed, canAddRsvpWhenClosed) {
        if (rsvpsAreClosed && !canAddRsvpWhenClosed) {
            return React.createElement(React.Fragment, null, "RSVPs are not open at this time.");
        }
        return (React.createElement(React.Fragment, null,
            "Add an RSVP",
            !rsvpsAreClosed && event.rsvpClosesAtLocal != null
                ? React.createElement(React.Fragment, null,
                    React.createElement("br", null),
                    React.createElement("small", null,
                        "RSVPs are open until ",
                        React.createElement(DateTime_1.default, { date: event.rsvpClosesAtLocal, timeZoneOffset: event.timeZoneOffset, id: "eventDetailRsvpsCloseAt" })))
                : React.createElement(React.Fragment, null),
            rsvpsAreClosed && canAddRsvpWhenClosed
                ? React.createElement(React.Fragment, null,
                    React.createElement("br", null),
                    React.createElement("small", null, "RSVPs are open only to organizers as this time"))
                : React.createElement(React.Fragment, null)));
    };
    EventDetail.prototype.render = function () {
        var _this = this;
        if (this.props.fursvpEvent !== undefined) {
            var event = this.props.fursvpEvent;
            var member = this.props.modalMember;
            var responses = this.props.modalMember !== undefined ? this.props.modalMember.responses : [];
            var rsvpsAreClosed = this.rsvpsAreClosed(event);
            var canAddRsvpWhenClosed = this.canAddRsvpWhenClosed(this.props.actingMember);
            var padlock = React.createElement(React.Fragment, null);
            if (!event.isPublished) {
                padlock = React.createElement(React.Fragment, null,
                    React.createElement("span", { id: "privateEventIndicator", role: "img", "aria-label": "This event is visible only to you." }, "\uD83D\uDD12"),
                    React.createElement(reactstrap_1.UncontrolledTooltip, { target: "privateEventIndicator" }, "This event is visible only to you."));
            }
            return (React.createElement(React.Fragment, null,
                React.createElement("h1", { id: "tabelLabel" },
                    event.name,
                    padlock,
                    this.props.actingMember !== undefined && (this.props.actingMember.isAuthor || this.props.actingMember.isOrganizer)
                        ? React.createElement(React.Fragment, null,
                            ' ',
                            React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.openEditEventModal }, "Edit"))
                        : React.createElement(React.Fragment, null)),
                React.createElement(reactstrap_1.Container, null,
                    React.createElement("span", { className: "text-muted" }, "Starts"),
                    " ",
                    React.createElement(DateTime_1.default, { date: event.startsAtLocal, timeZoneOffset: event.timeZoneOffset, id: "eventDetail_startsAt" })),
                React.createElement(reactstrap_1.Container, null,
                    React.createElement("span", { className: "text-muted" }, "Ends"),
                    " ",
                    React.createElement(DateTime_1.default, { date: event.endsAtLocal, timeZoneOffset: event.timeZoneOffset, id: "eventDetail_endsAt" })),
                React.createElement(reactstrap_1.Container, null,
                    React.createElement("span", { className: "text-muted" }, "Location"),
                    " ",
                    event.location),
                React.createElement(reactstrap_1.Container, null, event.otherDetails),
                React.createElement(reactstrap_1.Container, null,
                    React.createElement(reactstrap_1.ListGroup, null,
                        React.createElement(reactstrap_1.ListGroupItem, { active: true, tag: "button", action: true, onClick: this.openNewMemberModal, disabled: rsvpsAreClosed && !canAddRsvpWhenClosed }, this.renderAddRsvpButtonContent(event, rsvpsAreClosed, canAddRsvpWhenClosed)),
                        event.members.map(function (member) {
                            return React.createElement(reactstrap_1.ListGroupItem, { key: member.id, tag: "button", action: true, onClick: _this.showMember.bind(_this, member) },
                                _this.memberTypeEmoji(member),
                                member.name);
                        }))),
                React.createElement(reactstrap_1.Modal, { isOpen: this.props.memberModalIsOpen, toggle: this.toggleModal }, this.props.modalIsInEditMemberMode
                    ? this.renderEditMemberModalContent(event, member)
                    : this.renderViewOnlyModalContent(event, member, responses)),
                React.createElement(reactstrap_1.Modal, { isOpen: this.props.editEventModalIsOpen, toggle: this.toggleEditEventModal },
                    React.createElement(formik_1.Formik, { initialValues: this.getEditEventFormInitialValues(this.props.fursvpEvent), onSubmit: function (values) { _this.saveEventChanges(values); } }, function (formik) { return (React.createElement(formik_1.Form, { translate: undefined },
                        React.createElement(reactstrap_1.ModalBody, null,
                            React.createElement(reactstrap_1.Nav, { tabs: true },
                                React.createElement(reactstrap_1.NavItem, null,
                                    React.createElement(reactstrap_1.NavLink, { className: _this.props.editEventModalActiveTab == 'editEventDetailsTab' ? "active" : "", onClick: _this.setEditEventModalActiveTab.bind(_this, 'editEventDetailsTab') }, "Details")),
                                React.createElement(reactstrap_1.NavItem, null,
                                    React.createElement(reactstrap_1.NavLink, { className: _this.props.editEventModalActiveTab == 'editEventFormTab' ? "active" : "", onClick: _this.setEditEventModalActiveTab.bind(_this, 'editEventFormTab') }, "RSVP Form")),
                                React.createElement(reactstrap_1.NavItem, null,
                                    React.createElement(reactstrap_1.NavLink, { className: _this.props.editEventModalActiveTab == 'editEventPublishTab' ? "active" : "", onClick: _this.setEditEventModalActiveTab.bind(_this, 'editEventPublishTab') }, "Publish"))),
                            React.createElement(reactstrap_1.TabContent, { activeTab: _this.props.editEventModalActiveTab },
                                React.createElement(reactstrap_1.TabPane, { tabId: "editEventDetailsTab" },
                                    React.createElement(reactstrap_1.FormGroup, null,
                                        React.createElement(RsvpDateTimePicker, { label: "Starts At", id: "eventStartsAt" })),
                                    React.createElement(reactstrap_1.FormGroup, null,
                                        React.createElement(RsvpDateTimePicker, { label: "Ends At", id: "eventEndsAt" }))),
                                React.createElement(reactstrap_1.TabPane, { tabId: "editEventFormTab" }, "RSVP Form"),
                                React.createElement(reactstrap_1.TabPane, { tabId: "editEventPublishTab" }, "Publish"))),
                        React.createElement(reactstrap_1.ModalFooter, null,
                            React.createElement(reactstrap_1.TabContent, { activeTab: _this.props.editEventModalActiveTab },
                                React.createElement(reactstrap_1.TabPane, { tabId: "editEventDetailsTab" },
                                    React.createElement(reactstrap_1.Button, { color: "primary", onClick: _this.setEditEventModalActiveTab.bind(_this, 'editEventFormTab') }, "Next")),
                                React.createElement(reactstrap_1.TabPane, { tabId: "editEventFormTab" },
                                    React.createElement(reactstrap_1.Button, { color: "secondary", onClick: _this.setEditEventModalActiveTab.bind(_this, 'editEventDetailsTab') }, "Back"),
                                    ' ',
                                    React.createElement(reactstrap_1.Button, { color: "primary", onClick: _this.setEditEventModalActiveTab.bind(_this, 'editEventPublishTab') }, "Next")),
                                React.createElement(reactstrap_1.TabPane, { tabId: "editEventPublishTab" },
                                    React.createElement(reactstrap_1.Button, { color: "secondary", onClick: _this.setEditEventModalActiveTab.bind(_this, 'editEventFormTab') }, "Back"),
                                    ' ',
                                    React.createElement(reactstrap_1.Button, { color: "primary", onClick: _this.toggleEditEventModal }, "Save")))))); })),
                React.createElement(reactstrap_1.Modal, { isOpen: this.props.isAskingForRemoveRsvpConfirmation, toggle: this.toggleRemoveRsvpModal },
                    React.createElement(reactstrap_1.ModalHeader, null, "Remove RSVP?"),
                    React.createElement(reactstrap_1.ModalBody, null,
                        "Please confirm that you wish to withdraw ",
                        member !== undefined ? member.name : "this member",
                        "'s RSVP from this event."),
                    React.createElement(reactstrap_1.ModalFooter, null,
                        React.createElement(reactstrap_1.Button, { color: "danger", onClick: function () { return _this.removeRsvp(event.id, member !== undefined ? member.id : undefined); } }, "Remove RSVP"),
                        ' ',
                        React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.toggleRemoveRsvpModal }, "Cancel"))),
                React.createElement(reactstrap_1.Modal, { isOpen: this.props.rsvpRemovedModalIsOpen, toggle: this.toggleRsvpRemovedModal },
                    React.createElement(reactstrap_1.ModalHeader, null, "RSVP Removed"),
                    React.createElement(reactstrap_1.ModalBody, null, "This RSVP has been removed."),
                    React.createElement(reactstrap_1.ModalFooter, null,
                        React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.toggleRsvpRemovedModal }, "Close")))));
        }
        else if (this.props.isLoading) {
            return (React.createElement(React.Fragment, null, "(Loading)"));
        }
        else {
            //Not loading and no event defined means we 404ed
            return React.createElement(react_router_1.Redirect, { to: "/" });
        }
    };
    EventDetail.prototype.getEditEventFormInitialValues = function (event) {
        var result = {
            eventName: event.name,
            eventStartsAt: event.startsAtLocal,
            eventEndsAt: event.endsAtLocal,
            timeZoneId: event.timeZoneId,
            location: event.location,
            otherDetails: event.otherDetails,
            isPublished: event.isPublished,
            rsvpOpen: event.rsvpOpen,
            rsvpClosesAt: event.rsvpClosesAtLocal
        };
        for (var _i = 0, _a = event.form; _i < _a.length; _i++) {
            var prompt = _a[_i];
            result["editEventPrompt" + prompt.id] = prompt.prompt;
            result["editEventPromptBehavior" + prompt.id] = prompt.behavior;
            result["editEventPromptRequired" + prompt.id] = prompt.required;
            result["editEventPromptSortOrder" + prompt.id] = prompt.sortOrder;
            for (var i = 0; i < prompt.options.length; i++) {
                result["editEventPromptOption" + prompt.id + "_" + i] = prompt.options[i];
            }
        }
        return result;
    };
    EventDetail.prototype.getExistingMemberInitialValues = function (prompts, member) {
        var result = {
            editMemberName: member.name,
            editMemberEmail: member.emailAddress,
            editMemberIsAttending: member.isAttending,
            editMemberIsOrganizer: member.isOrganizer
        };
        var promptsWithResponses = this.joinResponsesToPrompts(member.responses, prompts);
        for (var _i = 0, promptsWithResponses_1 = promptsWithResponses; _i < promptsWithResponses_1.length; _i++) {
            var item = promptsWithResponses_1[_i];
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
    };
    EventDetail.prototype.joinResponsesToPrompts = function (responses, prompts) {
        var result = [];
        for (var _i = 0, prompts_1 = prompts; _i < prompts_1.length; _i++) {
            var prompt_2 = prompts_1[_i];
            var responsesForPrompt = undefined;
            for (var _a = 0, responses_1 = responses; _a < responses_1.length; _a++) {
                var response = responses_1[_a];
                if (response.promptId === prompt_2.id) {
                    responsesForPrompt = response;
                    break;
                }
            }
            result.push({ prompt: prompt_2, responses: responsesForPrompt });
        }
        return result;
    };
    EventDetail.prototype.toolTip = function (text, toolTipText, id) {
        return React.createElement(React.Fragment, null,
            React.createElement("span", { id: id }, text),
            React.createElement(reactstrap_1.UncontrolledTooltip, { target: id }, toolTipText));
    };
    EventDetail.prototype.memberTypeEmoji = function (member) {
        var id = "member_" + member.id;
        var suffix = member.isAttending
            ? ""
            : " (not attending)";
        if (member.isOrganizer) {
            return this.toolTip("⭐", "Organizer" + suffix, id);
        }
        if (member.isAttending) {
            return React.createElement(React.Fragment, null);
            //return this.toolTip("🧑", "Attending", id);
        }
        if (member.isAuthor) {
            return this.toolTip("⭐", "Author" + suffix, id);
        }
        return React.createElement(React.Fragment, null);
    };
    EventDetail.prototype.showMember = function (member) {
        this.props.history.push('/event/' + this.props.id + '/member/' + member.id);
        this.props.openMemberModal(member);
    };
    EventDetail.prototype.openEditEventModal = function () {
        this.props.history.push("/event/" + this.props.id + "/edit");
        this.props.openEditEventModal();
    };
    EventDetail.prototype.toggleEditEventModal = function () {
        this.props.history.push("/event/" + this.props.id);
        this.props.toggleEditEventModal();
    };
    EventDetail.prototype.openNewMemberModal = function () {
        this.props.openNewMemberModal();
    };
    EventDetail.prototype.addNewMember = function (values) {
        this.props.addNewMember(values);
    };
    EventDetail.prototype.editExistingMember = function (member, values) {
        this.props.editExistingMember(member, values);
    };
    EventDetail.prototype.saveEventChanges = function (values) {
        this.props.saveEventChanges(values);
    };
    EventDetail.prototype.toggleModal = function () {
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
    };
    EventDetail.prototype.cancelEditMember = function () {
        this.props.cancelEditMember();
    };
    EventDetail.prototype.toggleRemoveRsvpModal = function () {
        this.props.toggleRemoveRsvpModal();
    };
    EventDetail.prototype.toggleRsvpRemovedModal = function () {
        this.props.history.push('/event/' + this.props.id);
        this.props.toggleRsvpRemovedModal();
    };
    EventDetail.prototype.removeRsvp = function (eventId, memberId) {
        this.props.removeRsvp(eventId, memberId);
    };
    EventDetail.prototype.askForRemoveRsvpConfirmation = function () {
        this.props.askForRemoveRsvpConfirmation();
    };
    EventDetail.prototype.ensureDataFetched = function () {
        this.props.requestFursvpEvent(this.props.match.params.eventId, this.props.match.params.memberId, this.props.match.path.search("edit") !== -1, this.props.history);
    };
    EventDetail.prototype.setEditEventModalActiveTab = function (tabId) {
        this.props.setEditEventModalActiveTab(tabId);
    };
    return EventDetail;
}(React.PureComponent));
exports.default = react_redux_1.connect(function (state) { return state.targetEvent; }, // Selects which state properties are merged into the component's props
EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail);
//# sourceMappingURL=EventDetail.js.map