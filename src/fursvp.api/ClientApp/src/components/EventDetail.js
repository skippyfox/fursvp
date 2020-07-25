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
var RsvpCheckboxes = function (props) {
    var _a = formik_1.useField({ id: props.id, name: props.id }), field = _a[0], meta = _a[1];
    return (React.createElement(reactstrap_1.Label, { check: true, id: props.id },
        React.createElement(reactstrap_1.Container, null, props.label),
        props.options.map(function (option) { return React.createElement(reactstrap_1.Container, null,
            React.createElement(reactstrap_1.Input, { id: props.id + option, key: option, type: "checkbox" }),
            ' ',
            option); })));
};
var getNewMemberInitialValues = function (form) {
    var result = {
        newMemberName: "",
        newMemberEmail: ""
    };
    for (var _i = 0, form_1 = form; _i < form_1.length; _i++) {
        var prompt_1 = form_1[_i];
        if (prompt_1.behavior == "Checkboxes") {
            for (var _a = 0, _b = prompt_1.options; _a < _b.length; _a++) {
                var option = _b[_a];
                result["newPrompt" + prompt_1.id + option] = "";
            }
        }
        else {
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
    EventDetail.prototype.canEditMember = function (userEmail) {
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
    };
    EventDetail.prototype.getAuthor = function () {
        if (this.props.fursvpEvent === undefined) {
            return undefined;
        }
        for (var _i = 0, _a = this.props.fursvpEvent.members; _i < _a.length; _i++) {
            var member = _a[_i];
            if (member.isAuthor) {
                return member;
            }
        }
        return undefined;
    };
    EventDetail.prototype.renderViewOnlyModalContent = function (event, member, responses, userEmail) {
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
                                React.createElement(reactstrap_1.ListGroupItemHeading, null, promptWithResponse.prompt.prompt),
                                promptWithResponse.responses.responses.map(function (individualResponse) { return React.createElement(reactstrap_1.ListGroupItemText, { key: individualResponse }, individualResponse); }))
                            : React.createElement(React.Fragment, null);
                    }),
                    React.createElement(reactstrap_1.ListGroupItem, null,
                        "\u2714",
                        React.createElement(DateTime_1.default, { date: member.rsvpedAtLocal, timeZoneOffset: event.timeZoneOffset, id: "eventDetail_memberModal_rsvpedAt" })))),
            React.createElement(reactstrap_1.ModalFooter, null,
                userEmail === undefined
                    ? React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.props.openLoginModal }, "Log In To Edit")
                    : React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.props.openEditExistingMemberModal, disabled: !this.canEditMember(userEmail) }, "Edit"),
                ' ',
                React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.toggleModal }, "Close")));
    };
    EventDetail.prototype.renderAddNewMemberModalContent = function (event) {
        var _this = this;
        return React.createElement(formik_1.Formik, { initialValues: getNewMemberInitialValues(event.form), onSubmit: function (values, _a) {
                var setSubmitting = _a.setSubmitting;
                _this.addNewMember(values);
            } },
            React.createElement(formik_1.Form, { translate: undefined },
                React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal },
                    "RSVP for ",
                    this.props.fursvpEvent ? this.props.fursvpEvent.name : ""),
                React.createElement(reactstrap_1.ModalBody, null,
                    React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpTextInput, { id: "newMemberName", label: "Name", required: true })),
                    React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpTextInput, { id: "newMemberEmail", label: "Email", required: true })),
                    event.form.sort(function (x) { return x.sortOrder; }).map(function (prompt) {
                        return React.createElement(reactstrap_1.FormGroup, { key: prompt.id, check: prompt.behavior == 'Checkboxes' },
                            prompt.behavior == 'Text'
                                ?
                                    React.createElement(RsvpTextInput, { id: "newPrompt" + prompt.id, label: prompt.prompt, required: prompt.required })
                                : React.createElement(React.Fragment, null),
                            prompt.behavior == 'Checkboxes'
                                ? React.createElement(RsvpCheckboxes, { id: "newPrompt" + prompt.id, label: prompt.prompt, options: prompt.options })
                                : React.createElement(React.Fragment, null),
                            prompt.behavior == 'Dropdown'
                                ? React.createElement(RsvpDropdown, { label: prompt.prompt, id: "newPrompt" + prompt.id, required: prompt.required },
                                    React.createElement("option", { key: "", value: "" }, "Select one..."),
                                    React.createElement(React.Fragment, null, prompt.options.map(function (option) { return React.createElement("option", { key: option }, option); })))
                                : React.createElement(React.Fragment, null));
                    })),
                React.createElement(reactstrap_1.ModalFooter, null,
                    React.createElement(reactstrap_1.Button, { type: "submit", color: "primary", disabled: this.props.isSaving }, "Add RSVP"),
                    ' ',
                    React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.toggleModal, disabled: this.props.isSaving }, "Cancel"))));
    };
    EventDetail.prototype.renderEditMemberModalContent = function (event, member, responses) {
        var _this = this;
        if (member === undefined) {
            return this.renderAddNewMemberModalContent(event);
        }
        return React.createElement(formik_1.Formik, { initialValues: this.getExistingMemberInitialValues(event.form, member), onSubmit: function (values, _a) {
                var setSubmitting = _a.setSubmitting;
                _this.editExistingMember(member.id, values);
            } },
            React.createElement(formik_1.Form, { translate: undefined },
                React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal },
                    "Edit RSVP for ",
                    this.props.fursvpEvent ? this.props.fursvpEvent.name : ""),
                React.createElement(reactstrap_1.ModalBody, null,
                    React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpTextInput, { id: "editMemberName", label: "Name", required: true })),
                    React.createElement(reactstrap_1.FormGroup, null,
                        React.createElement(RsvpTextInput, { id: "editMemberEmail", label: "Email", required: true })),
                    event.form.sort(function (x) { return x.sortOrder; }).map(function (prompt) {
                        return React.createElement(reactstrap_1.FormGroup, { key: prompt.id, check: prompt.behavior == 'Checkboxes' },
                            prompt.behavior == 'Text'
                                ?
                                    React.createElement(RsvpTextInput, { id: "editPrompt" + prompt.id, label: prompt.prompt, required: prompt.required })
                                : React.createElement(React.Fragment, null),
                            prompt.behavior == 'Checkboxes'
                                ? React.createElement(RsvpCheckboxes, { id: "editPrompt" + prompt.id, label: prompt.prompt, options: prompt.options })
                                : React.createElement(React.Fragment, null),
                            prompt.behavior == 'Dropdown'
                                ? React.createElement(RsvpDropdown, { label: prompt.prompt, id: "editPrompt" + prompt.id, required: prompt.required },
                                    React.createElement("option", { key: "", value: "" }, "Select one..."),
                                    React.createElement(React.Fragment, null, prompt.options.map(function (option) { return React.createElement("option", { key: option }, option); })))
                                : React.createElement(React.Fragment, null));
                    })),
                React.createElement(reactstrap_1.ModalFooter, null,
                    React.createElement(reactstrap_1.Button, { type: "submit", color: "primary", disabled: this.props.isSaving }, "Save Changes"),
                    ' ',
                    React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.cancelEditMember, disabled: this.props.isSaving }, "Cancel"),
                    ' ',
                    React.createElement(reactstrap_1.Button, { outline: true, color: "danger", onClick: this.askForRemoveRsvpConfirmation, disabled: this.props.isSaving }, "Remove RSVP"))));
    };
    EventDetail.prototype.render = function () {
        var _this = this;
        if (this.props.fursvpEvent !== undefined) {
            var event = this.props.fursvpEvent;
            var member = this.props.modalMember;
            var responses = this.props.modalMember !== undefined ? this.props.modalMember.responses : [];
            var userEmail = UserStore_1.getStoredVerifiedEmail();
            var padlock = React.createElement(React.Fragment, null);
            if (!event.isPublished) {
                padlock = React.createElement(React.Fragment, null,
                    React.createElement("span", { id: "privateEventIndicator", role: "img", "aria-label": "private" }, "\uD83D\uDD12"),
                    React.createElement(reactstrap_1.UncontrolledTooltip, { target: "privateEventIndicator" }, "Private Event"));
            }
            return (React.createElement(React.Fragment, null,
                React.createElement("h1", { id: "tabelLabel" },
                    event.name,
                    padlock),
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
                        React.createElement(reactstrap_1.ListGroupItem, { active: true, tag: "button", action: true, onClick: this.openNewMemberModal }, "Add an RSVP"),
                        event.members.map(function (member) {
                            return React.createElement(reactstrap_1.ListGroupItem, { key: member.id, tag: "button", action: true, onClick: _this.showMember.bind(_this, member) },
                                _this.memberTypeEmoji(member),
                                member.name);
                        }))),
                React.createElement(reactstrap_1.Modal, { isOpen: this.props.modalIsOpen, toggle: this.toggleModal }, this.props.modalIsInEditMode
                    ? this.renderEditMemberModalContent(event, member, responses)
                    : this.renderViewOnlyModalContent(event, member, responses, userEmail)),
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
    EventDetail.prototype.getExistingMemberInitialValues = function (prompts, member) {
        var result = {
            editMemberName: member.name,
            editMemberEmail: member.emailAddress
        };
        var promptsWithResponses = this.joinResponsesToPrompts(member.responses, prompts);
        for (var _i = 0, promptsWithResponses_1 = promptsWithResponses; _i < promptsWithResponses_1.length; _i++) {
            var item = promptsWithResponses_1[_i];
            if (item.prompt.behavior == "Checkboxes") {
                for (var _a = 0, _b = item.prompt.options; _a < _b.length; _a++) {
                    var option = _b[_a];
                    result["editPrompt" + item.prompt.id + option] = false;
                    if (item.responses !== undefined) {
                        for (var _c = 0, _d = item.responses.responses; _c < _d.length; _c++) {
                            var response = _d[_c];
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
    };
    EventDetail.prototype.joinResponsesToPrompts = function (responses, prompts) {
        var result = [];
        for (var _i = 0, prompts_1 = prompts; _i < prompts_1.length; _i++) {
            var prompt_2 = prompts_1[_i];
            var responsesForPrompt = undefined;
            for (var _a = 0, responses_1 = responses; _a < responses_1.length; _a++) {
                var response = responses_1[_a];
                if (response.promptId == prompt_2.id) {
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
        if (member.isOrganizer) {
            return this.toolTip("⭐", "Organizer", id);
        }
        if (member.isAttending) {
            return React.createElement(React.Fragment, null);
            //return this.toolTip("🧑", "Attending", id);
        }
        if (member.isAuthor) {
            return this.toolTip("⭐", "Author", id);
        }
        return React.createElement(React.Fragment, null);
    };
    EventDetail.prototype.showMember = function (member) {
        this.props.history.push('/event/' + this.props.id + '/member/' + member.id);
        this.props.openModal(member);
    };
    EventDetail.prototype.openNewMemberModal = function () {
        this.props.openNewMemberModal();
    };
    EventDetail.prototype.addNewMember = function (values) {
        this.props.addNewMember(values);
    };
    EventDetail.prototype.editExistingMember = function (memberId, values) {
        this.props.editExistingMember(memberId, values);
    };
    EventDetail.prototype.toggleModal = function () {
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
        this.props.requestFursvpEvent(this.props.match.params.eventId, this.props.match.params.memberId);
    };
    return EventDetail;
}(React.PureComponent));
exports.default = react_redux_1.connect(function (state) { return state.targetEvent; }, // Selects which state properties are merged into the component's props
EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail);
//# sourceMappingURL=EventDetail.js.map