<!-- views/HomeView.vue - с улучшенной отладкой -->
<template>
  <div class="home">
    <header class="header">
      <h1>Медицинская клиника</h1>
      <div class="debug-info" v-if="showDebug">
        <small>API: {{ apiUrl }}</small>
        <small>Env: {{ envMode }}</small>
      </div>
      <nav>
        <router-link to="/">Главная</router-link>
        <router-link to="/login" v-if="!isAuthenticated">Вход</router-link>
        <router-link to="/register" v-if="!isAuthenticated">Регистрация</router-link>
        <template v-else>
          <span>Добро пожаловать, {{ userName }}</span>
          <button @click="logout">Выйти</button>

          <router-link v-if="isUser" to="/patient" class="role-btn">Пациент</router-link>
          <router-link v-if="isEmployee" to="/doctor" class="role-btn">Врач</router-link>
          <router-link v-if="isAdmin" to="/admin" class="role-btn">Админ</router-link>
        </template>
        <button @click="showDebug = !showDebug" class="debug-btn">Debug</button>
      </nav>
    </header>

    <main>
      <!-- Отладочная информация -->
      <div v-if="showDebug" class="debug-panel">
        <h3>Отладка API</h3>
        <p>API URL: {{ apiUrl }}</p>
        <p>Doctors endpoint: {{ apiUrl }}/Employee/employee</p>
        <p>Services endpoint: {{ apiUrl }}/Service/services</p>
        <button @click="testEndpoints">Тест endpoints</button>
        <button @click="loadData">Перезагрузить данные</button>
        <div v-if="debugResults">
          <h4>Результаты теста:</h4>
          <pre>{{ debugResults }}</pre>
        </div>
      </div>

      <!-- Секция услуг -->
      <section class="services-section">
        <h2>Наши услуги ({{ services.length }})</h2>
        <div v-if="loading" class="loading">Загрузка услуг...</div>
        <div v-else-if="services.length > 0" class="services-grid">
          <div v-for="service in services" :key="service.serviceCode" class="service-card">
            <h3>{{ service.serviceName }}</h3>
            <p>{{ service.description }}</p>
            <span class="price">{{ service.basePrice }} руб.</span>
            <small>Код: {{ service.serviceCode }}</small>
          </div>
        </div>
        <div v-else class="empty-state">
          <p>Услуги не загружены</p>
          <button @click="loadData">Повторить попытку</button>
        </div>
      </section>

      <!-- Секция врачей -->
      <section class="doctors-section">
        <h2>Наши врачи ({{ doctors.length }})</h2>
        <div v-if="loading" class="loading">Загрузка врачей...</div>
        <div v-else-if="doctors.length > 0" class="doctors-grid">
          <div v-for="doctor in doctors" :key="doctor.employeeId" class="doctor-card">
            <h3>{{ doctor.employeeFio }}</h3>
            <p><strong>{{ doctor.post }}</strong></p>
            <p>Специализация: {{ doctor.specialization }}</p>
            <p>Опыт: {{ doctor.experience }} лет</p>
            <p v-if="doctor.phone">📞 {{ doctor.phone }}</p>
            <p v-if="doctor.email">✉️ {{ doctor.email }}</p>
            <button v-if="isAuthenticated && isUser"
                    @click="bookAppointment(doctor)"
                    class="book-btn">
              Записаться на прием
            </button>
            <button v-else-if="!isAuthenticated"
                    @click="goToLogin"
                    class="book-btn">
              Войдите для записи
            </button>
          </div>
        </div>
        <div v-else class="empty-state">
          <p>Врачи не загружены</p>
          <button @click="loadData">Повторить попытку</button>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue';
  import { useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';
  import axios from 'axios'; // Импортируем axios напрямую для отладки

  const router = useRouter();
  const authStore = useAuthStore();

  const doctors = ref([]);
  const services = ref([]);
  const loading = ref(false);
  const showDebug = ref(false);
  const debugResults = ref(null);

  const apiUrl = computed(() => axios.defaults.baseURL || 'не установлен');
  const envMode = computed(() => import.meta.env.MODE);

  const isAuthenticated = computed(() => authStore.isAuthenticated);
  const userName = computed(() => authStore.userName);
  const isUser = computed(() => authStore.isUser);
  const isEmployee = computed(() => authStore.isEmployee);
  const isAdmin = computed(() => authStore.isAdmin);

  const testEndpoints = async () => {
    const endpoints = [
      { url: '/Employee/employee', name: 'Врачи' },
      { url: '/Service/services', name: 'Услуги' },
    ];

    const results = [];

    for (const endpoint of endpoints) {
      try {
        console.log(`Тестируем ${endpoint.name}: ${endpoint.url}`);
        const response = await axios.get(endpoint.url);
        results.push({
          name: endpoint.name,
          status: '✅ Успех',
          statusCode: response.status,
          dataCount: Array.isArray(response.data) ? response.data.length : 'N/A',
          dataSample: Array.isArray(response.data) && response.data.length > 0
            ? response.data[0]
            : response.data
        });
      } catch (error) {
        results.push({
          name: endpoint.name,
          status: '❌ Ошибка',
          statusCode: error.response?.status,
          error: error.message,
          details: error.response?.data
        });
      }
    }

    debugResults.value = results;
    console.table(results);
  };

  const loadData = async () => {
    try {
      loading.value = true;
      console.log('Начинаем загрузку данных...');

      // Загружаем данные параллельно
      const [doctorsResponse, servicesResponse] = await Promise.all([
        axios.get('/Employee/employee'),
        axios.get('/Service/services')
      ]);

      console.log('Данные врачей:', doctorsResponse.data);
      console.log('Данные услуг:', servicesResponse.data);

      doctors.value = doctorsResponse.data;
      services.value = servicesResponse.data;

      console.log(`Загружено ${doctors.value.length} врачей и ${services.value.length} услуг`);

    } catch (error) {
      console.error('🔥 Критическая ошибка загрузки данных:');
      console.error('Сообщение:', error.message);
      console.error('Полная ошибка:', error);

      // Показываем детали ошибки
      if (error.response) {
        console.error('Статус:', error.response.status);
        console.error('Данные ответа:', error.response.data);
        console.error('Заголовки:', error.response.headers);
      } else if (error.request) {
        console.error('Запрос был сделан, но ответ не получен');
        console.error('Запрос:', error.request);
      }

      // Используем мок-данные в случае ошибки
      doctors.value = [
        {
          employeeId: '11111111-1111-1111-1111-111111111111',
          employeeFio: 'Иванов Иван Иванович',
          post: 'Терапевт',
          specialization: 'Общая терапия',
          experience: 12
        }
      ];

      services.value = [
        {
          serviceCode: 'TEST001',
          serviceName: 'Тестовая услуга',
          description: 'Описание тестовой услуги',
          basePrice: 1000
        }
      ];

    } finally {
      loading.value = false;
    }
  };

  const bookAppointment = (doctor) => {
    if (!authStore.isAuthenticated) {
      router.push('/login');
      return;
    }

    router.push({ 
    path: '/booking',
    query: { doctorId: doctor.employeeId, doctorName: doctor.employeeFio } 
    });
  };

  const goToLogin = () => {
    router.push('/login');
  };

  const logout = () => {
    authStore.logout();
  };

  onMounted(() => {
    console.log('HomeView mounted');
    console.log('Base URL:', axios.defaults.baseURL);
    console.log('Environment:', import.meta.env);

    loadData();
  });
</script>

<style scoped>
  .home {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
  }

  .header {
    display: flex;
    flex-direction: column;
    gap: 15px;
    padding: 20px 0;
    border-bottom: 2px solid #e0e0e0;
    margin-bottom: 40px;
  }

    .header h1 {
      margin: 0;
      color: #2c3e50;
    }

  .debug-info {
    display: flex;
    gap: 20px;
    font-size: 12px;
    color: #666;
    background: #f5f5f5;
    padding: 5px 10px;
    border-radius: 4px;
  }

  .header nav {
    display: flex;
    gap: 15px;
    align-items: center;
    flex-wrap: wrap;
  }

  .header a {
    text-decoration: none;
    color: #2c3e50;
    padding: 8px 16px;
    border-radius: 4px;
    transition: background-color 0.3s;
  }

    .header a:hover {
      background-color: #f5f5f5;
    }

  .role-btn {
    background-color: #3498db;
    color: white !important;
  }

  .debug-btn {
    padding: 5px 10px;
    background-color: #95a5a6;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 12px;
  }

  .header button {
    padding: 8px 16px;
    background-color: #e74c3c;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }

  .debug-panel {
    background: #f8f9fa;
    border: 1px solid #dee2e6;
    border-radius: 8px;
    padding: 20px;
    margin-bottom: 30px;
  }

    .debug-panel h3 {
      margin-top: 0;
      color: #495057;
    }

    .debug-panel button {
      margin-right: 10px;
      padding: 8px 16px;
      background: #6c757d;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }

      .debug-panel button:hover {
        background: #5a6268;
      }

  .services-section, .doctors-section {
    margin-bottom: 60px;
  }

    .services-section h2, .doctors-section h2 {
      color: #2c3e50;
      margin-bottom: 30px;
      text-align: center;
    }

  .loading {
    text-align: center;
    padding: 40px;
    color: #7f8c8d;
    font-size: 1.2rem;
  }

  .empty-state {
    text-align: center;
    padding: 40px;
    background: #f8f9fa;
    border-radius: 8px;
    color: #6c757d;
  }

    .empty-state button {
      margin-top: 15px;
      padding: 10px 20px;
      background: #007bff;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }

  .services-grid, .doctors-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 30px;
  }

  .service-card, .doctor-card {
    background: white;
    border-radius: 12px;
    padding: 25px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
    transition: transform 0.3s, box-shadow 0.3s;
  }

    .service-card:hover, .doctor-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
    }

    .service-card h3, .doctor-card h3 {
      color: #2c3e50;
      margin-bottom: 15px;
      font-size: 1.3rem;
    }

  .price {
    display: inline-block;
    margin-top: 15px;
    padding: 8px 16px;
    background-color: #2ecc71;
    color: white;
    border-radius: 20px;
    font-weight: bold;
  }

  .doctor-card p {
    margin: 8px 0;
    color: #555;
  }

  .book-btn {
    margin-top: 15px;
    padding: 10px 20px;
    background-color: #3498db;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    width: 100%;
    font-size: 1rem;
  }

    .book-btn:hover {
      background-color: #2980b9;
    }

  small {
    display: block;
    margin-top: 10px;
    color: #7f8c8d;
    font-size: 12px;
  }
</style>
