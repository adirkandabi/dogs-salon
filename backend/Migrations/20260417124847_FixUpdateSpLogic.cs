using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogsSalon.Migrations
{
    /// <inheritdoc />
    public partial class FixUpdateSpLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER PROCEDURE sp_UpdateAppointment 
                    @AppointmentId INT, 
                    @UserId INT, 
                    @DogSizeId INT, 
                    @AppointmentDate DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;

                    
                    IF NOT EXISTS (SELECT 1 FROM Appointments WHERE Id = @AppointmentId AND UserId = @UserId)
                    BEGIN
                        RAISERROR('Error: Appointment not found or unauthorized access.', 16, 1);
                        RETURN;
                    END

                   
                    DECLARE @Duration INT, @BasePrice DECIMAL(18,2), @EndTime DATETIME;
                    SELECT @Duration = DurationMinutes, @BasePrice = BasePrice 
                    FROM DogSizes WHERE Id = @DogSizeId;

                    SET @EndTime = DATEADD(MINUTE, @Duration, @AppointmentDate);

                    
                    IF EXISTS (
                        SELECT 1 FROM Appointments a
                        JOIN DogSizes ds ON a.DogSizeId = ds.Id
                        WHERE a.Id <> @AppointmentId 
                          AND @AppointmentDate < DATEADD(MINUTE, ds.DurationMinutes, a.AppointmentDate)
                          AND @EndTime > a.AppointmentDate
                    )
                    BEGIN
                        RAISERROR('Conflict: This time slot is already occupied by another dog.', 16, 1);
                        RETURN;
                    END

                    
                    DECLARE @Count INT, @FinalPrice DECIMAL(18,2);
                    SELECT @Count = COUNT(*) FROM Appointments WHERE UserId = @UserId AND Id <> @AppointmentId;
    
                   
                    IF (@Count >= 3)
                        SET @FinalPrice = @BasePrice * 0.9;
                    ELSE
                        SET @FinalPrice = @BasePrice;

                    
                    UPDATE Appointments 
                    SET DogSizeId = @DogSizeId, 
                        AppointmentDate = @AppointmentDate, 
                        FinalPrice = @FinalPrice
                    WHERE Id = @AppointmentId AND UserId = @UserId;

                    
                   
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
