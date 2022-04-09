import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { config } from '../../utils/config';

export const halcyonApi = createApi({
    reducerPath: 'halcyonApi',
    baseQuery: fetchBaseQuery({
        baseUrl: config.API_URL,
        prepareHeaders: (headers, { getState }) => {
            const { accessToken } = getState().auth;

            if (accessToken) {
                headers.set('authorization', `Bearer ${accessToken}`);
            }

            return headers;
        }
    }),
    endpoints: builder => ({
        register: builder.mutation({
            query: body => ({
                url: '/account/register',
                method: 'POST',
                body
            }),
            invalidatesTags: [{ type: 'Users', id: 'PARTIAL-LIST' }]
        }),
        forgotPassword: builder.mutation({
            query: body => ({
                url: '/account/forgotpassword',
                method: 'PUT',
                body
            })
        }),
        resetPassword: builder.mutation({
            query: body => ({
                url: '/account/resetpassword',
                method: 'PUT',
                body
            })
        }),
        createToken: builder.mutation({
            query: body => ({
                url: '/token',
                method: 'POST',
                body
            })
        }),
        getProfile: builder.query({
            query: () => '/manage',
            providesTags: result => [{ type: 'Users', id: result?.data?.id }]
        }),
        updateProfile: builder.mutation({
            query: body => ({
                url: '/manage',
                method: 'PUT',
                body
            }),
            invalidatesTags: result => [{ type: 'Users', id: result?.data?.id }]
        }),
        changePassword: builder.mutation({
            query: body => ({
                url: '/manage/changepassword',
                method: 'PUT',
                body
            }),
            invalidatesTags: result => [{ type: 'Users', id: result?.data?.id }]
        }),
        deleteAccount: builder.mutation({
            query: () => ({
                url: '/manage',
                method: 'DELETE'
            }),
            invalidatesTags: result => [
                { type: 'Users', id: 'PARTIAL-LIST' },
                { type: 'Users', id: result?.data?.id }
            ]
        }),
        searchUsers: builder.query({
            query: params => ({
                url: `/user`,
                params
            }),
            providesTags: result => [
                { type: 'Users', id: 'PARTIAL-LIST' },
                ...(result?.data?.items?.map(user => ({
                    type: 'Users',
                    id: user.id
                })) || [])
            ]
        }),
        createUser: builder.mutation({
            query: body => ({
                url: '/user',
                method: 'POST',
                body
            }),
            invalidatesTags: [{ type: 'Users', id: 'PARTIAL-LIST' }]
        }),
        getUser: builder.query({
            query: id => `/user/${id}`,
            providesTags: result => [{ type: 'Users', id: result?.data?.id }]
        }),
        updateUser: builder.mutation({
            query: ({ id, body }) => ({
                url: `/user/${id}`,
                method: 'PUT',
                body
            }),
            invalidatesTags: result => [{ type: 'Users', id: result?.data?.id }]
        }),
        lockUser: builder.mutation({
            query: id => ({
                url: `/user/${id}/lock`,
                method: 'PUT'
            }),
            invalidatesTags: result => [{ type: 'Users', id: result?.data?.id }]
        }),
        unlockUser: builder.mutation({
            query: id => ({
                url: `/user/${id}/unlock`,
                method: 'PUT'
            }),
            invalidatesTags: result => [{ type: 'Users', id: result?.data?.id }]
        }),
        deleteUser: builder.mutation({
            query: id => ({
                url: `/user/${id}`,
                method: 'DELETE'
            }),
            invalidatesTags: result => [
                { type: 'Users', id: 'PARTIAL-LIST' },
                { type: 'Users', id: result?.data?.id }
            ]
        })
    })
});

export const {
    useRegisterMutation,
    useForgotPasswordMutation,
    useResetPasswordMutation,
    useCreateTokenMutation,
    useGetProfileQuery,
    useUpdateProfileMutation,
    useChangePasswordMutation,
    useDeleteAccountMutation,
    useSearchUsersQuery,
    useCreateUserMutation,
    useGetUserQuery,
    useUpdateUserMutation,
    useLockUserMutation,
    useUnlockUserMutation,
    useDeleteUserMutation
} = halcyonApi;
