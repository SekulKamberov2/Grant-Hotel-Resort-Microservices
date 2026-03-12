import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const api = createApi({
    reducerPath: 'api',
    baseQuery: fetchBaseQuery({
        baseUrl: 'http://localhost:5000/api',
        credentials: 'include',
    }),
    tagTypes: ['Users'],
    endpoints: (builder) => ({
        getAllUsers: builder.query({
            query: () => '/users/all-users',
            providesTags: (result) =>
                result?.data
                    ? result.data.map(({ Id }) => ({ type: 'Users', id: Id }))
                    : ['Users'],
        }),

        getUserProfile: builder.mutation({
            query: () => ({
                url: '/users/SignIn',
                method: 'POST',
            }),
        }),

        updateUser: builder.mutation({
            query: (userData) => ({
                url: `/users/update-user/${userData.id}`,
                method: 'PATCH',
                body: userData,
            }),
            invalidatesTags: (result, error, { id }) => [{ type: 'Users', id }],
        }),

        deleteUser: builder.mutation({
            query: (id) => ({
                url: `/users/delete-user/${id}`,
                method: 'DELETE',
            }),
            invalidatesTags: (result, error, id) => [{ type: 'Users', id }],
        }),
    }),
});

export const {
    useGetAllUsersQuery,
    useGetUserProfileMutation,
    useUpdateUserMutation,
    useDeleteUserMutation,
} = api;
