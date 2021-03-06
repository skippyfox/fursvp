"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var reactstrap_1 = require("reactstrap");
exports.default = (function (props) { return (React.createElement(React.Fragment, null,
    React.createElement("span", { id: "timeZoneSpan_" + props.id }, new Intl.DateTimeFormat("en-US", {
        year: "numeric",
        month: "long",
        day: "2-digit",
        weekday: "long",
        hour: "numeric",
        minute: 'numeric',
        second: 'numeric'
    }).format(new Date(props.date))),
    React.createElement(reactstrap_1.UncontrolledTooltip, { target: "timeZoneSpan_" + props.id }, props.timeZoneOffset))); });
//# sourceMappingURL=DateTime.js.map