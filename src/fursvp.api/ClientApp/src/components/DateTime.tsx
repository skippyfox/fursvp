import * as React from 'react';
import { Badge, UncontrolledTooltip } from 'reactstrap';

export default (props: { date: string, timeZoneOffset: string; id: string }) => (
    <>
        <span id={"timeZoneSpan_" + props.id}>{
            new Intl.DateTimeFormat("en-US", {
                year: "numeric",
                month: "long",
                day: "2-digit",
                weekday: "long",
                hour: "numeric",
                minute: 'numeric',
                second: 'numeric'
            }).format(new Date(props.date))
        }</span>
        <UncontrolledTooltip target={"timeZoneSpan_" + props.id}>{props.timeZoneOffset}</UncontrolledTooltip>
    </>
);