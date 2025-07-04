using Microsoft.AspNetCore.SignalR;
using Vidyasetu_API.DTOs.Request;
using Vidyasetu_API.Hubs;

namespace Vidyasetu_API.Services
{
	public class QuizService : IQuizService
	{
		private readonly IHubContext<LeaderboardHub> _hubContext;

		public QuizService(IHubContext<LeaderboardHub> hubContext)
		{
			_hubContext = hubContext;
		}

		public async Task<string> ShareQuizAsync(ShareQuizRequest request, long userId)
		{
			string shareCode = GenerateCode(); // e.g., random 6-digit code
											   // Save in DB `shared_quiz_sessions` (EF or Dapper)

			return shareCode;
		}

		public async Task SubmitAnswersAsync(string shareCode, SubmitQuizAnswersRequest request)
		{
			// 1. Load sessionId from shareCode
			// 2. Validate + store participant and answers
			// 3. Calculate score
			// 4. Save to `quiz_participants` and `participant_answers`
			// 5. Fetch updated leaderboard
			var leaderboard = await GetLeaderboardAsync(shareCode);

			// 6. Push update via SignalR
			await _hubContext.Clients.Group(shareCode)
				.SendAsync("UpdateLeaderboard", leaderboard);
		}

		public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(string shareCode)
		{
			// Query DB: Load participants from `quiz_participants`
			return new List<LeaderboardEntryDto>();
		}

		private string GenerateCode()
		{
			return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
		}
	}
}
