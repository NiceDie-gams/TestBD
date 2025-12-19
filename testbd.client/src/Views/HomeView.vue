<template>
  <div class="home">
    <!-- Hero секция -->
    <header class="hero-section">
      <div class="hero-overlay">
        <div class="container">
          <div class="hero-content">
            <h1 class="hero-title">Медицинская клиника "Здоровье"</h1>
            <p class="hero-subtitle">Современная медицина, забота о вашем здоровье</p>
            <div class="hero-stats">
              <div class="stat-item">
                <span class="stat-number">{{ services.length }}+</span>
                <span class="stat-label">Медицинских услуг</span>
              </div>
              <div class="stat-item">
                <span class="stat-number">{{ doctors.length }}+</span>
                <span class="stat-label">Квалифицированных врачей</span>
              </div>
              <div class="stat-item">
                <span class="stat-number">24/7</span>
                <span class="stat-label">Поддержка пациентов</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Навигационная панель -->
    <nav class="main-nav">
      <div class="container">
        <div class="nav-content">
          <router-link to="/" class="nav-logo">
            <img src="../assets/icons/health-medical-healthcare-heart.svg" class="logo-icon"/>
            <span class="logo-text">Клиника "Здоровье"</span>
          </router-link>

          <div class="nav-links">
            <router-link to="/" class="nav-link active">Главная</router-link>
            <a href="#Services" class="nav-link">Услуги</a>
            <a href="#Doctors" class="nav-link">Врачи</a>
            <a href="#Contacts" class="nav-link">Контакты</a>
          </div>

          <div class="nav-auth" v-if="!isAuthenticated">
            <router-link to="/login" class="btn btn-outline">Вход</router-link>
            <router-link to="/register" class="btn btn-primary">Регистрация</router-link>
          </div>

          <div class="nav-user" v-else>
            <div class="user-info">
              <div class="user-greeting">Добро пожаловать</div>
              <div class="user-name">{{ userName }}</div>
            </div>
            <div class="user-actions">
              <router-link v-if="isUser" to="/patient" class="btn btn-outline">Личный кабинет</router-link>
              <router-link v-if="isEmployee" to="/doctor" class="btn btn-outline">Панель врача</router-link>
              <router-link v-if="isAdmin" to="/admin" class="btn btn-outline">Админ-панель</router-link>
              <button @click="logout" class="btn btn-logout">Выйти</button>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Основной контент -->
    <main class="main-content">
      <div class="container">
        <a name="Services"></a>
        <!-- Секция услуг -->
        <section class="section services-section">
          <div class="section-header">
            <h2 class="section-title">Наши медицинские услуги</h2>
            <p class="section-subtitle">Полный спектр медицинских услуг для вашего здоровья</p>
          </div>

          <div v-if="loading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Загрузка услуг...</p>
          </div>

          <div v-else-if="services.length > 0" class="services-grid">
            <div v-for="service in services" :key="service.serviceCode" class="service-card">
              <div class="service-icon">
                <div class="icon-circle"></div>
              </div>
              <div class="service-content">
                <h3 class="service-title">{{ service.serviceName }}</h3>
                <p class="service-description">{{ service.description }}</p>
                <div class="service-footer">
                  <div class="service-price">
                    <span class="price-amount">{{ formatPrice(service.basePrice) }}</span>
                    <span class="price-unit">руб.</span>
                  </div>
                  <div class="service-code">Код: {{ service.serviceCode }}</div>
                </div>
              </div>
            </div>
          </div>

          <div v-else class="empty-state">
            <div class="empty-icon"></div>
            <p class="empty-text">Услуги временно недоступны</p>
            <button @click="loadData" class="btn btn-primary">Обновить</button>
          </div>

        </section>

        <!-- Секция врачей -->
        <a name="Doctors"></a>
        <section class="section doctors-section">
          <div class="section-header">
            <h2 class="section-title">Наши специалисты</h2>
            <p class="section-subtitle">Квалифицированные врачи с многолетним опытом</p>
          </div>

          <div v-if="loading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Загрузка врачей...</p>
          </div>

          <div v-else-if="doctors.length > 0" class="doctors-grid">
            <div v-for="doctor in doctors" :key="doctor.employeeId" class="doctor-card">
              <div class="doctor-avatar">
                <div class="avatar-placeholder">
                  {{ getInitials(doctor.employeeFio) }}
                </div>
              </div>
              <div class="doctor-info">
                <h3 class="doctor-name">{{ doctor.employeeFio }}</h3>
                <p class="doctor-position">{{ doctor.post }}</p>
                <div class="doctor-specialty">{{ doctor.specialization }}</div>
                <div class="doctor-experience">
                  <span class="exp-icon">📅</span>
                  <span>Опыт: {{ doctor.experience }} лет</span>
                </div>
                <div class="doctor-contacts">
                  <div v-if="doctor.phone" class="contact-item">
                    <span class="contact-icon">📞</span>
                    <span>{{ doctor.phone }}</span>
                  </div>
                  <div v-if="doctor.email" class="contact-item">
                    <span class="contact-icon">✉️</span>
                    <span>{{ doctor.email }}</span>
                  </div>
                </div>
              </div>
              <div class="doctor-actions">
                <button v-if="isAuthenticated && isUser"
                        @click="bookAppointment(doctor)"
                        class="btn btn-primary">
                  Записаться на прием
                </button>
                <button v-else-if="!isAuthenticated"
                        @click="goToLogin"
                        class="btn btn-outline">
                  Войти для записи
                </button>
                <router-link to="/doctors" class="btn btn-link">
                  Подробнее
                </router-link>
              </div>
            </div>
          </div>

          <div v-else class="empty-state">
            <div class="empty-icon"></div>
            <p class="empty-text">Врачи временно недоступны</p>
            <button @click="loadData" class="btn btn-primary">Обновить</button>
          </div>
        </section>

        <!-- Секция преимуществ -->
        <section class="section advantages-section">
          <div class="section-header">
            <h2 class="section-title">Почему выбирают нас</h2>
            <p class="section-subtitle">Наши преимущества для вашего комфорта и здоровья</p>
          </div>

          <div class="advantages-grid">
            <div class="advantage-card">
              <h3 class="advantage-title">Современное оборудование</h3>
              <p class="advantage-description">Используем новейшее медицинское оборудование для точной диагностики</p>
            </div>

            <div class="advantage-card">
              <h3 class="advantage-title">Опытные специалисты</h3>
              <p class="advantage-description">Врачи высшей категории с многолетним практическим опытом</p>
            </div>

            <div class="advantage-card">
              <h3 class="advantage-title">Удобное время</h3>
              <p class="advantage-description">Работаем 5 дней в неделю, запись на удобное для вас время</p>
            </div>

            <div class="advantage-card">
              <h3 class="advantage-title">Удобное расположение</h3>
              <p class="advantage-description">Клиника расположена в центре города с парковкой</p>
            </div>
          </div>
        </section>
      </div>
    </main>

    <!-- Футер -->
    <footer class="main-footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-section">
            <div class="footer-logo">
              <span class="logo-text">Клиника "Здоровье"</span>
            </div>
            <p class="footer-description">
              Медицинская клиника, предоставляющая полный спектр медицинских услуг с заботой о каждом пациенте.
            </p>
          </div>
          <a name="Contacts" ></a>
          <div class="footer-section">
            <h3 class="footer-title">Контакты</h3>
            <div class="footer-contacts">
              <div class="contact-item">
                <span class="contact-label">Адрес:</span>
                <span>г. Севастополь, ул. Вакуленчука, д. 17</span>
              </div>
              <div class="contact-item">
                <span class="contact-label">Телефон:</span>
                <span>+7 (978) 123-45-67</span>
              </div>
              <div class="contact-item">
                <span class="contact-label">Email:</span>
                <span>info@clinic-health.ru</span>
              </div>
            </div>
          </div>

          <div class="footer-section">
            <h3 class="footer-title">Режим работы</h3>
            <div class="working-hours">
              <div class="hours-item">
                <span>Пн-Пт:</span>
                <span>9:00 - 18:00</span>
              </div>
            </div>
          </div>
        </div>

        <div class="footer-bottom">
          <p class="copyright">© 2025 Медицинская клиника "Здоровье". Все права защищены.</p>
          <div class="footer-debug" v-if="showDebug">
            <span>API: {{ apiUrl }}</span>
            <span>Env: {{ envMode }}</span>
          </div>
        </div>
      </div>
    </footer>
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

  const formatPrice = (price) => {
    return new Intl.NumberFormat('ru-RU').format(price);
  };

  const getInitials = (fullName) => {
    if (!fullName) return '';
    return fullName
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase();
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
  /* Общие стили */
  .home {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  }

  .container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 20px;
  }

  /* Hero секция */
  .hero-section {
    background: linear-gradient(rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.7)), url('https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1950&q=80');
    background-size: cover;
    background-position: center;
    color: white;
    padding: 120px 0;
    text-align: center;
  }

  .hero-title {
    font-size: 3.5rem;
    font-weight: 700;
    margin-bottom: 1rem;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
  }

  .hero-subtitle {
    font-size: 1.5rem;
    margin-bottom: 3rem;
    opacity: 0.9;
  }

  .hero-stats {
    display: flex;
    justify-content: center;
    gap: 3rem;
    flex-wrap: wrap;
  }

  .stat-item {
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .stat-number {
    font-size: 2.5rem;
    font-weight: 700;
    color: #4CAF50;
  }

  .stat-label {
    font-size: 1rem;
    opacity: 0.8;
  }

  /* Навигация */
  .main-nav {
    background: white;
    box-shadow: 0 2px 20px rgba(0, 0, 0, 0.1);
    position: sticky;
    top: 0;
    z-index: 1000;
  }

  .nav-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 0;
  }

  .nav-logo {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    text-decoration: none;
    font-size: 1.5rem;
    font-weight: 700;
    color: #2c3e50;
  }

  .logo-icon {
    position: relative;
    width: 10vw;
    height: 10vh;
    font-size: 2rem;
  }

  .nav-links {
    display: flex;
    gap: 2rem;
  }

  .nav-link {
    text-decoration: none;
    color: #666;
    font-weight: 500;
    padding: 0.5rem 0;
    position: relative;
    transition: color 0.3s;
  }

    .nav-link:hover,
    .nav-link.active {
      color: #4CAF50;
    }

      .nav-link.active::after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        height: 2px;
        background: #4CAF50;
      }

  /* Кнопки */
  .btn {
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
  }

  .btn-primary {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
  }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
    }

  .btn-outline {
    background: transparent;
    border: 2px solid #667eea;
    color: #667eea;
  }

    .btn-outline:hover {
      background: #667eea;
      color: white;
    }

  .btn-link {
    background: transparent;
    color: #667eea;
    padding: 0.5rem;
  }

    .btn-link:hover {
      text-decoration: underline;
    }

  .btn-logout {
    background: #ff6b6b;
    color: white;
  }

    .btn-logout:hover {
      background: #ff5252;
    }

  /* Основной контент */
  .main-content {
    flex: 1;
    background: #f8f9fa;
    padding: 3rem 0;
  }

  .section {
    margin-bottom: 4rem;
  }

  .section-header {
    text-align: center;
    margin-bottom: 3rem;
  }

  .section-title {
    font-size: 2.5rem;
    color: #2c3e50;
    margin-bottom: 1rem;
  }

  .section-subtitle {
    font-size: 1.2rem;
    color: #666;
    max-width: 600px;
    margin: 0 auto;
  }

  /* Сетки */
  .services-grid,
  .doctors-grid,
  .advantages-grid {
    display: grid;
    gap: 2rem;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  }

  /* Карточки */
  .service-card {
    background: white;
    border-radius: 12px;
    padding: 2rem;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    transition: transform 0.3s, box-shadow 0.3s;
    position: relative;
    overflow: hidden;
  }

    .service-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
    }

  .service-icon {
    margin-bottom: 1rem;
  }

  .icon-circle {
    width: 60px;
    height: 60px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border-radius: 50%;
    margin: 0 auto;
  }

  .service-title {
    font-size: 1.5rem;
    color: #2c3e50;
    margin-bottom: 1rem;
  }

  .service-description {
    color: #666;
    margin-bottom: 1.5rem;
    line-height: 1.6;
  }

  .service-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
  }

  .service-price {
    display: flex;
    align-items: baseline;
    gap: 0.25rem;
  }

  .price-amount {
    font-size: 1.75rem;
    font-weight: 700;
    color: #4CAF50;
  }

  .price-unit {
    color: #666;
  }

  .service-code {
    font-size: 0.9rem;
    color: #999;
    background: #f8f9fa;
    padding: 0.25rem 0.5rem;
    border-radius: 4px;
  }

  .btn-service-book {
    width: 100%;
    background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
    color: white;
    border: none;
    padding: 0.75rem;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.3s;
  }

    .btn-service-book:hover {
      background: linear-gradient(135deg, #45a049 0%, #3d8b40 100%);
    }

  /* Врачи */
  .doctor-card {
    background: white;
    border-radius: 12px;
    overflow: hidden;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    transition: transform 0.3s;
  }

    .doctor-card:hover {
      transform: translateY(-5px);
    }

  .doctor-avatar {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    padding: 2rem;
    text-align: center;
  }

  .avatar-placeholder {
    width: 100px;
    height: 100px;
    background: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto;
    font-size: 2rem;
    font-weight: 700;
    color: #667eea;
  }

  .doctor-info {
    padding: 1.5rem;
  }

  .doctor-name {
    font-size: 1.5rem;
    color: #2c3e50;
    margin-bottom: 0.5rem;
  }

  .doctor-position {
    color: #667eea;
    font-weight: 600;
    margin-bottom: 0.5rem;
  }

  .doctor-specialty {
    background: #f0f4ff;
    color: #667eea;
    padding: 0.5rem 1rem;
    border-radius: 20px;
    display: inline-block;
    margin-bottom: 1rem;
    font-size: 0.9rem;
  }

  .doctor-experience,
  .contact-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    color: #666;
    margin-bottom: 0.5rem;
  }

  .doctor-actions {
    padding: 1.5rem;
    padding-top: 0;
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
  }

  /* Преимущества */
  .advantages-section {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 4rem 0;
    border-radius: 20px;
  }

  .advantages-grid {
    display: flex;
    flex-wrap: wrap;
    justify-content: space-around;
    gap: 2rem;
  }

  .advantage-card {
    width: 15vw;
    height: 25vh;
    text-align: center;
    padding: 2rem;
    background: rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(10px);
    border-radius: 12px;
    transition: transform 0.3s;
  }

    .advantage-card:hover {
      transform: translateY(-5px);
      background: rgba(255, 255, 255, 0.15);
    }

  .advantage-icon {
    font-size: 3rem;
    margin-bottom: 1rem;
  }

  .advantage-title {
    font-size: 1.5rem;
    margin-bottom: 1rem;
  }

  .advantage-description {
    opacity: 0.9;
    line-height: 1.6;
  }

  /* Состояния загрузки */
  .loading-state {
    text-align: center;
    padding: 3rem;
  }

  .loading-spinner {
    width: 50px;
    height: 50px;
    border: 3px solid #f3f3f3;
    border-top: 3px solid #667eea;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin: 0 auto 1rem;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }

  .empty-state {
    text-align: center;
    padding: 4rem 2rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  }

  .empty-icon {
    width: 80px;
    height: 80px;
    background: #f0f4ff;
    border-radius: 50%;
    margin: 0 auto 1.5rem;
    position: relative;
  }

    .empty-icon::before {
      content: '?';
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      font-size: 2rem;
      color: #667eea;
    }

  .empty-text {
    font-size: 1.2rem;
    color: #666;
    margin-bottom: 1.5rem;
  }

  /* Футер */
  .main-footer {
    background: #2c3e50;
    color: white;
    padding: 3rem 0 1.5rem;
  }

  .footer-content {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 3rem;
    margin-bottom: 3rem;
  }

  .footer-section {
    display: flex;
    flex-direction: column;
  }

  .footer-logo {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-size: 1.5rem;
    font-weight: 700;
    margin-bottom: 1rem;
  }

  .footer-description {
    opacity: 0.8;
    line-height: 1.6;
  }

  .footer-title {
    font-size: 1.2rem;
    margin-bottom: 1.5rem;
    color: #4CAF50;
  }

  .footer-contacts .contact-item,
  .working-hours .hours-item {
    display: flex;
    justify-content: space-between;
    margin-bottom: 0.75rem;
    padding-bottom: 0.75rem;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  .contact-label {
    font-weight: 600;
    color: #4CAF50;
  }

  .footer-bottom {
    border-top: 1px solid rgba(255, 255, 255, 0.1);
    padding-top: 1.5rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    gap: 1rem;
  }

  .copyright {
    opacity: 0.7;
  }

  .footer-debug {
    display: flex;
    gap: 1rem;
    font-size: 0.9rem;
    opacity: 0.5;
  }

  /* Адаптивность */
  @media (max-width: 768px) {
    .hero-title {
      font-size: 2.5rem;
    }

    .hero-stats {
      flex-direction: column;
      gap: 1.5rem;
    }

    .nav-content {
      flex-direction: column;
      gap: 1rem;
    }

    .nav-links {
      flex-wrap: wrap;
      justify-content: center;
    }

    .nav-user {
      text-align: center;
    }

    .section-title {
      font-size: 2rem;
    }

    .services-grid,
    .doctors-grid {
      grid-template-columns: 1fr;
    }

    .footer-bottom {
      flex-direction: column;
      text-align: center;
    }
  }
</style>
