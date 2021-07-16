export const config = {
    VERSION: process.env.REACT_APP_VERSION || '1.0.0',
    STAGE: process.env.REACT_APP_STAGE || 'local',
    GA_MEASUREMENTID: process.env.REACT_APP_GA_MEASUREMENTID,
    API_URL: process.env.REACT_APP_API_URL || '/api'
};
