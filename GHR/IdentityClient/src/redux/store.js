import { configureStore } from '@reduxjs/toolkit';
import authReducer from './features/auth/authSlice'; 
import { api as apiSlice } from '../redux/services/apiSlice';
import usersReducer from '../redux/features/usersSlice';  

export const store = configureStore({
  reducer: {
    auth: authReducer,
    users: usersReducer,  
    [apiSlice.reducerPath]: apiSlice.reducer,  
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(apiSlice.middleware),
});
