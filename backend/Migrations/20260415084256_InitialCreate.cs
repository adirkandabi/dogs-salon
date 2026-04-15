using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DogsSalon.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DogSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DogSizeId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_DogSizes_DogSizeId",
                        column: x => x.DogSizeId,
                        principalTable: "DogSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DogSizes",
                columns: new[] { "Id", "BasePrice", "DurationMinutes", "SizeName" },
                values: new object[,]
                {
                    { 1, 100m, 30, "Small" },
                    { 2, 150m, 60, "Medium" },
                    { 3, 200m, 90, "Large" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DogSizeId",
                table: "Appointments",
                column: "DogSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments",
                column: "UserId");
            // Create the view for appointment summaries
            migrationBuilder.Sql(@"
                        EXEC('CREATE VIEW vw_AppointmentSummaries AS
                        SELECT a.Id AS AppointmentId, u.FirstName AS CustomerName, 
                               ds.SizeName AS DogSizeName, a.AppointmentDate, a.FinalPrice AS Price, a.CreatedAt
                        FROM Appointments a
                        JOIN Users u ON a.UserId = u.Id
                        JOIN DogSizes ds ON a.DogSizeId = ds.Id')
                    ");
            // Create the stored procedure for creating appointments with discount logic
            migrationBuilder.Sql(@"
                    EXEC('CREATE PROCEDURE sp_CreateAppointment 
                        @UserId INT, 
                        @DogSizeId INT, 
                        @AppointmentDate DATETIME
                    AS
                    BEGIN
                        DECLARE @BasePrice DECIMAL(18,2);
                        DECLARE @PrevApptsCount INT;
                        DECLARE @FinalPrice DECIMAL(18,2);

                        -- Fetch the base price for the selected dog size
                        SELECT @BasePrice = BasePrice FROM DogSizes WHERE Id = @DogSizeId;

                        -- Count how many appointments the user has had in the past
                        SELECT @PrevApptsCount = COUNT(*) FROM Appointments WHERE UserId = @UserId;

                        -- Calculate discount: if there are 3 or more appointments, 10% discount
                        IF @PrevApptsCount >= 3
                            SET @FinalPrice = @BasePrice * 0.9;
                        ELSE
                            SET @FinalPrice = @BasePrice;

                        -- Insert the new appointment into the table
                        INSERT INTO Appointments (UserId, DogSizeId, AppointmentDate, CreatedAt, FinalPrice)
                        VALUES (@UserId, @DogSizeId, @AppointmentDate, GETUTCDATE(), @FinalPrice);
                    END')
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DogSizes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
