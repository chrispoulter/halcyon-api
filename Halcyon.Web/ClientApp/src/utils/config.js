export const config = {
    STAGE: process.env.REACT_APP_STAGE || 'local',
    RELEASE: process.env.REACT_APP_RELEASE || 'local',
    API_URL: process.env.REACT_APP_API_URL || '/api',
    GA_MEASUREMENTID: process.env.REACT_APP_GA_MEASUREMENTID
};
