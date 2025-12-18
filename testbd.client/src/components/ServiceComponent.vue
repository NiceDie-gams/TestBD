<template>
  <div>
    <div>
      <h3>
        Перечень услуг
      </h3>
      <ul>
        <li v-for="service in services" :key ="service.serviceCode">
          <pre>
            {{service.serviceName}}
            {{service.description}}
            {{service.basePrice}}
          </pre>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';

  const API_BASE_URL = '';

  // Реактивные переменные
  const services = ref([]);
  const loading = ref(false);
  const error = ref(null);

  // Функция для получения пациентов
  const fetchServices = async () => {
  loading.value = true;
  error.value = null;

  try {
    const response = await axios.get(`${API_BASE_URL}/Service/services`);
  console.log(response.data);
  services.value = response.data;
  } catch (err) {
  error.value = err.response?.data?.message || err.message || 'Ошибка при загрузке данных';
  console.error('Ошибка:', err);
  } finally {
  loading.value = false;
  }
  };

  onMounted(() => {
  fetchServices();
  });
</script>
