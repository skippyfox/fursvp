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
var EventDetail = /** @class */ (function (_super) {
    __extends(EventDetail, _super);
    function EventDetail(props) {
        var _this = _super.call(this, props) || this;
        _this.toggleModal = _this.toggleModal.bind(_this);
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
    EventDetail.prototype.render = function () {
        var _this = this;
        if (this.props.fursvpEvent !== undefined) {
            var event = this.props.fursvpEvent;
            var member = this.props.modalMember;
            var responses = this.props.modalMember !== undefined ? this.props.modalMember.responses : [];
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
                        React.createElement(reactstrap_1.ListGroupItem, { active: true, tag: "button", action: true }, "Add an RSVP"),
                        event.members.map(function (member) {
                            return React.createElement(reactstrap_1.ListGroupItem, { key: member.id, tag: "button", action: true, onClick: _this.showMember.bind(_this, member) },
                                _this.memberTypeEmoji(member),
                                member.name);
                        }))),
                React.createElement(reactstrap_1.Modal, { isOpen: this.props.modalIsOpen, toggle: this.toggleModal }, member !== undefined ?
                    React.createElement(React.Fragment, null,
                        React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal }, member.name),
                        React.createElement(reactstrap_1.ModalBody, null,
                            React.createElement(reactstrap_1.ListGroup, null,
                                member.emailAddress ? React.createElement(reactstrap_1.ListGroupItem, null,
                                    "\u2709",
                                    member.emailAddress) : React.createElement(React.Fragment, null),
                                React.createElement(reactstrap_1.ListGroupItem, null,
                                    "\u2714",
                                    React.createElement(DateTime_1.default, { date: member.rsvpedAtLocal, timeZoneOffset: event.timeZoneOffset, id: "eventDetail_memberModal_rsvpedAt" })),
                                this.matchResponsesToPrompts(responses, event.form).map(function (response) {
                                    return React.createElement(reactstrap_1.ListGroupItem, null,
                                        React.createElement(reactstrap_1.ListGroupItemHeading, null, response.prompt),
                                        response.responses.map(function (individualResponse) { return React.createElement(reactstrap_1.ListGroupItemText, null, individualResponse); }));
                                }))),
                        React.createElement(reactstrap_1.ModalFooter, null,
                            React.createElement(reactstrap_1.Button, { color: "primary", onClick: this.toggleModal }, "Edit"),
                            ' ',
                            React.createElement(reactstrap_1.Button, { color: "secondary", onClick: this.toggleModal }, "Close")))
                    :
                        React.createElement(React.Fragment, null,
                            React.createElement(reactstrap_1.ModalHeader, { toggle: this.toggleModal }, "Member Info Not Found"),
                            React.createElement(reactstrap_1.ModalBody, null, "Sorry! We couldn't find the member info you're looking for.")))));
        }
        else if (this.props.isLoading) {
            return (React.createElement(React.Fragment, null, "(Loading)"));
        }
        else {
            //Not loading and no event defined means we 404ed
            return React.createElement(react_router_1.Redirect, { to: "/" });
        }
    };
    EventDetail.prototype.matchResponsesToPrompts = function (responses, prompts) {
        var result = [];
        for (var _i = 0, responses_1 = responses; _i < responses_1.length; _i++) {
            var r = responses_1[_i];
            for (var _a = 0, prompts_1 = prompts; _a < prompts_1.length; _a++) {
                var p = prompts_1[_a];
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
    EventDetail.prototype.emailToolTip = function (member) {
        if (member.emailAddress === undefined || member.emailAddress === null) {
            return React.createElement(React.Fragment, null);
        }
        return this.toolTip("✉", member.emailAddress, "emailAddress_" + member.id);
    };
    EventDetail.prototype.showMember = function (member) {
        this.props.history.push('/event/' + this.props.id + '/member/' + member.id);
        this.props.openModal(member);
    };
    EventDetail.prototype.toggleModal = function () {
        if (this.props.modalIsOpen) {
            this.props.history.push('/event/' + this.props.id);
        }
        else if (this.props.modalMember !== undefined) {
            this.props.history.push('/event/' + this.props.id + '/member/' + this.props.modalMember.id);
        }
        else {
            return;
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