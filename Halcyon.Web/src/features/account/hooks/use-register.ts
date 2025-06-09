import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
    RegisterRequest,
    RegisterResponse,
} from '@/features/account/account-types';
import { apiClient } from '@/lib/api-client';

export const useRegister = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (request: RegisterRequest) =>
            apiClient.post<RegisterResponse>('/account/register', request),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }),
    });
};
