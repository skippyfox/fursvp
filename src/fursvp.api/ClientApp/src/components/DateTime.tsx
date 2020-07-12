import * as React from 'react';
import { Badge, UncontrolledTooltip } from 'reactstrap';

export default (props: { date: string, timeZoneOffset: string; id: string }) => (
    <>
        <span>{
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
        &nbsp;<Badge id={"timeZoneBadge_" + props.id} color="secondary">T</Badge>
        <UncontrolledTooltip target={"timeZoneBadge_" + props.id}>{props.timeZoneOffset}</UncontrolledTooltip>
    </>
);