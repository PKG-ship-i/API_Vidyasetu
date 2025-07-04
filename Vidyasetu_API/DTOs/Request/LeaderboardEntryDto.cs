namespace Vidyasetu_API.DTOs.Request
{
	public class LeaderboardEntryDto
	{
		public string Nickname { get; set; } = default!;
		public int Score { get; set; }
		public decimal Percentage { get; set; }
	}
}
