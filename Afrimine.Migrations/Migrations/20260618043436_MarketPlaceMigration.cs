using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class MarketPlaceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                schema: "afrimine-api-dev",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "afrimine-api-dev",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                schema: "afrimine-api-dev",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisputeReason",
                schema: "afrimine-api-dev",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                schema: "afrimine-api-dev",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                schema: "afrimine-api-dev",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                schema: "afrimine-api-dev",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Inquiries",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<string>(type: "text", nullable: false),
                    VendorId = table.Column<string>(type: "text", nullable: false),
                    ListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inquiries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inquiries_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inquiries_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inquiries_Listings_ListingId",
                        column: x => x.ListingId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentInsights",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentInsights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketTrends",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Commodity = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    ChangePercent = table.Column<double>(type: "double precision", nullable: false),
                    AsOf = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTrends", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rfqs",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MineralType = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    TargetPrice = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_Rfqs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rfqs_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "6/18/2026 4:34:35 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/18/2026 4:34:35 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/18/2026 4:34:35 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/18/2026 4:34:35 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "6/18/2026 4:34:35 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531",
                column: "ConcurrencyStamp",
                value: "6/18/2026 4:34:35 AM");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_VendorId",
                schema: "afrimine-api-dev",
                table: "Orders",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_BuyerId",
                schema: "afrimine-api-dev",
                table: "Inquiries",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_ListingId",
                schema: "afrimine-api-dev",
                table: "Inquiries",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_VendorId",
                schema: "afrimine-api-dev",
                table: "Inquiries",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rfqs_BuyerId",
                schema: "afrimine-api-dev",
                table: "Rfqs",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_VendorId",
                schema: "afrimine-api-dev",
                table: "Orders",
                column: "VendorId",
                principalSchema: "afrimine-api-dev",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_VendorId",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Inquiries",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "InvestmentInsights",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "MarketTrends",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "Rfqs",
                schema: "afrimine-api-dev");

            migrationBuilder.DropIndex(
                name: "IX_Orders_VendorId",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Amount",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DisputeReason",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                schema: "afrimine-api-dev",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VendorId",
                schema: "afrimine-api-dev",
                table: "Orders");

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
        }
    }
}
