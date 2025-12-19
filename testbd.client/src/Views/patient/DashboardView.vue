<template>
  <div class="patient-dashboard">
    <!-- Шапка панели пациента -->
    <header class="patient-header">
      <div class="container">
        <div class="header-content">
          <div class="header-logo">
            <router-link to="/" class="logo-link">
              <img src="../../assets/icons/health-medical-healthcare-heart.svg" class="logo-icon" />
              <span class="logo-text">Клиника "Здоровье"</span>
            </router-link>
            <div class="user-badge">
              <div class="user-info">
                <div class="user-role">Пациент</div>
              </div>
            </div>
          </div>

          <nav class="header-nav">
            <router-link to="/" class="nav-link">
              <span class="nav-icon"></span>
              <span class="nav-text">На главную</span>
            </router-link>

            <div class="nav-separator"></div>

            <router-link to="/patient/appointments">
              <span class="nav-icon"></span>
              <span class="nav-text">Мои записи</span>
            </router-link>

            <router-link to="/patient/history">
              <span class="nav-icon"></span>
              <span class="nav-text">История посещений</span>
            </router-link>

            <router-link to="/patient/profile">
              <span class="nav-text">Профиль</span>
            </router-link>
          </nav>

          <div class="header-actions">
            <button @click="logout" class="btn-logout">
              <span class="logout-icon"></span>
              <span class="logout-text">Выйти</span>
            </button>
          </div>
        </div>
      </div>
    </header>

    <main class="patient-main">
      <div class="container">
        <div class="page-header">
          <h1 class="page-title">{{ pageTitle }}</h1>
          <p class="page-subtitle" v-if="pageSubtitle">{{ pageSubtitle }}</p>
        </div>
        <div class="content-wrapper">
          <router-view></router-view>
        </div>
      </div>
    </main>

    <!-- Футер -->
    <footer class="patient-footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-info">
            <p class="footer-text">© 2025 Клиника "Здоровье". Панель пациента v1.0</p>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

  const route = useRoute();

  const pageTitle = computed(() => {
    const routeName = route.name || '';
    const titles = {
      'patient-appointments': 'Мои записи',
      'patient-history': 'История посещений',
      'patient-profile': 'Профиль пациента',
      'patient': 'Панель пациента'
    };
    return titles[routeName] || 'Панель пациента';
  });

  const pageSubtitle = computed(() => {
    const routeName = route.name || '';
    const subtitles = {
      'patient-appointments': 'Управление вашими записями к врачам',
      'patient-history': 'Архив ваших посещений и результатов',
      'patient-profile': 'Личная информация и настройки'
    };
    return subtitles[routeName] || 'Добро пожаловать в личный кабинет';
  });

const router = useRouter();
const authStore = useAuthStore();

const logout = () => {
  authStore.logout();
};
</script>

<style scoped>
  .patient-dashboard {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  }

  /* Шапка */
  .patient-header {
    background: white;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
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
    color: #2c3e50;
    font-weight: 700;
    font-size: 1.25rem;
    transition: opacity 0.3s;
  }

    .logo-link:hover {
      opacity: 0.8;
    }

  .logo-icon {
    position: relative;
    width: 10vw;
    height: 10vh;
    font-size: 2rem;
  }

  .user-badge {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    padding: 0.5rem 1rem;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 20%);
    border-radius: 50px;
    color: white;
  }

  .user-avatar {
    width: 40px;
    height: 40px;
    background: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    color: #667eea;
    font-size: 1.1rem;
  }

  .user-info {
    display: flex;
    flex-direction: column;
  }

  .user-name {
    font-weight: 600;
    font-size: 0.95rem;
  }

  .user-role {
    font-size: 0.85rem;
    opacity: 0.9;
  }

  /* Навигация */
  .header-nav {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    background: #f8f9fa;
    padding: 0.5rem;
    border-radius: 12px;
  }

  .nav-link {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1.25rem;
    text-decoration: none;
    color: #666;
    border-radius: 8px;
    transition: all 0.3s;
    font-weight: 500;
  }

    .nav-link:hover {
      background: rgba(102, 126, 234, 0.1);
      color: #667eea;
    }

    .nav-link.active {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

  .nav-icon {
    font-size: 1.25rem;
  }

  .nav-separator {
    width: 1px;
    height: 24px;
    background: #e0e0e0;
    margin: 0 0.5rem;
  }

  /* Кнопки */
  .btn-logout {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    background: #ff6b6b;
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
  }

    .btn-logout:hover {
      background: #ff5252;
      transform: translateY(-1px);
      box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3);
    }

  .logout-icon {
    font-size: 1.1rem;
  }

  /* Основной контент */
  .patient-main {
    flex: 1;
    padding: 2rem 0;
  }

  /* Хлебные крошки */
  .breadcrumbs {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin-bottom: 2rem;
    padding: 1rem;
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
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
      color: #666;
      font-weight: 600;
    }

      .breadcrumb-item.last:hover {
        color: #666;
        text-decoration: none;
        cursor: default;
      }

  .breadcrumb-separator {
    color: #999;
    margin: 0 0.25rem;
  }

  /* Заголовок страницы */
  .page-header {
    margin-bottom: 2.5rem;
    padding: 2rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    border-left: 4px solid #667eea;
  }

  .page-title {
    font-size: 2.5rem;
    color: #2c3e50;
    margin-bottom: 0.5rem;
    font-weight: 700;
  }

  .page-subtitle {
    font-size: 1.1rem;
    color: #666;
    opacity: 0.9;
  }

  /* Контейнер контента */
  .content-wrapper {
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    min-height: 400px;
    overflow: hidden;
  }

  /* Футер */
  .patient-footer {
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

  .footer-links {
    display: flex;
    gap: 1.5rem;
  }

  .footer-link {
    color: #4CAF50;
    text-decoration: none;
    font-size: 0.9rem;
    transition: color 0.3s;
  }

    .footer-link:hover {
      color: #45a049;
      text-decoration: underline;
    }

  /* Адаптивность */
  @media (max-width: 1024px) {
    .header-content {
      flex-direction: column;
      gap: 1rem;
    }

    .header-nav {
      width: 100%;
      justify-content: center;
      flex-wrap: wrap;
    }

    .page-title {
      font-size: 2rem;
    }
  }

  @media (max-width: 768px) {
    .header-logo {
      flex-direction: column;
      gap: 1rem;
      text-align: center;
    }

    .header-nav {
      flex-direction: column;
      width: 100%;
    }

    .nav-link {
      width: 100%;
      justify-content: center;
    }

    .nav-separator {
      width: 100%;
      height: 1px;
      margin: 0.5rem 0;
    }

    .breadcrumbs {
      flex-wrap: wrap;
    }

    .footer-content {
      flex-direction: column;
      text-align: center;
    }
  }
</style>
