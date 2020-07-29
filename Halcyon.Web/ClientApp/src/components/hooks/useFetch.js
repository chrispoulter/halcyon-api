import { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../providers';

export const useFetch = ({ url, manual, ...options }) => {
    const { accessToken, removeToken } = useContext(AuthContext);
    const [loading, setLoading] = useState(false);
    const [data, setData] = useState();

    const headers = new Headers();
    if (accessToken) {
        headers.append('Authorization', accessToken);
    }

    useEffect(() => {
        if (manual) {
            return;
        }

        fetchData();
    }, []);

    const fetchData = async body => {
        setLoading(true);

        const response = await fetch(url, { ...options, body, headers });
        const responseBody = await response.json();

        console.log('responseBody', responseBody);

        setData(responseBody);
        setLoading(false);
    };

    return { loading, data, refetch: fetchData };
};
