using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrimine.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SupplierMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b531");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorType",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<string>(type: "text", nullable: false),
                    MachineType = table.Column<int>(type: "integer", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    YearOfManufacture = table.Column<int>(type: "integer", nullable: false),
                    EngineHours = table.Column<int>(type: "integer", nullable: false),
                    HasCertifiedOperator = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DailyRentalRate = table.Column<decimal>(type: "numeric", nullable: false),
                    MobilizationFeePerKm = table.Column<decimal>(type: "numeric", nullable: false),
                    FrontPhotoUrl = table.Column<string>(type: "text", nullable: true),
                    SidePhotoUrl = table.Column<string>(type: "text", nullable: true),
                    SerialPlatePhotoUrl = table.Column<string>(type: "text", nullable: true),
                    FrontPhotoPublicId = table.Column<string>(type: "text", nullable: true),
                    SidePhotoPublicId = table.Column<string>(type: "text", nullable: true),
                    SerialPlatePublicId = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AspNetUsers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Operators",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    LicenseNumber = table.Column<string>(type: "text", nullable: false),
                    LicenseCategory = table.Column<string>(type: "text", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    LicenseDocumentUrl = table.Column<string>(type: "text", nullable: true),
                    LicenseDocumentPublicId = table.Column<string>(type: "text", nullable: true),
                    VettingStatus = table.Column<int>(type: "integer", nullable: false),
                    VettingAnswers = table.Column<string>(type: "text", nullable: true),
                    PassedVetting = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operators_AspNetUsers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProfiles",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    BusinessPhone = table.Column<string>(type: "text", nullable: true),
                    BusinessEmail = table.Column<string>(type: "text", nullable: true),
                    PrimaryBaseCity = table.Column<string>(type: "text", nullable: true),
                    YardAddress = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    CacCertificateUrl = table.Column<string>(type: "text", nullable: true),
                    CacCertificatePublicId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    OnboardingStep = table.Column<int>(type: "integer", nullable: false),
                    IsSubmitted = table.Column<bool>(type: "boolean", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    BankCode = table.Column<string>(type: "text", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "text", nullable: true),
                    BankAccountName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierWallets",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<string>(type: "text", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    PendingBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierWallets_AspNetUsers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinerId = table.Column<string>(type: "text", nullable: false),
                    SupplierId = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalDays = table.Column<int>(type: "integer", nullable: false),
                    DistanceKm = table.Column<double>(type: "double precision", nullable: false),
                    RentalFee = table.Column<decimal>(type: "numeric", nullable: false),
                    MobilizationFee = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PlatformFee = table.Column<decimal>(type: "numeric", nullable: false),
                    SupplierPayout = table.Column<decimal>(type: "numeric", nullable: false),
                    SiteAddress = table.Column<string>(type: "text", nullable: true),
                    SiteLatitude = table.Column<double>(type: "double precision", nullable: true),
                    SiteLongitude = table.Column<double>(type: "double precision", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DeclineReason = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    PayscrowTransactionId = table.Column<string>(type: "text", nullable: true),
                    PayscrowTransactionNumber = table.Column<string>(type: "text", nullable: true),
                    PayscrowPaymentLink = table.Column<string>(type: "text", nullable: true),
                    PayscrowTransactionReference = table.Column<string>(type: "text", nullable: true),
                    PayscrowStatus = table.Column<int>(type: "integer", nullable: false),
                    LogisticsStatus = table.Column<int>(type: "integer", nullable: false),
                    LogisticsPartnerId = table.Column<string>(type: "text", nullable: true),
                    TrackingData = table.Column<string>(type: "text", nullable: true),
                    LogisticsType = table.Column<int>(type: "integer", nullable: false),
                    ThirdPartyLogisticsRef = table.Column<string>(type: "text", nullable: true),
                    GitInsuranceActive = table.Column<bool>(type: "boolean", nullable: false),
                    InsurancePolicyNumber = table.Column<string>(type: "text", nullable: true),
                    InsuranceCertificateUrl = table.Column<string>(type: "text", nullable: true),
                    ParInsuranceActive = table.Column<bool>(type: "boolean", nullable: false),
                    ParInsurancePolicyNumber = table.Column<string>(type: "text", nullable: true),
                    Milestone1Status = table.Column<int>(type: "integer", nullable: false),
                    Milestone2Status = table.Column<int>(type: "integer", nullable: false),
                    Milestone3Status = table.Column<int>(type: "integer", nullable: false),
                    Milestone1ReleasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Milestone2ReleasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Milestone3ReleasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Milestone1Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Milestone2Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Milestone3Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_MinerId",
                        column: x => x.MinerId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetOperators",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetOperators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetOperators_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOperators_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Guarantors",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Occupation = table.Column<string>(type: "text", nullable: false),
                    IdType = table.Column<string>(type: "text", nullable: false),
                    IdNumber = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guarantors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guarantors_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_SupplierWallets_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "SupplierWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingDisputes",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RaisedById = table.Column<string>(type: "text", nullable: false),
                    RaisedByRole = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Resolution = table.Column<string>(type: "text", nullable: true),
                    PayscrowDisputeRef = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDisputes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingDisputes_AspNetUsers_RaisedById",
                        column: x => x.RaisedById,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingDisputes_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyChecks",
                schema: "afrimine-api-dev",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EngineOilChecked = table.Column<bool>(type: "boolean", nullable: false),
                    HydraulicFluidChecked = table.Column<bool>(type: "boolean", nullable: false),
                    CoolingSystemChecked = table.Column<bool>(type: "boolean", nullable: false),
                    UndercarriageChecked = table.Column<bool>(type: "boolean", nullable: false),
                    GreaseChecked = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CheckedByOperatorId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyChecks_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "afrimine-api-dev",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b526",
                column: "ConcurrencyStamp",
                value: "7/19/2026 4:00:33 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b527",
                column: "ConcurrencyStamp",
                value: "7/19/2026 4:00:33 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b528",
                column: "ConcurrencyStamp",
                value: "7/19/2026 4:00:33 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b529",
                column: "ConcurrencyStamp",
                value: "7/19/2026 4:00:33 PM");

            migrationBuilder.UpdateData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "feaf882e-49d1-4047-b8d6-79bb1217b530",
                column: "ConcurrencyStamp",
                value: "7/19/2026 4:00:33 PM");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOperators_AssetId",
                schema: "afrimine-api-dev",
                table: "AssetOperators",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOperators_OperatorId",
                schema: "afrimine-api-dev",
                table: "AssetOperators",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SupplierId",
                schema: "afrimine-api-dev",
                table: "Assets",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDisputes_BookingId",
                schema: "afrimine-api-dev",
                table: "BookingDisputes",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDisputes_RaisedById",
                schema: "afrimine-api-dev",
                table: "BookingDisputes",
                column: "RaisedById");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AssetId",
                schema: "afrimine-api-dev",
                table: "Bookings",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MinerId",
                schema: "afrimine-api-dev",
                table: "Bookings",
                column: "MinerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SupplierId",
                schema: "afrimine-api-dev",
                table: "Bookings",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyChecks_BookingId",
                schema: "afrimine-api-dev",
                table: "DailyChecks",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_OperatorId",
                schema: "afrimine-api-dev",
                table: "Guarantors",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_SupplierId",
                schema: "afrimine-api-dev",
                table: "Operators",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProfiles_UserId",
                schema: "afrimine-api-dev",
                table: "SupplierProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierWallets_SupplierId",
                schema: "afrimine-api-dev",
                table: "SupplierWallets",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                schema: "afrimine-api-dev",
                table: "WalletTransactions",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetOperators",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "BookingDisputes",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "DailyChecks",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "Guarantors",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "SupplierProfiles",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "WalletTransactions",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "Operators",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "SupplierWallets",
                schema: "afrimine-api-dev");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "afrimine-api-dev");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "VendorType",
                schema: "afrimine-api-dev",
                table: "VendorProfiles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "afrimine-api-dev",
                table: "VendorProfiles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

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

            migrationBuilder.InsertData(
                schema: "afrimine-api-dev",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "feaf882e-49d1-4047-b8d6-79bb1217b531", "7/5/2026 7:52:49 AM", "Supplier", "SUPPLIER" });
        }
    }
}
