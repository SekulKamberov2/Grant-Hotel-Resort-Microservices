import api from '../api/axios';

export const signUp = async (userData, token) => {
  try { 
    const response = await api.post('/people/signUp', userData, {
      headers: {
        Authorization: `Bearer ${token}`,  
      },
    });
    return response.data;  
  } catch (error) {
    handleError(error);
  }
};

export const signIn = async (credentials) => {
  try {
    const response = await api.post('/SignIn', credentials);
    const { token } = response.data;
    if (token) {
      localStorage.setItem('token', token);  
    }
    return response.data;
  } catch (error) {
    handleError(error);
  }
};

export const deleteUser = async (userId) => {
  try {
    const response = await api.delete(`/delete-user/${userId}`);
    return response.data;
  } catch (error) {
    handleError(error);
  }
};

export const updateUser = async (userId, userData) => {
  try {
    const response = await api.patch(`/update-user/${userId}`, userData);
    return response.data;
  } catch (error) {
    handleError(error);
  }
};

export const createRole = async (roleData) => {
  try {
    const response = await api.post('/create-role', roleData);
    return response.data;
  } catch (error) {
    handleError(error);
  }
};

export const updateRole = async (roleId, roleData) => {
  try {
    const response = await api.put(`/update-role/${roleId}`, roleData);
    return response.data;
  } catch (error) {
    handleError(error);
  }
};

export const resetPassword = async (userId, newPassword) => {
  try {
    const response = await api.post('/me/reset-password', { Id: userId, NewPassword: newPassword });
    return response.data;
  } catch (error) {
    handleError(error);
  }
};

export const assignRole = async (userId, roleId) => {
  try {
    const response = await api.put('/admin/assign-role', { UserId: userId, RoleId: roleId });
    return response.data;
  } catch (error) {
    handleError(error);
  }
};

const handleError = (error) => {
  if (error.response) {
    console.error('API Error:', error.response.data);
  } else {
    console.error('Network Error:', error.message);
  }
};
