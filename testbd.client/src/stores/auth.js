// stores/auth.js - ДОБАВЛЯЕМ МЕТОД
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

export const useAuthStore = defineStore('auth', () => {
  const router = useRouter();

  // Состояние
  const token = ref(localStorage.getItem('auth_token') || '');
  const user = ref(JSON.parse(localStorage.getItem('user_data') || 'null'));
  const isAuthenticated = computed(() => !!token.value);

  // Геттеры
  const userRole = computed(() => user.value?.role || '');
  const userName = computed(() => user.value?.login || '');
  const userId = computed(() => user.value?.userId || '');
  const patientId = computed(() => user.value?.patientId || null);
  const employeeId = computed(() => user.value?.employeeId || null);
  const isAdmin = computed(() => userRole.value === 'admin');
  const isEmployee = computed(() => userRole.value === 'employee');
  const isUser = computed(() => userRole.value === 'user');

  // Действия
  const setAuthData = (authResponse) => {
    const { token: authToken, login, role, patientId, employeeId } = authResponse;
    token.value = authToken;
    user.value = { login, role, patientId, employeeId };
    localStorage.setItem('auth_token', authToken);
    localStorage.setItem('user_data', JSON.stringify(user.value));
  };

  const updateUserData = (userData) => {
    user.value = { ...user.value, ...userData };
    localStorage.setItem('user_data', JSON.stringify(user.value));
  };

  const clearAuthData = () => {
    token.value = '';
    user.value = null;
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_data');
    router.push('/login');
  };

  const logout = () => {
    clearAuthData();
  };

  // 🔴 ДОБАВЛЯЕМ ЭТОТ МЕТОД
  const restoreSession = () => {
    const savedToken = localStorage.getItem('auth_token');
    const savedUser = localStorage.getItem('user_data');

    if (savedToken && savedUser ) {
      try {
        token.value = savedToken;
        user.value = JSON.parse(savedUser);
        return true;
      } catch (error) {
        console.error('Ошибка восстановления сессии:', error);
        clearAuthData();
        return false;
      }
    }
    return false;
  };

  // Автоматически восстанавливаем сессию при инициализации
  const init = () => {
    restoreSession();
  };

  // Вызываем инициализацию
  init();

  return {
    // State
    token,
    user,

    // Getters
    isAuthenticated,
    userRole,
    userName,
    userId,
    patientId,
    employeeId,
    isAdmin,
    isEmployee,
    isUser,

    // Actions
    setAuthData,
    updateUserData,
    clearAuthData,
    logout,
    restoreSession
  };
});

//// stores/auth.js
//import { defineStore } from 'pinia';
//import { ref, computed } from 'vue';
//import { useRouter } from 'vue-router';
//import { jwtDecode } from 'jwt-decode'; // Установите: npm install jwt-decode

//export const useAuthStore = defineStore('auth', () => {
//  const router = useRouter();

//  // Состояние
//  const token = ref(localStorage.getItem('auth_token') || '');
//  const user = ref(JSON.parse(localStorage.getItem('user_data') || 'null'));
//  const isAuthenticated = computed(() => !!token.value);

//  // Геттеры
//  const userRole = computed(() => user.value?.role || '');
//  const userName = computed(() => user.value?.login || '');
//  const userId = computed(() => user.value?.userId || '');

//  // Получаем patientId и employeeId из токена
//  const patientId = computed(() => {
//    if (!token.value) return null;
//    try {
//      const decoded = jwtDecode(token.value);
//      return decoded.patientId || user.value?.patientId || null;
//    } catch {
//      return user.value?.patientId || null;
//    }
//  });

//  const employeeId = computed(() => {
//    if (!token.value) return null;
//    try {
//      const decoded = jwtDecode(token.value);
//      return decoded.employeeId || user.value?.employeeId || null;
//    } catch {
//      return user.value?.employeeId || null;
//    }
//  });

//  const isAdmin = computed(() => userRole.value === 'admin');
//  const isEmployee = computed(() => userRole.value === 'employee');
//  const isUser = computed(() => userRole.value === 'user');

//  // Действия
//  const setAuthData = (authResponse) => {
//    const { token: authToken, login, role, userId, patientId, employeeId } = authResponse;

//    token.value = authToken;

//    // Сохраняем все данные из ответа
//    user.value = {
//      login,
//      role,
//      userId,
//      patientId,
//      employeeId
//    };

//    localStorage.setItem('auth_token', authToken);
//    localStorage.setItem('user_data', JSON.stringify(user.value));

//    console.log('Auth data set:', {
//      login,
//      role,
//      userId,
//      patientId,
//      employeeId
//    });
//  };

//  const updateUserData = (userData) => {
//    user.value = { ...user.value, ...userData };
//    localStorage.setItem('user_data', JSON.stringify(user.value));
//  };

//  const clearAuthData = () => {
//    token.value = '';
//    user.value = null;
//    localStorage.removeItem('auth_token');
//    localStorage.removeItem('user_data');
//    router.push('/login');
//  };

//  const logout = () => {
//    clearAuthData();
//  };

//  // Восстанавливаем данные из токена при инициализации
//  const init = () => {
//    const savedToken = localStorage.getItem('auth_token');
//    const savedUser = localStorage.getItem('user_data');

//    if (savedToken) {
//      token.value = savedToken;

//      try {
//        // Декодируем токен для получения актуальных данных
//        const decoded = jwtDecode(savedToken);

//        // Обновляем user данными из токена
//        user.value = {
//          login: decoded.login || decoded.name,
//          role: decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
//          userId: decoded.userId || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
//          patientId: decoded.patientId,
//          employeeId: decoded.employeeId
//        };

//        // Если есть сохраненные данные, мержим их
//        if (savedUser) {
//          const parsedSaved = JSON.parse(savedUser);
//          user.value = { ...user.value, ...parsedSaved };
//        }

//        localStorage.setItem('user_data', JSON.stringify(user.value));

//        console.log('Session restored from token:', user.value);
//      } catch (error) {
//        console.error('Error decoding token:', error);
//        if (savedUser) {
//          try {
//            user.value = JSON.parse(savedUser);
//          } catch {
//            clearAuthData();
//          }
//        }
//      }
//    }
//  };

//  // Вызываем инициализацию
//  init();

//  return {
//    // State
//    token,
//    user,

//    // Getters
//    isAuthenticated,
//    userRole,
//    userName,
//    userId,
//    patientId,
//    employeeId,
//    isAdmin,
//    isEmployee,
//    isUser,

//    // Actions
//    setAuthData,
//    updateUserData,
//    clearAuthData,
//    logout
//  };
//});
