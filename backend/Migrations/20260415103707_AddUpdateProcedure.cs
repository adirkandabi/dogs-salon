using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogsSalon.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    CREATE PROCEDURE sp_UpdateAppointment 
                        @AppointmentId INT,
                        @UserId INT, 
                        @DogSizeId INT, 
                        @AppointmentDate DATETIME
                    AS
                    BEGIN
                        -- Declare local variables for calculation
                        DECLARE @BasePrice DECIMAL(18,2);
                        DECLARE @Count INT;
                        DECLARE @FinalPrice DECIMAL(18,2);
    
                        -- Fetch the base price for the new dog size
                        SELECT @BasePrice = BasePrice FROM DogSizes WHERE Id = @DogSizeId;
    
                        -- Count total appointments for this user (including the current one)
                        SELECT @Count = COUNT(*) FROM Appointments WHERE UserId = @UserId;
    
                        -- Discount logic: 10% off if user has 3 or more appointments
                        IF @Count >= 3
                            SET @FinalPrice = @BasePrice * 0.9;
                        ELSE
                            SET @FinalPrice = @BasePrice;

                        -- Perform the update only if the appointment belongs to the user
                        UPDATE Appointments 
                        SET DogSizeId = @DogSizeId, 
                            AppointmentDate = @AppointmentDate, 
                            FinalPrice = @FinalPrice
                        WHERE Id = @AppointmentId AND UserId = @UserId;
                    END
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
