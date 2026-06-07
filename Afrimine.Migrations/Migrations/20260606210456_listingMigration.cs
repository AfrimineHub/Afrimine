using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class listingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                schema: "afrimine-api-dev",
                table: "Listings",
                newName: "CategoryType");

            migrationBuilder.AddColumn<decimal>(
                name: "AcreageHectares",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminReviewNote",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Availability",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentType",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GradeOrPurity",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeaseType",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManpowerRole",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MineralType",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAmount",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceCurrency",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PriceDescription",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceUnit",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Publish",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateOrRegion",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "YearManufactured",
                schema: "afrimine-api-dev",
                table: "Listings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ListingImages",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingImages_Listings_ListingId",
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
                value: "6/6/2026 9:04:55 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/6/2026 9:04:55 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/6/2026 9:04:55 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/6/2026 9:04:55 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "6/6/2026 9:04:55 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531",
                column: "ConcurrencyStamp",
                value: "6/6/2026 9:04:55 PM");

            migrationBuilder.CreateIndex(
                name: "IX_ListingImages_ListingId",
                schema: "afrimine-api-dev",
                table: "ListingImages",
                column: "ListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListingImages",
                schema: "afrimine-api-dev");

            migrationBuilder.DropColumn(
                name: "AcreageHectares",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "AdminReviewNote",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Availability",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Condition",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ContactInfo",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "EquipmentType",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "GradeOrPurity",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "LeaseType",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ManpowerRole",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "MineralType",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PriceAmount",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PriceCurrency",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PriceDescription",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PriceUnit",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Publish",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "StateOrRegion",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "YearManufactured",
                schema: "afrimine-api-dev",
                table: "Listings");

            migrationBuilder.RenameColumn(
                name: "CategoryType",
                schema: "afrimine-api-dev",
                table: "Listings",
                newName: "Category");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "6/5/2026 5:14:27 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "6/5/2026 5:14:27 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "6/5/2026 5:14:27 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "6/5/2026 5:14:27 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "6/5/2026 5:14:27 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531",
                column: "ConcurrencyStamp",
                value: "6/5/2026 5:14:27 AM");
        }
    }
}
