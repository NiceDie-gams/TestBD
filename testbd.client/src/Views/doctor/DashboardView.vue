<template>
  <div class="doctor-dashboard">
    <!-- Шапка панели врача -->
    <header class="doctor-header">
      <div class="container">
        <div class="header-content">
          <div class="header-logo">
            <router-link to="/" class="logo-link">
              <img src="../../assets/icons/health-medical-healthcare-heart.svg" class="logo-icon" />
              <span class="logo-text">Клиника "Здоровье"</span>
            </router-link>
            <div class="user-badge">
              <div class="user-avatar">
                {{ getUserInitials }}
              </div>
              <div class="user-info">
                <div class="user-name">{{ userName }}</div>
                <div class="user-role">Врач</div>
                <div class="doctor-specialty" v-if="doctorSpecialty">{{ doctorSpecialty }}</div>
              </div>
            </div>
          </div>

          <nav class="header-nav">
            <router-link to="/" class="nav-link">
              <span class="nav-text">На главную</span>
            </router-link>

            <div class="nav-separator"></div>

            <router-link :to="{ name: 'DoctorSchedule' }"
                         :class="['nav-link', { active: $route.name === 'DoctorSchedule' }]">
              <span class="nav-text">Расписание</span>
            </router-link>

            <router-link :to="{ name: 'Patients' }"
                         :class="['nav-link', { active: $route.name === 'Patients' }]">
              <span class="nav-text">Пациенты</span>
            </router-link>

            <div class="nav-separator"></div>

          </nav>

          <div class="header-actions">
            <div class="current-time">
              <span class="time-text">{{ currentTime }}</span>
            </div>
            <button @click="logout" class="btn-logout">
              <span class="logout-text">Выйти</span>
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Основной контент -->
    <main class="doctor-main">
      <div class="container">
        <!-- Хлебные крошки -->
        <nav class="breadcrumbs" v-if="breadcrumbs.length > 0">
          <router-link to="/" class="breadcrumb-item">Главная</router-link>
          <span class="breadcrumb-separator">/</span>
          <router-link to="/doctor" class="breadcrumb-item">Панель врача</router-link>
          <span v-for="(crumb, index) in breadcrumbs" :key="crumb.path" class="breadcrumb-segment">
            <span class="breadcrumb-separator">/</span>
            <router-link :to="crumb.path"
                         :class="['breadcrumb-item', { 'last': index === breadcrumbs.length - 1 }]">
              {{ crumb.title }}
            </router-link>
          </span>
        </nav>

        <!-- Заголовок страницы -->
        <div class="page-header">
          <div class="page-header-content">
            <div>
              <h1 class="page-title">{{ pageTitle }}</h1>
              <p class="page-subtitle" v-if="pageSubtitle">{{ pageSubtitle }}</p>
            </div>
            <div class="page-actions" v-if="showPageActions">
              <button v-if="$route.name === 'Patients'"
                      @click="searchPatients"
                      class="btn btn-outline">
                <span class="action-icon">🔍</span>
                <span>Поиск пациента</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Контент -->
        <div class="content-wrapper">
          <router-view></router-view>

          <!-- Заглушка если нет активного дочернего маршрута -->
          <div v-if="!hasActiveChildRoute" class="empty-content">
            <div class="empty-icon">
              <img src="../../assets/icons/doctor.svg" class="img-icon"/>
            </div>
            <h2>Добро пожаловать в панель врача!</h2>
            <p>Здесь вы можете управлять своим расписанием и просматривать информацию о пациентах.</p>
            <div class="empty-actions">
              <router-link :to="{ name: 'DoctorSchedule' }" class="btn btn-primary">
                Перейти к расписанию
              </router-link>
              <router-link :to="{ name: 'Patients' }" class="btn btn-outline">
                Посмотреть пациентов
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Футер -->
    <footer class="doctor-footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-info">
            <p class="footer-text">© 2025 Клиника "Здоровье". Панель врача v1.0</p>
            <p class="footer-text">Рабочий день: {{ workHours }}</p>
          </div>
          <div class="footer-status">
            <div class="status-indicator" :class="{ 'active': isOnline }"></div>
            <span class="status-text">{{ isOnline ? 'Онлайн' : 'Оффлайн' }}</span>
          </div>
        </div>
      </div>
    </footer>

    <!-- Быстрые действия (плавающая панель) -->
    <div class="quick-actions">
      <button @click="showTodaySchedule" class="quick-action" title="Сегодняшнее расписание">
        <span class="quick-icon">📅</span>
        <span class="quick-text">Сегодня</span>
      </button>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted, onUnmounted } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';

  const route = useRoute();
  const router = useRouter();
  const authStore = useAuthStore();

  // Состояние
  const currentTime = ref(new Date().toLocaleTimeString('ru-RU', {
    hour: '2-digit',
    minute: '2-digit'
  }));
  const isOnline = ref(true);
  const workHours = ref('09:00 - 18:00');
  const todayAppointments = ref(0);
  const nextAppointmentTime = ref('--:--');

  // Вычисляемые свойства
  const userName = computed(() => authStore.userName || 'Врач');
  const getUserInitials = computed(() => {
    const name = userName.value;
    if (!name) return 'В';
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase();
  });

  const doctorSpecialty = computed(() => {
    // Здесь можно получать специализацию из store или API
    return 'Терапевт';
  });

  const hasActiveChildRoute = computed(() => {
    return route.matched.length > 1;
  });

  const breadcrumbs = computed(() => {
    const crumbs = [];
    const pathSegments = route.path.split('/').filter(segment => segment);

    const doctorIndex = pathSegments.indexOf('doctor');
    if (doctorIndex !== -1) {
      pathSegments.splice(doctorIndex, 1);
    }

    let currentPath = '/doctor';
    pathSegments.forEach((segment, index) => {
      currentPath += `/${segment}`;
      crumbs.push({
        path: currentPath,
        title: getBreadcrumbTitle(segment, index)
      });
    });

    return crumbs;
  });

  const pageTitle = computed(() => {
    const routeName = route.name || '';
    const titles = {
      'DoctorSchedule': 'Расписание приёмов',
      'Patients': 'Мои пациенты',
      'DoctorDashboard': 'Панель врача'
    };
    return titles[routeName] || 'Панель врача';
  });

  const pageSubtitle = computed(() => {
    const routeName = route.name || '';
    const subtitles = {
      'DoctorSchedule': 'Управление рабочим расписанием и приёмами',
      'Patients': 'Информация о пациентах и история их посещений'
    };
    return subtitles[routeName] || 'Добро пожаловать в рабочий кабинет';
  });

  const showPageActions = computed(() => {
    return ['DoctorSchedule', 'Patients'].includes(route.name);
  });

  // Методы
  const getBreadcrumbTitle = (segment, index) => {
    const titles = {
      'schedule': 'Расписание',
      'patients': 'Пациенты'
    };
    return titles[segment] || segment.charAt(0).toUpperCase() + segment.slice(1);
  };

  const logout = () => {
    authStore.logout();
    router.push('/login');
  };

  const updateTime = () => {
    currentTime.value = new Date().toLocaleTimeString('ru-RU', {
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const addSchedule = () => {
    console.log('Добавить расписание');
    // Реализация добавления расписания
  };

  const searchPatients = () => {
    console.log('Поиск пациентов');
    // Реализация поиска пациентов
  };

  const showTodaySchedule = () => {
    router.push({ name: 'DoctorSchedule', query: { date: 'today' } });
  };

  const showNextPatient = () => {
    console.log('Показать следующего пациента');
  };

  const showQuickNotes = () => {
    console.log('Открыть быстрые заметки');
  };

  // Загрузка данных врача
  const loadDoctorData = async () => {
    try {
      // Здесь можно загружать данные врача с API
      todayAppointments.value = 8; // Примерное количество
      const now = new Date();
      const nextHour = new Date(now.getTime() + 60 * 60 * 1000);
      nextAppointmentTime.value = nextHour.toLocaleTimeString('ru-RU', {
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch (error) {
      console.error('Ошибка загрузки данных врача:', error);
    }
  };

  // Хуки жизненного цикла
  onMounted(() => {
    // Обновляем время каждую минуту
    const timeInterval = setInterval(updateTime, 60000);

    // Загружаем данные врача
    loadDoctorData();

    // Следим за состоянием онлайн/оффлайн
    window.addEventListener('online', () => isOnline.value = true);
    window.addEventListener('offline', () => isOnline.value = false);

    // Сохраняем интервал для очистки
    window.doctorTimeInterval = timeInterval;
  });

  onUnmounted(() => {
    if (window.doctorTimeInterval) {
      clearInterval(window.doctorTimeInterval);
    }
  });
</script>

<style scoped>
  .doctor-dashboard {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    background: linear-gradient(135deg, #f0f4ff 0%, #e6f0ff 100%);
  }

  /* Контейнер */
  .container {
    max-width: 1400px;
    margin: 0 auto;
    padding: 0 20px;
  }

  /* Шапка */
  .doctor-header {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    position: sticky;
    top: 0;
    z-index: 1000;
  }

  .header-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 0;
  }

  .header-logo {
    display: flex;
    align-items: center;
    gap: 2rem;
  }

  .logo-link {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    text-decoration: none;
    color: white;
    font-weight: 700;
    font-size: 1.25rem;
    transition: opacity 0.3s;
  }

    .logo-link:hover {
      opacity: 0.9;
    }

  .logo-icon {
    position: relative;
    width: 5vw;
    height: 5vh;
    font-size: 2rem;
  }

  /* Бейдж пользователя */
  .user-badge {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.75rem 1.5rem;
    background: rgba(255, 255, 255, 0.15);
    backdrop-filter: blur(10px);
    border-radius: 12px;
    border: 1px solid rgba(255, 255, 255, 0.2);
  }

  .user-avatar {
    width: 48px;
    height: 48px;
    background: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    color: #667eea;
    font-size: 1.2rem;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }

  .user-info {
    display: flex;
    flex-direction: column;
  }

  .user-name {
    font-weight: 600;
    font-size: 1rem;
  }

  .user-role {
    font-size: 0.85rem;
    opacity: 0.9;
    margin-bottom: 0.25rem;
  }

  .doctor-specialty {
    font-size: 0.8rem;
    background: rgba(255, 255, 255, 0.2);
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    display: inline-block;
  }

  /* Навигация */
  .header-nav {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    background: rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(10px);
    padding: 0.5rem;
    border-radius: 12px;
    border: 1px solid rgba(255, 255, 255, 0.1);
  }

  .nav-link {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    text-decoration: none;
    color: rgba(255, 255, 255, 0.9);
    border-radius: 8px;
    transition: all 0.3s;
    font-weight: 500;
    white-space: nowrap;
  }

    .nav-link:hover {
      background: rgba(255, 255, 255, 0.15);
      color: white;
      transform: translateY(-1px);
    }

    .nav-link.active {
      background: white;
      color: #667eea;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }

  .nav-icon {
    font-size: 1.25rem;
  }

  .nav-separator {
    width: 1px;
    height: 24px;
    background: rgba(255, 255, 255, 0.3);
    margin: 0 0.25rem;
  }

  /* Статистика врача */
  .doctor-stats {
    display: flex;
    gap: 1.5rem;
    margin-left: 0.5rem;
  }

  .stat-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.5rem 1rem;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 8px;
  }

  .stat-icon {
    font-size: 1rem;
  }

  .stat-value {
    font-weight: 700;
    font-size: 1.1rem;
  }

  .stat-label {
    font-size: 0.8rem;
    opacity: 0.9;
  }

  /* Действия в шапке */
  .header-actions {
    display: flex;
    align-items: center;
    gap: 1.5rem;
  }

  .current-time {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.5rem 1rem;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 8px;
    font-weight: 600;
  }

  .time-icon {
    font-size: 1.1rem;
  }

  .btn-logout {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    background: rgba(255, 107, 107, 0.9);
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    backdrop-filter: blur(10px);
  }

    .btn-logout:hover {
      background: rgba(255, 82, 82, 0.9);
      transform: translateY(-2px);
      box-shadow: 0 4px 15px rgba(255, 107, 107, 0.3);
    }

  /* Основной контент */
  .doctor-main {
    flex: 1;
    padding: 2rem 0;
  }

  /* Хлебные крошки */
  .breadcrumbs {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin-bottom: 2rem;
    padding: 1rem 1.5rem;
    background: white;
    border-radius: 10px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  }

  .breadcrumb-item {
    text-decoration: none;
    color: #667eea;
    font-weight: 500;
    transition: color 0.3s;
  }

    .breadcrumb-item:hover {
      color: #764ba2;
      text-decoration: underline;
    }

    .breadcrumb-item.last {
      color: #4a5568;
      font-weight: 600;
    }

  .breadcrumb-separator {
    color: #a0aec0;
    margin: 0 0.25rem;
  }

  /* Заголовок страницы */
  .page-header {
    margin-bottom: 2.5rem;
    padding: 2rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    border-left: 5px solid #667eea;
  }

  .page-header-content {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 2rem;
  }

  .page-title {
    font-size: 2.25rem;
    color: #2c3e50;
    margin-bottom: 0.75rem;
    font-weight: 700;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }

  .page-subtitle {
    font-size: 1.1rem;
    color: #718096;
    max-width: 600px;
    line-height: 1.6;
  }

  .page-actions {
    display: flex;
    gap: 1rem;
    flex-shrink: 0;
  }

  /* Кнопки */
  .btn {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    text-decoration: none;
    font-size: 0.95rem;
  }

  .btn-primary {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
  }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(102, 126, 234, 0.3);
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

  .action-icon {
    font-size: 1.1rem;
  }

  /* Контейнер контента */
  .content-wrapper {
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    min-height: 500px;
    overflow: hidden;
    position: relative;
  }

  /* Заглушка для пустого контента */
  .empty-content {
    text-align: center;
    padding: 4rem 2rem;
  }

  .empty-icon {
    font-size: 4rem;
    margin-bottom: 2rem;
    opacity: 0.2;
  }

  .empty-content h2 {
    font-size: 2rem;
    color: #2c3e50;
    margin-bottom: 1rem;
  }

  .empty-content p {
    color: #718096;
    font-size: 1.1rem;
    max-width: 500px;
    margin: 0 auto 2rem;
    line-height: 1.6;
  }

  .empty-actions {
    display: flex;
    justify-content: center;
    gap: 1rem;
    flex-wrap: wrap;
  }

  /* Футер */
  .doctor-footer {
    background: #2c3e50;
    color: white;
    padding: 1.5rem 0;
    margin-top: auto;
  }

  .footer-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    gap: 1.5rem;
  }

  .footer-text {
    margin: 0;
    opacity: 0.8;
    font-size: 0.9rem;
  }

  .footer-status {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  .status-indicator {
    width: 10px;
    height: 10px;
    border-radius: 50%;
    background: #718096;
  }

    .status-indicator.active {
      background: #4CAF50;
      box-shadow: 0 0 10px rgba(76, 175, 80, 0.5);
    }

  .status-text {
    font-size: 0.9rem;
    opacity: 0.9;
  }

  /* Быстрые действия */
  .quick-actions {
    position: fixed;
    bottom: 2rem;
    right: 2rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    z-index: 100;
  }

  .quick-action {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    padding: 1rem 1.5rem;
    background: white;
    border: none;
    border-radius: 50px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
    cursor: pointer;
    transition: all 0.3s;
    font-weight: 600;
    color: #2c3e50;
  }

    .quick-action:hover {
      transform: translateY(-3px);
      box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
      background: #667eea;
      color: white;
    }

  .quick-icon {
    font-size: 1.25rem;
  }

  .quick-text {
    white-space: nowrap;
  }

  .img-icon{
    width:25vw;
    height:25vh;
  }

  /* Адаптивность */
  @media (max-width: 1200px) {
    .header-content {
      flex-wrap: wrap;
      gap: 1rem;
    }

    .header-nav {
      order: 3;
      width: 100%;
      margin-top: 1rem;
      justify-content: center;
    }
  }

  @media (max-width: 768px) {
    .page-header-content {
      flex-direction: column;
      align-items: stretch;
    }

    .page-actions {
      flex-wrap: wrap;
    }

    .doctor-stats {
      display: none;
    }

    .header-logo {
      flex-direction: column;
      gap: 1rem;
      text-align: center;
    }

    .user-badge {
      padding: 0.5rem 1rem;
    }

    .nav-link {
      padding: 0.5rem 1rem;
    }

    .quick-actions {
      bottom: 1rem;
      right: 1rem;
    }

    .quick-action {
      padding: 0.75rem;
    }

    .quick-text {
      display: none;
    }
  }

  @media (max-width: 480px) {
    .header-content {
      flex-direction: column;
      align-items: stretch;
    }

    .header-actions {
      justify-content: space-between;
      width: 100%;
    }

    .nav-link {
      flex: 1;
      justify-content: center;
    }
  }
</style>
