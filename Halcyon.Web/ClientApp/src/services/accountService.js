import { useFetch } from '../hooks/useFetch';

export const useRegister = () =>
    useFetch({
        method: 'POST',
        url: '/account/register'
    });

export const useForgotPassword = () =>
    useFetch({
        method: 'PUT',
        url: '/account/forgot-password'
    });

export const useResetPassword = () =>
    useFetch({
        method: 'PUT',
        url: '/account/reset-password'
    });
