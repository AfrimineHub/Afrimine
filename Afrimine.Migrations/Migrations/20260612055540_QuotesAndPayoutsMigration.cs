using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class QuotesAndPayoutsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payouts",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payouts_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<string>(type: "text", nullable: false),
                    BuyerId = table.Column<string>(type: "text", nullable: false),
                    ListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotes_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotes_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotes_Listings_ListingId",
                        column: x => x.ListingId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:55:40 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:55:40 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:55:40 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:55:40 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:55:40 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:55:40 AM");

            migrationBuilder.CreateIndex(
                name: "IX_Payouts_VendorId",
                schema: "afrimine-api-dev",
                table: "Payouts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_BuyerId",
                schema: "afrimine-api-dev",
                table: "Quotes",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_ListingId",
                schema: "afrimine-api-dev",
                table: "Quotes",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_VendorId",
                schema: "afrimine-api-dev",
                table: "Quotes",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payouts",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "Quotes",
                schema: "afrimine-api-dev");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:01:12 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:01:12 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:01:12 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:01:12 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:01:12 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531",
                column: "ConcurrencyStamp",
                value: "6/12/2026 5:01:12 AM");
        }
    }
}
