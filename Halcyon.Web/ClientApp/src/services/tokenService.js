import { useFetch } from '../hooks/useFetch';

export const useCreateToken = () =>
    useFetch({
        method: 'POST',
        url: '/token',
        manual: true
    });
