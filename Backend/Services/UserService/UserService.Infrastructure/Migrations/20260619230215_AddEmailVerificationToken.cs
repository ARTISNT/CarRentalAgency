using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "EmailVerified",
                table: "users",
                newName: "email_verified");

            migrationBuilder.AddColumn<DateTime>(
                name: "verification_token_created_at",
                table: "users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "verification_token_expires_at",
                table: "users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "verification_token_hash",
                table: "users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.Sql("UPDATE users SET email_verified = 1 WHERE email_verified = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verification_token_created_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "verification_token_expires_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "verification_token_hash",
                table: "users");

            migrationBuilder.Sql("UPDATE users SET email_verified = 0;");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "email_verified",
                table: "users",
                newName: "EmailVerified");
        }
    }
}
