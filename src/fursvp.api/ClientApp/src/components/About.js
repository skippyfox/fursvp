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
var react_redux_1 = require("react-redux");
var CounterStore = require("../store/Counter");
var react_router_dom_1 = require("react-router-dom");
var About = /** @class */ (function (_super) {
    __extends(About, _super);
    function About() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    About.prototype.render = function () {
        return (React.createElement(React.Fragment, null,
            React.createElement("div", null,
                React.createElement("h1", null, "A simple community event manager."),
                React.createElement("p", null, "This tool is for organizers looking for a quick solution to share details about an event and get a head count. It's a free and hassle-free alternative to mainstream event services. Signing up for an event is as simple as filling in your name, and creating one is easy too."),
                React.createElement("h1", null, "Open-source."),
                React.createElement("p", null,
                    "FURsvp is available under the MIT license ",
                    React.createElement("a", { href: 'https://github.com/skippyfox/fursvp' }, "on GitHub"),
                    ", where you can view and pull down the source code, review planned additional features, and contribute."),
                React.createElement("p", null, "The latest major update was not only a change to the UI, but an entire system rewrite. Why? The UI hadn't been updated since 2008, nor the back-end since 2010. The UI grew stale, the code grew less manageable and it was full of hardcoded secrets. A rewrite was a good excuse to go open-source and open our arms to help from the community, and to make the codebase a joy to work with again. The solution is now containerized which makes it cheaper to host, too."),
                React.createElement("h1", null, "What are the alternatives?"),
                React.createElement("p", null, "FURsvp was written when few other options were available. Today some are quite prevalent. We actually recommend using Google Forms over FURsvp if you want something fast, you don't want to share who's coming, and you don't need event details to stay visible after you stop accepting RSVPs."),
                React.createElement("ul", null,
                    React.createElement("li", null,
                        React.createElement("a", { href: 'https://www.google.com/forms/about/' }, "Google Forms"),
                        " - Simple to use. Attendees don't need an account to RSVP."),
                    React.createElement("li", null,
                        React.createElement("a", { href: 'https://www.facebook.com/events/' }, "Facebook Events"),
                        " - Better visibility. Details can stay up indefinitely. Attendees can see who else is coming."),
                    React.createElement("li", null,
                        React.createElement("a", { href: 'https://www.meetup.com/' }, "Meetup"),
                        " - Good for communities. Details can stay up indefinitely. Attendees can see who else is coming."),
                    React.createElement("li", null,
                        React.createElement(react_router_dom_1.Link, { to: '/' }, "FURsvp"),
                        " - Good for small communities. Simple to use (we hope!). Attendees don't need an account to RSVP. Details can stay up indefinitely. Attendees can see who else is coming. Free and open-source - extra customizable!")),
                React.createElement("h1", null, "Is this tool just for the furry community?"),
                React.createElement("p", null, "There's no rule against hosting meetups on FURsvp outside of the furry community or private events, but that is how it has been predominantly used. Feel free to use FURsvp for your church group or political campaign rally, but you may be happier forking the source code and hosting the tool independently!"))));
    };
    return About;
}(React.PureComponent));
;
exports.default = react_redux_1.connect(function (state) { return state.counter; }, CounterStore.actionCreators)(About);
//# sourceMappingURL=About.js.map