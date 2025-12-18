<template>
  <div class="registration-container">
    <!-- Шаг 1: Создание учетной записи -->
    <div v-if="currentStep === 1" class="step">
      <h2>Создание учетной записи</h2>
      <form @submit.prevent="nextStep">
        <div class="form-group">
          <label for="login">Логин *</label>
          <input id="login"
                 type="text"
                 v-model="formData.login"
                 :class="{ 'error-input': errors.login }"
                 required />
          <div v-if="errors.login" class="error">{{ errors.login }}</div>
        </div>

        <div class="form-group">
          <label for="password">Пароль *</label>
          <input id="password"
                 type="password"
                 v-model="formData.password"
                 :class="{ 'error-input': errors.password }"
                 required />
          <div v-if="errors.password" class="error">{{ errors.password }}</div>
        </div>

        <div class="form-group">
          <label for="confirmPassword">Подтвердите пароль *</label>
          <input id="confirmPassword"
                 type="password"
                 v-model="formData.confirmPassword"
                 :class="{ 'error-input': errors.confirmPassword }"
                 required />
          <div v-if="errors.confirmPassword" class="error">{{ errors.confirmPassword }}</div>
        </div>

        <button type="submit" :disabled="!isStep1Valid">Далее</button>
      </form>
    </div>

    <!-- Шаг 2: Выбор типа пользователя -->
    <div v-if="currentStep === 2" class="step">
      <h2>Выберите тип пользователя</h2>
      <div class="role-selector">
        <div class="role-card"
             :class="{ 'selected': selectedUserType === 'patient' }"
             @click="selectUserType('patient')">
          <div class="role-icon">👤</div>
          <h3>Пациент</h3>
          <p>Запись на прием, просмотр истории, получение услуг</p>
        </div>

        <div class="role-card"
             :class="{ 'selected': selectedUserType === 'doctor' }"
             @click="selectUserType('doctor')">
          <div class="role-icon">👨‍⚕️</div>
          <h3>Врач</h3>
          <p>Ведение приема, управление расписанием, оказание услуг</p>
        </div>

        <div class="role-card"
             :class="{ 'selected': selectedUserType === 'admin' }"
             @click="selectUserType('admin')">
          <div class="role-icon">👔</div>
          <h3>Администратор</h3>
          <p>Управление системой, пользователями, настройками</p>
        </div>
      </div>

      <div class="step-buttons">
        <button class="secondary" @click="prevStep">Назад</button>
        <button @click="nextStep" :disabled="!selectedUserType">Далее</button>
      </div>
    </div>

    <!-- Шаг 3: Форма для пациента -->
    <div v-if="currentStep === 3 && selectedUserType === 'patient'" class="step">
      <h2>Данные пациента</h2>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="patientFio">ФИО *</label>
          <input id="patientFio"
                 type="text"
                 v-model="formData.patientFio"
                 required />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="patientBirthdate">Дата рождения *</label>
            <input id="patientBirthdate"
                   type="date"
                   v-model="formData.patientBirthdate"
                   required />
          </div>

          <div class="form-group">
            <label for="gender">Пол *</label>
            <select id="gender" v-model="formData.gender" required>
              <option value="">Выберите пол</option>
              <option value="М">Мужской</option>
              <option value="Ж">Женский</option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label for="contactPhone">Контактный телефон *</label>
          <input id="contactPhone"
                 type="tel"
                 v-model="formData.contactPhone"
                 placeholder="+7 (XXX) XXX-XX-XX"
                 required />
        </div>

        <div class="step-buttons">
          <button class="secondary" @click="prevStep">Назад</button>
          <button type="submit" :disabled="!isPatientFormValid">Зарегистрироваться</button>
        </div>
      </form>
    </div>

    <!-- Шаг 3: Форма для врача -->
    <div v-if="currentStep === 3 && selectedUserType === 'doctor'" class="step">
      <h2>Данные врача</h2>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="employeeFio">ФИО *</label>
          <input id="employeeFio"
                 type="text"
                 v-model="formData.employeeFio"
                 required />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="post">Должность *</label>
            <input id="post"
                   type="text"
                   v-model="formData.post"
                   required />
          </div>

          <div class="form-group">
            <label for="specialization">Специализация *</label>
            <input id="specialization"
                   type="text"
                   v-model="formData.specialization"
                   required />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="experience">Стаж (лет) *</label>
            <input id="experience"
                   type="number"
                   v-model="formData.experience"
                   min="0"
                   required />
          </div>

          <div class="form-group">
            <label for="phone">Телефон</label>
            <input id="phone"
                   type="tel"
                   v-model="formData.phone" />
          </div>
        </div>

        <div class="form-group">
          <label for="email">Email</label>
          <input id="email"
                 type="email"
                 v-model="formData.email" />
        </div>

        <div class="step-buttons">
          <button class="secondary" @click="prevStep">Назад</button>
          <button type="submit" :disabled="!isDoctorFormValid">Зарегистрироваться</button>
        </div>
      </form>
    </div>

    <!-- Шаг 3: Форма для администратора (только пароль) -->
    <div v-if="currentStep === 3 && selectedUserType === 'admin'" class="step">
      <h2>Подтверждение прав администратора</h2>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="adminPassword">Пароль администратора *</label>
          <input id="adminPassword"
                 type="password"
                 v-model="formData.adminPassword"
                 :class="{ 'error-input': errors.adminPassword }"
                 placeholder="Введите специальный пароль"
                 required />
          <div v-if="errors.adminPassword" class="error">{{ errors.adminPassword }}</div>
          <div class="hint">Для тестирования используйте пароль: <strong>admin123</strong></div>
        </div>

        <div class="step-buttons">
          <button class="secondary" @click="prevStep">Назад</button>
          <button type="submit" :disabled="!isAdminFormValid">Зарегистрироваться</button>
        </div>
      </form>
    </div>

    <!-- Шаг 4: Успешная регистрация -->
    <div v-if="currentStep === 4" class="step success-step">
      <div class="success-icon">✓</div>
      <h2>Регистрация успешна!</h2>
      <p>Ваша учетная запись создана</p>
      <p>Тип: <strong>{{ getUserTypeDisplayName }}</strong></p>
      <p>Роль в системе: <strong>{{ getRoleDisplayName }}</strong></p>
      <button @click="goToLogin">Войти в систему</button>
    </div>
  </div>
</template>

<script setup>
  import { ref, reactive, computed } from 'vue';
  import { useRouter } from 'vue-router';
  import axios from 'axios';

  const API_BASE_URL = '';
  const router = useRouter();

  // Заглушка пароля администратора
  const ADMIN_PASSWORD = 'admin123';

  // Состояние формы
  const currentStep = ref(1);
  const selectedUserType = ref(''); // 'patient', 'doctor' или 'admin'

  // Данные формы
  const formData = reactive({
    login: '',
    password: '',
    confirmPassword: '',
    role: 'user', // По умолчанию 'user'

    // Данные пациента
    patientFio: '',
    patientBirthdate: '',
    gender: '',
    contactPhone: '',

    // Данные врача
    employeeFio: '',
    post: '',
    specialization: '',
    experience: null,
    phone: '',
    email: '',

    // Данные администратора
    adminPassword: ''
  });

  // Ошибки валидации
  const errors = reactive({
    login: '',
    password: '',
    confirmPassword: '',
    adminPassword: ''
  });

  // Метки шагов
  const stepLabels = {
    patient: ['Аккаунт', 'Тип', 'Данные', 'Готово'],
    doctor: ['Аккаунт', 'Тип', 'Данные', 'Готово'],
    admin: ['Аккаунт', 'Тип', 'Пароль', 'Готово']
  };

  // Валидация шага 1
  const isStep1Valid = computed(() => {
    return formData.login.trim() !== '' &&
      formData.password.trim() !== '' &&
      formData.password === formData.confirmPassword &&
      formData.password.length >= 6;
  });

  // Валидация формы пациента
  const isPatientFormValid = computed(() => {
    return formData.patientFio.trim() !== '' &&
      formData.patientBirthdate !== '' &&
      formData.gender !== '' &&
      formData.contactPhone.trim() !== '';
  });

  // Валидация формы врача
  const isDoctorFormValid = computed(() => {
    return formData.employeeFio.trim() !== '' &&
      formData.post.trim() !== '' &&
      formData.specialization.trim() !== '' &&
      formData.experience !== null;
  });

  // Валидация формы администратора
  const isAdminFormValid = computed(() => {
    return formData.adminPassword.trim() !== '';
  });

  // Отображение типа пользователя
  const getUserTypeDisplayName = computed(() => {
    switch (selectedUserType.value) {
      case 'patient': return 'Пациент';
      case 'doctor': return 'Врач';
      case 'admin': return 'Администратор';
      default: return '';
    }
  });

  // Отображение роли
  const getRoleDisplayName = computed(() => {
    switch (formData.role) {
      case 'user': return 'User';
      case 'employee': return 'Employee';
      case 'admin': return 'Admin';
      default: return 'User';
    }
  });

  // Получение метки шага
  const getStepLabel = (stepNum) => {
    const labels = stepLabels[selectedUserType.value] || stepLabels.patient;
    return labels[stepNum - 1];
  };

  // Методы навигации
  const nextStep = () => {
    if (currentStep.value === 1) {
      validateStep1();
      if (!isStep1Valid.value) return;
    } else if (currentStep.value === 2) {
      if (!selectedUserType.value) return;
    }

    currentStep.value++;
  };

  const prevStep = () => {
    if (currentStep.value > 1) {
      currentStep.value--;
    }
  };

  // Валидация шага 1
  const validateStep1 = () => {
    errors.login = '';
    errors.password = '';
    errors.confirmPassword = '';

    if (!formData.login.trim()) {
      errors.login = 'Логин обязателен';
    }

    if (!formData.password.trim()) {
      errors.password = 'Пароль обязателен';
    } else if (formData.password.length < 6) {
      errors.password = 'Пароль должен содержать минимум 6 символов';
    }

    if (formData.password !== formData.confirmPassword) {
      errors.confirmPassword = 'Пароли не совпадают';
    }
  };

  // Выбор типа пользователя
  const selectUserType = (userType) => {
    selectedUserType.value = userType;

    // Устанавливаем роль в зависимости от типа
    switch (userType) {
      case 'patient':
        formData.role = 'user';
        break;
      case 'doctor':
        formData.role = 'employee';
        break;
      case 'admin':
        formData.role = 'admin';
        break;
    }
  };

  // Проверка пароля администратора (заглушка)
  const validateAdminPassword = () => {
    errors.adminPassword = '';

    if (!formData.adminPassword.trim()) {
      errors.adminPassword = 'Пароль администратора обязателен';
      return false;
    }

    if (formData.adminPassword !== ADMIN_PASSWORD) {
      errors.adminPassword = 'Неверный пароль администратора';
      return false;
    }

    return true;
  };

  // Отправка формы
  const submitForm = async () => {
    try {
      // Для администратора проверяем пароль
      if (selectedUserType.value === 'admin') {
        if (!validateAdminPassword()) {
          return;
        }
      }

      // Подготовка данных для отправки
      const requestData = {
        login: formData.login,
        password: formData.password,
        role: formData.role
      };

      // Добавляем данные в зависимости от типа пользователя
      if (selectedUserType.value === 'patient') {
        // Для пациента
        Object.assign(requestData, {
          patientFio: formData.patientFio,
          patientBirthdate: formData.patientBirthdate,
          gender: formData.gender,
          contactPhone: formData.contactPhone
        });
      } else if (selectedUserType.value === 'doctor') {
        // Для врача
        Object.assign(requestData, {
          employeeFio: formData.employeeFio,
          post: formData.post,
          specialization: formData.specialization,
          experience: formData.experience,
          phone: formData.phone,
          email: formData.email
        });
      }
      // Для администратора дополнительные данные не нужны

      console.log('Отправка данных:', requestData);

      // Отправка на сервер
      const response = await axios.post(`${API_BASE_URL}/Auth/register`, requestData);

      console.log('Ответ сервера:', response.data);

      // Переход к шагу успеха
      currentStep.value = 4;

    } catch (error) {
      console.error('Ошибка регистрации:', error);

      // Обработка ошибок сервера
      if (error.response) {
        const serverError = error.response.data;
        alert(`Ошибка регистрации: ${serverError.message || serverError.error || 'Неизвестная ошибка'}`);
      } else {
        alert('Ошибка сети или сервера');
      }
    }
  };

  // Переход к логину
  const goToLogin = () => {
    router.push('/login');
  };
</script>

<style scoped>
  /* Существующие стили остаются без изменений, добавляем только новые */
  .registration-container {
    max-width: 800px;
    margin: 0 auto;
    padding: 20px;
  }

  .step {
    background: white;
    border-radius: 10px;
    padding: 30px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    margin-bottom: 30px;
  }

  .form-group {
    margin-bottom: 20px;
  }

  .form-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 20px;
  }

  label {
    display: block;
    margin-bottom: 8px;
    font-weight: 600;
    color: #333;
  }

  input, select {
    width: 100%;
    padding: 10px;
    border: 1px solid #ddd;
    border-radius: 5px;
    font-size: 16px;
    box-sizing: border-box;
  }

    input.error-input {
      border-color: #ff4444;
    }

  .error {
    color: #ff4444;
    font-size: 14px;
    margin-top: 5px;
  }

  .hint {
    font-size: 12px;
    color: #666;
    margin-top: 5px;
  }

  button {
    background: #4CAF50;
    color: white;
    border: none;
    padding: 12px 24px;
    border-radius: 5px;
    font-size: 16px;
    cursor: pointer;
    transition: background 0.3s;
  }

    button:hover:not(:disabled) {
      background: #45a049;
    }

    button:disabled {
      background: #cccccc;
      cursor: not-allowed;
    }

    button.secondary {
      background: #6c757d;
    }

      button.secondary:hover {
        background: #5a6268;
      }

  .role-selector {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 20px;
    margin: 30px 0;
  }

  .role-card {
    border: 2px solid #e0e0e0;
    border-radius: 10px;
    padding: 25px;
    text-align: center;
    cursor: pointer;
    transition: all 0.3s;
  }

    .role-card:hover {
      border-color: #4CAF50;
      transform: translateY(-2px);
    }

    .role-card.selected {
      border-color: #4CAF50;
      background: #f8fff8;
    }

  .role-icon {
    font-size: 48px;
    margin-bottom: 15px;
  }

  .role-card h3 {
    margin: 10px 0;
    color: #333;
  }

  .role-card p {
    color: #666;
    font-size: 14px;
    line-height: 1.4;
  }

  .step-buttons {
    display: flex;
    justify-content: space-between;
    margin-top: 30px;
  }

  .success-step {
    text-align: center;
    padding: 50px 30px;
  }

  .success-icon {
    font-size: 80px;
    color: #4CAF50;
    margin-bottom: 20px;
  }

  .success-step h2 {
    color: #333;
    margin-bottom: 10px;
  }

  .success-step p {
    color: #666;
    margin-bottom: 5px;
  }

  /* Прогресс-бар */
  .progress-bar {
    display: flex;
    justify-content: space-between;
    position: relative;
    margin: 40px 0;
  }

    .progress-bar::before {
      content: '';
      position: absolute;
      top: 20px;
      left: 0;
      right: 0;
      height: 2px;
      background: #e0e0e0;
      z-index: 1;
    }

  .progress-step {
    display: flex;
    flex-direction: column;
    align-items: center;
    position: relative;
    z-index: 2;
  }

  .step-number {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: #e0e0e0;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: bold;
    color: #666;
    margin-bottom: 10px;
    transition: all 0.3s;
  }

  .progress-step.active .step-number {
    background: #4CAF50;
    color: white;
  }

  .progress-step.completed .step-number {
    background: #2e7d32;
    color: white;
  }

  .step-label {
    font-size: 14px;
    color: #666;
    text-align: center;
  }

  @media (max-width: 768px) {
    .form-row {
      grid-template-columns: 1fr;
    }

    .role-selector {
      grid-template-columns: 1fr;
    }

    .step {
      padding: 20px;
    }
  }
</style>
