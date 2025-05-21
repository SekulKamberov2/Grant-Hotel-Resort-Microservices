import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { setCredentials } from '../redux/features/auth/authSlice';   
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';
 
const Container = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f4f4f4;
`;

const FormWrapper = styled.div`
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 300px;
  text-align: center;

  @media (max-width: 600px) {
    max-width: 90%;
  }
`;

const Title = styled.h2`
  margin-bottom: 20px;
  font-size: 24px;
  color: #333;
`;

const Label = styled.label`
  display: block;
  margin-bottom: 8px;
  text-align: left;
  font-size: 14px;
  color: #333;
`;

const Input = styled.input`
  width: 100%;
  padding: 10px;
  margin-bottom: 15px;
  border-radius: 5px;
  border: 1px solid #ddd;
  font-size: 16px;
  box-sizing: border-box;

  &:focus {
    border-color: #4e9f3d;
    outline: none;
  }
`;

const Button = styled.button`
  width: 100%;
  padding: 12px;
  background-color: #4e9f3d;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;

  &:disabled {
    background-color: #ccc;
    cursor: not-allowed;
  }

  &:hover {
    background-color: #3b8d2f;
  }
`;

const ErrorMessage = styled.p`
  color: red;
  margin-bottom: 15px;
  font-size: 14px;
`;

const SignIn = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({ email: '', password: '' });
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
        const response = await fetch('http://localhost:5003/api/HR/signin', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        const err = await response.json();
        throw new Error(err.message || 'Login failed');
      }

      const data = await response.json();  
     
      if (!data.data.token || !isValidJwt(data.data.token)) throw new Error('Invalid token received');  
      const decodedToken = decodeJwt(data.data.token);
         
      const user = {
        id: data.data.user.id,
        userName: data.data.user.userName,
        email: data.data.user.email,
        phoneNumber: data.data.user.phoneNumber,
        roles: data.data.user.roles,  
        dateCreated: new Date(decodedToken.exp * 1000).toLocaleString(),
      };
      dispatch(setCredentials({ token: data.data.token, user }));
 
      localStorage.setItem('token', data.data.token);
      localStorage.setItem('user', JSON.stringify(user));
 
      navigate('/profile');
    } catch (err) {
        setError(err.message || 'An error occurred during sign-in.');
    } finally {
        setLoading(false);
    }
  };
 
  const isValidJwt = (token) => {
    const parts = token.split('.');
    return parts.length === 3;  
  };

  const decodeJwt = (token) => {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
          })
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch (error) {
        console.error('Error decoding JWT:', error);
      return null;
    }
  };
 
  return (
    <Container>
      <FormWrapper>
        <Title>Sign In</Title> 
        <div style={{ display: 'flex', gap: '10px', fontSize: 14  }}>
            <div>sekul7@gmail.com</div>
            <div>Qwerty1!@%</div>
          </div>
          <div style={{ display: 'flex', gap: '10px', fontSize: 14, marginBottom: 11 }}>
            <div>sekul8@gmail.com</div>
            <div>Qwerty1!@%</div>
          </div> 
          {error && <ErrorMessage>{error}</ErrorMessage>} 
          {loading && <p>Signing in...</p>} 
          <form onSubmit={handleSubmit}>
            <div> 
              <Label htmlFor="email">Email:</Label>
              <Input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>

            <div>
              <Label htmlFor="password">Password:</Label>
              <Input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
              />
            </div>

            <Button type="submit" disabled={loading}>
              Sign In
            </Button>
          </form> 
      </FormWrapper> 
    </Container>
  );
};

export default SignIn;
