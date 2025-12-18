<!-- components/LoginComponent.vue -->
<template>
  <div class="login-form">
    <h2>Вход в систему</h2>
    <form @submit.prevent="handleLogin">
      <div class="form-group">
        <label>Логин:</label>
        <input v-model="credentials.login" type="text" required />
      </div>

      <div class="form-group">
        <label>Пароль:</label>
        <input v-model="credentials.password" type="password" required />
      </div>

      <button type="submit" :disabled="loading">
        {{ loading ? 'Загрузка...' : 'Войти' }}
      </button>

      <p v-if="error" class="error">{{ error }}</p>
    </form>

    <p>
      Нет аккаунта? <router-link to="/register">Зарегистрироваться</router-link>
    </p>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';
  import { authService } from '@/services/auth.service';

  const router = useRouter();
  const authStore = useAuthStore();

  const credentials = ref({
    login: '',
    password: ''
  });
  const loading = ref(false);
  const error = ref('');

  const handleLogin = async () => {
    try {
      loading.value = true;
      error.value = '';

      const response = await authService.login(credentials.value);

      // Сохраняем данные аутентификации
      authStore.setAuthData(response);

      console.log('Login successful, user data:', {
        patientId: authStore.patientId,
        employeeId: authStore.employeeId,
        role: authStore.userRole
      });

      // Редирект в зависимости от роли
      switch (response.role) {
        case 'user':
          router.push('/patient');
          break;
        case 'employee':
          router.push('/doctor');
          break;
        case 'admin':
          router.push('/admin');
          break;
        default:
          router.push('/');
      }
    } catch (err) {
      error.value = err.response?.data?.message || 'Ошибка входа';
    } finally {
      loading.value = false;
    }
  };
</script>
