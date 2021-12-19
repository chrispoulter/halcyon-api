import { useFetch } from '../hooks/useFetch';

export const useSearchUsers = params =>
    useFetch({
        method: 'GET',
        url: '/user',
        params
    });

export const useCreateUser = () =>
    useFetch({
        method: 'POST',
        url: '/user',
        manual: true
    });

export const useGetUser = id =>
    useFetch({
        method: 'GET',
        url: `/user/${id}`
    });

export const useUpdateUser = id =>
    useFetch({
        method: 'PUT',
        url: `/user/${id}`,
        manual: true
    });

export const useLockUser = id =>
    useFetch({
        method: 'PUT',
        url: `/user/${id}/lock`,
        manual: true
    });

export const useUnlockUser = id =>
    useFetch({
        method: 'PUT',
        url: `/user/${id}/unlock`,
        manual: true
    });

export const useDeleteUser = id =>
    useFetch({
        method: 'DELETE',
        url: `/user/${id}`,
        manual: true
    });
