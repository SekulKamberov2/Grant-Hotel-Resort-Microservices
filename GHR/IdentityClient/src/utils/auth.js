 
export const ROLES = [ 'EMPLOYEE','MANAGER','HR ADMIN']; 
const token = localStorage.getItem('token');  
  export const isAuthenticated = () => !!token;
  