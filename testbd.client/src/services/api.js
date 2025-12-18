import axios from 'axios';

export const getPatientProfile = async (patientId) => {
  const token = localStorage.getItem('auth_token');
  const response = await axios.get(`/Patients/GetById/${patientId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return response.data;
};

export const getAllPatients = async () => {
  const token = localStorage.getItem('auth_token');
  const responce = await axios.get(`/Patients/patients`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return responce.data;
}

export const updatePatientProfile = async (profileData) => {
  const token = localStorage.getItem('auth_token');
  const response = await axios.put(`/Patients`, profileData, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  return response.data;
};

export const createPatient = async (patientData) => {
  try {
    console.log('Creating new patient:', patientData);
    const token = localStorage.getItem('auth_token');

    if (!token) {
      console.warn('No auth token found, mock creation');
      return {
        success: true,
        message: 'Patient created (mock)',
        patientId: 'mock-' + Date.now()
      };
    }

    const response = await axios.post('/Patients/addPatient', patientData, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    console.log('Patient created successfully:', response.data);
    return {
      success: true,
      message: 'Patient created successfully',
      data: response.data
    };
  } catch (error) {
    console.error('Error creating patient:', error);
    if (error.response?.status === 403) {
      throw new Error('Access denied: Admin privileges required');
    }
    throw error;
  }
};

// Функция для получения истории посещений
export const getVisitHistory = async (patientId) => {
  try {
    const token = localStorage.getItem('auth_token');
    const response = await axios.get(`/VisitHistory/${patientId}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    console.log('Visit history response:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error fetching visit history:', error);
    throw error;
  }
};

export const getAllPayments = async () => {
  const token = localStorage.getItem('auth_token');
  const responce = await axios.get(`/Payment/all`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return responce.data;
}

export const updatePaymentStatus = async (id, status) => {
  const token = localStorage.getItem('auth_token');
  const responce = await axios.put('/Payment',
    {
      paymentId: id,
      status: status
    },
    {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return responce.data;
}

export const deleteFromPatient = async (patient_id) => {
  const token = localStorage.getItem('auth_token');
  const response = await axios.delete(`/Patients/${patient_id}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  return response.data;
}
