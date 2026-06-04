using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UserTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "afrimine-api-dev",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "6/4/2026 12:02:22 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/4/2026 12:02:22 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/4/2026 12:02:22 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/4/2026 12:02:22 AM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "afrimine-api-dev",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "6/1/2026 7:21:39 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/1/2026 7:21:39 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/1/2026 7:21:39 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/1/2026 7:21:39 AM");
        }
    }
}
