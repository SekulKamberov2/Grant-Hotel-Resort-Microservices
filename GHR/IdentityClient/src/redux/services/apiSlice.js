import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const api = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
      baseUrl: 'http://localhost:5003/api/HR',
    prepareHeaders: (headers) => {
      const token = localStorage.getItem('token');
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }
      return headers;
    },
  }),
  endpoints: (builder) => ({
    getAllUsers: builder.query({
      query: () => '/all-users', 
    }),
    getUserProfile: builder.mutation({
      query: () => ({
        url: '/SignIn',
        method: 'POST',  
      }),
    }),
    query: (id) => ({
      url: `/delete-user/${id}`,
      method: 'DELETE',
    }),
    updateUser: builder.mutation({
      query: (userData) => ({
        url: `/update-user/${userData.id}`,
        method: 'PATCH',
        body: userData,
      }),
    }),
    deleteUser: builder.mutation({ 
      query: (id) => ({
        url: `/delete-user/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Users'],
    }),
  }),
});

export const { useGetAllUsersQuery, useGetUserProfileMutation,  
  useUpdateUserMutation, useDeleteUserMutation } = api;   
