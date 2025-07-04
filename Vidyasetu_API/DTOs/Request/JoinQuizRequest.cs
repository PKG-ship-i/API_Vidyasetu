namespace Vidyasetu_API.DTOs.Request
{
	public class JoinQuizRequest
	{
		public string Nickname { get; set; } = default!;
		public string DeviceId { get; set; } = default!;
	}
}
