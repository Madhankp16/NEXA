import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { UserProfile, AuthResponse } from '../types';

interface AuthState {
  user: UserProfile | null;
  token: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

const savedUser = localStorage.getItem('nexawork_user');
const savedToken = localStorage.getItem('nexawork_access_token');
const savedRefresh = localStorage.getItem('nexawork_refresh_token');

const initialState: AuthState = {
  user: savedUser ? JSON.parse(savedUser) : null,
  token: savedToken || null,
  refreshToken: savedRefresh || null,
  isAuthenticated: Boolean(savedToken && savedUser),
  isLoading: false,
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ auth: AuthResponse }>
    ) => {
      const { user, accessToken, refreshToken } = action.payload.auth;
      state.user = user;
      state.token = accessToken;
      state.refreshToken = refreshToken;
      state.isAuthenticated = true;

      localStorage.setItem('nexawork_user', JSON.stringify(user));
      localStorage.setItem('nexawork_access_token', accessToken);
      localStorage.setItem('nexawork_refresh_token', refreshToken);
    },
    logout: (state) => {
      state.user = null;
      state.token = null;
      state.refreshToken = null;
      state.isAuthenticated = false;

      localStorage.removeItem('nexawork_user');
      localStorage.removeItem('nexawork_access_token');
      localStorage.removeItem('nexawork_refresh_token');
    },
    updateProfile: (state, action: PayloadAction<UserProfile>) => {
      state.user = action.payload;
      localStorage.setItem('nexawork_user', JSON.stringify(action.payload));
    },
  },
});

export const { setCredentials, logout, updateProfile } = authSlice.actions;
export default authSlice.reducer;
