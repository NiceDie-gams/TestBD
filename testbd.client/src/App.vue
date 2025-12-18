<!-- App.vue -->
<template>
  <div id="app">
    <!-- Только после инициализации Pinia -->
    <router-view v-if="isPiniaReady" />
    <div v-else class="loading-container">
      <div class="loading-spinner"></div>
      <p>Загрузка приложения...</p>
    </div>
  </div>
</template>

<script>
  import { ref, onMounted } from 'vue'
  import { useAuthStore } from '@/stores/auth'

  export default {
    name: 'App',
    setup() {
      const isPiniaReady = ref(false)
      const authStore = useAuthStore() // Это инициализирует store

      onMounted(() => {
        // Не нужно ждать - Pinia уже инициализирована
        isPiniaReady.value = true
        console.log('✅ Приложение готово')
      })

      return {
        isPiniaReady
      }
    }
  }
</script>

<style>
  .loading-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100vh;
  }

  .loading-spinner {
    border: 4px solid #f3f3f3;
    border-top: 4px solid #3498db;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    animation: spin 1s linear infinite;
    margin-bottom: 20px;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }
</style>
