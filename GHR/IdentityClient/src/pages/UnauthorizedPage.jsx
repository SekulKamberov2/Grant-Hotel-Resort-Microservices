import React from 'react';
import styled from 'styled-components';
import { useNavigate } from 'react-router-dom';

const PageWrapper = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
  background-color: #1a1a1a;
  color: white;
  text-align: center;
  padding: 20px;
`;

const Title = styled.h1`
  font-size: 3rem;
  margin-bottom: 20px;
`;

const Message = styled.p`
  font-size: 1.2rem;
  margin-bottom: 30px;
  color: #ccc;
`;

const ReturnButton = styled.button`
  padding: 12px 24px;
  font-size: 1rem;
  border: 2px solid white;
  border-radius: 25px;
  background: transparent;
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;

  &:hover {
    background: white;
    color: black;
  }
`;

const UnauthorizedPage = () => {
  const navigate = useNavigate();

  const handleGoBack = () => {
    navigate('/profile');  
  };

  return (
    <PageWrapper>
      <Title>401 Unauthorized</Title>
      <Message>You do not have permission to access this page.</Message>
      <ReturnButton onClick={handleGoBack}>Go to Home</ReturnButton>
    </PageWrapper>
  );
};

export default UnauthorizedPage;
