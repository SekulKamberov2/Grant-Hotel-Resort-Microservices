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
  user: parsedUser,
};
const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (state, action) => {
      const { user } = action.payload;
      state.user = user;

      localStorage.setItem('user', JSON.stringify(user));
    },
    logout: (state) => {
      state.user = null;
      localStorage.removeItem('user');
    },
  },
});

export const { setCredentials, logout } = authSlice.actions;

export default authSlice.reducer;
