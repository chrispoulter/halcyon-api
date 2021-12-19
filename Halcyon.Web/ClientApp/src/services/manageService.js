import { useFetch } from '../hooks/useFetch';

export const useGetProfile = () =>
    useFetch({
        method: 'GET',
        url: '/manage'
    });

export const useUpdateProfile = () =>
    useFetch({
        method: 'PUT',
        url: '/manage',
        manual: true
    });

export const useChangePassword = () =>
    useFetch({
        method: 'PUT',
        url: '/manage/changepassword',
        manual: true
    });

export const useDeleteAccount = () =>
    useFetch({
        method: 'DELETE',
        url: '/manage',
        manual: true
    });
