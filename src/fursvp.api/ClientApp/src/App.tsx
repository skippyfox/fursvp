import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import About from './components/About';
import Counter from './components/Counter';
import FetchData from './components/FetchData';

import './custom.css'

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/about' component={About} />
        <Route path='/fetch-data/:startDateIndex?' component={FetchData} />
    </Layout>
);
