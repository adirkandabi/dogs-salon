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
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_CreateAppointment 
                    @UserId INT, @DogSizeId INT, @AppointmentDate DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @Duration INT, @EndTime DATETIME, @BasePrice DECIMAL(18,2);
                    SELECT @Duration = DurationMinutes, @BasePrice = BasePrice FROM DogSizes WHERE Id = @DogSizeId;
                    SET @EndTime = DATEADD(MINUTE, @Duration, @AppointmentDate);

                    IF EXISTS (
                        SELECT 1 FROM Appointments a JOIN DogSizes ds ON a.DogSizeId = ds.Id
                        WHERE @AppointmentDate < DATEADD(MINUTE, ds.DurationMinutes, a.AppointmentDate)
                          AND @EndTime > a.AppointmentDate
                    )
                    BEGIN
                        RAISERROR('Conflict: Time slot occupied.', 16, 1);
                        RETURN;
                    END

                    DECLARE @Count INT, @FinalPrice DECIMAL(18,2);
                    SELECT @Count = COUNT(*) FROM Appointments WHERE UserId = @UserId;
                    SET @FinalPrice = CASE WHEN @Count >= 3 THEN @BasePrice * 0.9 ELSE @BasePrice END;

                    INSERT INTO Appointments (UserId, DogSizeId, AppointmentDate, FinalPrice, CreatedAt)
                    VALUES (@UserId, @DogSizeId, @AppointmentDate, @FinalPrice, GETUTCDATE());
                END
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_UpdateAppointment 
                    @AppointmentId INT, @UserId INT, @DogSizeId INT, @AppointmentDate DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @Duration INT, @EndTime DATETIME, @BasePrice DECIMAL(18,2);
                    SELECT @Duration = DurationMinutes, @BasePrice = BasePrice FROM DogSizes WHERE Id = @DogSizeId;
                    SET @EndTime = DATEADD(MINUTE, @Duration, @AppointmentDate);

                    IF EXISTS (
                        SELECT 1 FROM Appointments a JOIN DogSizes ds ON a.DogSizeId = ds.Id
                        WHERE a.Id <> @AppointmentId
                          AND @AppointmentDate < DATEADD(MINUTE, ds.DurationMinutes, a.AppointmentDate)
                          AND @EndTime > a.AppointmentDate
                    )
                    BEGIN
                        RAISERROR('Conflict: Time slot occupied.', 16, 1);
                        RETURN;
                    END

                    DECLARE @Count INT, @FinalPrice DECIMAL(18,2);
                    SELECT @Count = COUNT(*) FROM Appointments WHERE UserId = @UserId;
                    SET @FinalPrice = CASE WHEN @Count >= 3 THEN @BasePrice * 0.9 ELSE @BasePrice END;

                    UPDATE Appointments SET DogSizeId = @DogSizeId, AppointmentDate = @AppointmentDate, FinalPrice = @FinalPrice
                    WHERE Id = @AppointmentId AND UserId = @UserId;
                END
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.views WHERE name = 'v_AppointmentSummaries')
                    DROP VIEW vw_AppointmentSummaries;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW vw_AppointmentSummaries AS
                SELECT 
                    a.Id AS AppointmentId,
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
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DogSizes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
