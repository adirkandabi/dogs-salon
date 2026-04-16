import React from "react";
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
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import VisibilityIcon from "@mui/icons-material/Visibility";
import Navbar from "../components/Navbar";
import { useAppointments } from "../hooks/useAppointments";

const AppointmentsPage = () => {
  const { appointments, loading, filters, setFilters, deleteAppointment } =
    useAppointments();

  return (
    <>
      <Navbar />
      <Container maxWidth="lg">
        <Typography variant="h4" sx={{ mb: 3 }}>
          Waiting List
        </Typography>

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
            value={filters.date}
            onChange={(e) => setFilters({ ...filters, date: e.target.value })}
          />
        </Paper>

        {/* Table Section */}
        <TableContainer component={Paper} elevation={3}>
          <Table>
            <TableHead sx={{ bgcolor: "#eeeeee" }}>
              <TableRow>
                <TableCell>Customer</TableCell>
                <TableCell>Dog Size</TableCell>
                <TableCell>Date & Time</TableCell>
                <TableCell>Price</TableCell>
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
                appointments.map((app) => (
                  <TableRow key={app.appointmentId}>
                    <TableCell>{app.customerName}</TableCell>
                    <TableCell>{app.dogSizeName}</TableCell>
                    <TableCell>
                      {new Date(app.appointmentDate).toLocaleString()}
                    </TableCell>
                    <TableCell>${app.price}</TableCell>
                    <TableCell align="center">
                      <IconButton color="primary" title="View Details">
                        <VisibilityIcon />
                      </IconButton>
                      <IconButton
                        color="error"
                        onClick={() => deleteAppointment(app.appointmentId)}
                        title="Delete"
                      >
                        <DeleteIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))
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
      </Container>
    </>
  );
};

export default AppointmentsPage;
