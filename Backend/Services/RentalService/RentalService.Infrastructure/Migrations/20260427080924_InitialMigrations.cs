using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Car_Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Car_Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Car_Generation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Car_Variant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Car_IsFacelift = table.Column<bool>(type: "bit", nullable: false),
                    Car_LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Car_AvailabilityStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Car_PricePerHour = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Car_CarClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Renter_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Renter_SurName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Renter_Patronymic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Renter_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rentals");
        }
    }
}
