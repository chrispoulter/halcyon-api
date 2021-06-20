import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import { v4 as uuidv4 } from 'uuid';
import { AuthContext } from '../providers';
import { config } from '../../utils/config';

const headers = new Headers();
headers.set('Content-Type', 'application/json');

export const useFetch = request => {
    const { t } = useTranslation();

    const { accessToken, removeToken } = useContext(AuthContext);

    const [loading, setLoading] = useState(false);

    const [data, setData] = useState();

    if (accessToken) {
        headers.set('Authorization', `Bearer ${accessToken}`);
        headers.set('x-transaction-id', uuidv4());
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

        const data = json?.data;
        const code = json?.code;

        switch (status) {
            case 400:
                toast.error(t(`api.codes.${code}`, body));
                break;

            case 401:
                removeToken();
                break;

            case 403:
                toast.warn(t(`api.codes.${code}`, body));
                break;

            case 404:
            case 200:
                break;

            default:
                toast.error(
                    t(
                        [
                            `api.codes.${code}`,
                            'api.codes.INTERNAL_SERVER_ERROR'
                        ],
                        body
                    )
                );
                break;
        }

        setData(data);
        setLoading(false);

        return { ok, code, data };
    };

    return { loading, data, refetch };
};
