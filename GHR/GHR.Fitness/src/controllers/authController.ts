import { Request, Response } from 'express';
import axios from 'axios';
import User from '../models/User';
import ApiResponse from '../utils/ApiResponse';

export const signIn = async (req: Request, res: Response) => {
  const { email, password } = req.body;
//http://identity-service:8081/api/users/signin //in containers
//http://localhost:5001/api/users/signin   
  try { 
    const authResponse = await axios.post('http://identity-service:8081/api/users/signin', { email, password });  
    const userData = authResponse.data?.data;
     
    if (!userData.token || !userData.user.id) {
      return res.status(400).json(new ApiResponse({ success: false, message: 'Invalid user data from auth service' }));
    }
 
    const existingUser = await User.findOne({ sqlUserId: userData.user.id }); 
    if (!existingUser) { 
      await User.create({
        sqlUserId: userData.user.id,
        userName: userData.user.userName || '',
        email: userData.user.email || '',
        phoneNumber: userData.user.phoneNumber || '',
        dateCreated: new Date(),
        roles: userData.user.roles || [] 
      });
    }
 
    res.status(200).json(new ApiResponse({
      message: 'Login Successful',
      data: {
        sqlUserId: userData.user.id,
        email: userData.user.email,
        userName: userData.user.userName,
        token: userData.token
      }
    }));

  } catch (error: any) {
    const message = error.response?.data?.message || 'Auth service error';
    res.status(401).json(new ApiResponse({ success: false, message, error: error.message }));
  }
};
