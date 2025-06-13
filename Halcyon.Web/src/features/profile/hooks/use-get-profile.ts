import { useQuery } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import { GetProfileResponse } from '@/features/profile/profile-types';
import { apiClient } from '@/lib/api-client';

export const useGetProfile = () => {
    const { accessToken } = useAuth();

    return useQuery({
        queryKey: ['profile'],
        queryFn: () =>
            apiClient.get<GetProfileResponse>('/profile', undefined, {
                Authorization: `Bearer ${accessToken}`,
            }),
    });
};
