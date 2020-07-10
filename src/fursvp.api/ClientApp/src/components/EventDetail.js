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
    EventDetail.prototype.render = function () {
        if (this.props.fursvpEvent !== undefined) {
            return (React.createElement(React.Fragment, null,
                React.createElement("h1", { id: "tabelLabel" }, this.props.fursvpEvent.name),
                React.createElement("br", null),
                React.createElement("div", { key: this.props.fursvpEvent.id, className: "container-fluid" },
                    React.createElement("small", null,
                        this.props.fursvpEvent.startsAt,
                        " | ",
                        this.props.fursvpEvent.location,
                        " \u00A0"),
                    React.createElement(reactstrap_1.Badge, { color: "info" }, this.props.fursvpEvent.members.length))));
        }
        return (React.createElement(React.Fragment, null, "Loading..."));
    };
    EventDetail.prototype.ensureDataFetched = function () {
        this.props.requestFursvpEvent(this.props.match.params.id);
    };
    return EventDetail;
}(React.PureComponent));
exports.default = react_redux_1.connect(function (state) { return state.fursvpEvents; }, // Selects which state properties are merged into the component's props
EventDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(EventDetail);
//# sourceMappingURL=EventDetail.js.map