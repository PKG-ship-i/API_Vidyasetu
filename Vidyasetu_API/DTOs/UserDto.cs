namespace Vidyasetu_API.DTOs
{
	public class UserDto
	{
		public long Id { get; set; }
		public string Firstname { get; set; } = "";
		public string Lastname { get; set; } = "";
		public string Email { get; set; } = "";
		public string Role { get; set; } = "";
	}
}
