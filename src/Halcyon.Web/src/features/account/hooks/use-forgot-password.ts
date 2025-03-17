import { useMutation } from '@tanstack/react-query';
import { ForgotPasswordRequest } from '@/features/account/account-types';
import { apiClient } from '@/lib/api-client';

export const useForgotPassword = () =>
    useMutation({
        mutationFn: (request: ForgotPasswordRequest) =>
            apiClient.put('/account/forgot-password', request),
    });
