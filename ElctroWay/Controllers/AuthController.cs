////using ElctroWay.DTOs.AuthDto;
////using ElctroWay.Enums;
////using ElctroWay.Data;
////using ElctroWay.Models.Identity;
////using ElctroWay.Models.Provider;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using System.Net;
////using System.Net.Mail;
////using Microsoft.IdentityModel.Tokens;
////using System.IdentityModel.Tokens.Jwt;
////using System.Security.Claims;
////using System.Text;
////using Microsoft.AspNetCore.Authorization;

////namespace ElctroWay.Controllers
////{
////    [ApiController]
////    [Route("api/auth")]
////    public class AuthController : ControllerBase
////    {
////        private readonly UserManager<ApplicationUser> _userManager;
////        private readonly AppDbContext _context;
////        private readonly IConfiguration _config;

////        public AuthController(
////            UserManager<ApplicationUser> userManager,
////            AppDbContext context,
////            IConfiguration config)
////        {
////            _userManager = userManager;
////            _context = context;
////            _config = config;
////        }

////        // =========================
////        // Provider Registration
////        // =========================
////        [HttpPost("register-provider")]
////        public async Task<IActionResult> RegisterProvider([FromForm] ProviderRegisterDto dto)
////        {
////            // 1) Create User
////            var user = new ApplicationUser
////            {
////                UserName = dto.Email,
////                Email = dto.Email,
////                FullName = dto.FullName,
////                PhoneNumber = dto.PhoneNumber,
////                CreatedAt = DateTime.UtcNow,
////                IsActive = false
////            };

////            var result = await _userManager.CreateAsync(user, dto.Password);

////            if (!result.Succeeded)
////            {
////                return BadRequest(new
////                {
////                    message = "Registration failed",
////                    errors = result.Errors.Select(e => e.Description)
////                });
////            }

////            await _userManager.AddToRoleAsync(user, "Provider");

////            // 2) Create ProviderProfile
////            var providerProfile = new ProviderProfile
////            {
////                UserId = user.Id,
////                VerificationStatus = VerificationStatus.DocumentsUploaded,
////                CreatedAt = DateTime.UtcNow
////            };

////            _context.ProviderProfiles.Add(providerProfile);
////            await _context.SaveChangesAsync();

////            // 3) Generate OTP
////            var otp = GenerateOtp();

////            var otpEntity = new OtpCode
////            {
////                Email = dto.Email,
////                Code = otp,
////                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
////                IsUsed = false
////            };

////            _context.OtpCodes.Add(otpEntity);
////            await _context.SaveChangesAsync();

////            // 4) Send OTP Email
////            SendOtpEmail(dto.Email, otp);

////            // 5) Save Images
////            var uploadsFolder = Path.Combine("wwwroot", "providers", user.Id.ToString());
////            Directory.CreateDirectory(uploadsFolder);

////            var documents = new List<ProviderDocument>();

////            void SaveFile(IFormFile file, string type, DocumentType docType)
////            {
////                var fileName = $"{Guid.NewGuid()}_{type}{Path.GetExtension(file.FileName)}";
////                var path = Path.Combine(uploadsFolder, fileName);

////                using var stream = new FileStream(path, FileMode.Create);
////                file.CopyTo(stream);

////                documents.Add(new ProviderDocument
////                {
////                    ProviderId = providerProfile.ProviderId,
////                    ImageUrl = $"/providers/{user.Id}/{fileName}",
////                    DocumentType = docType,
////                    Status = DocumentStatus.Pending,
////                    UploadedAt = DateTime.UtcNow
////                });
////            }

////            SaveFile(dto.FrontId, "front", DocumentType.NationalIdFront);
////            SaveFile(dto.BackId, "back", DocumentType.NationalIdBack);
////            SaveFile(dto.SelfieWithId, "selfie", DocumentType.SelfieWithId);

////            _context.ProviderDocuments.AddRange(documents);
////            await _context.SaveChangesAsync();

////            // 6) Response to Frontend
////            return Ok(new
////            {
////                message = "OTP sent to your email. Please verify your account.",
////                userId = user.Id
////            });
////        }

////        // =========================
////        // Verify OTP
////        // =========================
////        [HttpPost("verify-otp")]
////        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
////        {
////            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

////            if (user == null)
////                return BadRequest("User not found");

////            var otpRecord = await _context.OtpCodes
////                .FirstOrDefaultAsync(x => x.Email == user.Email && x.Code == dto.Code);

////            if (otpRecord == null)
////                return BadRequest("Invalid OTP");

////            if (otpRecord.IsUsed)
////                return BadRequest("OTP already used");

////            if (otpRecord.ExpiryTime < DateTime.UtcNow)
////                return BadRequest("OTP expired");

////            otpRecord.IsUsed = true;

////            user.IsActive = true;

////            await _userManager.UpdateAsync(user);
////            await _context.SaveChangesAsync();

////            return Ok(new
////            {
////                message = "Account verified successfully"
////            });
////        }

////        // =========================
////        // OTP Generator
////        // =========================
////        private string GenerateOtp()
////        {
////            var random = new Random();
////            return random.Next(100000, 999999).ToString();
////        }

////        // =========================
////        // Sending Email
////        // =========================
////        private void SendOtpEmail(string email, string otp)
////        {
////            var client = new SmtpClient("smtp.gmail.com")
////            {
////                Port = 587,
////                Credentials = new NetworkCredential(
////                    "alfshorouk@gmail.com",
////                    "rzjm elwt sllo emqm"
////                ),
////                EnableSsl = true,
////            };

////            client.Send(
////                "alfshorouk@gmail.com",
////                email,
////                "OTP Code",
////                $"Your OTP is: {otp}"
////            );
////        }





////        //=============
////        //Login
////        //==========

////        [HttpPost("login")]
////        public async Task<IActionResult> Login(LoginDto dto)
////        {
////            var user = await _userManager.FindByEmailAsync(dto.Email);

////            if (user == null)
////                return BadRequest("Invalid credentials");

////            var checkPassword = await _userManager.CheckPasswordAsync(user, dto.Password);

////            if (!checkPassword)
////                return BadRequest("Invalid credentials");

////            if (!user.IsActive)
////                return BadRequest("Account not verified");

////            var roles = await _userManager.GetRolesAsync(user);

////            var token = GenerateJwtToken(user, roles.FirstOrDefault());

////            return Ok(new
////            {
////                token,
////                userId = user.Id,
////                role = roles.FirstOrDefault(),
////                email = user.Email,
////                fullName = user.FullName
////            });
////        }
////        private string GenerateJwtToken(ApplicationUser user, string role)
////        {
////            var claims = new List<Claim>
////    {
////        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
////        new Claim(ClaimTypes.Email, user.Email),
////        new Claim(ClaimTypes.Name, user.FullName ?? ""),
////        new Claim(ClaimTypes.Role, role ?? "")
////    };

////            var key = new SymmetricSecurityKey(
////                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
////            );

////            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

////            var token = new JwtSecurityToken(
////                issuer: _config["Jwt:Issuer"],
////                audience: _config["Jwt:Audience"],
////                claims: claims,
////                expires: DateTime.Now.AddDays(7),
////                signingCredentials: creds
////            );

////            return new JwtSecurityTokenHandler().WriteToken(token);
////        }


////        //==================Provider Test EndPoint===============
////        //[Authorize(Roles = "Provider")]
////        //[HttpGet("dashboard")]
////        //public IActionResult GetProviderDashboard()
////        //{
////        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
////        //    var email = User.FindFirst(ClaimTypes.Email)?.Value;
////        //    var name = User.FindFirst(ClaimTypes.Name)?.Value;

////        //    return Ok(new
////        //    {
////        //        message = "Welcome Provider ",
////        //        userId,
////        //        email,
////        //        name,
////        //        role = "Provider"
////        //    });
////        //}

////}

////}
//using ElctroWay.DTOs.AuthDto;
//using ElctroWay.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace ElctroWay.Controllers
//{
//    [ApiController]
//    [Route("api/auth")]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthService _authService;

//        public AuthController(
//            IAuthService authService)
//        {
//            _authService = authService;
//        }

//        [HttpPost("register-provider")]
//        public async Task<IActionResult> RegisterProvider(
//            [FromForm] ProviderRegisterDto dto)
//        {
//            var result =
//                await _authService
//                .RegisterProviderAsync(dto);

//            return Ok(result);
//        }

//        [HttpPost("verify-otp")]
//        public async Task<IActionResult> VerifyOtp(
//            VerifyOtpDto dto)
//        {
//            var result =
//                await _authService
//                .VerifyOtpAsync(dto);

//            return Ok(result);
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login(
//            LoginDto dto)
//        {
//            var result =
//                await _authService
//                .LoginAsync(dto);

//            return Ok(result);
//        }

//        [HttpPost("resend-otp/{userId}")]
//        public async Task<IActionResult> ResendOtp(string userId)
//        {
//            var result = await _authService.ResendOtpAsync(userId);
//            return Ok(result);
//        }
//        [HttpPost("register-driver")]
//        public async Task<IActionResult> RegisterDriver(
//    DriverRegisterDto dto)
//        {
//            var result =
//                await _authService.RegisterDriverAsync(dto);

//            return Ok(result);
//        }
//        [HttpPost("Provider")]
//        [AllowAnonymous]
//        public IActionResult Provider()
//        {
//            return Ok("Provider endpoint");
//        }
//        [HttpPost("Driver")]
//        [AllowAnonymous]
//        public IActionResult Driver()
//        {
//            return Ok("Driver endpoint");
//        }
//        [HttpGet("Admin")]
//        [Authorize(Roles = "Admin")]
//        public IActionResult Admin()
//        {
//            return Ok("Admin endpoint");
//        }
//    }
//}
using ElctroWay.DTOs;
using ElctroWay.DTOs.AuthDto;
using ElctroWay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElctroWay.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register-provider")]
        public async Task<IActionResult> RegisterProvider([FromForm] ProviderRegisterDto dto)
        {
            var result = await _auth.RegisterProviderAsync(dto);
            return Ok(result);
        }

        [HttpPost("register-driver")]
        public async Task<IActionResult> RegisterDriver([FromBody] DriverRegisterDto dto)
        {
            var result = await _auth.RegisterDriverAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _auth.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok("Logged out successfully");
        }

        [HttpGet("provider-test")]
        [Authorize(Roles = "Provider")]
        public IActionResult ProviderTest()
        {
            return Ok(new
            {
                Message = "Welcome Provider"
            });
        }

        [HttpGet("driver-test")]
        [Authorize(Roles = "Driver")]
        public IActionResult DriverTest()
        {
            return Ok(new
            {
                Message = "Welcome Driver"
            });
        }

        [HttpGet("admin-test")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminTest()
        {
            return Ok(new
            {
                Message = "Welcome Admin"
            });
        }
        
       
    }
}