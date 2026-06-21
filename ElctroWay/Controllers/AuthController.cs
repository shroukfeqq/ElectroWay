//using ElctroWay.DTOs.AuthDto;
//using ElctroWay.Enums;
//using ElctroWay.Models.Identity;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ElctroWay.Controllers
//{
//    [ApiController]
//    [Route("api/auth")]
//    public class AuthController : ControllerBase
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly IConfiguration _config;

//        public AuthController(
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            IConfiguration config)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _config = config;
//        }

//        //=============Registration========================================
//        //        [HttpPost("register")]
//        //        public async Task<IActionResult> Register(RegisterDto dto)
//        //        {
//        //            if (dto.Role != "Driver" && dto.Role != "Provider")
//        //            {
//        //                return BadRequest("Invalid Role");
//        //            }

//        //var user = new ApplicationUser
//        //{
//        //    UserName = dto.Email,
//        //    Email = dto.Email,
//        //    FullName = dto.FullName,
//        //    CreatedAt = DateTime.UtcNow,
//        //    IsActive = true
//        //};

//        //            var result = await _userManager.CreateAsync(user, dto.Password);

//        //            if (!result.Succeeded)
//        //                return BadRequest(result.Errors);

//        //            await _userManager.AddToRoleAsync(user, dto.Role);

//        //            return Ok("User registered successfully");


//        //}
//        [HttpPost("register")]
//        public async Task<IActionResult> Register(RegisterDto dto)
//        {
//            var user = new ApplicationUser
//            {
//                UserName = dto.Email,
//                Email = dto.Email,
//                FullName = dto.FullName,
//                CreatedAt = DateTime.UtcNow,
//                IsActive = true
//            };

//            var result = await _userManager.CreateAsync(user, dto.Password);

//            if (!result.Succeeded)
//                return BadRequest(result.Errors);

//            // assign role
//            await _userManager.AddToRoleAsync(user, dto.Role);

//            // لو Provider → اعملي Profile Pending
//            if (dto.Role == "Provider")
//            {
//                var profile = new ProviderProfile
//                {
//                    UserId = user.Id,
//                    VerificationStatus = VerificationStatus.Pending,
//                    CreatedAt = DateTime.UtcNow
//                };

//                // مهم جدًا: لازم يكون عندك DbContext injected
//                _context.ProviderProfiles.Add(profile);
//                await _context.SaveChangesAsync();
//            }

//            return Ok(new
//            {
//                message = "Registered successfully",
//                role = dto.Role
//            });
//        }
//        //=============================login=============================
//        [HttpPost("login")]
//        public async Task<IActionResult> Login(LoginDto dto)
//        {
//            var user = await _userManager.FindByEmailAsync(dto.Email);


//if (user == null)
//                return Unauthorized("Invalid credentials");

//            var result = await _signInManager
//                .CheckPasswordSignInAsync(user, dto.Password, false);

//            if (!result.Succeeded)
//                return Unauthorized("Invalid credentials");

//            var token = await GenerateJwtToken(user);

//            var roles = await _userManager.GetRolesAsync(user);

//            return Ok(new
//            {
//                Token = token,
//                Role = roles.FirstOrDefault(),
//                UserId = user.Id,
//                FullName = user.FullName
//            });


//}

//        //=============================JwtGeneration==============================
//        private async Task<string> GenerateJwtToken(ApplicationUser user)
//        {
//            var roles = await _userManager.GetRolesAsync(user);


//var claims = new List<Claim>
//{
//    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//    new Claim(ClaimTypes.Email, user.Email ?? ""),
//    new Claim(ClaimTypes.Name, user.FullName ?? "")
//};

//            foreach (var role in roles)
//            {
//                claims.Add(new Claim(ClaimTypes.Role, role));
//            }

//            var key = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
//            );

//            var creds = new SigningCredentials(
//                key,
//                SecurityAlgorithms.HmacSha256
//            );

//            var token = new JwtSecurityToken(
//                issuer: _config["Jwt:Issuer"],
//                audience: _config["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddDays(7),
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);


//}


//        [Authorize(Roles = "Provider")]
//        [HttpGet("test")]
//        public IActionResult Test()
//        {
//            return Ok("You are authenticated");
//        }
//    }
//}
using ElctroWay.DTOs.AuthDto;
using ElctroWay.Enums;
using ElctroWay.Data;
using ElctroWay.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElctroWay.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config,
        AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _context = context;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Role != "Driver" && dto.Role != "Provider")
                return BadRequest("Invalid Role");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.Role);

            // Provider → Pending Profile
            if (dto.Role == "Provider")
            {
                var profile = new ProviderProfile
                {
                    UserId = user.Id,
                    VerificationStatus = VerificationStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ProviderProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Registered successfully",
                role = dto.Role
            });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            var token = await GenerateJwtToken(user);

            var roles = await _userManager.GetRolesAsync(user);

            bool isProfileCompleted = true;

            if (roles.Contains("Provider"))
            {
                var provider = await _context.ProviderProfiles
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                isProfileCompleted = provider != null &&
                                      provider.VerificationStatus != VerificationStatus.Pending;
            }

            return Ok(new
            {
                Token = token,
                Role = roles.FirstOrDefault(),
                UserId = user.Id,
                FullName = user.FullName,
                IsProfileCompleted = isProfileCompleted
            });
        }

        // ================= JWT =================
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.FullName ?? "")
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ================= TEST =================
        [Authorize(Roles = "Provider")]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("You are authenticated as Provider");
        }
    }


}
