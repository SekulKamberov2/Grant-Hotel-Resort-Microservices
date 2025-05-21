import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import styled from 'styled-components';
 
const FormContainer = styled.div`
  max-width: 500px;
  margin: 2rem auto;
  padding: 2rem;
  border: 1px solid #ccc;
  border-radius: 8px;
`;

const Title = styled.h2`
  text-align: center;
`;

const Form = styled.form`
  display: flex;
  flex-direction: column;
`;

const Label = styled.label`
  margin-top: 1rem;
`;

const Input = styled.input`
  padding: 0.5rem;
  font-size: 1rem;
  margin-top: 0.5rem;
`;

const TextArea = styled.textarea`
  padding: 0.5rem;
  font-size: 1rem;
  margin-top: 0.5rem;
  resize: vertical;
`;

const Error = styled.span`
  color: red;
  font-size: 0.9rem;
`;

const Button = styled.button`
  margin-top: 1.5rem;
  padding: 0.75rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;

  &:hover {
    background-color: #0056b3;
  }
`;

const SuccessMessage = styled.div`
  color: green;
  margin-top: 1rem;
  text-align: center;
`;

const CreateRole = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ Name: '', Description: '' });
  const [errors, setErrors] = useState({});
  const [success, setSuccess] = useState('');

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setErrors({ ...errors, [e.target.name]: '' });
    setSuccess('');
  };

  const validate = () => {
    const newErrors = {};
    if (!form.Name.trim()) newErrors.Name = 'Name is required';
    if (!form.Description.trim()) newErrors.Description = 'Description is required';
    return newErrors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      const token = localStorage.getItem('token'); 
        const response = await fetch('http://localhost:5003/api/HR/create-role', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
            },
        body: JSON.stringify(form),
      });

      if (!response.ok)  throw new Error('Failed to create role'); 
      navigate('/users');
      setSuccess('Role created successfully!');
      setForm({ Name: '', Description: '' });
    } catch (error) {
      setErrors({ submit: error.message });
    }
  };

  return (
    <FormContainer>
      <Title>Create Role</Title>
      <Form onSubmit={handleSubmit}>
        <Label>Name</Label>
        <Input name="Name" value={form.Name} onChange={handleChange} />
        {errors.Name && <Error>{errors.Name}</Error>}

        <Label>Description</Label>
        <TextArea name="Description" rows="4" value={form.Description} onChange={handleChange} />
        {errors.Description && <Error>{errors.Description}</Error>}

        {errors.submit && <Error>{errors.submit}</Error>}
        <Button type="submit">Create</Button>
        {success && <SuccessMessage>{success}</SuccessMessage>}
      </Form>
    </FormContainer>
  );
};

export default CreateRole;
