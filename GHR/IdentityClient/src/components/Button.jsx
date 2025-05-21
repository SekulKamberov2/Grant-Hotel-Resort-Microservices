import styled from 'styled-components';

const StyledButton = styled.button`
  padding: 10px 20px;
  background-color: #007bfc;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
`;

const Button = ({ children, ...props }) => <StyledButton {...props}>{children}</StyledButton>;

export default Button;
