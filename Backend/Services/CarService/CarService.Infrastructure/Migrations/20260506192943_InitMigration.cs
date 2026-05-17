using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    license_plate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vin_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mileage = table.Column<double>(type: "float", nullable: false),
                    body_style = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transmission = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    drive_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    engine_horse_power = table.Column<double>(type: "float", nullable: false),
                    engine_volume = table.Column<double>(type: "float", nullable: true),
                    engine_power_reverse = table.Column<double>(type: "float", nullable: true),
                    engine_type = table.Column<int>(type: "int", nullable: false),
                    model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    generation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_facelift = table.Column<bool>(type: "bit", nullable: false),
                    variant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price_per_hour = table.Column<double>(type: "float", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cars", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cars");
        }
    }
}
