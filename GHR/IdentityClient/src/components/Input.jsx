import styled from 'styled-components';

const StyledInput = styled.input`
  padding: 10px;
  margin: 10px 0;
  border: 1px solid #ccc;
  border-radius: 4px;
`;

const Input = (props) => <StyledInput {...props} />;

export default Input;
