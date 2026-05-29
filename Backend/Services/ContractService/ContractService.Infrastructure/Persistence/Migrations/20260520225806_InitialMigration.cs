#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace ContractService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Client_PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Client_PassportIdentificationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Client_PassportNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Client_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Client_Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Client_Patronymic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Client_PassportIssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Client_BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Car_Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Car_Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Car_CarBodyStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Car_LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Car_Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Template_Version = table.Column<int>(type: "int", nullable: false),
                    Template_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Template_Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Template_ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Template_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Rental_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rental_EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rental_TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTemplates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "ContractTemplates");
        }
    }
}
