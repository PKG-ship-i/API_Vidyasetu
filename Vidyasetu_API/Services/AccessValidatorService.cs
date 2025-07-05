using Vidyasetu_API.Common;
using Vidyasetu_API.Models;

namespace Vidyasetu_API.Services
{
	public class AccessValidatorService : IAccessValidatorService
	{
		private readonly IConfiguration _config;
		private readonly ITokenValidator _tokenValidator;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AccessValidatorService(
			IConfiguration config,
			ITokenValidator tokenValidator,
			IHttpContextAccessor httpContextAccessor)
		{
			_config = config;
			_tokenValidator = tokenValidator;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<ApiResponse<string>?> ValidateTrialLimitAsync(DeviceDetail device, int logCount)
		{
			int allowedTries = Convert.ToInt32(_config["AllowedRequestCount"]);

			if (logCount < allowedTries)
				return null; // ✅ allowed without token

			if (device.UserId == null)
			{
				return ApiResponse<string>.CreateFailure(
					"You have exceeded the limit of free access. Please log in or sign up.", 302);
			}

			// ✅ Must validate token
			var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

			var principal = _tokenValidator.ValidateToken(authHeader);
			if (principal == null)
			{
				return ApiResponse<string>.CreateFailure("Missing or invalid token", 401);
			}

			var userIdFromToken = principal.FindFirst("id")?.Value;
			if (userIdFromToken != device.UserId.ToString())
			{
				return ApiResponse<string>.CreateFailure("Token user mismatch", 401);
			}

			return null; // ✅ All good
		}
	}
}
