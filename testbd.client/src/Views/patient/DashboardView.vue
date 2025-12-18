<!-- views/patient/DashboardView.vue -->
<template>
  <div class="patient-dashboard">
    <div class="sidebar">
      <h3>Панель пациента</h3>
      <nav>
        <router-link to="/patient/appointments">Мои записи</router-link>
        <router-link to="/patient/history">История посещений</router-link>
        <router-link to="/patient/profile">Профиль</router-link>
        <button @click="logout">Выйти</button>
      </nav>
    </div>

    <div class="content">
      <router-view></router-view>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const authStore = useAuthStore();

const logout = () => {
  authStore.logout();
};
</script>
