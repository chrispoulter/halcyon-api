import { useEffect, useState } from 'react';
import { useAuth, useToast } from '../contexts';
import { config } from '../utils/config';

const headers = new Headers();
headers.set('Content-Type', 'application/json');

export const useFetch = request => {
    const searchParams = new URLSearchParams(request.params || {});

    const url = `${config.API_URL}${request.url}?${searchParams}`;

    const { accessToken, removeToken } = useAuth();

    const toast = useToast();

    const [loading, setLoading] = useState(false);

    const [data, setData] = useState();

    if (accessToken) {
        headers.set('Authorization', `Bearer ${accessToken}`);
    }

    useEffect(() => {
        if (request.manual) {
            return;
        }

        refetch();
    }, [url]);

    const refetch = async body => {
        setLoading(true);

        let response;
        try {
            response = await fetch(url, {
                method: request.method,
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
                toast.error(message);
                break;

            case 401:
                removeToken();
                break;

            case 403:
                toast.warn(message);
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

    return { loading, data, refetch };
};
