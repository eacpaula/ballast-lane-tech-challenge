import { createBrowserRouter } from 'react-router-dom'
import AppShell from './AppShell'
import AuthenticatedRoute from './AuthenticatedRoute'
import AdministratorRoute from './AdministratorRoute'
import PublicPostListPage from '../features/posts/PublicPostListPage'
import PublicPostDetailPage from '../features/posts/PublicPostDetailPage'
import RegisterPage from '../features/auth/RegisterPage'
import LoginPage from '../features/auth/LoginPage'
import MyPostsPage from '../features/posts/MyPostsPage'
import CreatePostPage from '../features/posts/CreatePostPage'
import EditPostPage from '../features/posts/EditPostPage'
import AdminCategoriesPage from '../features/categories/AdminCategoriesPage'
import NotFoundPage from './NotFoundPage'

const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <PublicPostListPage /> },
      { path: 'posts/:postId', element: <PublicPostDetailPage /> },
      { path: 'login', element: <LoginPage /> },
      { path: 'register', element: <RegisterPage /> },
      {
        element: <AuthenticatedRoute />,
        children: [
          { path: 'my-posts', element: <MyPostsPage /> },
          { path: 'my-posts/new', element: <CreatePostPage /> },
          { path: 'my-posts/:postId/edit', element: <EditPostPage /> },
        ],
      },
      {
        element: <AdministratorRoute />,
        children: [{ path: 'admin/categories', element: <AdminCategoriesPage /> }],
      },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
])

export default router
