using Afrimine.Model.Enums;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Afrimine.Services.DTOs.SupplierDto;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IServiceManager _service;

        public EquipmentController(IServiceManager service)
        {
            _service = service;
        }

        // ── Supplier Onboarding ────────────────────────────────────────────────

        /// <summary>Register as an equipment/mineral/manpower supplier</summary>
        /// <remarks>
        /// Creates a new Vendor account for equipment suppliers, mineral sellers, or manpower providers.
        /// After registration, complete the 4 onboarding steps before submitting for verification:
        ///
        /// 1. `PATCH /suppliers/profile` — update company name, phone, email
        /// 2. `PUT /suppliers/location` — set yard/office GPS location
        /// 3. `POST /suppliers/documents` — upload CAC certificate or proof of purchase
        /// 4. `POST /suppliers/submit` — submit for field agent verification
        ///
        /// **Password requirements:** Min 8 chars, uppercase, lowercase, digit, special character.
        /// </remarks>
    


        /// <summary>Get authenticated supplier's full profile (Vendor only)</summary>
        /// <remarks>
        /// Returns the current supplier's company info, location, verification status, and onboarding step.
        /// Use this to determine which onboarding step to show in the UI.
        ///
        /// **SupplierStatus values:**
        /// - `Pending` — submitted, awaiting field agent review
        /// - `Active` — verified, full platform access
        /// - `Rejected` — rejected, see `rejectionReason` and resubmit
        /// - `Suspended` — account suspended by admin
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("suppliers/me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetProfileAsync(userId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Update supplier company info — Onboarding Step 2 (Vendor only)</summary>
        /// <remarks>
        /// Partial update — only send fields you want to change.
        /// Updates company name, business phone, and business email.
        /// Advances onboarding progress to step 2.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPatch("suppliers/profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] SupplierProfileUpdateDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateProfileAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Save yard/office GPS location — Onboarding Step 3 (Vendor only)</summary>
        /// <remarks>
        /// Save the supplier's primary base city, physical yard address, and GPS coordinates.
        ///
        /// **How to get coordinates on the frontend:**
        /// Use the browser Geolocation API (`navigator.geolocation.getCurrentPosition`) or a map picker (Google Maps / Leaflet).
        /// Send the resulting `latitude` and `longitude` values here.
        ///
        /// **Why this matters:**
        /// These coordinates are used to auto-calculate the mobilization distance when a buyer creates a booking —
        /// the system uses the Haversine formula to compute distance from this yard to the buyer's mine site.
        ///
        /// **Primary Base City options:** Abuja, Jos, Lafia, Kaduna, Lokoja
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPut("suppliers/location")]
        public async Task<IActionResult> UpdateLocation([FromBody] SupplierLocationDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateLocationAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Upload CAC certificate or proof of purchase — Onboarding Step 4 (Vendor only)</summary>
        /// <remarks>
        /// Upload the company's CAC registration certificate or equipment proof of purchase document.
        /// Use `multipart/form-data` — field name: `document`
        ///
        /// **Accepted file types:** PDF, JPG, PNG, DOCX
        /// **Max file size:** 10MB
        ///
        /// Previously uploaded documents are automatically replaced.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("suppliers/documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument([FromForm] SupplierDocumentUploadDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UploadDocumentAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Submit profile for field agent verification — Onboarding Step 5 (Vendor only)</summary>
        /// <remarks>
        /// Finalizes onboarding and triggers the offline verification process.
        /// Status changes to `Pending`. Our field agents in the Jos/Nasarawa hub will contact you within 24 hours to physically verify your machines and yard.
        ///
        /// **Requirements before submitting:**
        /// - Company info completed (step 2)
        /// - Location saved (step 3)
        /// - Document uploaded (step 4)
        ///
        /// You cannot list assets or accept bookings until your account is `Active`.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("suppliers/submit")]
        public async Task<IActionResult> Submit()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.SubmitForVerificationAsync(userId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Get supplier verification status (Vendor only)</summary>
        /// <remarks>
        /// Poll this endpoint to check if the field agent has verified your account.
        ///
        /// **SupplierStatus values:**
        /// - `Pending` — submitted, awaiting field agent visit
        /// - `Active` — verified and approved, you can now list assets
        /// - `Rejected` — rejected, check `rejectionReason` for details and resubmit
        /// - `Suspended` — suspended by admin, contact support
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("suppliers/status")]
        public async Task<IActionResult> GetStatus()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetStatusAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Assets ─────────────────────────────────────────────────────────────

        /// <summary>List a new machine/asset in your yard (Vendor only)</summary>
        /// <remarks>
        /// Add a new heavy machine to your inventory. After creation, upload photos using `POST /assets/{id}/photos`.
        ///
        /// **MachineType values:**
        /// - `0` = Excavator
        /// - `1` = Bulldozer
        /// - `2` = Payloader
        /// - `3` = Tipper
        /// - `4` = Grader
        /// - `5` = Crane
        /// - `6` = Compactor
        ///
        /// **DailyRentalRate** — your rental price per day in the specified currency (e.g. NGN 150,000/day)
        /// **MobilizationFeePerKm** — transport cost per km charged to the buyer for delivering the machine to their site
        /// **HasCertifiedOperator** — set `true` if this machine comes with its own certified operator included in the rental
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("assets")]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.CreateAssetAsync(Guid.Parse(userId), request);
            return StatusCode(response.StatusCode, response);
        }



        /// <summary>Get all machines listed by the authenticated supplier (Vendor only)</summary>
        /// <remarks>
        /// Returns all the supplier's machines including their current status.
        ///
        /// **AssetStatus values:**
        /// - `Available` — ready to be booked
        /// - `Rented` — currently on an active lease
        /// - `UnderMaintenance` — being serviced, not available
        /// - `Inactive` — manually deactivated by supplier
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("assets")]
        public async Task<IActionResult> GetAssets()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetAssetsAsync(Guid.Parse(userId));
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Get a single machine with full details (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns full machine details including photos and operator assignments.
        /// Vendors can only access their own machines. Buyers can view any available machine.
        /// Returns 403 if a vendor tries to access another vendor's machine.
        /// </remarks>

        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("assets/{assetId:guid}")]
        public async Task<IActionResult> GetAsset(Guid assetId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty;

            var response = await _service.Equipment.GetAssetAsync(Guid.Parse(userId), role, assetId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Update machine details (Vendor only)</summary>
        /// <remarks>
        /// Partial update — only include the fields you want to change.
        /// Useful for updating engine hours after maintenance, adjusting pricing, or changing availability status.
        ///
        /// **Cannot update:** MachineType, Brand (create a new asset instead)
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPut("assets/{assetId:guid}")]
        public async Task<IActionResult> UpdateAsset(Guid assetId, [FromBody] UpdateAssetDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateAssetAsync(Guid.Parse(userId), assetId, request);
            return StatusCode(response.StatusCode, response);
        }



        /// <summary>Remove a machine from the platform — soft delete (Vendor only)</summary>
        /// <remarks>
        /// Soft-deletes the machine — it is archived and no longer visible to buyers.
        /// The machine record is preserved for historical booking records.
        /// Cannot delete a machine with an active booking.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpDelete("assets/{assetId:guid}")]
        public async Task<IActionResult> DeleteAsset(Guid assetId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.DeleteAssetAsync(Guid.Parse(userId), assetId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Upload machine photos (Vendor only)</summary>
        /// <remarks>
        /// Upload front view, side view, and serial number plate photos for a machine.
        /// Use `multipart/form-data` with these field names:
        /// - `frontPhoto` — front view of the machine
        /// - `sidePhoto` — side profile view
        /// - `serialPlatePhoto` — close-up of the serial number plate
        ///
        /// All fields are optional — only send the photos you want to update.
        /// **Accepted formats:** JPG, PNG, WEBP — **Max size:** 10MB each
        /// Previously uploaded photos are automatically replaced.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("assets/{assetId:guid}/photos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAssetPhotos(Guid assetId, [FromForm] AssetPhotoUploadDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UploadAssetPhotosAsync(Guid.Parse(userId), assetId, request);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Calculate rental cost preview — no auth required (public)</summary>
        /// <remarks>
        /// Use this on the booking form to show buyers the full cost breakdown before they confirm.
        /// Distance is calculated automatically if you provide site coordinates, or pass `distanceKm` directly.
        ///
        /// **Response breakdown:**
        /// - `rentalFee` = dailyRentalRate × totalDays
        /// - `mobilizationFee` = mobilizationFeePerKm × distanceKm
        /// - `totalAmount` = rentalFee + mobilizationFee
        /// - `platformFee` = 15% of totalAmount (Afrimine's cut)
        /// - `supplierPayout` = 85% of totalAmount (what supplier receives)
        /// - `payscrowCharge` = PayScrow escrow service fee (borne by buyer on top)
        /// - `totalPayable` = exact amount buyer pays via PayScrow
        ///
        /// **Currency values:** `NGN`, `USD`, `GBP`
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("assets/{assetId:guid}/pricing")]
        public async Task<IActionResult> GetPricing(
            Guid assetId, [FromQuery] int totalDays, [FromQuery] double distanceKm,
            [FromQuery] string currency = "NGN")
        {
            var response = await _service.Equipment.GetAssetPricingAsync(assetId, totalDays, distanceKm, currency);
            return StatusCode(response.StatusCode, response);
        }



        /// <summary>Assign a vetted operator to a machine (Vendor only)</summary>
        /// <remarks>
        /// Link an operator to a specific machine. The operator must have passed vetting first.
        /// Check vetting status at `GET /operators/{id}/vetting-status` before assigning.
        /// A machine can have multiple operators but only one primary operator per booking.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("assets/{assetId:guid}/operators")]
        public async Task<IActionResult> AssignOperator(Guid assetId, [FromQuery] Guid operatorId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.AssignOperatorToAssetAsync(Guid.Parse(userId), assetId, operatorId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Operators ──────────────────────────────────────────────────────────


        /// <summary>Add a certified machine operator (Vendor only)</summary>
        /// <remarks>
        /// Add an operator to your account. Operators must be vetted before they can be assigned to active bookings.
        /// Use `multipart/form-data` to optionally include their license document.
        ///
        /// **Vetting process after adding an operator:**
        /// 1. `POST /operators/{id}/guarantors` — add 2 guarantors (required)
        /// 2. `POST /operators/{id}/vetting` — submit terrain knowledge assessment
        /// 3. `GET /operators/{id}/vetting-status` — check if passed
        ///
        /// **Passing criteria:**
        /// - License Category E
        /// - Minimum 3 years experience
        /// - 2 guarantors added
        /// - Detailed terrain knowledge answer (20+ characters)
        /// </remarks>
        
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("operators")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateOperator([FromForm] CreateOperatorDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.CreateOperatorAsync(Guid.Parse(userId), request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get all operators linked to the authenticated supplier (Vendor only)</summary>
        /// <remarks>
        /// Returns all operators with their vetting status and guarantor details.
        ///
        /// **VettingStatus values:**
        /// - `NotStarted` — added but vetting not submitted yet
        /// - `Submitted` — assessment submitted, being reviewed
        /// - `Passed` — operator cleared, can be assigned to bookings
        /// - `Failed` — did not meet criteria, review and resubmit
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("operators")]
        public async Task<IActionResult> GetOperators()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetOperatorsAsync(Guid.Parse(userId));
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Update operator details (Vendor only)</summary>
        /// <remarks>
        /// Update name, phone, license number, or years of experience.
        /// Use `multipart/form-data` to also update the license document.
        /// Note: updating experience or answers after a failed vetting allows resubmission.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPut("operators/{operatorId:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateOperator(Guid operatorId, [FromForm] CreateOperatorDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateOperatorAsync(Guid.Parse(userId), operatorId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Add a guarantor for an operator — max 2 per operator (Vendor only)</summary>
        /// <remarks>
        /// Each operator requires exactly 2 guarantors before vetting can be submitted.
        /// Guarantors vouch for the operator's identity and integrity.
        ///
        /// **Required fields:** fullName, phoneNumber, occupation, idType, idNumber
        ///
        /// **idType examples:** NIN, BVN, Driver's License, International Passport, Voter's Card
        ///
        /// Returns 400 if a third guarantor is added (maximum is 2).
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("operators/{operatorId:guid}/guarantors")]
        public async Task<IActionResult> AddGuarantor(Guid operatorId, [FromBody] CreateGuarantorDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.AddGuarantorAsync(Guid.Parse(userId), operatorId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Submit terrain knowledge vetting assessment (Vendor only)</summary>
        /// <remarks>
        /// Submit the operator's answers to the terrain knowledge and daily maintenance assessment.
        /// The system scores automatically based on:
        /// - Years of experience (must be 3+)
        /// - Quality of terrain knowledge answer (must be 20+ characters, detailed)
        ///
        /// **Result is immediate** — check `passedVetting` in the response or `GET /operators/{id}/vetting-status`.
        ///
        /// If failed, update the operator's experience or answers and resubmit.
        /// </remarks>
        
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("operators/{operatorId:guid}/vetting")]
        public async Task<IActionResult> SubmitVetting(Guid operatorId, [FromBody] VettingSubmitDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.SubmitVettingAsync(Guid.Parse(userId), operatorId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Check if an operator passed vetting (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns the operator's current vetting status and whether they are cleared for active bookings.
        ///
        /// **VettingStatus values:**
        /// - `NotStarted` — vetting assessment not yet submitted
        /// - `Submitted` — under review
        /// - `Passed` — operator cleared, can be assigned to bookings
        /// - `Failed` — did not meet criteria, see `feedback` for reason
        /// </remarks>
        
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("operators/{operatorId:guid}/vetting-status")]
        public async Task<IActionResult> GetVettingStatus(Guid operatorId)
        {
            var response = await _service.Equipment.GetVettingStatusAsync(operatorId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Bookings ───────────────────────────────────────────────────────────

        /// <summary>Create a booking/rental request for a machine (Buyer only)</summary>
        /// <remarks>
        /// Initiates the equipment rental process. A PayScrow escrow transaction is created automatically
        /// and a `paymentLink` is returned for the buyer to complete payment.
        ///
        /// **Full booking flow:**
        /// 1. Buyer creates booking → `paymentLink` returned
        /// 2. Buyer pays via `paymentLink` on PayScrow
        /// 3. PayScrow webhook fires → booking status changes to `Active`, supplier notified
        /// 4. Supplier approves → `PUT /bookings/{id}/approve`
        /// 5. Supplier dispatches machine → `POST /bookings/{id}/dispatch`
        /// 6. Machine arrives on site → `POST /bookings/{id}/site-arrival` (Milestone 1: 20%)
        /// 7. Day 5 daily check → Milestone 2 auto-released (40%)
        /// 8. Lease complete → `POST /bookings/{id}/return-clearance` (Milestone 3: 40%)
        ///
        /// **LogisticsType values:**
        /// - `0` = SupplierOwned — supplier uses their own low-bed truck (default for equipment)
        /// - `1` = ThirdParty — platform arranges 3PL logistics partner (used for mineral deliveries)
        ///
        /// **Distance auto-calculation:** If you provide `siteLatitude` and `siteLongitude`, the system
        /// auto-calculates distance from the supplier's yard using the Haversine formula.
        /// Or pass `distanceKm` directly to override.
        ///
        /// **MinerPhone** is required by PayScrow to create the escrow transaction (Nigerian format: 08012345678).
        /// </remarks>
        
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.CreateBookingAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get all bookings for the authenticated user (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns bookings where you are either the buyer (miner) or the supplier.
        /// Use the `status` query param to filter by booking state.
        ///
        /// **Status filter values:**
        /// - `Pending` — booking created, awaiting supplier approval
        /// - `Approved` — supplier accepted, awaiting machine dispatch
        /// - `Declined` — supplier declined (see `declineReason`)
        /// - `Active` — machine dispatched, currently on lease
        /// - `Completed` — lease finished, all milestones released
        /// - `Disputed` — dispute raised, under admin review
        /// - `Cancelled` — cancelled before dispatch
        /// </remarks>
        
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings([FromQuery] string? status)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetBookingsAsync(userId, status);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get full booking details with milestones and payment breakdown (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns complete booking info including:
        /// - All 3 milestone statuses and amounts
        /// - Payment breakdown (15%/85% split)
        /// - Logistics and insurance status
        /// - Only accessible by the buyer or supplier in this booking
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}")]
        public async Task<IActionResult> GetBooking(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetBookingDetailAsync(userId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Approve a booking request (Vendor only)</summary>
        /// <remarks>
        /// Supplier accepts the miner's rental request.
        /// The machine status changes to `Rented` so it cannot be double-booked.
        /// The buyer should have already funded the PayScrow escrow before you approve.
        ///
        /// After approving, dispatch the machine using `POST /bookings/{id}/dispatch`.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPut("bookings/{bookingId:guid}/approve")]
        public async Task<IActionResult> ApproveBooking(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.ApproveBookingAsync(supplierId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Decline a booking request with mandatory reason (Vendor only)</summary>
        /// <remarks>
        /// Supplier rejects the miner's rental request. A reason is required.
        /// The buyer will see the reason and can search for another machine.
        /// The machine remains `Available` for other bookings.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPut("bookings/{bookingId:guid}/decline")]
        public async Task<IActionResult> DeclineBooking(Guid bookingId, [FromBody] DeclineBookingDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.DeclineBookingAsync(supplierId, bookingId, request);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Download digital contract / E-Waybill (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns the digital contract and Interstate Transit Clearance document for this booking.
        /// **Note:** Contract PDF generation is coming soon — currently returns a placeholder response.
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}/contract")]
        public IActionResult GetContract(Guid bookingId)
        {
            // TODO: Generate PDF contract / E-Waybill
            return Ok(new { message = "Contract generation coming soon.", bookingId });
        }

        // ── Logistics ──────────────────────────────────────────────────────────

        /// <summary>Confirm machine has left the yard — trigger dispatch (Vendor only)</summary>
        /// <remarks>
        /// Confirms the machine has departed the yard heading to the mining site.
        /// Booking status changes to `Active`. Logistics status changes to `Dispatched`.
        ///
        /// **Equipment bookings (SupplierOwned):**
        /// Supplier manages their own low-bed truck — no external call is made.
        /// You are responsible for delivering the machine to the site coordinates.
        ///
        /// **Mineral bookings (ThirdParty):**
        /// Platform automatically notifies the nearest vetted 3PL logistics partner for pickup and delivery.
        ///
        /// After dispatch, call `POST /bookings/{id}/insurance` to activate GIT insurance (recommended).
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("bookings/{bookingId:guid}/dispatch")]
        public async Task<IActionResult> Dispatch(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.DispatchBookingAsync(supplierId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get current logistics/haulage status (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns the current stage in the delivery pipeline.
        ///
        /// **LogisticsStatus values:**
        /// - `NotStarted` — booking approved but machine not yet dispatched
        /// - `Dispatched` — machine left the yard
        /// - `EnRoute` — in transit to the mining site
        /// - `Arrived` — confirmed arrival at site coordinates (triggers Milestone 1)
        /// - `Returned` — machine returned to yard after lease completion
        ///
        /// Also returns insurance status (`gitInsuranceActive`, `parInsuranceActive`) and certificate URL.
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}/logistics-status")]
        public async Task<IActionResult> GetLogisticsStatus(Guid bookingId)
        {
            var response = await _service.Equipment.GetLogisticsStatusAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Get real-time GPS tracking coordinates from ETU device (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns the latest GPS coordinates from the Electronic Tracking Unit (ETU) attached to the delivery vehicle.
        /// Coordinates are updated whenever the ETU device sends a ping to `POST /webhooks/tracking/etu`.
        ///
        /// Use `latitude` and `longitude` to display the vehicle location on a map.
        /// `lastUpdated` shows when the position was last received from the device.
        ///
        /// Returns `"No tracking data available"` if the ETU has not sent any data yet for this booking.
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}/tracking")]
        public async Task<IActionResult> GetTracking(Guid bookingId)
        {
            var response = await _service.Equipment.GetTrackingAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Activate insurance policy for a booking — optional (Vendor only)</summary>
        /// <remarks>
        /// Manually trigger an insurance policy for the machine during the lease.
        ///
        /// **Type values:**
        /// - `GIT` — Goods in Transit insurance — covers the machine during road transport to/from site. Recommended on dispatch.
        /// - `PAR` — Plant All Risk insurance — covers the machine while operating on-site. Optional add-on, buyer-initiated.
        ///
        /// **Note:** Insurance integration with Leadway/AXA is coming soon.
        /// Currently generates a placeholder policy number for tracking purposes.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("bookings/{bookingId:guid}/insurance")]
        public async Task<IActionResult> TriggerInsurance(Guid bookingId, [FromBody] TriggerInsuranceDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.TriggerInsuranceAsync(supplierId, bookingId, request.Type);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Download the active GIT or PAR insurance certificate (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns the insurance certificate URL for the active policy on this booking.
        /// Use `insuranceCertificateUrl` from the response to download the document.
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}/insurance-certificate")]
        public async Task<IActionResult> GetInsuranceCertificate(Guid bookingId)
        {
            var response = await _service.Equipment.GetLogisticsStatusAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Milestones & Sign-off ──────────────────────────────────────────────

        /// <summary>Driver/Site sign-off — confirm machine arrived at mining coordinates (Vendor and Buyer)</summary>
        /// <remarks>
        /// Both the supplier driver and the buyer can confirm site arrival.
        /// This is the physical sign-off that the machine has reached the mining site.
        ///
        /// **Triggers Milestone 1 — releases 20% of supplier payout:**
        /// - 20% of the supplier's 85% share is moved from `pendingBalance` to `availableBalance`
        /// - Logistics status changes to `Arrived`
        ///
        /// **Example:** For a ₦1,000,000 total booking → supplier payout = ₦850,000 → Milestone 1 = ₦170,000 released
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpPost("bookings/{bookingId:guid}/site-arrival")]
        public async Task<IActionResult> SiteArrival(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.SiteArrivalSignOffAsync(supplierId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Submit operator's daily pre-start safety checklist (Vendor only)</summary>
        /// <remarks>
        /// Operator logs the daily machine safety checks before starting work.
        /// Should be submitted every day of the active lease.
        ///
        /// **Checklist items:** engine oil, hydraulic fluid, cooling system, undercarriage, grease
        ///
        /// **Triggers Milestone 2 automatically on Day 5:**
        /// If it is Day 5 or later of the lease AND there are no open disputes, Milestone 2 (40%) is auto-released.
        /// If a dispute is active, Milestone 2 is paused until the dispute is resolved.
        ///
        /// **Example:** For a ₦850,000 supplier payout → Milestone 2 = ₦340,000 released on Day 5
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("bookings/{bookingId:guid}/daily-check")]
        public async Task<IActionResult> DailyCheck(Guid bookingId, [FromBody] DailyCheckDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.SubmitDailyCheckAsync(supplierId, bookingId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get status of all 3 payout milestones (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns the three milestone payment tranches with their current status and amounts.
        ///
        /// **Milestone structure (based on supplier's 85% share):**
        ///
        /// | Milestone | Percentage | Trigger |
        /// |-----------|-----------|---------|
        /// | Milestone 1 | 20% | Machine arrives at mining site |
        /// | Milestone 2 | 40% | Day 5 of lease (auto, if no disputes) |
        /// | Milestone 3 | 40% | Return clearance sign-off at lease end |
        ///
        /// **MilestoneStatus values:**
        /// - `Locked` — trigger condition not yet met
        /// - `Pending` — trigger condition met, processing payment
        /// - `Released` — funds moved to supplier's available wallet balance
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}/milestones")]
        public async Task<IActionResult> GetMilestones(Guid bookingId)
        {
            var response = await _service.Equipment.GetMilestonesAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Completion sign-off — confirm machine loaded for return transit (Vendor and Buyer)</summary>
        /// <remarks>
        /// Both the supplier and buyer can confirm the lease is complete and the machine is heading back.
        ///
        /// **Triggers Milestone 3 — releases final 40% of supplier payout:**
        /// - Remaining 40% moves from `pendingBalance` to `availableBalance`
        /// - Booking status changes to `Completed`
        /// - Machine status changes back to `Available` for new bookings
        ///
        /// **Example:** For a ₦850,000 supplier payout → Milestone 3 = ₦340,000 released
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpPost("bookings/{bookingId:guid}/return-clearance")]
        public async Task<IActionResult> ReturnClearance(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.ReturnClearanceAsync(userId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get the full 15%/85% payment breakdown for a booking (Vendor and Buyer)</summary>
        /// <remarks>
        /// Shows the exact financial breakdown of a booking's total value.
        ///
        /// **Example for ₦1,000,000 total booking:**
        /// - Total escrow: ₦1,000,000
        /// - Platform fee (15%): ₦150,000
        /// - Supplier share (85%): ₦850,000
        ///   - Milestone 1 (20%): ₦170,000
        ///   - Milestone 2 (40%): ₦340,000
        ///   - Milestone 3 (40%): ₦340,000
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}/payment-breakdown")]
        public async Task<IActionResult> GetPaymentBreakdown(Guid bookingId)
        {
            var response = await _service.Equipment.GetPaymentBreakdownAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Disputes ───────────────────────────────────────────────────────────

        /// <summary>Raise a dispute on a booking (Vendor and Buyer)</summary>
        /// <remarks>
        /// Either the supplier or buyer can raise a dispute (e.g. equipment breakdown, non-delivery, damage).
        ///
        /// **Effect of raising a dispute:**
        /// - Milestone 2 auto-release is paused until the dispute is resolved
        /// - Dispute is also raised on PayScrow escrow — funds are frozen
        /// - Admin reviews the dispute and resolves within 24–48 hours
        ///
        /// **RaisedByRole values:** `"miner"` | `"supplier"`
        ///
        /// Provide a clear description of the issue — admin will use this to make their decision.
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpPost("bookings/{bookingId:guid}/disputes")]
        public async Task<IActionResult> RaiseDispute(Guid bookingId, [FromBody] BookingDisputeDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.RaiseDisputeAsync(userId, bookingId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get all disputes for a specific booking (Vendor and Buyer)</summary>
        /// <remarks>
        /// Returns the dispute history for this booking ordered by most recent.
        ///
        /// **DisputeStatus values:**
        /// - `Open` — newly raised, awaiting admin review
        /// - `UnderReview` — admin is investigating
        /// - `ResolvedBuyer` — resolved in favour of the miner (escrow refunded)
        /// - `ResolvedVendor` — resolved in favour of the supplier (escrow released)
        /// - `Closed` — dispute closed without escalation
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("bookings/{bookingId:guid}/disputes")]
        public async Task<IActionResult> GetBookingDisputes(Guid bookingId)
        {
            var response = await _service.Equipment.GetBookingDisputesAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Get all disputes involving the authenticated supplier (Vendor only)</summary>
        /// <remarks>
        /// Returns all disputes across all the supplier's bookings, ordered by most recent.
        /// Use this to manage and monitor all ongoing disputes from the supplier dashboard.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("disputes")]
        public async Task<IActionResult> GetAllDisputes()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.GetAllSupplierDisputesAsync(supplierId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Wallet ─────────────────────────────────────────────────────────────

        /// <summary>Get supplier wallet balance (Vendor only)</summary>
        /// <remarks>
        /// Returns the supplier's current wallet balances.
        ///
        /// - `availableBalance` — funds already released from milestones, ready to withdraw to bank
        /// - `pendingBalance` — funds locked in PayScrow escrow, waiting for milestones to trigger
        ///
        /// Only `availableBalance` can be withdrawn. `pendingBalance` becomes available as milestones are released.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("wallet/balance")]
        public async Task<IActionResult> GetWalletBalance()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.GetWalletBalanceAsync(supplierId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Request a bank withdrawal from available balance (Vendor only)</summary>
        /// <remarks>
        /// Submits a withdrawal request to transfer funds from `availableBalance` to the supplier's registered bank account.
        ///
        /// **Requirements:**
        /// - Bank account details must be set up in your supplier profile (`BankName`, `BankCode`, `BankAccountNumber`, `BankAccountName`)
        /// - Amount cannot exceed `availableBalance`
        ///
        /// Withdrawal is reviewed by admin and processed within 24–48 hours.
        /// Check status at `GET /admin/withdrawals` (admin) or track via wallet transaction history.
        ///
        /// Use `GET /banks` to get valid bank codes for your bank.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("wallet/withdrawal")]
        public async Task<IActionResult> RequestWithdrawal([FromBody] WithdrawalRequestDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.RequestWithdrawalAsync(supplierId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get wallet transaction history (Vendor only)</summary>
        /// <remarks>
        /// Returns the full ledger of all wallet movements ordered by most recent.
        ///
        /// **Type values:**
        /// - `credit` — funds added (milestone release, escrow lock)
        /// - `debit` — funds removed (withdrawal request)
        ///
        /// Use this to display the transaction history table in the supplier dashboard.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("wallet/transactions")]
        public async Task<IActionResult> GetWalletTransactions()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.GetWalletTransactionsAsync(supplierId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Dashboard ──────────────────────────────────────────────────────────

        /// <summary>Get supplier dashboard KPI stats (Vendor only)</summary>
        /// <remarks>
        /// Returns aggregated data for the 4 KPI cards on the supplier dashboard.
        ///
        /// - `totalMachines` — total machines listed (all statuses)
        /// - `activeLeases` — bookings currently in `Active` status
        /// - `currentMonthEarnings` — current available wallet balance (released milestones)
        /// - `pendingEscrow` — funds locked in PayScrow escrow (pending milestones)
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("dashboard/supplier/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var supplierId = Guid.Parse(userId);
            var response = await _service.Equipment.GetDashboardStatsAsync(supplierId);
            return StatusCode(response.StatusCode, response);
        }

        // ── PayScrow Escrow ───────────────────────────────────────────────────────────

        /// <summary>Apply escrow release code to release funds to supplier (Vendor and Buyer)</summary>
        /// <remarks>
        /// The buyer receives a unique escrow release code by email when payment is confirmed by PayScrow.
        /// When the buyer is satisfied with the delivery, they share this code to release funds.
        ///
        /// **Important:** Use `transactionId` (GUID format from the booking) — NOT `transactionNumber`.
        /// The `transactionId` is the internal PayScrow GUID, not the human-readable `MKT-XXXXX` number.
        ///
        /// On success, the PayScrow transaction status changes to `Completed` and funds are released to the vendor.
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpPost("escrow/apply-code")]
        public async Task<IActionResult> ApplyEscrowCode([FromBody] EscrowCodeApplyDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.ApplyEscrowCodeAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get PayScrow escrow transaction status (Vendor and Buyer)</summary>
        /// <remarks>
        /// Fetches the live status directly from PayScrow for a transaction.
        /// Use the `payscrowTransactionNumber` (e.g. `MKT-00012345`) from the booking response.
        ///
        /// **PayScrow status values:**
        /// - `Pending` — transaction created, buyer has not yet paid
        /// - `In Progress` — buyer paid, funds held in escrow
        /// - `Completed` — escrow code applied, funds being released
        /// - `Finalized` — fully settled, funds disbursed
        /// - `Terminated` — transaction cancelled or refunded
        /// </remarks>
        [Authorize(Roles = Roles.VendorAndBuyer)]
        [HttpGet("escrow/status/{transactionNumber}")]
        public async Task<IActionResult> GetPayscrowStatus(string transactionNumber)
        {
            var response = await _service.Equipment.GetPayscrowTransactionStatusAsync(transactionNumber);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Calculate PayScrow escrow charges before creating a booking — no auth required (public)</summary>
        /// <remarks>
        /// Preview the exact PayScrow service fee before showing the buyer the final amount.
        /// Call this on the booking confirmation screen to display the complete cost breakdown.
        ///
        /// **merchantChargePercentage:**
        /// - `0` = buyer pays all PayScrow fees (default — buyer bears the full escrow charge on top of rental cost)
        /// - `100` = supplier pays all fees (deducted from supplier's settlement amount)
        /// - `50` = fees split equally between buyer and supplier
        ///
        /// **Returned values to show the buyer:**
        /// - `grandTotalPayable` — exact total the buyer will be charged by PayScrow
        /// - `customerCharge` — buyer's share of the escrow fee
        /// - `totalSettlementAmount` — net amount supplier will receive after fees
        ///
        /// **Supported currencies:** `NGN`, `USD`, `GBP`
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("escrow/charges")]
        public async Task<IActionResult> CalculateCharges(
            [FromQuery] string currencyCode, [FromQuery] decimal amount,
            [FromQuery] decimal merchantChargePercentage = 0)
        {
            var response = await _service.Equipment.CalculatePayscrowChargesAsync(
                currencyCode, amount, merchantChargePercentage);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get list of Nigerian banks supported for PayScrow settlement — no auth required (public)</summary>
        /// <remarks>
        /// Returns all Nigerian banks with their CBN bank codes.
        /// Use the `code` field when setting up your bank account details for withdrawals.
        ///
        /// **Example banks:**
        /// - Access Bank: `044`
        /// - GTBank: `058`
        /// - UBA: `033`
        /// - Zenith Bank: `057`
        ///
        /// Display this as a searchable dropdown when collecting supplier bank account information.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("banks")]
        public async Task<IActionResult> GetSupportedBanks()
        {
            var response = await _service.Equipment.GetSupportedBanksAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Browse all available machines on the marketplace — no auth required (public)</summary>
        /// <remarks>
        /// Public listing of all available equipment for rent. Buyers can browse and filter before booking.
        ///
        /// **Query params:**
        /// - `q` — search text (matches brand, model, description)
        /// - `machineType` — filter by type: 0=Excavator, 1=Bulldozer, 2=Payloader, 3=Tipper, 4=Grader, 5=Crane, 6=Compactor
        /// - `location` — filter by supplier base city (e.g. "Jos", "Abuja")
        /// - `maxDailyRate` — maximum daily rental rate in NGN
        /// - `availableOnly` — `true` to show only machines ready to book (default: true)
        /// - `page`, `pageSize` — pagination
        ///
        /// Use `GET /assets/{id}/pricing` to get the full cost breakdown for a specific machine before booking.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("marketplace/assets")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AssetResponseDto>>), 200)]
        public async Task<IActionResult> SearchAssets([FromQuery] string? q, [FromQuery] MachineType? machineType, [FromQuery] string? location, [FromQuery] decimal? maxDailyRate, [FromQuery] bool availableOnly = true, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _service.Equipment.SearchAssetsAsync(q, machineType, location, maxDailyRate, availableOnly, page, pageSize);
            return StatusCode(response.StatusCode, response);
        }
    }
}