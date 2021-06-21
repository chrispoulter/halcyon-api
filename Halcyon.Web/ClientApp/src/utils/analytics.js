import { config } from '../utils/config';

let initialized = false;

export const initialize = () => {
    if (!config.GA_MEASUREMENTID) {
        return;
    }

    const script = document.createElement('script');
    script.src = `https://www.googletagmanager.com/gtag/js?id=${config.GA_MEASUREMENTID}`;
    script.id = 'googleAnalytics';
    script.crossorigin = 'anonymous';
    document.body.appendChild(script);

    window.dataLayer = window.dataLayer || [];
    window.gtag = function () {
        window.dataLayer.push(arguments);
    };
    window.gtag('js', new Date());
    window.gtag('config', config.GA_MEASUREMENTID);

    initialized = true;
};

export const setUser = user => {
    if (!initialized) {
        return;
    }

    window.gtag('set', {
        user_id: user?.sub,
        role: user?.role
    });
};

export const setContext = context => {
    if (!initialized) {
        return;
    }

    window.gtag('set', context);
};

export const trackEvent = (event, params) => {
    if (!initialized) {
        return;
    }

    window.gtag('event', event, params);
};
