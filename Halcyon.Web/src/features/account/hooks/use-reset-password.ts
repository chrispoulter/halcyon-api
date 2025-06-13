import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
    ResetPasswordRequest,
    ResetPasswordResponse,
} from '@/features/account/account-types';
import { apiClient } from '@/lib/api-client';

export const useResetPassword = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (request: ResetPasswordRequest) =>
            apiClient.put<ResetPasswordResponse>(
                '/account/reset-password',
                request
            ),
        onSuccess: (data) => {
            queryClient.invalidateQueries({ queryKey: ['profile'] });
            queryClient.invalidateQueries({ queryKey: ['users'] });
            queryClient.invalidateQueries({ queryKey: ['user', data.id] });
        },
    });
};
