using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class BookingPaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                schema: "afrimine-api-dev",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                schema: "afrimine-api-dev",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                schema: "afrimine-api-dev",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentStatusUpdatedAt",
                schema: "afrimine-api-dev",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                schema: "afrimine-api-dev",
                table: "BookingDisputes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolvedById",
                schema: "afrimine-api-dev",
                table: "BookingDisputes",
                type: "text",
                nullable: true);

            // ── Backfill ──────────────────────────────────────────────────────
            // Bookings whose PayScrow webhook was already processed (PayscrowStatus moved off
            // Pending, or a transaction id was stored) are paid. Derive the release state from
            // milestones, and mark frozen ones as Disputed.
            migrationBuilder.Sql(@"
UPDATE ""afrimine-api-dev"".""Bookings""
SET ""PaymentStatus"" = CASE
        WHEN ""FundsFrozen"" THEN 4
        WHEN (CASE WHEN ""Milestone1Status"" = 2 THEN 1 ELSE 0 END
            + CASE WHEN ""Milestone2Status"" = 2 THEN 1 ELSE 0 END
            + CASE WHEN ""Milestone3Status"" = 2 THEN 1 ELSE 0 END) = 3 THEN 3
        WHEN ""Milestone1Status"" = 2 OR ""Milestone2Status"" = 2 OR ""Milestone3Status"" = 2 THEN 2
        ELSE 1 END,
    ""PaidAt"" = ""UpdatedAt"",
    ""AmountPaid"" = ""TotalAmount"",
    ""PaymentStatusUpdatedAt"" = now()
WHERE (""PayscrowStatus"" IN (1, 2, 3) OR ""PayscrowTransactionId"" IS NOT NULL);

UPDATE ""afrimine-api-dev"".""Bookings""
SET ""PaymentStatus"" = 7, ""PaymentStatusUpdatedAt"" = now()
WHERE ""PayscrowStatus"" = 4 AND ""PaymentStatus"" = 0;

UPDATE ""afrimine-api-dev"".""BookingDisputes""
SET ""ResolvedAt"" = ""UpdatedAt""
WHERE ""Status"" IN (2, 3, 4) AND ""ResolvedAt"" IS NULL;

UPDATE ""afrimine-api-dev"".""WalletTransactions"" t
SET ""BookingId"" = substring(t.""Description"" from '([0-9a-fA-F-]{36})$')::uuid
WHERE t.""BookingId"" IS NULL
  AND t.""Description"" ~ 'booking [0-9a-fA-F-]{36}$'
  AND EXISTS (SELECT 1 FROM ""afrimine-api-dev"".""Bookings"" b
              WHERE b.""Id"" = substring(t.""Description"" from '([0-9a-fA-F-]{36})$')::uuid);
");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "11/26/2026 3:50:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "11/26/2026 3:50:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "11/26/2026 3:50:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "11/26/2026 3:50:49 AM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "11/26/2026 3:50:49 AM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                schema: "afrimine-api-dev",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                schema: "afrimine-api-dev",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "afrimine-api-dev",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentStatusUpdatedAt",
                schema: "afrimine-api-dev",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                schema: "afrimine-api-dev",
                table: "BookingDisputes");

            migrationBuilder.DropColumn(
                name: "ResolvedById",
                schema: "afrimine-api-dev",
                table: "BookingDisputes");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "11/1/2026 5:22:03 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "11/1/2026 5:22:03 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "11/1/2026 5:22:03 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "11/1/2026 5:22:03 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "11/1/2026 5:22:03 PM");
        }
    }
}
