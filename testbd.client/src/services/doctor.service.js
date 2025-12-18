import axios from 'axios';

export const getAllDoctors = async () => {
  try {
    console.log('Fetching doctors from /Employee/employee');
    const token = localStorage.getItem('auth_token');
    const response = await axios.get('/Employee/employee', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching doctors:', error);
    return getMockDoctors();
  }
};

export const createDoctorSchedule = async (scheduleData) => {
  try {
    const token = localStorage.getItem('auth_token');
    const response = await axios.post('/DoctorSchedule/createSchedule', scheduleData, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    console.error('Error creating schedule:', error);
  }
};

export const updateDoctor = async (employeeData) => {
  try {
    console.log('Updating doctor:', employeeData);

    const token = localStorage.getItem('auth_token');

    if (!token) {
      throw new Error('No authentication token found');
    }

    if (!employeeData || !employeeData.employeeId) {
      throw new Error('Invalid employee data. employeeId is required');
    }

    const response = await axios.put('/Employee', employeeData, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    console.log('Doctor updated successfully:', response.data);
    return response.data;

  } catch (error) {
    console.error('Error updating doctor:', error);

    // Детализированная обработка ошибок
    if (error.response) {
      // Сервер ответил с кодом ошибки
      console.error('Response status:', error.response.status);
      console.error('Response data:', error.response.data);

      if (error.response.status === 401) {
        throw new Error('Unauthorized: Invalid or expired token');
      } else if (error.response.status === 403) {
        throw new Error('Forbidden: Admin privileges required');
      } else if (error.response.status === 400) {
        throw new Error(`Bad request: ${error.response.data?.message || 'Invalid data'}`);
      } else if (error.response.status === 404) {
        throw new Error('Doctor not found');
      }
    } else if (error.request) {
      // Запрос был сделан, но ответ не получен
      console.error('No response received:', error.request);
      throw new Error('Network error: No response from server');
    }

    throw error;
  }
};

export const getDoctorSchedule = async (employeeId, date) => {
  try {
    // Форматируем дату в ISO строку (YYYY-MM-DD)
    const formattedDate = formatDateToISO(date);

    console.log(`Fetching schedule for doctor ${employeeId} on ${formattedDate}`);

    const token = localStorage.getItem('auth_token');
    const response = await axios.get(`/DoctorSchedule`, {
      params: {
        employeeId: employeeId,
        date: formattedDate
      },
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    console.log('Schedule response:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error fetching doctor schedule:', error);
    console.error('Error details:', error.response?.data);

    // Для отладки показываем mock данные
    return generateMockSchedule(date, employeeId);
  }
};

// НОВЫЙ МЕТОД: получение расписания с записями
export const getDoctorScheduleWithAppointments = async (employeeId, date) => {
  try {
    // Форматируем дату в ISO строку (YYYY-MM-DD)
    const formattedDate = formatDateToISO(date);

    console.log(`Fetching schedule with appointments for doctor ${employeeId} on ${formattedDate}`);

    const token = localStorage.getItem('auth_token');
    const response = await axios.get(`/DoctorSchedule/GetWithAppointment`, {
      params: {
        id: employeeId, // Параметр должен называться 'id' как в контроллере
        date: formattedDate
      },
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    console.log('Schedule with appointments response:', response.data);

    // Преобразуем данные в удобный формат для фронтенда
    return transformScheduleData(response.data);

  } catch (error) {
    console.error('Error fetching schedule with appointments:', error);
    console.error('Error details:', error.response?.data);

    // Для отладки генерируем тестовые данные с записями
    return generateMockScheduleWithAppointments(date, employeeId);
  }
};

// Преобразование данных с бэкенда в удобный для фронтенда формат
const transformScheduleData = (backendData) => {
  if (!backendData || !Array.isArray(backendData)) {
    console.warn('Invalid schedule data:', backendData);
    return [];
  }

  return backendData.map(item => {
    // Проверяем структуру данных (может быть разной в зависимости от бэкенда)
    if (item.schedule && item.appointment !== undefined) {
      // Формат: { schedule: {...}, appointment: {...} }
      return {
        ...item.schedule,
        appointment: item.appointment
      };
    } else if (item.scheduleNoteId) {
      // Формат: { scheduleNoteId: ..., pointDate: ..., appointment: {...} }
      return item;
    } else {
      console.warn('Unknown data format:', item);
      return item;
    }
  });
};

// Вспомогательный метод для форматирования даты
const formatDateToISO = (dateInput) => {
  if (!dateInput) return '';

  // Если это строка уже в формате YYYY-MM-DD
  if (typeof dateInput === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(dateInput)) {
    return dateInput;
  }

  // Если это объект Date
  if (dateInput instanceof Date) {
    return dateInput.toISOString().split('T')[0];
  }

  // Если это строка другого формата
  try {
    const date = new Date(dateInput);
    return date.toISOString().split('T')[0];
  } catch {
    return '';
  }
};

// Метод для генерации тестового расписания С ЗАПИСЯМИ
const generateMockScheduleWithAppointments = (date, employeeId) => {
  console.log('Using mock schedule with appointments for development');

  const dateStr = formatDateToISO(date);
  const slots = [];
  const startHour = 9;
  const endHour = 17;
  const slotDuration = 60;

  for (let hour = startHour; hour < endHour; hour++) {
    for (let minute = 0; minute < 60; minute += slotDuration) {
      const startTime = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}:00`;
      const endHourCalc = minute + slotDuration >= 60 ? hour + 1 : hour;
      const endMinute = minute + slotDuration >= 60 ? minute + slotDuration - 60 : minute + slotDuration;
      const endTime = `${endHourCalc.toString().padStart(2, '0')}:${endMinute.toString().padStart(2, '0')}:00`;

      // 30% слотов с записями
      const hasAppointment = Math.random() > 0.7;

      const slot = {
        scheduleNoteId: `mock-${dateStr}-${hour}-${minute}`,
        pointDate: dateStr,
        startTime: startTime,
        endTime: endTime,
        cabinetNumber: Math.floor(Math.random() * 20) + 100,
        isAvailable: Math.random() > 0.3,
        employeeId: employeeId
      };

      // Добавляем запись если есть
      if (hasAppointment) {
        slot.appointment = {
          appointmentId: `appointment-mock-${dateStr}-${hour}-${minute}`,
          status: Math.random() > 0.8 ? 'completed' : 'booked',
          createdAt: new Date().toISOString(),
          patient: {
            patientId: `patient-mock-${hour}-${minute}`,
            patientFio: `Пациент ${hour}:${minute}`,
            contactPhone: '+7 999 123-45-67'
          }
        };
      } else {
        slot.appointment = null;
      }

      slots.push(slot);
    }
  }

  return slots;
};

// Метод для генерации тестового расписания (старый, без записей)
const generateMockSchedule = (date, employeeId) => {
  console.log('Using mock schedule for development');

  const dateStr = formatDateToISO(date);
  const slots = [];
  const startHour = 9;
  const endHour = 17;
  const slotDuration = 60;

  for (let hour = startHour; hour < endHour; hour++) {
    for (let minute = 0; minute < 60; minute += slotDuration) {
      const startTime = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}:00`;
      const endHourCalc = minute + slotDuration >= 60 ? hour + 1 : hour;
      const endMinute = minute + slotDuration >= 60 ? minute + slotDuration - 60 : minute + slotDuration;
      const endTime = `${endHourCalc.toString().padStart(2, '0')}:${endMinute.toString().padStart(2, '0')}:00`;

      slots.push({
        scheduleNoteId: `mock-${dateStr}-${hour}-${minute}`,
        pointDate: dateStr,
        startTime: startTime,
        endTime: endTime,
        cabinetNumber: Math.floor(Math.random() * 20) + 100,
        isAvailable: Math.random() > 0.3,
        employeeId: employeeId
      });
    }
  }

  return slots;
};

// Тестовые данные врачей
const getMockDoctors = () => {
  return [
    {
      employeeId: '11111111-1111-1111-1111-111111111111',
      employeeFio: 'Иванов Иван Иванович (Тест)',
      post: 'Терапевт',
      specialization: 'Общая терапия',
      experience: 12,
      phone: '+7 (999) 123-45-67',
      email: 'ivanov@clinic.ru'
    }
  ];
};

// Экспорт всех функций как объект
export const doctorService = {
  getAllDoctors,
  getDoctorSchedule,
  getDoctorScheduleWithAppointments,
  updateDoctor,
  createDoctorSchedule
};

// Также экспортируем отдельные функции для удобства
export default doctorService;











//import axios from 'axios';

//export const getAllDoctors = async () => {
//  try {
//    console.log('Fetching doctors from /Employee/employee');
//    const token = localStorage.getItem('auth_token');
//    const response = await axios.get('/Employee/employee', {
//      headers: {
//        Authorization: `Bearer ${token}`
//      }
//    });
//    return response.data;
//  } catch (error) {
//    console.error('Error fetching doctors:', error);
//    return getMockDoctors();
//  }
//};

//export const getDoctorSchedule = async (employeeId, date) => {
//  try {
//    // Форматируем дату в ISO строку (YYYY-MM-DD)
//    const formattedDate = formatDateToISO(date);

//    console.log(`Fetching schedule for doctor ${employeeId} on ${formattedDate}`);

//    const token = localStorage.getItem('auth_token');
//    const response = await axios.get(`/DoctorSchedule`, {
//      params: {
//        employeeId: employeeId,
//        date: formattedDate
//      },
//      headers: {
//        Authorization: `Bearer ${token}`
//      }
//    });

//    console.log('Schedule response:', response.data);
//    return response.data;
//  } catch (error) {
//    console.error('Error fetching doctor schedule:', error);
//    console.error('Error details:', error.response?.data);

//    // Для отладки показываем mock данные
//    return generateMockSchedule(date, employeeId);
//  }
//};

//// Вспомогательный метод для форматирования даты
//const formatDateToISO = (dateInput) => {
//  if (!dateInput) return '';

//  // Если это строка уже в формате YYYY-MM-DD
//  if (typeof dateInput === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(dateInput)) {
//    return dateInput;
//  }

//  // Если это объект Date
//  if (dateInput instanceof Date) {
//    return dateInput.toISOString().split('T')[0];
//  }

//  // Если это строка другого формата
//  try {
//    const date = new Date(dateInput);
//    return date.toISOString().split('T')[0];
//  } catch {
//    return '';
//  }
//};

//// Метод для генерации тестового расписания
//const generateMockSchedule = (date, employeeId) => {
//  console.log('Using mock schedule for development');

//  const dateStr = formatDateToISO(date);
//  const slots = [];
//  const startHour = 9; // Начало рабочего дня
//  const endHour = 17; // Конец рабочего дня
//  const slotDuration = 60; // Длительность слота в минутах

//  for (let hour = startHour; hour < endHour; hour++) {
//    for (let minute = 0; minute < 60; minute += slotDuration) {
//      const startTime = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}:00`;
//      const endHourCalc = minute + slotDuration >= 60 ? hour + 1 : hour;
//      const endMinute = minute + slotDuration >= 60 ? minute + slotDuration - 60 : minute + slotDuration;
//      const endTime = `${endHourCalc.toString().padStart(2, '0')}:${endMinute.toString().padStart(2, '0')}:00`;

//      slots.push({
//        scheduleNoteId: `mock-${dateStr}-${hour}-${minute}`,
//        pointDate: dateStr,
//        startTime: startTime,
//        endTime: endTime,
//        cabinetNumber: Math.floor(Math.random() * 20) + 100,
//        isAvailable: Math.random() > 0.3, // 70% слотов доступны
//        employeeId: employeeId
//      });
//    }
//  }

//  return slots;
//};

//// Тестовые данные врачей
//const getMockDoctors = () => {
//  return [
//    {
//      employeeId: '11111111-1111-1111-1111-111111111111',
//      employeeFio: 'Иванов Иван Иванович (Тест)',
//      post: 'Терапевт',
//      specialization: 'Общая терапия',
//      experience: 12,
//      phone: '+7 (999) 123-45-67',
//      email: 'ivanov@clinic.ru'
//    }
//  ];
//};

//// Экспорт отдельных функций
//export const doctorService = {
//  getAllDoctors,
//  getDoctorSchedule
//};
