import { useContext, useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { AuthContext } from '../providers';
import { config } from '../../utils/config';

const headers = new Headers();
headers.set('Content-Type', 'application/json');

export const useFetch = request => {
    const { accessToken, removeToken } = useContext(AuthContext);

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
    }, [request.url, request.params]);

    const refetch = async body => {
        setLoading(true);

        const params = new URLSearchParams(request.params || {});
        const url = `${config.API_URL}${request.url}?${params}`;

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

        const message = json?.message;
        const data = json?.data;

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
                break;

            case 200:
                toast.success(message);
                break;

            default:
                toast.error(
                    'An unknown error has occurred whilst communicating with the server.'
                );
                break;
        }

        setData(data);
        setLoading(false);

        return { ok, data };
    };

    return { loading, data, refetch };
};
