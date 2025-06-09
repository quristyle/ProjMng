import { createRouter, createWebHistory } from 'vue-router';
//import routes from '~pages'; // vite-plugin-pages가 생성한 라우트
import generatedPageRoutes from '~pages'; // vite-plugin-pages가 생성한 라우트 (src/pages 기반)
// Layout Components
import MainLayout from '../layouts/MainLayout.vue';
import AuthLayout from '../layouts/AuthLayout.vue';

// View Components for MainLayout
import HomeView from '../views/HomeView.vue';
// import DashboardView from '../views/DashboardView.vue'; // 예시

// View Components for AuthLayout
import LoginView from '../views/auth/LoginView.vue';
import RegisterView from '../views/auth/RegisterView.vue';

const otherPageRoutes = generatedPageRoutes.filter(route => route.path !== '/');
const routes = [
  {
    path: '/',
    component: MainLayout,
    children: [
      {
        path: '', // '/' 경로 접근 시 기본으로 보여줄 페이지
        name: 'Home',
        component: HomeView,
      },
      // {
      //   path: 'dashboard',
      //   name: 'Dashboard',
      //   component: DashboardView,
      // },
      // MainLayout을 사용하는 다른 라우트들...
      ...otherPageRoutes, // src/pages 폴더의 나머지 페이지들 (예: /dashboard)
    ],
  },
  {
    path: '/login',
    component: AuthLayout, // /login 경로에 AuthLayout 적용
    children: [
      {
        path: '', // /login 경로 접근 시 AuthLayout 내부에 LoginView를 표시
        name: 'Login',
        component: LoginView,
      },
    ],
  },
  {
    path: '/register',
    component: AuthLayout, // /register 경로에 AuthLayout 적용
    children: [
      {
        path: '', // /register 경로 접근 시 AuthLayout 내부에 RegisterView를 표시
        name: 'Register',
        component: RegisterView,
      },
    ],
  },
  // 필요한 경우 404 페이지 라우트 추가
  // { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFoundView }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL || '/'),
  routes,
});

export default router;