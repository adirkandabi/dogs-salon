import React, { useState } from "react";
import {
  Container,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Box,
  IconButton,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import VisibilityIcon from "@mui/icons-material/Visibility";
import Navbar from "../components/Navbar";
import { useAppointments } from "../hooks/useAppointments";
import AppointmentDialog from "./AppointmentDialog";

const AppointmentsPage = () => {
  const {
    appointments,
    loading,
    filters,
    setFilters,
    updateAppointment,
    deleteAppointment,
    addAppointment,
  } = useAppointments();
  const [isAddOpen, setIsAddOpen] = useState(false);
  const [editingApp, setEditingApp] = useState(null);
  const [selectedApp, setSelectedApp] = useState(null);

  const currentUserId = localStorage.getItem("userId");

  const isOwner = (appointmentUserId) => {
    return String(appointmentUserId) === String(currentUserId);
  };

  const isToday = (dateString) => {
    const today = new Date().toISOString().split("T")[0];
    const appointmentDate = new Date(dateString).toISOString().split("T")[0];
    return today === appointmentDate;
  };
  const isPast = (dateString) => {
    return new Date(dateString) < new Date();
  };
  const handleOpenDetails = (app) => {
    setSelectedApp(app);
  };

  const handleCloseDetails = () => {
    setSelectedApp(null);
  };
  return (
    <>
      <Navbar />
      <Container maxWidth="lg">
        <Typography variant="h4" sx={{ mb: 3 }}>
          Appointments
        </Typography>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 3,
          }}
        >
          <Button
            variant="contained"
            color="success"
            onClick={() => setIsAddOpen(true)}
          >
            + New Appointment
          </Button>
        </Box>
        {/* Filters Section */}
        <Paper
          sx={{ p: 2, mb: 3, display: "flex", gap: 2, alignItems: "center" }}
        >
          <TextField
            label="Filter by Name"
            variant="outlined"
            size="small"
            value={filters.customerName}
            onChange={(e) =>
              setFilters({ ...filters, customerName: e.target.value })
            }
          />
          <TextField
            type="date"
            label="Filter by Date"
            variant="outlined"
            size="small"
            InputLabelProps={{ shrink: true }}
            inputProps={{
              placeholder: "",
            }}
            value={filters.date}
            onClick={(e) => {
              e.target?.showPicker();
            }}
            onChange={(e) => setFilters({ ...filters, date: e.target.value })}
            sx={{
              width: 200,
              "& input::-webkit-datetime-edit-fields-wrapper": {
                display: filters.date ? "flex" : "none",
              },
            }}
          />

          <Button
            variant="outlined"
            onClick={() => setFilters({ customerName: "", date: "" })}
          >
            Clear Filters
          </Button>
        </Paper>

        {/* Table Section */}
        <TableContainer
          component={Paper}
          elevation={3}
          sx={{ marginBottom: 10 }}
        >
          <Table>
            <TableHead sx={{ bgcolor: "#eeeeee" }}>
              <TableRow>
                <TableCell>Customer</TableCell>
                <TableCell>Dog Size</TableCell>
                <TableCell>Date & Time</TableCell>

                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={5} align="center">
                    <CircularProgress />
                  </TableCell>
                </TableRow>
              ) : (
                appointments
                  .sort(
                    (a, b) =>
                      new Date(a.appointmentDate) - new Date(b.appointmentDate),
                  )
                  .map((app) => {
                    const past = isPast(app.appointmentDate);
                    return (
                      <TableRow
                        key={app.appointmentId}
                        sx={{
                          bgcolor: past ? "action.hover" : "inherit",
                          opacity: past ? 0.7 : 1,
                        }}
                      >
                        <TableCell
                          sx={{ fontStyle: past ? "italic" : "normal" }}
                        >
                          {app.customerName} {past && "(Past)"}
                        </TableCell>
                        <TableCell>{app.dogSizeName}</TableCell>
                        <TableCell>
                          {new Date(app.appointmentDate).toLocaleDateString(
                            "he-IL",
                            {
                              day: "2-digit",
                              month: "2-digit",
                              year: "numeric",
                              hour: "2-digit",
                              minute: "2-digit",
                              hour12: false,
                            },
                          )}
                        </TableCell>

                        <TableCell align="center">
                          <IconButton
                            color="primary"
                            title="View Details"
                            onClick={() => handleOpenDetails(app)}
                          >
                            <VisibilityIcon />
                          </IconButton>
                          {isOwner(app.userId) && (
                            <>
                              <IconButton
                                color="info"
                                onClick={() => setEditingApp(app)}
                                disabled={past}
                                sx={{ display: past ? "none" : "inline-flex" }}
                              >
                                <EditIcon />
                              </IconButton>
                              <IconButton
                                color="error"
                                disabled={past || isToday(app.appointmentDate)}
                                onClick={() => {
                                  if (window.confirm("Are you sure?")) {
                                    deleteAppointment(app.appointmentId);
                                  }
                                }}
                                sx={{ display: past ? "none" : "inline-flex" }}
                              >
                                <DeleteIcon />
                              </IconButton>
                            </>
                          )}
                        </TableCell>
                      </TableRow>
                    );
                  })
              )}

              {!loading && appointments.length === 0 && (
                <TableRow>
                  <TableCell colSpan={5} align="center">
                    No appointments found.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
        <AppointmentDialog
          open={Boolean(editingApp) || isAddOpen}
          onClose={() => {
            setIsAddOpen(false);
            setEditingApp(null);
          }}
          onSave={(data) =>
            editingApp
              ? updateAppointment(editingApp.appointmentId, data)
              : addAppointment(data)
          }
          loading={loading}
          appointment={editingApp}
        />
      </Container>
      <Dialog
        open={Boolean(selectedApp)}
        onClose={handleCloseDetails}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle sx={{ bgcolor: "primary.main", color: "white" }}>
          Appointment Details
        </DialogTitle>
        <DialogContent dividers>
          {selectedApp && (
            <Box
              sx={{ display: "flex", flexDirection: "column", gap: 2, pt: 1 }}
            >
              <Typography variant="subtitle1">
                <strong>Customer:</strong> {selectedApp.customerName}
              </Typography>
              <Typography variant="subtitle1">
                <strong>Dog Size:</strong> {selectedApp.dogSizeName}
              </Typography>
              <Typography variant="subtitle1">
                <strong>Appointment Date:</strong>{" "}
                {new Date(selectedApp.appointmentDate).toLocaleDateString(
                  "he-IL",
                  {
                    day: "2-digit",
                    month: "2-digit",
                    year: "numeric",
                    hour: "2-digit",
                    minute: "2-digit",
                    hour12: false,
                  },
                )}
              </Typography>
              <Typography variant="subtitle1">
                <strong>Price:</strong> ₪{selectedApp.price}
              </Typography>
              <hr />
              <Typography variant="body2" color="text.secondary">
                <strong>Request Created At:</strong>{" "}
                {new Date(selectedApp.createdAt).toLocaleDateString("he-IL", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                  hour12: false,
                })}
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDetails} variant="contained">
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default AppointmentsPage;
