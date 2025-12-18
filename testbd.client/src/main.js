import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import axios from 'axios'

// Импорт глобальных стилей если есть
import './assets/main.css'

// Создаем приложение Vue
const app = createApp(App)

// Создаем и подключаем Pinia store
const pinia = createPinia()
app.use(pinia)

// main.js - ДОБАВИТЬ ПОСЛЕ app.use(pinia)
import { useAuthStore } from '@/stores/auth'


// Подключаем Vue Router
app.use(router)

const authStore = useAuthStore()
console.log('Pinia store инициализирован:', authStore)

app.config.globalProperties.$axios = axios
app.config.globalProperties.$authStore = authStore

// Настройка axios
const API_BASE_URL = '';

axios.defaults.baseURL = API_BASE_URL
axios.defaults.headers.common['Content-Type'] = 'application/json'
axios.defaults.headers.common['Accept'] = 'application/json'

// Интерцептор для добавления токена к запросам
axios.interceptors.request.use(
  (config) => {
    // Получаем токен из localStorage
    const token = localStorage.getItem('auth_token')

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Интерцептор для обработки ошибок
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    const { response } = error

    // Обработка ошибки 401 (Unauthorized)
    if (response && response.status === 401) {
      console.warn('Unauthorized, возможно истёк токен');
    }

    // Обработка ошибки 403 (Forbidden)
    if (response && response.status === 403) {
      router.push('/unauthorized')
    }

    // Обработка ошибки 404 (Not Found)
    if (response && response.status === 404) {
      console.error('Ресурс не найден:', error.config.url)
    }

    // Пробрасываем ошибку дальше
    return Promise.reject(error)
  }
)

// Делаем axios доступным глобально (опционально)
app.config.globalProperties.$axios = axios

// Глобальные компоненты (если есть)
// import BaseButton from './components/BaseButton.vue'
// app.component('BaseButton', BaseButton)

// Глобальные директивы (если есть)
// app.directive('focus', {
//   mounted(el) {
//     el.focus()
//   }
// })

// Монтируем приложение

app.mount('#app')

// Для отладки
console.log('Vue приложение запущено')
console.log('API Base URL:', API_BASE_URL)
console.log('Текущая среда:', import.meta.env.MODE)
