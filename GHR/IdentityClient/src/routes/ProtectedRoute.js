import { Navigate } from 'react-router-dom'; 
import { useSelector } from 'react-redux';

const ProtectedRoute = ({ children, roles }) => {
  const { token, user } = useSelector((state) => state.auth); 
 
  if (!token) 
    return <Navigate to="/signIn" replace />;

  if (roles && !roles.some(role => user?.roles.includes(role))) 
    return <Navigate to="/unauthorized" replace />;
  
  return children;
};

export default ProtectedRoute;
