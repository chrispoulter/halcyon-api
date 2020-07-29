export const AVAILABLE_ROLES = ['System Administrator', 'User Administrator'];

export const USER_ADMINISTRATOR = [
    'System Administrator',
    'User Administrator'
];

export const isAuthorized = (user, requiredRoles) => {
    if (!user) {
        return false;
    }

    if (!requiredRoles) {
        return true;
    }

    if (!user.role) {
        return false;
    }

    const userRoles = user.role || [];
    if (!requiredRoles.some(value => userRoles.includes(value))) {
        return false;
    }

    return true;
};
