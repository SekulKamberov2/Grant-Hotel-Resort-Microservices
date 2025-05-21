import { createSlice } from '@reduxjs/toolkit';
import { api } from '../services/apiSlice';   

const initialState = {
  users: [],
  loading: false,
  error: null,
};

const usersSlice = createSlice({
  name: 'users',
  initialState,
  reducers: {
    setUsers: (state, action) => {
      state.users = action.payload;
    },
    setLoading: (state, action) => {
      state.loading = action.payload;
    },
    setError: (state, action) => {
      state.error = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addMatcher(api.endpoints.getAllUsers.matchPending, (state) => {
        state.loading = true;
      })
      .addMatcher(api.endpoints.getAllUsers.matchFulfilled, (state, action) => {
        state.loading = false;
        state.users = action.payload;
      })
      .addMatcher(api.endpoints.getAllUsers.matchRejected, (state, action) => {
        state.loading = false;
        state.error = action.payload.message;
      });
  },
});

export const { setUsers, setLoading, setError } = usersSlice.actions;

export default usersSlice.reducer;
