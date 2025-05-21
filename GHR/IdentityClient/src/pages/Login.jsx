import React, { useState } from 'react';
import Input from '../components/Input';
import Button from '../components/Button';
import { useNavigate } from 'react-router-dom';

const fakeToken = (role) => {
  return btoa(JSON.stringify({ role }));
};

const Login = () => {
  const [role, setRole] = useState('');
  const navigate = useNavigate();

  const handleLogin = () => {
    const token = fakeToken(role);
    localStorage.setItem('token', token);
    if (role === 'EMPLOYEE') navigate('/employee');
    else if (role === 'MANAGER') navigate('/manager');
    else if (role === 'HR ADMIN') navigate('/hr');
  };

  return (
    <div>
      <h2>Login</h2>
      <Input placeholder="Role (EMPLOYEE, MANAGER, HR ADMIN)" value={role} onChange={(e) => setRole(e.target.value)} />
      <Button onClick={handleLogin}>Login</Button>
    </div>
  );
};

export default Login;
