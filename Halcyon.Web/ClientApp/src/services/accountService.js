import { useFetch } from '../hooks/useFetch';

export const useRegister = () =>
    useFetch({
        method: 'POST',
        url: '/account/register',
        manual: true
    });

export const useForgotPassword = () =>
    useFetch({
        method: 'PUT',
        url: '/account/forgotpassword',
        manual: true
    });

export const useResetPassword = () =>
    useFetch({
        method: 'PUT',
        url: '/account/resetpassword',
        manual: true
    });
