<!-- views/patient/ProfileView.vue -->
<template>
  <div class="profile-container">
    <div class="profile-header">
      <h2>Мой профиль</h2>
      <button @click="editMode = !editMode" class="edit-btn" :disabled="saving">
        {{ editMode ? 'Отмена' : 'Редактировать' }}
      </button>
    </div>

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Загрузка профиля...</p>
    </div>

    <div v-else-if="error" class="error-message">
      <p>❌ {{ error }}</p>
      <button @click="loadProfile" class="retry-btn">Повторить</button>
    </div>

    <div v-else class="profile-form-container">
      <form @submit.prevent="saveProfile" class="profile-form">
        <!-- Основная информация -->
        <div class="section">
          <h3>Основная информация</h3>
          <div class="form-grid">
            <div class="form-group">
              <label>ФИО *</label>
              <input v-model="profile.patientFio"
                     :readonly="!editMode"
                     :class="{ 'readonly': !editMode }"
                     required
                     :disabled="saving" />
            </div>

            <div class="form-group">
              <label>Дата рождения</label>
              <input type="date"
                     v-model="profile.patientBirthdate"
                     :readonly="!editMode"
                     :class="{ 'readonly': !editMode }"
                     :disabled="saving" />
            </div>

            <div class="form-group">
              <label>Пол</label>
              <select v-model="profile.gender"
                      :disabled="!editMode || saving"
                      :class="{ 'readonly': !editMode }">
                <option value="М">Мужской</option>
                <option value="Ж">Женский</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Контактная информация -->
        <div class="section">
          <h3>Контактная информация</h3>
          <div class="form-grid">
            <div class="form-group">
              <label>Телефон *</label>
              <input v-model="profile.contactPhone"
                     :readonly="!editMode"
                     :class="{ 'readonly': !editMode }"
                     type="tel"
                     required
                     :disabled="saving" />
            </div>
          </div>
        </div>

        <!-- Документы -->
        <div class="section">
          <h3>Документы</h3>
          <div class="form-grid">
            <div class="form-group">
              <label>Номер полиса ОМС</label>
              <input v-model="profile.omsPolisNumber"
                     :readonly="!editMode"
                     :class="{ 'readonly': !editMode }"
                     type="number"
                     :disabled="saving"
                     placeholder="Введите номер полиса" />
            </div>
          </div>
        </div>

        <!-- Информация о регистрации -->
        <div class="section readonly-info">
          <h3>Информация о регистрации</h3>
          <div class="form-grid">
            <div class="form-group">
              <label>Дата регистрации</label>
              <input :value="formatDate(profile.registrationDate)"
                     readonly
                     class="readonly" />
            </div>

            <div class="form-group">
              <label>Статус</label>
              <input :value="profile.isActive ? 'Активен' : 'Неактивен'"
                     readonly
                     class="readonly" />
            </div>
          </div>
        </div>

        <div v-if="editMode" class="form-actions">
          <button type="submit" class="save-btn" :disabled="saving">
            <span v-if="saving">
              <span class="spinner-small"></span> Сохранение...
            </span>
            <span v-else>Сохранить изменения</span>
          </button>
          <button type="button" @click="cancelEdit" class="cancel-btn" :disabled="saving">
            Отмена
          </button>
        </div>
      </form>

      <!-- Информация о профиле -->
      <div class="profile-info">
        <div class="info-card">
          <div class="info-icon">ℹ️</div>
          <div class="info-content">
            <h4>Важная информация</h4>
            <ul>
              <li>Обновляйте свои контактные данные, чтобы мы могли связаться с вами</li>
              <li>Убедитесь, что все поля заполнены корректно</li>
              <li>Поля отмеченные * обязательны для заполнения</li>
              <li>Для изменения номера полиса ОМС обратитесь в регистратуру</li>
            </ul>
          </div>
        </div>

        <div class="info-card">
          <div class="info-icon">📞</div>
          <div class="info-content">
            <h4>Контактная информация</h4>
            <p>Если у вас возникли проблемы с обновлением профиля, свяжитесь с нами:</p>
            <p class="contact-info">
              📞 Регистратура: +7 (978) 123-45-67<br>
              📧 Email: support@clinic.ru
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Сообщение об успешном сохранении -->
    <div v-if="successMessage" class="success-message" @click="successMessage = ''">
      ✅ {{ successMessage }}
    </div>

    <!-- Валидационные ошибки -->
    <div v-if="validationErrors.length > 0" class="validation-errors">
      <h4>Исправьте следующие ошибки:</h4>
      <ul>
        <li v-for="error in validationErrors" :key="error">{{ error }}</li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { getPatientProfile, updatePatientProfile } from '@/services/api';

const authStore = useAuthStore();
const profile = ref({
  patientFio: '',
  patientBirthdate: '',
  gender: 'M',
  contactPhone: '',
  omsPolisNumber: null,
  registrationDate: '',
  isActive: true,
  patientId: null
});
const originalProfile = ref({});
const editMode = ref(false);
const loading = ref(true);
const saving = ref(false);
const error = ref(null);
const successMessage = ref('');
const validationErrors = ref([]);

const loadProfile = async () => {
  try {
    loading.value = true;
    error.value = null;
    validationErrors.value = [];

    const patientId = authStore.user?.patientId;
    if (!patientId) {
      throw new Error('ID пациента не найден');
    }

    const response = await getPatientProfile(patientId);
    profile.value = {
      ...response,
      // Преобразуем DateOnly в формат для input[type="date"]
      patientBirthdate: formatDateForInput(response.patientBirthdate),
      registrationDate: response.registrationDate
    };
    originalProfile.value = { ...profile.value };
  } catch (err) {
    console.error('Ошибка загрузки профиля:', err);
    error.value = err.response?.data?.message || err.message || 'Не удалось загрузить профиль';
  } finally {
    loading.value = false;
  }
};

const validateProfile = () => {
  validationErrors.value = [];

  if (!profile.value.patientFio?.trim()) {
    validationErrors.value.push('ФИО обязательно для заполнения');
  }

  if (!profile.value.contactPhone?.trim()) {
    validationErrors.value.push('Телефон обязателен для заполнения');
  }

  // Проверка формата телефона (простая)
  const phoneRegex = /^[\d\s\-\+\(\)]{10,20}$/;
  if (profile.value.contactPhone && !phoneRegex.test(profile.value.contactPhone.replace(/\s/g, ''))) {
    validationErrors.value.push('Введите корректный номер телефона');
  }

  // Проверка даты рождения
  if (profile.value.patientBirthdate) {
    const birthDate = new Date(profile.value.patientBirthdate);
    const today = new Date();
    if (birthDate > today) {
      validationErrors.value.push('Дата рождения не может быть в будущем');
    }
  }

  return validationErrors.value.length === 0;
};

const saveProfile = async () => {
  if (!validateProfile()) {
    return;
  }

  try {
    saving.value = true;
    error.value = null;
    successMessage.value = '';

    // Преобразуем данные для отправки
    const dataToSend = {
      ...profile.value,
      patientId: authStore.user?.patientId,
      // Преобразуем дату обратно в формат для бэкенда
      patientBirthdate: profile.value.patientBirthdate ?
        profile.value.patientBirthdate.split('-').join('-') : null
    };

    await updatePatientProfile(dataToSend);

    successMessage.value = 'Профиль успешно обновлен';
    originalProfile.value = { ...profile.value };
    editMode.value = false;

    // Обновляем данные в authStore если нужно
    if (authStore.user) {
      authStore.user.patientFio = profile.value.patientFio;
    }

    // Скрываем сообщение через 3 секунды
    setTimeout(() => {
      successMessage.value = '';
    }, 3000);
  } catch (err) {
    console.error('Ошибка сохранения профиля:', err);
    error.value = err.response?.data?.message || err.message || 'Не удалось сохранить изменения';

    // Прокрутка к ошибке
    window.scrollTo({ top: 0, behavior: 'smooth' });
  } finally {
    saving.value = false;
  }
};

const cancelEdit = () => {
  profile.value = { ...originalProfile.value };
  editMode.value = false;
  successMessage.value = '';
  error.value = null;
  validationErrors.value = [];
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

const formatDateForInput = (dateString) => {
  if (!dateString) return '';
  // Преобразуем DateOnly в YYYY-MM-DD
  const date = new Date(dateString);
  return date.toISOString().split('T')[0];
};

const calculateAge = () => {
  if (!profile.value.patientBirthdate) return '-';
  const birthDate = new Date(profile.value.patientBirthdate);
  const today = new Date();
  let age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();

  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--;
  }

  return age;
};

watch(editMode, (newValue) => {
  if (!newValue) {
    validationErrors.value = [];
  }
});

onMounted(() => {
  loadProfile();
});
</script>

<style scoped>
  .profile-container {
    padding: 20px;
    max-width: 1000px;
    margin: 0 auto;
  }

  .profile-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 30px;
    padding-bottom: 15px;
    border-bottom: 2px solid #eaeaea;
  }

    .profile-header h2 {
      color: #2c3e50;
      font-size: 28px;
      font-weight: 600;
      margin: 0;
    }

  .edit-btn {
    padding: 10px 24px;
    background-color: #3498db;
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 500;
    transition: all 0.3s ease;
    box-shadow: 0 2px 4px rgba(52, 152, 219, 0.2);
  }

    .edit-btn:hover:not(:disabled) {
      background-color: #2980b9;
      transform: translateY(-1px);
      box-shadow: 0 4px 8px rgba(52, 152, 219, 0.3);
    }

    .edit-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
      box-shadow: none;
    }

  /* Loading */
  .loading {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 80px 20px;
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

  .spinner-small {
    display: inline-block;
    width: 16px;
    height: 16px;
    border: 2px solid #ffffff;
    border-top: 2px solid transparent;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin-right: 8px;
    vertical-align: middle;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }

  .loading p {
    color: #7f8c8d;
    font-size: 16px;
    margin-top: 10px;
  }

  /* Error */
  .error-message {
    text-align: center;
    padding: 40px 20px;
    background: linear-gradient(135deg, #ffeaea 0%, #ffd6d6 100%);
    border-radius: 12px;
    margin: 20px 0;
    border-left: 4px solid #e74c3c;
  }

    .error-message p {
      color: #c0392b;
      font-size: 16px;
      margin-bottom: 20px;
      font-weight: 500;
    }

  .retry-btn {
    padding: 10px 24px;
    background-color: #e74c3c;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 500;
    transition: background-color 0.3s;
  }

    .retry-btn:hover {
      background-color: #c0392b;
    }

  /* Profile Form Container */
  .profile-form-container {
    display: flex;
    gap: 30px;
    align-items: flex-start;
  }

  .profile-form {
    flex: 2;
    min-width: 0; /* Для корректного сжатия */
  }

  .profile-info {
    flex: 1;
    min-width: 300px;
  }

  /* Sections */
  .section {
    background: white;
    border-radius: 12px;
    padding: 24px;
    margin-bottom: 20px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.08);
    border: 1px solid #f0f0f0;
    transition: box-shadow 0.3s ease;
  }

    .section:hover {
      box-shadow: 0 4px 12px rgba(0,0,0,0.12);
    }

    .section.readonly-info {
      background-color: #f8f9fa;
      border-color: #eaeaea;
    }

    .section h3 {
      color: #2c3e50;
      margin: 0 0 20px 0;
      font-size: 18px;
      font-weight: 600;
      padding-bottom: 12px;
      border-bottom: 2px solid #f0f0f0;
    }

    .section.readonly-info h3 {
      color: #7f8c8d;
      border-bottom-color: #eaeaea;
    }

  /* Form Grid */
  .form-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 20px;
  }

  .form-group {
    display: flex;
    flex-direction: column;
  }

    .form-group label {
      margin-bottom: 8px;
      color: #555;
      font-weight: 500;
      font-size: 14px;
      display: flex;
      align-items: center;
      gap: 4px;
    }

      .form-group label::after {
        content: '*';
        color: #e74c3c;
        opacity: 0;
        transition: opacity 0.3s;
      }

      .form-group label.required::after {
        opacity: 1;
      }

    .form-group input,
    .form-group select {
      padding: 12px 16px;
      border: 2px solid #e0e0e0;
      border-radius: 8px;
      font-size: 14px;
      transition: all 0.3s ease;
      background-color: white;
    }

      .form-group input:focus,
      .form-group select:focus {
        outline: none;
        border-color: #3498db;
        box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
      }

      .form-group input.readonly,
      .form-group select.readonly {
        background-color: #f8f9fa;
        cursor: not-allowed;
        color: #666;
        border-color: #eaeaea;
      }

      .form-group input:disabled,
      .form-group select:disabled {
        background-color: #f5f5f5;
        cursor: not-allowed;
        opacity: 0.7;
      }

  /* Form Actions */
  .form-actions {
    display: flex;
    gap: 15px;
    justify-content: flex-end;
    margin-top: 30px;
    padding: 24px;
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.08);
    border: 1px solid #f0f0f0;
  }

  .save-btn {
    padding: 12px 32px;
    background: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%);
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 600;
    transition: all 0.3s ease;
    min-width: 160px;
    box-shadow: 0 2px 4px rgba(46, 204, 113, 0.2);
  }

    .save-btn:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(46, 204, 113, 0.3);
    }

    .save-btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none !important;
      box-shadow: none !important;
    }

  .cancel-btn {
    padding: 12px 32px;
    background-color: #95a5a6;
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 500;
    transition: all 0.3s ease;
    min-width: 120px;
  }

    .cancel-btn:hover:not(:disabled) {
      background-color: #7f8c8d;
      transform: translateY(-2px);
    }

    .cancel-btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

  /* Info Cards */
  .info-card {
    background: white;
    border-radius: 12px;
    padding: 24px;
    margin-bottom: 20px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.08);
    border: 1px solid #f0f0f0;
    display: flex;
    gap: 20px;
    align-items: flex-start;
  }

  .info-icon {
    font-size: 32px;
    flex-shrink: 0;
  }

  .info-content h4 {
    color: #2c3e50;
    margin: 0 0 12px 0;
    font-size: 16px;
    font-weight: 600;
  }

  .info-content p,
  .info-content li {
    color: #555;
    font-size: 14px;
    line-height: 1.5;
    margin-bottom: 8px;
  }

  .info-content ul {
    margin: 0;
    padding-left: 20px;
  }

  .info-content li {
    margin-bottom: 6px;
  }

  .contact-info {
    background-color: #f8f9fa;
    padding: 12px;
    border-radius: 6px;
    margin-top: 12px !important;
    font-size: 13px !important;
    color: #666 !important;
  }

  /* Success Message */
  .success-message {
    position: fixed;
    bottom: 24px;
    right: 24px;
    background: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%);
    color: white;
    padding: 16px 24px;
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(46, 204, 113, 0.3);
    z-index: 1000;
    animation: slideIn 0.3s ease-out;
    cursor: pointer;
    font-weight: 500;
    max-width: 400px;
    backdrop-filter: blur(10px);
  }

    .success-message:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 16px rgba(46, 204, 113, 0.4);
    }

  @keyframes slideIn {
    from {
      transform: translateX(100%);
      opacity: 0;
    }

    to {
      transform: translateX(0);
      opacity: 1;
    }
  }

  /* Validation Errors */
  .validation-errors {
    background: linear-gradient(135deg, #ffeaa7 0%, #fdcb6e 100%);
    border-radius: 12px;
    padding: 20px;
    margin-top: 24px;
    border-left: 4px solid #f39c12;
    animation: shake 0.5s ease;
  }

  @keyframes shake {
    0%, 100% {
      transform: translateX(0);
    }

    10%, 30%, 50%, 70%, 90% {
      transform: translateX(-5px);
    }

    20%, 40%, 60%, 80% {
      transform: translateX(5px);
    }
  }

  .validation-errors h4 {
    color: #d35400;
    margin: 0 0 12px 0;
    font-size: 16px;
    font-weight: 600;
  }

  .validation-errors ul {
    margin: 0;
    padding-left: 20px;
  }

  .validation-errors li {
    color: #d35400;
    font-size: 14px;
    margin-bottom: 6px;
    font-weight: 500;
  }

  /* Responsive */
  @media (max-width: 1024px) {
    .profile-form-container {
      flex-direction: column;
    }

    .profile-info {
      min-width: 100%;
    }
  }

  @media (max-width: 768px) {
    .profile-container {
      padding: 15px;
    }

    .profile-header {
      flex-direction: column;
      align-items: flex-start;
      gap: 15px;
    }

    .edit-btn {
      align-self: stretch;
    }

    .form-grid {
      grid-template-columns: 1fr;
    }

    .form-actions {
      flex-direction: column;
    }

    .save-btn,
    .cancel-btn {
      width: 100%;
      min-width: auto;
    }

    .success-message {
      left: 15px;
      right: 15px;
      bottom: 15px;
      text-align: center;
    }
  }
</style>
