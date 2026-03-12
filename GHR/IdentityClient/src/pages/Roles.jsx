import React, { useEffect, useState } from 'react';
import styled from 'styled-components';
import api from '../api/axios'; 

const Container = styled.div`
  max-width: 800px;
  margin: 2rem auto;
  padding: 2rem;
`;

const Title = styled.h2`
  text-align: center;
`;

const Form = styled.form`
  display: flex;
  flex-direction: column;
  margin-bottom: 2rem;
`;

const Input = styled.input`
  padding: 0.5rem;
  margin-top: 0.5rem;
  margin-bottom: 1rem;
`;

const TextArea = styled.textarea`
  padding: 0.5rem;
  margin-bottom: 1rem;
`;

const Button = styled.button`
  padding: 0.5rem;
  background-color: ${(props) => (props.delete ? '#dc3545' : '#007bff')};
  color: white;
  border: none;
  border-radius: 4px;
  margin-right: 0.5rem;
  margin-top: 0.5rem;
  cursor: pointer;

  &:hover {
    background-color: ${(props) => (props.delete ? '#b02a37' : '#0056b3')};
  }
`;

const Error = styled.div`
  color: red;
  margin-bottom: 1rem;
`;

const Success = styled.div`
  color: green;
  margin-bottom: 1rem;
`;

const RoleCardContainer = styled.div`
  display: grid;
  grid-template-columns: repeat(3, 1fr);  
  gap: 1rem;
  width: 100%;

  @media (max-width: 1024px) {
    grid-template-columns: repeat(2, 1fr); 
  }

  @media (max-width: 600px) {
    grid-template-columns: 1fr;  
  }
`;

const RoleCard = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  border: 1px solid #ccc;
  padding: 1rem;
  border-radius: 4px;
  background-color: #f9f9f9;
  height: 100%;
  box-sizing: border-box;
`;

const RoleTitle = styled.strong`
  font-size: 1.1rem;
  margin-bottom: 0.5rem;
`;

const RoleDescription = styled.p`
  flex-grow: 1;
`;

const ButtonContainer = styled.div`
  display: flex;
  gap: 0.5rem;
  margin-top: auto;
`;

const ToggleButton = styled(Button)`
  background-color: white;
  margin-bottom: 1rem; 
  border: 2px solid black;
  color: black;
  border-radius: 9px; 
  font-weight: 600;
  &:hover {
    background-color: #d4edda;  
    border-color: black;  
  }
`;

const Roles = () => {
    const [roles, setRoles] = useState([]);
    const [form, setForm] = useState({ Name: '', Description: '' });
    const [editingRoleId, setEditingRoleId] = useState(null);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [showForm, setShowForm] = useState(false);

    const fetchRoles = async () => {
        try {
            const response = await api.get('/users/admin/all-roles');
            setRoles(response.data.data); 
        } catch (err) {
            setError('Failed to load roles');
        }
    };

    useEffect(() => {
        fetchRoles();
    }, []);

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
        setError('');
        setMessage('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            if (editingRoleId) {
                await api.patch(`/users/update-role/${editingRoleId}`, form);
                setMessage('Role updated successfully');
            } else {
                await api.post('/users/create-role', form);
                setMessage('Role created successfully');
            }

            setForm({ Name: '', Description: '' });
            setEditingRoleId(null);
            setShowForm(false);
            fetchRoles();
        } catch (err) {
            const msg = err.response?.data?.message || 'Error submitting form';
            setError(msg);
        }
    };

    const handleEdit = (role) => {
        setForm({ Name: role.name, Description: role.description });
        setEditingRoleId(role.id);
        setShowForm(true);
    };

    const handleDelete = async (id) => {
        try {
            await api.delete(`/users/delete-role/${id}`);
            setMessage('Role deleted');
            fetchRoles();
        } catch (err) {
            setError('Delete failed');
        }
    };
   
    return (
        <Container>
            <Title>Roles Management</Title>

            <ToggleButton onClick={() => setShowForm(!showForm)}>
                {showForm ? 'Cancel' : 'New Role'}
            </ToggleButton>

            {showForm && (
                <Form onSubmit={handleSubmit}>
                    {error && <Error>{error}</Error>}
                    {message && <Success>{message}</Success>}

                    <label>Name</label>
                    <Input name="Name" value={form.Name} onChange={handleChange} />

                    <label>Description</label>
                    <TextArea name="Description" rows="4" value={form.Description} onChange={handleChange} />

                    <Button type="submit">{editingRoleId ? 'Update Role' : 'Create Role'}</Button>
                </Form>
            )}

            {roles.length === 0 && <div>No roles found.</div>}
            <RoleCardContainer>
                {roles.slice().reverse().map((role) => (
                    <RoleCard key={role.id}>
                        <RoleTitle>{role.name}</RoleTitle>
                        <RoleDescription>{role.description}</RoleDescription>

                        <ButtonContainer>
                            <Button onClick={() => handleEdit(role)}>Edit</Button>
                            <Button delete onClick={() => handleDelete(role.id)}>Delete</Button>
                        </ButtonContainer>
                    </RoleCard>
                ))}
            </RoleCardContainer>
        </Container>
    );
};

export default Roles;
