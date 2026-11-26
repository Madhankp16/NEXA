import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Attach JWT Token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('nexawork_access_token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Handle Refresh Token on 401
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const refreshToken = localStorage.getItem('nexawork_refresh_token');
      const accessToken = localStorage.getItem('nexawork_access_token');

      if (refreshToken && accessToken) {
        try {
          const res = await axios.post('/api/auth/refresh-token', {
            accessToken,
            refreshToken,
          });

          if (res.data?.success && res.data.data?.accessToken) {
            const newAccess = res.data.data.accessToken;
            const newRefresh = res.data.data.refreshToken;
            localStorage.setItem('nexawork_access_token', newAccess);
            localStorage.setItem('nexawork_refresh_token', newRefresh);

            originalRequest.headers.Authorization = `Bearer ${newAccess}`;
            return api(originalRequest);
          }
        } catch (refreshErr) {
          localStorage.removeItem('nexawork_access_token');
          localStorage.removeItem('nexawork_refresh_token');
          localStorage.removeItem('nexawork_user');
          window.location.href = '/login';
        }
      } else {
        localStorage.removeItem('nexawork_access_token');
        localStorage.removeItem('nexawork_refresh_token');
        localStorage.removeItem('nexawork_user');
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export default api;
