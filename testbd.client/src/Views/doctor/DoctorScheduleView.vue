<!-- views/doctor/ScheduleView.vue -->
<template>
  <div class="schedule-view">
    <h2>Мое расписание</h2>

    <div class="controls">
      <div class="date-selector">
        <label>Дата:</label>
        <input type="date"
               v-model="selectedDate"
               @change="loadSchedule"
               :max="maxDate"
               :min="minDate" />
      </div>

      <div class="stats" v-if="schedule.length > 0">
        <span>Всего слотов: {{ schedule.length }}</span>
        <span>Занято: {{ occupiedSlots }}</span>
        <span>Свободно: {{ freeSlots }}</span>
      </div>
    </div>

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Загрузка расписания...</p>
    </div>

    <div v-else-if="error" class="error-message">
      <p>❌ {{ error }}</p>
      <button @click="loadSchedule" class="retry-btn">Повторить</button>
    </div>

    <div v-else-if="schedule.length === 0" class="empty-schedule">
      <div class="empty-icon">
        📅
      </div>
      <h3>Расписание не найдено</h3>
      <p>На выбранную дату нет запланированных слотов</p>
      <p class="hint">Попробуйте выбрать другую дату</p>
    </div>

    <div v-else class="schedule-list">
      <div class="schedule-header">
        <h3>Расписание на {{ formatDate(selectedDate) }}</h3>
        <div class="legend">
          <span class="legend-item free">Свободно</span>
          <span class="legend-item occupied">Занято</span>
          <span class="legend-item current">Текущее время</span>
        </div>
      </div>

      <div class="slots-grid">
        <div v-for="slot in schedule"
             :key="slot.scheduleNoteId"
             class="schedule-slot"
             :class="{
               'occupied': hasAppointment(slot),
               'current': isCurrentSlot(slot)
             }">
          <div class="slot-header">
            <span class="slot-time">
              {{ formatTime(slot.startTime) }} - {{ formatTime(slot.endTime) }}
            </span>
            <span class="slot-status">
              <span v-if="hasAppointment(slot)" class="badge occupied">
                Занято
              </span>
              <span v-else class="badge free">Свободно</span>
            </span>
          </div>

          <div class="slot-info">
            <p><strong>Кабинет:</strong> {{ slot.cabinetNumber || 'Не указан' }}</p>

            <div v-if="hasAppointment(slot)" class="appointment-info">
              <p><strong>Пациент:</strong> {{ getPatientName(slot) }}</p>
              <p><strong>Статус:</strong> {{ getAppointmentStatus(slot) }}</p>
              <p v-if="getPatientPhone(slot)">
                <strong>Телефон:</strong> {{ getPatientPhone(slot) }}
              </p>
              <p v-if="slot.appointment?.createdAt">
                <strong>Создано:</strong> {{ formatDateTime(slot.appointment.createdAt) }}
              </p>
            </div>
            <div v-else class="no-appointment">
              <p>Нет записи</p>
              <p class="slot-available" v-if="slot.isAvailable">
                <small>Слот доступен для записи</small>
              </p>
              <p class="slot-unavailable" v-else>
                <small>Слот недоступен</small>
              </p>
            </div>
          </div>

          <div class="slot-actions" v-if="hasAppointment(slot)">
            <button @click="openServiceForm(slot)"
                    class="action-btn primary"
                    :disabled="slot.appointment?.status === 'completed'">
              📋 Заполнить услуги
            </button>
            <button @click="viewAppointmentDetails(slot)"
                    class="action-btn secondary">
              👁️ Подробнее
            </button>
            <button v-if="slot.appointment?.status === 'booked'"
                    @click="cancelAppointment(slot)"
                    class="action-btn danger">
              ❌ Отменить
            </button>
          </div>
          <div class="slot-actions" v-else-if="slot.isAvailable">
            <button class="action-btn free" disabled>
              📝 Свободно для записи
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Форма оказания услуг -->
    <div v-if="showServiceForm" class="modal-overlay" @click.self="closeServiceForm">
      <div class="modal">
        <div class="modal-header">
          <h3>Оказанные услуги для пациента</h3>
          <button @click="closeServiceForm" class="close-btn">×</button>
        </div>

        <div class="modal-body">
          <div class="patient-info" v-if="currentSlot?.appointment">
            <h4>Информация о приеме:</h4>
            <p><strong>Пациент:</strong> {{ getPatientName(currentSlot) }}</p>
            <p><strong>Время:</strong> {{ formatTime(currentSlot.startTime) }} - {{ formatTime(currentSlot.endTime) }}</p>
            <p><strong>Дата:</strong> {{ formatDate(currentSlot.pointDate) }}</p>
            <p><strong>Кабинет:</strong> {{ currentSlot.cabinetNumber }}</p>
            <p><strong>Телефон пациента:</strong> {{ getPatientPhone(currentSlot) }}</p>
          </div>

          <div class="services-form">
            <h4>Оказанные услуги:</h4>

            <div v-for="(service, index) in services" :key="index" class="service-item">
              <div class="service-form">
                <div class="form-group">
                  <label>Услуга *</label>
                  <select v-model="service.serviceCode"
                          required
                          @change="updateServicePrice(index)">
                    <option value="">Выберите услугу</option>
                    <option v-for="serv in availableServices"
                            :value="serv.serviceCode"
                            :key="serv.serviceCode">
                      {{ serv.serviceName }} - {{ serv.basePrice }} руб.
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>Фактическая цена (руб.) *</label>
                  <input type="number"
                         v-model="service.factPrice"
                         placeholder="Введите цену"
                         required
                         min="0"
                         step="0.01" />
                </div>

                <button v-if="services.length > 1"
                        @click="removeService(index)"
                        class="remove-btn"
                        type="button">
                  🗑️ Удалить
                </button>
              </div>
            </div>

            <button @click="addService" class="add-btn" type="button">
              ➕ Добавить услугу
            </button>

            <div class="total-price" v-if="totalPrice > 0">
              <h4>Общая стоимость: {{ totalPrice.toFixed(2) }} руб.</h4>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="saveServices"
                  class="save-btn"
                  :disabled="!isFormValid || saving">
            <span v-if="saving">
              <span class="spinner-small"></span> Сохранение...
            </span>
            <span v-else>💾 Сохранить услуги</span>
          </button>
          <button @click="closeServiceForm" class="cancel-btn" :disabled="saving">
            Отмена
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed, watch } from 'vue';
  import { useAuthStore } from '@/stores/auth';
  import { doctorService } from '@/services/doctor.service';
  import { serviceService } from '@/services/service.service';

  const authStore = useAuthStore();

  // Реактивные переменные
  const schedule = ref([]);
  const selectedDate = ref(new Date().toISOString().split('T')[0]);
  const loading = ref(false);
  const error = ref(null);
  const showServiceForm = ref(false);
  const currentSlot = ref(null); // Изменено с currentAppointment на currentSlot
  const services = ref([{ serviceCode: '', factPrice: 0, quantity: 1 }]);
  const availableServices = ref([]);
  const saving = ref(false);

  // Вычисляемые свойства
  const maxDate = computed(() => {
    const date = new Date();
    date.setMonth(date.getMonth() + 3);
    return date.toISOString().split('T')[0];
  });

  const minDate = computed(() => {
    const date = new Date();
    date.setMonth(date.getMonth() - 1);
    return date.toISOString().split('T')[0];
  });

  const occupiedSlots = computed(() => {
    return schedule.value.filter(slot => hasAppointment(slot)).length;
  });

  const freeSlots = computed(() => {
    return schedule.value.length - occupiedSlots.value;
  });

  const totalPrice = computed(() => {
    return services.value.reduce((total, service) => {
      const price = parseFloat(service.factPrice) || 0;
      const quantity = parseInt(service.quantity) || 1;
      return total + (price * quantity);
    }, 0);
  });

  const isFormValid = computed(() => {
    return services.value.every(service =>
      service.serviceCode && service.factPrice > 0
    );
  });

  // Методы
  const loadSchedule = async () => {
    try {
      loading.value = true;
      error.value = null;
      schedule.value = [];

      if (!authStore.employeeId) {
        throw new Error('ID сотрудника не найден. Требуется авторизация врача.');
      }

      console.log('Загрузка расписания с записями для врача:', authStore.employeeId, 'дата:', selectedDate.value);

      // ИСПРАВЛЕНО: Используем новый метод
      const data = await doctorService.getDoctorScheduleWithAppointments(
        authStore.employeeId,
        selectedDate.value
      );

      console.log('Полученные данные расписания с записями:', data);

      if (Array.isArray(data)) {
        schedule.value = data;
      } else {
        console.warn('Получены неожиданные данные:', data);
        schedule.value = [];
      }

      if (schedule.value.length === 0) {
        console.log('Расписание пустое для выбранной даты');
      }
    } catch (err) {
      console.error('Ошибка загрузки расписания:', err);
      error.value = err.response?.data?.message || err.message || 'Не удалось загрузить расписание';
      schedule.value = [];
    } finally {
      loading.value = false;
    }
  };

  const loadServices = async () => {
    try {
      availableServices.value = await serviceService.getAllServices();
      console.log('Загружено услуг:', availableServices.value.length);
    } catch (err) {
      console.error('Ошибка загрузки услуг:', err);
      availableServices.value = [];
    }
  };

  // Проверка наличия записи в слоте
  const hasAppointment = (slot) => {
    return slot.appointment && slot.appointment.appointmentId && slot.appointment.status=='booked';
  };

  // Получение имени пациента
  const getPatientName = (slot) => {
    if (!hasAppointment(slot)) return 'Не указан';
    return slot.appointment.patient?.patientFio || 'Не указан';
  };

  // Получение телефона пациента
  const getPatientPhone = (slot) => {
    if (!hasAppointment(slot)) return null;
    return slot.appointment.patient?.contactPhone || null;
  };

  // Получение статуса записи
  const getAppointmentStatus = (slot) => {
    if (!hasAppointment(slot)) return 'Свободен';

    const statusMap = {
      'booked': 'Запланирован',
      'completed': 'Завершен',
      'cancelled': 'Отменен'
    };

    return statusMap[slot.appointment.status] || slot.appointment.status || 'Неизвестно';
  };

  const openServiceForm = (slot) => {
    if (!hasAppointment(slot)) {
      alert('Нет записи на этот слот');
      return;
    }

    currentSlot.value = slot;
    services.value = [{ serviceCode: '', factPrice: 0, quantity: 1 }];
    showServiceForm.value = true;

    console.log('Открыта форма для приема:', {
      slotId: slot.scheduleNoteId,
      appointmentId: slot.appointment.appointmentId,
      patient: getPatientName(slot)
    });
  };

  const closeServiceForm = () => {
    if (!saving.value) {
      showServiceForm.value = false;
      currentSlot.value = null;
      services.value = [{ serviceCode: '', factPrice: 0, quantity: 1 }];
    }
  };

  const addService = () => {
    services.value.push({ serviceCode: '', factPrice: 0, quantity: 1 });
  };

  const removeService = (index) => {
    if (services.value.length > 1) {
      services.value.splice(index, 1);
    }
  };

  const updateServicePrice = (index) => {
    const selectedService = availableServices.value.find(
      s => s.serviceCode === services.value[index].serviceCode
    );
    if (selectedService) {
      services.value[index].factPrice = selectedService.basePrice;
    }
  };

  const saveServices = async () => {
    if (!isFormValid.value) {
      alert('Заполните все обязательные поля');
      return;
    }

    try {
      saving.value = true;

      if (!currentSlot.value?.appointment?.appointmentId) {
        throw new Error('ID записи не найден');
      }
      const nowUtc = new Date().toISOString()

      // Сохраняем каждую услугу
      for (const service of services.value) {
        if (service.serviceCode) {
          await serviceService.addProvidedService({
            serviceCode: service.serviceCode,
            factPrice: parseFloat(service.factPrice),
            appointmentId: currentSlot.value.appointment.appointmentId,
            contractorId: authStore.employeeId,
            providedDate: nowUtc
          });
        }
      }

      alert('✅ Услуги успешно сохранены');
      closeServiceForm();

      // Обновляем расписание
      await loadSchedule();
    } catch (err) {
      console.error('Ошибка сохранения услуг:', err);
      alert('❌ Ошибка сохранения услуг: ' + (err.response?.data?.message || err.message || 'Неизвестная ошибка'));
    } finally {
      saving.value = false;
    }
  };

  const viewAppointmentDetails = (slot) => {
    if (!hasAppointment(slot)) {
      alert('Нет записи на этот слот');
      return;
    }

    const details = [
      `Пациент: ${getPatientName(slot)}`,
      `Время: ${formatTime(slot.startTime)} - ${formatTime(slot.endTime)}`,
      `Дата: ${formatDate(slot.pointDate)}`,
      `Кабинет: ${slot.cabinetNumber}`,
      `Статус: ${getAppointmentStatus(slot)}`,
      `Телефон: ${getPatientPhone(slot) || 'Не указан'}`
    ];

    alert('Детали приема:\n' + details.join('\n'));
  };

  const cancelAppointment = async (slot) => {
    if (!hasAppointment(slot)) {
      alert('Нет записи для отмены');
      return;
    }

    if (!confirm(`Вы уверены, что хотите отменить запись пациента ${getPatientName(slot)}?`)) {
      return;
    }

    try {
      // Здесь нужно добавить метод для отмены записи
      // await appointmentService.cancelAppointment(slot.appointment.appointmentId);
      alert('Запись отменена (реализация на бэкенде)');
      await loadSchedule(); // Обновляем расписание
    } catch (err) {
      console.error('Ошибка отмены записи:', err);
      alert('❌ Ошибка отмены записи');
    }
  };

  const formatTime = (time) => {
    if (!time) return '--:--';

    // Если время в формате "HH:MM:SS"
    if (typeof time === 'string' && time.includes(':')) {
      return time.substring(0, 5);
    }

    // Если время в формате TimeOnly (может быть объектом)
    if (time && typeof time === 'object' && 'hour' in time && 'minute' in time) {
      return `${time.hour.toString().padStart(2, '0')}:${time.minute.toString().padStart(2, '0')}`;
    }

    return time;
  };

  const formatDate = (dateInput) => {
    if (!dateInput) return '';

    // Если это DateOnly объект
    if (dateInput && typeof dateInput === 'object' && 'year' in dateInput && 'month' in dateInput) {
      const date = new Date(dateInput.year, dateInput.month - 1, dateInput.day);
      return date.toLocaleDateString('ru-RU', {
        day: '2-digit',
        month: 'long',
        year: 'numeric',
        weekday: 'long'
      });
    }

    // Если это строка
    try {
      const date = new Date(dateInput);
      return date.toLocaleDateString('ru-RU', {
        day: '2-digit',
        month: 'long',
        year: 'numeric',
        weekday: 'long'
      });
    } catch {
      return dateInput;
    }
  };

  const formatDateTime = (dateTimeString) => {
    if (!dateTimeString) return '';

    try {
      const date = new Date(dateTimeString);
      return date.toLocaleString('ru-RU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch {
      return dateTimeString;
    }
  };

  const isCurrentSlot = (slot) => {
    if (!slot.startTime || !slot.pointDate) return false;

    const now = new Date();
    const today = now.toISOString().split('T')[0];
    const slotDate = slot.pointDate;

    // Проверяем, что слот на сегодня
    if (typeof slotDate === 'object') {
      // Если DateOnly объект
      const slotDateStr = `${slotDate.year}-${slotDate.month.toString().padStart(2, '0')}-${slotDate.day.toString().padStart(2, '0')}`;
      if (slotDateStr !== today) return false;
    } else if (slotDate !== today) {
      return false;
    }

    const startTimeStr = typeof slot.startTime === 'object'
      ? `${slot.startTime.hour}:${slot.startTime.minute}:00`
      : slot.startTime;

    const start = new Date(`${today}T${startTimeStr}`);

    return now >= start;
  };

  const isPastSlot = (slot) => { // isPastSlot for accessing to the appointment
    if (!slot.startTime || !slot.pointDate) return false;

    const now = new Date();

    // Получаем дату слота
    let slotDateStr;
    if (typeof slot.pointDate === 'object') {
      // Если DateOnly объект
      slotDateStr = `${slot.pointDate.year}-${slot.pointDate.month.toString().padStart(2, '0')}-${slot.pointDate.day.toString().padStart(2, '0')}`;
    } else {
      slotDateStr = slot.pointDate;
    }

    // Получаем время начала
    const startTimeStr = typeof slot.endTime === 'object' // изменил на endTime
      ? `${slot.startTime.hour.toString().padStart(2, '0')}:${slot.startTime.minute.toString().padStart(2, '0')}:00`
      : slot.startTime;

    const slotDateTime = new Date(`${slotDateStr}T${startTimeStr}`);

    return now > slotDateTime;
  };

  // Наблюдатели
  watch(selectedDate, () => {
    loadSchedule();
  });

  // Хуки жизненного цикла
  onMounted(() => {
    console.log('Инициализация компонента расписания врача');
    console.log('Employee ID:', authStore.employeeId);
    console.log('User role:', authStore.user?.role);

    if (!authStore.employeeId) {
      error.value = 'Доступно только для врачей. Ваш аккаунт не имеет привязки к врачу.';
      return;
    }

    loadSchedule();
    loadServices();
  });
</script>

<style scoped>
  .slot-available {
    color: #28a745;
    font-size: 12px;
  }

  .slot-unavailable {
    color: #dc3545;
    font-size: 12px;
  }

  .action-btn.danger {
    background-color: #dc3545;
    color: white;
  }

    .action-btn.danger:hover {
      background-color: #c82333;
    }

  .action-btn.free {
    background-color: #6c757d;
    color: white;
    cursor: not-allowed;
  }
  .schedule-view {
    padding: 20px;
    max-width: 1200px;
    margin: 0 auto;
  }

  h2 {
    color: #2c3e50;
    margin-bottom: 30px;
    font-size: 28px;
  }

  .controls {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 30px;
    padding: 20px;
    background: white;
    border-radius: 10px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  }

  .date-selector {
    display: flex;
    align-items: center;
    gap: 15px;
  }

    .date-selector label {
      font-weight: 600;
      color: #2c3e50;
    }

    .date-selector input {
      padding: 10px 15px;
      border: 2px solid #ddd;
      border-radius: 8px;
      font-size: 16px;
      min-width: 200px;
    }

  .stats {
    display: flex;
    gap: 20px;
    font-size: 14px;
    color: #666;
  }

    .stats span {
      background: #f8f9fa;
      padding: 8px 15px;
      border-radius: 20px;
      border: 1px solid #eaeaea;
    }

  /* Loading */
  .loading {
    text-align: center;
    padding: 60px 20px;
  }

  .spinner {
    width: 50px;
    height: 50px;
    border: 4px solid #f3f3f3;
    border-top: 4px solid #3498db;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin: 0 auto 20px;
  }

  .spinner-small {
    display: inline-block;
    width: 16px;
    height: 16px;
    border: 2px solid #fff;
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

  /* Error */
  .error-message {
    text-align: center;
    padding: 40px 20px;
    background: #ffeaea;
    border-radius: 10px;
    margin: 20px 0;
  }

  .retry-btn {
    padding: 10px 20px;
    background: #e74c3c;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    margin-top: 10px;
  }

  /* Empty schedule */
  .empty-schedule {
    text-align: center;
    padding: 60px 20px;
    background: #f8f9fa;
    border-radius: 10px;
    border: 2px dashed #ddd;
  }

  .empty-icon {
    font-size: 64px;
    margin-bottom: 20px;
  }

  .empty-schedule h3 {
    color: #666;
    margin-bottom: 10px;
  }

  .empty-schedule p {
    color: #888;
    margin-bottom: 5px;
  }

  .hint {
    font-style: italic;
    color: #999 !important;
    font-size: 14px;
  }

  /* Schedule list */
  .schedule-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
    padding-bottom: 15px;
    border-bottom: 2px solid #eaeaea;
  }

  .legend {
    display: flex;
    gap: 15px;
  }

  .legend-item {
    padding: 5px 10px;
    border-radius: 4px;
    font-size: 12px;
    font-weight: 500;
  }

    .legend-item.free {
      background: #d4edda;
      color: #155724;
    }

    .legend-item.occupied {
      background: #f8d7da;
      color: #721c24;
    }

    .legend-item.current {
      background: #fff3cd;
      color: #856404;
    }

  /* Slots grid */
  .slots-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 20px;
  }

  .schedule-slot {
    background: white;
    border-radius: 10px;
    padding: 20px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    border: 2px solid #e0e0e0;
    transition: all 0.3s ease;
  }

    .schedule-slot.free {
      border-color: #c3e6cb;
      background: #f8fff9;
    }

    .schedule-slot.occupied {
      border-color: #f5c6cb;
      background: #fff8f9;
    }

    .schedule-slot.current {
      border-color: #ffeaa7;
      background: #fffdf0;
      animation: pulse 2s infinite;
    }

  @keyframes pulse {
    0% {
      box-shadow: 0 0 0 0 rgba(255, 193, 7, 0.4);
    }

    70% {
      box-shadow: 0 0 0 10px rgba(255, 193, 7, 0);
    }

    100% {
      box-shadow: 0 0 0 0 rgba(255, 193, 7, 0);
    }
  }

  .schedule-slot:hover {
    transform: translateY(-5px);
    box-shadow: 0 5px 20px rgba(0,0,0,0.15);
  }

  .slot-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 15px;
    padding-bottom: 10px;
    border-bottom: 1px solid #eee;
  }

  .slot-time {
    font-weight: 600;
    font-size: 16px;
    color: #2c3e50;
  }

  .badge {
    padding: 4px 10px;
    border-radius: 12px;
    font-size: 12px;
    font-weight: 600;
  }

    .badge.free {
      background: #d4edda;
      color: #155724;
    }

    .badge.occupied {
      background: #f8d7da;
      color: #721c24;
    }

  .slot-info p {
    margin: 8px 0;
    font-size: 14px;
    color: #555;
  }

  .appointment-info {
    margin-top: 15px;
    padding: 15px;
    background: #f8f9fa;
    border-radius: 8px;
    border-left: 4px solid #3498db;
  }

  .no-appointment {
    text-align: center;
    padding: 20px;
    color: #999;
    font-style: italic;
  }

  .slot-actions {
    margin-top: 20px;
    display: flex;
    gap: 10px;
  }

  .action-btn {
    flex: 1;
    padding: 10px 15px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 500;
    transition: all 0.3s;
  }

    .action-btn.primary {
      background: #3498db;
      color: white;
    }

      .action-btn.primary:hover:not(:disabled) {
        background: #2980b9;
      }

    .action-btn.secondary {
      background: #95a5a6;
      color: white;
    }

      .action-btn.secondary:hover {
        background: #7f8c8d;
      }

    .action-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  /* Modal */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0,0,0,0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
  }

  .modal {
    background: white;
    border-radius: 15px;
    width: 90%;
    max-width: 600px;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 10px 40px rgba(0,0,0,0.3);
  }

  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px;
    border-bottom: 1px solid #eee;
  }

  .close-btn {
    background: none;
    border: none;
    font-size: 24px;
    cursor: pointer;
    color: #999;
    padding: 5px 10px;
  }

    .close-btn:hover {
      color: #333;
    }

  .modal-body {
    padding: 20px;
  }

  .patient-info {
    background: #e8f4fc;
    padding: 15px;
    border-radius: 8px;
    margin-bottom: 20px;
  }

  .service-item {
    background: #f8f9fa;
    padding: 15px;
    border-radius: 8px;
    margin-bottom: 15px;
    border: 1px solid #eaeaea;
  }

  .service-form {
    display: grid;
    grid-template-columns: 1fr 1fr auto;
    gap: 15px;
    align-items: end;
  }

  .form-group {
    display: flex;
    flex-direction: column;
  }

    .form-group label {
      margin-bottom: 5px;
      font-weight: 500;
      color: #555;
      font-size: 14px;
    }

    .form-group select,
    .form-group input {
      padding: 10px;
      border: 2px solid #ddd;
      border-radius: 6px;
      font-size: 14px;
    }

      .form-group select:focus,
      .form-group input:focus {
        outline: none;
        border-color: #3498db;
      }

  .remove-btn {
    background: #e74c3c;
    color: white;
    border: none;
    padding: 10px 15px;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
  }

    .remove-btn:hover {
      background: #c0392b;
    }

  .add-btn {
    width: 100%;
    padding: 12px;
    background: #2ecc71;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    margin-top: 15px;
    font-size: 14px;
  }

    .add-btn:hover {
      background: #27ae60;
    }

  .total-price {
    margin-top: 20px;
    padding: 15px;
    background: #d4edda;
    border-radius: 8px;
    text-align: center;
  }

  .modal-footer {
    display: flex;
    gap: 15px;
    padding: 20px;
    border-top: 1px solid #eee;
  }

  .save-btn {
    flex: 2;
    padding: 15px;
    background: #2ecc71;
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-size: 16px;
    font-weight: 600;
  }

    .save-btn:hover:not(:disabled) {
      background: #27ae60;
    }

    .save-btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

  .cancel-btn {
    flex: 1;
    padding: 15px;
    background: #95a5a6;
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-size: 16px;
  }

    .cancel-btn:hover:not(:disabled) {
      background: #7f8c8d;
    }

  /* Responsive */
  @media (max-width: 768px) {
    .controls {
      flex-direction: column;
      gap: 15px;
      align-items: stretch;
    }

    .date-selector {
      flex-direction: column;
      align-items: stretch;
    }

    .stats {
      justify-content: center;
      flex-wrap: wrap;
    }

    .slots-grid {
      grid-template-columns: 1fr;
    }

    .service-form {
      grid-template-columns: 1fr;
    }

    .modal-footer {
      flex-direction: column;
    }

    .save-btn,
    .cancel-btn {
      width: 100%;
    }
  }
</style>
