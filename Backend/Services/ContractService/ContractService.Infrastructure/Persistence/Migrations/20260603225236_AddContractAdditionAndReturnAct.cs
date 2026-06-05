using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContractAdditionAndReturnAct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rental_TotalPrice",
                table: "Contracts",
                newName: "Rental_EstimatedPrice");

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "ContractTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Template_DocumentType",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ContractAdditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreviousEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdditionalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Template_Version = table.Column<int>(type: "int", nullable: false),
                    Template_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Template_Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Template_ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Template_DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Template_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractAdditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractAdditions_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractReturnActs",
                columns: table => new
                {
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mileage = table.Column<int>(type: "int", nullable: false),
                    FuelLevel = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DamageDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Template_Version = table.Column<int>(type: "int", nullable: false),
                    Template_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Template_Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Template_ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Template_DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Template_IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReturnActs", x => x.ContractId);
                    table.ForeignKey(
                        name: "FK_ContractReturnActs_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractAdditions_ContractId",
                table: "ContractAdditions",
                column: "ContractId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractAdditions");

            migrationBuilder.DropTable(
                name: "ContractReturnActs");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "ContractTemplates");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Template_DocumentType",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "Rental_EstimatedPrice",
                table: "Contracts",
                newName: "Rental_TotalPrice");
        }
    }
}
