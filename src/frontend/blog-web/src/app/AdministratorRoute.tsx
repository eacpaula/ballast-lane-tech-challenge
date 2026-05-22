import { Navigate, Outlet } from 'react-router-dom'
import ErrorMessage from '../components/ErrorMessage'
import Card from '../components/Card'
import { useAuth } from '../features/auth/useAuth'

export default function AdministratorRoute() {
  const { isAuthenticated, isAdministrator } = useAuth()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  if (!isAdministrator) {
    return (
      <Card>
        <ErrorMessage
          title="Administrator access required"
          detail="Your account does not have permission to manage categories."
        />
      </Card>
    )
  }

  return <Outlet />
}
