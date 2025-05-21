import React, { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useGetAllUsersQuery, useDeleteUserMutation } from '../redux/services/apiSlice'; 
import { useNavigate } from 'react-router-dom'; 
import styled from 'styled-components';

const PageWrapper = styled.div`
  height: 100vh;
  width: 100vw;
  padding: 20px;
  box-sizing: border-box;
  background-color: #f2f6f9;
  display: flex;
  flex-direction: column;
`;

const Title = styled.h1`
  text-align: center;
  color: #333;
  margin-bottom: 20px;
`;

const UserGrid = styled.div`
  flex: 1;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 20px;
  overflow-y: auto;
  padding: 10px;
`;

const UserCard = styled.div`
  background-color: white;
  padding: 20px;
  border-radius: 10px;
  border-left: 6px solid rgb(10, 86, 60);
  box-shadow: 0 3px 10px rgba(0, 0, 0, 0.07);
  display: flex;
  flex-direction: column;
  justify-content: space-between; 
  cursor: pointer; 
  transition: border 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    border-left: 6px solid orange; 
    box-shadow: 0 4px 14px rgba(0, 0, 0, 0.1);
  }

  span {
    font-size: 14px;
    color: #555;
    margin-bottom: 6px;
  }

  strong {
    color: #222;
  }
`;

const DeleteButton = styled.button`
  margin-top: 10px;
  padding: 6px 10px;
  font-size: 12px;
  background-color: crimson;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  align-self: flex-start;

  &:hover {
    background-color: darkred;
  }
`;

const UpdateButton = styled.button` 
  margin-top: 10px;
  padding: 6px 10px;
  font-size: 12px;
  background-color: orange;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  align-self: flex-start;

  &:hover {
    background-color: darkred;
  }
`;

const AssignRoleButton = styled.button` 
  margin-top: 10px;
  padding: 6px 10px;
  font-size: 12px;
  background-color: #00A693;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  align-self: flex-start;

  &:hover {
    background-color: darkred;
  }
`;

const RestPasswordButton = styled.button`
  margin-top: 10px;
  padding: 6px 10px;
  font-size: 12px;
  background-color: #004953;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  align-self: flex-start;

  &:hover {
    background-color: darkred;
  }
`;

const ButtonWrapper = styled.div` 
  border-radius: 10px;  
  display: flex;
  flex-wrap: wrap;
  flex-direction: row; 
  cursor: pointer; 
  gap: 5px;
`;

const ModalBackdrop = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0,0,0,0.3);
  display: flex;
  justify-content: center;
  align-items: center;
`;

const Modal = styled.div`
  background: white;
  padding: 30px;
  border-radius: 10px;
  min-width: 300px;
`;

const RoleSelect = styled.select`
  width: 100%;
  padding: 10px;
  margin-top: 10px;
`;

const ConfirmButton = styled.button`
  margin-top: 20px;
  padding: 10px;
  background-color: green;
  color: white;
  border: none;
  width: 100%;
  border-radius: 4px;
  cursor: pointer;

  &:hover {
    background-color: darkgreen;
  }
`;

const AllUsers = () => {
    const navigate = useNavigate();
    const [isModalOpen, setIsModalOpen] = useState(false); 
    const [isModalResetPassOpen, setIsModalResetPassOpen] = useState(false); 
    const currentUser = useSelector((state) => state.auth.user); 
    const [selectedRole, setSelectedRole] = useState('');
    const [selectedUser, setSelectedUser] = useState(null);
    const [newPassword, setNewPassword] = useState('');
    const [passwordError, setPasswordError] = useState('');
    const [roles, setRoles] = useState([]);  

    const { data: users = [], error, isLoading, refetch } = useGetAllUsersQuery(undefined, { refetchOnMountOrArgChange: true });
    const [deleteUser] = useDeleteUserMutation();  
 
    useEffect(() => {
      const fetchRoles = async () => {
        try {
          const token = localStorage.getItem('token'); 
            const response = await fetch('http://localhost:5003/api/HR/admin/all-roles',{
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` 
                },
          });
          const data = await response.json(); 
          setRoles(data.data);
        } catch (err) {
          console.error('Failed to fetch roles', err);
        }
      };

      if (isModalOpen) {
        fetchRoles();
      }
    }, [isModalOpen]);

    if (isLoading) return <p>Loading...</p>;
    if (error) return <p>Error loading users</p>;

    const handleDelete = async (id) => {
        const confirmed = window.confirm('Are you sure you want to delete this user?');
        if (confirmed) {
          try {
            await deleteUser(id).unwrap();
            refetch();  
          } catch (err) {
            alert('Failed to delete user.');
          }
        }
    };

    const handleAssignRole = async () => {
        if (!selectedUser || !selectedRole) return; 
        try {  
            const token = localStorage.getItem('token'); 
            const response = await fetch('http://localhost:5003/api/HR/admin/assign-role', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` 
                },
            body: JSON.stringify({
                UserId: selectedUser.Id,
                RoleId: selectedRole
            })
            });
        
            if (!response.ok) throw new Error('Failed to assign role');
        
            setIsModalOpen(false);
            setSelectedUser(null);
            setSelectedRole('');
            refetch(); 
        } catch (error) {
            alert('Error assigning role');
        }
    }; 

    const handleResetPassword = async () => {  
        if (!newPassword || newPassword.length < 8) {
          setPasswordError('Password must be at least 8 characters');
          return;
        } 
        
        try { 
          const token = localStorage.getItem('token');  
          const response = await fetch(
              currentUser.id === selectedUser.Id ? 'http://localhost:5003/api/HR/me/reset-password' :
            'http://localhost:5003/api/HR/admin/reset-password', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
              Id: selectedUser.Id,
              NewPassword: newPassword
            })
          });
          
          if (response) { 
            setIsModalResetPassOpen(false); 
            setNewPassword('');
            setPasswordError('');
          } else {
            const errorData = await response.json();
            setPasswordError(errorData.message || 'Failed to reset password');
          }
        } catch (err) {
          console.error('Error:', err); 
          setPasswordError('An unexpected error occurred.' + err);
        }
    };
    
    return (
        <PageWrapper>
          <Title>All Users</Title>
          {users.data.length > 0 ? (
            <UserGrid>
              {users?.data.slice().reverse().map((user) => (
                <UserCard key={user.Id}>
                    <span><strong>ID:</strong> {user.Id}</span>
                    <span><strong>Email:</strong> {user.Email}</span>
                    <span><strong>Username:</strong> {user.UserName}</span>
                    <span><strong>Phone:</strong> {user.PhoneNumber}</span>
                    <span><strong>Roles:</strong> {user?.Roles?.join(' & ')}</span>
                    <span><strong>Created:</strong> {new Date(user.DateCreated).toLocaleString()}</span>

                    {isModalOpen && selectedUser?.Id === user.Id && (
                        <ModalBackdrop onClick={() => setIsModalOpen(false)}>
                            <Modal onClick={(e) => e.stopPropagation()}>
                                <h3>Assign Role to {selectedUser?.UserName}</h3>
                                <RoleSelect 
                                    value={selectedRole} 
                                    onChange={(e) => setSelectedRole(e.target.value)}
                                >
                                    <option value="">Select Role</option>
                                    {roles?.map((role) => (
                                        <option key={role.Id} value={role.Id}>
                                            {role.Name}
                                        </option>
                                    ))}
                                </RoleSelect>
                                <ConfirmButton onClick={handleAssignRole}>Confirm</ConfirmButton>
                            </Modal>
                        </ModalBackdrop>
                    )}

                    <ButtonWrapper>
                      <DeleteButton onClick={(e) => {
                        e.stopPropagation();  
                        handleDelete(user.Id);
                      }}>
                          Delete 
                      </DeleteButton>

                      <UpdateButton onClick={() => navigate(`/edit-user/${user.Id}`, { state: user })}>
                          Update 
                      </UpdateButton>

                      <AssignRoleButton onClick={(e) => {
                        e.stopPropagation();
                        setSelectedUser(user);
                        setIsModalOpen(true);
                      }}>
                          Assign Role
                      </AssignRoleButton>
                      {isModalResetPassOpen && selectedUser && (
                        <ModalBackdrop onClick={() => setIsModalResetPassOpen(false)}>
                          <Modal onClick={(e) => e.stopPropagation()}>
                            <h3>Reset Password for {selectedUser.UserName}</h3>
                            <input
                              type="password"
                              placeholder="Enter new password"
                              value={newPassword}
                              onChange={(e) => {
                                setNewPassword(e.target.value);
                                setPasswordError('');
                              }}
                              style={{
                                width: '100%',
                                padding: '10px',
                                marginTop: '10px',
                                borderRadius: '4px',
                                border: '1px solid #ccc'
                              }}
                            />
                            {passwordError && (
                              <p style={{ color: 'red', marginTop: '5px' }}>{passwordError}</p>
                            )}
                            <ConfirmButton onClick={handleResetPassword}>Confirm Reset</ConfirmButton>
                          </Modal>
                        </ModalBackdrop>
                      )}

                      <RestPasswordButton onClick={(e) => {
                        e.stopPropagation();  
                        setSelectedUser(user);
                        setIsModalResetPassOpen(true);
                      }}>
                          Reset Pass
                      </RestPasswordButton>
                    </ButtonWrapper>
                </UserCard>
              ))}
            </UserGrid>
          ) : (
            <p>No users found.</p>
          )}
        </PageWrapper>
    );
};

export default AllUsers;
