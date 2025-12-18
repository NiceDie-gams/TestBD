<template>
  <div class="admin-dashboard">
    <!-- Шапка админки -->
    <header class="admin-header">
      <div class="header-left">
        <h1>Панель администратора</h1>
        <p class="subtitle">Управление медицинской клиникой</p>
      </div>
      <div class="header-right">
        <div class="user-info">
          <span class="username">{{ userName }}</span>
          <span class="role-badge">Администратор</span>
        </div>
        <button @click="logout" class="logout-btn">Выйти</button>
      </div>
    </header>

    <!-- Основной контент -->
    <main class="admin-content">
      <div class="sidebar">
        <nav class="admin-nav">
          <h3>Навигация</h3>
          <ul>
            <li>
              <a href="#" @click.prevent="setActiveTab('dashboard')"
                 :class="{ active: activeTab === 'dashboard' }">
                📊 Дашборд
              </a>
            </li>
            <li>
              <a href="#" @click.prevent="setActiveTab('doctors')"
                 :class="{ active: activeTab === 'doctors' }">
                👨‍⚕️ Врачи
              </a>
            </li>
            <li>
              <a href="#" @click.prevent="setActiveTab('patients')"
                 :class="{ active: activeTab === 'patients' }">
                👤 Пациенты
              </a>
            </li>
            <li>
              <a href="#" @click.prevent="setActiveTab('payments')"
                 :class="{ active: activeTab === 'payments' }">
                📅 Платежи
              </a>
            </li>
          </ul>
        </nav>

        <div class="sidebar-footer">
          <p>Версия: {{ appVersion }}</p>
          <p>Среда: {{ appEnv }}</p>
        </div>
      </div>

      <div class="content-area">
        <div v-if="activeTab === 'dashboard'" class="tab-content">
          <div class="empty-state">
            <h2>📊 Дашборд</h2>
            <p>Общая статистика клиники</p>
            <div class="stats-placeholder">
              <div class="stat-card">👥 Пациенты: {{ stats.patients }}</div>
              <div class="stat-card">👨‍⚕️ Врачи: {{ stats.doctors }}</div>
              <div class="stat-card">📅 Записи сегодня: {{ stats.appointments }}</div>
              <div class="stat-card">💰 Выручка: {{ formattedRevenue }}</div>
            </div>
          </div>
        </div>
        <!-- Вкладка Пациенты -->
        <div v-else-if="activeTab === 'patients'" class="tab-content">
          <div class="patients-header">
            <h2>👤 Управление пациентами</h2>
            <div class="patients-actions">
              <button @click="openCreateModal" class="btn-primary">
                + Добавить пациента
              </button>
              <div class="search-box">
                <input v-model="searchQuery"
                       type="text"
                       placeholder="Поиск пациентов..."
                       @input="filterPatients">
                <span class="search-icon">🔍</span>
              </div>
            </div>
          </div>

          <!-- Таблица пациентов -->
          <div class="patients-table-container" v-if="!loading">
            <div class="table-responsive">
              <table class="patients-table">
                <thead>
                  <tr>
                    <th @click="sortPatients('patientFio')">
                      ФИО
                      <span v-if="sortField === 'patientFio'" class="sort-icon">
                        {{ sortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th @click="sortPatients('patientBirthdate')">
                      Дата рождения
                      <span v-if="sortField === 'patientBirthdate'" class="sort-icon">
                        {{ sortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th>Пол</th>
                    <th>Телефон</th>
                    <th @click="sortPatients('omsPolisNumber')">
                      OMS полис
                      <span v-if="sortField === 'omsPolisNumber'" class="sort-icon">
                        {{ sortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th @click="sortPatients('registrationDate')">
                      Дата регистрации
                      <span v-if="sortField === 'registrationDate'" class="sort-icon">
                        {{ sortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th>Статус</th>
                    <th>Действия</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="patient in paginatedPatients" :key="patient.patientId">
                    <td>
                      <div class="patient-name">
                        <strong>{{ patient.patientFio }}</strong>
                      </div>
                    </td>
                    <td>{{ formatDate(patient.patientBirthdate) }}</td>
                    <td>
                      <span class="gender-badge" :class="patient.gender.toLowerCase()">
                        {{ patient.gender === 'М' ? 'Мужской' : 'Женский' }}
                      </span>
                    </td>
                    <td>{{ patient.contactPhone }}</td>
                    <td>{{ patient.omsPolisNumber || '-' }}</td>
                    <td>{{ formatDate(patient.registrationDate) }}</td>
                    <td>
                      <span class="status-badge" :class="patient.isActive ? 'active' : 'inactive'">
                        {{ patient.isActive ? 'Активен' : 'Не активен' }}
                      </span>
                    </td>
                    <td>
                      <div class="action-buttons">
                        <button @click="openEditModal(patient)"
                                class="btn-edit"
                                title="Редактировать">
                          ✏️
                        </button>
                        <button @click="showDeleteConfirm(patient)"
                                class="btn-delete"
                                title="Удалить">
                          🗑️
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Пагинация -->
            <div class="pagination" v-if="filteredPatients.length > itemsPerPage">
              <button @click="prevPage"
                      :disabled="currentPage === 1"
                      class="pagination-btn">
                ←
              </button>
              <span class="page-info">
                Страница {{ currentPage }} из {{ totalPages }}
              </span>
              <button @click="nextPage"
                      :disabled="currentPage === totalPages"
                      class="pagination-btn">
                →
              </button>
              <span class="total-info">
                Всего пациентов: {{ filteredPatients.length }}
              </span>
            </div>

            <!-- Сообщение если нет пациентов -->
            <div v-if="filteredPatients.length === 0" class="empty-table">
              <p>Пациенты не найдены</p>
            </div>
          </div>

          <!-- Загрузка -->
          <div v-else class="loading-state">
            <p>Загрузка пациентов...</p>
          </div>
        </div>

        <!-- Модальное окно создания/редактирования пациента -->
        <div v-if="showPatientModal" class="modal-overlay" @click.self="closePatientModal">
          <div class="modal-content">
            <div class="modal-header">
              <h3>
                {{ modalMode === 'create' ? 'Создание нового пациента' : 'Редактирование пациента' }}
              </h3>
              <button class="modal-close" @click="closePatientModal">×</button>
            </div>
            <div class="modal-body">
              <form @submit.prevent="savePatient">
                <div class="form-grid">
                  <div class="form-group">
                    <label>ФИО *</label>
                    <input v-model="currentPatient.patientFio"
                           type="text"
                           required
                           placeholder="Иванов Иван Иванович">
                  </div>
                  <div class="form-group">
                    <label>Дата рождения *</label>
                    <input v-model="currentPatient.patientBirthdate"
                           type="date"
                           required>
                  </div>
                  <div class="form-group">
                    <label>Пол *</label>
                    <select v-model="currentPatient.gender" required>
                      <option value="">Выберите пол</option>
                      <option value="М">Мужской</option>
                      <option value="Ж">Женский</option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label>Телефон *</label>
                    <input v-model="currentPatient.contactPhone"
                           type="tel"
                           required
                           placeholder="+7 (999) 999-99-99">
                  </div>
                  <div class="form-group">
                    <label>OMS полис</label>
                    <input v-model="currentPatient.omsPolisNumber"
                           type="number"
                           placeholder="123456789012345">
                  </div>
                  <div class="form-group">
                    <label>Дата регистрации</label>
                    <input v-model="currentPatient.registrationDate"
                           type="date"
                           :disabled="modalMode === 'edit'">
                  </div>
                  <div class="form-group" v-if="modalMode === 'edit'">
                    <label>Статус</label>
                    <select v-model="currentPatient.isActive">
                      <option :value="true">Активен</option>
                      <option :value="false">Не активен</option>
                    </select>
                  </div>
                </div>

                <div class="modal-footer">
                  <button type="submit" class="btn-primary" :disabled="saving">
                    {{ saving ? 'Сохранение...' : 'Сохранить' }}
                  </button>
                  <button type="button" class="btn-secondary" @click="closePatientModal">
                    Отмена
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>

        <!-- Модальное окно подтверждения удаления -->
        <div v-if="showDeleteModal" class="modal-overlay" @click.self="closeDeleteModal">
          <div class="modal-content delete-modal">
            <div class="modal-header">
              <h3>Подтверждение удаления</h3>
              <button class="modal-close" @click="closeDeleteModal">×</button>
            </div>
            <div class="modal-body">
              <p>Вы уверены, что хотите удалить пациента <strong>{{ patientToDelete?.patientFio }}</strong>?</p>
              <p class="warning-text">Это действие нельзя отменить!</p>
            </div>
            <div class="modal-footer">
              <button @click="deletePatient" class="btn-delete" :disabled="deleting">
                {{ deleting ? 'Удаление...' : 'Удалить' }}
              </button>
              <button @click="closeDeleteModal" class="btn-secondary">Отмена</button>
            </div>
          </div>
        </div>

        <!-- Вкладка Врачи -->
        <div v-else-if="activeTab === 'doctors'" class="tab-content">
          <div class="doctors-header">
            <h2>👨‍⚕️ Управление врачами</h2>
            <div class="doctors-actions">
              <div class="search-box">
                <input v-model="doctorSearchQuery"
                       type="text"
                       placeholder="Поиск врачей..."
                       @input="filterDoctors">
                <span class="search-icon">🔍</span>
              </div>
            </div>
          </div>

          <!-- Таблица врачей -->
          <div class="doctors-table-container" v-if="!loadingDoctors">
            <div class="table-responsive">
              <table class="doctors-table">
                <thead>
                  <tr>
                    <th @click="sortDoctors('employeeFio')">
                      ФИО
                      <span v-if="doctorSortField === 'employeeFio'" class="sort-icon">
                        {{ doctorSortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th @click="sortDoctors('post')">
                      Должность
                      <span v-if="doctorSortField === 'post'" class="sort-icon">
                        {{ doctorSortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th>Специализация</th>
                    <th @click="sortDoctors('experience')">
                      Опыт (лет)
                      <span v-if="doctorSortField === 'experience'" class="sort-icon">
                        {{ doctorSortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th>Телефон</th>
                    <th>Email</th>
                    <th>Действия</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="doctor in paginatedDoctors" :key="doctor.employeeId">
                    <td>
                      <div class="doctor-name">
                        <strong>{{ doctor.employeeFio }}</strong>
                      </div>
                    </td>
                    <td>{{ doctor.post }}</td>
                    <td>{{ doctor.specialization }}</td>
                    <td>
                      <span class="experience-badge">{{ doctor.experience }} лет</span>
                    </td>
                    <td>{{ doctor.phone || '-' }}</td>
                    <td>{{ doctor.email || '-' }}</td>
                    <td>
                      <div class="action-buttons">
                        <button @click="openDoctorEditModal(doctor)"
                                class="btn-edit"
                                title="Редактировать данные врача">
                          ✏️
                        </button>
                        <button @click="openScheduleModal(doctor)"
                                class="btn-view"
                                title="Управление расписанием">
                          📅
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Пагинация -->
            <div class="pagination" v-if="filteredDoctors.length > doctorItemsPerPage">
              <button @click="prevDoctorPage"
                      :disabled="doctorCurrentPage === 1"
                      class="pagination-btn">
                ←
              </button>
              <span class="page-info">
                Страница {{ doctorCurrentPage }} из {{ doctorTotalPages }}
              </span>
              <button @click="nextDoctorPage"
                      :disabled="doctorCurrentPage === doctorTotalPages"
                      class="pagination-btn">
                →
              </button>
              <span class="total-info">
                Всего врачей: {{ filteredDoctors.length }}
              </span>
            </div>

            <!-- Сообщение если нет врачей -->
            <div v-if="filteredDoctors.length === 0" class="empty-table">
              <p>Врачи не найдены</p>
            </div>
          </div>

          <!-- Загрузка -->
          <div v-else class="loading-state">
            <p>Загрузка врачей...</p>
          </div>
        </div>

        <!-- Модальное окно редактирования врача -->
        <div v-if="showDoctorModal" class="modal-overlay" @click.self="closeDoctorModal">
          <div class="modal-content">
            <div class="modal-header">
              <h3>Редактирование врача</h3>
              <button class="modal-close" @click="closeDoctorModal">×</button>
            </div>
            <div class="modal-body">
              <form @submit.prevent="saveDoctor">
                <div class="form-grid">
                  <div class="form-group">
                    <label>ФИО врача *</label>
                    <input v-model="currentDoctor.employeeFio"
                           type="text"
                           required
                           placeholder="Иванов Иван Иванович">
                  </div>
                  <div class="form-group">
                    <label>Должность *</label>
                    <input v-model="currentDoctor.post"
                           type="text"
                           required
                           placeholder="Терапевт">
                  </div>
                  <div class="form-group">
                    <label>Специализация *</label>
                    <input v-model="currentDoctor.specialization"
                           type="text"
                           required
                           placeholder="Общая терапия">
                  </div>
                  <div class="form-group">
                    <label>Опыт работы (лет) *</label>
                    <input v-model.number="currentDoctor.experience"
                           type="number"
                           required
                           min="0"
                           max="50">
                  </div>
                  <div class="form-group">
                    <label>Телефон</label>
                    <input v-model="currentDoctor.phone"
                           type="tel"
                           placeholder="+7 (999) 999-99-99">
                  </div>
                  <div class="form-group">
                    <label>Email</label>
                    <input v-model="currentDoctor.email"
                           type="email"
                           placeholder="doctor@clinic.ru">
                  </div>
                </div>

                <div class="modal-footer">
                  <button type="submit" class="btn-primary" :disabled="savingDoctor">
                    {{ savingDoctor ? 'Сохранение...' : 'Сохранить' }}
                  </button>
                  <button type="button" class="btn-secondary" @click="closeDoctorModal">
                    Отмена
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>

        <!-- Модальное окно расписания врача -->
        <div v-if="showScheduleModal" class="modal-overlay" @click.self="closeScheduleModal">
          <div class="modal-content schedule-modal">
            <div class="modal-header">
              <h3>Расписание врача: {{ selectedDoctor?.employeeFio }}</h3>
              <button class="modal-close" @click="closeScheduleModal">×</button>
            </div>
            <div class="modal-body">
              <div class="schedule-form">
                <h4>Добавить расписание на период:</h4>
                <div class="form-grid">
                  <div class="form-group">
                    <label>Кабинет *</label>
                    <input v-model.number="scheduleData.cabinetNumber"
                           type="number"
                           required
                           min="1"
                           max="999"
                           placeholder="Номер кабинета">
                  </div>
                  <div class="form-group">
                    <label>Дата начала *</label>
                    <input v-model="scheduleData.startDate"
                           type="date"
                           required>
                  </div>
                  <div class="form-group">
                    <label>Дата окончания *</label>
                    <input v-model="scheduleData.endDate"
                           type="date"
                           required>
                  </div>
                </div>

                <!-- Кнопка добавления расписания -->
                <div class="schedule-action">
                  <button @click="createDoctorSchedule" class="btn-primary" :disabled="savingSchedule">
                    {{ savingSchedule ? 'Создание...' : 'Добавить расписание' }}
                  </button>
                </div>

                <!-- Информация о текущем расписании -->
                <div class="schedule-info">
                  <p><strong>Примечание:</strong> Расписание будет создано на указанный период времени.</p>
                  <p>Врач будет доступен для записи пациентов в рабочие часы (обычно 9:00-18:00).</p>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn-secondary" @click="closeScheduleModal">
                Закрыть
              </button>
            </div>
          </div>
        </div>
        <!-- Вкладка Платежи -->
        <div v-else-if="activeTab === 'payments'" class="tab-content">
          <div class="payments-header">
            <h2>💳 Управление платежами</h2>
            <div class="payments-actions">
              <div class="search-box">
                <input v-model="paymentSearchQuery"
                       type="text"
                       placeholder="Поиск по ФИО пациента..."
                       @input="filterPayments">
                <span class="search-icon">🔍</span>
              </div>
              <div class="filters">
                <select v-model="paymentStatusFilter" @change="filterPayments">
                  <option value="">Все статусы</option>
                  <option value="Waiting">Ожидание</option>
                  <option value="Paied">Оплачено</option>
                  <option value="Cancelled">Отменено</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Таблица платежей -->
          <div class="payments-table-container" v-if="!loadingPayments">
            <div class="table-responsive">
              <table class="payments-table">
                <thead>
                  <tr>
                    <th @click="sortPayments('paymentDate')">
                      Дата платежа
                      <span v-if="paymentSortField === 'paymentDate'" class="sort-icon">
                        {{ paymentSortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th @click="sortPayments('patientFio')">
                      Пациент
                      <span v-if="paymentSortField === 'patientFio'" class="sort-icon">
                        {{ paymentSortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th @click="sortPayments('summaryPrice')">
                      Сумма
                      <span v-if="paymentSortField === 'summaryPrice'" class="sort-icon">
                        {{ paymentSortDirection === 'asc' ? '↑' : '↓' }}
                      </span>
                    </th>
                    <th>Статус</th>
                    <th>Действия</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="payment in paginatedPayments" :key="payment.paymentId">
                    <td>
                      <div class="payment-date">
                        <strong>{{ formatDate(payment.paymentDate) }}</strong>
                        <small>{{ formatTime(payment.paymentDate) }}</small>
                      </div>
                    </td>
                    <td>
                      <div class="patient-info">
                        <strong>{{ payment.patientFio }}</strong>
                        <small>ID: {{ payment.paymentId.substring(0, 8) }}...</small>
                      </div>
                    </td>
                    <td>
                      <div class="payment-amount">
                        <strong>{{ formatCurrency(payment.summaryPrice) }}</strong>
                      </div>
                    </td>
                    <td>
                      <span class="status-badge" :class="getStatusClass(payment.status)">
                        {{ getStatusText(payment.status) }}
                      </span>
                    </td>
                    <td>
                      <div class="action-buttons">
                        <select v-model="payment.status"
                                @change="updatePaymentStatus(payment)"
                                :disabled="payment.status === 'Cancelled'"
                                class="status-select">
                          <option value="Waiting">Ожидание</option>
                          <option value="Paied">Оплачено</option>
                          <option value="Cancelled">Отменено</option>
                        </select>
                        <button @click="viewPaymentDetails(payment)"
                                class="btn-view"
                                title="Детали платежа">
                          👁️
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Статистика -->
            <div class="payment-stats">
              <div class="stat-item">
                <span class="stat-label">Всего платежей:</span>
                <span class="stat-value">{{ filteredPayments.length }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Общая сумма:</span>
                <span class="stat-value">{{ formatCurrency(totalAmount) }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Ожидают оплаты:</span>
                <span class="stat-value">{{ waitingCount }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Оплачено:</span>
                <span class="stat-value">{{ paidCount }}</span>
              </div>
            </div>

            <!-- Пагинация -->
            <div class="pagination" v-if="filteredPayments.length > paymentItemsPerPage">
              <button @click="prevPaymentPage"
                      :disabled="paymentCurrentPage === 1"
                      class="pagination-btn">
                ←
              </button>
              <span class="page-info">
                Страница {{ paymentCurrentPage }} из {{ paymentTotalPages }}
              </span>
              <button @click="nextPaymentPage"
                      :disabled="paymentCurrentPage === paymentTotalPages"
                      class="pagination-btn">
                →
              </button>
              <span class="total-info">
                Всего платежей: {{ filteredPayments.length }}
              </span>
            </div>

            <!-- Сообщение если нет платежей -->
            <div v-if="filteredPayments.length === 0" class="empty-table">
              <p>Платежи не найдены</p>
            </div>
          </div>

          <!-- Загрузка -->
          <div v-else class="loading-state">
            <p>Загрузка платежей...</p>
          </div>
        </div>

        <!-- Модальное окно деталей платежа -->
        <div v-if="showPaymentModal" class="modal-overlay" @click.self="closePaymentModal">
          <div class="modal-content">
            <div class="modal-header">
              <h3>Детали платежа</h3>
              <button class="modal-close" @click="closePaymentModal">×</button>
            </div>
            <div class="modal-body">
              <div v-if="selectedPayment" class="payment-details">
                <div class="detail-row">
                  <span class="detail-label">ID платежа:</span>
                  <span class="detail-value">{{ selectedPayment.paymentId }}</span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">Пациент:</span>
                  <span class="detail-value">{{ selectedPayment.patientFio }}</span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">Дата и время:</span>
                  <span class="detail-value">{{ formatDateTime(selectedPayment.paymentDate) }}</span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">Сумма:</span>
                  <span class="detail-value amount">{{ formatCurrency(selectedPayment.summaryPrice) }}</span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">Статус:</span>
                  <span class="detail-value">
                    <span class="status-badge" :class="getStatusClass(selectedPayment.status)">
                      {{ getStatusText(selectedPayment.status) }}
                    </span>
                  </span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">История изменений:</span>
                  <span class="detail-value">
                    <div v-if="paymentHistory.length > 0" class="history-list">
                      <div v-for="history in paymentHistory" :key="history.date" class="history-item">
                        <span>{{ formatDateTime(history.date) }}</span>
                        <span class="history-status" :class="getStatusClass(history.status)">
                          {{ history.status }}
                        </span>
                        <span v-if="history.user">({{ history.user }})</span>
                      </div>
                    </div>
                    <span v-else>Нет истории изменений</span>
                  </span>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn-secondary" @click="closePaymentModal">
                Закрыть
              </button>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Футер -->
    <footer class="admin-footer">
      <p>© {{ currentYear }} Медицинская клиника. Админ-панель v1.0</p>
      <p>Текущее время: {{ currentTime }}</p>
    </footer>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed, watch, onUnmounted } from 'vue';
  import { useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';
  import { adminService } from '@/services/admin.service';
  import * as patientService from '@/services/api.js'; // Используем методы из api.js
  import { doctorService } from '@/services/doctor.service.js';
  const router = useRouter();
  const authStore = useAuthStore();


  // Состояние
  const activeTab = ref('dashboard');
  const currentTime = ref(new Date().toLocaleTimeString('ru-RU'));
  const stats = ref({
    patients: 0,
    doctors: 0,
    appointments: 0,
    revenue: 0
  });

  // Состояние для управления пациентами
  const patients = ref([]);
  const filteredPatients = ref([]);
  const currentPatient = ref(null);
  const showPatientModal = ref(false);
  const modalMode = ref('create'); // 'create', 'edit'
  const loading = ref(false);
  const saving = ref(false);
  const searchQuery = ref('');
  const sortField = ref('patientFio');
  const sortDirection = ref('asc');
  const currentPage = ref(1);
  const itemsPerPage = 10;

  const doctors = ref([]);
  const filteredDoctors = ref([]);
  const currentDoctor = ref(null);
  const showDoctorModal = ref(false);
  const doctorModalMode = ref('create');
  const loadingDoctors = ref(false);
  const savingDoctor = ref(false);
  const doctorSearchQuery = ref('');
  const doctorSortField = ref('employeeFio');
  const doctorSortDirection = ref('asc');
  const doctorCurrentPage = ref(1);
  const doctorItemsPerPage = 10;

  const showScheduleModal = ref(false);
  const selectedDoctor = ref(null);
  const scheduleData = ref({
    doctorId: '',
    cabinetNumber: 1,
    startDate: '',
    endDate: ''
  });

  const payments = ref([]);
  const filteredPayments = ref([]);
  const loadingPayments = ref(false);
  const paymentSearchQuery = ref('');
  const paymentStatusFilter = ref('');
  const paymentSortField = ref('paymentDate');
  const paymentSortDirection = ref('desc');
  const paymentCurrentPage = ref(1);
  const paymentItemsPerPage = 10;

  const showPaymentModal = ref(false);
  const selectedPayment = ref(null);
  const paymentHistory = ref([]);

  const savingSchedule = ref(false);
  const doctorSchedules = ref([]);

  const showDeleteModal = ref(false);
  const deleting = ref(false);
  const patientToDelete = ref(null);

  const paymentTotalPages = computed(() => {
    return Math.ceil(filteredPayments.value.length / paymentItemsPerPage);
  });

  const paginatedPayments = computed(() => {
    const start = (paymentCurrentPage.value - 1) * paymentItemsPerPage;
    const end = start + paymentItemsPerPage;
    return filteredPayments.value.slice(start, end);
  });

  const totalAmount = computed(() => {
    return filteredPayments.value.reduce((sum, payment) => sum + payment.summaryPrice, 0);
  });

  const waitingCount = computed(() => {
    return filteredPayments.value.filter(p => p.status === 'Waiting').length;
  });

  const paidCount = computed(() => {
    return filteredPayments.value.filter(p => p.status === 'Paied').length;
  });

  // Методы для платежей
  const loadPayments = async () => {
    loadingPayments.value = true;
    try {
      // Используем метод getAllPayments
      const data = await patientService.getAllPayments();
      payments.value = data;
      filteredPayments.value = [...data];
      sortPayments('paymentDate');
    } catch (error) {
      console.error('Ошибка загрузки платежей:', error);
      payments.value = [];
      filteredPayments.value = [];
    } finally {
      loadingPayments.value = false;
    }
  };

  const filterPayments = () => {
    let filtered = [...payments.value];

    // Фильтр по поиску
    if (paymentSearchQuery.value.trim()) {
      const query = paymentSearchQuery.value.toLowerCase();
      filtered = filtered.filter(payment =>
        payment.patientFio?.toLowerCase().includes(query)
      );
    }

    // Фильтр по статусу
    if (paymentStatusFilter.value) {
      filtered = filtered.filter(payment => payment.status === paymentStatusFilter.value);
    }

    filteredPayments.value = filtered;
    paymentCurrentPage.value = 1;
    sortPayments(paymentSortField.value);
  };

  const sortPayments = (field) => {
    if (paymentSortField.value === field) {
      paymentSortDirection.value = paymentSortDirection.value === 'asc' ? 'desc' : 'asc';
    } else {
      paymentSortField.value = field;
      paymentSortDirection.value = field === 'paymentDate' ? 'desc' : 'asc';
    }

    filteredPayments.value.sort((a, b) => {
      let aValue = a[field];
      let bValue = b[field];

      // Для дат
      if (field === 'paymentDate') {
        aValue = new Date(aValue).getTime();
        bValue = new Date(bValue).getTime();
      }

      // Для чисел
      if (field === 'summaryPrice') {
        aValue = aValue || 0;
        bValue = bValue || 0;
      }

      if (aValue < bValue) return paymentSortDirection.value === 'asc' ? -1 : 1;
      if (aValue > bValue) return paymentSortDirection.value === 'asc' ? 1 : -1;
      return 0;
    });
  };

  const updatePaymentStatus = async (payment) => {
    try {
      // Используем метод updatePaymentStatus
      await patientService.updatePaymentStatus(payment.paymentId, payment.status);

      // Обновляем локальные данные
      const index = payments.value.findIndex(p => p.paymentId === payment.paymentId);
      if (index !== -1) {
        payments.value[index].status = payment.status;
      }

      // Обновляем отфильтрованный список
      filterPayments();

      // Показываем уведомление
      alert(`Статус платежа изменен на: ${getStatusText(payment.status)}`);
    } catch (error) {
      console.error('Ошибка обновления статуса:', error);
      alert('Ошибка обновления статуса: ' + error.message);

      // Восстанавливаем предыдущее значение
      await loadPayments();
    }
  };

  const viewPaymentDetails = async (payment) => {
    selectedPayment.value = payment;
    showPaymentModal.value = true;

    // Загружаем историю платежа (если есть такой метод)
    // paymentHistory.value = await patientService.getPaymentHistory(payment.paymentId);

    // Временные тестовые данные
    paymentHistory.value = [
      { date: new Date(payment.paymentDate), status: payment.status, user: 'Система' }
    ];
  };

  const closePaymentModal = () => {
    showPaymentModal.value = false;
    selectedPayment.value = null;
    paymentHistory.value = [];
  };

  const nextPaymentPage = () => {
    if (paymentCurrentPage.value < paymentTotalPages.value) {
      paymentCurrentPage.value++;
    }
  };

  const prevPaymentPage = () => {
    if (paymentCurrentPage.value > 1) {
      paymentCurrentPage.value--;
    }
  };

  // Вспомогательные методы
  const getStatusText = (status) => {
    const statusMap = {
      'Waiting': 'Ожидание',
      'Paied': 'Оплачено',
      'Cancelled': 'Отменено'
    };
    return statusMap[status] || status;
  };

  const getStatusClass = (status) => {
    const classMap = {
      'Waiting': 'Waiting',
      'Paied': 'Paied',
      'Cancelled': 'Cancelled'
    };
    return classMap[status] || '';
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('ru-RU', {
      style: 'currency',
      currency: 'RUB',
      minimumFractionDigits: 0
    }).format(amount);
  };

  const formatDate = (dateString) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  const formatTime = (dateString) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleTimeString('ru-RU', {
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const formatDateTime = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleString('ru-RU');
  };

  const doctorTotalPages = computed(() => {
    return Math.ceil(filteredDoctors.value.length / doctorItemsPerPage);
  });

  const paginatedDoctors = computed(() => {
    const start = (doctorCurrentPage.value - 1) * doctorItemsPerPage;
    const end = start + doctorItemsPerPage;
    return filteredDoctors.value.slice(start, end);
  });

  // Методы для врачей
  const loadDoctors = async () => {
    loadingDoctors.value = true;
    try {
      // Используем метод getAllDoctors из doctorService
      const data = await doctorService.getAllDoctors();
      doctors.value = data;
      filteredDoctors.value = [...data];
      sortDoctors('employeeFio');
    } catch (error) {
      console.error('Ошибка загрузки врачей:', error);
      doctors.value = [];
      filteredDoctors.value = [];
    } finally {
      loadingDoctors.value = false;
    }
  };

  const filterDoctors = () => {
    if (!doctorSearchQuery.value.trim()) {
      filteredDoctors.value = [...doctors.value];
    } else {
      const query = doctorSearchQuery.value.toLowerCase();
      filteredDoctors.value = doctors.value.filter(doctor =>
        doctor.employeeFio?.toLowerCase().includes(query) ||
        doctor.post?.toLowerCase().includes(query) ||
        doctor.specialization?.toLowerCase().includes(query) ||
        doctor.phone?.toLowerCase().includes(query) ||
        doctor.email?.toLowerCase().includes(query)
      );
    }
    doctorCurrentPage.value = 1;
    sortDoctors(doctorSortField.value);
  };

  const sortDoctors = (field) => {
    if (doctorSortField.value === field) {
      doctorSortDirection.value = doctorSortDirection.value === 'asc' ? 'desc' : 'asc';
    } else {
      doctorSortField.value = field;
      doctorSortDirection.value = 'asc';
    }

    filteredDoctors.value.sort((a, b) => {
      let aValue = a[field];
      let bValue = b[field];

      if (field === 'experience') {
        aValue = aValue || 0;
        bValue = bValue || 0;
      }

      if (aValue < bValue) return doctorSortDirection.value === 'asc' ? -1 : 1;
      if (aValue > bValue) return doctorSortDirection.value === 'asc' ? 1 : -1;
      return 0;
    });
  };

  const openDoctorEditModal = (doctor) => {
    currentDoctor.value = { ...doctor };
    showDoctorModal.value = true;
  };

  const saveDoctor = async () => {
    savingDoctor.value = true;
    try {
      // Используем метод updateDoctor из doctorService
      await doctorService.updateDoctor(currentDoctor.value);
      await loadDoctors(); // Перезагружаем список
      closeDoctorModal();
    } catch (error) {
      console.error('Ошибка сохранения врача:', error);
      alert('Ошибка сохранения врача: ' + error.message);
    } finally {
      savingDoctor.value = false;
    }
  };

  const closeDoctorModal = () => {
    showDoctorModal.value = false;
    currentDoctor.value = null;
  };

  const nextDoctorPage = () => {
    if (doctorCurrentPage.value < doctorTotalPages.value) {
      doctorCurrentPage.value++;
    }
  };

  const prevDoctorPage = () => {
    if (doctorCurrentPage.value > 1) {
      doctorCurrentPage.value--;
    }
  };

  // Методы для расписания
  const openScheduleModal = (doctor) => {
    selectedDoctor.value = doctor;
    scheduleData.value = {
      doctorId: doctor.employeeId,
      cabinetNumber: 1,
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
    };
    showScheduleModal.value = true;
  };

  const closeScheduleModal = () => {
    showScheduleModal.value = false;
    selectedDoctor.value = null;
    scheduleData.value = {
      doctorId: '',
      cabinetNumber: 1,
      startDate: '',
      endDate: ''
    };
  };

  const createDoctorSchedule = async () => {
    if (!scheduleData.value.doctorId || !scheduleData.value.cabinetNumber ||
      !scheduleData.value.startDate || !scheduleData.value.endDate) {
      alert('Заполните все обязательные поля');
      return;
    }

    // Проверка дат
    const startDate = new Date(scheduleData.value.startDate);
    const endDate = new Date(scheduleData.value.endDate);

    if (endDate < startDate) {
      alert('Дата окончания не может быть раньше даты начала');
      return;
    }

    savingSchedule.value = true;
    try {
      // Используем метод createDoctorSchedule из doctorService
      const result = await doctorService.createDoctorSchedule(scheduleData.value);

      if (result) {
        alert('Расписание успешно добавлено');
        closeScheduleModal();
      }
    } catch (error) {
      console.error('Ошибка создания расписания:', error);
      alert('Ошибка создания расписания: ' + error.message);
    } finally {
      savingSchedule.value = false;
    }
  };

  const loadDashboardData = async () => {
    try {
      const doctorsCount = await adminService.getDoctorsCount();
      stats.value.doctors = doctorsCount;
    } catch (error) {
      console.error('Ошибка загрузки врачей:', error);
      stats.value.doctors = 0;
    }

    try {
      const patientsCount = await adminService.getPatientsCount();
      stats.value.patients = patientsCount;
    } catch (error) {
      console.error('Ошибка загрузки пациентов:', error);
      stats.value.patients = 0;
    }

    try {
      const appointmentsCount = await adminService.getTodayAppointmentsCount();
      stats.value.appointments = appointmentsCount;
    } catch (error) {
      console.error('Ошибка загрузки записей:', error);
      stats.value.appointments = 0;
    }

    try {
      const revenue = await adminService.getRevenue();
      stats.value.revenue = revenue;
    } catch (error) {
      console.error('Ошибка загрузки выручки:', error);
      stats.value.revenue = 0;
    }
  };

  // Добавьте эти методы
  const showDeleteConfirm = (patient) => {
    patientToDelete.value = patient;
    showDeleteModal.value = true;
  };

  const deletePatient = async () => {
    if (!patientToDelete.value) return;

    deleting.value = true;
    try {
      // Используем метод deleteFromPatient из api.js
      console.log(patientToDelete.value.patientId);
      await patientService.deleteFromPatient(patientToDelete.value.patientId);

      // Обновляем список пациентов
      await loadPatients();
      closeDeleteModal();
    } catch (error) {
      console.error('Ошибка удаления пациента:', error);
      alert('Ошибка удаления пациента: ' + error.message);
    } finally {
      deleting.value = false;
    }
  };

  const closeDeleteModal = () => {
    showDeleteModal.value = false;
    patientToDelete.value = null;
  };

  let timeInterval;

  // Вычисляемые свойства
  const userName = computed(() => authStore.userName || 'Администратор');
  const currentYear = computed(() => new Date().getFullYear());
  const appVersion = computed(() => import.meta.env.VITE_APP_VERSION || '1.0.0');
  const appEnv = computed(() => import.meta.env.VITE_APP_ENV || 'development');
  const formattedRevenue = computed(() => {
    return new Intl.NumberFormat('ru-RU', {
      style: 'currency',
      currency: 'RUB',
      minimumFractionDigits: 0
    }).format(stats.value.revenue);
  });

  const totalPages = computed(() => {
    return Math.ceil(filteredPatients.value.length / itemsPerPage);
  });

  const paginatedPatients = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage;
    const end = start + itemsPerPage;
    return filteredPatients.value.slice(start, end);
  });

  // Методы
  const setActiveTab = async (tab) => {
    activeTab.value = tab;
    if (tab === 'patients') {
      await loadPatients();
    }
    else if (tab === 'doctors') {
      await loadDoctors();
    }
    else if (tab === 'payments') {
      await loadPayments();
    }
  };

  const logout = () => {
    authStore.logout();
  };

  const updateTime = () => {
    currentTime.value = new Date().toLocaleTimeString('ru-RU');
  };

  const loadPatients = async () => {
    loading.value = true;
    try {
      // Используем метод getAllPatients из api.js
      const data = await patientService.getAllPatients();
      patients.value = data;
      filteredPatients.value = [...data];
      sortPatients('patientFio');
    } catch (error) {
      console.error('Ошибка загрузки пациентов:', error);
      patients.value = [];
      filteredPatients.value = [];
    } finally {
      loading.value = false;
    }
  };

  const filterPatients = () => {
    if (!searchQuery.value.trim()) {
      filteredPatients.value = [...patients.value];
    } else {
      const query = searchQuery.value.toLowerCase();
      filteredPatients.value = patients.value.filter(patient =>
        patient.patientFio.toLowerCase().includes(query) ||
        patient.contactPhone.toLowerCase().includes(query) ||
        (patient.omsPolisNumber && patient.omsPolisNumber.toString().includes(query))
      );
    }
    currentPage.value = 1;
    sortPatients(sortField.value);
  };

  const sortPatients = (field) => {
    if (sortField.value === field) {
      sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
    } else {
      sortField.value = field;
      sortDirection.value = 'asc';
    }

    filteredPatients.value.sort((a, b) => {
      let aValue = a[field];
      let bValue = b[field];

      // Для дат преобразуем в timestamp
      if (field.includes('date') || field.includes('Date') || field.includes('Birthdate')) {
        aValue = new Date(aValue).getTime();
        bValue = new Date(bValue).getTime();
      }

      // Для числовых значений
      if (field === 'omsPolisNumber') {
        aValue = aValue || 0;
        bValue = bValue || 0;
      }

      if (aValue < bValue) return sortDirection.value === 'asc' ? -1 : 1;
      if (aValue > bValue) return sortDirection.value === 'asc' ? 1 : -1;
      return 0;
    });
  };

  const openCreateModal = () => {
    currentPatient.value = {
      patientId: '00000000-0000-0000-0000-000000000000',
      patientBirthdate: new Date().toISOString().split('T')[0],
      gender: 'Male',
      patientFio: '',
      contactPhone: '',
      omsPolisNumber: null,
      registrationDate: new Date().toISOString().split('T')[0],
      isActive: true
    };
    modalMode.value = 'create';
    showPatientModal.value = true;
  };

  const openEditModal = (patient) => {
    currentPatient.value = {
      ...patient,
      patientBirthdate: formatDateForInput(patient.patientBirthdate),
      registrationDate: formatDateForInput(patient.registrationDate)
    };
    modalMode.value = 'edit';
    showPatientModal.value = true;
  };

  const savePatient = async () => {
    saving.value = true;
    try {
      // Подготавливаем данные согласно PatientDto
      const patientData = {
        patientId: currentPatient.value.patientId,
        patientBirthdate: currentPatient.value.patientBirthdate,
        gender: currentPatient.value.gender,
        patientFio: currentPatient.value.patientFio,
        contactPhone: currentPatient.value.contactPhone,
        omsPolisNumber: currentPatient.value.omsPolisNumber,
        registrationDate: currentPatient.value.registrationDate,
        isActive: currentPatient.value.isActive
      };

      if (modalMode.value === 'create') {
        // Используем метод createPatient из api.js
        const result = await patientService.createPatient(patientData);
        if (result.success) {
          await loadPatients();
          closePatientModal();
        }
      } else if (modalMode.value === 'edit') {
        // Используем метод updatePatientProfile из api.js
        await patientService.updatePatientProfile(patientData);
        await loadPatients();
        closePatientModal();
      }
    } catch (error) {
      console.error('Ошибка сохранения пациента:', error);
      alert('Ошибка сохранения пациента: ' + error.message);
    } finally {
      saving.value = false;
    }
  };

  const closePatientModal = () => {
    showPatientModal.value = false;
    currentPatient.value = null;
  };

  const nextPage = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++;
    }
  };

  const prevPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--;
    }
  };

  //const formatDate = (dateString) => {
  //  if (!dateString) return '-';
  //  const date = new Date(dateString);
  //  return date.toLocaleDateString('ru-RU');
  //};

  const formatDateForInput = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toISOString().split('T')[0];
  };

  // Хуки жизненного цикла
  onMounted(() => {
    // Обновляем время каждую секунду
    timeInterval = setInterval(updateTime, 1000);

    // Проверяем права доступа
    if (!authStore.isAdmin) {
      router.push('/unauthorized');
    }

    loadDashboardData();
  });

  // Очистка интервала
  onUnmounted(() => {
    if (timeInterval) {
      clearInterval(timeInterval);
    }
  });

  // Наблюдаем за изменением активной вкладки
  watch(activeTab, (newTab) => {
    if (newTab === 'patients') {
      loadPatients();
    }
  });
</script>

<style scoped>
  .admin-dashboard {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    background-color: #f5f7fa;
  }

  .admin-header {
    background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
    color: white;
    padding: 1rem 2rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  }

  .header-left h1 {
    margin: 0;
    font-size: 1.8rem;
  }

  .subtitle {
    margin: 0.25rem 0 0;
    opacity: 0.9;
    font-size: 0.9rem;
  }

  .header-right {
    display: flex;
    align-items: center;
    gap: 1.5rem;
  }

  .user-info {
    display: flex;
    flex-direction: column;
    align-items: flex-end;
  }

  .username {
    font-weight: 600;
    font-size: 1.1rem;
  }

  .role-badge {
    background: rgba(255, 255, 255, 0.2);
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.85rem;
    margin-top: 0.25rem;
  }

  .logout-btn {
    background: rgba(255, 255, 255, 0.1);
    border: 1px solid rgba(255, 255, 255, 0.3);
    color: white;
    padding: 0.5rem 1.5rem;
    border-radius: 6px;
    cursor: pointer;
    transition: all 0.3s;
  }

    .logout-btn:hover {
      background: rgba(255, 255, 255, 0.2);
    }

  .admin-content {
    display: flex;
    flex: 1;
  }

  .sidebar {
    width: 250px;
    background: white;
    border-right: 1px solid #e1e5eb;
    padding: 1.5rem;
    display: flex;
    flex-direction: column;
  }

  .admin-nav h3 {
    margin-top: 0;
    color: #2a5298;
    font-size: 1.2rem;
    margin-bottom: 1.5rem;
  }

  .admin-nav ul {
    list-style: none;
    padding: 0;
    margin: 0;
  }

  .admin-nav li {
    margin-bottom: 0.5rem;
  }

  .admin-nav a {
    display: block;
    padding: 0.75rem 1rem;
    color: #4a5568;
    text-decoration: none;
    border-radius: 8px;
    transition: all 0.3s;
  }

    .admin-nav a:hover {
      background: #edf2f7;
      color: #2a5298;
    }

    .admin-nav a.active {
      background: #2a5298;
      color: white;
    }

  .sidebar-footer {
    margin-top: auto;
    padding-top: 1.5rem;
    border-top: 1px solid #e1e5eb;
    font-size: 0.85rem;
    color: #718096;
  }

    .sidebar-footer p {
      margin: 0.25rem 0;
    }

  .content-area {
    flex: 1;
    padding: 2rem;
    overflow-y: auto;
  }

  .tab-content {
    background: white;
    border-radius: 12px;
    padding: 2rem;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
    margin-bottom: 2rem;
  }

  .patients-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
  }

  .patients-actions {
    display: flex;
    gap: 1rem;
    align-items: center;
  }

  .search-box {
    position: relative;
  }

    .search-box input {
      padding: 0.5rem 1rem 0.5rem 2.5rem;
      border: 1px solid #e1e5eb;
      border-radius: 6px;
      width: 250px;
    }

  .search-icon {
    position: absolute;
    left: 0.75rem;
    top: 50%;
    transform: translateY(-50%);
  }

  .patients-table {
    width: 100%;
    border-collapse: collapse;
  }

    .patients-table th {
      text-align: left;
      padding: 1rem;
      border-bottom: 2px solid #e1e5eb;
      background: #f8fafc;
      cursor: pointer;
      user-select: none;
    }

    .patients-table td {
      padding: 1rem;
      border-bottom: 1px solid #e1e5eb;
    }

  .gender-badge {
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.85rem;
  }

    .gender-badge.male {
      background: #dbeafe;
      color: #1e40af;
    }

    .gender-badge.female {
      background: #fce7f3;
      color: #9d174d;
    }

  .status-badge {
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.85rem;
  }

    .status-badge.active {
      background: #d1fae5;
      color: #065f46;
    }

    .status-badge.inactive {
      background: #fef3c7;
      color: #92400e;
    }

  .action-buttons {
    display: flex;
    gap: 0.5rem;
  }

  .btn-edit, .btn-view, .btn-delete {
    padding: 0.5rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 1rem;
  }

  .btn-edit {
    background: #dbeafe;
    color: #1e40af;
  }

  .btn-view {
    background: #dcfce7;
    color: #166534;
  }

  .btn-delete {
    background: #fee2e2;
    color: #991b1b;
  }

  .pagination {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 1rem;
    margin-top: 2rem;
    padding: 1rem;
  }

  .pagination-btn {
    padding: 0.5rem 1rem;
    border: 1px solid #e1e5eb;
    background: white;
    border-radius: 4px;
    cursor: pointer;
  }

    .pagination-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

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
  }

  .modal-content {
    background: white;
    border-radius: 12px;
    width: 90%;
    max-width: 800px;
    max-height: 90vh;
    overflow-y: auto;
  }

  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.5rem;
    border-bottom: 1px solid #e1e5eb;
  }

  .modal-close {
    background: none;
    border: none;
    font-size: 1.5rem;
    cursor: pointer;
  }

  .modal-body {
    padding: 1.5rem;
  }

  .form-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }

  .form-group.full-width {
    grid-column: 1 / -1;
  }

  .form-group label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: 500;
  }

  .form-group input,
  .form-group select,
  .form-group textarea {
    width: 100%;
    padding: 0.5rem;
    border: 1px solid #e1e5eb;
    border-radius: 4px;
  }

  .modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
    padding: 1.5rem;
    border-top: 1px solid #e1e5eb;
  }

  .btn-primary {
    background: #2a5298;
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: 6px;
    cursor: pointer;
  }

  .btn-secondary {
    background: #e1e5eb;
    color: #4a5568;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: 6px;
    cursor: pointer;
  }

  .loading-state {
    text-align: center;
    padding: 3rem;
  }

  .empty-table {
    text-align: center;
    padding: 3rem;
    color: #718096;
  }

  .info-message {
    background: #fff3cd;
    border: 1px solid #ffeaa7;
    border-radius: 10px;
    padding: 1.5rem;
    display: flex;
    align-items: flex-start;
    gap: 1rem;
    margin-top: 2rem;
  }

  .message-icon {
    font-size: 2rem;
  }

  .message-content h3 {
    margin: 0 0 0.5rem;
    color: #856404;
  }

  .message-content p {
    margin: 0.5rem 0;
    color: #856404;
  }

  .admin-footer {
    background: white;
    border-top: 1px solid #e1e5eb;
    padding: 1rem 2rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    color: #718096;
    font-size: 0.9rem;
  }

  /* Стили для модального окна удаления */
  .delete-modal .modal-body {
    padding: 2rem;
  }

  .warning-text {
    color: #dc2626;
    font-weight: 600;
    margin-top: 1rem;
  }

  /* Стили для кнопок действий в таблице */
  .action-buttons {
    display: flex;
    gap: 0.5rem;
  }

  .btn-edit, .btn-delete {
    padding: 0.5rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 1rem;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 36px;
    height: 36px;
  }

  .btn-edit {
    background: #dbeafe;
    color: #1e40af;
  }

    .btn-edit:hover {
      background: #bfdbfe;
    }

  .btn-delete {
    background: #fee2e2;
    color: #dc2626;
  }

    .btn-delete:hover {
      background: #fecaca;
    }

    .btn-delete:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  /* Стили для модального окна удаления */
  .delete-modal {
    max-width: 500px;
  }

    .delete-modal .modal-footer {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding: 1.5rem;
      border-top: 1px solid #e1e5eb;
    }

    /* Стили для вкладки платежей */
.payments-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.payments-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.filters select {
  padding: 0.5rem;
  border: 1px solid #e1e5eb;
  border-radius: 6px;
  background: white;
  min-width: 150px;
}

.payments-table {
  width: 100%;
  border-collapse: collapse;
}

  .payments-table th {
    text-align: left;
    padding: 1rem;
    border-bottom: 2px solid #e1e5eb;
    background: #f8fafc;
    cursor: pointer;
    user-select: none;
  }

  .payments-table td {
    padding: 1rem;
    border-bottom: 1px solid #e1e5eb;
  }

.payment-date {
  display: flex;
  flex-direction: column;
}

  .payment-date small {
    color: #718096;
    font-size: 0.85rem;
  }

.patient-info {
  display: flex;
  flex-direction: column;
}

  .patient-info small {
    color: #718096;
    font-size: 0.85rem;
  }

.payment-amount {
  font-weight: 600;
  color: #1e40af;
}

/* Стили для статусов платежей */
.status-badge {
  padding: 0.35rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
  display: inline-block;
}

  .status-badge.waiting {
    background: #fef3c7;
    color: #92400e;
  }

  .status-badge.paid {
    background: #d1fae5;
    color: #065f46;
  }

  .status-badge.cancelled {
    background: #fee2e2;
    color: #991b1b;
  }

/* Стили для селекта статуса */
.status-select {
  padding: 0.5rem;
  border: 1px solid #e1e5eb;
  border-radius: 4px;
  background: white;
  min-width: 120px;
  margin-right: 0.5rem;
}

  .status-select:disabled {
    background: #f8fafc;
    cursor: not-allowed;
  }

/* Статистика платежей */
.payment-stats {
  display: flex;
  justify-content: space-between;
  background: #f8fafc;
  padding: 1.5rem;
  border-radius: 8px;
  margin: 1.5rem 0;
  flex-wrap: wrap;
  gap: 1rem;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 120px;
}

.stat-label {
  font-size: 0.9rem;
  color: #718096;
  margin-bottom: 0.5rem;
}

.stat-value {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2a5298;
}

/* Стили для модального окна деталей */
.payment-details {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #e1e5eb;
}

  .detail-row:last-child {
    border-bottom: none;
  }

.detail-label {
  font-weight: 600;
  color: #4a5568;
  min-width: 150px;
}

.detail-value {
  text-align: right;
  color: #2d3748;
}

  .detail-value.amount {
    font-weight: 600;
    color: #1e40af;
    font-size: 1.1rem;
  }

.history-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-height: 200px;
  overflow-y: auto;
}

.history-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem;
  background: #f8fafc;
  border-radius: 4px;
  font-size: 0.9rem;
}

.history-status {
  padding: 0.2rem 0.5rem;
  border-radius: 12px;
  font-size: 0.8rem;
}

/* Адаптивность */
@media (max-width: 768px) {
  .payments-actions {
    flex-direction: column;
    align-items: stretch;
  }

  .payment-stats {
    flex-direction: column;
    align-items: center;
  }

  .stat-item {
    width: 100%;
    flex-direction: row;
    justify-content: space-between;
  }

  .detail-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.25rem;
  }

  .detail-value {
    text-align: left;
    width: 100%;
  }
}
</style>
