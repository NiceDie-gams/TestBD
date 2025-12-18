import axios from 'axios';

export const getCompletedAppointments = async (employee_id) => {
  try {
    console.log('Fetching completed appointments...');
    const token = localStorage.getItem('auth_token');

    const response = await axios.get(`/Appointment/completed/${employee_id}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    console.log('Raw response data:', response.data);

    // Трансформируем данные
    const transformedData = transformAppointmentData(response.data);
    console.log('Transformed data:', transformedData);

    return transformedData;

  } catch (error) {
    console.error('Error fetching completed appointments:', error);
    console.error('Error details:', error.response?.data);

    // Для тестирования вернем mock данные
    return getMockCompletedAppointments();
  }
};

// Трансформация данных с бэкенда
const transformAppointmentData = (appointments) => {
  if (!appointments) return [];

  console.log('Transforming data:', appointments);

  // Если это не массив, пробуем преобразовать
  let dataArray = appointments;
  if (!Array.isArray(appointments)) {
    if (appointments.data && Array.isArray(appointments.data)) {
      dataArray = appointments.data;
    } else {
      console.warn('Expected array but got:', appointments);
      return [];
    }
  }

  return dataArray.map(item => {
    console.log('Processing item:', item);

    // Проверяем структуру данных
    const appointment = { ...item };

    // Преобразуем appointmentDate
    if (appointment.appointmentDate) {
      if (typeof appointment.appointmentDate === 'string') {
        // Уже строка
      } else if (appointment.appointmentDate.year) {
        // DateOnly объект
        const { year, month, day } = appointment.appointmentDate;
        appointment.appointmentDate = `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
      }
    }

    // Преобразуем scheduleNote если есть
    if (appointment.scheduleNote) {
      if (appointment.scheduleNote.pointDate && appointment.scheduleNote.pointDate.year) {
        const { year, month, day } = appointment.scheduleNote.pointDate;
        appointment.scheduleNote.pointDate = `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
      }

      if (appointment.scheduleNote.startTime && appointment.scheduleNote.startTime.hours !== undefined) {
        const { hours, minutes } = appointment.scheduleNote.startTime;
        appointment.scheduleNote.startTime = `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:00`;
      }

      if (appointment.scheduleNote.endTime && appointment.scheduleNote.endTime.hours !== undefined) {
        const { hours, minutes } = appointment.scheduleNote.endTime;
        appointment.scheduleNote.endTime = `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:00`;
      }
    }

    return appointment;
  });
};

// Обновленная функция добавления записи с учетом типа посещения
export const addVisitHistory = async (visitHistoryData) => {
  try {
    console.log('Adding visit history:', visitHistoryData);

    // Конвертируем дату в UTC формат ISO
    const dataToSend = {
      ...visitHistoryData,
      visitDate: convertToUTC(visitHistoryData.visitDate)
    };

    console.log('Sending data:', dataToSend);

    const token = localStorage.getItem('auth_token');
    const response = await axios.post('/VisitHistory', dataToSend, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    return response.data;
  } catch (error) {
    console.error('Error adding visit history:', error);
    if (error.response) {
      console.error('Server response:', error.response.data);
    }
    throw error;
  }
};

// Функция для конвертации даты в UTC формат
const convertToUTC = (dateString) => {
  if (!dateString) return '';

  // Если дата уже в формате YYYY-MM-DD, добавляем время
  if (dateString.match(/^\d{4}-\d{2}-\d{2}$/)) {
    // Создаем дату с временем 00:00:00
    const date = new Date(`${dateString}T00:00:00`);
    // Конвертируем в UTC
    return date.toISOString();
  }

  // Если это уже полная дата
  try {
    const date = new Date(dateString);
    return date.toISOString();
  } catch {
    return dateString;
  }
};

// Mock данные
const getMockCompletedAppointments = () => {
  const today = new Date();
  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);

  return [
    {
      appointmentId: '11111111-1111-1111-1111-111111111111',
      status: 'completed',
      createdAt: today.toISOString(),
      appointmentDate: today.toISOString().split('T')[0],
      patient: {
        patientId: '22222222-2222-2222-2222-222222222222',
        patientFio: 'Иванов Петр Сидорович (Test)',
        contactPhone: '+7 999 123-45-67'
      },
      scheduleNote: {
        pointDate: today.toISOString().split('T')[0],
        startTime: '10:00:00',
        endTime: '11:00:00',
        cabinetNumber: 101
      }
    }
  ];
};

export default {
  getCompletedAppointments,
  addVisitHistory
};
