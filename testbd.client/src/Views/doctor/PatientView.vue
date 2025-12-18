<template>
  <div class="patients-container">
    <!-- Заголовок -->
    <div class="header">
      <div class="header-icon">👥</div>
      <div>
        <h1>Завершенные приемы пациентов</h1>
        <p class="subtitle">Просмотр завершенных приемов и добавление записей в историю посещений</p>
      </div>
    </div>

    <!-- Сообщения об ошибках/успехе -->
    <div v-if="errorMessage" class="alert alert-error">
      {{ errorMessage }}
      <button class="close-btn" @click="errorMessage = ''">×</button>
    </div>

    <div v-if="successMessage" class="alert alert-success">
      {{ successMessage }}
      <button class="close-btn" @click="successMessage = ''">×</button>
    </div>

    <!-- Таблица завершенных приемов -->
    <div class="card">
      <div class="card-header">
        <div class="card-title">
          <span class="icon">📋</span>
          <span>Список завершенных приемов</span>
        </div>

        <div class="header-actions">
          <button class="btn btn-outline"
                  @click="toggleDateFilter">
            📅 {{ dateRangeText }}
          </button>

          <button class="btn btn-primary"
                  @click="refreshAppointments"
                  :disabled="loading">
            <span v-if="loading">🔄 Загрузка...</span>
            <span v-else>🔄 Обновить</span>
          </button>
        </div>
      </div>

      <!-- Фильтр по дате -->
      <div v-if="showDateFilter" class="date-filter">
        <div class="filter-content">
          <div class="filter-inputs">
            <div class="input-group">
              <label for="startDate">Начальная дата:</label>
              <input id="startDate"
                     v-model="startDate"
                     type="date"
                     class="form-control" />
            </div>
            <div class="input-group">
              <label for="endDate">Конечная дата:</label>
              <input id="endDate"
                     v-model="endDate"
                     type="date"
                     class="form-control" />
            </div>
          </div>
          <div class="filter-actions">
            <button class="btn btn-secondary" @click="clearDateFilter">Сбросить</button>
            <button class="btn btn-primary" @click="applyDateFilter">Применить</button>
          </div>
        </div>
      </div>

      <div class="card-body">
        <div v-if="loading" class="loading">
          <div class="spinner"></div>
          <p>Загрузка завершенных приемов...</p>
        </div>

        <div v-else-if="appointments.length === 0" class="empty-state">
          <div class="empty-icon">📅</div>
          <h3>Нет завершенных приемов</h3>
          <p>Завершенные приемы появятся здесь после их завершения</p>
        </div>

        <!-- Статистика -->
        <div v-else class="stats">
          <span class="stat-item">
            <span class="stat-icon">📋</span>
            Всего приемов: {{ appointments.length }}
          </span>
          <span class="stat-item">
            <span class="stat-icon">👤</span>
            Уникальных пациентов: {{ uniquePatientsCount }}
          </span>
          <span class="stat-item">
            <span class="stat-icon">👨‍⚕️</span>
            Врач ID: {{ currentDoctorId ? currentDoctorId.substring(0, 8) + '...' : 'Не определен' }}
          </span>
        </div>

        <!-- Таблица -->
        <div v-if="!loading && appointments.length > 0" class="table-container">
          <table class="table">
            <thead>
              <tr>
                <th @click="sortBy('appointmentDate')">
                  Дата приема
                  <span class="sort-icon" v-if="sortField === 'appointmentDate'">
                    {{ sortDirection === 'asc' ? '↑' : '↓' }}
                  </span>
                </th>
                <th @click="sortBy('appointmentTime')">
                  Время и кабинет
                  <span class="sort-icon" v-if="sortField === 'appointmentTime'">
                    {{ sortDirection === 'asc' ? '↑' : '↓' }}
                  </span>
                </th>
                <th @click="sortBy('patient')">
                  Пациент
                  <span class="sort-icon" v-if="sortField === 'patient'">
                    {{ sortDirection === 'asc' ? '↑' : '↓' }}
                  </span>
                </th>
                <th>Статус</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="appointment in sortedAppointments" :key="appointment.appointmentId">
                <td>
                  <div class="date-cell">
                    <div>{{ formatDate(appointment.appointmentDate) }}</div>
                    <div class="text-muted">{{ formatDayOfWeek(appointment.appointmentDate) }}</div>
                  </div>
                </td>
                <td>
                  <div v-if="appointment.scheduleNote">
                    <div class="time-slot">
                      {{ formatTime(appointment.scheduleNote.startTime) }} -
                      {{ formatTime(appointment.scheduleNote.endTime) }}
                    </div>
                    <div v-if="appointment.scheduleNote.cabinetNumber" class="cabinet">
                      🚪 Кабинет {{ appointment.scheduleNote.cabinetNumber }}
                    </div>
                  </div>
                  <span v-else class="text-muted">Нет данных</span>
                </td>
                <td>
                  <div class="patient-info">
                    <div class="patient-avatar">
                      {{ getInitials(appointment.patient.patientFio) }}
                    </div>
                    <div class="patient-details">
                      <div class="patient-name">{{ appointment.patient.patientFio }}</div>
                      <div class="patient-id">ID: {{ formatPatientId(appointment.patient.patientId) }}</div>
                      <div class="patient-phone">📞 {{ appointment.patient.contactPhone }}</div>
                    </div>
                  </div>
                </td>
                <td>
                  <span class="status-badge status-completed">
                    ✅ {{ getStatusText(appointment.status) }}
                  </span>
                </td>
                <td>
                  <div class="action-buttons">
                    <button class="btn btn-primary btn-sm"
                            @click="openAddRecordDialog(appointment)"
                            :disabled="appointment.addingRecord">
                      ➕ Добавить запись
                    </button>
                    <button class="btn btn-secondary btn-sm"
                            @click="viewPatientInfo(appointment)">
                      ℹ️ Инфо
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>

          <!-- Пагинация -->
          <div v-if="totalPages > 1" class="pagination">
            <button class="btn btn-outline"
                    @click="prevPage"
                    :disabled="currentPage === 1">
              ← Назад
            </button>
            <span>Страница {{ currentPage }} из {{ totalPages }}</span>
            <button class="btn btn-outline"
                    @click="nextPage"
                    :disabled="currentPage === totalPages">
              Вперед →
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Модальное окно добавления записи -->
    <div v-if="dialogVisible" class="modal-overlay" @click="closeDialog">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>➕ Добавление записи в историю пациента</h2>
          <button class="close-btn" @click="closeDialog">×</button>
        </div>

        <div v-if="selectedAppointment" class="modal-subheader">
          <div class="patient-tags">
            <span class="tag tag-primary">
              👤 {{ selectedAppointment.patient.patientFio }}
            </span>
            <span class="tag tag-secondary">
              📞 {{ selectedAppointment.patient.contactPhone }}
            </span>
            <span class="tag tag-success">
              📅 {{ formatDate(selectedAppointment.appointmentDate) }}
            </span>
            <span v-if="selectedAppointment.scheduleNote" class="tag tag-info">
              ⏰ {{ formatTime(selectedAppointment.scheduleNote.startTime) }}
            </span>
          </div>
        </div>

        <div class="modal-body">
          <div v-if="formSuccess" class="alert alert-success">
            {{ formSuccess }}
          </div>

          <form v-else @submit.prevent="submitForm" class="form">
            <div class="form-row">
              <div class="form-group">
                <label for="visitDate">Дата посещения *</label>
                <input id="visitDate"
                       v-model="formData.visitDate"
                       type="date"
                       class="form-control"
                       required
                       :max="new Date().toISOString().split('T')[0]" />
              </div>

              <div class="form-group">
                <label for="visitType">Тип посещения *</label>
                <select id="visitType"
                        v-model="formData.visitType"
                        class="form-control"
                        required>
                  <option value="">Выберите тип посещения</option>
                  <option value="Primary">Первичный (Primary)</option>
                  <option value="Secondary">Повторный (Secondary)</option>
                  <option value="Control">Контрольный (Control)</option>
                  <option value="Emergency">Экстренный (Emergency)</option>
                </select>
              </div>
            </div>

            <div class="form-group">
              <label for="prediagnose">Предварительный диагноз</label>
              <textarea id="prediagnose"
                        v-model="formData.prediagnose"
                        class="form-control"
                        rows="3"
                        placeholder="Введите предварительный диагноз, если требуется"
                        :maxlength="500"></textarea>
              <div class="char-count" v-if="formData.prediagnose">
                {{ formData.prediagnose.length }}/500
              </div>
            </div>

            <div class="form-group">
              <label for="diagnose">Диагноз *</label>
              <textarea id="diagnose"
                        v-model="formData.diagnose"
                        class="form-control"
                        rows="4"
                        required
                        placeholder="Введите окончательный диагноз"
                        :maxlength="1000"></textarea>
              <div class="char-count" v-if="formData.diagnose">
                {{ formData.diagnose.length }}/1000
              </div>
            </div>

            <input type="hidden" v-model="formData.patientId" />
            <input type="hidden" v-model="formData.appointmentId" />

            <div v-if="formError" class="alert alert-error">
              {{ formError }}
            </div>
          </form>
        </div>

        <div class="modal-footer">
          <button class="btn btn-secondary"
                  @click="closeDialog"
                  :disabled="submitting">
            Отмена
          </button>
          <button v-if="!formSuccess"
                  class="btn btn-primary"
                  @click="submitForm"
                  :disabled="submitting || !formValid">
            <span v-if="submitting">💾 Сохранение...</span>
            <span v-else>💾 Сохранить запись</span>
          </button>
          <button v-else
                  class="btn btn-primary"
                  @click="closeDialog">
            Закрыть
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue'
  import appointmentService from '@/services/appointmentService'

  // Реактивные переменные
  const appointments = ref([])
  const loading = ref(false)
  const errorMessage = ref('')
  const successMessage = ref('')
  const dialogVisible = ref(false)
  const selectedAppointment = ref(null)
  const submitting = ref(false)
  const formError = ref('')
  const formSuccess = ref('')
  const showDateFilter = ref(false)
  const startDate = ref('')
  const endDate = ref('')
  const sortField = ref('appointmentDate')
  const sortDirection = ref('desc')
  const currentPage = ref(1)
  const itemsPerPage = ref(10)
  const currentDoctorId = ref('')

  // Данные формы
  const formData = ref({
    visitDate: '',
    prediagnose: '',
    visitType: '',
    diagnose: '',
    patientId: '',
    appointmentId: ''
  })

  // Вычисляемые свойства
  const dateRangeText = computed(() => {
    if (startDate.value && endDate.value) {
      return `${formatDisplayDate(startDate.value)} - ${formatDisplayDate(endDate.value)}`
    } else if (startDate.value) {
      return `С ${formatDisplayDate(startDate.value)}`
    } else if (endDate.value) {
      return `По ${formatDisplayDate(endDate.value)}`
    }
    return 'Фильтр по дате'
  })

  const uniquePatientsCount = computed(() => {
    const patientIds = new Set(appointments.value.map(a => a.patient.patientId))
    return patientIds.size
  })

  const sortedAppointments = computed(() => {
    let sorted = [...appointments.value]

    if (sortField.value) {
      sorted.sort((a, b) => {
        let aValue, bValue

        switch (sortField.value) {
          case 'appointmentDate':
            aValue = a.appointmentDate || ''
            bValue = b.appointmentDate || ''
            break
          case 'appointmentTime':
            aValue = a.scheduleNote?.startTime || ''
            bValue = b.scheduleNote?.startTime || ''
            break
          case 'patient':
            aValue = a.patient.patientFio || ''
            bValue = b.patient.patientFio || ''
            break
          default:
            return 0
        }

        const direction = sortDirection.value === 'asc' ? 1 : -1
        return aValue.localeCompare(bValue) * direction
      })
    }

    // Пагинация
    const start = (currentPage.value - 1) * itemsPerPage.value
    const end = start + itemsPerPage.value
    return sorted.slice(start, end)
  })

  const totalPages = computed(() => {
    return Math.ceil(appointments.value.length / itemsPerPage.value)
  })

  const formValid = computed(() => {
    return formData.value.visitDate &&
      formData.value.visitType &&
      formData.value.diagnose.trim()
  })

  // Методы
  const fetchCompletedAppointments = async () => {
    try {
      loading.value = true
      errorMessage.value = ''

      // Получаем ID врача
      const doctorId = await getCurrentDoctorId()
      currentDoctorId.value = doctorId

      if (!doctorId) {
        throw new Error('Не удалось определить ID врача')
      }

      console.log('Fetching appointments for doctor ID:', doctorId)

      // Передаем ID врача в метод
      const data = await appointmentService.getCompletedAppointments(doctorId)
      appointments.value = data

      console.log('Appointments loaded:', data.length)

    } catch (error) {
      console.error('Error fetching appointments:', error)
      errorMessage.value = error.message || 'Ошибка при загрузке данных. Попробуйте обновить страницу.'
    } finally {
      loading.value = false
    }
  }

  // Функция получения ID текущего врача
  const getCurrentDoctorId = async () => {
    try {
      const token = localStorage.getItem('auth_token')

      if (!token) {
        console.error('No auth token found')
        return null
      }

      // Декодируем JWT токен
      const payload = decodeJWT(token)
      console.log('Token payload:', payload)

      // Пробуем разные возможные поля, где может быть ID врача
      const possibleIdFields = [
        'employeeId',
        'EmployeeId',
        'doctorId',
        'DoctorId',
        'userId',
        'UserId',
        'sub',
        'nameid',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ]

      for (const field of possibleIdFields) {
        if (payload[field]) {
          console.log(`Found doctor ID in field '${field}':`, payload[field])
          return payload[field]
        }
      }
      
      // Если не нашли в токене, пробуем из localStorage
      const userData = localStorage.getItem('user')
      if (userData) {
        try {
          const user = JSON.parse(userData)
          if (user.employeeId || user.id) {
            console.log('Found doctor ID in localStorage:', user.employeeId || user.id)
            return user.employeeId || user.id
          }
        } catch (e) {
          console.error('Error parsing user data:', e)
        }
      }

      console.warn('Could not find doctor ID in token')
      return null

    } catch (error) {
      console.error('Error getting doctor id:', error)
      return null
    }
  }

  // Функция декодирования JWT
  const decodeJWT = (token) => {
    try {
      // JWT состоит из 3 частей: header.payload.signature
      const base64Url = token.split('.')[1]
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      )

      return JSON.parse(jsonPayload)
    } catch (error) {
      console.error('Error decoding JWT:', error)
      return {}
    }
  }

  const refreshAppointments = () => {
    fetchCompletedAppointments()
  }

  const toggleDateFilter = () => {
    showDateFilter.value = !showDateFilter.value
  }

  const applyDateFilter = () => {
    showDateFilter.value = false
    // Здесь можно добавить фильтрацию на бэкенде
    successMessage.value = 'Фильтр по дате применен'
    setTimeout(() => { successMessage.value = '' }, 3000)
  }

  const clearDateFilter = () => {
    startDate.value = ''
    endDate.value = ''
    fetchCompletedAppointments()
  }

  const sortBy = (field) => {
    if (sortField.value === field) {
      sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
    } else {
      sortField.value = field
      sortDirection.value = 'asc'
    }
  }

  const prevPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--
    }
  }

  const nextPage = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
    }
  }

  const openAddRecordDialog = (appointment) => {
    selectedAppointment.value = appointment
    formError.value = ''
    formSuccess.value = ''

    // Устанавливаем дату посещения
    let visitDate = ''
    if (appointment.appointmentDate) {
      visitDate = appointment.appointmentDate
    } else if (appointment.scheduleNote?.pointDate) {
      visitDate = appointment.scheduleNote.pointDate
    } else {
      visitDate = new Date().toISOString().split('T')[0]
    }

    // Заполняем форму
    formData.value = {
      visitDate: visitDate,
      prediagnose: '',
      visitType: '',
      diagnose: '',
      patientId: appointment.patient.patientId,
      appointmentId: appointment.appointmentId
    }

    dialogVisible.value = true
  }

  const closeDialog = () => {
    if (!submitting.value) {
      dialogVisible.value = false
      selectedAppointment.value = null
      formError.value = ''
      formSuccess.value = ''
    }
  }

  const submitForm = async () => {
    if (!formValid.value) {
      formError.value = 'Пожалуйста, заполните все обязательные поля'
      return
    }

    try {
      submitting.value = true
      formError.value = ''

      await appointmentService.addVisitHistory(formData.value)

      formSuccess.value = 'Запись успешно добавлена в историю пациента'
      submitting.value = false

      // Обновляем статус записи в таблице
      const index = appointments.value.findIndex(
        a => a.appointmentId === selectedAppointment.value.appointmentId
      )
      if (index !== -1) {
        appointments.value[index].hasRecord = true
      }

    } catch (error) {
      console.error('Error submitting form:', error)
      formError.value = error.response?.data?.message || 'Ошибка при сохранении записи'
      submitting.value = false
    }
  }

  const viewPatientInfo = (appointment) => {
    alert('View patient info:', appointment.patient)
  }

  // Вспомогательные функции
  const formatDate = (dateString) => {
    if (!dateString) return 'Дата не указана'

    const date = new Date(dateString)
    return date.toLocaleDateString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    })
  }

  const formatDisplayDate = (dateString) => {
    if (!dateString) return ''
    const date = new Date(dateString)
    return date.toLocaleDateString('ru-RU')
  }

  const formatDayOfWeek = (dateString) => {
    if (!dateString) return ''
    const date = new Date(dateString)
    const days = ['ВС', 'ПН', 'ВТ', 'СР', 'ЧТ', 'ПТ', 'СБ']
    return days[date.getDay()]
  }

  const formatTime = (timeString) => {
    if (!timeString) return ''
    // Оставляем только часы и минуты
    return timeString.substring(0, 5)
  }

  const getInitials = (name) => {
    if (!name) return '??'
    const parts = name.split(' ')
    if (parts.length >= 2) {
      return `${parts[0][0]}${parts[1][0]}`.toUpperCase()
    }
    return name.substring(0, 2).toUpperCase()
  }

  const formatPatientId = (patientId) => {
    if (!patientId) return 'N/A'
    return patientId.substring(0, 8).toUpperCase()
  }

  const getStatusText = (status) => {
    const statusMap = {
      'completed': 'Завершен',
      'booked': 'Забронирован',
      'cancelled': 'Отменен',
      'in_progress': 'В процессе'
    }
    return statusMap[status] || status
  }

  // Жизненный цикл
  onMounted(() => {
    fetchCompletedAppointments()
  })
</script>

<style scoped>
  .patients-container {
    padding: 24px;
    max-width: 1400px;
    margin: 0 auto;
  }

  /* Заголовок */
  .header {
    display: flex;
    align-items: center;
    gap: 16px;
    margin-bottom: 32px;
  }

  .header-icon {
    font-size: 48px;
  }

  .header h1 {
    margin: 0;
    font-size: 32px;
    color: #333;
  }

  .subtitle {
    margin: 8px 0 0;
    color: #666;
    font-size: 16px;
  }

  /* Карточка */
  .card {
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    margin-bottom: 24px;
    overflow: hidden;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px 24px;
    border-bottom: 1px solid #eaeaea;
  }

  .card-title {
    display: flex;
    align-items: center;
    gap: 12px;
    font-size: 20px;
    font-weight: 600;
  }

  .header-actions {
    display: flex;
    gap: 12px;
  }

  /* Фильтр даты */
  .date-filter {
    padding: 16px 24px;
    border-bottom: 1px solid #eaeaea;
    background: #f8f9fa;
  }

  .filter-content {
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    gap: 16px;
  }

  .filter-inputs {
    display: flex;
    gap: 16px;
    flex: 1;
  }

  .input-group {
    flex: 1;
  }

    .input-group label {
      display: block;
      margin-bottom: 8px;
      font-size: 14px;
      color: #555;
    }

  .filter-actions {
    display: flex;
    gap: 8px;
  }

  /* Тело карточки */
  .card-body {
    padding: 24px;
  }

  /* Состояние загрузки */
  .loading {
    text-align: center;
    padding: 48px;
  }

  .spinner {
    width: 48px;
    height: 48px;
    border: 4px solid #e0e0e0;
    border-top: 4px solid #1976D2;
    border-radius: 50%;
    margin: 0 auto 16px;
    animation: spin 1s linear infinite;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }

  /* Пустое состояние */
  .empty-state {
    text-align: center;
    padding: 48px;
  }

  .empty-icon {
    font-size: 64px;
    margin-bottom: 16px;
    opacity: 0.5;
  }

  .empty-state h3 {
    margin: 0 0 8px;
    color: #666;
  }

  .empty-state p {
    color: #888;
    margin: 0;
  }

  /* Статистика */
  .stats {
    display: flex;
    gap: 16px;
    margin-bottom: 16px;
    padding: 12px 16px;
    background: #f8f9fa;
    border-radius: 8px;
    flex-wrap: wrap;
  }

  .stat-item {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 8px 16px;
    background: white;
    border-radius: 6px;
    border: 1px solid #dee2e6;
    white-space: nowrap;
  }

  .stat-icon {
    font-size: 20px;
  }

  /* Таблица */
  .table-container {
    overflow-x: auto;
  }

  .table {
    width: 100%;
    border-collapse: collapse;
    font-size: 14px;
  }

    .table th {
      padding: 16px;
      text-align: left;
      border-bottom: 2px solid #dee2e6;
      font-weight: 600;
      color: #495057;
      background: #f8f9fa;
      cursor: pointer;
      user-select: none;
    }

      .table th:hover {
        background: #e9ecef;
      }

  .sort-icon {
    margin-left: 8px;
    font-size: 12px;
  }

  .table td {
    padding: 16px;
    border-bottom: 1px solid #dee2e6;
  }

  .table tr:hover {
    background: #f8f9fa;
  }

  .date-cell {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .time-slot {
    font-weight: 500;
  }

  .cabinet {
    font-size: 12px;
    color: #666;
  }

  /* Информация о пациенте */
  .patient-info {
    display: flex;
    align-items: center;
    gap: 12px;
  }

  .patient-avatar {
    width: 40px;
    height: 40px;
    background: linear-gradient(135deg, #1976D2 0%, #0D47A1 100%);
    color: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 600;
    font-size: 14px;
  }

  .patient-details {
    flex: 1;
  }

  .patient-name {
    font-weight: 500;
    margin-bottom: 2px;
  }

  .patient-id {
    font-size: 12px;
    color: #666;
    margin-bottom: 2px;
  }

  .patient-phone {
    font-size: 12px;
    color: #666;
  }

  /* Статус */
  .status-badge {
    display: inline-block;
    padding: 6px 12px;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 500;
  }

  .status-completed {
    background: #d4edda;
    color: #155724;
    border: 1px solid #c3e6cb;
  }

  /* Кнопки действий */
  .action-buttons {
    display: flex;
    gap: 8px;
  }

  /* Пагинация */
  .pagination {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 16px;
    margin-top: 24px;
    padding: 16px;
    border-top: 1px solid #dee2e6;
  }

  /* Модальное окно */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
    padding: 20px;
  }

  .modal-content {
    background: white;
    border-radius: 12px;
    width: 100%;
    max-width: 800px;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 4px 24px rgba(0, 0, 0, 0.15);
  }

  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px 24px;
    border-bottom: 1px solid #eaeaea;
  }

    .modal-header h2 {
      margin: 0;
      font-size: 20px;
      color: #333;
    }

  .modal-subheader {
    padding: 16px 24px;
    background: #f8f9fa;
    border-bottom: 1px solid #eaeaea;
  }

  .patient-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
  }

  .tag {
    padding: 6px 12px;
    border-radius: 20px;
    font-size: 14px;
    font-weight: 500;
  }

  .tag-primary {
    background: #e3f2fd;
    color: #1976D2;
    border: 1px solid #bbdefb;
  }

  .tag-secondary {
    background: #f5f5f5;
    color: #666;
    border: 1px solid #e0e0e0;
  }

  .tag-success {
    background: #e8f5e9;
    color: #2e7d32;
    border: 1px solid #c8e6c9;
  }

  .tag-info {
    background: #e1f5fe;
    color: #0288d1;
    border: 1px solid #b3e5fc;
  }

  .modal-body {
    padding: 24px;
  }

  /* Форма */
  .form-row {
    display: flex;
    gap: 16px;
    margin-bottom: 16px;
  }

  .form-group {
    flex: 1;
    margin-bottom: 16px;
  }

    .form-group label {
      display: block;
      margin-bottom: 8px;
      font-weight: 500;
      color: #333;
    }

  .form-control {
    width: 100%;
    padding: 10px 12px;
    border: 1px solid #ddd;
    border-radius: 6px;
    font-size: 14px;
    transition: border-color 0.2s;
  }

    .form-control:focus {
      outline: none;
      border-color: #1976D2;
      box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1);
    }

    .form-control[required] {
      border-left: 3px solid #1976D2;
    }

  textarea.form-control {
    resize: vertical;
    min-height: 80px;
  }

  .char-count {
    text-align: right;
    font-size: 12px;
    color: #666;
    margin-top: 4px;
  }

  .modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
    padding: 20px 24px;
    border-top: 1px solid #eaeaea;
    background: #f8f9fa;
  }

  /* Алерты */
  .alert {
    padding: 12px 16px;
    border-radius: 6px;
    margin-bottom: 16px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .alert-error {
    background: #fdecea;
    color: #d32f2f;
    border: 1px solid #f5c2c7;
  }

  .alert-success {
    background: #edf7ed;
    color: #2e7d32;
    border: 1px solid #d4edda;
  }

  /* Кнопки */
  .btn {
    padding: 10px 20px;
    border: none;
    border-radius: 6px;
    font-size: 14px;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
  }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  .btn-primary {
    background: #1976D2;
    color: white;
  }

    .btn-primary:hover:not(:disabled) {
      background: #1565c0;
    }

  .btn-secondary {
    background: #6c757d;
    color: white;
  }

    .btn-secondary:hover:not(:disabled) {
      background: #5a6268;
    }

  .btn-outline {
    background: transparent;
    color: #1976D2;
    border: 1px solid #1976D2;
  }

    .btn-outline:hover:not(:disabled) {
      background: #f8f9fa;
    }

  .btn-sm {
    padding: 6px 12px;
    font-size: 13px;
  }

  .close-btn {
    background: none;
    border: none;
    font-size: 24px;
    color: #666;
    cursor: pointer;
    padding: 4px;
    line-height: 1;
  }

    .close-btn:hover {
      color: #333;
    }

  /* Утилитарные классы */
  .text-muted {
    color: #666;
    font-size: 12px;
  }

  .text-center {
    text-align: center;
  }

  /* Адаптивность */
  @media (max-width: 768px) {
    .patients-container {
      padding: 16px;
    }

    .header {
      flex-direction: column;
      align-items: flex-start;
      gap: 8px;
    }

    .card-header {
      flex-direction: column;
      align-items: flex-start;
      gap: 16px;
    }

    .header-actions {
      width: 100%;
    }

    .filter-content {
      flex-direction: column;
      align-items: stretch;
    }

    .filter-inputs {
      flex-direction: column;
    }

    .form-row {
      flex-direction: column;
    }

    .patient-info {
      flex-direction: column;
      align-items: flex-start;
    }

    .action-buttons {
      flex-direction: column;
    }

    .modal-content {
      margin: 0;
      border-radius: 0;
      max-height: 100vh;
    }
  }
</style>
