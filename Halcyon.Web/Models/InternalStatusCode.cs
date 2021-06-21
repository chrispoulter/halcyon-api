namespace Halcyon.Web.Models
{
    public enum InternalStatusCode
    {
        DUPLICATE_USER,
        USER_REGISTERED,
        FORGOT_PASSWORD,
        INVALID_TOKEN,
        PASSWORD_RESET,
        USER_NOT_FOUND,
        PROFILE_UPDATED,
        INCORRECT_PASSWORD,
        PASSWORD_CHANGED,
        ACCOUNT_DELETED,
        USER_CREATED,
        CREDENTIALS_INVALID,
        USER_LOCKED_OUT,
        USER_UPDATED,
        LOCK_CURRENT_USER,
        USER_LOCKED,
        USER_UNLOCKED,
        DELETE_CURRENT_USER,
        USER_DELETED,
        INTERNAL_SERVER_ERROR
    }
}