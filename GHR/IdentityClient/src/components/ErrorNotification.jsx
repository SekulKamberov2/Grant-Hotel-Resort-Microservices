import styled from 'styled-components';

const ErrorContainer = styled.div`
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
  border-radius: 4px;
  padding: 10px;
  margin-bottom: 1rem;
  font-size: 14px;
  font-weight: bold;
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

const ErrorMessage = styled.span`
  flex: 1;
`;

const CloseButton = styled.button`
  background: transparent;
  border: none;
  color: #721c24;
  font-weight: bold;
  cursor: pointer;

  &:hover {
    color: #0056b3;
  }
`;

const ErrorNotification = ({ message, onClose }) => {
  return (
    <ErrorContainer>
      <ErrorMessage>{message}</ErrorMessage>
      <CloseButton onClick={onClose}>×</CloseButton>
    </ErrorContainer>
  );
};

export default ErrorNotification;
