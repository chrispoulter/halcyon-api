export type LoginRequest = {
    emailAddress: string;
    password: string;
};

export type LoginResponse = {
    accessToken: string;
};

export type RegisterRequest = {
    emailAddress: string;
    password: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
};

export type RegisterResponse = {
    id: string;
};

export type ForgotPasswordRequest = { emailAddress: string };

export type ResetPasswordRequest = {
    token: string;
    emailAddress: string;
    newPassword: string;
};

export type ResetPasswordResponse = {
    id: string;
};
