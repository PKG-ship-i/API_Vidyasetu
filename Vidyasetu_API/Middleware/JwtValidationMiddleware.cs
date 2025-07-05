using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Vidyasetu_API.Middleware
{
	public class JwtValidationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IConfiguration _config;

		public JwtValidationMiddleware(RequestDelegate next, IConfiguration config)
		{
			_next = next;
			_config = config;
		}

		public async Task Invoke(HttpContext context)
		{
			var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
			if (authHeader != null && authHeader.StartsWith("Bearer "))
			{
				var token = authHeader.Substring("Bearer ".Length);
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

				try
				{
					tokenHandler.ValidateToken(token, new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false,
						ClockSkew = TimeSpan.Zero
					}, out _);
					// token valid
				}
				catch
				{
					context.Response.StatusCode = 401;
					await context.Response.WriteAsync("Invalid or expired token");
					return;
				}
			}

			await _next(context); // pass to next
		}
	}
}
