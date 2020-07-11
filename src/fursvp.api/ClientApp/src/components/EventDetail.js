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
var EventDetailStore = require("../store/EventDetailStore");
var DateTime_1 = require("./DateTime");
var EventDetail = /** @class */ (function (_super) {
    __extends(EventDetail, _super);
    function EventDetail() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    // This method is called when the component is first added to the document
    EventDetail.prototype.componentDidMount = function () {
        this.ensureDataFetched();
    };
    // This method is called when the route parameters change
    EventDetail.prototype.componentDidUpdate = function () {
        this.ensureDataFetched();
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
            return this.toolTip("🧑", "Attending", id);
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
    EventDetail.prototype.render = function () {
        var _this = this;
        if (this.props.fursvpEvent !== undefined) {
            var event = this.props.fursvpEvent;
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
                    React.createElement(DateTime_1.default, { date: event.startsAt, timeZoneId: event.timeZoneId, id: "eventDetail_startsAt" })),
                React.createElement(reactstrap_1.Container, null,
                    React.createElement("span", { className: "text-muted" }, "Ends"),
                    " ",
                    React.createElement(DateTime_1.default, { date: event.endsAt, timeZoneId: event.timeZoneId, id: "eventDetail_endsAt" })),
                React.createElement(reactstrap_1.Container, null,
                    React.createElement("span", { className: "text-muted" }, "Location"),
                    " ",
                    event.location),
                React.createElement(reactstrap_1.Container, null, event.otherDetails),
                React.createElement(reactstrap_1.Container, null, event.members.map(function (member) {
                    return React.createElement("div", { key: member.id, className: "container-fluid" },
                        _this.memberTypeEmoji(member),
                        member.name,
                        _this.emailToolTip(member));
                }))));
        }
        else {
            return (React.createElement(React.Fragment, null, "(Loading)"));
        }
    };
    EventDetail.prototype.ensureDataFetched = function () {
        this.props.requestFursvpEvent(this.props.match.params.id);
    };
    return EventDetail;
}(React.PureComponent));
exports.default = react_redux_1.connect(function (state) { return state.targetEvent; }, // Selects which state properties are merged into the component's props
EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail);
//# sourceMappingURL=EventDetail.js.map