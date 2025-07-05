using System.Security.Claims;

namespace Vidyasetu_API.Services
{
	public interface ITokenValidator
	{
		ClaimsPrincipal? ValidateToken(string? authHeader);
	}
}
