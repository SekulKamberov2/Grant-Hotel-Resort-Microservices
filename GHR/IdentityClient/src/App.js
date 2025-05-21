import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
 
import ProtectedRoute from './routes/ProtectedRoute';
import { ROLES } from './utils/roles'; 

import ProfilePage from './pages/ProfilePage'; 
import UpdateUserPage from './pages/UpdateUserPage';  
import EditUser from './pages/EditUser';  
import AllUsers from './pages/AllUsers';  
import UserLayout from './components/UserLayout'; 
import UnauthorizedPage from './pages/UnauthorizedPage'; 
import SignIn from './pages/SignIn';
import SignUp from './pages/SignUp'; 
import Roles from './pages/Roles';  
import CreateRole from './pages/CreateRole'; 

function App() {
  return (
    <Router>
      <Routes> 
        <Route path="/" element={<SignIn />} />
        <Route path="/SignIn" element={<SignIn />} />

        <Route element={<UserLayout />}> 
          <Route
              path="/signup" element={
                <ProtectedRoute roles={[ROLES.HR_ADMIN]}>
                  <SignUp />
                </ProtectedRoute>
              }
            />
            <Route path="/profile" element={
                <ProtectedRoute roles={[ROLES.EMPLOYEE, ROLES.MANAGER, ROLES.HR_ADMIN]}>
                  <ProfilePage />
                </ProtectedRoute>
              }
            />
            <Route path="/profile" element={
                <ProtectedRoute roles={[ROLES.MANAGER, ROLES.HR_ADMIN]}>
                  <UpdateUserPage />
                </ProtectedRoute>
              }
            /> 
            <Route path="/users" element={
                <ProtectedRoute roles={[ROLES.MANAGER, ROLES.HR_ADMIN]}>
                  <AllUsers />
                </ProtectedRoute>
              }
            />
            <Route path="/edit-user/:id" element={
                  <ProtectedRoute roles={[ROLES.MANAGER, ROLES.HR_ADMIN]}>
                    <EditUser />
                  </ProtectedRoute>
                }
            />
            <Route path="/create-role" element={
                  <ProtectedRoute roles={[ROLES.HR_ADMIN]}>
                    <CreateRole />
                  </ProtectedRoute>
                }
            />
            <Route path="/roles" element={
                  <ProtectedRoute roles={[ROLES.HR_ADMIN]}>
                    <Roles />
                  </ProtectedRoute>
                }
              />
             <Route path="/me/reset-password" element={
                  <ProtectedRoute roles={[ROLES.EMPLOYEE]}>
                    <Roles />
                  </ProtectedRoute>
                }
            />
            <Route path="/admin/reset-password" element={
                  <ProtectedRoute roles={[ROLES.HR_ADMIN]}>
                    <Roles />
                  </ProtectedRoute>
                }
            />
            <Route path="/delete-user/:id"  />
        </Route>

        {/* 404 fallback */}
        <Route path="*" element={<UnauthorizedPage/>} />
      </Routes>
    </Router>
  );
}

export default App;
