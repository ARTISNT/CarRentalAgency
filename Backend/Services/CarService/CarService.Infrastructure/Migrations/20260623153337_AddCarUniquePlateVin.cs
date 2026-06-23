using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCarUniquePlateVin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "vin_code",
                table: "cars",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "license_plate",
                table: "cars",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_cars_license_plate",
                table: "cars",
                column: "license_plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cars_vin_code",
                table: "cars",
                column: "vin_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cars_license_plate",
                table: "cars");

            migrationBuilder.DropIndex(
                name: "IX_cars_vin_code",
                table: "cars");

            migrationBuilder.AlterColumn<string>(
                name: "vin_code",
                table: "cars",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "license_plate",
                table: "cars",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
