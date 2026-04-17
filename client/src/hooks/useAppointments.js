import { useState, useEffect, useCallback } from "react";
import { appointmentsAPI } from "../services/api";

export const useAppointments = () => {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Filters state
  const [filters, setFilters] = useState({
    customerName: "",
    date: "",
  });

  const fetchAppointments = useCallback(async () => {
    setLoading(true);
    try {
      const response = await appointmentsAPI.getAll();
      setAppointments(response.data);
    } catch (err) {
      setError("Failed to load appointments");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchAppointments();
  }, [fetchAppointments]);

  const deleteAppointment = async (id) => {
    try {
      await appointmentsAPI.delete(id);
      setAppointments((prev) => prev.filter((a) => a.appointmentId !== id));
    } catch (err) {
      alert(err.response?.data?.message || "Delete failed");
    }
  };
  const addAppointment = async (appointmentData) => {
    setLoading(true);
    try {
      await appointmentsAPI.create(appointmentData);
      await fetchAppointments();

      return { success: true };
    } catch (err) {
      return {
        success: false,
        error: err.response?.data?.message || "Failed to add",
      };
    } finally {
      setLoading(false);
    }
  };

  const updateAppointment = async (id, updatedData) => {
    setLoading(true);
    try {
      await appointmentsAPI.update(id, updatedData);
      await fetchAppointments();
      return { success: true };
    } catch (err) {
      return {
        success: false,
        error: err.response?.data?.message || "Update failed",
      };
    } finally {
      setLoading(false);
    }
  };
  // filtering logic
  const filteredAppointments = appointments.filter((app) => {
    const matchesName = app.customerName
      .toLowerCase()
      .includes(filters.customerName.toLowerCase());
    const matchesDate = filters.date
      ? app.appointmentDate.startsWith(filters.date)
      : true;
    return matchesName && matchesDate;
  });

  return {
    appointments: filteredAppointments,
    loading,
    error,
    filters,

    setFilters,
    addAppointment,
    updateAppointment,
    refresh: fetchAppointments,
    deleteAppointment,
  };
};
