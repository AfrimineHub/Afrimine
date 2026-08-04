using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Model.ViewModels;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Configs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static Afrimine.Services.DTOs.SupplierDto;

namespace Afrimine.Services.BL.Implementation
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IRepositoryManager _repository;
        private readonly IEquipmentRepository _equipment;
        private readonly IPayscrowService _payscrow;
        private readonly ICloudinaryService _cloudinary;
        private readonly UserManager<User> _userManager;
        private readonly AppConfig _config;

        public EquipmentService(
            IRepositoryManager repository,
            IEquipmentRepository equipment,
            IPayscrowService payscrow,
            ICloudinaryService cloudinary,
            UserManager<User> userManager,
            IOptions<AppConfig> config)
        {
            _repository = repository;
            _equipment = equipment;
            _payscrow = payscrow;
            _cloudinary = cloudinary;
            _userManager = userManager;
            _config = config.Value;
        }

        // ── Supplier Registration ──────────────────────────────────────────────
        //public async Task<ApiResponse<string>> RegisterSupplierAsync(SupplierRegisterDto request)
        //{
        //    var existing = await _userManager.FindByEmailAsync(request.BusinessEmail);
        //    if (existing is not null)
        //        return ApiResponse<string>.Fail("Email already registered.", 409);

        //    var user = new User
        //    {
        //        FullName = request.FullName,
        //        UserName = request.BusinessEmail,
        //        Email = request.BusinessEmail,
        //        PhoneNumber = request.BusinessPhone,
        //        EmailConfirmed = false,
        //        Type = RoleType.Vendor,
        //        Status = AccountStatus.Pending
        //    };

        //    var result = await _userManager.CreateAsync(user, request.Password);
        //    if (!result.Succeeded)
        //        return ApiResponse<string>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)), 400);

        //    // Create supplier profile
        //    await _equipment.CreateSupplierProfileAsync(new SupplierProfile
        //    {
        //        UserId = user.Id,
        //        CompanyName = request.CompanyName,
        //        BusinessPhone = request.BusinessPhone,
        //        BusinessEmail = request.BusinessEmail,
        //        OnboardingStep = 1
        //    });

        //    // Create wallet
        //    await _equipment.CreateWalletAsync(new SupplierWallet { SupplierId = user.Id });
        //    await _repository.SaveAsync();

        //    return ApiResponse<string>.Ok("Supplier registered successfully. Please verify your phone number.");
        //}

        public async Task<ApiResponse<SupplierProfileResponseDto>> UpdateProfileAsync(string userId, SupplierProfileUpdateDto request)
        {
            var profile = await _equipment.GetSupplierProfileAsync(Guid.Parse(userId));
            if (profile is null) return ApiResponse<SupplierProfileResponseDto>.Fail("Profile not found.", 404);

            if (request.CompanyName is not null) profile.CompanyName = request.CompanyName;
            if (request.BusinessPhone is not null) profile.BusinessPhone = request.BusinessPhone;
            if (request.BusinessEmail is not null) profile.BusinessEmail = request.BusinessEmail;
            if (profile.OnboardingStep < 2) profile.OnboardingStep = 2;
            profile.UpdatedAt = DateTime.UtcNow;

            _equipment.UpdateSupplierProfile(profile);
            await _repository.SaveAsync();

            return ApiResponse<SupplierProfileResponseDto>.Ok(MapToSupplierProfileDto(profile));
        }

        public async Task<ApiResponse<string>> UpdateLocationAsync(string userId, SupplierLocationDto request)
        {
            var profile = await _equipment.GetSupplierProfileAsync(Guid.Parse(userId));
            if (profile is null) return ApiResponse<string>.Fail("Profile not found.", 404);

            profile.PrimaryBaseCity = request.PrimaryBaseCity;
            profile.YardAddress = request.YardAddress;
            profile.Latitude = request.Latitude;
            profile.Longitude = request.Longitude;
            if (profile.OnboardingStep < 3) profile.OnboardingStep = 3;
            profile.UpdatedAt = DateTime.UtcNow;

            _equipment.UpdateSupplierProfile(profile);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Location saved.");
        }

        public async Task<ApiResponse<string>> UploadDocumentAsync(string userId, SupplierDocumentUploadDto request)
        {
            var profile = await _equipment.GetSupplierProfileAsync(Guid.Parse(userId));
            if (profile is null) return ApiResponse<string>.Fail("Profile not found.", 404);

            if (!string.IsNullOrWhiteSpace(profile.CacCertificatePublicId))
                await _cloudinary.DeleteAsync(profile.CacCertificatePublicId);

            var result = await _cloudinary.UploadDocumentAsync(request.Document, $"afrimine/suppliers/{userId}/docs");
            if (!result.Success)
                return ApiResponse<string>.Fail($"Upload failed: {result.Error}", 400);

            profile.CacCertificateUrl = result.Url;
            profile.CacCertificatePublicId = result.PublicId;
            if (profile.OnboardingStep < 4) profile.OnboardingStep = 4;
            profile.UpdatedAt = DateTime.UtcNow;

            _equipment.UpdateSupplierProfile(profile);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Document uploaded.");
        }

        public async Task<ApiResponse<string>> SubmitForVerificationAsync(string userId)
        {
            var profile = await _equipment.GetSupplierProfileAsync(Guid.Parse(userId));
            if (profile is null) return ApiResponse<string>.Fail("Profile not found.", 404);

            if (profile.OnboardingStep < 4)
                return ApiResponse<string>.Fail("Please complete all onboarding steps first.", 400);

            profile.IsSubmitted = true;
            profile.Status = SupplierStatus.Pending;
            profile.OnboardingStep = 5;
            profile.UpdatedAt = DateTime.UtcNow;

            _equipment.UpdateSupplierProfile(profile);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Submitted for verification. Our field agents will contact you within 24 hours.");
        }

        public async Task<ApiResponse<SupplierStatusDto>> GetStatusAsync(string userId)
        {
            var profile = await _equipment.GetSupplierProfileAsync(Guid.Parse(userId));
            if (profile is null) return ApiResponse<SupplierStatusDto>.Fail("Profile not found.", 404);

            return ApiResponse<SupplierStatusDto>.Ok(new SupplierStatusDto
            {
                Status = profile.Status.ToString(),
                RejectionReason = profile.RejectionReason,
                OnboardingStep = profile.OnboardingStep,
                IsSubmitted = profile.IsSubmitted
            });
        }

        public async Task<ApiResponse<SupplierProfileResponseDto>> GetProfileAsync(string userId)
        {
            var profile = await _equipment.GetSupplierProfileAsync(Guid.Parse(userId));
            if (profile is null) return ApiResponse<SupplierProfileResponseDto>.Fail("Profile not found.", 404);
            return ApiResponse<SupplierProfileResponseDto>.Ok(MapToSupplierProfileDto(profile));
        }

        // ── Assets ────────────────────────────────────────────────────────────
        public async Task<ApiResponse<AssetResponseDto>> CreateAssetAsync(Guid supplierId, CreateAssetDto request)
        {
            var asset = new Asset
            {
                SupplierId = supplierId,
                MachineType = request.MachineType,
                Brand = request.Brand,
                Model = request.Model,
                YearOfManufacture = request.YearOfManufacture,
                EngineHours = request.EngineHours,
                HasCertifiedOperator = request.HasCertifiedOperator,
                DailyRentalRate = request.DailyRentalRate,
                MobilizationFeePerKm = request.MobilizationFeePerKm,
                Description = request.Description
            };

            await _equipment.CreateAssetAsync(asset);
            await _repository.SaveAsync();

            return ApiResponse<AssetResponseDto>.Ok(MapToAssetDto(asset), 201, "Asset created.");
        }

        public async Task<ApiResponse<IEnumerable<AssetResponseDto>>> GetAssetsAsync(Guid supplierId)
        {
            var assets = await _equipment.GetAssetsBySupplierAsync(supplierId);
            return ApiResponse<IEnumerable<AssetResponseDto>>.Ok(assets.Select(MapToAssetDto));
        }

        public async Task<ApiResponse<AssetResponseDto>> GetAssetAsync(Guid userId, string role, Guid assetId)
        {
            var asset = await _equipment.GetAssetAsync(assetId);
            if (asset is null)
                return ApiResponse<AssetResponseDto>.Fail("Asset not found.", 404);

            // Buyers can view any available asset
            if (role.Equals(RoleType.Buyer.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse<AssetResponseDto>.Ok(MapToAssetDto(asset));
            }

            // Vendors can only view their own assets
            if (asset.SupplierId != userId)
                return ApiResponse<AssetResponseDto>.Fail("Access denied.", 403);

            return ApiResponse<AssetResponseDto>.Ok(MapToAssetDto(asset));
        }

        public async Task<ApiResponse<AssetResponseDto>> UpdateAssetAsync(
            Guid supplierId, Guid assetId, UpdateAssetDto request)
        {
            var asset = await _equipment.GetAssetAsync(assetId);
            if (asset is null) return ApiResponse<AssetResponseDto>.Fail("Asset not found.", 404);
            if (asset.SupplierId != supplierId) return ApiResponse<AssetResponseDto>.Fail("Access denied.", 403);

            if (request.Brand is not null) asset.Brand = request.Brand;
            if (request.Model is not null) asset.Model = request.Model;
            if (request.EngineHours.HasValue) asset.EngineHours = request.EngineHours.Value;
            if (request.DailyRentalRate.HasValue) asset.DailyRentalRate = request.DailyRentalRate.Value;
            if (request.MobilizationFeePerKm.HasValue) asset.MobilizationFeePerKm = request.MobilizationFeePerKm.Value;
            if (request.Description is not null) asset.Description = request.Description;
            if (request.Status.HasValue) asset.Status = request.Status.Value;
            asset.UpdatedAt = DateTime.UtcNow;

            _equipment.UpdateAsset(asset);
            await _repository.SaveAsync();

            return ApiResponse<AssetResponseDto>.Ok(MapToAssetDto(asset));
        }

        public async Task<ApiResponse<string>> DeleteAssetAsync(Guid supplierId, Guid assetId)
        {
            var asset = await _equipment.GetAssetAsync(assetId);
            if (asset is null) return ApiResponse<string>.Fail("Asset not found.", 404);
            if (asset.SupplierId != supplierId) return ApiResponse<string>.Fail("Access denied.", 403);

            asset.IsDeleted = true;
            asset.UpdatedAt = DateTime.UtcNow;
            _equipment.UpdateAsset(asset);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Asset removed.");
        }

        public async Task<ApiResponse<AssetResponseDto>> UploadAssetPhotosAsync(
            Guid supplierId, Guid assetId, AssetPhotoUploadDto request)
        {
            var asset = await _equipment.GetAssetAsync(assetId);
            if (asset is null) return ApiResponse<AssetResponseDto>.Fail("Asset not found.", 404);
            if (asset.SupplierId != supplierId) return ApiResponse<AssetResponseDto>.Fail("Access denied.", 403);

            if (request.FrontPhoto is not null)
            {
                if (!string.IsNullOrWhiteSpace(asset.FrontPhotoPublicId))
                    await _cloudinary.DeleteAsync(asset.FrontPhotoPublicId);
                var r = await _cloudinary.UploadImageAsync(request.FrontPhoto, $"afrimine/assets/{assetId}");
                if (r.Success) { asset.FrontPhotoUrl = r.Url; asset.FrontPhotoPublicId = r.PublicId; }
            }

            if (request.SidePhoto is not null)
            {
                if (!string.IsNullOrWhiteSpace(asset.SidePhotoPublicId))
                    await _cloudinary.DeleteAsync(asset.SidePhotoPublicId);
                var r = await _cloudinary.UploadImageAsync(request.SidePhoto, $"afrimine/assets/{assetId}");
                if (r.Success) { asset.SidePhotoUrl = r.Url; asset.SidePhotoPublicId = r.PublicId; }
            }

            if (request.SerialPlatePhoto is not null)
            {
                if (!string.IsNullOrWhiteSpace(asset.SerialPlatePublicId))
                    await _cloudinary.DeleteAsync(asset.SerialPlatePublicId);
                var r = await _cloudinary.UploadImageAsync(request.SerialPlatePhoto, $"afrimine/assets/{assetId}");
                if (r.Success) { asset.SerialPlatePhotoUrl = r.Url; asset.SerialPlatePublicId = r.PublicId; }
            }

            asset.UpdatedAt = DateTime.UtcNow;
            _equipment.UpdateAsset(asset);
            await _repository.SaveAsync();

            return ApiResponse<AssetResponseDto>.Ok(MapToAssetDto(asset));
        }

        public async Task<ApiResponse<AssetPricingDto>> GetAssetPricingAsync(
            Guid assetId, int totalDays, double distanceKm, string currency = "NGN")
        {
            var asset = await _equipment.GetAssetAsync(assetId);
            if (asset is null) return ApiResponse<AssetPricingDto>.Fail("Asset not found.", 404);

            var rentalFee = asset.DailyRentalRate * totalDays;
            var mobilizationFee = asset.MobilizationFeePerKm * (decimal)distanceKm;
            var total = rentalFee + mobilizationFee;
            var platformFee = Math.Round(total * 0.15m, 2);
            var supplierPayout = total - platformFee;

            // Get PayScrow charges preview (miner bears charge = merchantChargePercentage 0)
            var charges = await _payscrow.CalculateChargesAsync(currency, total, 0);

            return ApiResponse<AssetPricingDto>.Ok(new AssetPricingDto
            {
                TotalDays = totalDays,
                DistanceKm = distanceKm,
                DailyRentalRate = asset.DailyRentalRate,
                RentalFee = rentalFee,
                MobilizationFee = mobilizationFee,
                PlatformFee = platformFee,
                TotalAmount = total,
                SupplierPayout = supplierPayout,
                Currency = currency,
                PayscrowCharge = charges.Success ? charges.CustomerCharge : 0,
                TotalPayable = charges.Success ? charges.GrandTotalPayable : total
            });
        }

        // ── Operators ─────────────────────────────────────────────────────────
        public async Task<ApiResponse<OperatorResponseDto>> CreateOperatorAsync(
            Guid supplierId, CreateOperatorDto request)
        {
            var op = new Operator
            {
                SupplierId = supplierId,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                LicenseNumber = request.LicenseNumber,
                LicenseCategory = request.LicenseCategory,
                YearsOfExperience = request.YearsOfExperience
            };

            if (request.LicenseDocument is not null)
            {
                var r = await _cloudinary.UploadDocumentAsync(request.LicenseDocument, $"afrimine/operators/{supplierId}");
                if (r.Success) { op.LicenseDocumentUrl = r.Url; op.LicenseDocumentPublicId = r.PublicId; }
            }

            await _equipment.CreateOperatorAsync(op);
            await _repository.SaveAsync();

            return ApiResponse<OperatorResponseDto>.Ok(MapToOperatorDto(op), 201, "Operator added.");
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponseDto>>> GetOperatorsAsync(Guid supplierId)
        {
            var operators = await _equipment.GetOperatorsBySupplierAsync(supplierId);
            return ApiResponse<IEnumerable<OperatorResponseDto>>.Ok(operators.Select(MapToOperatorDto));
        }

        public async Task<ApiResponse<OperatorResponseDto>> UpdateOperatorAsync(
            Guid supplierId, Guid operatorId, CreateOperatorDto request)
        {
            var op = await _equipment.GetOperatorAsync(operatorId);
            if (op is null) return ApiResponse<OperatorResponseDto>.Fail("Operator not found.", 404);
            if (op.SupplierId != supplierId) return ApiResponse<OperatorResponseDto>.Fail("Access denied.", 403);

            op.FullName = request.FullName;
            op.PhoneNumber = request.PhoneNumber;
            op.LicenseNumber = request.LicenseNumber;
            op.YearsOfExperience = request.YearsOfExperience;
            op.UpdatedAt = DateTime.UtcNow;

            _equipment.UpdateOperator(op);
            await _repository.SaveAsync();

            return ApiResponse<OperatorResponseDto>.Ok(MapToOperatorDto(op));
        }

        public async Task<ApiResponse<string>> AssignOperatorToAssetAsync(
            Guid supplierId, Guid assetId, Guid operatorId)
        {
            var asset = await _equipment.GetAssetAsync(assetId);
            if (asset is null || asset.SupplierId != supplierId)
                return ApiResponse<string>.Fail("Asset not found.", 404);

            var op = await _equipment.GetOperatorAsync(operatorId);
            if (op is null || op.SupplierId != supplierId)
                return ApiResponse<string>.Fail("Operator not found.", 404);

            await _equipment.AssignOperatorAsync(new AssetOperator
            {
                AssetId = assetId,
                OperatorId = operatorId,
                IsPrimary = true
            });
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Operator assigned.");
        }

        public async Task<ApiResponse<string>> AddGuarantorAsync(
            Guid supplierId, Guid operatorId, CreateGuarantorDto request)
        {
            var op = await _equipment.GetOperatorAsync(operatorId);
            if (op is null || op.SupplierId != supplierId)
                return ApiResponse<string>.Fail("Operator not found.", 404);

            var existingCount = await _equipment.CountGuarantorsAsync(operatorId);
            if (existingCount >= 2)
                return ApiResponse<string>.Fail("Maximum of 2 guarantors allowed per operator.", 400);

            await _equipment.AddGuarantorAsync(new Guarantor
            {
                OperatorId = operatorId,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Occupation = request.Occupation,
                IdType = request.IdType,
                IdNumber = request.IdNumber
            });
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Guarantor added.");
        }

        public async Task<ApiResponse<string>> SubmitVettingAsync(
            Guid supplierId, Guid operatorId, VettingSubmitDto request)
        {
            var op = await _equipment.GetOperatorAsync(operatorId);
            if (op is null || op.SupplierId != supplierId)
                return ApiResponse<string>.Fail("Operator not found.", 404);

            var answers = JsonSerializer.Serialize(request);
            op.VettingAnswers = answers;
            op.VettingStatus = VettingStatus.Submitted;

            // Auto-score: pass if experience >= 3 years and answers are substantive
            var passed = op.YearsOfExperience >= 3
                && !string.IsNullOrWhiteSpace(request.TerrainKnowledgeAnswer)
                && request.TerrainKnowledgeAnswer.Length >= 20;

            op.PassedVetting = passed;
            op.VettingStatus = passed ? VettingStatus.Passed : VettingStatus.Failed;
            op.UpdatedAt = DateTime.UtcNow;

            _equipment.UpdateOperator(op);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok(passed ? "Vetting passed." : "Vetting failed. Minimum 3 years experience and detailed answers required.");
        }

        public async Task<ApiResponse<VettingStatusDto>> GetVettingStatusAsync(Guid operatorId)
        {
            var op = await _equipment.GetOperatorAsync(operatorId);
            if (op is null) return ApiResponse<VettingStatusDto>.Fail("Operator not found.", 404);

            return ApiResponse<VettingStatusDto>.Ok(new VettingStatusDto
            {
                Status = op.VettingStatus.ToString(),
                PassedVetting = op.PassedVetting,
                Feedback = op.PassedVetting
                    ? "Operator passed all vetting criteria."
                    : "Operator must have 3+ years experience and provide detailed terrain knowledge answers."
            });
        }

        // ── Bookings ──────────────────────────────────────────────────────────
        public async Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(string minerId, CreateBookingDto request)
        {
            var asset = await _equipment.GetAssetAsync(request.AssetId);
            if (asset is null) return ApiResponse<BookingResponseDto>.Fail("Asset not found.", 404);
            if (asset.Status != AssetStatus.Available)
                return ApiResponse<BookingResponseDto>.Fail("Asset is not available.", 409);

            var miner = await _userManager.FindByIdAsync(minerId);
            if (miner is null) return ApiResponse<BookingResponseDto>.Fail("Miner not found.", 404);

            var totalDays = (int)(request.EndDate - request.StartDate).TotalDays;
            if (totalDays <= 0) return ApiResponse<BookingResponseDto>.Fail("Invalid date range.", 400);

            var rentalFee = asset.DailyRentalRate * totalDays;
            var mobilizationFee = asset.MobilizationFeePerKm * (decimal)request.DistanceKm;
            var total = rentalFee + mobilizationFee;
            var platformFee = Math.Round(total * 0.15m, 2);
            var supplierPayout = total - platformFee;
            var milestone1 = Math.Round(supplierPayout * 0.20m, 2);
            var milestone2 = Math.Round(supplierPayout * 0.40m, 2);
            var milestone3 = supplierPayout - milestone1 - milestone2;

            var txRef = $"AFRIMINE-{Guid.NewGuid().ToString("N")[..12].ToUpperInvariant()}";

            // Get supplier profile for bank details
            var supplierProfile = await _equipment.GetSupplierProfileAsync(asset.SupplierId);

            // Create PayScrow transaction
            var payscrowRequest = new PayscrowCreateTransactionRequest
            {
                TransactionReference = txRef,
                MerchantEmailAddress = supplierProfile?.BusinessEmail ?? string.Empty,
                MerchantPhoneNo = supplierProfile?.BusinessPhone,
                MerchantName = supplierProfile?.CompanyName ?? "Equipment Supplier",
                CustomerEmailAddress = miner.Email!,
                CustomerPhoneNo = request.MinerPhone,
                CustomerName = miner.FullName,
                CurrencyCode = request.Currency,
                MerchantChargePercentage = 0, // miner bears charge
                ReturnUrl = _config.PayscrowReturnUrl,
                WebhookNotificationUrl = _config.PayscrowWebhookUrl,
                Items = new List<PayscrowItem>
                {
                    new()
                    {
                        Name = $"{asset.Brand} {asset.Model} Rental",
                        Description = $"{totalDays} day(s) rental + mobilization",
                        Quantity = 1,
                        Price = total
                    }
                }
            };

            // Add settlement account if supplier has bank details
            if (!string.IsNullOrWhiteSpace(supplierProfile?.BankCode)
                && !string.IsNullOrWhiteSpace(supplierProfile?.BankAccountNumber))
            {
                // Calculate charges first to get correct settlement amount
                var charges = await _payscrow.CalculateChargesAsync(request.Currency, total, 0);
                if (charges.Success)
                {
                    payscrowRequest.SettlementAccounts = new List<PayscrowSettlementAccount>
                    {
                        new()
                        {
                            BankCode = supplierProfile.BankCode,
                            AccountNumber = supplierProfile.BankAccountNumber,
                            AccountName = supplierProfile.BankAccountName ?? string.Empty,
                            Amount = charges.TotalSettlementAmount
                        }
                    };
                }
            }

            var payscrowResult = await _payscrow.CreateTransactionAsync(payscrowRequest);

            var booking = new Booking
            {
                AssetId = request.AssetId,
                MinerId = minerId,
                SupplierId = asset.SupplierId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalDays = totalDays,
                DistanceKm = request.DistanceKm,
                RentalFee = rentalFee,
                MobilizationFee = mobilizationFee,
                TotalAmount = total,
                PlatformFee = platformFee,
                SupplierPayout = supplierPayout,
                SiteAddress = request.SiteAddress,
                SiteLatitude = request.SiteLatitude,
                SiteLongitude = request.SiteLongitude,
                Currency = request.Currency,
                Milestone1Amount = milestone1,
                Milestone2Amount = milestone2,
                Milestone3Amount = milestone3,
                PayscrowTransactionReference = txRef,
                PayscrowTransactionNumber = payscrowResult.TransactionNumber,
                PayscrowPaymentLink = payscrowResult.PaymentLink,
                PayscrowStatus = PayscrowTransactionStatus.Pending,
                LogisticsType = request.LogisticsType
            };

            await _equipment.CreateBookingAsync(booking);
            await _repository.SaveAsync();

            var dto = MapToBookingDto(booking);
            dto.PaymentLink = payscrowResult.Success ? payscrowResult.PaymentLink : null;

            if (!payscrowResult.Success)
                return ApiResponse<BookingResponseDto>.Ok(dto, 201,
                    $"Booking created but payment link generation failed: {payscrowResult.Error}. Contact support.");

            return ApiResponse<BookingResponseDto>.Ok(dto, 201, "Booking created. Share payment link with miner.");
        }

        public async Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetBookingsAsync(
            string userId, string? status)
        {
            BookingStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BookingStatus>(status, true, out var s))
                statusEnum = s;

            var bookings = await _equipment.GetBookingsAsync(userId, statusEnum);
            return ApiResponse<IEnumerable<BookingResponseDto>>.Ok(bookings.Select(MapToBookingDto));
        }

        public async Task<ApiResponse<BookingDetailDto>> GetBookingDetailAsync(string userId, Guid bookingId)
        {
            var supplierId = Guid.Parse(userId);
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<BookingDetailDto>.Fail("Booking not found.", 404);
            if (booking.MinerId != userId && booking.SupplierId != supplierId)
                return ApiResponse<BookingDetailDto>.Fail("Access denied.", 403);

            return ApiResponse<BookingDetailDto>.Ok(MapToBookingDetailDto(booking));
        }

        public async Task<ApiResponse<BookingResponseDto>> ApproveBookingAsync(Guid supplierId, Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<BookingResponseDto>.Fail("Booking not found.", 404);
            if (booking.SupplierId != supplierId) return ApiResponse<BookingResponseDto>.Fail("Access denied.", 403);
            if (booking.Status != BookingStatus.Pending)
                return ApiResponse<BookingResponseDto>.Fail("Booking is not in pending state.", 409);

            booking.Status = BookingStatus.Approved;
            booking.UpdatedAt = DateTime.UtcNow;
            _equipment.UpdateBooking(booking);

            // Mark asset as rented
            var asset = await _equipment.GetAssetAsync(booking.AssetId);
            if (asset is not null)
            {
                asset.Status = AssetStatus.Rented;
                _equipment.UpdateAsset(asset);
            }

            await _repository.SaveAsync();
            return ApiResponse<BookingResponseDto>.Ok(MapToBookingDto(booking));
        }

        public async Task<ApiResponse<string>> DeclineBookingAsync(
            Guid supplierId, Guid bookingId, DeclineBookingDto request)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<string>.Fail("Booking not found.", 404);
            if (booking.SupplierId != supplierId) return ApiResponse<string>.Fail("Access denied.", 403);

            booking.Status = BookingStatus.Declined;
            booking.DeclineReason = request.Reason;
            booking.UpdatedAt = DateTime.UtcNow;
            _equipment.UpdateBooking(booking);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Booking declined.");
        }

        // ── Logistics & Milestones ─────────────────────────────────────────────
        public async Task<ApiResponse<string>> DispatchBookingAsync(Guid supplierId, Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<string>.Fail("Booking not found.", 404);
            if (booking.SupplierId != supplierId) return ApiResponse<string>.Fail("Access denied.", 403);
            if (booking.Status != BookingStatus.Approved)
                return ApiResponse<string>.Fail("Booking must be approved before dispatch.", 409);

            booking.LogisticsStatus = LogisticsStatus.Dispatched;
            booking.Status = BookingStatus.Active;
            booking.UpdatedAt = DateTime.UtcNow;

            if (booking.LogisticsType == LogisticsType.SupplierOwned)
            {
                // Equipment business: supplier uses their own low-bed truck
                // No external call needed — supplier manages their own transport
                _equipment.UpdateBooking(booking);
                await _repository.SaveAsync();
                return ApiResponse<string>.Ok("Dispatch confirmed. You are responsible for delivering the equipment to site.");
            }
            else
            {
                // Minerals business: platform pings nearest vetted 3PL partner
                // TODO: Call 3PL partner API here when partnerships are established
                booking.ThirdPartyLogisticsRef = $"3PL-{Guid.NewGuid():N[..8].ToUpper()}";
                _equipment.UpdateBooking(booking);
                await _repository.SaveAsync();
                return ApiResponse<string>.Ok("3PL logistics partner has been notified for pickup and delivery.");
            }
        }

        public async Task<ApiResponse<LogisticsStatusDto>> GetLogisticsStatusAsync(Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<LogisticsStatusDto>.Fail("Booking not found.", 404);

            return ApiResponse<LogisticsStatusDto>.Ok(new LogisticsStatusDto
            {
                Status = booking.LogisticsStatus.ToString(),
                TrackingData = booking.TrackingData,
                GitInsuranceActive = booking.GitInsuranceActive,
                InsuranceCertificateUrl = booking.InsuranceCertificateUrl
            });
        }

        public async Task<ApiResponse<TrackingDto>> GetTrackingAsync(Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<TrackingDto>.Fail("Booking not found.", 404);

            TrackingDto? tracking = null;
            if (!string.IsNullOrWhiteSpace(booking.TrackingData))
                tracking = JsonSerializer.Deserialize<TrackingDto>(booking.TrackingData);

            return ApiResponse<TrackingDto>.Ok(tracking ?? new TrackingDto { Status = "No tracking data available" });
        }

        public async Task<ApiResponse<string>> TriggerInsuranceAsync(Guid supplierId, Guid bookingId, string type)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<string>.Fail("Booking not found.", 404);
            if (booking.SupplierId != supplierId) return ApiResponse<string>.Fail("Access denied.", 403);

            // TODO: Call Leadway/AXA API
            if (type.ToUpper() == "PAR")
            {
                booking.ParInsuranceActive = true;
                booking.ParInsurancePolicyNumber = $"PAR-{Guid.NewGuid():N[..8].ToUpper()}";
            }
            else
            {
                booking.GitInsuranceActive = true;
                booking.InsurancePolicyNumber = $"GIT-{Guid.NewGuid():N[..8].ToUpper()}";
            }

            booking.UpdatedAt = DateTime.UtcNow;
            _equipment.UpdateBooking(booking);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok($"{type.ToUpper()} Insurance activated.");
        }

        public async Task<ApiResponse<string>> SiteArrivalSignOffAsync(Guid supplierId, Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<string>.Fail("Booking not found.", 404);

            if (booking.SupplierId != supplierId)
                return ApiResponse<string>.Fail("Access denied.", 403);

            booking.LogisticsStatus = LogisticsStatus.Arrived;
            booking.Milestone1Status = MilestoneStatus.Pending;
            booking.UpdatedAt = DateTime.UtcNow;
            _equipment.UpdateBooking(booking);

            // Release Milestone 1 (20%) to supplier wallet
            await ReleaseMilestoneAsync(booking, 1);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Site arrival confirmed. Milestone 1 (20%) released to supplier.");
        }

        public async Task<ApiResponse<string>> SubmitDailyCheckAsync(
Guid supplierId, Guid bookingId, DailyCheckDto request)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<string>.Fail("Booking not found.", 404);

            await _equipment.AddDailyCheckAsync(new DailyCheck
            {
                BookingId = bookingId,
                CheckDate = DateTime.UtcNow,
                EngineOilChecked = request.EngineOilChecked,
                HydraulicFluidChecked = request.HydraulicFluidChecked,
                CoolingSystemChecked = request.CoolingSystemChecked,
                UndercarriageChecked = request.UndercarriageChecked,
                GreaseChecked = request.GreaseChecked,
                Notes = request.Notes,
                CheckedByOperatorId = request.CheckedByOperatorId
            });

            // Auto-release Milestone 2 on Day 5 if no active disputes
            var dayNumber = (int)(DateTime.UtcNow - booking.StartDate).TotalDays + 1;
            if (dayNumber >= 5 && booking.Milestone2Status == MilestoneStatus.Locked)
            {
                var hasDispute = booking.Disputes.Any(d => d.Status == DisputeStatus.Open);
                if (!hasDispute)
                {
                    booking.Milestone2Status = MilestoneStatus.Pending;
                    await ReleaseMilestoneAsync(booking, 2);
                    _equipment.UpdateBooking(booking);
                }
            }

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Daily check submitted.");
        }

        public async Task<ApiResponse<IEnumerable<MilestoneDto>>> GetMilestonesAsync(Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<IEnumerable<MilestoneDto>>.Fail("Booking not found.", 404);

            return ApiResponse<IEnumerable<MilestoneDto>>.Ok(new List<MilestoneDto>
            {
                new() { Name = "Milestone 1 - Site Arrival (20%)", Amount = booking.Milestone1Amount,
                    Status = booking.Milestone1Status.ToString(), ReleasedAt = booking.Milestone1ReleasedAt,
                    Description = "Released upon confirmed arrival at mining site." },
                new() { Name = "Milestone 2 - Mid-Term (40%)", Amount = booking.Milestone2Amount,
                    Status = booking.Milestone2Status.ToString(), ReleasedAt = booking.Milestone2ReleasedAt,
                    Description = "Released automatically on Day 5 if no disputes." },
                new() { Name = "Milestone 3 - Completion (40%)", Amount = booking.Milestone3Amount,
                    Status = booking.Milestone3Status.ToString(), ReleasedAt = booking.Milestone3ReleasedAt,
                    Description = "Released on lease completion and return clearance." }
            });
        }

        public async Task<ApiResponse<string>> ReturnClearanceAsync(string userId, Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<string>.Fail("Booking not found.", 404);

            var supplierId = Guid.Parse(userId);
            if (booking.SupplierId != supplierId && booking.MinerId != userId)
                return ApiResponse<string>.Fail("Access denied.", 403);

            booking.Status = BookingStatus.Completed;
            booking.LogisticsStatus = LogisticsStatus.Returned;
            booking.Milestone3Status = MilestoneStatus.Pending;
            booking.UpdatedAt = DateTime.UtcNow;
            _equipment.UpdateBooking(booking);

            // Release Milestone 3 (40%)
            await ReleaseMilestoneAsync(booking, 3);

            // Free up asset
            var asset = await _equipment.GetAssetAsync(booking.AssetId);
            if (asset is not null) { asset.Status = AssetStatus.Available; _equipment.UpdateAsset(asset); }

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Return clearance confirmed. Milestone 3 (40%) released.");
        }

        public async Task<ApiResponse<PaymentBreakdownDto>> GetPaymentBreakdownAsync(Guid bookingId)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<PaymentBreakdownDto>.Fail("Booking not found.", 404);

            return ApiResponse<PaymentBreakdownDto>.Ok(new PaymentBreakdownDto
            {
                TotalEscrow = booking.TotalAmount,
                PlatformFee = booking.PlatformFee,
                SupplierShare = booking.SupplierPayout,
                Milestone1Amount = booking.Milestone1Amount,
                Milestone2Amount = booking.Milestone2Amount,
                Milestone3Amount = booking.Milestone3Amount,
                Currency = booking.Currency ?? "NGN"
            });
        }

        // ── Disputes ──────────────────────────────────────────────────────────
        public async Task<ApiResponse<string>> RaiseDisputeAsync(
            string userId, Guid bookingId, BookingDisputeDto request)
        {
            var booking = await _equipment.GetBookingDetailAsync(bookingId);
            if (booking is null) return ApiResponse<string>.Fail("Booking not found.", 404);

             var supplierId = Guid.Parse(userId);

            if (booking.MinerId != userId && booking.SupplierId != supplierId)
                return ApiResponse<string>.Fail("Access denied.", 403);

            var dispute = new BookingDispute
            {
                BookingId = bookingId,
                RaisedById = userId,
                RaisedByRole = request.RaisedByRole,
                Description = request.Description,
                Status = DisputeStatus.Open
            };

            // Also raise on PayScrow if transaction exists
            if (!string.IsNullOrWhiteSpace(booking.PayscrowTransactionNumber))
            {
                var raised = await _payscrow.RaiseDisputeAsync(
                    booking.PayscrowTransactionNumber, request.RaisedByRole, request.Description);
                if (raised) dispute.PayscrowDisputeRef = booking.PayscrowTransactionNumber;
            }

            await _equipment.AddDisputeAsync(dispute);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Dispute raised. Our team will review within 24 hours.");
        }

        public async Task<ApiResponse<IEnumerable<BookingDisputeResponseDto>>> GetBookingDisputesAsync(Guid bookingId)
        {
            var disputes = await _equipment.GetBookingDisputesAsync(bookingId);
            return ApiResponse<IEnumerable<BookingDisputeResponseDto>>.Ok(disputes.Select(MapToDisputeDto));
        }

        public async Task<ApiResponse<IEnumerable<BookingDisputeResponseDto>>> GetAllSupplierDisputesAsync(Guid supplierId)
        {
            var disputes = await _equipment.GetSupplierDisputesAsync(supplierId);
            return ApiResponse<IEnumerable<BookingDisputeResponseDto>>.Ok(disputes.Select(MapToDisputeDto));
        }

        // ── Wallet ────────────────────────────────────────────────────────────
        public async Task<ApiResponse<WalletBalanceDto>> GetWalletBalanceAsync(Guid supplierId)
        {
            var wallet = await _equipment.GetWalletAsync(supplierId);
            if (wallet is null) return ApiResponse<WalletBalanceDto>.Fail("Wallet not found.", 404);

            return ApiResponse<WalletBalanceDto>.Ok(new WalletBalanceDto
            {
                AvailableBalance = wallet.AvailableBalance,
                PendingBalance = wallet.PendingBalance,
                Currency = wallet.Currency
            });
        }

        public async Task<ApiResponse<string>> RequestWithdrawalAsync(Guid supplierId, WithdrawalRequestDto request)
        {
            var wallet = await _equipment.GetWalletAsync(supplierId);
            if (wallet is null) return ApiResponse<string>.Fail("Wallet not found.", 404);
            if (wallet.AvailableBalance < request.Amount)
                return ApiResponse<string>.Fail("Insufficient balance.", 400);

            wallet.AvailableBalance -= request.Amount;
            _equipment.UpdateWallet(wallet);

            await _equipment.AddWalletTransactionAsync(new WalletTransaction
            {
                WalletId = wallet.Id,
                Type = "debit",
                Amount = request.Amount,
                Description = "Withdrawal request",
                Currency = request.Currency
            });

            // Also create a payout record
            await _repository.Payout.Create(new Payout
            {
                VendorId = supplierId.ToString(),
                Amount = request.Amount,
                Currency = request.Currency,
                Status = PayoutStatus.Pending
            });

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Withdrawal request submitted. Processing within 24-48 hours.");
        }

        public async Task<ApiResponse<IEnumerable<WalletTransactionDto>>> GetWalletTransactionsAsync(Guid supplierId)
        {
            var transactions = await _equipment.GetWalletTransactionsAsync(supplierId);
            return ApiResponse<IEnumerable<WalletTransactionDto>>.Ok(transactions.Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                Type = t.Type,
                Amount = t.Amount,
                Description = t.Description,
                Reference = t.Reference,
                Currency = t.Currency,
                CreatedAt = t.CreatedAt
            }));
        }

        // ── Dashboard ─────────────────────────────────────────────────────────
        public async Task<ApiResponse<SupplierDashboardStatsDto>> GetDashboardStatsAsync(Guid supplierId)
        {
            var totalMachines = await _equipment.CountAssetsAsync(supplierId);
            var activeLeases = await _equipment.CountActiveBookingsAsync(supplierId);
            var wallet = await _equipment.GetWalletAsync(supplierId);

            return ApiResponse<SupplierDashboardStatsDto>.Ok(new SupplierDashboardStatsDto
            {
                TotalMachines = totalMachines,
                ActiveLeases = activeLeases,
                CurrentMonthEarnings = wallet?.AvailableBalance ?? 0,
                PendingEscrow = wallet?.PendingBalance ?? 0,
                Currency = "NGN"
            });
        }

        // ── PayScrow ──────────────────────────────────────────────────────────
        public async Task<ApiResponse<string>> ApplyEscrowCodeAsync(string userId, EscrowCodeApplyDto request)
        {
            var success = await _payscrow.ApplyEscrowCodeAsync(request.TransactionId, request.Code);
            return success
                ? ApiResponse<string>.Ok("Escrow code applied. Funds released.")
                : ApiResponse<string>.Fail("Invalid escrow code or transaction not found.", 400);
        }

        public async Task<ApiResponse<object>> GetPayscrowTransactionStatusAsync(string transactionNumber)
        {
            var result = await _payscrow.GetTransactionStatusAsync(transactionNumber);
            return result.Success
                ? ApiResponse<object>.Ok(result)
                : ApiResponse<object>.Fail(result.Error ?? "Failed to get status.", 400);
        }

        public async Task<ApiResponse<object>> CalculatePayscrowChargesAsync(
            string currencyCode, decimal amount, decimal merchantChargePercentage = 0)
        {
            var result = await _payscrow.CalculateChargesAsync(currencyCode, amount, merchantChargePercentage);
            return result.Success
                ? ApiResponse<object>.Ok(result)
                : ApiResponse<object>.Fail(result.Error ?? "Failed to calculate charges.", 400);
        }

        public async Task<ApiResponse<IEnumerable<PayscrowBank>>> GetSupportedBanksAsync()
        {
            var banks = await _payscrow.GetSupportedBanksAsync();
            return ApiResponse<IEnumerable<PayscrowBank>>.Ok(banks);
        }

        // ── Webhook handler ───────────────────────────────────────────────────
        public async Task HandlePayscrowWebhookAsync(PayscrowWebhookPayload payload)
        {
            var booking = await _equipment.GetBookingByPayscrowRefAsync(payload.ExternalReference);
            if (booking is null) return;

            if (payload.PaymentStatus == "Paid" && payload.EscrowStatus == "InEscrow")
            {
                booking.PayscrowTransactionId = payload.TransactionId;
                booking.PayscrowStatus = PayscrowTransactionStatus.InProgress;
                booking.Status = BookingStatus.Active;
                booking.UpdatedAt = DateTime.UtcNow;

                // Add to supplier pending wallet balance
                var wallet = await _equipment.GetWalletAsync(booking.SupplierId);
                if (wallet is not null)
                {
                    wallet.PendingBalance += booking.SupplierPayout;
                    _equipment.UpdateWallet(wallet);
                    await _equipment.AddWalletTransactionAsync(new WalletTransaction
                    {
                        WalletId = wallet.Id,
                        Type = "credit",
                        Amount = booking.SupplierPayout,
                        Description = $"Escrow locked for booking {booking.Id}",
                        Reference = payload.TransactionNumber,
                        Currency = booking.Currency ?? "NGN"
                    });
                }

                _equipment.UpdateBooking(booking);
                await _repository.SaveAsync();
            }
        }

        public async Task<ApiResponse<PagedResultDto<AssetResponseDto>>> SearchAssetsAsync(string? q, MachineType? machineType, string? location,decimal? maxDailyRate, bool availableOnly,
    int page, int pageSize)
        {
            var (items, total) = await _equipment.SearchAssetsAsync(
                q, machineType, location, maxDailyRate, availableOnly, page, pageSize);

            var assetList = items.ToList();

            // Fetch supplier profiles for location data
            var supplierIds = assetList.Select(a => a.SupplierId).Distinct().ToList();
            var profiles = await _repository.Profile.GetByIdsAsync(supplierIds);
            var profileMap = profiles.ToDictionary(p => p.Id, p => p);

            var dtos = assetList.Select(a =>
            {
                var dto = MapToAssetDto(a);
                if (profileMap.TryGetValue(a.SupplierId, out var profile))
                {
                    dto.SupplierCity = profile.StateOrRegion;
                    dto.SupplierYardAddress = profile.OfficeAddress;
                }
                return dto;
            });

            return ApiResponse<PagedResultDto<AssetResponseDto>>.Ok(new PagedResultDto<AssetResponseDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }


        // ── Private helpers ───────────────────────────────────────────────────
        private async Task ReleaseMilestoneAsync(Booking booking, int milestoneNumber)
        {
            var wallet = await _equipment.GetWalletAsync(booking.SupplierId);
            if (wallet is null) return;

            decimal amount = milestoneNumber switch
            {
                1 => booking.Milestone1Amount,
                2 => booking.Milestone2Amount,
                3 => booking.Milestone3Amount,
                _ => 0
            };

            wallet.PendingBalance = Math.Max(0, wallet.PendingBalance - amount);
            wallet.AvailableBalance += amount;
            _equipment.UpdateWallet(wallet);

            await _equipment.AddWalletTransactionAsync(new WalletTransaction
            {
                WalletId = wallet.Id,
                Type = "credit",
                Amount = amount,
                Description = $"Milestone {milestoneNumber} released for booking {booking.Id}",
                Reference = booking.PayscrowTransactionNumber,
                Currency = booking.Currency ?? "NGN"
            });

            switch (milestoneNumber)
            {
                case 1:
                    booking.Milestone1Status = MilestoneStatus.Released;
                    booking.Milestone1ReleasedAt = DateTime.UtcNow;
                    break;
                case 2:
                    booking.Milestone2Status = MilestoneStatus.Released;
                    booking.Milestone2ReleasedAt = DateTime.UtcNow;
                    break;
                case 3:
                    booking.Milestone3Status = MilestoneStatus.Released;
                    booking.Milestone3ReleasedAt = DateTime.UtcNow;
                    break;
            }
        }

        private static SupplierProfileResponseDto MapToSupplierProfileDto(SupplierProfile p) => new()
        {
            UserId = p.UserId,
            CompanyName = p.CompanyName,
            BusinessPhone = p.BusinessPhone,
            BusinessEmail = p.BusinessEmail,
            PrimaryBaseCity = p.PrimaryBaseCity,
            YardAddress = p.YardAddress,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            CacCertificateUrl = p.CacCertificateUrl,
            Status = p.Status.ToString(),
            OnboardingStep = p.OnboardingStep
        };

        private static AssetResponseDto MapToAssetDto(Asset a) => new()
        {
            Id = a.Id,
            MachineType = a.MachineType.ToString(),
            Brand = a.Brand,
            Model = a.Model,
            YearOfManufacture = a.YearOfManufacture,
            EngineHours = a.EngineHours,
            HasCertifiedOperator = a.HasCertifiedOperator,
            Status = a.Status.ToString(),
            DailyRentalRate = a.DailyRentalRate,
            MobilizationFeePerKm = a.MobilizationFeePerKm,
            FrontPhotoUrl = a.FrontPhotoUrl,
            SidePhotoUrl = a.SidePhotoUrl,
            SerialPlatePhotoUrl = a.SerialPlatePhotoUrl,
            Description = a.Description,
            CreatedAt = a.CreatedAt
        };

        private static OperatorResponseDto MapToOperatorDto(Operator o) => new()
        {
            Id = o.Id,
            FullName = o.FullName,
            PhoneNumber = o.PhoneNumber,
            LicenseNumber = o.LicenseNumber,
            LicenseCategory = o.LicenseCategory,
            YearsOfExperience = o.YearsOfExperience,
            VettingStatus = o.VettingStatus.ToString(),
            PassedVetting = o.PassedVetting,
            LicenseDocumentUrl = o.LicenseDocumentUrl,
            Guarantors = o.Guarantors?.Select(g => new GuarantorDto
            {
                Id = g.Id,
                FullName = g.FullName,
                PhoneNumber = g.PhoneNumber,
                Occupation = g.Occupation,
                IdType = g.IdType
            }).ToList() ?? new()
        };

        private static BookingResponseDto MapToBookingDto(Booking b) => new()
        {
            Id = b.Id,
            AssetId = b.AssetId,
            AssetName = b.Asset != null ? $"{b.Asset.Brand} {b.Asset.Model}" : string.Empty,
            MinerName = b.Miner?.FullName ?? string.Empty,
            SupplierName = b.Supplier?.CompanyName ?? string.Empty,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalDays = b.TotalDays,
            RentalFee = b.RentalFee,
            MobilizationFee = b.MobilizationFee,
            TotalAmount = b.TotalAmount,
            PlatformFee = b.PlatformFee,
            SupplierPayout = b.SupplierPayout,
            Status = b.Status.ToString(),
            PaymentLink = b.PayscrowPaymentLink,
            PayscrowTransactionNumber = b.PayscrowTransactionNumber,
            SiteAddress = b.SiteAddress,
            Currency = b.Currency ?? "NGN",
            CreatedAt = b.CreatedAt
        };

        private static BookingDetailDto MapToBookingDetailDto(Booking b)
        {
            var dto = new BookingDetailDto
            {
                Id = b.Id,
                AssetId = b.AssetId,
                AssetName = b.Asset != null ? $"{b.Asset.Brand} {b.Asset.Model}" : string.Empty,
                MinerName = b.Miner?.FullName ?? string.Empty,
                SupplierName = b.Supplier?.CompanyName ?? string.Empty,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                TotalDays = b.TotalDays,
                RentalFee = b.RentalFee,
                MobilizationFee = b.MobilizationFee,
                TotalAmount = b.TotalAmount,
                PlatformFee = b.PlatformFee,
                SupplierPayout = b.SupplierPayout,
                Status = b.Status.ToString(),
                PaymentLink = b.PayscrowPaymentLink,
                PayscrowTransactionNumber = b.PayscrowTransactionNumber,
                SiteAddress = b.SiteAddress,
                Currency = b.Currency ?? "NGN",
                CreatedAt = b.CreatedAt,
                LogisticsStatus = b.LogisticsStatus.ToString(),
                GitInsuranceActive = b.GitInsuranceActive,
                InsurancePolicyNumber = b.InsurancePolicyNumber,
                Milestone1 = new MilestoneDto { Name = "Site Arrival (20%)",
                    Amount = b.Milestone1Amount,
                    Status = b.Milestone1Status.ToString(),
                    ReleasedAt = b.Milestone1ReleasedAt,
                    Description = "Released on site arrival" },
                Milestone2 = new MilestoneDto { Name = "Mid-Term (40%)",
                    Amount = b.Milestone2Amount,
                    Status = b.Milestone2Status.ToString(),
                    ReleasedAt = b.Milestone2ReleasedAt,
                    Description = "Released on Day 5" },
                Milestone3 = new MilestoneDto { Name = "Completion (40%)",
                    Amount = b.Milestone3Amount,
                    Status = b.Milestone3Status.ToString(),
                    ReleasedAt = b.Milestone3ReleasedAt,
                    Description = "Released on return clearance" },
                PaymentBreakdown = new PaymentBreakdownDto
                {
                    TotalEscrow = b.TotalAmount,
                    PlatformFee = b.PlatformFee,
                    SupplierShare = b.SupplierPayout,
                    Milestone1Amount = b.Milestone1Amount,
                    Milestone2Amount = b.Milestone2Amount,
                    Milestone3Amount = b.Milestone3Amount,
                    Currency = b.Currency ?? "NGN"
                }
            };
            return dto;
        }

        private static BookingDisputeResponseDto MapToDisputeDto(BookingDispute d) => new()
        {
            Id = d.Id,
            RaisedByRole = d.RaisedByRole,
            Description = d.Description,
            Status = d.Status.ToString(),
            Resolution = d.Resolution,
            CreatedAt = d.CreatedAt
        };
    }
}
