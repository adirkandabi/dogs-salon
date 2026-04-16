import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7024/api",
});

// add a request interceptor to include the token in the headers
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authAPI = {
  login: (credentials) => api.post("/Auth/login", credentials),
  register: (userData) => api.post("/Auth/register", userData),
};

export const appointmentsAPI = {
  getAll: () => api.get("/Appointments"),
  create: (data) => api.post("/Appointments", data),
  delete: (id) => api.delete(`/Appointments/${id}`),
  update: (id, data) => api.put(`/Appointments/${id}`, data),
};

export default api;
