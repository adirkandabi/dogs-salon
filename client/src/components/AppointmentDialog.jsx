import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Typography,
  Alert,
} from "@mui/material";
import { dogSizesAPI } from "../services/api";
import { useAppointments } from "../hooks/useAppointments";
const INITIAL_FORM = {
  appointmentDate: "",
  dogSizeId: "",
};
const AppointmentDialog = ({ open, onClose, onSave, appointment }) => {
  const [dogSizes, setDogSizes] = useState([]);
  const [formData, setFormData] = useState(INITIAL_FORM);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  useEffect(() => {
    dogSizesAPI.getAll().then((res) => setDogSizes(res.data));
  }, []);
  useEffect(() => {
    if (appointment) {
      setFormData({
        appointmentDate: appointment.appointmentDate.slice(0, 16), // פורמט datetime-local
        dogSizeId: appointment.dogSizeId || "",
      });
    } else {
      setFormData(INITIAL_FORM);
    }
  }, [appointment, open]);

  const isEdit = Boolean(appointment);
  const handleSubmit = async () => {
    setLoading(true);
    const result = await onSave(formData, appointment?.id);
    if (result.success) {
      handleClose();
      setFormData({ appointmentDate: "", dogSizeId: "" });
    } else {
      setError(result.error);
    }
    setLoading(false);
  };
  const handleClose = () => {
    setFormData(INITIAL_FORM);
    setError("");
    onClose();
  };
  const getMinDateTime = () => {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  };
  const selectedSize = dogSizes.find((s) => s.id === formData.dogSizeId);

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="xs">
      <DialogTitle>
        {isEdit ? "Edit Appointment" : "Book New Appointment"}
      </DialogTitle>
      <DialogContent
        sx={{
          display: "flex",
          flexDirection: "column",
          gap: 3,
          mt: 1,
          "&.MuiDialogContent-root": {
            paddingTop: "7px !important",
          },
        }}
      >
        <TextField
          type="datetime-local"
          label="Date & Time"
          InputLabelProps={{ shrink: true }}
          InputProps={{
            inputProps: {
              min: getMinDateTime(),
              step: 1800,
            },
          }}
          value={formData.appointmentDate}
          onClick={(e) => {
            const input = e.currentTarget.querySelector("input");
            input?.showPicker();
          }}
          onChange={(e) => {
            if (new Date(e.target.value) < new Date()) {
              setError("Please select a future date and time.");
            } else {
              setError("");
            }
            setFormData({ ...formData, appointmentDate: e.target.value });
          }}
          sx={{
            width: 200,
            "& input::-webkit-datetime-edit-fields-wrapper": {
              display: formData.appointmentDate ? "flex" : "none",
            },
          }}
        />
        <TextField
          select
          label="Dog Size"
          fullWidth
          value={formData.dogSizeId}
          onChange={(e) =>
            setFormData({ ...formData, dogSizeId: e.target.value })
          }
        >
          {dogSizes.map((size) => (
            <MenuItem key={size.id} value={size.id}>
              {size.sizeName}
            </MenuItem>
          ))}
        </TextField>

        {selectedSize && (
          <Alert severity="info">
            <Typography variant="body2">
              Duration: {selectedSize.durationMinutes} min
            </Typography>
            <Typography variant="body2">
              Base Price: ₪{selectedSize.basePrice}
            </Typography>
            <Typography variant="caption">
              * Final price (incl. discounts) calculated at checkout
            </Typography>
          </Alert>
        )}

        {error && <Alert severity="error">{error}</Alert>}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={
            !formData.dogSizeId ||
            !formData.appointmentDate ||
            new Date(formData.appointmentDate) < new Date()
          }
        >
          {loading ? "Saving..." : isEdit ? "Update" : "Confirm"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AppointmentDialog;
