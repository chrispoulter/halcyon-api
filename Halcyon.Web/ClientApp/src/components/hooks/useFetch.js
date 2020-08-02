import { useContext, useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { AuthContext } from '../providers';
import config from '../../utils/config';

export const useFetch = ({ url, manual, ...options }) => {
    const { accessToken, removeToken } = useContext(AuthContext);

    const [loading, setLoading] = useState(false);

    const [data, setData] = useState();

    const headers = new Headers();
    headers.append('Content-Type', 'application/json');

    if (accessToken) {
        headers.append('Authorization', `Bearer ${accessToken}`);
    }

    useEffect(() => {
        if (manual) {
            return;
        }

        refetch();
    }, []);

    const refetch = async body => {
        setLoading(true);

        const response = await fetch(`${config.API_URL}${url}`, {
            ...options,
            headers,
            body: body && JSON.stringify(body)
        });

        console.log('response', response);

        let json;
        try {
            json = await response.json()
        } catch {

        }

        var messages = json?.messages || [];

        switch (response.status) {
            case 400:
                messages.forEach(toast.error);
                break;

            case 401:
                removeToken();
                break;

            case 403:
                messages.forEach(toast.warn);
                break;

            case 200:
                messages.forEach(toast.success);
                break;

            default:
                toast.error(
                    'An unknown error has occurred whilst communicating with the server.'
                );
                break;
        }

        setData(json?.data);
        setLoading(false);

        if (!response.ok) {
            throw response;
        }

        return json;
    };

    return { loading, data, refetch };
};
