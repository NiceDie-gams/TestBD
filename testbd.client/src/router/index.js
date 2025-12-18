import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('@/views/HomeView.vue') // Нужно создать
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/components/LoginComponent.vue'),
    meta: { guestOnly: true }
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/components/RegistrationComponent.vue'),
    meta: { guestOnly: true }
  },
  {
    path: '/booking',
    name: 'Booking',
    component: () => import('@/views/BookingView.vue'),
    meta: {
      requiresAuth: true,
      roles: ['user'] // Только для пациентов
    }
  },
  // Панель пациента
  {
    path: '/patient',
    name: 'PatientDashboard',
    component: () => import('@/views/patient/DashboardView.vue'),
    meta: { requiresAuth: true, roles: ['user'] },
    children: [
      {
        path: 'profile',
        component: () => import('@/views/patient/ProfileView.vue'),
        name: 'patient-profile'
      },
      {
        path: 'history',
        component: () => import('@/views/patient/HistoryView.vue'),
        name: 'patient-history'
      }
    ]
  },
  {
    path: '/patient/appointments',
    name: 'PatientAppointments',
    component: () => import('@/views/patient/AppointmentView.vue'),
    meta: { requiresAuth: true, roles: ['user'] }
  },
  // Панель врача

  {
    path: '/doctor',
    name: 'DoctorDashboard',
    component: () => import('@/views/doctor/DashboardView.vue'),
    meta: { requiresAuth: true, roles: ['employee'] },
    children: [
      {
        path: 'schedule',
        component: () => import('@/views/doctor/DoctorScheduleView.vue'),
        name: 'DoctorSchedule'
      },
      {
        path: 'patients',
        component: () => import('@/views/doctor/PatientView.vue'),
        name: 'Patients'
      }
    ]
  },
  // Панель администратора
  {
    path: '/admin',
    name: 'AdminDashboard',
    component: () => import('@/views/AdminDashboard.vue'),
    meta: { requiresAuth: true, roles: ['admin'] }
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  // Если маршрут требует авторизации
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }

  // Если маршрут только для гостей
  if (to.meta.guestOnly && authStore.isAuthenticated) {
    // Редирект в зависимости от роли
    switch (authStore.userRole) {
      case 'user': next('/patient'); break;
      case 'employee': next('/doctor'); break;
      case 'admin': next('/admin'); break;
      default: next('/');
    }
    return
  }

  // Проверка ролей
  if (to.meta.roles && !to.meta.roles.includes(authStore.userRole)) {
    next('/')
    return
  }

  next()
})

export default router
