<!-- views/doctor/DashboardView.vue -->
<template>
  <div class="doctor-dashboard">
    <div class="sidebar">
      <h3>Панель врача</h3>
      <nav>
        <router-link to="/doctor/schedule">Расписание</router-link>
        <router-link to="/doctor/patients">Пациенты</router-link>
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
