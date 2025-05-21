using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SportBookingWebAPI.Dtos;

namespace SportBookingWebAPI.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly EXE201_Rental_Sport_Field1Context _context;

		public AuthController(IConfiguration config, EXE201_Rental_Sport_Field1Context context)
		{
			_config = config;
			_context = context;
		}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email đã tồn tại.");
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = 3,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Tạo tài khoản thành công!");
        }
        [HttpPost("registerOwner")]
        public async Task<IActionResult> RegisterOwner([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email đã tồn tại.");
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = 2,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Tạo tài khoản thành công!");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Invalid request payload");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            var role = await _context.Roles.FindAsync(user.RoleId);
            var token = GenerateJwtToken(user.Email, role?.RoleName ?? "User");

            return Ok(new { token, roleId = user.RoleId, fullName = user.FullName,UserId = user.UserId }); // Trả về cả roleId
        }
        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                return Ok(new { isAuthenticated = true, message = "Đăng xuất" });
            }
            return Ok(new { isAuthenticated = false, message = "Đăng nhập" });
        }



        [Authorize]
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null) return NotFound("User not found");

			return Ok(new { user.FullName, user.Email, user.PhoneNumber, user.RoleId });
		}

		private string GenerateJwtToken(string email, string role)
		{
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
		new Claim(ClaimTypes.Email, email),
		new Claim(ClaimTypes.Role, role)
	};

			var token = new JwtSecurityToken(
				issuer: _config["JwtSettings:Issuer"],
				audience: _config["JwtSettings:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:ExpiryMinutes"])),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}


}