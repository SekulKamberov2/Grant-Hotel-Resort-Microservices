import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { setCredentials } from '../redux/features/auth/authSlice';
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import api from '../api/axios';  

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
            const response = await api.post('/users/signin', formData);
            const user = response.data?.user; 
            console.log('user', response.data.user)
            if (!user) {
                throw new Error('Invalid response from server');
            }

            dispatch(setCredentials({ user }));

            localStorage.setItem('user', JSON.stringify(user));

            navigate('/profile');
        } catch (err) {
            const message = err.response?.data?.message || err.message || 'An error occurred during sign-in.';
            setError(message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container>
            <FormWrapper>
                <Title>Sign In</Title>
                <div style={{ display: 'flex', gap: '10px', fontSize: 14 }}>
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
