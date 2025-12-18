<!-- views/patient/AppointmentsView.vue -->
<template>
  <div class="appointments">
    <h2>Мои записи на прием</h2>

    <div class="appointments-list">
      <div v-for="appointment in appointments" :key="appointment.appointmentId"
           class="appointment-card" :class="appointment.status">
        <div class="appointment-info">
          <h3>Врач: {{ appointment.employeeFio }}</h3>
          <p>Дата: {{ formatDate(appointment.appointmentDate) }}</p>
          <p>Время: {{ appointment.appointmentTime }}</p>
          <p>Кабинет: {{ appointment.cabinet }}</p>
          <p>Статус: {{ getStatusText(appointment.status) }}</p>
        </div>

        <div v-if="appointment.status === 'booked'" class="appointment-actions">
          <button @click="cancelAppointment(appointment)" class="cancel-btn">
            Отменить запись
          </button>
        </div>
      </div>
    </div>

    <!-- Модальное окно подтверждения отмены -->
    <div v-if="showCancelModal" class="modal">
      <div class="modal-content">
        <h3>Подтверждение отмены</h3>
        <p>Вы уверены, что хотите отменить запись?</p>
        <div class="modal-actions">
          <button @click="confirmCancel">Да, отменить</button>
          <button @click="showCancelModal = false">Нет</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { appointmentService } from '@/services/appointment.service';

const authStore = useAuthStore();
const appointments = ref([]);
const showCancelModal = ref(false);
const appointmentToCancel = ref(null);

const loadAppointments = async () => {
  try {
    if (authStore.patientId) {
      appointments.value = await appointmentService.getPatientAppointments(authStore.patientId);
    }
  } catch (error) {
    console.error('Ошибка загрузки записей:', error);
  }
};

const cancelAppointment = (appointment) => {
  appointmentToCancel.value = appointment;
  showCancelModal.value = true;
};

const confirmCancel = async () => {
  try {
    await appointmentService.cancelAppointment(appointmentToCancel.value.appointmentId);
    await loadAppointments(); // Перезагружаем список
    showCancelModal.value = false;
  } catch (error) {
    console.error('Ошибка отмены записи:', error);
  }
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('ru-RU');
};

const getStatusText = (status) => {
  const statusMap = {
    'booked': 'Забронировано',
    'completed': 'Завершено',
    'cancelled': 'Отменено'
  };
  return statusMap[status] || status;
};

onMounted(() => {
  loadAppointments();
});
</script>
