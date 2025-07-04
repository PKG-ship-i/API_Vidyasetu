using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vidyasetu_API.DTOs;
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
				return BadRequest("User already exists");

			var user = new User
			{
				//Id = Guid.NewGuid(),
				Email = dto.Email,
				Firstname = dto.Firstname,
				Lastname = dto.Lastname,
				CreatedDate = DateTime.UtcNow,
				Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // install BCrypt package
				Role = "User"
			};

			 _db.Users.Add(user);
			await _db.SaveChangesAsync();

			//var token = _authService.GenerateJwtToken(user.Id);

			var existingDevice = await _db.DeviceDetails.FirstOrDefaultAsync(x => x.Id == dto.DeviceId);
			existingDevice!.UserId = user.Id;

			var token = _authService.GenerateToken(user, existingDevice.Id);

			return Ok(new { token });
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
			if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
				return Unauthorized("Invalid credentials");

			var device = await _db.DeviceDetails.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
			if(device == null)
			return Unauthorized("Unauthorized Device");

			var token = _authService.GenerateToken(user,device.Id);
			return Ok(new { token });
		}


        [HttpPost("device-register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto dto)
        {
            var existingDevice = await _db.DeviceDetails.FirstOrDefaultAsync(d => d.DeviceIdentifier == dto.DeviceIdentifier || d.DeviceToken == dto.DeviceToken);

            if (existingDevice != null)

                return Ok(existingDevice);

            var device = new DeviceDetail

            {

                DeviceIdentifier = dto.DeviceIdentifier,

                DeviceToken = dto.DeviceToken

            };

            _db.DeviceDetails.Add(device);

            await _db.SaveChangesAsync();

            return Ok(device);

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
