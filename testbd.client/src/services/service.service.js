import axios from 'axios';

export const serviceService = {
  async getAllServices() {
    const response = await axios.get('/Service/services');
    return response.data;
  },

  async addProvidedService(serviceData) {
    const response = await axios.post('/ProvidedService', serviceData);
    return response.data;
  },

  async updateProvidedService(serviceData) {
    const response = await axios.put('/ProvidedService', serviceData);
    return response.data;
  },

  async getProvidedServices(appointmentId) {
    const response = await axios.get(`/ProvidedService?id=${appointmentId}`);
    return response.data;
  }
};
