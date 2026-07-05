using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AdminMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirth",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DocumentFileSizeBytes",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentIdNumber",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KycRejectionReason",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KycStatus",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoUrl",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RfqId",
                schema: "afrimine-api-dev",
                table: "Conversations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannedReason",
                schema: "afrimine-api-dev",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendedReason",
                schema: "afrimine-api-dev",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "7/5/2026 7:52:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "7/5/2026 7:52:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "7/5/2026 7:52:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "7/5/2026 7:52:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "7/5/2026 7:52:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531",
                column: "ConcurrencyStamp",
                value: "7/5/2026 7:52:49 AM");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_RfqId",
                schema: "afrimine-api-dev",
                table: "Conversations",
                column: "RfqId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Rfqs_RfqId",
                schema: "afrimine-api-dev",
                table: "Conversations",
                column: "RfqId",
                principalSchema: "afrimine-api-dev",
                principalTable: "Rfqs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Rfqs_RfqId",
                schema: "afrimine-api-dev",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_RfqId",
                schema: "afrimine-api-dev",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "BankName",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentFileSizeBytes",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "DocumentIdNumber",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "KycRejectionReason",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "KycStatus",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoUrl",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "RfqId",
                schema: "afrimine-api-dev",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "BannedReason",
                schema: "afrimine-api-dev",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SuspendedReason",
                schema: "afrimine-api-dev",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "6/28/2026 7:52:51 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/28/2026 7:52:51 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/28/2026 7:52:51 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/28/2026 7:52:51 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "6/28/2026 7:52:51 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531",
                column: "ConcurrencyStamp",
                value: "6/28/2026 7:52:51 PM");
        }
    }
}
