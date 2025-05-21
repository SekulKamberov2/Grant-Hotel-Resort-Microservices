 
import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5003/api/HR',  
  timeout: 10000,
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token && !config.headers['Authorization']) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export default api;

