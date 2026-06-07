using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCarId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContractAutoId",
                table: "Contracts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractAutoId",
                table: "Contracts");
        }
    }
}
