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

const AddAppointmentDialog = ({ open, onClose, onAdd }) => {
  const [dogSizes, setDogSizes] = useState([]);
  const [formData, setFormData] = useState({
    appointmentDate: "",
    dogSizeId: "",
  });
  const [error, setError] = useState("");

  useEffect(() => {
    dogSizesAPI.getAll().then((res) => setDogSizes(res.data));
  }, []);

  const handleSubmit = async () => {
    const result = await onAdd(formData);
    if (result.success) {
      onClose();
      setFormData({ appointmentDate: "", dogSizeId: "" });
    } else {
      setError(result.error);
    }
  };
  const getMinDateTime = () => {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  };
  const selectedSize = dogSizes.find((s) => s.id === formData.dogSizeId);

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Book a New Appointment</DialogTitle>
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
          inputProps={{
            placeholder: "",
            min: getMinDateTime(),
          }}
          value={formData.appointmentDate}
          onClick={(e) => {
            e.target?.showPicker();
          }}
          onChange={(e) =>
            setFormData({ ...formData, appointmentDate: e.target.value })
          }
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
        <Button onClick={onClose}>Cancel</Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={
            !formData.dogSizeId ||
            !formData.appointmentDate ||
            new Date(formData.appointmentDate) < new Date()
          }
        >
          Confirm Booking
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddAppointmentDialog;
