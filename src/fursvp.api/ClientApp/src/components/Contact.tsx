import * as React from 'react';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';

const Contact = () => (
    <div>
        <h1>Contact Fursvp</h1>
        <p><strong>Under construction!</strong></p>
        <p>A nice, pretty contact form will appear here eventually. For now, kindly email all complaints to <a href="mailto:where.is.skippy@gmail.com">skippyfox</a>.</p>
    </div>
);

export default connect()(Contact);