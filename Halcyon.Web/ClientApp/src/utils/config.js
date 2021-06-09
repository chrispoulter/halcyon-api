export const config = {
    STAGE: process.env.REACT_APP_STAGE || 'local',
    RELEASE: process.env.REACT_APP_RELEASE || 'local',
    API_URL: process.env.REACT_APP_API_URL || '/api',
    SENTRY_DSN: process.env.REACT_APP_SENTRY_DSN,
    GA_MEASUREMENTID: process.env.REACT_APP_GA_MEASUREMENTID
};
