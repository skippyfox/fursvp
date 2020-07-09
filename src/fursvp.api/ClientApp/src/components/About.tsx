import * as React from 'react';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';

const About = () => (
    <div>
        <h1>A simple community event manager.</h1>
        <p>This tool is for organizers looking for a quick solution to share details about an event and get a head count. It's a free and hassle-free alternative to mainstream event services. Signing up for an event is as simple as filling in your name, and creating one is easy too.</p>
        <h1>Open-source.</h1>
        <p>FURsvp is available under the MIT license <a href='https://github.com/skippyfox/fursvp'>on GitHub</a>, where you can view and pull down the source code, review planned additional features, and contribute.</p>
        <p>The latest major update was not only a change to the UI, but an entire system rewrite. Why? The UI hadn't been updated since 2008, nor the back-end since 2010. The UI grew stale, the code grew less manageable and it was full of hardcoded secrets. A rewrite was a good excuse to go open-source and open our arms to help from the community, and to make the codebase a joy to work with again. The solution is now containerized which makes it cheaper to host, too.</p>
        <h1>What are the alternatives?</h1>
        <p>FURsvp was written when few other options were available. Today some are quite prevalent. We actually recommend using Google Forms over FURsvp if you want something fast, you don't want to share who's coming, and you don't need event details to stay visible after you stop accepting RSVPs.</p>
        <ul>
            <li><a href='https://www.google.com/forms/about/'>Google Forms</a> - Simple to use. Attendees don't need an account to RSVP.</li>
            <li><a href='https://www.facebook.com/events/'>Facebook Events</a> - Better visibility. Details can stay up indefinitely. Attendees can see who else is coming.</li>
            <li><a href='https://www.meetup.com/'>Meetup</a> - Good for communities. Details can stay up indefinitely. Attendees can see who else is coming.</li>
            <li><Link to='/'>FURsvp</Link> - Good for small communities. Simple to use (we hope!). Attendees don't need an account to RSVP. Details can stay up indefinitely. Attendees can see who else is coming. Free and open-source - extra customizable!</li>
        </ul>
        <h1>Is this tool just for the furry community?</h1>
        <p>There's no rule against hosting meetups on FURsvp outside of the furry community or private events, but that is how it has been predominantly used. Feel free to use FURsvp for your church group or political campaign rally, but you may be happier forking the source code and hosting the tool independently!</p>
    </div>
);

export default connect()(About);