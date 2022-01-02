import { useEffect, useState } from 'react';
import { useAuth, useToast } from '../contexts';
import { config } from '../utils/config';

const headers = new Headers();
headers.set('Content-Type', 'application/json');

export const useFetch = ({ url, method, params }) => {
    const searchParams = new URLSearchParams(params || {});

    const apiUrl = `${config.API_URL}${url}?${searchParams}`;

    const { accessToken, removeToken } = useAuth();

    const toast = useToast();

    const [loading, setLoading] = useState(false);

    const [data, setData] = useState();

    if (accessToken) {
        headers.set('Authorization', `Bearer ${accessToken}`);
    }

    useEffect(() => {
        if (method !== 'GET') {
            return;
        }

        request();
    }, [apiUrl]);

    const request = async body => {
        setLoading(true);

        let response;
        try {
            response = await fetch(apiUrl, {
                method,
                headers,
                body: body && JSON.stringify(body)
            });
        } catch {}

        const ok = response?.ok;
        const status = response?.status;

        let json;
        try {
            json = await response.json();
        } catch {}

        const data = json?.data;
        const message = json?.message;

        switch (status) {
            case 400:
                toast.error(
                    message || 'Sorry, the current request is invalid.'
                );
                break;

            case 401:
                removeToken();
                break;

            case 403:
                toast.warn(
                    message || 'Sorry, you do not have access to this resource.'
                );
                break;

            case 404:
            case 200:
                break;

            default:
                toast.error(
                    message ||
                        'An unknown error has occurred whilst communicating with the server.'
                );
                break;
        }

        setData(data);
        setLoading(false);

        return { ok, message, data };
    };

    return { loading, data, request };
};
