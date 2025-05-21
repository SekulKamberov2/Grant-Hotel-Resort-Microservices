import React, { useEffect, useState } from 'react';
import styled from 'styled-components';
 
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
        const token = localStorage.getItem('token'); 
          const res = await fetch('http://localhost:5003/api/HR/admin/all-roles', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
          }, 
        });
        const data = await res.json();
        console.log('roles', data);
        setRoles(data.data);
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
      const url = editingRoleId
          ? `http://localhost:5003/api/HR/update-role/${editingRoleId}`
          : 'http://localhost:5003/api/HR/create-role';
  
      const method = editingRoleId ? 'PATCH' : 'POST';
  
      try {
        const token = localStorage.getItem('token'); 
        const res = await fetch(url, {
          method,
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
          },
          body: JSON.stringify(form),
        });
  
        if (!res.ok) throw new Error('Request failed');
  
        setForm({ Name: '', Description: '' });
        setEditingRoleId(null);
        setMessage(editingRoleId ? 'Role updated successfully' : 'Role created successfully');
        setShowForm(false); 
        fetchRoles();
      } catch (err) {
        setError('Error submitting form');
      }
    };
  
    const handleEdit = (role) => {
      setForm({ Name: role.Name, Description: role.Description });
      setEditingRoleId(role.Id);
      setShowForm(true);  
    };
  
    const handleDelete = async (id) => {
      try {
        const token = localStorage.getItem('token');
          const res = await fetch(`http://localhost:5003/api/HR/delete-role/${id}`, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
          },
        });
        if (!res.ok) throw new Error('Delete failed');
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
          {roles.reverse().map((role) => (
            <RoleCard key={role.Id}>
              <RoleTitle>{role.Name}</RoleTitle>
              <RoleDescription>{role.Description}</RoleDescription>
  
              <ButtonContainer>
                <Button onClick={() => handleEdit(role)}>Edit</Button>
                <Button delete onClick={() => handleDelete(role.Id)}>Delete</Button>
              </ButtonContainer>
            </RoleCard>
          ))}
        </RoleCardContainer>
      </Container>
    );
  };

export default Roles;
