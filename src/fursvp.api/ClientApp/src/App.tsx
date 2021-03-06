import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import About from './components/About';
import Contact from './components/Contact';
import EventDetail from './components/EventDetail';

import './custom.css'

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/contact' component={Contact} />
        <Route path='/about' component={About} />
        <Route exact path='/event/:eventId' component={EventDetail} />
        <Route exact path='/event/:eventId/edit' component={EventDetail} />
        <Route exact path='/event/:eventId/member/:memberId' component={EventDetail} />
    </Layout>
);
