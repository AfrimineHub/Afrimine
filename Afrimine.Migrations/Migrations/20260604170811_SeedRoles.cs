using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear any existing roles first
            migrationBuilder.Sql(@"DELETE FROM ""afrimine-api-dev"".""AspNetRoles"";");

            // Insert ALL 5 roles (not Update + Insert)
            migrationBuilder.InsertData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
            { "feaf882e-49d1-4047-b8d6-79bb1217b526", "6/4/2026 5:08:11 PM", "Vendor", "VENDOR" },
            { "feaf882e-49d1-4047-b8d6-79bb1217b527", "6/4/2026 5:08:11 PM", "Buyer", "BUYER" },
            { "feaf882e-49d1-4047-b8d6-79bb1217b528", "6/4/2026 5:08:11 PM", "Support", "SUPPORT" },
            { "feaf882e-49d1-4047-b8d6-79bb1217b529", "6/4/2026 5:08:11 PM", "SuperAdmin", "SUPERADMIN" },
            { "feaf882e-49d1-4047-b8d6-79bb1217b530", "6/4/2026 5:08:11 PM", "Investor", "INVESTOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530");

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
    }
}
