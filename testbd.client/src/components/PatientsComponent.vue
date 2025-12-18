<template>
  <div>
    <h2>Список пациентов</h2>

    <!-- Состояние загрузки -->
    <div v-if="loading" class="loading">
      Загрузка данных...
    </div>

    <!-- Состояние ошибки -->
    <div v-if="error" class="error">
      Ошибка: {{ error }}
    </div>

    <!-- Таблица с пациентами -->
    <div v-if="!loading && !error">
      <table v-if="patients.length > 0" class="patients-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>ФИО</th>
            <th>Дата рождения</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="patient in patients" :key="patient.id">
            <td>{{ patient.patientId }}</td>
            <td>{{ patient.patientFio }}</td>
            <td>
              <button @click="editPatient(patient)">Редактировать</button>
              <button @click="deletePatient(patient.id)">Удалить</button>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-else class="no-data">
        Пациенты не найдены
      </div>
    </div>

    <!-- Кнопка обновления -->
    <button @click="fetchPatients" :disabled="loading">
      {{ loading ? 'Загрузка...' : 'Обновить список' }}
    </button>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';

  const API_BASE_URL = '';

  // Реактивные переменные
  const patients = ref([]);
  const loading = ref(false);
  const error = ref(null);

  // Функция для получения пациентов
  const fetchPatients = async () => {
    loading.value = true;
    error.value = null;

    try {
      const response = await axios.get(`${API_BASE_URL}/Patients/patients`);
      console.log(response.data);
      patients.value = response.data;
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Ошибка при загрузке данных';
      console.error('Ошибка:', err);
    } finally {
      loading.value = false;
    }
  };

  onMounted(() => {
    fetchPatients();
  });
</script>

<style scoped>
  .patients-table {
    width: 100%;
    border-collapse: collapse;
    margin: 20px 0;
  }

    .patients-table th,
    .patients-table td {
      border: 1px solid #ddd;
      padding: 8px;
      text-align: left;
    }

    .patients-table th {
      background-color: #f2f2f2;
    }

  .loading {
    padding: 20px;
    text-align: center;
    color: #666;
  }

  .error {
    padding: 20px;
    text-align: center;
    color: #d32f2f;
    background-color: #ffebee;
    border-radius: 4px;
    margin: 10px 0;
  }

  .no-data {
    padding: 20px;
    text-align: center;
    color: #666;
  }

  button {
    margin: 5px;
    padding: 8px 16px;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }

    button:hover {
      background-color: #0056b3;
    }

    button:disabled {
      background-color: #ccc;
      cursor: not-allowed;
    }
</style>
