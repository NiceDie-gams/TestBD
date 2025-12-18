import axios from 'axios';

export const authService = {
  async login(credentials) {
    const response = await axios.post('/Auth/login', credentials);
    return response.data;
  },

  async register(userData) {
    const response = await axios.post('/Auth/register', userData);
    return response.data;
  },

  async checkUserExists(login) {
    const response = await axios.get(`/Auth/check/${login}`);
    return response.data;
  }
};
