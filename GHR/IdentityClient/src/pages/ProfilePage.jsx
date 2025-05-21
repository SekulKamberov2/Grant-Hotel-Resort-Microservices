import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';  
import styled from 'styled-components'; 

import { useSelector } from 'react-redux';

const UserCard = styled.div`
  background-color: white;
  padding: 30px;
  border-radius: 10px;
  border-left: 6px solid rgb(10, 86, 60);
  box-shadow: 0 3px 10px rgba(0, 0, 0, 0.07);
  display: flex;
  flex-direction: column;
  justify-content: space-between; 

  span {
    font-size: 18px;
    color: #555;
    margin-bottom: 4px;
  }

  strong {
    color: #222;
  }
`;

const PageContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  height: 100vh;
  background-color: #f4f4f9;
  padding: 40px;
  box-sizing: border-box;
`;

 

const ProfileHeader = styled.div`
  font-size: 37px;
  font-weight: 800;
  color: #333; 
  width: 85%;
  text-align: center;  
`;

const UserNameHeader = styled.div`
  font-size: 37px;
  font-weight: 700;
  color: #333;
  margin-top: 11px;
  margin-bottom: 31px;
  text-align: center; 
`;  
  
const Button = styled.button`
  padding: 10px 20px;
  font-size: 16px;
  border-radius: 5px;
  border: none;
  cursor: pointer;
  transition: background-color 0.3s ease;
  width: 45%; 

  &:hover {
    opacity: 0.9;
  }
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

const RoundedResetButton = styled.button`
  padding: 5px 8px;
  background-color: transparent;
  color: black;
  border: 2px solid black;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 650;
  margin-left: 9px;
  margin-bottom: 15px;
  width: ${({ width }) => width || 'auto'};  
  transition: background-color 0.3s ease, color 0.3s ease;

  &:hover {
    background-color: #d4edda;  
    border-color: black; 
  }
`;

const ProfilePage = () => {  
  const user = useSelector((state) => state.auth.user);
  const EMPLOYEE = user?.roles.some(u => u === "EMPLOYEE");
  const [isModalResetPassOpen, setIsModalResetPassOpen] = useState(false); 
  const navigate = useNavigate();   
  const [newPassword, setNewPassword] = useState('');
  const [passwordError, setPasswordError] = useState('');

  const handleResetPassword = async () => {  
        if (!newPassword || newPassword.length < 8) {
          setPasswordError('Password must be at least 8 characters');
          return;
        } 
        
        try { 
          console.log('ID', user.id);
          const token = localStorage.getItem('token');  
            const response = await fetch('http://localhost:5003/api/HR/me/reset-password', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
              Id: user.id,
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
    }
  return (
    <PageContainer>
      {isModalResetPassOpen && user && (
        <ModalBackdrop onClick={() => setIsModalResetPassOpen(false)}>
          <Modal onClick={(e) => e.stopPropagation()}>
            <h3>Reset Password for {user.UserName}</h3>
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
      {user?.id > 0 ?  
        < > 
          <ProfileHeader>{ user.roles ? user?.roles.join(" & ") : "Not Employee"}</ProfileHeader>
          <UserNameHeader>{ user.userName}</UserNameHeader> 
          {EMPLOYEE && 
            <>  
              <RoundedResetButton width="111px" onClick={(e) => {
                e.stopPropagation();   
                setIsModalResetPassOpen(true);
              }}>Reset Pass</RoundedResetButton>    
            </>
          } 
          <UserCard>  
            <span><strong>ID: </strong> { user.id}</span>
            <span><strong>Email: </strong> { user.email}</span>
            <span><strong>Username: </strong> { user.userName}</span>
            <span><strong>Phone: </strong> { user.phoneNumber}</span>
            <span><strong>Created: </strong> { new Date(user.dateCreated).toLocaleString()}</span>
            <span><strong>Your Roles: </strong>{ user.roles ? user?.roles.join(" & ") : "Not Employee"}</span>
          </UserCard> 
        </ > 
      : navigate('/signin') }
    </PageContainer>
  );
};

export default ProfilePage;
