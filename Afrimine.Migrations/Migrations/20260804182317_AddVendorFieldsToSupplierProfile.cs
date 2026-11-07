using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorFieldsToSupplierProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessType",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirth",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentFileName",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DocumentFileSizeBytes",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentIdNumber",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentPublicId",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentType",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KycRejectionReason",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KycStatus",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OfficeAddress",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoUrl",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateOrRegion",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VendorType",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "8/4/2026 6:23:16 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "8/4/2026 6:23:16 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "8/4/2026 6:23:16 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "8/4/2026 6:23:16 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "8/4/2026 6:23:16 PM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessType",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentFileName",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentFileSizeBytes",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentIdNumber",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentPublicId",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "IsComplete",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "KycRejectionReason",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "KycStatus",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "OfficeAddress",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoUrl",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "StateOrRegion",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "VendorType",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "Website",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "8/1/2026 10:52:20 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "8/1/2026 10:52:20 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "8/1/2026 10:52:20 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "8/1/2026 10:52:20 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "8/1/2026 10:52:20 AM");
        }
    }
}
