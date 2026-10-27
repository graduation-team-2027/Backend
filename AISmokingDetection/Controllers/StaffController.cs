
using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Staff> _passwordHasher = new();

        public StaffController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Email and password are required."
                });
            }

            var email = request.Email.Trim();

            var staff = await _context.Staff
                .FirstOrDefaultAsync(s => s.Email == email);

            if (staff == null || !staff.IsActive)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                staff,
                staff.PasswordHash,
                request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var keyValue = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(keyValue) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, new
                {
                    message = "Authentication configuration is missing."
                });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, staff.StaffId.ToString()),
                new Claim(ClaimTypes.Name, staff.FullName),
                new Claim(ClaimTypes.Email, staff.Email),
                new Claim(ClaimTypes.Role, staff.Role)
            };

            var key = new SymmetricSecurityKey(
                Convert.FromBase64String(keyValue));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                tokenType = "Bearer",
                expiresInMinutes = 60,
                staff = new
                {
                    staff.StaffId,
                    staff.FullName,
                    staff.Email,
                    staff.Role
                }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentStaff()
        {
            var staffIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(staffIdValue, out var staffId))
            {
                return Unauthorized();
            }

            var staff = await _context.Staff
                .Where(s => s.StaffId == staffId && s.IsActive)
                .Select(s => new
                {
                    s.StaffId,
                    s.FullName,
                    s.Email,
                    s.Role,
                    s.IsActive
                })
                .FirstOrDefaultAsync();

            if (staff == null)
            {
                return Unauthorized();
            }

            return Ok(staff);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = await _context.Staff
                .Select(s => new
                {
                    s.StaffId,
                    s.FullName,
                    s.Email,
                    s.Role,
                    s.IsActive
                })
                .ToListAsync();

            return Ok(staff);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateStaff(CreateStaffRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Full name, email, and password are required."
                });
            }

            if (request.Password.Length < 12)
            {
                return BadRequest(new
                {
                    message = "Password must be at least 12 characters long."
                });
            }

            var email = request.Email.Trim();

            var emailExists = await _context.Staff
                .AnyAsync(s => s.Email == email);

            if (emailExists)
            {
                return Conflict(new
                {
                    message = "A staff account with this email already exists."
                });
            }

            var role = request.Role?.Trim();

            if (role != "Administrator" && role != "Reviewer")
            {
                return BadRequest(new
                {
                    message = "Role must be Administrator or Reviewer."
                });
            }

            var staff = new Staff
            {
                FullName = request.FullName.Trim(),
                Email = email,
                Role = role,
                IsActive = true
            };

            staff.PasswordHash = _passwordHasher.HashPassword(
                staff,
                request.Password);

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAllStaff),
                new
                {
                    staff.StaffId,
                    staff.FullName,
                    staff.Email,
                    staff.Role,
                    staff.IsActive
                });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class CreateStaffRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Reviewer";
    }
}
