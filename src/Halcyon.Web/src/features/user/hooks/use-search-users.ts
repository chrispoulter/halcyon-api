import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import {
    SearchUsersRequest,
    SearchUsersResponse,
} from '@/features/user/user-types';
import { apiClient } from '@/lib/api-client';

export const useSearchUsers = (request: SearchUsersRequest) => {
    const { accessToken } = useAuth();

    return useQuery({
        queryKey: ['users', request],
        queryFn: () =>
            apiClient.get<SearchUsersResponse>('/user', request, {
                Authorization: `Bearer ${accessToken}`,
            }),
        placeholderData: keepPreviousData,
    });
};
