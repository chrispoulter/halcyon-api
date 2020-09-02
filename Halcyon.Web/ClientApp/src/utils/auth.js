const SYSTEM_ADMINISTRATOR = 'System Administrator';

const USER_ADMINISTRATOR = 'User Administrator';

export const AVAILABLE_ROLES = [SYSTEM_ADMINISTRATOR, USER_ADMINISTRATOR];

export const IS_USER_ADMINISTRATOR = [SYSTEM_ADMINISTRATOR, USER_ADMINISTRATOR];

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
