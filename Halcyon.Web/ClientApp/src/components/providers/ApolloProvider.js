import React, { useContext } from 'react';
import { ApolloProvider as BaseApolloProvider } from '@apollo/react-hooks';
import ApolloClient from 'apollo-boost';
import { toast } from 'react-toastify';
import { AuthContext } from './AuthProvider';
import config from '../../utils/config';

export const ApolloProvider = ({ children }) => {
    const { accessToken, removeToken } = useContext(AuthContext);

    const client = new ApolloClient({
        uri: config.GRAPHQL_URL,
        resolvers: {},
        request: operation =>
            operation.setContext({
                headers: {
                    authorization: accessToken ? `Bearer ${accessToken}` : ''
                }
            }),
        onError: ({ graphQLErrors, networkError }) => {
            if (graphQLErrors) {
                for (const graphQLError of graphQLErrors || []) {
                    switch (graphQLError.extensions.code) {
                        case 'BAD_USER_INPUT':
                            toast.error(graphQLError.message);
                            break;

                        case 'UNAUTHENTICATED':
                            removeToken();
                            break;

                        case 'FORBIDDEN':
                            toast.warn(graphQLError.message);
                            break;

                        default:
                            toast.error(
                                graphQLError.message ||
                                    'An unknown error has occurred whilst communicating with the server.'
                            );

                            break;
                    }
                }
            } else if (networkError) {
                toast.error(
                    'An unknown error has occurred whilst communicating with the server.'
                );
            }
        }
    });

    return <BaseApolloProvider client={client}>{children}</BaseApolloProvider>;
};
