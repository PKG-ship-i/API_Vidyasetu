namespace Vidyasetu_API.DTOs.Response
{
	public class SignupResponseDto
	{
		public string Token { get; set; } = string.Empty;
		public UserDto UserDetails { get; set; } = new();
	}
}
