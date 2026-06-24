using ElctroWay.Data;
using ElctroWay.DTOs;
using ElctroWay.DTOs.AuthDto;
using ElctroWay.DTOs.Common;
using ElctroWay.Enums;
using ElctroWay.Models.Identity;
using ElctroWay.Models.Provider;
using ElctroWay.Repositories.Interfaces;
using ElctroWay.Service.Interfaces;
using ElctroWay.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElctroWay.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthRepository _repo;
        private readonly AppDbContext _context;
        private readonly IOcrService _ocrService;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IAuthRepository repo,
            AppDbContext context,
            IOcrService ocrService,
            IConfiguration config)
        {
            _userManager = userManager;
            _repo = repo;
            _context = context;
            _ocrService = ocrService;
            _config = config;
        }

        // ================= REGISTER PROVIDER =================
        public async Task<ApiResponse<object>> RegisterProviderAsync(ProviderRegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return ApiResponse<object>.FailResponse(
                    string.Join(",", result.Errors.Select(x => x.Description)));

            await _userManager.AddToRoleAsync(user, "Provider");

            var profile = new ProviderProfile
            {
                UserId = user.Id,
                VerificationStatus = VerificationStatus.Processing,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ProviderProfiles.AddAsync(profile);
            await _context.SaveChangesAsync(); 

            var front = await SaveFile(dto.FrontId);
            var back = await SaveFile(dto.BackId);
            var selfie = await SaveFile(dto.SelfieWithId);

            await _context.ProviderDocuments.AddRangeAsync(
                new ProviderDocument
                {
                    ProviderId = profile.ProviderId,
                    ImageUrl = front,
                    DocumentType = DocumentType.NationalIdFront
                },
                new ProviderDocument
                {
                    ProviderId = profile.ProviderId,
                    ImageUrl = back,
                    DocumentType = DocumentType.NationalIdBack
                },
                new ProviderDocument
                {
                    ProviderId = profile.ProviderId,
                    ImageUrl = selfie,
                    DocumentType = DocumentType.SelfieWithId
                });

            await _context.SaveChangesAsync();

            var ocr = _ocrService.VerifyAsync(front, back, selfie, dto.FullName);
            //==============================================================
            //score we change(rvrse condations)
            //if (ocr.Score >= 0.8)
            //    profile.VerificationStatus = VerificationStatus.Verified;
            //else if (ocr.Score >= 0.5)
            //    profile.VerificationStatus = VerificationStatus.PendingReview;
            //else
            //    profile.VerificationStatus = VerificationStatus.Rejected;
            if (ocr.Score >= 0.8)
                profile.VerificationStatus = VerificationStatus.Rejected;
            else if (ocr.Score >= 0.5)
                profile.VerificationStatus = VerificationStatus.PendingReview;
            else
                profile.VerificationStatus = VerificationStatus.Verified;
            //==================================================================

            profile.ReviewNotes = ocr.Reason;

            await _context.SaveChangesAsync();

            return ApiResponse<object>.SuccessResponse(new
            {
                user.Id,
                profile.VerificationStatus,
                ocr.Score
            });
        }

        // ================= LOGIN =================
        public async Task<ApiResponse<object>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return ApiResponse<object>.FailResponse("Invalid credentials");

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return ApiResponse<object>.FailResponse("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var profile = await _context.ProviderProfiles
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            var token = GenerateToken(user, roles.FirstOrDefault(), profile);

            return ApiResponse<object>.SuccessResponse(new
            {
                token,
                user.Id,
                user.Email,
                role = roles.FirstOrDefault(),
                verificationStatus = profile?.VerificationStatus,
                reviewNotes = profile?.ReviewNotes
            });
        }

        // ================= DRIVER =================
        public async Task<ApiResponse<object>> RegisterDriverAsync(DriverRegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return ApiResponse<object>.FailResponse(
                    string.Join(",", result.Errors.Select(x => x.Description)));

            await _userManager.AddToRoleAsync(user, "Driver");

            return ApiResponse<object>.SuccessResponse(new { user.Id });
        }

        // ================= TOKEN =================
        private string GenerateToken(
    ApplicationUser user,
    string? role,
    ProviderProfile? profile)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Role, role ?? ""),
        new Claim(
            "VerificationStatus",
            profile?.VerificationStatus.ToString() ?? "None")
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }


        private async Task<string> SaveFile(IFormFile file)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

           
            return path;
        }
    }
}
