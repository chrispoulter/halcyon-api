export type GetProfileResponse = {
    id: string;
    emailAddress: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    version: number;
};

export type UpdateProfileRequest = {
    emailAddress: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    version?: number;
};

export type UpdateProfileResponse = { id: string };

export type ChangePasswordRequest = {
    currentPassword: string;
    newPassword: string;
    version?: number;
};

export type ChangePasswordResponse = { id: string };

export type DeleteAccountRequst = { version?: number };

export type DeleteAccountResponse = { id: string };
