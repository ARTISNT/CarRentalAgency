using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePdfPathFromContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "Contracts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
