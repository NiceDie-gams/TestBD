<!-- views/patient/HistoryView.vue -->
<template>
  <div class="history-container">
    <div class="history-header">
      <h2>История посещений</h2>
    </div>

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Загрузка истории...</p>
    </div>

    <div v-else-if="error" class="error-message">
      <p>❌ {{ error }}</p>
      <button @click="loadHistory" class="retry-btn">Повторить</button>
    </div>

    <div v-else-if="!history || history.length === 0" class="empty-history">
      <div class="empty-icon">
        📋
      </div>
      <h3>История посещений пуста</h3>
      <p>У вас пока нет записей о посещениях</p>
      <router-link to="/patient/appointments" class="link-btn">
        Записаться на прием
      </router-link>
    </div>

    <div v-else class="history-list">
      <div class="history-summary">
        <p>Всего посещений: <strong>{{ history.length }}</strong></p>
        <p v-if="uniqueDoctorsCount > 0">Посещено врачей: <strong>{{ uniqueDoctorsCount }}</strong></p>
        <p v-if="firstVisit && lastVisit">
          Период:
          <strong>{{ formatDate(firstVisit) }}</strong> -
          <strong>{{ formatDate(lastVisit) }}</strong>
        </p>
      </div>

      <div class="history-cards">
        <div v-for="(visit, index) in sortedHistory" :key="index" class="history-card">
          <div class="card-header">
            <span class="visit-date">{{ formatDate(visit.visitDate) }}</span>
            <span class="visit-type" :class="getVisitTypeClass(visit.visit_Type)">
              {{ getVisitTypeText(visit.visitType) }}
            </span>
          </div>

          <div class="card-body">
            <div class="visit-info">
              <div class="info-row">
                <span class="label">Врач:</span>
                <span class="value doctor-name">{{ visit.employeeFio || 'Не указан' }}</span>
              </div>

              <div v-if="visit.prediagnose" class="info-row">
                <span class="label">Предварительный диагноз:</span>
                <span class="value diagnosis">{{ visit.prediagnose }}</span>
              </div>

              <div v-if="visit.diagnose" class="info-row">
                <span class="label">Диагноз:</span>
                <span class="value diagnosis">{{ visit.diagnose }}</span>
              </div>

              <div v-if="!visit.prediagnose && !visit.diagnose" class="info-row">
                <span class="label">Диагноз:</span>
                <span class="value text-muted">Не указан</span>
              </div>
            </div>
          </div>

          <div class="card-footer">
            <div class="visit-meta">
              <span class="patient-id">
                ID пациента: {{ formatPatientId(visit.patientId) }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Фильтры и сортировка -->
      <div class="history-filters" v-if="history.length > 1">
        <div class="filter-group">
          <label for="sortBy">Сортировать по:</label>
          <select id="sortBy" v-model="sortBy" class="form-control">
            <option value="date_desc">Дате (сначала новые)</option>
            <option value="date_asc">Дате (сначала старые)</option>
            <option value="doctor">Врачу</option>
            <option value="type">Типу посещения</option>
          </select>
        </div>

        <div class="filter-group">
          <label for="visitTypeFilter">Тип посещения:</label>
          <select id="visitTypeFilter" v-model="visitTypeFilter" class="form-control">
            <option value="">Все типы</option>
            <option v-for="type in availableVisitTypes" :key="type.value" :value="type.value">
              {{ type.text }}
            </option>
          </select>
        </div>

        <button @click="resetFilters" class="btn btn-secondary">Сбросить фильтры</button>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue';
  import { useAuthStore } from '@/stores/auth';
  import { getVisitHistory } from '@/services/api';

  const authStore = useAuthStore();
  const history = ref([]);
  const loading = ref(true);
  const error = ref(null);
  const sortBy = ref('date_desc');
  const visitTypeFilter = ref('');

  // Вычисляемые свойства
  const uniqueDoctorsCount = computed(() => {
    if (!history.value.length) return 0;
    const doctors = new Set(history.value.map(visit => visit.employeeFio));
    return doctors.size;
  });

  const firstVisit = computed(() => {
    if (!history.value.length) return null;
    const dates = history.value.map(visit => new Date(visit.visitDate));
    return new Date(Math.min(...dates));
  });

  const lastVisit = computed(() => {
    if (!history.value.length) return null;
    const dates = history.value.map(visit => new Date(visit.visitDate));
    return new Date(Math.max(...dates));
  });

  const availableVisitTypes = computed(() => {
    const types = new Set(history.value.map(visit => visit.visit_Type));
    return Array.from(types).map(type => ({
      value: type,
      text: getVisitTypeText(type)
    }));
  });

  const filteredHistory = computed(() => {
    if (!visitTypeFilter.value) return history.value;

    return history.value.filter(visit =>
      visit.visit_Type === visitTypeFilter.value
    );
  });

  const sortedHistory = computed(() => {
    let sorted = [...filteredHistory.value];

    switch (sortBy.value) {
      case 'date_desc':
        sorted.sort((a, b) => new Date(b.visitDate) - new Date(a.visitDate));
        break;
      case 'date_asc':
        sorted.sort((a, b) => new Date(a.visitDate) - new Date(b.visitDate));
        break;
      case 'doctor':
        sorted.sort((a, b) => a.employeeFio.localeCompare(b.employeeFio));
        break;
      case 'type':
        sorted.sort((a, b) => a.visit_Type.localeCompare(b.visit_Type));
        break;
    }

    return sorted;
  });

  const loadHistory = async () => {
    try {
      loading.value = true;
      error.value = null;

      const patientId = authStore.user?.patientId;
      if (!patientId) {
        throw new Error('ID пациента не найден');
      }

      history.value = await getVisitHistory(patientId);
      console.log('Загруженная история:', history.value);

    } catch (err) {
      console.error('Ошибка загрузки истории:', err);
      error.value = err.message || 'Не удалось загрузить историю посещений';
    } finally {
      loading.value = false;
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'Не указана';
    const date = new Date(dateString);
    return date.toLocaleDateString('ru-RU', {
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  };

  const formatDateTime = (dateString) => {
    if (!dateString) return 'Не указана';
    const date = new Date(dateString);
    return date.toLocaleString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const getVisitTypeClass = (visitType) => {
    const typeMap = {
      'Primary': 'type-primary',
      'Secondary': 'type-secondary',
      'Control': 'type-control',
      'Emergency': 'type-emergency'
    };
    return typeMap[visitType] || 'type-unknown';
  };

  const getVisitTypeText = (visitType) => {
    const textMap = {
      'Primary': 'Первичный',
      'Secondary': 'Повторный',
      'Control': 'Контрольный',
      'Emergency': 'Экстренный'
    };
    return textMap[visitType] || visitType || 'Не указан';
  };

  const formatPatientId = (patientId) => {
    if (!patientId) return 'N/A';
    return patientId.substring(0, 8).toUpperCase();
  };

  const resetFilters = () => {
    sortBy.value = 'date_desc';
    visitTypeFilter.value = '';
  };

  onMounted(() => {
    loadHistory();
  });
</script>

<style scoped>
  .history-container {
    padding: 20px;
    max-width: 1200px;
    margin: 0 auto;
  }

  .history-header {
    margin-bottom: 30px;
  }

    .history-header h2 {
      color: #2c3e50;
      font-size: 28px;
      font-weight: 600;
    }

  /* Loading */
  .loading {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 60px 0;
  }

  .spinner {
    width: 50px;
    height: 50px;
    border: 4px solid #f3f3f3;
    border-top: 4px solid #3498db;
    border-radius: 50%;
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

  /* Error */
  .error-message {
    text-align: center;
    padding: 60px 20px;
    background-color: #ffeaea;
    border-radius: 10px;
    margin: 20px 0;
  }

    .error-message p {
      color: #e74c3c;
      font-size: 16px;
      margin-bottom: 20px;
    }

  .retry-btn {
    padding: 10px 20px;
    background-color: #e74c3c;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 14px;
    transition: background-color 0.3s;
  }

    .retry-btn:hover {
      background-color: #c0392b;
    }

  /* Empty History */
  .empty-history {
    text-align: center;
    padding: 80px 20px;
    background-color: #f8f9fa;
    border-radius: 10px;
    border: 2px dashed #dee2e6;
  }

  .empty-icon {
    font-size: 64px;
    margin-bottom: 20px;
  }

  .empty-history h3 {
    color: #6c757d;
    margin-bottom: 10px;
    font-size: 24px;
  }

  .empty-history p {
    color: #6c757d;
    margin-bottom: 30px;
    font-size: 16px;
  }

  .link-btn {
    display: inline-block;
    padding: 12px 24px;
    background-color: #3498db;
    color: white;
    text-decoration: none;
    border-radius: 5px;
    font-size: 14px;
    transition: background-color 0.3s;
  }

    .link-btn:hover {
      background-color: #2980b9;
    }

  /* History List */
  .history-summary {
    background-color: #e8f4fc;
    padding: 15px 20px;
    border-radius: 8px;
    margin-bottom: 20px;
    font-size: 16px;
    display: flex;
    flex-wrap: wrap;
    gap: 20px;
    align-items: center;
  }

    .history-summary p {
      margin: 0;
    }

  .history-filters {
    display: flex;
    flex-wrap: wrap;
    gap: 15px;
    margin-bottom: 20px;
    padding: 15px;
    background-color: #f8f9fa;
    border-radius: 8px;
    align-items: center;
  }

  .filter-group {
    display: flex;
    flex-direction: column;
    gap: 5px;
  }

    .filter-group label {
      font-size: 14px;
      color: #666;
      font-weight: 500;
    }

  .form-control {
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
    min-width: 200px;
  }

  .btn {
    padding: 8px 16px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    transition: background-color 0.3s;
  }

  .btn-secondary {
    background-color: #6c757d;
    color: white;
  }

    .btn-secondary:hover {
      background-color: #5a6268;
    }

  .history-cards {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
    gap: 20px;
  }

  .history-card {
    background: white;
    border-radius: 10px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    border: 1px solid #e0e0e0;
    overflow: hidden;
    transition: transform 0.3s, box-shadow 0.3s;
  }

    .history-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 5px 20px rgba(0,0,0,0.15);
    }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 15px;
    background-color: #f8f9fa;
    border-bottom: 1px solid #e0e0e0;
  }

  .visit-date {
    font-weight: 600;
    color: #2c3e50;
    font-size: 16px;
  }

  .visit-type {
    padding: 4px 12px;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 600;
  }

  .type-primary {
    background-color: #d4edda;
    color: #155724;
  }

  .type-secondary {
    background-color: #cce5ff;
    color: #004085;
  }

  .type-control {
    background-color: #fff3cd;
    color: #856404;
  }

  .type-emergency {
    background-color: #f8d7da;
    color: #721c24;
  }

  .type-unknown {
    background-color: #e2e3e5;
    color: #383d41;
  }

  .card-body {
    padding: 15px;
  }

  .info-row {
    display: flex;
    margin-bottom: 10px;
    line-height: 1.5;
  }

    .info-row:last-child {
      margin-bottom: 0;
    }

  .label {
    flex: 0 0 160px;
    color: #666;
    font-weight: 500;
    font-size: 14px;
  }

  .value {
    flex: 1;
    color: #333;
    font-size: 14px;
  }

  .doctor-name {
    font-weight: 600;
    color: #2c3e50;
  }

  .diagnosis {
    font-style: italic;
    color: #495057;
  }

  .text-muted {
    color: #6c757d;
    font-style: italic;
  }

  .card-footer {
    padding: 12px 15px;
    background-color: #f8f9fa;
    border-top: 1px solid #e0e0e0;
    font-size: 12px;
  }

  .visit-meta {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .patient-id {
    color: #6c757d;
    font-family: monospace;
  }

  /* Responsive */
  @media (max-width: 768px) {
    .history-cards {
      grid-template-columns: 1fr;
    }

    .history-container {
      padding: 15px;
    }

    .info-row {
      flex-direction: column;
    }

    .label {
      flex: none;
      margin-bottom: 4px;
    }

    .history-filters {
      flex-direction: column;
      align-items: stretch;
    }

    .filter-group {
      width: 100%;
    }

    .form-control {
      min-width: unset;
      width: 100%;
    }

    .history-summary {
      flex-direction: column;
      align-items: flex-start;
      gap: 10px;
    }
  }
</style>
