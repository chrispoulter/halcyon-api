import React from 'react';
import ReactDOM from 'react-dom';
import { setLocale } from 'yup';
import * as serviceWorker from './serviceWorker';
import { App } from './App';
import { initialize } from './utils/logger';
import './styles/index.scss';

initialize();

setLocale({
    mixed: {
        required: '${label} is a required field.',
        oneOf: 'The "${label}" field does not match.'
    },
    string: {
        email: '${label} must be a valid email.',
        min: '${label} must be at least ${min} characters.',
        max: '${label} must be at most ${max} characters.'
    }
});

ReactDOM.render(<App />, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();
