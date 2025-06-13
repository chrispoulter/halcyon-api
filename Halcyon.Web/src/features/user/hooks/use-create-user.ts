import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import {
    CreateUserRequest,
    CreateUserResponse,
} from '@/features/user/user-types';
import { apiClient } from '@/lib/api-client';

export const useCreateUser = () => {
    const { accessToken } = useAuth();

    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (request: CreateUserRequest) =>
            apiClient.post<CreateUserResponse>('/user', request, {
                Authorization: `Bearer ${accessToken}`,
            }),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }),
    });
};
