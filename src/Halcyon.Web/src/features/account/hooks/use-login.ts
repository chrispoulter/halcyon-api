import { useMutation } from '@tanstack/react-query';
import { LoginRequest, LoginResponse } from '@/features/account/account-types';
import { apiClient } from '@/lib/api-client';

export const useLogin = () => {
    return useMutation({
        mutationFn: (request: LoginRequest) =>
            apiClient.post<LoginResponse>('/account/login', request),
    });
};
