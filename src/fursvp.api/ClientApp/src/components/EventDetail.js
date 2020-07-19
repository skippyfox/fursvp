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
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var reactstrap_1 = require("reactstrap");
var react_redux_1 = require("react-redux");
var react_router_1 = require("react-router");
var EventDetailStore = require("../store/EventDetailStore");
var DateTime_1 = require("./DateTime");
var UserStore_1 = require("../store/UserStore");
var EventDetail = /** @class */ (function (_super) {
    __extends(EventDetail, _super);
    function EventDetail(props) {
        var _this = _super.call(this, props) || this;
        _this.toggleModal = _this.toggleModal.bind(_this);
        _this.openNewMemberModal = _this.openNewMemberModal.bind(_this);
        _this.addNewMember = _this.addNewMember.bind(_this);
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
        return React.createElement(reactstrap_1.Form, null,
            React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal },
                "RSVP for ",
                this.props.fursvpEvent ? this.props.fursvpEvent.name : ""),
            React.createElement(reactstrap_1.ModalBody, null,
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(reactstrap_1.Label, { for: "newMemberName" }, "Name"),
                    React.createElement(reactstrap_1.Input, { id: "newMemberName", required: true })),
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(reactstrap_1.Label, { for: "newMemberEmail" }, "Email"),
                    React.createElement(reactstrap_1.Input, { type: "email", id: "newMemberEmail", required: true })),
                event.form.sort(function (x) { return x.sortOrder; }).map(function (prompt) {
                    return React.createElement(reactstrap_1.FormGroup, { key: prompt.id, check: prompt.behavior == 'Checkboxes' },
                        React.createElement(reactstrap_1.Label, { for: "newPrompt" + prompt.id }, prompt.prompt),
                        prompt.behavior == 'Text'
                            ? React.createElement(reactstrap_1.Input, { id: "newPrompt" + prompt.id, required: prompt.required })
                            : React.createElement(React.Fragment, null),
                        prompt.behavior == 'Checkboxes'
                            ? React.createElement(reactstrap_1.Label, { check: true, id: "newPrompt" + prompt.id }, prompt.options.map(function (option) { return React.createElement(React.Fragment, null,
                                React.createElement(reactstrap_1.Input, { key: option, type: "checkbox" }),
                                ' ',
                                option); }))
                            : React.createElement(React.Fragment, null),
                        prompt.behavior == 'Dropdown'
                            ? React.createElement(reactstrap_1.Input, { type: "select", id: "newPrompt" + prompt.id, required: prompt.required }, prompt.options.map(function (option) { return React.createElement("option", { key: option }, option); }))
                            : React.createElement(React.Fragment, null));
                })),
            React.createElement(reactstrap_1.ModalFooter, null,
                React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.props.addNewMember }, "Add RSVP"),
                ' ',
                React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.toggleModal }, "Cancel")));
    };
    EventDetail.prototype.renderEditMemberModalContent = function (event, member, responses) {
        if (member === undefined) {
            return this.renderAddNewMemberModalContent(event);
        }
        return React.createElement(reactstrap_1.Form, null,
            React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal },
                "Edit RSVP for ",
                this.props.fursvpEvent ? this.props.fursvpEvent.name : ""),
            React.createElement(reactstrap_1.ModalBody, null,
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(reactstrap_1.Label, { for: "editMemberName" }, "Name"),
                    React.createElement(reactstrap_1.Input, { id: "editMemberName", required: true, value: member.name })),
                React.createElement(reactstrap_1.FormGroup, null,
                    React.createElement(reactstrap_1.Label, { for: "editMemberEmail" }, "Email"),
                    React.createElement(reactstrap_1.Input, { type: "email", id: "editMemberEmail", required: true, value: member.emailAddress !== null ? member.emailAddress : "" })),
                this.joinResponsesToPrompts(responses, event.form).sort(function (x) { return x.prompt.sortOrder; }).map(function (promptWithResponse) {
                    return React.createElement(reactstrap_1.FormGroup, { key: promptWithResponse.prompt.id, check: promptWithResponse.prompt.behavior == 'Checkboxes' },
                        React.createElement(reactstrap_1.Label, { for: "editPrompt" + promptWithResponse.prompt.id }, promptWithResponse.prompt.prompt),
                        promptWithResponse.prompt.behavior == 'Text'
                            ? React.createElement(reactstrap_1.Input, { id: "editPrompt" + promptWithResponse.prompt.id, required: promptWithResponse.prompt.required, value: promptWithResponse.responses !== undefined ? promptWithResponse.responses.responses[0] : "" })
                            : React.createElement(React.Fragment, null),
                        promptWithResponse.prompt.behavior == 'Checkboxes'
                            ? React.createElement(reactstrap_1.Label, { check: true, id: "editPrompt" + promptWithResponse.prompt.id }, promptWithResponse.prompt.options.map(function (option) { return React.createElement(React.Fragment, null,
                                React.createElement(reactstrap_1.Input, { key: option, type: "checkbox", checked: promptWithResponse.responses !== undefined && promptWithResponse.responses.responses.indexOf(option) > -1 }),
                                ' ',
                                option); }))
                            : React.createElement(React.Fragment, null),
                        promptWithResponse.prompt.behavior == 'Dropdown'
                            ? React.createElement(reactstrap_1.Input, { type: "select", id: "editPrompt" + promptWithResponse.prompt.id, required: promptWithResponse.prompt.required }, promptWithResponse.prompt.options.map(function (option) { return React.createElement("option", { key: option, selected: promptWithResponse.responses !== undefined && promptWithResponse.responses.responses[0] == option }, option); }))
                            : React.createElement(React.Fragment, null));
                })),
            React.createElement(reactstrap_1.ModalFooter, null,
                React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.toggleModal }, "Save Changes"),
                ' ',
                React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.toggleModal }, "Cancel"),
                ' ',
                React.createElement(reactstrap_1.Button, { outline: true, color: "danger", onClick: this.toggleModal }, "Remove RSVP")));
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
                    : this.renderViewOnlyModalContent(event, member, responses, userEmail))));
        }
        else if (this.props.isLoading) {
            return (React.createElement(React.Fragment, null, "(Loading)"));
        }
        else {
            //Not loading and no event defined means we 404ed
            return React.createElement(react_router_1.Redirect, { to: "/" });
        }
    };
    EventDetail.prototype.joinResponsesToPrompts = function (responses, prompts) {
        var result = [];
        for (var _i = 0, prompts_1 = prompts; _i < prompts_1.length; _i++) {
            var prompt_1 = prompts_1[_i];
            var responsesForPrompt = undefined;
            for (var _a = 0, responses_1 = responses; _a < responses_1.length; _a++) {
                var response = responses_1[_a];
                if (response.promptId == prompt_1.id) {
                    responsesForPrompt = response;
                    break;
                }
            }
            result.push({ prompt: prompt_1, responses: responsesForPrompt });
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
    EventDetail.prototype.addNewMember = function () {
        this.props.addNewMember();
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
    EventDetail.prototype.ensureDataFetched = function () {
        this.props.requestFursvpEvent(this.props.match.params.eventId, this.props.match.params.memberId);
    };
    return EventDetail;
}(React.PureComponent));
exports.default = react_redux_1.connect(function (state) { return state.targetEvent; }, // Selects which state properties are merged into the component's props
EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail);
//# sourceMappingURL=EventDetail.js.map