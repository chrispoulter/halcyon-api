export const getItem = key =>
    sessionStorage.getItem(key) || localStorage.getItem(key);

export const setItem = (key, value, persist) => {
    if (persist) {
        sessionStorage.removeItem(key);
        localStorage.setItem(key, value);
    } else {
        localStorage.removeItem(key);
        sessionStorage.setItem(key, value);
    }
};

export const removeItem = key => {
    sessionStorage.removeItem(key);
    localStorage.removeItem(key);
};
