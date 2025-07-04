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
			if (_db.Users.Any(u => u.Email == dto.Email))
				return BadRequest("User already exists");

			var user = new User
			{
				//Id = Guid.NewGuid(),
				Email = dto.Email,
				Firstname = dto.Firstname,
				Lastname = dto.Lastname,
				CreatedDate = DateTime.UtcNow,
				Password = BCrypt.Net.BCrypt.HashPassword(dto.Password) // install BCrypt package
			};

			_db.Users.Add(user);
			await _db.SaveChangesAsync();

			//var token = _authService.GenerateJwtToken(user.Id);
			var token = _authService.GenerateToken(user.Email);

			return Ok(new { token });
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
			if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
				return Unauthorized("Invalid credentials");

			var token = _authService.GenerateToken(user.Email);
			return Ok(new { token });
		}


		[HttpGet("whoami")]
        [Authorize]
        public IActionResult WhoAmI()
		{
			var userId = User.FindFirst("userId")?.Value;
			var role = User.FindFirst(ClaimTypes.Role)?.Value;
			return Ok(new { userId, role });
		}
	}
}
