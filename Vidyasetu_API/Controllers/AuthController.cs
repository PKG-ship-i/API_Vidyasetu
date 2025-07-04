using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vidyasetu_API.DTOs;
using Vidyasetu_API.DTOs.Response;
using Vidyasetu_API.Models;
using Vidyasetu_API.Services;


namespace VidyasetuAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly VidyasetuAI_DevContext _db;
		private readonly AuthService _authService;

		public AuthController(VidyasetuAI_DevContext db, AuthService authService)
		{
			_db = db;
			_authService = authService;
		}

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            if (_db.Users.Any(u => u.Email == dto.Email || u.Mobile == dto.Mobile))
                return BadRequest(ApiResponse<string>.Fail("User already exists", 400));

            var user = new User
            {
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                CreatedDate = DateTime.UtcNow,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var existingDevice = await _db.DeviceDetails.FirstOrDefaultAsync(x => x.Id == dto.DeviceId);
            if (existingDevice == null)
                return BadRequest(ApiResponse<string>.Fail("Invalid device", 400));

            existingDevice.UserId = user.Id;
            _db.DeviceDetails.Update(existingDevice);
            await _db.SaveChangesAsync();

            var token = _authService.GenerateToken(user, existingDevice.Id);

            var result = new
            {
                Token = token,
                UserDetails = user
            };

            return Ok(ApiResponse<object>.Success(result, "Signup successful"));
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized(ApiResponse<string>.Fail("Invalid credentials", 401));

            var device = await _db.DeviceDetails.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (device == null)
                return Unauthorized(ApiResponse<string>.Fail("Unauthorized device", 401));

            var token = _authService.GenerateToken(user, device.Id);

            var result = new
            {
                Token = token,
                UserDetails = user
            };

            return Ok(ApiResponse<object>.Success(result, "Login successful"));
        }



        [HttpPost("device-register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto dto)
        {
            var existingDevice = await _db.DeviceDetails.FirstOrDefaultAsync(d =>
                d.DeviceIdentifier == dto.DeviceIdentifier ||
                d.DeviceToken == dto.DeviceToken);

            if (existingDevice != null)
                return Ok(ApiResponse<DeviceDetail>.Success(existingDevice, "Device already registered"));

            var device = new DeviceDetail
            {
                DeviceIdentifier = dto.DeviceIdentifier,
                DeviceToken = dto.DeviceToken
            };

            _db.DeviceDetails.Add(device);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<DeviceDetail>.Success(device, "Device registered successfully"));
        }




        [HttpGet("whoami")]
        [Authorize]
        public IActionResult WhoAmI()
		{
			var userId = User.FindFirst("userId")?.Value;
			var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value });

			return Ok(new
			{
				IsAuthenticated = User.Identity?.IsAuthenticated,
				Claims = claims
			});
		}
	}
}
