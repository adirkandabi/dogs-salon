import { useState } from "react";
import { authAPI } from "../services/api";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

export const useAuth = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const login = async (credentials) => {
    setLoading(true);
    setError(null);
    try {
      const response = await authAPI.login(credentials);
      const token = response.data.token;
      const decoded = jwtDecode(token);
      console.log(decoded);

      localStorage.setItem("token", token);
      localStorage.setItem("firstName", decoded.FirstName);
      localStorage.setItem("userId", decoded.nameid);
      navigate("/appointments");
    } catch (err) {
      setError(err.response?.data?.message || "Login failed");
    } finally {
      setLoading(false);
    }
  };

  const register = async (userData) => {
    setLoading(true);
    setError(null);
    try {
      await authAPI.register(userData);
      navigate("/login");
    } catch (err) {
      setError(err.response?.data?.message || "Registration failed");
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return { login, register, logout, loading, error };
};
