using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogsSalon.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER VIEW vw_AppointmentSummaries AS
                SELECT 
                    a.Id AS AppointmentId,
                    u.Id AS UserId,
                    u.FirstName AS CustomerName,
                    ds.SizeName AS DogSizeName,
                    a.AppointmentDate,
                    a.FinalPrice AS Price,
                    a.CreatedAt
                FROM Appointments a
                JOIN Users u ON a.UserId = u.Id
                JOIN DogSizes ds ON a.DogSizeId = ds.Id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
