import { useFetch } from '../hooks/useFetch';

export const useGetProfile = () =>
    useFetch({
        method: 'GET',
        url: '/manage'
    });

export const useUpdateProfile = () =>
    useFetch({
        method: 'PUT',
        url: '/manage'
    });

export const useChangePassword = () =>
    useFetch({
        method: 'PUT',
        url: '/manage/changepassword'
    });

export const useDeleteAccount = () =>
    useFetch({
        method: 'DELETE',
        url: '/manage'
    });
