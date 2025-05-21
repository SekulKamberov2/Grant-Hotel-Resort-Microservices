import { createSlice } from '@reduxjs/toolkit';
 
const rawUser = localStorage.getItem('user');
let parsedUser = null;

try {
  if (rawUser && rawUser !== 'undefined') {
    parsedUser = JSON.parse(rawUser);
  }
} catch (err) {
  console.warn('Error parsing user from localStorage:', err);
}

const initialState = {
  token: localStorage.getItem('token') || null,
  user: parsedUser,
};
const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (state, action) => {
      const { token, user } = action.payload;
      state.token = token;
      state.user = user;

      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
    },
    logout: (state) => {
      state.token = null;
      state.user = null;
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    },
  },
});

export const { setCredentials, logout } = authSlice.actions;

export default authSlice.reducer;
