import { Role } from '@/lib/session-types';

export enum UserSort {
    EMAIL_ADDRESS_ASC = 'EMAIL_ADDRESS_ASC',
    EMAIL_ADDRESS_DESC = 'EMAIL_ADDRESS_DESC',
    NAME_ASC = 'NAME_ASC',
    NAME_DESC = 'NAME_DESC',
}

export type SearchUsersRequest = {
    search?: string;
    sort: UserSort;
    page: number;
    size: number;
};

export type SearchUsersResponse = {
    items: {
        id: string;
        emailAddress: string;
        firstName: string;
        lastName: string;
        isLockedOut?: boolean;
        roles?: Role[];
    }[];
    hasNextPage: boolean;
    hasPreviousPage: boolean;
};

export type CreateUserRequest = {
    emailAddress: string;
    password: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    roles?: Role[];
};

export type CreateUserResponse = {
    id: string;
};

export type GetUserResponse = {
    id: string;
    emailAddress: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    isLockedOut?: boolean;
    roles?: Role[];
    version: number;
};

export type UpdateUserRequest = {
    emailAddress: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    roles?: Role[];
    version?: number;
};

export type UpdateUserResponse = {
    id: string;
};

export type LockUserRequest = { version?: number };

export type LockUserResponse = {
    id: string;
};

export type UnlockUserRequest = { version?: number };

export type UnlockUserResponse = {
    id: string;
};

export type DeleteUserRequest = { version?: number };

export type DeleteUserResponse = {
    id: string;
};
