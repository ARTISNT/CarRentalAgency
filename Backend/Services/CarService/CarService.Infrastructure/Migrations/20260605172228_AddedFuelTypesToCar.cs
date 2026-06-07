using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFuelTypesToCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "battery_capacity_kwh",
                table: "cars",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "battery_current_kwh",
                table: "cars",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "fuel_capacity_liters",
                table: "cars",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "fuel_current_liters",
                table: "cars",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "battery_capacity_kwh",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "battery_current_kwh",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "fuel_capacity_liters",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "fuel_current_liters",
                table: "cars");
        }
    }
}
