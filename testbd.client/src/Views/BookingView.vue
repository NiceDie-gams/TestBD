<template>
  <div class="booked">
    <h2>Запись к врачу</h2>

    <div v-if="!authStore.isAuthenticated">
      <p>Для записи необходимо <router-link to="/login">войти</router-link>.</p>
    </div>

    <div v-else>
      <p>Вы записываетесь к врачу: <strong>{{ doctorName }}</strong></p>

      <label>
        Выберите дату:
        <input type="date" v-model="selectedDate" @change="loadSchedule" />
      </label>

      <div v-if="loading">Загрузка расписания...</div>

      <div v-if="slots.length > 0">
        <h3>Свободные слоты</h3>
        <ul>
          <li v-for="slot in slots" :key="slot.scheduleNoteId">
            <button :disabled="!slot.isAvailable"
                    @click="bookSlot(slot)">
              {{ slot.startTime.substring(0,5) }} - {{ slot.endTime.substring(0,5) }}
              (каб. {{ slot.cabinetNumber }})
              <span v-if="!slot.isAvailable"> — занято</span>
            </button>
          </li>
        </ul>
      </div>

      <p v-if="message" :class="{ error: isError }">{{ message }}</p>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRoute, useRouter } from 'vue-router'
  import { useAuthStore } from '@/stores/auth'
  import { doctorService } from '@/services/doctor.service'
  import { appointmentService } from '@/services/appointment.service'

  const route = useRoute()
  const router = useRouter()
  const authStore = useAuthStore()

  const doctorId = route.query.doctorId || null
  const doctorName = route.query.doctorName || 'Неизвестный врач'

  const selectedDate = ref('')
  const slots = ref([])
  const loading = ref(false)
  const message = ref('')
  const isError = ref(false)

  const loadSchedule = async () => {
    if (!selectedDate.value || !doctorId) {
      slots.value = []
      return
    }

    loading.value = true
    try {
      slots.value = await doctorService.getDoctorSchedule(doctorId, selectedDate.value)
    } catch (error) {
      console.error('Ошибка загрузки расписания:', error)
      message.value = 'Не удалось загрузить расписание'
      isError.value = true
    } finally {
      loading.value = false
    }
  }

  const bookSlot = async (slot) => {
    if (!authStore.patientId) {
      message.value = 'Ошибка: не найден идентификатор пациента'
      isError.value = true
      return
    }

    try {
      console.log(typeof(slot.scheduleNoteId))
      const appointmentData = {
        status: 'booked',
        createdAt: new Date().toISOString(),
        patientId: authStore.patientId,
        scheduleId: slot.scheduleNoteId
      }

      await appointmentService.createAppointment(appointmentData)
      message.value = 'Вы успешно записались!'
      isError.value = false

      // Возврат на главную через 2 секунды
      setTimeout(() => router.push('/'), 2000)
    } catch (error) {
      console.error('Ошибка записи:', error)
      message.value = 'Не удалось записаться. Попробуйте позже.'
      isError.value = true
    }
  }
</script>

<style scoped>
  .booked {
    max-width: 600px;
    margin: 0 auto;
  }

  ul {
    list-style: none;
    padding: 0;
  }

  button {
    margin: 0.5rem 0;
    padding: 0.5rem 1rem;
  }

  .error {
    color: red;
  }
</style>
