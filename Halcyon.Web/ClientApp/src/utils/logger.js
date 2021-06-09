import * as sentry from './sentry';
import * as analytics from './analytics';

const handlers = [sentry, analytics];

export const initialize = () => {
    for (const logger of handlers) {
        if (!logger.initialize) {
            continue;
        }

        logger.initialize();
    }
};

export const setUser = user => {
    for (const logger of handlers) {
        if (!logger.setUser) {
            continue;
        }

        logger.setUser(user);
    }
};

export const setContext = context => {
    for (const logger of handlers) {
        if (!logger.setContext) {
            continue;
        }

        logger.setContext(context);
    }
};

export const trackEvent = (event, params) => {
    for (const logger of handlers) {
        if (!logger.trackEvent) {
            continue;
        }

        logger.trackEvent(event, params);
    }
};

export const captureGraphQLError = error => {
    for (const logger of handlers) {
        if (!logger.captureGraphQLError) {
            continue;
        }

        logger.captureGraphQLError(error);
    }
};
