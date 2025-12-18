import axios from 'axios';

export const appointmentService = {
  // Для пациента
  async getPatientAppointments(patientId) {
    const token = localStorage.getItem('auth_token');
    const response = await axios.get(`/Appointment/ByPatientId/${patientId}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  },

  async createAppointment(appointmentData) {
    const token = localStorage.getItem('auth_token');
    const response = await axios.post('/Appointment', appointmentData, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    return response.data;
  },

  async cancelAppointment(id) {
    const token = localStorage.getItem('auth_token');
    const response = await axios.put(`/Appointment?id=${id}`, null, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    return response.data;
  }
};

export const getAllTodayAppointments = async () => {
  const token = localStorage.getItem('auth_token');
  const responce = await axios.get(`/Appointment/allToday`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return responce.data;
}

