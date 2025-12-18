import { getAllDoctors } from './doctor.service';
import { getAllPatients } from './api.js';
import { getAllPayments } from './api.js';
import { getAllTodayAppointments } from './appointment.service'

// Заглушка для пациентов
const getPatientsCount = async () => {
  const patients = await getAllPatients();
  return patients.length;
};

// Заглушка для записей на сегодня
const getTodayAppointmentsCount = async () => {
  const appointments = await getAllTodayAppointments();
  return appointments.length;
};

// Заглушка для выручки
export const getRevenue = async () => {
  try {
    console.log('Calculating today revenue...');
    const payments = await getAllPayments();

    // Получаем сегодняшнюю дату в UTC в формате YYYY-MM-DD
    const today = new Date().toISOString().split('T')[0];

    let revenue = 0;

    // Фильтруем платежи за сегодня со статусом "Completed"
    for (let payment of payments) {
      // Приводим дату платежа к такому же формату
      const paymentDate = new Date(payment.paymentDate).toISOString().split('T')[0];

      if (payment.status === "Paied" && paymentDate === today) {
        revenue += payment.summaryPrice;
      }
    }

    console.log(`Today revenue: ${revenue}`);
    return revenue;
  } catch (error) {
    console.error('Error calculating today revenue:', error);
  }
};

// Получение количества врачей
const getDoctorsCount = async () => {
  const doctors = await getAllDoctors();
  return doctors.length;
};

export const adminService = {
  getDoctorsCount,
  getPatientsCount,
  getTodayAppointmentsCount,
  getRevenue
};
