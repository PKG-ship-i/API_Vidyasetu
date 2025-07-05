using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Vidyasetu_API.Services
{
	public class TokenValidator : ITokenValidator
	{
		private readonly IConfiguration _config;

		public TokenValidator(IConfiguration config)
		{
			_config = config;
		}

		public ClaimsPrincipal? ValidateToken(string? authHeader)
		{
			if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
				return null;

			var token = authHeader.Substring("Bearer ".Length).Trim();
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

			try
			{
				var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				}, out _);

				return principal;
			}
			catch
			{
				return null;
			}
		}
	}
}
