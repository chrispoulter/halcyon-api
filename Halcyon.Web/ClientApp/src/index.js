import React from 'react';
import ReactDOM from 'react-dom';
import './i18n';
import * as serviceWorker from './serviceWorker';
import { App } from './App';
import { initialize } from './utils/logger';
import './styles/index.scss';

initialize();

ReactDOM.render(<App />, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();
