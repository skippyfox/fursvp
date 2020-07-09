"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var FursvpEvents = require("./FursvpEvents");
// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
exports.reducers = {
    fursvpEvents: FursvpEvents.reducer
};
//# sourceMappingURL=index.js.map